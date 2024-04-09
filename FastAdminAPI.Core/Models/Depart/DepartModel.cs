using FastAdminAPI.Common.Enums;
using FastAdminAPI.Common.JsonTree;
using FastAdminAPI.Framework.Extensions.Models;
using Newtonsoft.Json;
using SqlSugar.Attributes.Extension.Extensions;
using SqlSugar.Attributes.Extension.Extensions.Attributes.Operation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FastAdminAPI.Core.Models.Depart
{
    #region 部门

    #region 查询
    public class DepartInfoModel : SortedJsonTree
    {
        /// <summary>
        /// 部门属性 S99_Code
        /// </summary>
        public long? Property { get; set; }
        /// <summary>
        /// 部门属性名称
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// 部门标签 -1无 0线索 1客户 2代理，多个以,号分隔
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// 上级部门名称
        /// </summary>
        public string ParentName { get; set; }
        /// <summary>
        /// 角标 每一级4位数字
        /// </summary>
        public string CornerMark { get; set; }
        /// <summary>
        /// 部门负责人
        /// </summary>
        public string DirectordName { get; set; }
    }
    #endregion

    #region 操作
    public class DepartBaseModel : DbOperationBaseModel
    {
        /// <summary>
        /// 部门属性 S99_Code
        /// </summary>
        [Required(ErrorMessage = "部门属性不能为空!")]
        [DbOperationField("S05_Property")]
        public long? Property { get; set; }

        /// <summary>
        /// 部门标签 -1无 0线索 1客户 2代理，多个以,号分隔
        /// </summary>
        [DbIgnoreField]
        public List<long> DepartLabelList { get; set; }
        /// <summary>
        /// 标签
        /// </summary>
        [JsonIgnore]
        [DbOperationField("S05_Label")]
        public string Label { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        [Required(ErrorMessage = "优先级不能为空!")]
        [DbOperationField("S05_Priority")]
        public int? Priority { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        [Required(ErrorMessage = "部门名称不能为空!")]
        [DbOperationField("S05_DepartName")]
        public string DepartName { get; set; }
    }
    public class AddDepartModel : DepartBaseModel
    {
        /// <summary>
        /// 父级部门Id
        /// </summary>
        [DbOperationField("S05_ParentDepartId", false, true)]
        public long? ParentDepartId { get; set; }
        /// <summary>
        /// 角标 每一级4位数字
        /// </summary>
        [JsonIgnore]
        [DbOperationField("S05_CornerMark")]
        public string CornerMark { get; set; }

        [JsonIgnore]
        [DbOperationField("S05_IsDelete")]
        public byte IsDelete { get; private set; } = (byte)BaseEnums.TrueOrFalse.False;
        [JsonIgnore]
        [DbOperationField("S05_CreateId")]
        public long OperationId { get; set; }
        [JsonIgnore]
        [DbOperationField("S05_CreateBy")]
        public string OperationName { get; set; }
        [JsonIgnore]
        [DbOperationField("S05_CreateTime")]
        public DateTime OperationTime { get; set; }
    }
    public class EditDepartModel : DepartBaseModel
    {
        /// <summary>
        /// 部门Id
        /// </summary>
        [Required(ErrorMessage = "部门Id不能为空!")]
        [DbOperationField("S05_DepartId", true)]
        public long? DepartId { get; set; }
        [JsonIgnore]
        [DbOperationField("S05_ModifyId")]
        public long OperationId { get; set; }
        [JsonIgnore]
        [DbOperationField("S05_ModifyBy")]
        public string OperationName { get; set; }
        [JsonIgnore]
        [DbOperationField("S05_ModifyTime")]
        public DateTime OperationTime { get; set; }
    }
    #endregion

    #endregion

    #region 部门编制
    public class DepartMaxEmployeeNumsModel
    {
        /// <summary>
        /// 下级部门数
        /// </summary>
        public long? DepartNums { get; set; } = 0;
        /// <summary>
        /// 下级岗位数
        /// </summary>
        public long? PostSum { get; set; } = 0;
        /// <summary>
        /// 编制 (当前编制/总编制)
        /// </summary>
        public string MaxEmployeeNums { get; set; } = $"0/0";
    }
    #endregion
}
