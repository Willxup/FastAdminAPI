
using SqlSugar;
using System;


namespace FastAdminAPI.Framework.Entities
{
    ///<summary>
    ///部门
    ///</summary>
    [SugarTable("S05_Department")]
    [Serializable]
    public partial class S05_Department : BaseEntity
    {
        public S05_Department()
        {
            this.S05_IsDelete = Convert.ToByte("0");

        }
        /// <summary>
        /// Desc:部门Id
        /// Default:
        /// Nullable:False
        /// </summary>

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long S05_DepartId { get; set; }
        /// <summary>
        /// Desc:部门名称
        /// Default:
        /// Nullable:False
        /// </summary>

        public string S05_DepartName { get; set; }
        /// <summary>
        /// Desc:上级部门Id
        /// Default:
        /// Nullable:False
        /// </summary>

        public long S05_ParentDepartId { get; set; }
        /// <summary>
        /// Desc:角标 每一级4位数字
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S05_CornerMark { get; set; }
        /// <summary>
        /// Desc:部门属性 S99_Code
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? S05_Property { get; set; }
        /// <summary>
        /// Desc:部门标签 -1无 0线索 1客户 2代理，多个以,号分隔
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S05_Label { get; set; }
        /// <summary>
        /// Desc:优先级
        /// Default:
        /// Nullable:True
        /// </summary>

        public int? S05_Priority { get; set; }
        /// <summary>
        /// Desc:是否删除 0否 1是
        /// Default:0
        /// Nullable:False
        /// </summary>

        public byte S05_IsDelete { get; set; }
        /// <summary>
        /// Desc:创建者Id
        /// Default:
        /// Nullable:False
        /// </summary>

        public long S05_CreateId { get; set; }
        /// <summary>
        /// Desc:创建者名称
        /// Default:
        /// Nullable:False
        /// </summary>

        public string S05_CreateBy { get; set; }
        /// <summary>
        /// Desc:创建日期
        /// Default:
        /// Nullable:False
        /// </summary>

        public DateTime S05_CreateTime { get; set; }
        /// <summary>
        /// Desc:更新者Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? S05_ModifyId { get; set; }
        /// <summary>
        /// Desc:更新者名称
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S05_ModifyBy { get; set; }
        /// <summary>
        /// Desc:更新日期
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? S05_ModifyTime { get; set; }
        /// <summary>
        /// Desc:删除者Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? S05_DeleteId { get; set; }
        /// <summary>
        /// Desc:删除者名称
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S05_DeleteBy { get; set; }
        /// <summary>
        /// Desc:删除日期
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? S05_DeleteTime { get; set; }
    }
}
