using FastAdminAPI.Common.Enums;
using FastAdminAPI.Framework.Extensions.Models;
using Newtonsoft.Json;
using SqlSugar.Attributes.Extension.Extensions;
using SqlSugar.Attributes.Extension.Extensions.Attributes.Operation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FastAdminAPI.Core.Models.BasicSettings
{
    #region 查询
    public class CheckProcessPageSearch : DbQueryBaseModel
    {
        /// <summary>
        /// 申请类型 关联S99_Code表
        /// </summary>
        public List<long> ApplicationType { get; set; }
        /// <summary>
        /// 审批类型 0直接上级 1指定人员 2自选 3上级+指定人员
        /// </summary>
        public List<byte> ApproveTypes { get; set; }
    }
    public class CheckProcessPageResult
    {
        /// <summary>
        /// 审批流程Id
        /// </summary>
        public long CheckProcessId { get; set; }
        /// <summary>
        /// 申请类型 关联S99_Code表
        /// </summary>
        public long? ApplicationType { get; set; }
        /// <summary>
        /// 申请类型名称
        /// </summary>
        public string ApplicationTypeName { get; set; }
        /// <summary>
        /// 申请人 
        /// </summary>
        public string Applicants { get; set; }
        /// <summary>
        /// 申请人名称
        /// </summary>
        public string ApplicantName { get; set; }
        /// <summary>
        /// 审核人
        /// </summary>
        public string Approvers { get; set; }
        /// <summary>
        /// 审核人名称
        /// </summary>
        public string ApproverName { get; set; }
        /// <summary>
        /// 抄送人 
        /// </summary>
        public string CarbonCopies { get; set; }
        /// <summary>
        /// 抄送人名称
        /// </summary>
        public string CarbonCopieName { get; set; }
        /// <summary>
        /// 审批金额上限 使用,隔开
        /// </summary>
        public string Amounts { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 审批类型 0直接上级 1指定人员 2自选 3上级+指定人员 4上级+指定人员+金额
        /// </summary>
        public byte? ApproveType { get; set; }
    }
    #endregion

    #region 操作
    public class CheckProcessBaseModel : DbOperationBaseModel
    {
        /// <summary>
        /// 申请类型 关联S99_Code表
        /// </summary>
        [Required(ErrorMessage = "申请类型不能为空!")]
        [DbOperationField("S99_ApplicationType")]
        public long? ApplicationType { get; set; }

        /// <summary>
        /// 审批类型 0直接上级 1指定人员 2自选 3上级+指定人员 4上级+指定人员+金额
        /// </summary>
        [Required(ErrorMessage = "审批类型不能为空!")]
        [DbOperationField("S11_ApproveType")]
        public byte ApproveType { get; set; }

        /// <summary>
        /// 申请人 
        /// </summary>
        [DbIgnoreField]
        public List<string> ApplicantList { get; set; }
        /// <summary>
        /// 申请人 
        /// </summary>
        [JsonIgnore]
        [DbOperationField("S07_Applicants", false, true)]
        public string Applicants { get; set; } = null;

        /// <summary>
        /// 审批人 
        /// </summary>
        [DbIgnoreField]
        public List<string> ApproverList { get; set; }
        /// <summary>
        /// 审批人 
        /// </summary>
        [JsonIgnore]
        [DbOperationField("S07_Approvers", false, true)]
        public string Approvers { get; set; } = null;

        /// <summary>
        /// 抄送人
        /// </summary>
        [DbIgnoreField]
        public List<string> CarbonCopieList { get; set; }
        /// <summary>
        /// 抄送人
        /// </summary>
        [JsonIgnore]
        [DbOperationField("S07_CarbonCopies", false, true)]
        public string CarbonCopies { get; set; } = null;

        /// <summary>
        /// 审批金额上限
        /// </summary>
        [DbIgnoreField]
        public List<double> AmountList { get; set; }
        /// <summary>
        /// 审批金额上限 使用,隔开
        /// </summary>
        [JsonIgnore]
        [DbOperationField("S11_Amounts", false, true)]
        public string Amounts { get; set; } = null;

        /// <summary>
        /// 备注
        /// </summary>
        [DbOperationField("S11_Remark",false,true)]
        public string Remark { get; set; }
    }
    public class AddCheckProcessModel : CheckProcessBaseModel
    {
        [JsonIgnore]
        [DbOperationField("S11_IsDelete")]
        public byte IsDelete { get; private set; } = (byte)BaseEnums.TrueOrFalse.False;
        [JsonIgnore]
        [DbOperationField("S11_CreateId")]
        public long? OperationId { get; set; }
        [JsonIgnore]
        [DbOperationField("S11_CreateBy")]
        public string OperationName { get; set; }
        [JsonIgnore]
        [DbOperationField("S11_CreateTime")]
        public DateTime? OperationTime { get; set; }
    }
    public class EditCheckProcessModel : CheckProcessBaseModel
    {
        /// <summary>
        /// 审批流程Id
        /// </summary>
        [Required(ErrorMessage = "审批流程Id不能为空!")]
        [DbOperationField("S11_CheckProcessId", true)]
        public long? CheckProcessId { get; set; }
        [JsonIgnore]
        [DbOperationField("S11_ModifyId")]
        public long? OperationId { get; set; }
        [JsonIgnore]
        [DbOperationField("S11_ModifyBy")]
        public string OperationName { get; set; }
        [JsonIgnore]
        [DbOperationField("S11_ModifyTime")]
        public DateTime? OperationTime { get; set; }
    }
    #endregion
}
