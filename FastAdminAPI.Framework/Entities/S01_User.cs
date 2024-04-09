
using SqlSugar;
using System;


namespace FastAdminAPI.Framework.Entities
{
    ///<summary>
    ///用户表
    ///</summary>
    [SugarTable("S01_User")]
    [Serializable]
    public partial class S01_User : BaseEntity
    {
        public S01_User()
        {
            this.S01_AccountStatus = Convert.ToByte("0");
            this.S01_isDelete = Convert.ToByte("0");

        }
        /// <summary>
        /// Desc:用户Id
        /// Default:
        /// Nullable:False
        /// </summary>

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long S01_UserId { get; set; }
        /// <summary>
        /// Desc:用户账号
        /// Default:
        /// Nullable:False
        /// </summary>

        public string S01_Account { get; set; }
        /// <summary>
        /// Desc:用户密码
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S01_Password { get; set; }
        /// <summary>
        /// Desc:账号状态 0启用 1禁用
        /// Default:0
        /// Nullable:False
        /// </summary>

        public byte S01_AccountStatus { get; set; }
        /// <summary>
        /// Desc:是否删除 0否 1是
        /// Default:0
        /// Nullable:False
        /// </summary>

        public byte S01_isDelete { get; set; }
        /// <summary>
        /// Desc:创建者Id
        /// Default:
        /// Nullable:False
        /// </summary>

        public long S01_CreateId { get; set; }
        /// <summary>
        /// Desc:创建者名称
        /// Default:
        /// Nullable:False
        /// </summary>

        public string S01_CreateBy { get; set; }
        /// <summary>
        /// Desc:创建日期
        /// Default:
        /// Nullable:False
        /// </summary>

        public DateTime S01_CreateTime { get; set; }
        /// <summary>
        /// Desc:更新者Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? S01_ModifyId { get; set; }
        /// <summary>
        /// Desc:更新者名称
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S01_ModifyBy { get; set; }
        /// <summary>
        /// Desc:更新日期
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? S01_ModifyTime { get; set; }
        /// <summary>
        /// Desc:删除者Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? S01_DeleteId { get; set; }
        /// <summary>
        /// Desc:删除者名称
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S01_DeleteBy { get; set; }
        /// <summary>
        /// Desc:删除日期
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? S01_DeleteTime { get; set; }
    }
}
