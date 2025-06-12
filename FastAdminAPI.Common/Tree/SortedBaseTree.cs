using System.Collections.Generic;

namespace FastAdminAPI.Common.Tree;

/// <summary>
/// 有序基础树结构
/// </summary>
public class SortedBaseTree : SortedBaseTree<SortedBaseTree>
{
}

/// <summary>
/// 有序基础树结构
/// </summary>
/// <typeparam name="T"></typeparam>
public class SortedBaseTree<T>
    where T : SortedBaseTree<T>
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 父级Id
    /// </summary>
    public long? ParentId { get; set; }

    /// <summary>
    /// 优先级
    /// </summary>
    public long? Priority { get; set; }

    /// <summary>
    /// 额外信息
    /// </summary>
    public object Data { get; set; }

    /// <summary>
    /// 子集
    /// </summary>
    public List<T> Children { get; set; }

    /// <summary>
    /// 构建树结构
    /// </summary>
    /// <param name="dataSource">数据源</param>
    /// <param name="keyword">关键字</param>
    /// <returns></returns>
    public static List<T> BuildTree(List<T> dataSource, string keyword = null)
    {
        return TreeBuilder.BuildTree(
            dataSource: dataSource,
            idSelector: c => c.Id,
            parentIdSelector: c => c.ParentId,
            setChildren: (node, children) => node.Children = children,
            sortKey: c => c.Priority,
            keyword: keyword,
            filterSelector: c => c.Name);
    }

    /// <summary>
    /// 构建Json树结构
    /// </summary>
    /// <param name="dataSource">数据源</param>
    /// <param name="keyword">关键字</param>
    /// <returns></returns>
    public static string BuildJsonTree(List<T> dataSource, string keyword = null)
    {
        return BuildTree(dataSource, keyword).ToJson();
    }

    /// <summary>
    /// 构建自定义树结构
    /// </summary>
    /// <param name="dataSource">数据源</param>
    /// <param name="rootIds">自定义根节点Ids</param>
    /// <param name="keyword">关键字</param>
    /// <returns></returns>
    public static List<T> BuildCustomTree(List<T> dataSource, List<long> rootIds, string keyword = null)
    {
        return TreeBuilder.BuildCustomTree(
            dataSource: dataSource,
            rootIds: rootIds,
            idSelector: c => c.Id,
            parentIdSelector: c => c.ParentId,
            setChildren: (node, children) => node.Children = children,
            sortKey: c => c.Priority,
            keyword: keyword,
            filterSelector: c => c.Name);
    }

    /// <summary>
    /// 构建自定义Json树结构
    /// </summary>
    /// <param name="dataSource">数据源</param>
    /// <param name="rootIds">自定义根节点Ids</param>
    /// <param name="keyword">关键字</param>
    /// <returns></returns>
    public static string BuildCustomJsonTree(List<T> dataSource, List<long> rootIds, string keyword = null)
    {
        return BuildCustomTree(dataSource, rootIds, keyword).ToJson();
    }

    /// <summary>
    /// 扁平化展开树结构
    /// </summary>
    /// <param name="dataSource"></param>
    /// <returns></returns>
    public static List<T> FlattenTree(List<T> dataSource)
    {
        return TreeBuilder.FlattenTree(dataSource, c => c.Children);
    }
}