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
                var parents = dataSource.Where(c => c.ParentId == null || c.ParentId == 0)?.ToList();
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
        /// <param name="keyWord"></param>
        /// <returns></returns>
        public static string CreateJsonTrees<T>(List<T> dataSource, string keyWord = null)
            where T : JsonTree
        {
            var trees = CreateTrees(dataSource);
            if (!string.IsNullOrWhiteSpace(keyWord))
            {
                trees = FilterKeyWord(trees, keyWord);
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
            if(parents?.Count > 0 && childs?.Count > 0)
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
        /// <param name="keyWord"></param>
        /// <returns></returns>
        public static string CreateCustomJsonTrees<T>(List<T> parents, List<T> childs, string keyWord = null)
            where T : JsonTree
        {
            var trees = CreateCustomTrees(parents, childs);
            if (!string.IsNullOrWhiteSpace(keyWord))
            {
                trees = FilterKeyWord(trees, keyWord);
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
                var children = new List<JsonTree>();
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
        /// <param name="KeyWord"></param>
        /// <returns></returns>
        private static List<T> FilterKeyWord<T>(List<T> trees, string KeyWord)
            where T : JsonTree
        {
            var result = trees;
            if (trees?.Count > 0)
            {
                for (int i = 0; i < trees?.Count; i++)
                {
                    //如果包含子类就继续往下找
                    if (trees[i].Children?.Count > 0)
                    {
                        FilterKeyWord(trees[i].Children, KeyWord);
                    }
                    else
                    {
                        //沒有子节点且不包含关键字的直接移除
                        if (!trees[i].Name.Contains(KeyWord))
                        {
                            result.Remove(trees[i]);
                            i--;
                            continue;
                        }
                    }
                    //再次判断这个节点是否还包含子节点
                    //如果不包含则说明子节点都不包含关键字,需要移除父节点
                    if (!trees[i].Name.Contains(KeyWord) && (trees[i].Children == null || trees[i].Children.Count == 0))
                    {
                        result.Remove(trees[i]);
                        --i;
                    }
                }
            }
            return result;
        }
    }
}
