
using System;
using SqlSugar;


namespace FastAdminAPI.Framework.Entities
{
    ///<summary>
    ///模块
    ///</summary>
    [SugarTable("S02_Module")]
    [Serializable]
    public partial class S02_Module : BaseEntity
    {
        public S02_Module()
        {
            this.S02_IsDelete = Convert.ToByte("0");

        }
        /// <summary>
        /// Desc:模块Id
        /// Default:
        /// Nullable:False
        /// </summary>

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long S02_ModuleId { get; set; }
        /// <summary>
        /// Desc:父模块Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? S02_ParentModuleId { get; set; }
        /// <summary>
        /// Desc:属性 0菜单 1页面 2按钮 3列表 9其他
        /// Default:
        /// Nullable:False
        /// </summary>

        public byte S02_Kind { get; set; }
        /// <summary>
        /// Desc:名称
        /// Default:
        /// Nullable:False
        /// </summary>

        public string S02_ModuleName { get; set; }
        /// <summary>
        /// Desc:优先级
        /// Default:
        /// Nullable:True
        /// </summary>

        public int? S02_Priority { get; set; }
        /// <summary>
        /// Desc:深度 0根菜单 1一级菜单 2二级菜单 （以此类推）
        /// Default:
        /// Nullable:True
        /// </summary>

        public int? S02_Depth { get; set; }
        /// <summary>
        /// Desc:前端路由 Url路径/控件Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S02_FrontRoute { get; set; }
        /// <summary>
        /// Desc:图标
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S02_Logo { get; set; }
        /// <summary>
        /// Desc:后端接口 调用的接口，多个以|号分隔
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S02_BackInterface { get; set; }
        /// <summary>
        /// Desc:角标 每一级4位数字
        /// Default:
        /// Nullable:False
        /// </summary>

        public string S02_CornerMark { get; set; }
        /// <summary>
        /// Desc:是否删除 0否 1是
        /// Default:0
        /// Nullable:False
        /// </summary>

        public byte S02_IsDelete { get; set; }
        /// <summary>
        /// Desc:创建者Id
        /// Default:
        /// Nullable:False
        /// </summary>

        public long S02_CreateId { get; set; }
        /// <summary>
        /// Desc:创建者名称
        /// Default:
        /// Nullable:False
        /// </summary>

        public string S02_CreateBy { get; set; }
        /// <summary>
        /// Desc:创建日期
        /// Default:
        /// Nullable:False
        /// </summary>

        public DateTime S02_CreateTime { get; set; }
        /// <summary>
        /// Desc:更新者Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? S02_ModifyId { get; set; }
        /// <summary>
        /// Desc:更新者名称
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S02_ModifyBy { get; set; }
        /// <summary>
        /// Desc:更新日期
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? S02_ModifyTime { get; set; }
        /// <summary>
        /// Desc:删除者Id
        /// Default:
        /// Nullable:True
        /// </summary>

        public long? S02_DeleteId { get; set; }
        /// <summary>
        /// Desc:删除者名称
        /// Default:
        /// Nullable:True
        /// </summary>

        public string S02_DeleteBy { get; set; }
        /// <summary>
        /// Desc:删除日期
        /// Default:
        /// Nullable:True
        /// </summary>

        public DateTime? S02_DeleteTime { get; set; }
    }
}
