using FastAdminAPI.Common.JsonTree;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FastAdminAPI.Core.Models.RolePermission
{
    public class SaveRolePermissionModel
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        [Required(ErrorMessage = "角色Id不能为空!")]
        public long? RoleId { get; set; }
        /// <summary>
        /// 模块Ids
        /// </summary>
        public List<long> ModuleIds { get; set; }
    }
}
