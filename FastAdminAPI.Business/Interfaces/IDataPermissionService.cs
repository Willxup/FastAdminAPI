using System.Collections.Generic;
using System.Threading.Tasks;

namespace FastAdminAPI.Business.Interfaces
{
    /// <summary>
    /// 数据权限
    /// </summary>
    public interface IDataPermissionService
    {
        /// <summary>
        /// 获取数据权限
        /// </summary>
        /// <returns></returns>
        Task<List<long>> Get();

        /// <summary>
        /// 释放数据权限
        /// </summary>
        /// <returns></returns>
        Task<bool> Release();

        /// <summary>
        /// 释放数据权限
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        Task<bool> Release(long employeeId);
    }
}
