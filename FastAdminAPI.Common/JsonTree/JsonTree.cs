using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace FastAdminAPI.Common.JsonTree
{
    /// <summary>
    /// Json树结构
    /// </summary>
    public class JsonTree : JsonTree<JsonTree> { }
    
    /// <summary>
    /// Json树结构
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JsonTree<T> where T : JsonTree<T>
    {
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
        [JsonProperty(PropertyName = "Data", NullValueHandling = NullValueHandling.Ignore)]
        public object Data { get; set; }
        /// <summary>
        /// 子集
        /// </summary>
        [JsonProperty(PropertyName = "Children", NullValueHandling = NullValueHandling.Ignore)]
        public List<T> Children { get; set; }

        /// <summary>
        /// 创建树
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static List<T> CreateTree(List<T> dataSource, string keyword = null)
        {
            var tree = CreateTree(dataSource);
            return !string.IsNullOrWhiteSpace(keyword) ? FilterKeyword(tree, keyword) : tree;
        }

        /// <summary>
        /// 创建Json树
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static string CreateJsonTree(List<T> dataSource, string keyword = null)
        {
            var tree = CreateTree(dataSource);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                tree = FilterKeyword(tree, keyword);
            }
            
            return JsonConvert.SerializeObject(tree);
        }

        /// <summary>
        /// 创建自定义树
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="parents"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static List<T> CreateCustomTree(List<T> dataSource, List<T> parents, string keyword = null)
        {
            var tree = CreateCustomTree(dataSource, parents);
            return !string.IsNullOrWhiteSpace(keyword) ? FilterKeyword(tree, keyword) : tree;
        }

        /// <summary>
        /// 创建自定义Json树
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="parents"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static string CreateCustomJsonTree(List<T> dataSource, List<T> parents, string keyword = null)
        {
            var tree = CreateCustomTree(dataSource, parents);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                tree = FilterKeyword(tree, keyword);
            }
            
            return JsonConvert.SerializeObject(tree);
        }

        #region 私有方法

        /// <summary>
        /// 创建树
        /// </summary>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        private static List<T> CreateTree(List<T> dataSource)
        {
            if (dataSource?.Count > 0)
            {
                //获取父级
                var parents = dataSource.Where(c => c.ParentId == null || c.ParentId == 0)?.ToList();
                //获取子级
                var children = dataSource.Where(c => c.ParentId != null && c.ParentId != 0)?.ToList();

                parents?.ForEach(item =>
                {
                    item.AddChildren(children);
                });

                return parents;
            }
            return new List<T>();
        }

        /// <summary>
        /// 创建自定义树
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="parents"></param>
        /// <returns></returns>
        private static List<T> CreateCustomTree(List<T> dataSource, List<T> parents)
        {
            if (dataSource?.Count > 0 && parents?.Count > 0)
            {
                parents.ForEach(item =>
                {
                    item.AddChildren(dataSource);
                });
                
                return parents;
            }
            return new List<T>();
        }

        /// <summary>
        /// 添加子集
        /// </summary>
        /// <param name="dataSource"></param>
        private void AddChildren(List<T> dataSource)
        {
            var childrenByParent = dataSource?.Where(c => c.ParentId == this.ParentId)?.ToList();
            if (childrenByParent?.Count > 0)
            {
                List<T> children = new();
                foreach (var item in childrenByParent)
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
        /// <param name="tree"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        private static List<T> FilterKeyword(List<T> tree, string keyword)
        {
            List<T> result = new List<T>();
            if (tree?.Count > 0)
            {
                foreach (var item in tree)
                {
                    var children = FilterKeyword(item.Children, keyword);
                    
                    bool isMatch = item.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase);
                    if (isMatch || children?.Count > 0)
                    {
                        item.Children = children?.Count > 0 ? children : null;
                        result.Add(item);
                    }
                }
            }
            return result;
        }
        
        #endregion
    }
    
}
