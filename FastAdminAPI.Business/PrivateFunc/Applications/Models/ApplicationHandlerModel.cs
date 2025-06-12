using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FastAdminAPI.Framework.Entities;

namespace FastAdminAPI.Business.PrivateFunc.Applications.Models
{
    /// <summary>
    /// 申请类
    /// </summary>
    public class SetApplicationModel
    {
        /// <summary>
        /// 申请Id 与业务表主键Id关联
        /// </summary>
        [Required(ErrorMessage = "申请Id不能为空!")]
        public long? ApplicationId { get; set; }
        /// <summary>
        /// 申请类别
        /// </summary>
        [Required(ErrorMessage = "申请类别不能为空!")]
        public byte? ApplicationCategory { get; set; }
        /// <summary>
        /// 申请类型
        /// </summary>
        [Required(ErrorMessage = "申请类型不能为空!")]
        public long? ApplicationType { get; set; }

        /// <summary>
        /// 操作人Id
        /// </summary>
        [Required(ErrorMessage = "操作人Id不能为空!")]
        public long? OperationId { get; set; }
        /// <summary>
        /// 操作人名称
        /// </summary>
        [Required(ErrorMessage = "操作人名称不能为空!")]
        public string OperationName { get; set; }

        /// <summary>
        /// 申请描述
        /// </summary>
        [Required(ErrorMessage = "申请描述不能为空!")]
        public string Description { get; set; }
        /// <summary>
        /// 申请理由
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 总金额 【上级+指定人员+金额】专用
        /// </summary>
        public double? TotalAmount { get; set; }

        /// <summary>
        /// 完成申请时所需的数据包
        /// </summary>
        public string DataContent { get; set; }
        /// <summary>
        /// 审批人列表
        /// </summary>
        public List<long> ApproverList { get; set; }
        /// <summary>
        /// 抄送人列表
        /// </summary>
        public List<long> CarbonCopiesList { get; set; }
        /// <summary>
        /// 公共数据内容
        /// </summary>
        public ApplicationCommonDataModel CommonDataContent { get; set; } = null;

    }
    /// <summary>
    /// 申请通用类
    /// </summary>
    public class ApplicationCommonDataModel
    {
        /// <summary>
        /// 企业微信通知标题
        /// #checkId# 替换为审批Id
        /// </summary>
        public string QyWechatNotifyTitle { get; set; } = null;
        /// <summary>
        /// 企业微信通知描述 
        /// #checkId# 替换为审批Id
        /// </summary>
        public string QyWechatNotifyDescription { get; set; } = null;
        /// <summary>
        /// 企业微信通知地址
        /// </summary>
        public string QyWechatNotifyUrl { get; set; } = null;
    }
    /// <summary>
    /// 审批相关数据redis类
    /// </summary>
    public class ApprovalDataByRedisModel
    {
        /// <summary>
        /// 审批人Id
        /// </summary>
        public long ApproverId { get; set; }
        /// <summary>
        /// 审批人名称
        /// </summary>
        public string ApproverName { get; set; }
        /// <summary>
        /// 企业微信UserId
        /// </summary>
        public string QyUserId { get; set; }
        /// <summary>
        /// 所有审批人数据
        /// </summary>
        public string ApproversData { get; set; }
        /// <summary>
        /// 所有抄送人数据
        /// </summary>
        public string CarbonCopiesData { get; set; }
    }
    /// <summary>
    /// 审批类
    /// </summary>
    public class ProcessingApplicationModel
    {
        /// <summary>
        /// 审批实体类
        /// </summary>
        public S12_Check Check { get; set; }
        /// <summary>
        /// 是否审批通过 0否 1是
        /// </summary>
        [Required(ErrorMessage = "是否审批通过不能为空!")]
        public int? IsApprove { get; set; }
        /// <summary>
        /// 审批理由
        /// </summary>
        public string Reason { get; set; }
        /// <summary>
        /// 操作人Id
        /// </summary>
        [Required(ErrorMessage = "操作人Id不能为空!")]
        public long? OperationId { get; set; }
        /// <summary>
        /// 操作人名称
        /// </summary>
        [Required(ErrorMessage = "操作人名称不能为空!")]
        public string OperationName { get; set; }

    }
    /// <summary>
    /// 审批人类
    /// </summary>
    public class ApproverInfoModel
    {
        /// <summary>
        /// 员工Id
        /// </summary>
        public long? EmployeeId { get; set; }
        /// <summary>
        /// 员工名称
        /// </summary>
        public string EmployeeName { get; set; }
        /// <summary>
        /// 企业微信UserId
        /// </summary>
        public string QyUserId { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>
        public int? Priority { get; set; }
        /// <summary>
        /// 是否完成审批
        /// </summary>
        public bool IsFinishApproved { get; set; } = false;
    }
    /// <summary>
    /// 审批抄送人类
    /// </summary>
    public class CarbonCopiesInfoModel
    {
        /// <summary>
        /// 员工Id
        /// </summary>
        public long? EmployeeId { get; set; }
        /// <summary>
        /// 员工名称
        /// </summary>
        public string EmployeeName { get; set; }
        /// <summary>
        /// 企业微信UserId
        /// </summary>
        public string QyUserId { get; set; }
    }
    /// <summary>
    /// 申请且审批完成类
    /// </summary>
    public class CompleteApplicationModel
    {
        /// <summary>
        /// 申请类别
        /// </summary>
        [Required(ErrorMessage = "申请类别不能为空!")]
        public byte? ApplicationCategory { get; set; }
        /// <summary>
        /// 申请类型
        /// </summary>
        [Required(ErrorMessage = "申请类型不能为空!")]
        public long? ApplicationType { get; set; }
        /// <summary>
        /// 完成申请时所需数据Json包
        /// </summary>
        public string DataContent { get; set; }
        /// <summary>
        /// 申请Id 与业务表主键Id关联
        /// </summary>
        public long ApplicationId { get; set; }
    }
}
