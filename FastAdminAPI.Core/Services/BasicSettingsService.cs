using FastAdminAPI.Core.IServices;
using FastAdminAPI.Core.Models.BasicSettings;
using FastAdminAPI.Core.Services.BASE;
using FastAdminAPI.Business.IServices;
using FastAdminAPI.Common.Attributes;
using FastAdminAPI.Common.BASE;
using FastAdminAPI.Common.Enums;
using FastAdminAPI.Framework.Entities;
using FastAdminAPI.Framework.Extensions;
using FastAdminAPI.Framework.Extensions.DbQueryExtensions;
using Microsoft.AspNetCore.Http;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.Services
{
    /// <summary>
    /// 基础设置
    /// </summary>
    public class BasicSettingsService : BaseService, IBasicSettingsService
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
        public BasicSettingsService(ISqlSugarClient dbContext, IHttpContextAccessor httpContext,
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
                .Where(S99 => S99.S99_IsValid == (byte)BaseEnums.IsValid.Valid)
                .ToAutoBoxResultAsync(pageSearch, new CodePageResult());
        }
        /// <summary>
        /// 获取字典分组列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<GroupCodeModel>> GetGroupCodeList()
        {
            return await _dbContext.Queryable<S99_Code>()
                .Where(S99 => S99.S99_IsValid == (byte)BaseEnums.IsValid.Valid)
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
                .Where(S99 => S99.S99_IsValid == (byte)BaseEnums.IsValid.Valid && S99.S99_CodeId == model.CodeId && S99.S99_SysFlag == (byte)BaseEnums.SystemFlag.System)
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
                .Where(S99 => S99.S99_IsValid == (byte)BaseEnums.IsValid.Valid && S99.S99_CodeId == codeId && S99.S99_SysFlag == (byte)BaseEnums.SystemFlag.System)
                .AnyAsync();
            if (isSysFlag)
                throw new UserOperationException("系统字典无法进行删除!");

            return await _dbContext.Deleteable<S99_Code>()
                .Where(S99 => S99.S99_CodeId == codeId)
                .SoftDeleteAsync(S99 => new S99_Code
                {
                    S99_IsValid = (byte)BaseEnums.IsValid.InValid,
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
                .ToAutoBoxResultAsync(pageSearch, new DataPermissionSettingsPageResult());
            if (result?.Code == ResponseCode.Success)
            {
                var list = result.ToConvertData<List<DataPermissionSettingsPageResult>>();
                if (list?.Count > 0)
                {
                    //获取全部有效部门
                    var departs = await _dbContext.Queryable<S05_Department>()
                        .Where(S05 => S05.S05_IsValid == (byte)BaseEnums.IsValid.Valid)
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
            if (model.DepartIdList?.Count > 0)
            {
                bool isExist = await _dbContext.Queryable<S10_DataPermission>()
                    .Where(S10 => S10.S07_EmployeeId == model.EmployeeId)
                    .AnyAsync();
                if (isExist)
                    throw new UserOperationException("当前员工已设置过数据权限!");

                S10_DataPermission entity = new()
                {
                    S07_EmployeeId = (long)model.EmployeeId,
                    S05_DepartIds = string.Join(",", model.DepartIdList),
                    S10_CreateId = _employeeId,
                    S10_CreateBy = _employeeName,
                    S10_CreateTime = _dbContext.GetDate()
                };
                var result = await _dbContext.Insertable(entity).ExecuteAsync();
                if (result?.Code == ResponseCode.Success)
                {
                    //释放数据权限
                    await _dataPermission.ReleaseDataPermission((long)model.EmployeeId);
                }
                return result;
            }
            else
                throw new UserOperationException("请选择授权部门!");
        }
        /// <summary>
        /// 编辑数据权限设置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="UserOperationException"></exception>
        public async Task<ResponseModel> EditDataPermissionSetting(EditDataPermissionSettingModel model)
        {
            if (model.DepartIdList?.Count > 0)
            {
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

                //先释放之前员工数据权限
                await _dataPermission.ReleaseDataPermission(dataPermission.EmployeeId);

                //更新数据权限
                var result = await _dbContext.Updateable<S10_DataPermission>()
                    .SetColumns(S10 => S10.S07_EmployeeId == model.EmployeeId)
                    .SetColumns(S10 => S10.S05_DepartIds == string.Join(",", model.DepartIdList))
                    .SetColumns(S10 => S10.S10_ModifyId == _employeeId)
                    .SetColumns(S10 => S10.S10_ModifyBy == _employeeName)
                    .SetColumns(S10 => S10.S10_ModifyTime == SqlFunc.GetDate())
                    .Where(S10 => S10.S10_DataPermissionId == model.DataPermissionId)
                    .ExecuteAsync();
                if (result?.Code == ResponseCode.Success)
                {
                    //释放员工数据权限
                    await _dataPermission.ReleaseDataPermission((long)model.EmployeeId);
                    await _dataPermission.ReleaseDataPermission(dataPermission.EmployeeId);
                }
                return result;
            }
            else
                throw new UserOperationException("请选择授权部门!");
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
                await _dataPermission.ReleaseDataPermission(dataPermission.EmployeeId);
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
            string sql = $"";
            //申请人
            if (pageSearch.Applicants?.Count > 0)
            {
                foreach (var item in pageSearch.Applicants)
                {
                    sql += $" FIND_IN_SET('{item}', S11.S07_Applicants ) > 0 OR";
                }
            }
            //审核人
            if (pageSearch.Approvers?.Count > 0)
            {
                foreach (var item in pageSearch.Approvers)
                {
                    sql += $" FIND_IN_SET('{item}', S11.S07_Approvers ) > 0 OR";
                }
            }
            //抄送人
            if (pageSearch.CarbonCopies?.Count > 0)
            {
                foreach (var item in pageSearch.CarbonCopies)
                {
                    sql += $" FIND_IN_SET('{item}', S11.S07_CarbonCopies ) > 0 OR";
                }
            }

            sql += Regex.Replace(sql, "OR$", "");//去除末尾多余OR

            var result = await _dbContext.Queryable<S11_CheckProcess>()
                .WhereIF(pageSearch.Applicants?.Count > 0 || pageSearch.Approvers?.Count > 0 || pageSearch.CarbonCopies?.Count > 0, sql)
                .WhereIF(pageSearch.CheckProcessTypes?.Count > 0, S11 => pageSearch.CheckProcessTypes.Contains((long)S11.S99_ApplicationType))
                .WhereIF(pageSearch.ApproveTypes?.Count > 0, S11 => pageSearch.ApproveTypes.Contains(S11.S11_ApproveType))
                .Where(S11 => S11.S11_IsValid == (byte)BaseEnums.IsValid.Valid)
                .Select(S11 => new CheckProcessPageResult
                {
                    CheckProcessId = S11.S11_CheckProcessId,
                    CheckProcessType = S11.S99_ApplicationType,
                    CheckProcessTypeName = SqlFunc.Subqueryable<S99_Code>()
                                           .Where(S99 => S99.S99_IsValid == (byte)BaseEnums.IsValid.Valid &&
                                                         S99.S99_CodeId == S11.S99_ApplicationType)
                                           .Select(S99 => S99.S99_Name),
                    Applicants = S11.S07_Applicants,
                    Approvers = S11.S07_Approvers,
                    CarbonCopies = S11.S07_CarbonCopies,
                    ApproveType = S11.S11_ApproveType,
                    Amounts = S11.S11_Amounts,
                    Remark = S11.S11_Remark
                })
                .ToResultAsync(pageSearch.Index, pageSearch.Size);
            if (result?.Code == ResponseCode.Success)
            {
                var list = result.ToConvertData<List<CheckProcessPageResult>>();
                if (list?.Count > 0)
                {
                    var staffList = await _dbContext.Queryable<S07_Employee>()
                    .Select(S07 => new { S07.S07_EmployeeId, S07.S07_Name })
                    .ToListAsync();
                    foreach (var item in list)
                    {
                        //申请人
                        if (!string.IsNullOrEmpty(item.Applicants))
                        {
                            var applicantIds = item.Applicants.Split(",").ToList();
                            item.ApplicantName = string.Join(",", staffList.Where(S07 => applicantIds.Contains(S07.S07_EmployeeId.ToString()))
                                                                           .Select(S07 => S07.S07_Name).ToList());
                        }
                        //审核人
                        if (!string.IsNullOrEmpty(item.Approvers))
                        {
                            var ApproverIds = item.Approvers.Split(",").ToList();
                            if (ApproverIds?.Count > 0)
                            {
                                List<string> staffNameList = new();
                                foreach (var item1 in ApproverIds)
                                {
                                    staffNameList.Add(staffList.Where(S07 => S07.S07_EmployeeId == Convert.ToInt64(item1)).Select(S07 => S07.S07_Name).FirstOrDefault());
                                }
                                item.ApproverName = string.Join(",", staffNameList);
                            }
                        }
                        else//审核人为空时，当前申请审批类型是为"直接上级"
                        {
                            if (item.ApproveType == (byte)ApplicationEnums.ApproveType.Superior)
                                item.ApproverName = "上级";
                        }
                        //抄送人
                        if (!string.IsNullOrEmpty(item.CarbonCopies))
                        {
                            var CarbonCopieIds = item.CarbonCopies.Split(",").ToList();
                            item.CarbonCopieName = string.Join(",", staffList.Where(S07 => CarbonCopieIds.Contains(S07.S07_EmployeeId.ToString()))
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
            var applicantList = await _dbContext.Queryable<S11_CheckProcess>()
                .Where(S11 => S11.S99_ApplicationType == model.CheckProcessType && S11.S11_IsValid == (byte)BaseEnums.IsValid.Valid)
                .Select(S11 => S11.S07_Applicants).ToListAsync();
            if (applicantList?.Count > 0)
            {
                int nullCount = 0;
                List<string> applicant = new();
                foreach (var item in applicantList)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        applicant.AddRange(item.Split(",").ToList());
                    }
                    else
                    {
                        nullCount++;
                    }
                }
                if (model.ApplicantList?.Count > 0)
                {
                    applicant.AddRange(model.ApplicantList);
                    if (applicant.GroupBy(i => i).Any(g => g.Count() > 1))
                    {
                        throw new UserOperationException("已存在相同类型相同申请人的审批流程!");
                    }
                }
                else
                {
                    if (nullCount > 0)
                    {
                        throw new UserOperationException("已存在相同类型相同申请人的审批流程!");
                    }
                }
            }

            if (model.ApproveType == (byte)ApplicationEnums.ApproveType.Superior)
            {
                model.Approvers = null;
            }
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
            else if (model.ApproveType == (byte)ApplicationEnums.ApproveType.Customize)
            {
                model.Approvers = null;
            }
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
            else if (model.ApproveType == (byte)ApplicationEnums.ApproveType.SuperiorAndDesigneeWithAmount)
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
                //判断审批金额是否为空
                if (model.AmountList?.Count > 0)
                {
                    //判断审批金额数和审批人数相等
                    if (model.ApproverList?.Count > model.AmountList?.Count)
                    {
                        throw new UserOperationException("请选择审批人!");
                    }
                    else if (model.ApproverList?.Count < model.AmountList?.Count)
                    {
                        throw new UserOperationException("请选择审批金额!");
                    }
                    else
                        model.Amounts = string.Join(",", model.AmountList);
                }
                else
                    throw new UserOperationException("请选择审批金额!");
            }

            if (model.ApplicantList?.Count > 0)
            {
                model.Applicants = string.Join(",", model.ApplicantList);
            }
            if (model.ApproverList?.Count <= 0)
            {
                model.Approvers = null;
            }
            if (model.CarbonCopieList?.Count > 0)
            {
                model.CarbonCopies = string.Join(",", model.CarbonCopieList);
            }
            else
            {
                model.CarbonCopies = null;
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
            //查询当前被修改审批流程之外的审批流程
            var applicantList = await _dbContext.Queryable<S11_CheckProcess>()
                .Where(S11 => S11.S99_ApplicationType == model.CheckProcessType &&
                              S11.S11_CheckProcessId != model.CheckProcessId &&
                              S11.S11_IsValid == (byte)BaseEnums.IsValid.Valid)
                .Select(S11 => S11.S07_Applicants).ToListAsync();
            if (applicantList?.Count > 0)
            {
                int nullCount = 0;
                List<string> applicant = new();
                foreach (var item in applicantList)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        applicant.AddRange(item.Split(",").ToList());
                    }
                    else
                    {
                        nullCount++;
                    }
                }
                if (model.ApplicantList?.Count > 0)
                {
                    applicant.AddRange(model.ApplicantList);
                    if (applicant.GroupBy(i => i).Any(g => g.Count() > 1))
                    {
                        throw new UserOperationException("已存在相同类型相同申请人的审批流程!");
                    }
                }
                else
                {
                    if (nullCount > 0)
                    {
                        throw new UserOperationException("已存在相同类型相同申请人的审批流程!");
                    }
                }
            }

            if (model.ApproveType == (byte)ApplicationEnums.ApproveType.Superior)
            {
                model.Approvers = null;
            }
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
            else if (model.ApproveType == (byte)ApplicationEnums.ApproveType.Customize)
            {
                model.Approvers = null;
            }
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
            else if (model.ApproveType == (byte)ApplicationEnums.ApproveType.SuperiorAndDesigneeWithAmount)
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
                //判断审批金额是否为空
                if (model.AmountList?.Count > 0)
                {
                    //判断审批金额数和审批人数相等
                    if (model.ApproverList?.Count > model.AmountList?.Count)
                    {
                        throw new UserOperationException("请选择审批人!");
                    }
                    else if (model.ApproverList?.Count < model.AmountList?.Count)
                    {
                        throw new UserOperationException("请选择审批金额!");
                    }
                    else
                        model.Amounts = string.Join(",", model.AmountList);
                }
                else
                    throw new UserOperationException("请选择审批金额!");
            }

            if (model.ApplicantList?.Count > 0)
            {
                model.Applicants = string.Join(",", model.ApplicantList);
            }
            if (model.ApproverList?.Count <= 0)
            {
                model.Approvers = null;
            }
            if (model.CarbonCopieList?.Count > 0)
            {
                model.CarbonCopies = string.Join(",", model.CarbonCopieList);
            }
            else
            {
                model.CarbonCopies = null;
            }

            model.OperationId = _employeeId;
            model.OperationName = _employeeName;
            model.OperationTime = _dbContext.GetDate();
            return await _dbContext.UpdateResultAsync<EditCheckProcessModel, S11_CheckProcess>(model);
        }
        /// <summary>
        /// 删除审批流程
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseModel> DelCheckProcess(DelCheckProcessModel model)
        {
            model.OperationId = _employeeId;
            model.OperationName = _employeeName;
            model.OperationTime = _dbContext.GetDate();
            return await _dbContext.SoftDeleteAsync<DelCheckProcessModel, S11_CheckProcess>(model);
        }
        #endregion

    }
}
