
using System;
using SqlSugar;


namespace FastAdminAPI.Framework.Entities
{
    ///<summary>
    ///角色
    ///</summary>
    [SugarTable("S03_Role")]
    [Serializable]
    public partial class S03_Role : BaseEntity
    {
        public S03_Role()
        {
            this.S03_IsDelete = Convert.ToByte("0");

        }
        /// <summary>
        /// Desc:角色Id
        /// Default:
        /// Nullable:False
        /// </summary>

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long S03_RoleId { get; set; }
        /// <summary>
        /// Desc:父角色Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? S03_ParentRoleId { get; set; }
        /// <summary>
        /// Desc:名称
        /// Default:
        /// Nullable:False
        /// </summary>

        public string S03_RoleName { get; set; }
        /// <summary>
        /// Desc:角标 每一级4位数字
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S03_CornerMark { get; set; }
        /// <summary>
        /// Desc:是否删除 0否 1是
        /// Default:0
        /// Nullable:False
        /// </summary>

        public byte S03_IsDelete { get; set; }
        /// <summary>
        /// Desc:创建者Id
        /// Default:
        /// Nullable:False
        /// </summary>

        public long S03_CreateId { get; set; }
        /// <summary>
        /// Desc:创建者名称
        /// Default:
        /// Nullable:False
        /// </summary>

        public string S03_CreateBy { get; set; }
        /// <summary>
        /// Desc:创建日期
        /// Default:
        /// Nullable:False
        /// </summary>

        public DateTime S03_CreateTime { get; set; }
        /// <summary>
        /// Desc:更新者Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? S03_ModifyId { get; set; }
        /// <summary>
        /// Desc:更新者名称
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S03_ModifyBy { get; set; }
        /// <summary>
        /// Desc:更新日期
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? S03_ModifyTime { get; set; }
        /// <summary>
        /// Desc:删除者Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? S03_DeleteId { get; set; }
        /// <summary>
        /// Desc:删除者名称
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S03_DeleteBy { get; set; }
        /// <summary>
        /// Desc:删除日期
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? S03_DeleteTime { get; set; }
    }
}
