
using System;
using SqlSugar;


namespace FastAdminAPI.Framework.Entities
{
    ///<summary>
    ///用户权限
    ///</summary>
    [SugarTable("S09_UserPermission")]
    [Serializable]
    public partial class S09_UserPermission : BaseEntity
    {
        public S09_UserPermission()
        {

        }
        /// <summary>
        /// Desc:用户权限Id
        /// Default:
        /// Nullable:False
        /// </summary>

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long S09_UserPermissionId { get; set; }
        /// <summary>
        /// Desc:用户Id
        /// Default:
        /// Nullable:False
        /// </summary>

        public long S01_UserId { get; set; }
        /// <summary>
        /// Desc:权限类型 0角色 1用户
        /// Default:
        /// Nullable:False
        /// </summary>

        public byte S09_PermissionType { get; set; }
        /// <summary>
        /// Desc:通用Id 角色Id/模块Id
        /// Default:
        /// Nullable:False
        /// </summary>

        public long S09_CommonId { get; set; }
        /// <summary>
        /// Desc:权限名称
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S09_PermissionName { get; set; }
        /// <summary>
        /// Desc:创建者Id
        /// Default:
        /// Nullable:False
        /// </summary>

        public long S09_CreateId { get; set; }
        /// <summary>
        /// Desc:创建者名称
        /// Default:
        /// Nullable:False
        /// </summary>

        public string S09_CreateBy { get; set; }
        /// <summary>
        /// Desc:创建日期
        /// Default:
        /// Nullable:False
        /// </summary>

        public DateTime S09_CreateTime { get; set; }
        /// <summary>
        /// Desc:更新者Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? S09_ModifyId { get; set; }
        /// <summary>
        /// Desc:更新者名称
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S09_ModifyBy { get; set; }
        /// <summary>
        /// Desc:更新日期
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? S09_ModifyTime { get; set; }
    }
}
