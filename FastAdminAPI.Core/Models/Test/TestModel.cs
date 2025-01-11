using FastAdminAPI.Common.Enums;
using FastAdminAPI.Framework.Models;
using Mapster;
using Newtonsoft.Json;
using SqlSugar.Attributes.Extension.Common;
using SqlSugar.Attributes.Extension.Extensions.Attributes.Operation;
using SqlSugar.Attributes.Extension.Extensions.Attributes.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FastAdminAPI.Core.Models.Test
{
    #region 查询
    public class CodeMapsterModel
    {
        /// <summary>
        /// 系统代号Id
        /// </summary>
        [AdaptMember("S99_CodeId")]
        public long? CodeId { get; set; }
        /// <summary>
        /// 系统代号名称
        /// </summary>
        [AdaptMember("S99_Name")]
        public string CodeName { get; set; }
        [AdaptIgnore] //忽略该字段，Mapster不进行映射
        public string IgnoreField { get; set; }
    }

    [DbDefaultOrderBy("a.S99_CreateTime", DbSortWay.DESC)]
    [DbDefaultOrderBy("b.S99_CodeId", DbSortWay.ASC)]
    public class CodePageSearch : DbQueryBaseModel
    {
        /// <summary>
        /// 分组代号
        /// </summary>
        [DbTableAlias("a")]
        [DbQueryField("S99_GroupCode")]
        [DbQueryOperator(DbOperator.In)]
        public string[] GroupCode { get; set; }
        /// <summary>
        /// 分组名称
        /// </summary>
        [DbTableAlias("b")]
        [DbQueryField("S99_GroupName")]
        [DbQueryOperator(DbOperator.Like)]
        public string GroupName { get; set; }
        /// <summary>
        /// 系统代号Ids集合
        /// </summary>
        [DbTableAlias("a")]
        [DbQueryField("S99_CodeId")]
        [DbQueryOperator(DbOperator.In)]
        public List<long> CodeIds { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        [DbTableAlias("a")]
        [DbQueryField("S99_CreateTime", DbTimeSuffixType.StartTime)]
        [DbQueryOperator(DbOperator.GreaterThanOrEqual)]
        public string CreateStartDate { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        [DbTableAlias("a")]
        [DbQueryField("S99_CreateTime", DbTimeSuffixType.EndTime)]
        [DbQueryOperator(DbOperator.LessThanOrEqual)]
        public string CreateEndDate { get; set; }
    }
    public class CodePageResult
    {
        /// <summary>
        /// 系统代号Id
        /// </summary>
        [DbTableAlias("a")]
        [DbQueryField("S99_CodeId")]
        public long? CodeId { get; set; }
        /// <summary>
        /// 系统代号名称
        /// </summary>
        [DbTableAlias("b")]
        [DbQueryField("S99_Name")]
        public string CodeName { get; set; }

    }
    #endregion

    #region 操作
    public class CodeBaseModel : DbOperationBaseModel
    {
        /// <summary>
        /// 系统代号名称
        /// </summary>
        [Required(ErrorMessage = "系统代号名称不能为空!")]
        [DbOperationField("S99_Name")]
        public string Name { get; set; }
        /// <summary>
        /// 父级系统代号Id
        /// </summary>
        [DbOperationField("S99_ParentCodeId")]
        public long? ParentCodeId { get; set; }
        /// <summary>
        /// 序号 组内排序使用
        /// </summary>
        [DbOperationField("S99_SeqNo", false, true)]
        public long? SeqNo { get; set; }
        /// <summary>
        /// 系统标记 1不可修改
        /// </summary>
        [DbOperationField("S99_SysFlag", false, true)]
        public long? SysFlag { get; set; }
    }
    public class AddCodeModel : CodeBaseModel
    {
        /// <summary>
        /// 分组代号
        /// </summary>
        [Required(ErrorMessage = "分组代号不能为空!")]
        [DbOperationField("S99_GroupCode")]
        public string GroupCode { get; set; }
        /// <summary>
        /// 分组名称
        /// </summary>
        [Required(ErrorMessage = "分组名称不能为空!")]
        [DbOperationField("S99_GroupName")]
        public string GroupName { get; set; }
        /// <summary>
        /// 是否删除 0否 1是
        /// </summary>
        [JsonIgnore]
        [DbOperationField("S99_IsDelete")]
        public byte IsDelete { get; private set; } = (byte)BaseEnums.TrueOrFalse.False;
        /// <summary>
        /// 创建者Id
        /// </summary>
        [JsonIgnore]
        [DbOperationField("S99_CreateId")]
        public long? OperationId { get; set; }
        /// <summary>
        /// 创建者名称
        /// </summary>
        [JsonIgnore]
        [DbOperationField("S99_CreateBy")]
        public string OperationName { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        [JsonIgnore]
        [DbOperationField("S99_CreateTime")]
        public DateTime? OperationTime { get; set; }
    }
    public class EditCodeModel : CodeBaseModel
    {
        /// <summary>
        /// 系统代号Id
        /// </summary>
        [Required(ErrorMessage = "系统代号Id不能为空!")]
        [DbOperationField("S99_CodeId", true)]
        public long? CodeId { get; set; }
        /// <summary>
        /// 更新者Id
        /// </summary>
        [JsonIgnore]
        [DbOperationField("S99_ModifyId")]
        public long? OperationId { get; set; }
        /// <summary>
        /// 更新者名称
        /// </summary>
        [JsonIgnore]
        [DbOperationField("S99_ModifyBy")]
        public string OperationName { get; set; }
        /// <summary>
        /// 更新日期
        /// </summary>
        [JsonIgnore]
        [DbOperationField("S99_ModifyTime")]
        public DateTime? OperationTime { get; set; }
    }
    public class DelCodeModel : DbOperationBaseModel
    {
        /// <summary>
        /// 系统代号Id
        /// </summary>
        [Required(ErrorMessage = "系统代号Id不能为空!")]
        [DbOperationField("S99_CodeId", true)]
        public long? CodeId { get; set; }
        /// <summary>
        /// 是否删除 0否 1是
        /// </summary>
        [JsonIgnore]
        [DbOperationField("S99_IsDelete")]
        public byte IsDelete { get; private set; } = (byte)BaseEnums.TrueOrFalse.True;
        /// <summary>
        /// 更新者Id
        /// </summary>
        [JsonIgnore]
        [DbOperationField("S99_DeleteId")]
        public long? OperationId { get; set; }
        /// <summary>
        /// 更新者名称
        /// </summary>
        [JsonIgnore]
        [DbOperationField("S99_DeleteBy")]
        public string OperationName { get; set; }
        /// <summary>
        /// 更新日期
        /// </summary>
        [JsonIgnore]
        [DbOperationField("S99_DeleteTime")]
        public DateTime? OperationTime { get; set; }
    }
    #endregion
}
