using System.Collections.Generic;
using System.Threading.Tasks;
using FastAdminAPI.Common.BASE;
using FastAdminAPI.Core.Models.Modules;

namespace FastAdminAPI.Core.IServices
{
    /// <summary>
    /// 模块
    /// </summary>
    public interface IModuleService
    {
        /// <summary>
        /// 获取模块树
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <returns></returns>
        Task<string> GetModuleTree(string moduleName = null);
        /// <summary>
        /// 新增模块
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResponseModel> AddModule(AddModuleModel model);
        /// <summary>
        /// 编辑模块
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResponseModel> EditModule(EditModuleModel model);
        /// <summary>
        /// 删除模块
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        Task<ResponseModel> DelModule(long moduleId);
        /// <summary>
        /// 按模块Id获取员工列表
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        Task<List<long>> GetEmployeeListByModuleId(long moduleId);
    }
}
