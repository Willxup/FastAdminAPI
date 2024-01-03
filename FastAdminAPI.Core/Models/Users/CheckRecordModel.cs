using FastAdminAPI.Framework.Extensions.DbQueryExtensions;
using System;

namespace FastAdminAPI.Core.Models.Users
{
    public class CheckRecordModel
    {
        /// <summary>
        /// 审批记录Id
        /// </summary>
        [DbQueryField("S13_CheckRecordId")]
        public long? CheckRecordId { get; set; }
        /// <summary>
        /// 审批Id
        /// </summary>
        [DbQueryField("S12_CheckId")]
        public long? CheckId { get; set; }
        /// <summary>
        /// 审批人Id
        /// </summary>
        [DbQueryField("S07_ApproverId")]
        public long? ApproverId { get; set; }
        /// <summary>
        /// 审批人名称
        /// </summary>
        [DbQueryField("S13_ApproverName")]
        public string ApproverName { get; set; }
        /// <summary>
        /// 审批日期
        /// </summary>
        [DbQueryField("S13_ApprovalTime")]
        public DateTime? ApprovalTime { get; set; }
        /// <summary>
        /// 是否通过 -1待审批 0否 1是
        /// </summary>
        [DbQueryField("S13_IsApprove")]
        public int? IsApprove { get; set; }
        /// <summary>
        /// 理由
        /// </summary>
        [DbQueryField("S13_Reason")]
        public string Reason { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [DbQueryField("S13_Remark")]
        public string Remark { get; set; }

    }
}
