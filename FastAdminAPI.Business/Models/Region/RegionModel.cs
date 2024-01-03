using System.Collections.Generic;

namespace FastAdminAPI.Business.Models.Region
{
    public class RegionModel
    {
        /// <summary>
        /// 区域Id
        /// </summary>
        public long RegionId { get; set; }
        /// <summary>
        /// 区域Code
        /// </summary>
        public long? ParentId { get; set; }
        /// <summary>
        /// 区域名称
        /// </summary>
        public string RegionCode { get; set; }
        /// <summary>
        /// 区域父级Id
        /// </summary>
        public string RegionName { get; set; }
    }
    public class RegionStructureModel
    {
        /// <summary>
        /// 省份
        /// </summary>
        public Dictionary<string, string> Province { get; set; } = new Dictionary<string, string>();
        /// <summary>
        /// 城市
        /// </summary>
        public Dictionary<string, string> City { get; set; } = new Dictionary<string, string>();
        /// <summary>
        /// 区县
        /// </summary>
        public Dictionary<string, string> Region { get; set; } = new Dictionary<string, string>();
    }
}
