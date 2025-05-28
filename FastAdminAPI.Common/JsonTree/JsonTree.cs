using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace FastAdminAPI.Common.JsonTree
{
    /// <summary>
    /// Json树结构
    /// </summary>
    public class JsonTree
    {
        public JsonTree() { }
        /// <summary>
        /// Id
        /// </summary>
        [JsonProperty(PropertyName = "Id")]
        public long Id { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }
        /// <summary>
        /// 父级Id
        /// </summary>
        [JsonProperty(PropertyName = "ParentId")]
        public long? ParentId { get; set; }
        /// <summary>
        /// 额外信息字段
        /// </summary>
        [JsonProperty(PropertyName = "Data")]
        public object Data { get; set; }
        /// <summary>
        /// 子集
        /// </summary>
        [JsonProperty(PropertyName = "Children", NullValueHandling = NullValueHandling.Ignore)]
        public List<JsonTree> Children { get; set; }

        /// <summary>
        /// 创建树
        /// </summary>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        public static List<T> CreateTrees<T>(List<T> dataSource)
            where T : JsonTree
        {
            if (dataSource?.Count > 0)
            {
                //获取父级
                var parents = dataSource.Where(c => c.ParentId == null || c.ParentId == 0)?.ToList();

                //获取子级
                var childs = dataSource.Where(c => c.ParentId != null && c.ParentId != 0)?.ToList();

                parents?.ForEach(item =>
                {
                    item.AddChildren(childs);
                });

                return parents;
            }

            return new List<T>();
        }
        /// <summary>
        /// 创建Json树
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataSource"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static string CreateJsonTrees<T>(List<T> dataSource, string keyword = null)
            where T : JsonTree
        {
            //创建树结构
            var trees = CreateTrees(dataSource);

            //关键字过滤
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                trees = FilterKeyWord(trees, keyword);
            }

            return JsonConvert.SerializeObject(trees);
        }

        /// <summary>
        /// 创建自定义树
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parents"></param>
        /// <param name="childs"></param>
        /// <returns></returns>
        public static List<T> CreateCustomTrees<T>(List<T> parents, List<T> childs)
            where T : JsonTree
        {
            if (parents?.Count > 0 && childs?.Count > 0)
            {
                parents?.ForEach(item =>
                {
                    item.AddChildren(childs);
                });

                return parents;
            }

            return new List<T>();
        }
        /// <summary>
        /// 创建自定义Json树
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parents"></param>
        /// <param name="childs"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static string CreateCustomJsonTrees<T>(List<T> parents, List<T> childs, string keyword = null)
            where T : JsonTree
        {
            //创建树结构
            var trees = CreateCustomTrees(parents, childs);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                trees = FilterKeyWord(trees, keyword);
            }

            return JsonConvert.SerializeObject(trees);
        }

        /// <summary>
        /// 添加当前父级的子集
        /// </summary>
        /// <param name="dataSource"></param>
        private void AddChildren<TParameter>(List<TParameter> dataSource)
            where TParameter : JsonTree
        {
            var childs = dataSource?.Where(p => p.ParentId == this.Id)?.ToList();

            if (childs?.Count > 0)
            {
                List<JsonTree> children = new();

                foreach (var item in childs)
                {
                    item.AddChildren(dataSource);
                    children.Add(item);
                }

                this.Children = children;
            }
        }
        /// <summary>
        /// 过滤关键字
        /// </summary>
        /// <param name="trees"></param>
        /// <param name="Keyword"></param>
        /// <returns></returns>
        private static List<T> FilterKeyWord<T>(List<T> trees, string Keyword)
            where T : JsonTree
        {
            List<T> result = new List<T>();

            if (trees?.Count > 0)
            {
                foreach (var item in trees)
                {
                    var children = FilterKeyWord(item.Children, Keyword);
                    bool isMatch = item.Name.Contains(Keyword, StringComparison.OrdinalIgnoreCase);

                    if (isMatch || children?.Count > 0)
                    {
                        item.Children = children?.Count > 0 ? children : null;
                        result.Add(item);
                    }
                }
            }
            
            return result;
        }
    }
}
