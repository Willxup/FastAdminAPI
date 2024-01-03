
using SqlSugar;
using System;


namespace FastAdminAPI.Framework.Entities
{
    ///<summary>
    ///数据权限
    ///</summary>
    [SugarTable("S10_DataPermission")]
    [Serializable]
    public partial class S10_DataPermission : BaseEntity
    {
        public S10_DataPermission()
        {

        }
        /// <summary>
        /// Desc:数据权限Id
        /// Default:
        /// Nullable:False
        /// </summary>

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long S10_DataPermissionId { get; set; }
        /// <summary>
        /// Desc:员工Id
        /// Default:
        /// Nullable:False
        /// </summary>

        public long S07_EmployeeId { get; set; }
        /// <summary>
        /// Desc:部门Id集合 多个以|分隔
        /// Default:
        /// Nullable:False
        /// </summary>

        public string S05_DepartIds { get; set; }
        /// <summary>
        /// Desc:创建者Id
        /// Default:
        /// Nullable:False
        /// </summary>

        public long S10_CreateId { get; set; }
        /// <summary>
        /// Desc:创建者名称
        /// Default:
        /// Nullable:False
        /// </summary>

        public string S10_CreateBy { get; set; }
        /// <summary>
        /// Desc:创建日期
        /// Default:
        /// Nullable:False
        /// </summary>

        public DateTime S10_CreateTime { get; set; }
        /// <summary>
        /// Desc:更新者Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? S10_ModifyId { get; set; }
        /// <summary>
        /// Desc:更新者名称
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S10_ModifyBy { get; set; }
        /// <summary>
        /// Desc:更新日期
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? S10_ModifyTime { get; set; }
    }
}
