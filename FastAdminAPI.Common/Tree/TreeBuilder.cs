using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FastAdminAPI.Common.Tree;

/// <summary>
/// 树结构构建器
/// </summary>
public static class TreeBuilder
{
    /// <summary>
    /// 构建树结构
    /// </summary>
    /// <param name="dataSource">数据源</param>
    /// <param name="idSelector">主键选择器</param>
    /// <param name="parentIdSelector">父级Id选择器</param>
    /// <param name="setChildren">设置子集Func</param>
    /// <param name="sortKey">排序字段</param>
    /// <param name="keyword">过滤关键字</param>
    /// <param name="filterSelector">过滤关键字选择器</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<T> BuildTree<T>(
        List<T> dataSource,
        Func<T, long> idSelector,
        Func<T, long?> parentIdSelector,
        Action<T, List<T>> setChildren,
        Func<T, object> sortKey = null,
        string keyword = null,
        Func<T, string> filterSelector = null)
    {
        if (dataSource == null || dataSource.Count <= 0)
        {
            return new List<T>();
        }

        // 是否过滤关键字
        bool isFilter = keyword != null && filterSelector != null;

        Dictionary<long, List<T>> lookup = new();

        foreach (var item in dataSource)
        {
            // 是否过滤关键字
            if (isFilter)
            {
                bool isMatched = filterSelector(item).Contains(keyword, StringComparison.OrdinalIgnoreCase);
                if (!isMatched)
                {
                    continue;
                }
            }

            // 构建lookup
            long parentId = parentIdSelector(item) ?? 0;
            if (!lookup.ContainsKey(parentId))
            {
                lookup[parentId] = new List<T>();
            }

            lookup[parentId].Add(item);
        }

        // 获取根节点
        var rootNodes = lookup.TryGetValue(0, out var roots) ? roots : new List<T>();
        if (rootNodes.Count > 0)
        {
            // 排序
            if (sortKey != null)
            {
                rootNodes = rootNodes.OrderBy(sortKey).ToList();
            }

            Parallel.ForEach(dataSource, node =>
            {
                long id = idSelector(node);
                if (lookup.TryGetValue(id, out var children))
                {
                    if (sortKey != null)
                    {
                        children = children.OrderBy(sortKey).ToList();
                    }

                    setChildren(node, children);
                }
                else
                {
                    setChildren(node, null);
                }
            });
        }

        return rootNodes;
    }

    /// <summary>
    /// 构建自定义树结构
    /// </summary>
    /// <param name="dataSource">数据源</param>
    /// <param name="rootIds">自定义根节点Ids</param>
    /// <param name="idSelector">主键选择器</param>
    /// <param name="parentIdSelector">父级Id选择器</param>
    /// <param name="setChildren">设置子集Func</param>
    /// <param name="sortKey">排序字段</param>
    /// <param name="keyword">过滤关键字</param>
    /// <param name="filterSelector">过滤关键字选择器</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<T> BuildCustomTree<T>(
        List<T> dataSource,
        List<long> rootIds,
        Func<T, long> idSelector,
        Func<T, long?> parentIdSelector,
        Action<T, List<T>> setChildren,
        Func<T, object> sortKey = null,
        string keyword = null,
        Func<T, string> filterSelector = null)
    {
        if (dataSource is not { Count: > 0 } || rootIds is not { Count: > 0 })
        {
            return new List<T>();
        }

        // 是否过滤关键字
        bool isFilter = keyword != null && filterSelector != null;

        Dictionary<long, List<T>> lookup = new();
        List<T> rootNodes = new();

        foreach (var item in dataSource)
        {
            // 是否过滤关键字
            if (isFilter)
            {
                bool isMatched = filterSelector(item).Contains(keyword, StringComparison.OrdinalIgnoreCase);
                if (!isMatched)
                {
                    continue;
                }
            }

            // 设置根节点
            if (rootIds.Contains(idSelector(item)))
            {
                rootNodes.Add(item);
            }

            // 构建lookup
            long parentId = parentIdSelector(item) ?? 0;
            // 过滤掉没有父级的数据源
            if (parentId != 0)
            {
                if (!lookup.ContainsKey(parentId))
                {
                    lookup[parentId] = new List<T>();
                }

                lookup[parentId].Add(item);
            }
        }

        if (sortKey != null)
        {
            rootNodes = rootNodes.OrderBy(sortKey).ToList();
        }

        foreach (var node in rootNodes)
        {
            BuildNode(node, lookup, idSelector, setChildren, sortKey);
        }

        return rootNodes;
    }

    /// <summary>
    /// 过滤关键字
    /// </summary>
    /// <param name="tree">树结构</param>
    /// <param name="keyword">关键字</param>
    /// <param name="keywordSelector">字段过滤选择器</param>
    /// <param name="getChildren">获取子集Func</param>
    /// <param name="setChildren">设置子集Func</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<T> FilterKeyword<T>(
        List<T> tree,
        string keyword,
        Func<T, string> keywordSelector,
        Func<T, List<T>> getChildren,
        Action<T, List<T>> setChildren)
    {
        if (tree is not { Count: > 0 } || string.IsNullOrWhiteSpace(keyword))
        {
            return tree;
        }
        
        List<T> result = new List<T>();
        foreach (var node in tree)
        {
            var matchedChildren = FilterKeyword(
                getChildren(node) ?? new List<T>(),
                keyword,
                keywordSelector,
                getChildren,
                setChildren);
            
            bool isMatched = keywordSelector(node).Contains(keyword, StringComparison.OrdinalIgnoreCase);
            if (isMatched || matchedChildren?.Count > 0)
            {
                setChildren(node, matchedChildren?.Count > 0 ? matchedChildren : null);
                result.Add(node);
            }
        }
        
        return result;
    }

    /// <summary>
    /// 扁平化展开树结构
    /// </summary>
    /// <param name="tree">树结构</param>
    /// <param name="getChildren">获取子集Func</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<T> FlattenTree<T>(
        List<T> tree,
        Func<T, List<T>> getChildren)
    {
        List<T> result = new List<T>();
        foreach (var node in tree)
        {
            FlattenNode(node, getChildren, result);
        }
        
        return result;
    }

    /// <summary>
    /// 树结构转换为Json
    /// </summary>
    /// <param name="tree">树结构</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static string ToJson<T>(this List<T> tree)
    {
        return JsonConvert.SerializeObject(tree);
    }
    
    #region 私有方法

    /// <summary>
    /// 构建节点
    /// </summary>
    /// <param name="node">树节点</param>
    /// <param name="lookup">根与叶子节点关系</param>
    /// <param name="idSelector">主键选择器</param>
    /// <param name="setChildren">设置子集Func</param>
    /// <param name="sortKey">排序字段</param>
    /// <typeparam name="T"></typeparam>
    private static void BuildNode<T>(
        T node,
        Dictionary<long, List<T>> lookup,
        Func<T, long> idSelector,
        Action<T, List<T>> setChildren,
        Func<T, object> sortKey = null)
    {
        long id = idSelector(node);
        if (lookup.TryGetValue(id, out var children))
        {
            if (sortKey != null)
            {
                children = children.OrderBy(sortKey).ToList();
            }

            foreach (var item in children)
            {
                BuildNode(item, lookup, idSelector, setChildren, sortKey);
            }

            setChildren(node, children);
        }
        else
        {
            setChildren(node, null);
        }
    }

    /// <summary>
    /// 扁平化展开树节点
    /// </summary>
    /// <param name="node">树节点</param>
    /// <param name="getChildren">获取子集Func</param>
    /// <param name="result">返回结果集</param>
    /// <typeparam name="T"></typeparam>
    private static void FlattenNode<T>(
        T node,
        Func<T, List<T>> getChildren,
        List<T> result)
    {
        result.Add(node);
        var children = getChildren(node);
        if (children is { Count: > 0 })
        {
            foreach (var item in children)
            {
                FlattenNode(item, getChildren, result);
            }
        }
    }

    #endregion
}