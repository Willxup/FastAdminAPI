using FastAdminAPI.Business.Interfaces;
using FastAdminAPI.Common.Attributes;
using FastAdminAPI.Common.BASE;
using FastAdminAPI.Common.Enums;
using FastAdminAPI.Core.IServices;
using FastAdminAPI.Core.Models.BasicSettings;
using FastAdminAPI.Core.Services.BASE;
using FastAdminAPI.Framework.Entities;
using FastAdminAPI.Framework.Extensions;
using Microsoft.AspNetCore.Http;
using SqlSugar;
using SqlSugar.Attributes.Extension.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.Services
{
    /// <summary>
    /// 基础设置
    /// </summary>
    public class BasicService : BaseService, IBasicService
    {

        /// <summary>
        /// 数据权限Service
        /// </summary>
        private readonly IDataPermissionService _dataPermission;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="httpContext"></param>
        /// <param name="dataPermission"></param>
        public BasicService(ISqlSugarClient dbContext, IHttpContextAccessor httpContext,
            IDataPermissionService dataPermission) : base(dbContext, httpContext)
        {
            _dataPermission = dataPermission;
        }

        #region 字典
        /// <summary>
        /// 获取字典列表
        /// </summary>
        /// <param name="pageSearch"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetCodeList(CodePageSearch pageSearch)
        {
            return await _dbContext.Queryable<S99_Code>()
                .Where(S99 => S99.S99_IsDelete == (byte)BaseEnums.TrueOrFalse.False)
                .ToListResultAsync(pageSearch, new CodePageResult());
        }
        /// <summary>
        /// 获取字典分组列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<GroupCodeModel>> GetGroupCodeList()
        {
            return await _dbContext.Queryable<S99_Code>()
                .Where(S99 => S99.S99_IsDelete == (byte)BaseEnums.TrueOrFalse.False)
                .GroupBy(S99 => new { S99.S99_GroupCode, S99.S99_GroupName })
                .Select(S99 => new GroupCodeModel
                {
                    GroupCode = S99.S99_GroupCode,
                    GroupName = S99.S99_GroupName
                }).ToListAsync();
        }
        /// <summary>
        /// 新增字典
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddCode(AddCodeModel model)
        {
            model.OperationId = _employeeId;
            model.OperationName = _employeeName;
            model.OperationTime = _dbContext.GetDate();
            return await _dbContext.InsertResultAsync<AddCodeModel, S99_Code>(model);
        }
        /// <summary>
        /// 编辑字典
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="UserOperationException"></exception>
        public async Task<ResponseModel> EditCode(EditCodeModel model)
        {
            bool isSysFlag = await _dbContext.Queryable<S99_Code>()
                .Where(S99 => S99.S99_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                              S99.S99_SysFlag == (byte)BaseEnums.SystemFlag.System &&
                              S99.S99_CodeId == model.CodeId)
                .AnyAsync();
            if (isSysFlag)
                throw new UserOperationException("系统字典无法进行编辑!");

            model.OperationId = _employeeId;
            model.OperationName = _employeeName;
            model.OperationTime = _dbContext.GetDate();

            return await _dbContext.UpdateResultAsync<EditCodeModel, S99_Code>(model);
        }
        /// <summary>
        /// 删除字典
        /// </summary>
        /// <param name="codeId">字典Id</param>
        /// <returns></returns>
        /// <exception cref="UserOperationException"></exception>
        public async Task<ResponseModel> DelCode(long codeId)
        {
            bool isSysFlag = await _dbContext.Queryable<S99_Code>()
                .Where(S99 => S99.S99_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                              S99.S99_SysFlag == (byte)BaseEnums.SystemFlag.System &&
                              S99.S99_CodeId == codeId)
                .AnyAsync();
            if (isSysFlag)
                throw new UserOperationException("系统字典无法进行删除!");

            return await _dbContext.Deleteable<S99_Code>()
                .Where(S99 => S99.S99_CodeId == codeId)
                .SoftDeleteAsync(S99 => new S99_Code
                {
                    S99_IsDelete = (byte)BaseEnums.TrueOrFalse.True,
                    S99_DeleteId = _employeeId,
                    S99_DeleteBy = _employeeName,
                    S99_DeleteTime = SqlFunc.GetDate()
                });
        }
        #endregion

        #region 数据权限设置
        /// <summary>
        /// 获取数据权限设置列表
        /// </summary>
        /// <param name="pageSearch"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetDataPermissionSettingList(DataPermissionSettingsPageSearch pageSearch)
        {
            var result = await _dbContext.Queryable<S10_DataPermission>()
                .ToListResultAsync(pageSearch, new DataPermissionSettingsPageResult());

            if (result?.Code == ResponseCode.Success)
            {
                var list = result.ToConvertData<List<DataPermissionSettingsPageResult>>();

                if (list?.Count > 0)
                {
                    //获取全部有效部门
                    var departs = await _dbContext.Queryable<S05_Department>()
                        .Where(S05 => S05.S05_IsDelete == (byte)BaseEnums.TrueOrFalse.False)
                        .Select(S05 => new { S05.S05_DepartId, S05.S05_DepartName, S05.S05_CornerMark })
                        .ToListAsync();

                    //循环赋值
                    list.ForEach(item =>
                    {
                        //获取员工主部门
                        if (item.DepartId != null)
                        {
                            item.DepartName = departs.Where(c => c.S05_DepartId == item.DepartId).Select(c => c.S05_DepartName).FirstOrDefault();
                        }

                        //获取员工权限部门
                        if (!string.IsNullOrEmpty(item.PermitDepartIds))
                        {
                            item.PermitDepartIdList = item.PermitDepartIds.Split(",").Select(c => Convert.ToInt64(c)).ToList();
                            item.PermitDepartsName = string.Join(" ", departs.Where(c => item.PermitDepartIdList.Contains(c.S05_DepartId)).Select(c => c.S05_DepartName));
                        }
                    });

                    result.Data = list;
                }
            }

            return result;
        }
        /// <summary>
        /// 新增数据权限设置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="UserOperationException"></exception>
        public async Task<ResponseModel> AddDataPermissionSetting(AddDataPermissionSettingModel model)
        {
            if (!model.DepartIdList?.Any() ?? true)
                throw new UserOperationException("请选择授权部门!");

            bool isExist = await _dbContext.Queryable<S10_DataPermission>()
                    .Where(S10 => S10.S07_EmployeeId == model.EmployeeId)
                    .AnyAsync();
            if (isExist)
                throw new UserOperationException("当前员工已设置过数据权限!");

            model.OperationId = _employeeId;
            model.OperationName = _employeeName;
            model.OperationTime = _dbContext.GetDate();

            //新增数据权限
            var result = await _dbContext.InsertResultAsync<AddDataPermissionSettingModel, S10_DataPermission>(model);

            if (result?.Code == ResponseCode.Success)
            {
                //释放数据权限
                await _dataPermission.Release((long)model.EmployeeId);
            }

            return result;
        }
        /// <summary>
        /// 编辑数据权限设置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="UserOperationException"></exception>
        public async Task<ResponseModel> EditDataPermissionSetting(EditDataPermissionSettingModel model)
        {
            if (!model.DepartIdList?.Any() ?? true)
                throw new UserOperationException("请选择授权部门!");

            //获取数据权限信息
            var dataPermission = await _dbContext.Queryable<S10_DataPermission>()
                .Where(S10 => S10.S10_DataPermissionId == model.DataPermissionId)
                .Select(S10 => new
                {
                    DataPermissionId = S10.S10_DataPermissionId,
                    EmployeeId = S10.S07_EmployeeId,
                    DepartIds = S10.S05_DepartIds
                })
                .FirstAsync() ?? throw new UserOperationException("找不到该数据权限设置!");

            model.OperationId = _employeeId;
            model.OperationName = _employeeName;
            model.OperationTime = _dbContext.GetDate();

            //更新数据权限
            var result = await _dbContext.UpdateResultAsync<EditDataPermissionSettingModel, S10_DataPermission>(model);

            if (result?.Code == ResponseCode.Success)
            {
                //释放员工数据权限
                await _dataPermission.Release((long)model.EmployeeId);
                await _dataPermission.Release(dataPermission.EmployeeId);
            }

            return result;
        }
        /// <summary>
        /// 删除数据权限设置
        /// </summary>
        /// <param name="dataPermissionId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> DelDataPermissionSetting(long dataPermissionId)
        {
            //获取数据权限信息
            var dataPermission = await _dbContext.Queryable<S10_DataPermission>()
                .Where(S10 => S10.S10_DataPermissionId == dataPermissionId)
                .Select(S10 => new
                {
                    DataPermissionId = S10.S10_DataPermissionId,
                    EmployeeId = S10.S07_EmployeeId,
                    DepartIds = S10.S05_DepartIds
                })
                .FirstAsync() ?? throw new UserOperationException("找不到该数据权限设置!");

            //删除数据权限
            var result = await _dbContext.Deleteable<S10_DataPermission>()
                .Where(S10 => S10.S10_DataPermissionId == dataPermissionId)
                .ExecuteAsync();

            if (result?.Code == ResponseCode.Success)
            {
                //释放数据权限
                await _dataPermission.Release(dataPermission.EmployeeId);
            }

            return result;
        }
        #endregion

        #region 审批流程设置
        /// <summary>
        /// 获取审批流程列表
        /// </summary>
        /// <param name="pageSearch"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetCheckProcessList(CheckProcessPageSearch pageSearch)
        {
            var result = await _dbContext.Queryable<S11_CheckProcess>()
                .Where(S11 => S11.S11_IsDelete == (byte)BaseEnums.TrueOrFalse.False)
                //查询申请类型
                .WhereIF(pageSearch.ApplicationType?.Count > 0, S11 => pageSearch.ApplicationType.Contains((long)S11.S99_ApplicationType))
                //查询审批类型
                .WhereIF(pageSearch.ApproveTypes?.Count > 0, S11 => pageSearch.ApproveTypes.Contains(S11.S11_ApproveType))
                .Select(S11 => new CheckProcessPageResult
                {
                    CheckProcessId = S11.S11_CheckProcessId,
                    ApplicationType = S11.S99_ApplicationType,
                    ApplicationTypeName = SqlFunc.Subqueryable<S99_Code>()
                                           .Where(S99 => S99.S99_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                                                         S99.S99_CodeId == S11.S99_ApplicationType)
                                           .Select(S99 => S99.S99_Name),
                    Applicants = S11.S07_Applicants,
                    Approvers = S11.S07_Approvers,
                    CarbonCopies = S11.S07_CarbonCopies,
                    ApproveType = S11.S11_ApproveType,
                    Amounts = S11.S11_Amounts,
                    Remark = S11.S11_Remark
                })
                .ToListResultAsync(pageSearch.Index, pageSearch.Size);

            if (result?.Code == ResponseCode.Success)
            {
                var list = result.ToConvertData<List<CheckProcessPageResult>>();

                if (list?.Count > 0)
                {
                    //获取所有在职员工信息
                    var employees = await _dbContext.Queryable<S07_Employee>()
                        .Where(S07 => S07.S07_IsDelete == (byte)BaseEnums.TrueOrFalse.False)
                        .Select(S07 => new { S07.S07_EmployeeId, S07.S07_Name })
                        .ToListAsync();

                    foreach (var item in list)
                    {
                        //申请人
                        if (!string.IsNullOrEmpty(item.Applicants))
                        {
                            var applicantIds = item.Applicants.Split(",").ToList();
                            item.ApplicantName = string.Join(",", employees.Where(S07 => applicantIds.Contains(S07.S07_EmployeeId.ToString()))
                                                                           .Select(S07 => S07.S07_Name).ToList());
                        }

                        //审核人
                        if (!string.IsNullOrEmpty(item.Approvers))
                        {
                            var approverIds = item.Approvers.Split(",").ToList();
                            if (approverIds?.Count > 0)
                            {
                                List<string> employeeNameList = new();

                                foreach (var approverId in approverIds)
                                {
                                    employeeNameList.Add(employees.Where(S07 => S07.S07_EmployeeId == Convert.ToInt64(approverId)).Select(S07 => S07.S07_Name).FirstOrDefault());
                                }

                                item.ApproverName = string.Join(",", employeeNameList);
                            }
                        }
                        //审核人为空时，当前申请审批类型是为"直接上级"
                        else
                        {
                            if (item.ApproveType == (byte)ApplicationEnums.ApproveType.Superior)
                                item.ApproverName = "上级";
                        }

                        //抄送人
                        if (!string.IsNullOrEmpty(item.CarbonCopies))
                        {
                            var CarbonCopieIds = item.CarbonCopies.Split(",").ToList();
                            item.CarbonCopieName = string.Join(",", employees.Where(S07 => CarbonCopieIds.Contains(S07.S07_EmployeeId.ToString()))
                                                                             .Select(S07 => S07.S07_Name).ToList());
                        }
                    };

                    result.Data = list;
                }
            }

            return result;
        }
        /// <summary>
        /// 新增审批流程
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddCheckProcess(AddCheckProcessModel model)
        {
            #region 校验
            // 获取相同类型的审批流程
            var checkProcesses = await _dbContext.Queryable<S11_CheckProcess>()
                .Where(S11 => S11.S11_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                              S11.S99_ApplicationType == model.ApplicationType)
                .Select(S11 => new
                {
                    CheckProcessId = S11.S11_CheckProcessId,
                    Applicants = S11.S07_Applicants
                }).ToListAsync();

            if (checkProcesses?.Count > 0)
            {
                // 如果有申请人，说明不是通用审批流程
                if (model.ApplicantList?.Count > 0)
                {
                    // 获取所有申请人
                    List<string> applicantIds = new();

                    // 将本次审批流程中的申请人一起加入
                    applicantIds.AddRange(model.ApplicantList);

                    // 将所有相同类型的审批流程的申请人取出
                    foreach (var item in checkProcesses.Where(c => !string.IsNullOrEmpty(c.Applicants)))
                    {
                        applicantIds.AddRange(item.Applicants.Split(",").ToList());
                    }

                    // 判断申请人是否重复出现过
                    if (applicantIds.GroupBy(i => i).Any(g => g.Count() > 1))
                    {
                        throw new UserOperationException("已存在相同类型相同申请人的审批流程!");
                    }
                }
                // 如果没有申请人，说明是通用审批流程
                else
                {
                    bool isSameCommonCheck = checkProcesses.Where(c => string.IsNullOrEmpty(c.Applicants)).Any();

                    if (isSameCommonCheck)
                    {
                        throw new UserOperationException("已存在相同类型相同申请人的审批流程!");
                    }
                }
            }
            #endregion

            // 上级
            if (model.ApproveType == (byte)ApplicationEnums.ApproveType.Superior)
            {
                model.Approvers = null;
            }
            // 指定人员
            else if (model.ApproveType == (byte)ApplicationEnums.ApproveType.Designee)
            {
                if (model.ApproverList?.Count > 0)
                {
                    if (model.ApproverList.GroupBy(i => i).Any(g => g.Count() > 1))
                        throw new UserOperationException("审批人重复!");
                    else
                        model.Approvers = string.Join(",", model.ApproverList);
                }
                else
                    throw new UserOperationException("请选择审批人!");
            }
            // 自定义
            else if (model.ApproveType == (byte)ApplicationEnums.ApproveType.Customize)
            {
                model.Approvers = null;
            }
            // 上级+指定人员
            else if (model.ApproveType == (byte)ApplicationEnums.ApproveType.SuperiorAndDesignee)
            {
                if (model.ApproverList?.Count > 0)
                {
                    if (model.ApproverList.GroupBy(i => i).Any(g => g.Count() > 1))
                        throw new UserOperationException("审批人重复!");
                    else
                        model.Approvers = string.Join(",", model.ApproverList);
                }
                else
                    throw new UserOperationException("请选择审批人!");
            }

            // 申请人
            if (model.ApplicantList?.Count > 0)
            {
                model.Applicants = string.Join(",", model.ApplicantList);
            }
            // 抄送人
            if (model.CarbonCopieList?.Count > 0)
            {
                model.CarbonCopies = string.Join(",", model.CarbonCopieList);
            }

            model.OperationId = _employeeId;
            model.OperationName = _employeeName;
            model.OperationTime = _dbContext.GetDate();

            return await _dbContext.InsertResultAsync<AddCheckProcessModel, S11_CheckProcess>(model);
        }
        /// <summary>
        /// 编辑审批流程
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseModel> EditCheckProcess(EditCheckProcessModel model)
        {
            #region 校验
            // 获取相同类型的审批流程(除了当前编辑的流程)
            var checkProcesses = await _dbContext.Queryable<S11_CheckProcess>()
                .Where(S11 => S11.S11_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                              S11.S99_ApplicationType == model.ApplicationType &&
                              S11.S11_CheckProcessId != model.CheckProcessId)
                .Select(S11 => new
                {
                    CheckProcessId = S11.S11_CheckProcessId,
                    Applicants = S11.S07_Applicants
                }).ToListAsync();

            if (checkProcesses?.Count > 0)
            {
                // 如果有申请人，说明不是通用审批流程
                if (model.ApplicantList?.Count > 0)
                {
                    // 获取所有申请人
                    List<string> applicantIds = new();

                    // 将本次审批流程中的申请人一起加入
                    applicantIds.AddRange(model.ApplicantList);

                    // 将所有相同类型的审批流程的申请人取出
                    foreach (var item in checkProcesses.Where(c => !string.IsNullOrEmpty(c.Applicants)))
                    {
                        applicantIds.AddRange(item.Applicants.Split(",").ToList());
                    }

                    // 判断申请人是否重复出现过
                    if (applicantIds.GroupBy(i => i).Any(g => g.Count() > 1))
                    {
                        throw new UserOperationException("已存在相同类型相同申请人的审批流程!");
                    }
                }
                // 如果没有申请人，说明是通用审批流程
                else
                {
                    bool isSameCommonCheck = checkProcesses.Where(c => string.IsNullOrEmpty(c.Applicants)).Any();

                    if (isSameCommonCheck)
                    {
                        throw new UserOperationException("已存在相同类型相同申请人的审批流程!");
                    }
                }
            }
            #endregion

            // 上级
            if (model.ApproveType == (byte)ApplicationEnums.ApproveType.Superior)
            {
                model.Approvers = null;
            }
            // 指定人员
            else if (model.ApproveType == (byte)ApplicationEnums.ApproveType.Designee)
            {
                if (model.ApproverList?.Count > 0)
                {
                    if (model.ApproverList.GroupBy(i => i).Any(g => g.Count() > 1))
                        throw new UserOperationException("审批人重复!");
                    else
                        model.Approvers = string.Join(",", model.ApproverList);
                }
                else
                    throw new UserOperationException("请选择审批人!");
            }
            // 自定义
            else if (model.ApproveType == (byte)ApplicationEnums.ApproveType.Customize)
            {
                model.Approvers = null;
            }
            // 上级+指定人员
            else if (model.ApproveType == (byte)ApplicationEnums.ApproveType.SuperiorAndDesignee)
            {
                if (model.ApproverList?.Count > 0)
                {
                    if (model.ApproverList.GroupBy(i => i).Any(g => g.Count() > 1))
                        throw new UserOperationException("审批人重复!");
                    else
                        model.Approvers = string.Join(",", model.ApproverList);
                }
                else
                    throw new UserOperationException("请选择审批人!");
            }

            // 申请人
            if (model.ApplicantList?.Count > 0)
            {
                model.Applicants = string.Join(",", model.ApplicantList);
            }
            // 抄送人
            if (model.CarbonCopieList?.Count > 0)
            {
                model.CarbonCopies = string.Join(",", model.CarbonCopieList);
            }

            model.OperationId = _employeeId;
            model.OperationName = _employeeName;
            model.OperationTime = _dbContext.GetDate();

            return await _dbContext.UpdateResultAsync<EditCheckProcessModel, S11_CheckProcess>(model);
        }
        /// <summary>
        /// 删除审批流程
        /// </summary>
        /// <param name="checkProcessId">审批流程Id</param>
        /// <returns></returns>
        public async Task<ResponseModel> DelCheckProcess(long checkProcessId)
        {
            return await _dbContext.Deleteable<S11_CheckProcess>()
                .Where(S11 => S11.S11_CheckProcessId == checkProcessId)
                .SoftDeleteAsync(S11 => new S11_CheckProcess
                {
                    S11_IsDelete = (byte)BaseEnums.TrueOrFalse.True,
                    S11_DeleteId = _employeeId,
                    S11_DeleteBy = _employeeName,
                    S11_DeleteTime = SqlFunc.GetDate()
                });
        }
        #endregion

    }
}
