using System.ComponentModel;

namespace FastAdminAPI.Common.Enums
{
    public class BaseEnums
    {
        /// <summary>
        /// TrueOrFalse 0否 1是
        /// </summary>
        public enum TrueOrFalse
        {
            /// <summary>
            /// 否
            /// </summary>
            False,
            /// <summary>
            /// 是
            /// </summary>
            True
        }
        /// <summary>
        /// 是否完成 0未完成 1已完成
        /// </summary>
        public enum IsFinish
        {
            /// <summary>
            /// 未完成
            /// </summary>
            Unfinish,
            /// <summary>
            /// 已完成
            /// </summary>
            Finished,

        }
        /// <summary>
        /// 是否确认 0否 1是
        /// </summary>
        public enum IsConfirm
        {
            /// <summary>
            /// 未确认
            /// </summary>
            NotConfirm,
            /// <summary>
            /// 已确认
            /// </summary>
            Confirmed
        }
        /// <summary>
        /// 是否默认 0否 1是
        /// </summary>
        public enum IsDefault
        {
            UnDefault,
            Default
        }
        /// <summary>
        /// 系统标记 0系统 1用户
        /// </summary>
        public enum SystemFlag
        {
            /// <summary>
            /// 系统
            /// </summary>
            System,
            /// <summary>
            /// 用户
            /// </summary>
            User
        }
        /// <summary>
        /// 上下架状态 0上架 1下架
        /// </summary>
        public enum StockStatus 
        {
            /// <summary>
            /// 上架
            /// </summary>
            Up, 
            /// <summary>
            /// 下架
            /// </summary>
            Down
        }
    }
}
