using SqlSugar.Attributes.Extension.Extensions;

namespace FastAdminAPI.Framework.Extensions.Models
{
    public class DbQueryBaseModel
    {
        /// <summary>
        /// 第几页（从1开始）
        /// </summary>
        [DbIgnoreField]
        public int? Index { get; set; } = null;
        /// <summary>
        /// 返回的每页行数
        /// </summary>
        [DbIgnoreField]
        public int? Size { get; set; } = null;
        /// <summary>
        /// 偏移
        /// </summary>
        [DbIgnoreField]
        public virtual int? Offset => (Index - 1) * Size;
    }
}
