
using System;
using SqlSugar;


namespace FastAdminAPI.Framework.Entities
{
    ///<summary>
    ///审批
    ///</summary>
    [SugarTable("S12_Check")]
    [Serializable]
    public partial class S12_Check : BaseEntity
    {
        public S12_Check()
        {
            this.S12_IsFinishCheck = Convert.ToByte("0");
            this.S12_IsDelete = Convert.ToByte("0");

        }
        /// <summary>
        /// Desc:审批Id
        /// Default:
        /// Nullable:False
        /// </summary>

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long S12_CheckId { get; set; }
        /// <summary>
        /// Desc:申请Id 与业务数据关联
        /// Default:
        /// Nullable:False
        /// </summary>

        public long S12_ApplicationId { get; set; }
        /// <summary>
        /// Desc:审批流程Id
        /// Default:
        /// Nullable:False
        /// </summary>

        public long S11_CheckProcessId { get; set; }
        /// <summary>
        /// Desc:申请类别 0项目管理 1院校管理 2营销管理 3线索管理 4客户管理
        /// Default:
        /// Nullable:False
        /// </summary>

        public byte S12_ApplicationCategory { get; set; }
        /// <summary>
        /// Desc:申请类型 关联S99_Code表
        /// Default:
        /// Nullable:False
        /// </summary>

        public long S99_ApplicationType { get; set; }
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

        public string S12_ApproverName { get; set; }
        /// <summary>
        /// Desc:审批人json
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S12_ApproversData { get; set; }
        /// <summary>
        /// Desc:抄送人json
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S12_CarbonCopiesData { get; set; }
        /// <summary>
        /// Desc:公有数据内容json
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S12_CommonDataContent { get; set; }
        /// <summary>
        /// Desc:私有数据内容json
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S12_PrivateDataContent { get; set; }
        /// <summary>
        /// Desc:是否完成审批 0否 1是
        /// Default:0
        /// Nullable:False
        /// </summary>

        public byte S12_IsFinishCheck { get; set; }
        /// <summary>
        /// Desc:申请详情
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S12_ApplicationInfo { get; set; }
        /// <summary>
        /// Desc:申请理由
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S12_Reason { get; set; }
        /// <summary>
        /// Desc:备注
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S12_Remark { get; set; }
        /// <summary>
        /// Desc:是否删除 0否 1是
        /// Default:0
        /// Nullable:False
        /// </summary>

        public byte S12_IsDelete { get; set; }
        /// <summary>
        /// Desc:创建者Id
        /// Default:
        /// Nullable:False
        /// </summary>

        public long S12_CreateId { get; set; }
        /// <summary>
        /// Desc:创建者名称
        /// Default:
        /// Nullable:False
        /// </summary>

        public string S12_CreateBy { get; set; }
        /// <summary>
        /// Desc:创建日期
        /// Default:
        /// Nullable:False
        /// </summary>

        public DateTime S12_CreateTime { get; set; }
        /// <summary>
        /// Desc:更新者Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? S12_ModifyId { get; set; }
        /// <summary>
        /// Desc:更新者名称
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S12_ModifyBy { get; set; }
        /// <summary>
        /// Desc:更新日期
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? S12_ModifyTime { get; set; }
        /// <summary>
        /// Desc:删除者Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? S12_DeleteId { get; set; }
        /// <summary>
        /// Desc:删除者名称
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S12_DeleteBy { get; set; }
        /// <summary>
        /// Desc:删除日期
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? S12_DeleteTime { get; set; }
    }
}
