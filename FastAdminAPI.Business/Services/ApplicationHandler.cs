using DotNetCore.CAP;
using FastAdminAPI.Business.IServices;
using FastAdminAPI.Business.PrivateFunc.Applications;
using FastAdminAPI.Business.PrivateFunc.Applications.Models;
using FastAdminAPI.CAP.Subscribes;
using FastAdminAPI.Common.Attributes;
using FastAdminAPI.Common.BASE;
using FastAdminAPI.Common.Converters;
using FastAdminAPI.Common.Enums;
using FastAdminAPI.Common.Redis;
using FastAdminAPI.Framework.Entities;
using FastAdminAPI.Framework.Extensions;
using FastAdminAPI.Network.Interfaces;
using FastAdminAPI.Network.Models.QyWechat;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SqlSugar;
using SqlSugar.Attributes.Extension.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastAdminAPI.Business.Services
{
    /// <summary>
    /// 申请处理者
    /// </summary>
    public class ApplicationHandler : IApplicationHandler
    {
        /// <summary>
        /// 默认审批人
        /// </summary>
        private readonly long DEFAULT_APPROVER;
        /// <summary>
        /// 员工审批流程Rediskey缓存
        /// </summary>
        private readonly string APPROVAL_DATA_REDIS_KEY = "Applications:ApprovalInfo";
        /// <summary>
        /// 员工审批人员Rediskey缓存
        /// </summary>
        private readonly string APPROVERS_DATA_REDIS_KEY = "Applications:ApproversInfo";
        /// <summary>
        /// Redis锁前缀
        /// </summary>
        private readonly string REDIS_LOCK_PREFIX = "Lock:Application:Apply_";

        /// <summary>
        /// SugarScope
        /// </summary>
        private readonly SqlSugarScope _dbContext;
        /// <summary>
        /// Redis
        /// </summary>
        private readonly IRedisHelper _redis;
        /// <summary>
        /// 申请处理器
        /// </summary>
        private readonly IApplicationProcessor _processor;
        /// <summary>
        /// 事件总线
        /// </summary>
        private readonly ICapPublisher _capPublisher;
        /// <summary>
        /// 企业微信API
        /// </summary>
        private readonly IQyWechatApi _qyWechatApi;


        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="redis"></param>
        /// <param name="configuration"></param>
        /// <param name="dataPermission"></param>
        public ApplicationHandler(ISqlSugarClient dbContext, IRedisHelper redis, IConfiguration configuration,
            IApplicationProcessor processor, ICapPublisher capPublisher, IQyWechatApi qyWechatApi)
        {
            _dbContext = dbContext as SqlSugarScope;
            _redis = redis;
            _processor = processor;
            _capPublisher = capPublisher;
            _qyWechatApi = qyWechatApi;
            DEFAULT_APPROVER = configuration.GetValue<long>("Common.Applications.DefaultApprover");
        }

        #region 申请

        #region 内部方法

        #region 审批流程类型：直接上级

        #region 内部使用
        /// <summary>
        /// 按岗位递归寻找上级
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        private async Task<ApproverInfoModel> GetSuperiorByPost(long? postId)
        {
            ApproverInfoModel superior = null;
            if (postId != null)
            {
                //获取当前岗位的上级岗位
                var parentPost = await _dbContext.Queryable<S06_Post>()
                    .Where(S06 => S06.S06_PostId == postId && S06.S06_IsValid == 0)
                    .Select(S06 => S06.S06_ParentPostId).FirstAsync();
                if (parentPost != null)
                {
                    //获取上级主岗位最后创建的员工
                    superior = await _dbContext.Queryable<S08_EmployeePost>()
                        .LeftJoin<S07_Employee>((S08, S07) => S08.S07_EmployeeId == S07.S07_EmployeeId)
                        .Where((S08, S07) => S08.S08_IsValid == (byte)BaseEnums.IsValid.Valid &&
                                             S08.S08_IsMainPost == (byte)BaseEnums.TrueOrFalse.True && 
                                             S08.S06_PostId == parentPost)
                        .OrderBy((S08, S07) => S08.S08_CreateTime, OrderByType.Desc)
                        .Select((S08, S07) => new ApproverInfoModel
                        {
                            EmployeeId = S07.S07_EmployeeId,
                            EmployeeName = S07.S07_Name,
                            QyUserId = S07.S07_QyUserId
                        })
                        .FirstAsync();

                    //递归获取上级
                    superior ??= await GetSuperiorByPost((long)parentPost);
                }
            }
            return superior;
        }
        /// <summary>
        /// 按部门寻找最高上级
        /// </summary>
        /// <param name="operationId"></param>
        /// <param name="operationName"></param>
        /// <param name="operationDepartId"></param>
        /// <returns></returns>
        private async Task<ApproverInfoModel> GetSuperiorByDepart(long operationId, string operationName, long operationDepartId)
        {
            ApproverInfoModel superior = null;
            if (operationDepartId == 1) //如果当前部门为顶级部门，且前一步按岗位递归寻找上级未找到上级，说明该用户为最高上级
            {
                superior = new ApproverInfoModel
                {
                    EmployeeId = operationId,
                    EmployeeName = operationName,
                    QyUserId = await _dbContext.Queryable<S07_Employee>()
                            .Where(S07 => S07.S07_EmployeeId == operationId).Select(S07 => S07.S07_QyUserId).FirstAsync()
                };
            }
            else
            {
                //获取部门列表
                var departList = await _dbContext.Queryable<S05_Department>()
                    .Where(S05 => S05.S05_IsValid == 0 && S05.S05_DepartId != 1)
                    .Select(S05 => new S05_Department
                    {
                        S05_DepartId = S05.S05_DepartId,
                        S05_ParentDepartId = S05.S05_ParentDepartId
                    })
                    .ToListAsync();
                //获取岗位列表
                var postList = await _dbContext.Queryable<S06_Post>()
                    .Where(S06 => S06.S06_IsValid == 0)
                    .Select(S06 => new S06_Post
                    {
                        S06_PostId = S06.S06_PostId,
                        S06_ParentPostId = S06.S06_ParentPostId,
                        S05_DepartId = S06.S05_DepartId
                    })
                    .ToListAsync();
                //进行递归寻找上级部门领导
                superior = await GetDepartSuperiorRecursive(departList, postList, operationDepartId, operationDepartId);
            }
            return superior;
        }
        /// <summary>
        /// 递归寻找上级部门领导
        /// </summary>
        /// <param name="departList">部门列表</param>
        /// <param name="postList">岗位列表</param>
        /// <param name="departId">递归部门Id</param>
        /// <param name="originDepartId">原始部门Id</param>
        /// <returns></returns>
        private async Task<ApproverInfoModel> GetDepartSuperiorRecursive(List<S05_Department> departList, List<S06_Post> postList, long departId, long originDepartId)
        {
            //上级
            ApproverInfoModel superior = null;
            //查询递归中当前部门(非员工原始部门)的父级部门Id
            var parentDepartId = departList.Where(S05 => S05.S05_DepartId == departId)
                .Select(S05 => S05.S05_ParentDepartId).FirstOrDefault();

            //判断是否有父级部门，以用作判断部门树的位置
            //如果递归中的当前部门(非员工原始部门)有父级部门，开始寻找最高岗位
            if (parentDepartId > 0)
            {
                var superPostId = postList.Where(S06 => S06.S05_DepartId == parentDepartId && S06.S06_ParentPostId == null)
                .Select(S06 => S06.S06_PostId).FirstOrDefault();
                if (superPostId > 0)
                {
                    //获取上级主岗位最后创建的员工
                    superior = await _dbContext.Queryable<S08_EmployeePost>()
                        .LeftJoin<S07_Employee>((S08, S07) => S08.S07_EmployeeId == S07.S07_EmployeeId)
                        .Where((S08, S07) => S08.S08_IsValid == (byte)BaseEnums.IsValid.Valid &&
                                             S08.S08_IsMainPost == (byte)BaseEnums.TrueOrFalse.True && 
                                             S08.S06_PostId == superPostId)
                        .OrderBy((S08, S07) => S08.S08_CreateTime, OrderByType.Desc)
                        .Select((S08, S07) => new ApproverInfoModel
                        {
                            EmployeeId = S07.S07_EmployeeId,
                            EmployeeName = S07.S07_Name,
                            QyUserId = S07.S07_QyUserId
                        })
                        .FirstAsync();
                }

                //如果上级为空，接着递归寻找
                superior ??= await GetDepartSuperiorRecursive(departList, postList, parentDepartId, originDepartId);

            }

            return superior;
        }
        #endregion

        /// <summary>
        /// 获取直接上级
        /// </summary>
        /// <param name="operationId">操作人Id</param>
        /// <param name="operationName">操作人名称</param>
        /// <returns></returns>
        /// <exception cref="UserOperationException"></exception>
        private async Task<ApproverInfoModel> GetSuperiorApprover(long operationId, string operationName)
        {
            ApproverInfoModel superior = null;

            //获取当前操作人的主岗位
            var mainPost = await _dbContext.Queryable<S08_EmployeePost>()
                .Where(S08 => S08.S08_IsValid == (byte)BaseEnums.IsValid.Valid &&
                              S08.S08_IsMainPost == (byte)BaseEnums.TrueOrFalse.True && 
                              S08.S07_EmployeeId == operationId)
                .Select(S08 => new { PostId = S08.S06_PostId, DepartId = S08.S05_DepartId }).FirstAsync();

            //递归按岗位寻找上级
            superior = await GetSuperiorByPost(mainPost.PostId);
            //递归按部门寻找最高上级
            superior ??= await GetSuperiorByDepart(operationId, operationName, mainPost.DepartId);

            //获取默认审批人
            superior ??= await GetDefaultApprover();

            //返回最终结果
            if (superior != null)
            {
                superior.Priority = 1;
                return superior;
            }
            else
                throw new UserOperationException("获取审批人失败[1]!");
        }
        #endregion

        #region 审批流程类型：指定审批人
        /// <summary>
        /// 获取指定审批人
        /// </summary>
        /// <param name="approverIds">审批人Ids</param>
        /// <returns></returns>
        /// <exception cref="UserOperationException"></exception>
        private async Task<List<ApproverInfoModel>> GetDesigneeApprovers(List<long> approverIds)
        {
            if (approverIds?.Count > 0)
            {
                //查询设置的审批人
                var approvers = await _dbContext.Queryable<S07_Employee>()
                    .Where(S07 => S07.S07_IsValid == (byte)BaseEnums.IsValid.Valid && 
                                  approverIds.Contains(S07.S07_EmployeeId))
                    .Select(S07 => new
                    {
                        EmployeeId = S07.S07_EmployeeId,
                        EmployeeName = S07.S07_Name,
                        QyUserId = S07.S07_QyUserId
                    })
                    .ToListAsync();
                //默认审批人
                var defaultApprover = await GetDefaultApprover();
                int priority = 0;
                List<ApproverInfoModel> approverList = new();
                approverIds.ForEach(item =>
                {
                    var approver = approvers?.Where(c => c.EmployeeId == item).FirstOrDefault();
                    //判断审批人是否存在，不存在使用默认审批人
                    if (approver != null)
                    {
                        approverList.Add(new ApproverInfoModel
                        {
                            EmployeeId = approver.EmployeeId,
                            EmployeeName = approver.EmployeeName,
                            QyUserId = approver.QyUserId,
                            Priority = ++priority
                        });
                    }
                    else
                    {
                        approverList.Add(new ApproverInfoModel
                        {
                            EmployeeId = defaultApprover.EmployeeId,
                            EmployeeName = defaultApprover.EmployeeName,
                            QyUserId = defaultApprover.QyUserId,
                            Priority = ++priority
                        });
                    }
                });
                if (approverList?.Count > 0)
                    return approverList;
                else
                    throw new UserOperationException("没有可用的审批人!");
            }
            else
                throw new UserOperationException("获取审批人失败[2]!");

        }
        #endregion

        #region 审批流程类型：自选
        /// <summary>
        /// 获取自选审批人
        /// </summary>
        /// <param name="approverIds">审批人Ids</param>
        /// <returns></returns>
        /// <exception cref="UserOperationException"></exception>
        private async Task<List<ApproverInfoModel>> GetCustomizeApprovers(List<long> approverIds)
        {
            if (approverIds?.Count > 0)
            {
                //查询自选的审批人
                var approvers = await _dbContext.Queryable<S07_Employee>()
                    .Where(S07 => S07.S07_IsValid == (byte)BaseEnums.IsValid.Valid && 
                                  approverIds.Contains(S07.S07_EmployeeId))
                    .Select(S07 => new
                    {
                        EmployeeId = S07.S07_EmployeeId,
                        EmployeeName = S07.S07_Name,
                        QyUserId = S07.S07_QyUserId
                    })
                    .ToListAsync();
                //默认审批人
                var defaultApprover = await GetDefaultApprover();
                int priority = 0;
                List<ApproverInfoModel> approverList = new();
                approverIds.ForEach(item =>
                {
                    var approver = approvers?.Where(c => c.EmployeeId == item).FirstOrDefault();
                    //判断审批人是否存在，不存在使用默认审批人
                    if (approver != null)
                    {
                        approverList.Add(new ApproverInfoModel
                        {
                            EmployeeId = approver.EmployeeId,
                            EmployeeName = approver.EmployeeName,
                            QyUserId = approver.QyUserId,
                            Priority = ++priority
                        });
                    }
                    else
                    {
                        approverList.Add(new ApproverInfoModel
                        {
                            EmployeeId = defaultApprover.EmployeeId,
                            EmployeeName = defaultApprover.EmployeeName,
                            QyUserId = defaultApprover.QyUserId,
                            Priority = ++priority
                        });
                    }
                });
                if (approverList?.Count > 0)
                    return approverList;
                else
                    throw new UserOperationException("没有可用的审批人!");
            }
            else
                throw new UserOperationException("获取审批人失败[3]!");
        }
        #endregion

        /// <summary>
        /// 获取默认审批人
        /// </summary>
        /// <returns></returns>
        private async Task<ApproverInfoModel> GetDefaultApprover()
        {
            var defaultApprover = await _dbContext.Queryable<S07_Employee>()
                        .Where(S07 => S07.S07_IsValid == (byte)BaseEnums.IsValid.Valid && 
                                      S07.S07_EmployeeId == DEFAULT_APPROVER)
                        .Select(S07 => new ApproverInfoModel
                        {
                            EmployeeId = S07.S07_EmployeeId,
                            EmployeeName = S07.S07_Name,
                            QyUserId = S07.S07_QyUserId
                        }).FirstAsync();
            return defaultApprover ?? throw new UserOperationException("默认审批人不存在，请先配置!");
        }
        /// <summary>
        /// 获取抄送人
        /// </summary>
        /// <param name="process">审批流程</param>
        /// <param name="extenalCarbonCopiesIds">指定抄送人</param>
        /// <returns></returns>
        private async Task<List<CarbonCopiesInfoModel>> GetCarbonCopies(S11_CheckProcess process, List<long> extenalCarbonCopiesIds = null)
        {
            List<CarbonCopiesInfoModel> carbonCopiesList = null;
            List<long> carbonCopiesIds = null;
            if (process != null)
            {
                carbonCopiesIds = process.S07_CarbonCopies?.Split(",").Select(x => Convert.ToInt64(x)).ToList();
            }
            else if (extenalCarbonCopiesIds?.Count > 0)
            {
                carbonCopiesIds = extenalCarbonCopiesIds;
            }
            if (carbonCopiesIds?.Count > 0)
            {
                //查询设置的审批人
                var ccReceivers = await _dbContext.Queryable<S07_Employee>()
                    .Where(S07 => S07.S07_IsValid == (byte)BaseEnums.IsValid.Valid && 
                                  carbonCopiesIds.Contains(S07.S07_EmployeeId))
                    .Select(S07 => new CarbonCopiesInfoModel
                    {
                        EmployeeId = S07.S07_EmployeeId,
                        EmployeeName = S07.S07_Name,
                        QyUserId = S07.S07_QyUserId
                    })
                    .ToListAsync();
                carbonCopiesList = new List<CarbonCopiesInfoModel>();
                carbonCopiesIds.ForEach(item =>
                {
                    var ccReceiver = ccReceivers?.Where(c => c.EmployeeId == item).FirstOrDefault();
                    if (ccReceiver != null)
                    {
                        carbonCopiesList.Add(new CarbonCopiesInfoModel
                        {
                            EmployeeId = ccReceiver.EmployeeId,
                            EmployeeName = ccReceiver.EmployeeName,
                            QyUserId = ccReceiver.QyUserId
                        });
                    }
                });
            }
            return carbonCopiesList;
        }
        /// <summary>
        /// 获取审批流程
        /// </summary>
        /// <param name="checkProcessType"></param>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        private async Task<S11_CheckProcess> GetApproveProcess(long checkProcessType, long employeeId)
        {
            //创建redis键
            string key = $"{APPROVAL_DATA_REDIS_KEY}_{checkProcessType}_{employeeId}";
            //从缓存中获取
            if (await _redis.KeyExistsAsync(key))
                return await _redis.StringGetAsync<S11_CheckProcess>(key);

            //审批流程初始化
            S11_CheckProcess process = null;

            //查询当前审批类型的所有审批流程
            var processList = await _dbContext.Queryable<S11_CheckProcess>()
                .Where(Z04 => Z04.S11_IsValid == (byte)BaseEnums.IsValid.Valid && 
                              Z04.S99_ApplicationType == checkProcessType)
                //.Where(Z04 => DbExtension.FindInSetWhere(employeeId.ToString(), Z04.S07_Applicants))
                .ToListAsync();
            if (processList?.Count > 0) //循环查看是否有当前申请人
            {
                foreach (var item in processList)
                {
                    if (!string.IsNullOrEmpty(item.S07_Applicants))
                    {
                        var applicants = item.S07_Applicants.Split(",").Select(c => Convert.ToInt64(c)).ToList();
                        if (applicants.Count > 0 && applicants.Contains(employeeId))
                        {
                            process = item;
                            break;
                        }
                    }
                }
            }

            //如果依然没有审批流程，寻找默认审批流程
            if (process == null)
            {
                process = await _dbContext.Queryable<S11_CheckProcess>()
                    .Where(Z04 => Z04.S11_IsValid == (byte)BaseEnums.IsValid.Valid && 
                                  Z04.S99_ApplicationType == checkProcessType &&
                                  (Z04.S07_Applicants == null || Z04.S07_Applicants == ""))
                    .FirstAsync();
                if (process == null)
                    throw new UserOperationException("请先设置审批流程!");
            }
            //审批流程存入redis中
            await _redis.StringSetAsync(key, process, TimeSpan.FromSeconds(30));
            return await _redis.StringGetAsync<S11_CheckProcess>(key);

        }
        /// <summary>
        /// 获取审批人员
        /// </summary>
        /// <param name="check"></param>
        /// <param name="model"></param>
        /// <param name="process"></param>
        /// <returns></returns>
        private async Task<ApprovalDataByRedisModel> GetApprovers(SetApplicationModel model, S11_CheckProcess process)
        {
            //创建redis键
            string key = $"{APPROVERS_DATA_REDIS_KEY}_{process.S99_ApplicationType}_{model.OperationId}";
            //从缓存中获取
            if (await _redis.KeyExistsAsync(key))
                return await _redis.StringGetAsync<ApprovalDataByRedisModel>(key);

            //查询审批人数据
            ApprovalDataByRedisModel approversData = new();

            #region 上级
            if (process.S11_ApproveType == (byte)ApplicationEnums.ApproveType.Superior)
            {
                //获取上级审批人
                var superior = await GetSuperiorApprover((long)model.OperationId, model.OperationName);
                var approverList = new List<ApproverInfoModel> { superior };

                //打包数据
                approversData.ApproverId = (long)superior.EmployeeId;
                approversData.ApproverName = superior.EmployeeName;
                approversData.QyUserId = superior.QyUserId;
                approversData.ApproversData = JsonConvert.SerializeObject(approverList);
            }
            #endregion

            #region 指定审批人
            else if (process.S11_ApproveType == (byte)ApplicationEnums.ApproveType.Designee)
            {
                //获取指定审批人
                List<long> approveIds = process.S07_Approvers?.Split(",").Select(x => Convert.ToInt64(x)).ToList();
                var approverList = await GetDesigneeApprovers(approveIds);

                //打包数据
                approversData.ApproverId = (long)approverList[0].EmployeeId;
                approversData.ApproverName = approverList[0].EmployeeName;
                approversData.QyUserId = approverList[0].QyUserId;
                approversData.ApproversData = JsonConvert.SerializeObject(approverList);
            }
            #endregion

            #region 自选审批人
            else if (process.S11_ApproveType == (byte)ApplicationEnums.ApproveType.Customize)
            {
                //获取指定审批人
                var approverList = await GetCustomizeApprovers(model.ApproverList);

                //打包数据
                approversData.ApproverId = (long)approverList[0].EmployeeId;
                approversData.ApproverName = approverList[0].EmployeeName;
                approversData.QyUserId = approverList[0].QyUserId;
                approversData.ApproversData = JsonConvert.SerializeObject(approverList);
            }
            #endregion

            #region 上级+指定审批人
            else if (process.S11_ApproveType == (byte)ApplicationEnums.ApproveType.SuperiorAndDesignee)
            {
                //审批人数据
                List<ApproverInfoModel> approverList = new();

                //获取上级审批人
                var superior = await GetSuperiorApprover((long)model.OperationId, model.OperationName);
                approverList.Add(superior);

                //获取指定审批人
                List<long> approveIds = process.S07_Approvers?.Split(",").Select(x => Convert.ToInt64(x)).ToList();
                approverList.AddRange(await GetDesigneeApprovers(approveIds));

                //对审批人重新进行排序
                int priority = 0;
                approverList.ForEach(item => { item.Priority = ++priority; });

                //打包数据
                approversData.ApproverId = (long)superior.EmployeeId;
                approversData.ApproverName = superior.EmployeeName;
                approversData.QyUserId = superior.QyUserId;
                approversData.ApproversData = JsonConvert.SerializeObject(approverList);
            }
            #endregion

            #region 其他 => exception
            else
                throw new UserOperationException("审批流程类型不存在!");
            #endregion

            //抄送人
            var ccReceivers = await GetCarbonCopies(process);
            if (ccReceivers?.Count > 0)
                approversData.CarbonCopiesData = JsonConvert.SerializeObject(ccReceivers);

            //审批人存入redis中
            await _redis.StringSetAsync(key, approversData, TimeSpan.FromSeconds(30));
            return await _redis.StringGetAsync<ApprovalDataByRedisModel>(key);
        }
        #endregion

        /// <summary>
        /// 设置申请
        /// 所需条件：1.开启事务 2.是否需要消息通知
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isSendApprovalNotice">是否开启审批通知(默认是)</param>
        /// <returns></returns>
        public async Task<ResponseModel> SetApplication(SetApplicationModel model, bool isSendApprovalNotice = true)
        {
            #region Redis锁
            //redis锁名 = 前缀_申请类别_业务数据Id
            string lockName = REDIS_LOCK_PREFIX + model.ApplicationCategory + "_" + model.ApplicationId;
            //redis锁令牌
            string token = GuidConverter.GenerateShortGuid();

            //如果获取不到锁，就返回失败
            if (!await _redis.GetLockAsync(lockName, token))
            {
                return ResponseModel.Error("请求频繁，请稍后再试!");
            }
            #endregion

            try
            {
                //校验是否已存在未完成审批的申请
                bool isExist = await _dbContext.Queryable<S12_Check>()
                    .Where(S12 => S12.S12_IsValid == (byte)BaseEnums.IsValid.Valid && S12.S12_IsFinishCheck == (byte)BaseEnums.IsFinish.Unfinish &&
                                  S12.S12_ApplicationCategory == model.ApplicationCategory &&
                                  //S12.S99_ApplicationType == model.ApplicationType &&
                                  S12.S12_ApplicationId == model.ApplicationId)
                    .AnyAsync();
                if (isExist)
                    throw new UserOperationException($"该数据已存在申请!");

                //获取审批流程
                var process = await GetApproveProcess((long)model.ApplicationType, (long)model.OperationId);

                //获取审批人
                var approversData = await GetApprovers(model, process);

                S12_Check check = new()
                {
                    S12_ApplicationId = (long)model.ApplicationId,
                    S11_CheckProcessId = process.S11_CheckProcessId,
                    S12_ApplicationCategory = (byte)model.ApplicationCategory,
                    S99_ApplicationType = (long)model.ApplicationType,
                    S12_PrivateDataContent = model.DataContent,
                    S12_ApplicationInfo = model.Description,
                    S12_Reason = model.Reason,
                    S12_IsFinishCheck = (byte)BaseEnums.IsFinish.Unfinish,
                    S12_IsValid = (byte)BaseEnums.IsValid.Valid,
                    S12_CreateId = (long)model.OperationId,
                    S12_CreateBy = model.OperationName,
                    S12_CreateTime = _dbContext.GetDate(),
                    S07_ApproverId = approversData.ApproverId,
                    S12_ApproverName = approversData.ApproverName,
                    S12_ApproversData = approversData.ApproversData,
                    S12_CarbonCopiesData = approversData.CarbonCopiesData,
                    //打包公有数据
                    S12_CommonDataContent = model.CommonDataContent == null ? null : JsonConvert.SerializeObject(model.CommonDataContent)
                };
                //插入数据库
                var result = await _dbContext.Insertable(check).ExecuteAsync();

                //为审批人发送消息通知
                if (result?.Code == ResponseCode.Success && isSendApprovalNotice && !string.IsNullOrEmpty(approversData.QyUserId))
                {
                    long checkId = Convert.ToInt64(result.Data);

                    //发送企业微信消息通知
                    await SendQyWechatNotice(checkId, (int)model.ApplicationCategory, check.S07_ApproverId, approversData.QyUserId,
                        model.CommonDataContent?.QyWechatNotifyTitle, model.CommonDataContent?.QyWechatNotifyDescription, model.CommonDataContent?.QyWechatNotifyUrl);

                    //发送系统消息通知
                    await SendSystemNotice(checkId, check.S07_ApproverId, model.CommonDataContent?.QyWechatNotifyDescription);

                    result.Data = checkId;
                }

                return result;
            }
            catch (UserOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new UserOperationException($"操作失败，{ex.Message}");
            }
            finally
            {
                //如果锁存在
                if (!string.IsNullOrEmpty(lockName) && !string.IsNullOrEmpty(token))
                {
                    //释放Redis锁
                    await _redis.ReleaseLockAsync(lockName, token);
                }
            }
        }
        #endregion

        #region 审批

        #region 内部方法
        /// <summary>
        /// 下一审批
        /// </summary>
        /// <param name="model"></param>
        /// <param name="approvers">审批人列表</param>
        /// <param name="isSendApprovalNotice">是否开启通知</param>
        /// <returns></returns>
        private async Task<ResponseModel> Next(ProcessingApplicationModel model, List<ApproverInfoModel> approvers, bool isSendApprovalNotice)
        {
            var current = approvers.Where(c => c.EmployeeId == model.Check.S07_ApproverId && !c.IsFinishApproved).FirstOrDefault();
            var next = approvers.Where(c => c.Priority == current.Priority + 1 && !c.IsFinishApproved).FirstOrDefault();
            if (next != null)
            {
                //公有数据
                ApplicationCommonDataModel commonData = null;
                if (!string.IsNullOrEmpty(model.Check.S12_CommonDataContent))
                    commonData = JsonConvert.DeserializeObject<ApplicationCommonDataModel>(model.Check.S12_CommonDataContent);

                //当前审批完成
                current.IsFinishApproved = true;
                //重新打包成json
                string approversStr = JsonConvert.SerializeObject(approvers);

                //数据库更新
                var result = await _dbContext.Updateable<S12_Check>()
                    .SetColumns(S12 => S12.S07_ApproverId == next.EmployeeId)
                    .SetColumns(S12 => S12.S12_ApproverName == next.EmployeeName)
                    .SetColumns(S12 => S12.S12_ApproversData == approversStr)
                    .SetColumns(S12 => S12.S12_ModifyId == model.OperationId)
                    .SetColumns(S12 => S12.S12_ModifyBy == model.OperationName)
                    .SetColumns(S12 => S12.S12_ModifyTime == SqlFunc.GetDate())
                    .Where(S12 => S12.S12_CheckId == model.Check.S12_CheckId)
                    .ExecuteAsync();


                //为审批人发送消息通知
                if (result?.Code == ResponseCode.Success && isSendApprovalNotice && !string.IsNullOrEmpty(next.QyUserId))
                {
                    //发送企业微信消息通知
                    await SendQyWechatNotice(model.Check.S12_CheckId, model.Check.S12_ApplicationCategory, (long)next.EmployeeId, next.QyUserId,
                        commonData?.QyWechatNotifyTitle, commonData?.QyWechatNotifyDescription, commonData?.QyWechatNotifyUrl);

                    //发送系统消息通知
                    await SendSystemNotice(model.Check.S12_CheckId, (long)next.EmployeeId, commonData?.QyWechatNotifyDescription);
                }

                return result;
            }
            else
                throw new UserOperationException("获取下一审批人失败!");

        }
        /// <summary>
        /// 通过申请
        /// </summary>
        /// <param name="model"></param>
        /// <param name="approvers">审批人列表</param>
        /// <param name="isSendApprovalCCNotice">是否开启审批抄送通知</param>
        /// <param name="ccReceivers">抄送人</param>
        /// <returns></returns>
        private async Task<ResponseModel> AcceptApplication(ProcessingApplicationModel model, List<ApproverInfoModel> approvers, bool isSendApprovalCCNotice, List<CarbonCopiesInfoModel> ccReceivers = null)
        {
            var current = approvers.Where(c => c.EmployeeId == model.Check.S07_ApproverId && !c.IsFinishApproved).FirstOrDefault();
            //当前审批完成
            current.IsFinishApproved = true;
            //重新打包成json
            string approversStr = JsonConvert.SerializeObject(approvers);

            //数据库更新
            var result = await _dbContext.Updateable<S12_Check>()
                .SetColumns(S12 => S12.S12_IsFinishCheck == (byte)BaseEnums.IsFinish.Finished)
                .SetColumns(S12 => S12.S12_ApproversData == approversStr)
                .SetColumns(S12 => S12.S12_ModifyId == model.OperationId)
                .SetColumns(S12 => S12.S12_ModifyBy == model.OperationName)
                .SetColumns(S12 => S12.S12_ModifyTime == SqlFunc.GetDate())
                .Where(S12 => S12.S12_CheckId == model.Check.S12_CheckId)
                .ExecuteAsync();


            if (result?.Code == ResponseCode.Success)
            {
                //打包完成申请所需数据
                CompleteApplicationModel data = new()
                {
                    ApplicationCategory = model.Check.S12_ApplicationCategory,
                    ApplicationType = model.Check.S99_ApplicationType,
                    DataContent = model.Check.S12_PrivateDataContent,
                    ApplicationId = model.Check.S12_CheckId
                };

                //处理申请
                result = await _processor.CompleteApplication(model.Check.S12_ApplicationCategory, model.Check.S99_ApplicationType, data);

                //为抄送人发送消息通知
                if (result?.Code == ResponseCode.Success && isSendApprovalCCNotice && ccReceivers?.Count > 0)
                {
                    //公有数据
                    ApplicationCommonDataModel commonData = null;
                    if (!string.IsNullOrEmpty(model.Check.S12_CommonDataContent))
                        commonData = JsonConvert.DeserializeObject<ApplicationCommonDataModel>(model.Check.S12_CommonDataContent);

                    //发送企业微信消息通知
                    await SendBatchQyWechatNotice(model.Check.S12_CheckId, model.Check.S12_ApplicationCategory, ccReceivers,
                        commonData?.QyWechatNotifyTitle, commonData?.QyWechatNotifyDescription, commonData?.QyWechatNotifyUrl);

                    //发送系统消息通知
                    await SendBatchSystemNotice(model.Check.S12_CheckId, ccReceivers.Select(a => (long)a.EmployeeId).ToList(), commonData?.QyWechatNotifyDescription);
                }

            }
            return result;
        }
        /// <summary>
        /// 拒绝申请
        /// </summary>
        /// <param name="model"></param>
        /// <param name="approvers">审批人列表</param>
        /// <param name="isSendApprovalCCNotice">是否开启企业微信审批抄送通知</param>
        /// <param name="ccReceivers">抄送人</param>
        /// <returns></returns>
        private async Task<ResponseModel> RejectApplication(ProcessingApplicationModel model, List<ApproverInfoModel> approvers, bool isSendApprovalCCNotice, List<CarbonCopiesInfoModel> ccReceivers = null)
        {
            var current = approvers.Where(c => c.EmployeeId == model.Check.S07_ApproverId && !c.IsFinishApproved).FirstOrDefault();
            current.IsFinishApproved = true;
            string approversStr = JsonConvert.SerializeObject(approvers);

            //数据库更新
            var result = await _dbContext.Updateable<S12_Check>()
                .SetColumns(S12 => S12.S12_IsFinishCheck == (byte)BaseEnums.IsFinish.Finished)
                .SetColumns(S12 => S12.S12_ApproversData == approversStr)
                .SetColumns(S12 => S12.S12_ModifyId == model.OperationId)
                .SetColumns(S12 => S12.S12_ModifyBy == model.OperationName)
                .SetColumns(S12 => S12.S12_ModifyTime == SqlFunc.GetDate())
                .Where(S12 => S12.S12_CheckId == model.Check.S12_CheckId)
                .ExecuteAsync();

            if (result?.Code == ResponseCode.Success)
            {
                //打包完成申请所需数据
                CompleteApplicationModel data = new()
                {
                    ApplicationCategory = model.Check.S12_ApplicationCategory,
                    ApplicationType = model.Check.S99_ApplicationType,
                    DataContent = model.Check.S12_PrivateDataContent,
                    ApplicationId = model.Check.S12_CheckId
                };

                //处理申请
                result = await _processor.RejectApplication(model.Check.S12_ApplicationCategory, model.Check.S99_ApplicationType, data);


                //为抄送人发送消息通知
                if (result?.Code == ResponseCode.Success && isSendApprovalCCNotice && ccReceivers?.Count > 0)
                {
                    //公有数据
                    ApplicationCommonDataModel commonData = null;
                    if (!string.IsNullOrEmpty(model.Check.S12_CommonDataContent))
                        commonData = JsonConvert.DeserializeObject<ApplicationCommonDataModel>(model.Check.S12_CommonDataContent);

                    //发送企业微信消息通知
                    await SendBatchQyWechatNotice(model.Check.S12_CheckId, model.Check.S12_ApplicationCategory, ccReceivers,
                        commonData?.QyWechatNotifyTitle, commonData?.QyWechatNotifyDescription, commonData?.QyWechatNotifyUrl);

                    //发送系统消息通知
                    //await SystemMultipleMessageNotification(model.Check.S12_CheckId, ccReceivers.Select(a => (long)a.EmployeeId).ToList(), commonData?.QyWechatNotifyDescription);
                }

            }
            return result;
        }
        #endregion

        /// <summary>
        /// 审批
        /// 所需条件：1.开启事务 2.是否需要消息通知
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isSendApporvalNotice">是否开启审批通知(默认是)</param>
        /// <param name="isSendApprovalCCNotice">是否开启审批抄送通知(默认是)</param>
        /// <returns></returns>
        public async Task<ResponseModel> ProcessingApplication(ProcessingApplicationModel model, bool isSendApporvalNotice = true, bool isSendApprovalCCNotice = true)
        {
            S13_CheckRecords record = new()
            {
                S12_CheckId = model.Check.S12_CheckId,
                S07_ApproverId = model.Check.S07_ApproverId,
                S13_ApproverName = model.Check.S12_ApproverName,
                S13_ApprovalTime = _dbContext.GetDate(),
                S13_IsApprove = (byte)model.IsApprove,
                S13_Reason = model.Reason,
                S13_CreateId = (long)model.OperationId,
                S13_CreateBy = model.OperationName,
                S13_CreateTime = _dbContext.GetDate()
            };
            var result = await _dbContext.Insertable(record).ExecuteAsync();
            if (result?.Code == ResponseCode.Success)
            {
                //审批人
                if (string.IsNullOrEmpty(model.Check.S12_ApproversData)) throw new UserOperationException("审批设置错误[1]!");
                List<ApproverInfoModel> approvers = JsonConvert.DeserializeObject<List<ApproverInfoModel>>(model.Check.S12_ApproversData)
                    ?? throw new UserOperationException("审批设置错误[2]!");

                //抄送人
                List<CarbonCopiesInfoModel> ccReceivers = null;
                if (!string.IsNullOrEmpty(model.Check.S12_CarbonCopiesData))
                    ccReceivers = JsonConvert.DeserializeObject<List<CarbonCopiesInfoModel>>(model.Check.S12_CarbonCopiesData);


                //审批是否通过
                if (model.IsApprove == (int)ApplicationEnums.IsApprove.Approved)
                {
                    //审批人只有一个时，直接通过
                    if (approvers.Count == 1)
                        result = await AcceptApplication(model, approvers, isSendApprovalCCNotice, ccReceivers);
                    //审批人多个时，判断是否为最后审批人
                    else
                    {
                        var current = approvers.Where(c => c.EmployeeId == model.Check.S07_ApproverId && !c.IsFinishApproved).FirstOrDefault();
                        if (current != null)
                        {
                            //如果当前审批优先级为最后一个，说明审批结束，通过审批
                            if (current.Priority == approvers.Count)
                                result = await AcceptApplication(model, approvers, isSendApprovalCCNotice, ccReceivers);
                            //如果当前审批优先级不为最后一个，继续审批
                            else if (current.Priority < approvers.Count)
                                result = await Next(model, approvers, isSendApporvalNotice);
                            else
                                throw new UserOperationException("审批设置错误[2]!");
                        }
                        else
                            throw new UserOperationException("审批设置错误[3]!");
                    }
                }
                else
                    result = await RejectApplication(model, approvers, isSendApprovalCCNotice, ccReceivers);
            }
            return result;
        }

        #endregion

        #region 申请且通过
        /// <summary>
        /// 设置申请并通过
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AcceptApplication(SetApplicationModel model)
        {
            //获取审批流程
            S12_Check check = new()
            {
                S12_ApplicationId = (long)model.ApplicationId,
                S11_CheckProcessId = -1,//系统通过的审批，审批流程Id标记为-1
                S12_ApplicationCategory = (byte)model.ApplicationCategory,
                S99_ApplicationType = (long)model.ApplicationType,
                S07_ApproverId = -1, //系统审批
                S12_ApproverName = "系统",
                S12_ApproversData = null,
                S12_CarbonCopiesData = null,
                S12_PrivateDataContent = model.DataContent,
                S12_IsFinishCheck = (byte)BaseEnums.IsFinish.Finished,//已完成
                S12_ApplicationInfo = model.Description,
                S12_Reason = model.Reason,
                S12_IsValid = (byte)BaseEnums.IsValid.Valid,
                S12_CreateId = (long)model.OperationId,
                S12_CreateBy = model.OperationName,
                S12_CreateTime = _dbContext.GetDate(),
                //打包公有数据
                S12_CommonDataContent = model.CommonDataContent == null ? null : JsonConvert.SerializeObject(model.CommonDataContent)
            };
            //插入数据库
            var result = await _dbContext.Insertable(check).ExecuteAsync();
            if (result?.Code == ResponseCode.Success)
            {
                var checkId = Convert.ToInt64(result.Data);

                S13_CheckRecords record = new()
                {
                    S12_CheckId = checkId,
                    S07_ApproverId = -1, //系统审批
                    S13_ApproverName = "系统",
                    S13_ApprovalTime = _dbContext.GetDate(),
                    S13_IsApprove = (byte)BaseEnums.IsFinish.Finished,
                    S13_Reason = "系统自动审批通过",
                    S13_CreateId = (long)model.OperationId,
                    S13_CreateBy = model.OperationName,
                    S13_CreateTime = _dbContext.GetDate()
                };
                result = await _dbContext.Insertable(record).ExecuteAsync();
                if (result?.Code == ResponseCode.Success)
                {
                    //打包完成申请所需数据
                    CompleteApplicationModel data = new()
                    {
                        ApplicationCategory = model.ApplicationCategory,
                        ApplicationType = model.ApplicationType,
                        DataContent = model.DataContent,
                        ApplicationId = (long)model.ApplicationId
                    };

                    //处理申请
                    result = await _processor.CompleteApplication((byte)model.ApplicationCategory, (long)model.ApplicationType, data);
                }
            }
            return result;
        }
        #endregion

        #region 企业微信通知
        /// <summary>
        /// 发送企业微信通知(生产环境可用)
        /// </summary>
        /// <param name="checkId">审批Id</param>
        /// <param name="applyCategory">申请类别</param>
        /// <param name="approverId">审批人Id</param>
        /// <param name="qyUserId">审批人企业微信UserId</param>
        /// <param name="title">标题</param>
        /// <param name="description">描述</param>
        /// <param name="url">url地址</param>
        /// <returns></returns>
        private async Task SendQyWechatNotice(long checkId, int applyCategory, long approverId, string qyUserId, string title, string description, string url)
        {
            if (!string.IsNullOrWhiteSpace(qyUserId) && !string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(description) && !string.IsNullOrEmpty(url))
            {
                Textcard textcard = new()
                {
                    btntxt = "详情",
                    title = title.Replace("#checkId#", checkId.ToString()),
                    description = description.Replace("#checkId#", checkId.ToString()),
                    url = applyCategory switch
                    {
                        //线索管理
                        (int)ApplicationEnums.ApplicationCategory.Test => $"{url}?CheckId={checkId}&EmployeeId={approverId}",
                        _ => string.Empty
                    }
                };

                CardMsgSendModel sendModel = new()
                {
                    touser = qyUserId,
                    textcard = textcard
                };

                //发送企业微信卡片通知
                await _qyWechatApi.SendCardMessage(sendModel);
            }
        }
        /// <summary>
        /// 发送给多个抄送人的企业微信通知(生产环境可用)
        /// </summary>
        /// <param name="checkId">审批Id</param>
        /// <param name="applyCategory">申请类别</param>
        /// <param name="receivers">收件人</param>
        /// <param name="title">标题</param>
        /// <param name="description">描述</param>
        /// <param name="url">url地址</param>
        private async Task SendBatchQyWechatNotice(long checkId, int applyCategory, List<CarbonCopiesInfoModel> receivers, string title, string description, string url)
        {
            //企业微信UserIds
            string qyUserIds = string.Join("|", receivers.Where(c => c.QyUserId != null).Select(c => c.QyUserId));
            //抄送不需要校验QyUserId
            await SendQyWechatNotice(checkId, applyCategory, -1, qyUserIds, title, description, url);

        }
        #endregion

        #region 系统消息通知
        /// <summary>
        /// 发送系统消息通知
        /// </summary>
        /// <param name="checkId">审批Id</param>
        /// <param name="approverId">审批人Id</param>
        /// <param name="message">描述</param>
        /// <returns></returns>
        private async Task SendSystemNotice(long checkId, long? approverId, string message)
        {
            await _capPublisher.PublishAsync(SystemSubscriber.NOTIFY_MESSAGE, $"[{checkId}]-[{approverId}]" + message);
        }
        /// <summary>
        /// 批量发送系统消息通知
        /// </summary>
        /// <param name="checkId">审批Id</param>
        /// <param name="receivers">接收人</param>
        /// <param name="message">描述</param>
        /// <returns></returns>
        private async Task SendBatchSystemNotice(long checkId, List<long> receivers, string message)
        {
            foreach (var item in receivers)
            {
                await SendSystemNotice(checkId, item, message);
            }
        }
        #endregion
    }
}
