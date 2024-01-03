using System.Collections.Generic;
using System.Threading.Tasks;

namespace FastAdminAPI.Business.IServices
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
        Task<List<long>> GetDataPermission();

        /// <summary>
        /// 释放数据权限
        /// </summary>
        /// <returns></returns>
        Task ReleaseDataPermissions();

        /// <summary>
        /// 释放数据权限
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        Task<bool> ReleaseDataPermission(long employeeId);
    }
}
