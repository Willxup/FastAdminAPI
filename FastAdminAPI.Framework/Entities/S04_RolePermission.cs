
using System;
using SqlSugar;


namespace FastAdminAPI.Framework.Entities
{
    ///<summary>
    ///角色权限
    ///</summary>
    [SugarTable("S04_RolePermission")]
    [Serializable]
    public partial class S04_RolePermission : BaseEntity
    {
        public S04_RolePermission()
        {

        }
        /// <summary>
        /// Desc:角色权限Id
        /// Default:
        /// Nullable:False
        /// </summary>

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long S04_RolePermissionId { get; set; }
        /// <summary>
        /// Desc:模块Id
        /// Default:
        /// Nullable:False
        /// </summary>

        public long S02_ModuleId { get; set; }
        /// <summary>
        /// Desc:角色Id
        /// Default:
        /// Nullable:False
        /// </summary>

        public long S03_RoleId { get; set; }
        /// <summary>
        /// Desc:权限名称
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S04_PermissionName { get; set; }
        /// <summary>
        /// Desc:创建者Id
        /// Default:
        /// Nullable:False
        /// </summary>

        public long S04_CreateId { get; set; }
        /// <summary>
        /// Desc:创建者名称
        /// Default:
        /// Nullable:False
        /// </summary>

        public string S04_CreateBy { get; set; }
        /// <summary>
        /// Desc:创建日期
        /// Default:
        /// Nullable:False
        /// </summary>

        public DateTime S04_CreateTime { get; set; }
        /// <summary>
        /// Desc:更新者Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? S04_ModifyId { get; set; }
        /// <summary>
        /// Desc:更新者名称
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S04_ModifyBy { get; set; }
        /// <summary>
        /// Desc:更新日期
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? S04_ModifyTime { get; set; }
    }
}
