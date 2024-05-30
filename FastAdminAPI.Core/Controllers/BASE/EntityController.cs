using FastAdminAPI.Core.IServices.BASE;
using FastAdminAPI.Core.Models.BASE;
using Microsoft.AspNetCore.Mvc;

namespace FastAdminAPI.Core.Controllers.BASE
{
    /// <summary>
    /// 实体
    /// </summary>
    public class EntityController : BaseController
    {
        private readonly IEntityService _entityService;

        public EntityController(IEntityService entityService)
        {
            _entityService = entityService;
        }
        /// <summary>
        /// 生成实体类(本地环境使用)
        /// </summary>
        [HttpGet]
        #if !DEBUG
        [NonAction]
        #endif
        public void GenerateEntities()
        {
            _entityService.GenerateEntities();
        }
        /// <summary>
        /// 按指定表生成实体(本地环境使用)
        /// </summary>
        /// <param name="tables">需要生成的表</param>
        [HttpGet]
        #if !DEBUG
        [NonAction]
        #endif
        public void GenerateEntitiesByAssignTables(string[] tables)
        {
            _entityService.GenerateEntitiesByAssignTables(tables);
        }
        /// <summary>
        /// 自定义生成实体(本地环境使用)
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        #if !DEBUG
        [NonAction]
        #endif
        public void GenerateEntitiesByCustom([FromBody] GenerateDbEntitiesModel model)
        {
            _entityService.GenerateEntitiesByCustom(model);
        }
    }
}
