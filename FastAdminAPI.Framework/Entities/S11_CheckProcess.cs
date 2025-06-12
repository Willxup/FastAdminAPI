
using System;
using SqlSugar;


namespace FastAdminAPI.Framework.Entities
{
    ///<summary>
    ///审批流程
    ///</summary>
    [SugarTable("S11_CheckProcess")]
    [Serializable]
    public partial class S11_CheckProcess : BaseEntity
    {
        public S11_CheckProcess()
        {
            this.S11_IsDelete = Convert.ToByte("0");

        }
        /// <summary>
        /// Desc:审批流程Id
        /// Default:
        /// Nullable:False
        /// </summary>

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long S11_CheckProcessId { get; set; }
        /// <summary>
        /// Desc:审批类型 0直接上级 1指定人员 2自选 3上级+指定人员 4上级+指定人员+金额
        /// Default:
        /// Nullable:False
        /// </summary>

        public byte S11_ApproveType { get; set; }
        /// <summary>
        /// Desc:申请类型 关联S99_Code表
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? S99_ApplicationType { get; set; }
        /// <summary>
        /// Desc:申请人 使用,隔开
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S07_Applicants { get; set; }
        /// <summary>
        /// Desc:审核人 使用,隔开
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S07_Approvers { get; set; }
        /// <summary>
        /// Desc:抄送人 使用,隔开
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S07_CarbonCopies { get; set; }
        /// <summary>
        /// Desc:审批金额上限 使用,隔开
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S11_Amounts { get; set; }
        /// <summary>
        /// Desc:备注
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S11_Remark { get; set; }
        /// <summary>
        /// Desc:是否删除 0否 1是
        /// Default:0
        /// Nullable:False
        /// </summary>

        public byte S11_IsDelete { get; set; }
        /// <summary>
        /// Desc:创建者Id
        /// Default:
        /// Nullable:False
        /// </summary>

        public long S11_CreateId { get; set; }
        /// <summary>
        /// Desc:创建者名称
        /// Default:
        /// Nullable:False
        /// </summary>

        public string S11_CreateBy { get; set; }
        /// <summary>
        /// Desc:创建日期
        /// Default:
        /// Nullable:False
        /// </summary>

        public DateTime S11_CreateTime { get; set; }
        /// <summary>
        /// Desc:更新者Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? S11_ModifyId { get; set; }
        /// <summary>
        /// Desc:更新者名称
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S11_ModifyBy { get; set; }
        /// <summary>
        /// Desc:更新日期
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? S11_ModifyTime { get; set; }
        /// <summary>
        /// Desc:删除者Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? S11_DeleteId { get; set; }
        /// <summary>
        /// Desc:删除者名称
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S11_DeleteBy { get; set; }
        /// <summary>
        /// Desc:删除日期
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? S11_DeleteTime { get; set; }
    }
}
