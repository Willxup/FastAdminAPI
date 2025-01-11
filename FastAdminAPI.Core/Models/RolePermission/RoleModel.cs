using FastAdminAPI.Common.Enums;
using FastAdminAPI.Common.JsonTree;
using FastAdminAPI.Framework.Models;
using Newtonsoft.Json;
using SqlSugar.Attributes.Extension.Extensions.Attributes.Operation;
using System;
using System.ComponentModel.DataAnnotations;

namespace FastAdminAPI.Core.Models.RolePermission
{
    #region 查询
    public class RoleInfoModel : JsonTree
    {
    }
    #endregion

    #region 操作
    public class AddRoleModel : DbOperationBaseModel
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        [Required(ErrorMessage = "角色名称不能为空!")]
        [DbOperationField("S03_RoleName")]
        public string RoleName { get; set; }
        /// <summary>
        /// 父角色Id
        /// </summary>
        [DbOperationField("S03_ParentRoleId", false, true)]
        public long? ParentRoleId { get; set; }
        /// <summary>
        /// 角标 每一级4位数字
        /// </summary>
        [JsonIgnore]
        [DbOperationField("S03_CornerMark")]
        public string CornerMark { get; set; }
        /// <summary>
        /// 是否删除 0否 1是
        /// </summary>
        [JsonIgnore]
        [DbOperationField("S03_IsDelete")]
        public byte IsDelete { get; private set; } = (byte)BaseEnums.TrueOrFalse.False;
        [JsonIgnore]
        [DbOperationField("S03_CreateId")]
        public long OperationId { get; set; }
        [JsonIgnore]
        [DbOperationField("S03_CreateBy")]
        public string OperationName { get; set; }
        [JsonIgnore]
        [DbOperationField("S03_CreateTime")]
        public DateTime OperationTime { get; set; }
    }
    public class EditRoleModel : DbOperationBaseModel
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        [Required(ErrorMessage = "模块Id不能为空!")]
        [DbOperationField("S03_RoleId", true)]
        public long? RoleId { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        [Required(ErrorMessage = "角色名称不能为空!")]
        [DbOperationField("S03_RoleName")]
        public string RoleName { get; set; }
        [JsonIgnore]
        [DbOperationField("S03_ModifyId")]
        public long OperationId { get; set; }
        [JsonIgnore]
        [DbOperationField("S03_ModifyBy")]
        public string OperationName { get; set; }
        [JsonIgnore]
        [DbOperationField("S03_ModifyTime")]
        public DateTime OperationTime { get; set; }
    }
    public class CopyRoleModel
    {
        /// <summary>
        /// 源角色Id(复制时的源角色Id)
        /// </summary>
        [Required(ErrorMessage = "角色Id不能为空!")]
        public long? SourceRoleId { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        [Required(ErrorMessage = "角色名称不能为空!")]
        public string RoleName { get; set; }
        /// <summary>
        /// 父角色Id
        /// </summary>
        public long? ParentRoleId { get; set; }
    }
    #endregion
}
