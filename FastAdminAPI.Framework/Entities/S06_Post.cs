
using SqlSugar;
using System;


namespace FastAdminAPI.Framework.Entities
{
    ///<summary>
    ///岗位
    ///</summary>
    [SugarTable("S06_Post")]
    [Serializable]
    public partial class S06_Post : BaseEntity
    {
        public S06_Post()
        {
            this.S06_IsValid = Convert.ToByte("0");

        }
        /// <summary>
        /// Desc:岗位Id
        /// Default:
        /// Nullable:False
        /// </summary>

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long S06_PostId { get; set; }
        /// <summary>
        /// Desc:岗位名称
        /// Default:
        /// Nullable:False
        /// </summary>

        public string S06_PostName { get; set; }
        /// <summary>
        /// Desc:上级岗位Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? S06_ParentPostId { get; set; }
        /// <summary>
        /// Desc:部门Id
        /// Default:
        /// Nullable:False
        /// </summary>

        public long S05_DepartId { get; set; }
        /// <summary>
        /// Desc:角标 每一级4位数字
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S06_CornerMark { get; set; }
        /// <summary>
        /// Desc:岗位编制
        /// Default:
        /// Nullable:True
        /// </summary>

        public int S06_MaxEmployeeNums { get; set; }
        /// <summary>
        /// Desc:岗位职责
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S06_Responsibility { get; set; }
        /// <summary>
        /// Desc:能力需求
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S06_AbilityDemand { get; set; }
        /// <summary>
        /// Desc:是否有效 0有效 1无效
        /// Default:0
        /// Nullable:False
        /// </summary>

        public byte S06_IsValid { get; set; }
        /// <summary>
        /// Desc:创建者Id
        /// Default:
        /// Nullable:False
        /// </summary>

        public long S06_CreateId { get; set; }
        /// <summary>
        /// Desc:创建者名称
        /// Default:
        /// Nullable:False
        /// </summary>

        public string S06_CreateBy { get; set; }
        /// <summary>
        /// Desc:创建日期
        /// Default:
        /// Nullable:False
        /// </summary>

        public DateTime S06_CreateTime { get; set; }
        /// <summary>
        /// Desc:更新者Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? S06_ModifyId { get; set; }
        /// <summary>
        /// Desc:更新者名称
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S06_ModifyBy { get; set; }
        /// <summary>
        /// Desc:更新日期
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? S06_ModifyTime { get; set; }
        /// <summary>
        /// Desc:删除者Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? S06_DeleteId { get; set; }
        /// <summary>
        /// Desc:删除者名称
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S06_DeleteBy { get; set; }
        /// <summary>
        /// Desc:删除日期
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? S06_DeleteTime { get; set; }
    }
}
