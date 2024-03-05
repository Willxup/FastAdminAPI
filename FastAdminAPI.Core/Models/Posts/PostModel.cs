using FastAdminAPI.Common.Enums;
using FastAdminAPI.Common.JsonTree;
using FastAdminAPI.Framework.Extensions.Models;
using Newtonsoft.Json;
using SqlSugar.Attributes.Extension.Extensions.Attributes.Operation;
using System;
using System.ComponentModel.DataAnnotations;

namespace FastAdminAPI.Core.Models.Posts
{
    #region 岗位

    #region 查询
    public class PostInfoModel : JsonTree
    {
        /// <summary>
        /// 部门Id
        /// </summary>
        public long? DepartId { get; set; }
        /// <summary>
        /// 在编人数
        /// </summary>
        public int CurrentEmployeeNums { get; set; } = 0;
        /// <summary>
        /// 岗位编制
        /// </summary>
        public int MaxEmployeeNums { get; set; } = 0;
        /// <summary>
        /// 岗位职责
        /// </summary>
        public string Responsibility { get; set; }
        /// <summary>
        /// 能力需求
        /// </summary>
        public string AbilityDemand { get; set; }
        /// <summary>
        /// 角标
        /// </summary>
        public string CornerMark { get; set; }
    }
    #endregion

    #region 操作
    public class PostBaseModel : DbOperationBaseModel
    {
        /// <summary>
        /// 岗位名称
        /// </summary>
        [Required(ErrorMessage = "岗位名称不能为空!")]
        [DbOperationField("S06_PostName")]
        public string PostName { get; set; }
        /// <summary>
        /// 岗位编制
        /// </summary>
        [DbOperationField("S06_Staffing", false, true)]
        public long? Staffing { get; set; }
        /// <summary>
        /// 岗位职责
        /// </summary>
        [DbOperationField("S06_Responsibility", false, true)]
        public string Responsibility { get; set; }
        /// <summary>
        /// 能力需求
        /// </summary>
        [DbOperationField("S06_AbilityDemand", false, true)]
        public string AbilityDemand { get; set; }
    }
    public class AddPostModel : PostBaseModel
    {
        
        /// <summary>
        /// 父级岗位Id
        /// </summary>
        [DbOperationField("S06_ParentPostId", false, true)]
        public long? ParentPostId { get; set; }
        /// <summary>
        /// 部门Id
        /// </summary>
        [Required(ErrorMessage = "部门Id不能为空!")]
        [DbOperationField("S05_DepartId")]
        public long? DepartId { get; set; }
        /// <summary>
        /// 角标 每一级4位数字
        /// </summary>
        [JsonIgnore]
        [DbOperationField("S06_CornerMark")]
        public string CornerMark { get; set; }
        [JsonIgnore]
        [DbOperationField("S06_IsValid")]
        public byte IsValid { get; private set; } = (byte)BaseEnums.IsValid.Valid;
        [JsonIgnore]
        [DbOperationField("S06_CreateId")]
        public long OperationId { get; set; }
        [JsonIgnore]
        [DbOperationField("S06_CreateBy")]
        public string OperationName { get; set; }
        [JsonIgnore]
        [DbOperationField("S06_CreateTime")]
        public DateTime OperationTime { get; set; }
    }
    public class EditPostModel : PostBaseModel
    {
        /// <summary>
        /// 岗位Id
        /// </summary>
        [Required(ErrorMessage = "岗位Id不能为空!")]
        [DbOperationField("S06_PostId", true)]
        public long? PostId { get; set; }
        [JsonIgnore]
        [DbOperationField("S06_ModifyId")]
        public long OperationId { get; set; }
        [JsonIgnore]
        [DbOperationField("S06_ModifyBy")]
        public string OperationName { get; set; }
        [JsonIgnore]
        [DbOperationField("S06_ModifyTime")]
        public DateTime OperationTime { get; set; }
    }
    #endregion 

    #endregion
}
