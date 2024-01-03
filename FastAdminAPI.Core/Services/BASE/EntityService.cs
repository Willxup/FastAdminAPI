using FastAdminAPI.Core.IServices.BASE;
using FastAdminAPI.Core.Models.BASE;
using FastAdminAPI.Framework.Extensions;
using SqlSugar;

namespace FastAdminAPI.Core.Services.BASE
{
    public class EntityService : BaseService, IEntityService
    {
        public EntityService(ISqlSugarClient dbContext) : base(dbContext) { }

        /// <summary>
        /// 生成实体类(本地环境使用)
        /// </summary>
        public void GenerateEntities()
        {
            DbEntitiesGenerator.GenerateDbEntities(_dbContext);
        }
        /// <summary>
        /// 按所需表生成实体(本地环境使用)
        /// </summary>
        /// <param name="tables">需要生成的表</param>
        public void GenerateEntitiesByTables(string[] tables)
        {
            DbEntitiesGenerator.GenerateDbEntitiesByCustom(_dbContext, tables);
        }
        /// <summary>
        /// 自定义生成实体(本地环境使用)
        /// </summary>
        /// <param name="model"></param>
        public void GenerateEntitiesByCustom(GenerateDbEntitiesModel model)
        {
            DbEntitiesGenerator.GenerateDbEntitiesByCustom(_dbContext, model.Path, model.Namespace, model.TablesName, model.InterfaceName, model.IsSerializable);
        }
    }
}
