using System.ComponentModel;

namespace FastAdminAPI.Common.Enums
{
    public class BusinessEnums
    {
        #region 用户与登录
        /// <summary>
        /// 账号状态 0启用 1禁用
        /// </summary>
        public enum AccountStatus
        {
            /// <summary>
            /// 启用
            /// </summary>
            [Description("启用")]
            Enable,
            /// <summary>
            /// 禁用
            /// </summary>
            [Description("禁用")]
            Disable
        }
        /// <summary>
        /// 登录设备 0PC 1企业微信
        /// </summary>
        public enum LoginDevice
        {
            /// <summary>
            /// PC
            /// </summary>
            [Description("PC")]
            PC,
            /// <summary>
            /// 企业微信
            /// </summary>
            [Description("企业微信")]
            QyWechat
        }
        #endregion

        #region 模块与权限
        /// <summary>
        /// 属性 0菜单 1页面 2按钮 3列表 9其他
        /// </summary>
        public enum ModuleKind
        {
            /// <summary>
            /// 菜单
            /// </summary>
            Menu = 0,
            /// <summary>
            /// 页面
            /// </summary>
            Page = 1,
            /// <summary>
            /// 按钮
            /// </summary>
            Button = 2,
            /// <summary>
            /// 列表
            /// </summary>
            List = 3,
            /// <summary>
            /// 其他
            /// </summary>
            Others = 9
        }
        /// <summary>
        /// 权限类型 0角色 1用户
        /// </summary>
        public enum PermissionType
        {
            /// <summary>
            /// 角色
            /// </summary>
            Role,
            /// <summary>
            /// 用户
            /// </summary>
            User
        }

        #endregion

        #region 员工
        /// <summary>
        /// 员工状态 0正式 1实习 2离职
        /// </summary>
        public enum EmployeeStatus
        {
            /// <summary>
            /// 正式
            /// </summary>
            Formal,
            /// <summary>
            /// 实习
            /// </summary>
            Practice,
            /// <summary>
            /// 离职
            /// </summary>
            Dimission
        }
        #endregion
    }
}
