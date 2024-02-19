using FastAdminAPI.Common.Enums;
using FastAdminAPI.Framework.Extensions;
using FastAdminAPI.Framework.Extensions.DbOperationExtensions;
using FastAdminAPI.Framework.Extensions.DbQueryExtensions;
using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace FastAdminAPI.Core.Models.BasicSettings
{
    #region 查询
    public class CodePageSearch : DbQueryPagingModel
    {
        /// <summary>
        /// 分组代号
        /// </summary>
        [DbQueryField("S99_GroupCode")]
        [DbQueryOperator(DbOperator.Equal)]
        public string GroupCode { get; set; }
        /// <summary>
        /// 分组名称
        /// </summary>
        [DbQueryField("S99_GroupName")]
        [DbQueryOperator(DbOperator.Like)]
        public string GroupName { get; set; }
        /// <summary>
        /// 字典名称
        /// </summary>
        [DbQueryField("S99_Name")]
        [DbQueryOperator(DbOperator.Like)]
        public string Name { get; set; }
    }
    public class CodePageResult
    {
        /// <summary>
        /// 字典Id
        /// </summary>
        [DbQueryField("S99_CodeId")]
        public long? CodeId { get; set; }
        /// <summary>
        /// 分组代号
        /// </summary>
        [DbQueryField("S99_GroupCode")]
        public string GroupCode { get; set; }
        /// <summary>
        /// 分组名称
        /// </summary>
        [DbQueryField("S99_GroupName")]
        public string GroupName { get; set; }
        /// <summary>
        /// 字典名称
        /// </summary>
        [DbQueryField("S99_Name")]
        public string Name { get; set; }
        /// <summary>
        /// 序号 组内排序使用
        /// </summary>
        [DbQueryField("S99_SeqNo")]
        public int? SeqNo { get; set; }
        /// <summary>
        /// 系统标记 0系统 1用户
        /// </summary>
        [DbQueryField("S99_SysFlag")]
        public byte? SysFlag { get; private set; } = (byte)BaseEnums.SystemFlag.User;
    }
    public class GroupCodeModel
    {
        /// <summary>
        /// 分组代号
        /// </summary>
        public string GroupCode { get; set; }
        /// <summary>
        /// 分组名称
        /// </summary>
        public string GroupName { get; set; }
    } 
    #endregion

    #region 操作
    public class AddCodeModel : DbOperationBaseModel
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
        /// 字典名称
        /// </summary>
        [Required(ErrorMessage = "字典名称不能为空!")]
        [DbOperationField("S99_Name")]
        public string Name { get; set; }
        /// <summary>
        /// 序号 组内排序使用
        /// </summary>
        [DbOperationField("S99_SeqNo")]
        public int? SeqNo { get; set; }
        /// <summary>
        /// 系统标记 0系统 1用户
        /// </summary>
        [JsonIgnore]
        [DbOperationField("S99_SysFlag")]
        public byte? SysFlag { get; private set; } = (byte)BaseEnums.SystemFlag.User;
        /// <summary>
        /// 是否有效 0有效 1无效
        /// </summary>
        [JsonIgnore]
        [DbOperationField("S99_IsValid")]
        public byte? IsValid { get; private set; } = (byte)BaseEnums.IsValid.Valid;
        /// <summary>
        /// 操作者Id
        /// </summary>
        [JsonIgnore]
        [DbOperationField("S99_CreateId")]
        public long OperationId { get; set; }
        /// <summary>
        /// 操作者名称
        /// </summary>
        [JsonIgnore]
        [DbOperationField("S99_CreateBy")]
        public string OperationName { get; set; }
        /// <summary>
        /// 操作日期
        /// </summary>
        [JsonIgnore]
        [DbOperationField("S99_CreateTime")]
        public DateTime OperationTime { get; set; }
    }
    public class EditCodeModel : DbOperationBaseModel
    {
        /// <summary>
        /// 字典Id
        /// </summary>
        [Required(ErrorMessage = "字典Id不能为空!")]
        [DbOperationField("S99_CodeId", true)]
        public long? CodeId { get; set; }
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
        /// 字典名称
        /// </summary>
        [Required(ErrorMessage = "字典名称不能为空!")]
        [DbOperationField("S99_Name")]
        public string Name { get; set; }
        /// <summary>
        /// 序号 组内排序使用
        /// </summary>
        [DbOperationField("S99_SeqNo")]
        public int? SeqNo { get; set; }
        /// <summary>
        /// 操作者Id
        /// </summary>
        [JsonIgnore]
        [DbOperationField("S99_ModifyId")]
        public long OperationId { get; set; }
        /// <summary>
        /// 操作者名称
        /// </summary>
        [JsonIgnore]
        [DbOperationField("S99_ModifyBy")]
        public string OperationName { get; set; }
        /// <summary>
        /// 操作日期
        /// </summary>
        [JsonIgnore]
        [DbOperationField("S99_ModifyTime")]
        public DateTime OperationTime { get; set; }
    } 
    #endregion
}
