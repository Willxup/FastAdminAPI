using FastAdminAPI.Core.Models.BASE;

namespace FastAdminAPI.Core.IServices.BASE
{
    public interface IEntityService
    {
        /// <summary>
        /// 生成实体类(本地环境使用)
        /// </summary>
        void GenerateEntities();
        /// <summary>
        /// 按指定表生成实体(本地环境使用)
        /// </summary>
        /// <param name="tables">需要生成的表</param>
        void GenerateEntitiesByAssignTables(string[] tables);
        /// <summary>
        /// 自定义生成实体(本地环境使用)
        /// </summary>
        /// <param name="model"></param>
        void GenerateEntitiesByCustom(GenerateDbEntitiesModel model);
    }
}
