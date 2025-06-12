
using System;
using SqlSugar;


namespace FastAdminAPI.Framework.Entities
{
    ///<summary>
    ///审批记录表
    ///</summary>
    [SugarTable("S13_CheckRecords")]
    [Serializable]
    public partial class S13_CheckRecords : BaseEntity
    {
        public S13_CheckRecords()
        {

        }
        /// <summary>
        /// Desc:审批记录Id
        /// Default:
        /// Nullable:False
        /// </summary>

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long S13_CheckRecordId { get; set; }
        /// <summary>
        /// Desc:审批Id
        /// Default:
        /// Nullable:False
        /// </summary>

        public long S12_CheckId { get; set; }
        /// <summary>
        /// Desc:审批人Id
        /// Default:
        /// Nullable:False
        /// </summary>

        public long S07_ApproverId { get; set; }
        /// <summary>
        /// Desc:审批人名称
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S13_ApproverName { get; set; }
        /// <summary>
        /// Desc:审批日期
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? S13_ApprovalTime { get; set; }
        /// <summary>
        /// Desc:是否通过 0否 1是
        /// Default:
        /// Nullable:False
        /// </summary>

        public byte S13_IsApprove { get; set; }
        /// <summary>
        /// Desc:理由
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S13_Reason { get; set; }
        /// <summary>
        /// Desc:备注
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S13_Remark { get; set; }
        /// <summary>
        /// Desc:创建者Id
        /// Default:
        /// Nullable:False
        /// </summary>

        public long S13_CreateId { get; set; }
        /// <summary>
        /// Desc:创建者名称
        /// Default:
        /// Nullable:False
        /// </summary>

        public string S13_CreateBy { get; set; }
        /// <summary>
        /// Desc:创建日期
        /// Default:
        /// Nullable:False
        /// </summary>

        public DateTime S13_CreateTime { get; set; }
        /// <summary>
        /// Desc:更新者Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? S13_ModifyId { get; set; }
        /// <summary>
        /// Desc:更新者名称
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S13_ModifyBy { get; set; }
        /// <summary>
        /// Desc:更新日期
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? S13_ModifyTime { get; set; }
    }
}
