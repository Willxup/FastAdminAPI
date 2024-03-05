using FastAdminAPI.Common.Attributes.CheckValidators;
using FastAdminAPI.Common.Enums;
using FastAdminAPI.Framework.Extensions.Models;
using SqlSugar.Attributes.Extension.Common;
using SqlSugar.Attributes.Extension.Extensions.Attributes.Query;
using System;
using System.Collections.Generic;

namespace FastAdminAPI.Core.Models.Users
{
    #region 审批

    #region 查询类
    [DbDefaultOrderBy("S12_CreateTime", DbSortWay.DESC)]
    public class CheckPageSearch : DbQueryBaseModel
    {
        /// <summary>
        /// 申请大类 0项目管理 1代理管理 2线索 3客户
        /// </summary>
        [DbQueryField("S12_ApplicationCategory")]
        [DbQueryOperator(DbOperator.In)]
        public List<long> Category { get; set; }
        /// <summary>
        /// 申请类型 S99_Code 001
        /// </summary>
        [DbQueryField("S99_ApplicationType")]
        [DbQueryOperator(DbOperator.In)]
        public List<long> Type { get; set; }
        /// <summary>
        /// 申请人Ids
        /// </summary>
        [DbQueryField("S12_CreateId")]
        [DbQueryOperator(DbOperator.In)]
        public List<long> ApplicantIds { get; set; }
        /// <summary>
        /// 申请时间 开始
        /// </summary>
        [DbQueryField("S12_CreateTime", DbTimeSuffixType.StartTime)]
        [DbQueryOperator(DbOperator.GreaterThanOrEqual)]
        public string ApplyTimeStart { get; set; }
        /// <summary>
        /// 申请时间 结束
        /// </summary>
        [DbQueryField("S12_CreateTime", DbTimeSuffixType.EndTime)]
        [DbQueryOperator(DbOperator.LessThanOrEqual)]
        public string ApplyTimeEnd { get; set; }
    }
    public class CheckPageResult
    {
        /// <summary>
        /// 审批Id
        /// </summary>
        [DbQueryField("S12_CheckId")]
        public long? CheckId { get; set; }
        /// <summary>
        /// 申请Id 与业务数据关联
        /// </summary>
        [DbQueryField("S12_ApplicationId")]
        public long? ApplicationId { get; set; }
        /// <summary>
        /// 申请大类 0项目管理 1代理管理 2线索 3客户
        /// </summary>
        [DbQueryField("S12_ApplicationCategory")]
        public long? Category { get; set; }
        /// <summary>
        /// 申请类型 S99_Code 001
        /// </summary>
        [DbQueryField("S99_ApplicationType")]
        public long? Type { get; set; }
        /// <summary>
        /// 申请类型名称
        /// </summary>
        [DbSubQuery("(SELECT S99_Name FROM S99_Code WHERE S99_CodeId = S12_Check.S99_ApplicationType)")]
        public string TypeName { get; set; }
        /// <summary>
        /// 申请人Id
        /// </summary>
        [DbQueryField("S12_CreateId")]
        public long? ApplicantId { get; set; }
        /// <summary>
        /// 申请人名称
        /// </summary>
        [DbQueryField("S12_CreateBy")]
        public string ApplicantName { get; set; }
        /// <summary>
        /// 申请时间
        /// </summary>
        [DbQueryField("S12_CreateTime")]
        public DateTime? ApplyTime { get; set; }
        /// <summary>
        /// 内容描述
        /// </summary>
        [DbQueryField("S12_ApplicationInfo")]
        public string Description { get; set; }
        /// <summary>
        /// 申请理由
        /// </summary>
        [DbQueryField("S12_Reason")]
        public string Reason { get; set; }
    }
    #endregion

    #region 操作类
    public class ApprovalModel
    {
        /// <summary>
        /// 审批Ids
        /// </summary>
        public List<long> CheckIds { get; set; }
        /// <summary>
        /// 是否通过 0否 1是
        /// </summary>
        [EnumCheck(typeof(ApplicationEnums.IsApprove))]
        public byte? IsApprove { get; set; }
        /// <summary>
        /// 审批理由
        /// </summary>
        public string Reason { get; set; }
    }
    #endregion

    #endregion

    #region 审批记录
    [DbDefaultOrderBy("S13.S13_ApprovalTime", DbSortWay.DESC)]
    public class CheckRecordPageSearch : DbQueryBaseModel
    {
        /// <summary>
        /// 申请大类 0项目管理 1代理管理 2线索 3客户
        /// </summary>
        [DbTableAlias("S12")]
        [DbQueryField("S12_ApplicationCategory")]
        [DbQueryOperator(DbOperator.In)]
        public List<long> Category { get; set; }
        /// <summary>
        /// 申请类型 S99_Code 001
        /// </summary>
        [DbTableAlias("S12")]
        [DbQueryField("S99_ApplicationType")]
        [DbQueryOperator(DbOperator.In)]
        public List<long> Type { get; set; }
        /// <summary>
        /// 申请人Ids
        /// </summary>
        [DbTableAlias("S12")]
        [DbQueryField("S12_CreateId")]
        [DbQueryOperator(DbOperator.In)]
        public List<long> ApplicantIds { get; set; }
        /// <summary>
        /// 申请时间 开始
        /// </summary>
        [DbTableAlias("S12")]
        [DbQueryField("S12_CreateTime", DbTimeSuffixType.StartTime)]
        [DbQueryOperator(DbOperator.GreaterThanOrEqual)]
        public string ApplyTimeStart { get; set; }
        /// <summary>
        /// 申请时间 结束
        /// </summary>
        [DbTableAlias("S12")]
        [DbQueryField("S12_CreateTime", DbTimeSuffixType.EndTime)]
        [DbQueryOperator(DbOperator.LessThanOrEqual)]
        public string ApplyTimeEnd { get; set; }
        /// <summary>
        /// 是否通过 0否 1是
        /// </summary>
        [DbTableAlias("S13")]
        [DbQueryField("S13_IsApprove")]
        [DbQueryOperator(DbOperator.Equal)]
        public byte? IsApprove { get; set; }
        /// <summary>
        /// 审批时间 开始
        /// </summary>
        [DbTableAlias("S13")]
        [DbQueryField("S13_ApprovalTime", DbTimeSuffixType.StartTime)]
        [DbQueryOperator(DbOperator.GreaterThanOrEqual)]
        public string ApprovalimeStart { get; set; }
        /// <summary>
        /// 审批时间 结束
        /// </summary>
        [DbTableAlias("S13")]
        [DbQueryField("S13_ApprovalTime", DbTimeSuffixType.EndTime)]
        [DbQueryOperator(DbOperator.LessThanOrEqual)]
        public string ApprovalTimeEnd { get; set; }
    }
    public class CheckRecordPageResult
    {
        /// <summary>
        /// 审批记录Id
        /// </summary>
        [DbTableAlias("S13")]
        [DbQueryField("S13_CheckRecordId")]
        public long? CheckRecordId { get; set; }
        /// <summary>
        /// 是否通过 0否 1是
        /// </summary>
        [DbTableAlias("S13")]
        [DbQueryField("S13_IsApprove")]
        public byte? IsApprove { get; set; }
        /// <summary>
        /// 审批时间
        /// </summary>
        [DbTableAlias("S13")]
        [DbQueryField("S13_ApprovalTime")]
        public DateTime? ApprovalTime { get; set; }
        /// <summary>
        /// 申请理由
        /// </summary>
        [DbTableAlias("S12")]
        [DbQueryField("S12_Reason")]
        public string ApplyReason { get; set; }
        /// <summary>
        /// 审批理由
        /// </summary>
        [DbTableAlias("S13")]
        [DbQueryField("S13_Reason")]
        public string ApprovalReason { get; set; }
        /// <summary>
        /// 审批Id
        /// </summary>
        [DbTableAlias("S13")]
        [DbQueryField("S12_CheckId")]
        public long? CheckId { get; set; }
        /// <summary>
        /// 申请Id 与业务数据关联
        /// </summary>
        [DbTableAlias("S12")]
        [DbQueryField("S12_ApplicationId")]
        public long? ApplicationId { get; set; }
        /// <summary>
        /// 申请大类 0项目管理 1代理管理 2线索 3客户
        /// </summary>
        [DbTableAlias("S12")]
        [DbQueryField("S12_ApplicationCategory")]
        public long? Category { get; set; }
        /// <summary>
        /// 申请类型 S99_Code 001
        /// </summary>
        [DbTableAlias("S12")]
        [DbQueryField("S99_ApplicationType")]
        public long? Type { get; set; }
        /// <summary>
        /// 申请类型名称
        /// </summary>
        [DbSubQuery("(SELECT S99_Name FROM S99_Code WHERE S99_CodeId = S12.S99_ApplicationType)")]
        public string TypeName { get; set; }
        /// <summary>
        /// 申请人Id
        /// </summary>
        [DbTableAlias("S12")]
        [DbQueryField("S12_CreateId")]
        public long? ApplicantId { get; set; }
        /// <summary>
        /// 申请人名称
        /// </summary>
        [DbTableAlias("S12")]
        [DbQueryField("S12_CreateBy")]
        public string ApplicantName { get; set; }
        /// <summary>
        /// 申请时间
        /// </summary>
        [DbTableAlias("S12")]
        [DbQueryField("S12_CreateTime")]
        public DateTime? ApplyTime { get; set; }
        /// <summary>
        /// 内容描述
        /// </summary>
        [DbTableAlias("S12")]
        [DbQueryField("S12_ApplicationInfo")]
        public string Description { get; set; }
    }
    #endregion
}
