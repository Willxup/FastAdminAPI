using System.ComponentModel;

namespace FastAdminAPI.Common.Enums
{
    /// <summary>
    /// 通用申请枚举
    /// </summary>
    public class ApplicationEnums
    {
        /// <summary>
        /// 是否审批通过 0不通过 1通过
        /// </summary>
        public enum IsApprove
        {
            /// <summary>
            /// 审批不通过
            /// </summary>
            NotApprove,
            /// <summary>
            /// 审批通过
            /// </summary>
            Approved
        }
        /// <summary>
        /// 审批类型 0上级 1指定人员 2自选 3上级+指定人员 4上级+指定人员+金额
        /// </summary>
        public enum ApproveType
        {
            /// <summary>
            /// 上级
            /// </summary>
            Superior,
            /// <summary>
            /// 指定人员
            /// </summary>
            Designee,
            /// <summary>
            /// 自选
            /// </summary>
            Customize,
            /// <summary>
            /// 上级+指定人员
            /// </summary>
            SuperiorAndDesignee,
            /// <summary>
            /// 上级+指定人员+金额
            /// </summary>
            SuperiorAndDesigneeWithAmount
        }

        /// <summary>
        /// 申请类别
        /// </summary>
        public enum ApplicationCategory
        {
            /// <summary>
            /// 测试
            /// </summary>
            [Description("测试")]
            Test
        }
        /// <summary>
        /// 申请类型 S99Code
        /// </summary>
        public enum ApplicationType
        {
            /// <summary>
            /// 测试
            /// </summary>
            Test = 1,
        }
    }
}
