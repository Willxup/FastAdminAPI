using FastAdminAPI.Common.Enums;
using SqlSugar.Attributes.Extension.Extensions;
using FastAdminAPI.Framework.Extensions.Models;
using Newtonsoft.Json;
using SqlSugar.Attributes.Extension.Extensions.Attributes.Operation;
using SqlSugar.Attributes.Extension.Extensions.Attributes.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SqlSugar.Attributes.Extension.Common;

namespace FastAdminAPI.Core.Models.Employee
{
    #region 员工

    #region 查询

    #region 列表
    public class EmployeePageSearch : DbQueryBaseModel
    {
        /// <summary>
        /// 姓名
        /// </summary>
        [DbTableAlias("S07")]
        [DbQueryField("S07_Name")]
        [DbQueryOperator(DbOperator.Like)]
        public string Name { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        [DbTableAlias("S07")]
        [DbQueryField("S07_Phone")]
        [DbQueryOperator(DbOperator.Like)]
        public string Phone { get; set; }
        /// <summary>
        /// 状态 0正式 1实习
        /// </summary>
        [DbTableAlias("S07")]
        [DbQueryField("S07_Status")]
        [DbQueryOperator(DbOperator.In)]
        public List<byte> Status { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        [DbTableAlias("S07")]
        [DbQueryField("S01_UserId")]
        [DbQueryOperator(DbOperator.In)]
        public List<long> UserIds { get; set; }
        /// <summary>
        /// 部门角标
        /// </summary>
        [DbIgnoreField]
        public string CornerMark { get; set; }
        /// <summary>
        /// 是否查询全部
        /// </summary>
        [DbIgnoreField]
        public bool IsAll { get; set; } = false;
    }
    public class EmployeePageResult
    {
        /// <summary>
        /// 员工Id
        /// </summary>
        [DbTableAlias("S07")]
        [DbQueryField("S07_EmployeeId")]
        public long? EmployeeId { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        [DbTableAlias("S07")]
        [DbQueryField("S01_UserId")]
        public long? UserId { get; set; }
        /// <summary>
        /// 企业Id
        /// </summary>
        [DbTableAlias("S07")]
        [DbQueryField("S10_CompanyId")]
        public long? CompanyId { get; set; }
        /// <summary>
        /// 企业名称
        /// </summary>
        [DbIgnoreField]
        public string CompanyName { get; set; }
        /// <summary>
        /// 企业微信UserId
        /// </summary>
        [DbTableAlias("S07")]
        [DbQueryField("S07_QyUserId")]
        public string QyUserId { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [DbTableAlias("S07")]
        [DbQueryField("S07_Name")]
        public string Name { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        [DbTableAlias("S07")]
        [DbQueryField("S07_Phone")]
        public string Phone { get; set; }
        /// <summary>
        /// 性别 0男 1女
        /// </summary>
        [DbTableAlias("S07")]
        [DbQueryField("S07_Gender")]
        public byte? Gender { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        [DbSubQuery("(SELECT S05_DepartName FROM S05_Department WHERE S08.S05_DepartId = S05_DepartId)")]
        public string DepartmentName { get; set; }
        /// <summary>
        /// 岗位名称
        /// </summary>
        [DbSubQuery("(SELECT S06_PostName FROM S06_Post WHERE S08.S06_PostId = S06_PostId)")]
        public string PostName { get; set; }
        /// <summary>
        /// 类别 0全职 1兼职
        /// </summary>
        [DbTableAlias("S07")]
        [DbQueryField("S07_Kind")]
        public byte? Kind { get; set; }
        /// <summary>
        /// 状态 0正式 1实习
        /// </summary>
        [DbTableAlias("S07")]
        [DbQueryField("S07_Status")]
        public byte? Status { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        [DbIgnoreField]
        public List<EmployeeRoleModel> Roles { get; set; }
    }
    public class EmployeeSimpleModel
    {
        /// <summary>
        /// 员工Id
        /// </summary>
        public long? EmployeeId { get; set; }
        /// <summary>
        /// 员工名称
        /// </summary>
        public string EmployeeName { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }
    }
    #endregion

    /// <summary>
    /// 员工账号
    /// </summary>
    public class EmployeeAccountModel
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 账号状态 0启用 1禁用
        /// </summary>
        public byte? AccountStatus { get; set; }
    }
    /// <summary>
    /// 员工角色
    /// </summary>
    public class EmployeeRoleModel
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        public long? RoleId { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }
    }
    /// <summary>
    /// 员工模块
    /// </summary>
    public class EmployeePermissionModel
    {
        /// <summary>
        /// 模块Id
        /// </summary>
        public long? ModuleId { get; set; }
        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleName { get; set; }
        /// <summary>
        /// 属性 0菜单 1页面 2按钮 3列表 9其他
        /// </summary>
        public byte? Kind { get; set; }
    }
    /// <summary>
    /// 员工详情信息
    /// </summary>
    public class EmployeeInfoModel
    {
        /// <summary>
        /// 员工Id
        /// </summary>
        public long? EmployeeId { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public long? UserId { get; set; }
        /// <summary>
        /// 企业Id
        /// </summary>
        public long? CompanyId { get; set; }
        /// <summary>
        /// 企业名称
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 企业微信UserId
        /// </summary>
        public string QyUserId { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 性别 0男 1女
        /// </summary>
        public byte? Gender { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 类别 0全职 1兼职
        /// </summary>
        public byte? Kind { get; set; }
        /// <summary>
        /// 状态 0正式 1实习 2离职
        /// </summary>
        public byte? Status { get; set; }
        /// <summary>
        /// 个人简介
        /// </summary>
        public string Bio { get; set; }
        /// <summary>
        /// 入职日期
        /// </summary>
        public DateTime? HireDate { get; set; }
        /// <summary>
        /// 试用期日期
        /// </summary>
        public DateTime? TrialPeriodDate { get; set; }
        /// <summary>
        /// 转正日期
        /// </summary>
        public DateTime? ConfirmationDate { get; set; }
        /// <summary>
        /// 离职日期
        /// </summary>
        public DateTime? TerminationDate { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartmentName { get; set; }
        /// <summary>
        /// 岗位名称
        /// </summary>
        public string PostName { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public EmployeeAccountModel Account { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public List<EmployeeRoleModel> Roles { get; set; }

        /// <summary>
        /// 模块
        /// </summary>
        public List<EmployeePermissionModel> Permissions { get; set; }
    }
    #endregion

    #region 操作
    /// <summary>
    /// 员工基础信息
    /// </summary>
    public class EmployeeBaseInfoModel
    {
        /// <summary>
        /// 企业Id
        /// </summary>
        public long? CompanyId { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [Required(ErrorMessage = "姓名不能为空!")]
        public string Name { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        [Required(ErrorMessage = "手机号不能为空!")]
        public string Phone { get; set; }
        /// <summary>
        /// 性别 0男 1女
        /// </summary>
        public byte? Gender { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 类别 0全职 1兼职
        /// </summary>
        [Required(ErrorMessage = "类别不能为空!")]
        public byte? Kind { get; set; }
        /// <summary>
        /// 状态 0正式 1实习 2离职
        /// </summary>
        [Required(ErrorMessage = "状态不能为空!")]
        public byte? Status { get; set; }
        /// <summary>
        /// 个人简介
        /// </summary>
        public string Bio { get; set; }
        /// <summary>
        /// 入职日期
        /// </summary>
        public DateTime? HireDate { get; set; }
        /// <summary>
        /// 试用期日期
        /// </summary>
        public DateTime? TrialPeriodDate { get; set; }
        /// <summary>
        /// 转正日期
        /// </summary>
        public DateTime? ConfirmationDate { get; set; }
        /// <summary>
        /// 离职日期
        /// </summary>
        public DateTime? TerminationDate { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 账号状态 false禁用(默认) true启用
        /// </summary>
        [Required(ErrorMessage = "账号状态不能为空!")]
        public bool AccountStatus { get; set; } = false;

        /// <summary>
        /// 角色Id
        /// </summary>
        public List<long> RoleIds { get; set; }

        /// <summary>
        /// 模块Id
        /// </summary>
        public List<long> ModuleIds { get; set; }
    }
    public class AddEmployeeInfoModel : EmployeeBaseInfoModel
    {
        /// <summary>
        /// 部门Id
        /// </summary>
        [Required(ErrorMessage = "部门不能为空!")]
        public long? DepartmentId { get; set; }
        /// <summary>
        /// 岗位Id
        /// </summary>
        [Required(ErrorMessage = "岗位不能为空!")]
        public long? PostId { get; set; }
    }
    public class EditEmployeeInfoModel : EmployeeBaseInfoModel
    {
        /// <summary>
        /// 员工Id
        /// </summary>
        [Required(ErrorMessage = "员工Id不能为空!")]
        public long? EmployeeId { get; set; }
    }
    #endregion 

    #endregion

    #region 员工岗位

    #region 查询
    public class EmployeePostResult
    {
        /// <summary>
        /// 员工岗位Id
        /// </summary>
        public long EmployeePostId { get; set; }
        /// <summary>
        /// 部门Id
        /// </summary>
        public long DepartmentId { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartmentName { get; set; }
        /// <summary>
        /// 岗位Id
        /// </summary>
        public long PostId { get; set; }
        /// <summary>
        /// 岗位名称
        /// </summary>
        public string PostName { get; set; }
        /// <summary>
        /// 是否为主岗位 0否 1是
        /// </summary>
        public byte IsMainPost { get; set; }
    }
    #endregion

    #region 操作
    public class AddEmployeePostModel : DbOperationBaseModel
    {
        /// <summary>
        /// 员工Id
        /// </summary>
        [Required(ErrorMessage = "员工不能为空!")]
        [DbOperationField("S07_EmployeeId")]
        public long? EmployeeId { get; set; }
        /// <summary>
        /// 岗位Id
        /// </summary>
        [Required(ErrorMessage = "岗位不能为空!")]
        [DbOperationField("S06_PostId")]
        public long? PostId { get; set; }
        /// <summary>
        /// 部门Id
        /// </summary>
        [Required(ErrorMessage = "部门不能为空!")]
        [DbOperationField("S05_DepartId")]
        public long? DepartId { get; set; }
        /// <summary>
        /// 是否为主岗位 0否 1是
        /// </summary>
        [JsonIgnore]
        [DbOperationField("S08_IsMainPost")]
        public byte IsMainPost { get; set; } = (byte)BaseEnums.TrueOrFalse.False;

        [JsonIgnore]
        [DbOperationField("S08_IsDelete")]
        public byte IsDelete { get; private set; } = (byte)BaseEnums.TrueOrFalse.False;
        [JsonIgnore]
        [DbOperationField("S08_CreateId")]
        public long OperationId { get; set; }
        [JsonIgnore]
        [DbOperationField("S08_CreateBy")]
        public string OperationName { get; set; }
        [JsonIgnore]
        [DbOperationField("S08_CreateTime")]
        public DateTime OperationTime { get; set; }
    }
    #endregion

    #endregion
}
