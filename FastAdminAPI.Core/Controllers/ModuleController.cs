using FastAdminAPI.Common.BASE;
using FastAdminAPI.Core.Controllers.BASE;
using FastAdminAPI.Core.IServices;
using FastAdminAPI.Core.Models.Employee;
using FastAdminAPI.Core.Models.Modules;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.Controllers
{
    /// <summary>
    /// 模块
    /// </summary>
    public class ModuleController : BaseController
    {
        /// <summary>
        /// 模块Service
        /// </summary>
        private readonly IModuleService _moduleService;
        /// <summary>
        /// 员工Service
        /// </summary>
        private readonly IEmployeeService _employeeService;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="moduleService"></param>
        /// <param name="employeeService"></param>
        public ModuleController(IModuleService moduleService, IEmployeeService employeeService)
        {
            _moduleService = moduleService;
            _employeeService = employeeService;
        }

        /// <summary>
        /// 获取模块树
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ModuleInfoModel>), 200)]
        public async Task<ResponseModel> GetModuleTree([FromQuery] string moduleName = null)
        {
            return Success(await _moduleService.GetModuleTree(moduleName));
        }
        /// <summary>
        /// 新增模块
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseModel> AddModule([FromBody] AddModuleModel model)
        {
            return await _moduleService.AddModule(model);
        }
        /// <summary>
        /// 编辑模块
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ResponseModel> EditModule([FromBody] EditModuleModel model)
        {
            return await _moduleService.EditModule(model);
        }
        /// <summary>
        /// 删除模块
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ResponseModel> DelModule([FromQuery][Required(ErrorMessage = "模块Id不能为空!")] long? moduleId)
        {
            return await _moduleService.DelModule((long)moduleId);
        }
        /// <summary>
        /// 按模块Id获取员工列表
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<EmployeePageResult>), 200)]
        public async Task<ResponseModel> GetEmployeeListByModuleId([FromQuery][Required(ErrorMessage = "模块Id不能为空!")] long? moduleId)
        {
            var userIds = await _moduleService.GetEmployeeListByModuleId((long)moduleId);
            if (userIds?.Count > 0)
            {
                return await _employeeService.GetEmployeeList(new EmployeePageSearch { UserIds = userIds });
            }
            return Success();
        }
    }
}
