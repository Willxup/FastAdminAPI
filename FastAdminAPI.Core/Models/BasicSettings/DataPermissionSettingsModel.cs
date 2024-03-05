
using FastAdminAPI.Framework.Extensions.Models;
using Newtonsoft.Json;
using SqlSugar.Attributes.Extension.Common;
using SqlSugar.Attributes.Extension.Extensions;
using SqlSugar.Attributes.Extension.Extensions.Attributes.Query;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FastAdminAPI.Core.Models.BasicSettings
{
    #region 查询
    [DbDefaultOrderBy("S10_CreateTime", DbSortWay.DESC)]
    public class DataPermissionSettingsPageSearch : DbQueryBaseModel { }
    public class DataPermissionSettingsPageResult
    {
        /// <summary>
        /// 数据权限设置Id
        /// </summary>
        [DbQueryField("S10_DataPermissionId")]
        public long? DataPermissionId { get; set; }
        /// <summary>
        /// 员工Id
        /// </summary>
        [DbQueryField("S07_EmployeeId")]
        public long? EmployeeId { get; set; }
        /// <summary>
        /// 员工名称
        /// </summary>
        [DbSubQuery("(SELECT S07_Name FROM S07_Employee WHERE S07_EmployeeId = S10_DataPermission.S07_EmployeeId)")]
        public string EmployeeName { get; set; }
        /// <summary>
        /// 员工部门Id
        /// </summary>
        [DbSubQuery("(SELECT S05_DepartId FROM S08_EmployeePost WHERE " +
            "S07_EmployeeId = S10_DataPermission.S07_EmployeeId AND S08_IsMainPost = 1 LIMIT 1)")]
        public long? DepartId { get; set; }
        /// <summary>
        /// 员工部门名称
        /// </summary>
        [DbIgnoreField]
        public string DepartName { get; set; }
        /// <summary>
        /// 权限：部门Ids
        /// </summary>
        [JsonIgnore]
        [DbQueryField("S05_DepartIds")]
        public string PermitDepartIds { get; set; }
        /// <summary>
        /// 权限：部门Id集合
        /// </summary>
        [DbIgnoreField]
        public List<long> PermitDepartIdList { get; set; }
        /// <summary>
        /// 权限：部门集合名称
        /// </summary>
        [DbIgnoreField]
        public string PermitDepartsName { get; set; }
    }
    #endregion

    #region 操作
    public class AddDataPermissionSettingModel
    {
        /// <summary>
        /// 员工Id
        /// </summary>
        [Required(ErrorMessage = "员工Id不能为空!")]
        public long? EmployeeId { get; set; }
        /// <summary>
        /// 部门Id集合
        /// </summary>
        public List<long> DepartIdList { get; set; }
    }
    public class EditDataPermissionSettingModel
    {
        /// <summary>
        /// 数据权限设置Id
        /// </summary>
        [Required(ErrorMessage = "数据权限设置Id不能为空!")]
        public long? DataPermissionId { get; set; }
        /// <summary>
        /// 员工Id
        /// </summary>
        [Required(ErrorMessage = "员工Id不能为空!")]
        public long? EmployeeId { get; set; }
        /// <summary>
        /// 部门Id集合
        /// </summary>
        public List<long> DepartIdList { get; set; }
    }
    #endregion
}
