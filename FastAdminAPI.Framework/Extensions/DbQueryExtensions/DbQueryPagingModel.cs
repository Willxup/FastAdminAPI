using System.Collections.Generic;

namespace FastAdminAPI.Framework.Extensions.DbQueryExtensions
{
    public class DbQueryPagingModel
    {
        /// <summary>
        /// 第几页（从1开始）
        /// </summary>
        public int? Index { get; set; } = null;
        /// <summary>
        /// 返回的每页行数
        /// </summary>
        public int? Size { get; set; } = null;
        /// <summary>
        /// 排序 {排序字段:排序方式}
        /// </summary>
        public Dictionary<string, string> SortFields { get; set; } = null;
        /// <summary>
        /// 偏移
        /// </summary>
        public virtual int? Offset => (Index - 1) * Size;
    }
}
