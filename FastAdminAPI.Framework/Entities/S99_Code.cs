
using SqlSugar;
using System;


namespace FastAdminAPI.Framework.Entities
{
    ///<summary>
    ///系统字典
    ///</summary>
    [SugarTable("S99_Code")]
    [Serializable]
    public partial class S99_Code : BaseEntity
    {
        public S99_Code()
        {
            this.S99_SysFlag = Convert.ToByte("0");
            this.S99_IsValid = Convert.ToByte("0");

        }
        /// <summary>
        /// Desc:系统字典Id
        /// Default:
        /// Nullable:False
        /// </summary>

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long S99_CodeId { get; set; }
        /// <summary>
        /// Desc:分组代号
        /// Default:
        /// Nullable:False
        /// </summary>

        public string S99_GroupCode { get; set; }
        /// <summary>
        /// Desc:分组名称
        /// Default:
        /// Nullable:False
        /// </summary>

        public string S99_GroupName { get; set; }
        /// <summary>
        /// Desc:字典名称
        /// Default:
        /// Nullable:False
        /// </summary>

        public string S99_Name { get; set; }
        /// <summary>
        /// Desc:父级Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? S99_ParentCodeId { get; set; }
        /// <summary>
        /// Desc:序号 组内排序使用
        /// Default:
        /// Nullable:True
        /// </summary>

        public int? S99_SeqNo { get; set; }
        /// <summary>
        /// Desc:系统标记 0系统 1用户
        /// Default:0
        /// Nullable:False
        /// </summary>

        public byte S99_SysFlag { get; set; }
        /// <summary>
        /// Desc:是否有效 0有效 1无效
        /// Default:0
        /// Nullable:False
        /// </summary>

        public byte S99_IsValid { get; set; }
        /// <summary>
        /// Desc:创建者Id
        /// Default:
        /// Nullable:False
        /// </summary>

        public long S99_CreateId { get; set; }
        /// <summary>
        /// Desc:创建者名称
        /// Default:
        /// Nullable:False
        /// </summary>

        public string S99_CreateBy { get; set; }
        /// <summary>
        /// Desc:创建日期
        /// Default:
        /// Nullable:False
        /// </summary>

        public DateTime S99_CreateTime { get; set; }
        /// <summary>
        /// Desc:更新者Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? S99_ModifyId { get; set; }
        /// <summary>
        /// Desc:更新者名称
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S99_ModifyBy { get; set; }
        /// <summary>
        /// Desc:更新日期
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? S99_ModifyTime { get; set; }
        /// <summary>
        /// Desc:删除者Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? S99_DeleteId { get; set; }
        /// <summary>
        /// Desc:删除者名称
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S99_DeleteBy { get; set; }
        /// <summary>
        /// Desc:删除日期
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? S99_DeleteTime { get; set; }
    }
}
