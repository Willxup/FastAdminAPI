namespace FastAdminAPI.Core.Models.BASE
{
    public class GenerateDbEntitiesModel
    {
        /// <summary>
        /// 生成路径（默认）
        /// </summary>
        public string Path { get; set; } = string.Empty;
        /// <summary>
        /// 命名空间（默认）
        /// </summary>
        public string Namespace { get; set; } = string.Empty;
        /// <summary>
        /// 指定表名
        /// </summary>
        public string[] TablesName { get; set; } = null;
        /// <summary>
        /// 实现接口
        /// </summary>
        public string InterfaceName { get; set; } = string.Empty;
        /// <summary>
        /// 是否序列化（默认是）
        /// </summary>
        public bool IsSerializable { get; set; } = true;
    }
}
