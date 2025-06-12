using System;
using System.ComponentModel.DataAnnotations;
using FastAdminAPI.Common.Enums;
using FastAdminAPI.Common.Tree;
using FastAdminAPI.Framework.Models;
using Newtonsoft.Json;
using SqlSugar.Attributes.Extension.Extensions.Attributes.Operation;

namespace FastAdminAPI.Core.Models.Modules
{
    #region 查询
    public class ModuleInfoModel : SortedBaseTree<ModuleInfoModel>
    {
        /// <summary>
        /// 属性 0菜单 1页面 2按钮 3列表 9其他
        /// </summary>
        public byte? Kind { get; set; }
        /// <summary>
        /// 深度 0根菜单 1一级菜单 2二级菜单 （以此类推）
        /// </summary>
        public long? Depth { get; set; }
        /// <summary>
        /// 前端路由 Url路径/控件Id
        /// </summary>
        public string FrontRoute { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Logo { get; set; }
        /// <summary>
        /// 后端接口 调用的接口，多个以|号分隔
        /// </summary>
        public string BackInterface { get; set; }
        /// <summary>
        /// 角标 每一级4位数字
        /// </summary>
        public string CornerMark { get; set; }
    }
    #endregion

    #region 操作
    public class ModuleBaseModel : DbOperationBaseModel
    {
        /// <summary>
        /// 父模块Id
        /// </summary>
        [DbOperationField("S02_ParentModuleId", false, true)]
        public long? ParentModuleId { get; set; }
        /// <summary>
        /// 属性 0菜单 1页面 2按钮 3列表 9其他
        /// </summary>
        [Required(ErrorMessage = "属性不能为空!")]
        [DbOperationField("S02_Kind")]
        public byte Kind { get; set; }
        /// <summary>
        /// 模块名称
        /// </summary>
        [Required(ErrorMessage = "模块名称不能为空!")]
        [DbOperationField("S02_ModuleName")]
        public string ModuleName { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>
        [DbOperationField("S02_Priority", false, true)]
        public int? Priority { get; set; }
        /// <summary>
        /// 深度 0根菜单 1一级菜单 2二级菜单 （以此类推）
        /// </summary>
        [DbOperationField("S02_Depth", false, true)]
        public long? Depth { get; set; }
        /// <summary>
        /// 前端路由 Url路径/控件Id
        /// </summary>
        [Required(ErrorMessage = "前端路由不能为空!")]
        [DbOperationField("S02_FrontRoute", false, true)]
        public string FrontRoute { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        [DbOperationField("S02_Logo", false, true)]
        public string Logo { get; set; }
        /// <summary>
        /// 后端接口 调用的接口，多个以|号分隔
        /// </summary>
        [DbOperationField("S02_BackInterface", false, true)]
        public string BackInterface { get; set; }
    }
    public class AddModuleModel : ModuleBaseModel
    {
        /// <summary>
        /// 角标 每一级4位数字
        /// </summary>
        [JsonIgnore]
        [DbOperationField("S02_CornerMark")]
        public string CornerMark { get; set; }
        /// <summary>
        /// 是否删除 0否 1是
        /// </summary>
        [JsonIgnore]
        [DbOperationField("S02_IsDelete")]
        public byte IsDelete { get; private set; } = (byte)BaseEnums.TrueOrFalse.False;
        [JsonIgnore]
        [DbOperationField("S02_CreateId")]
        public long OperationId { get; set; }
        [JsonIgnore]
        [DbOperationField("S02_CreateBy")]
        public string OperationName { get; set; }
        [JsonIgnore]
        [DbOperationField("S02_CreateTime")]
        public DateTime OperationTime { get; set; }
    }
    public class EditModuleModel : ModuleBaseModel
    {
        /// <summary>
        /// 模块Id
        /// </summary>
        [Required(ErrorMessage = "模块Id不能为空!")]
        [DbOperationField("S02_ModuleId", true)]
        public long? ModuleId { get; set; }
        [JsonIgnore]
        [DbOperationField("S02_ModifyId")]
        public long OperationId { get; set; }
        [JsonIgnore]
        [DbOperationField("S02_ModifyBy")]
        public string OperationName { get; set; }
        [JsonIgnore]
        [DbOperationField("S02_ModifyTime")]
        public DateTime OperationTime { get; set; }
    }
    #endregion
}
