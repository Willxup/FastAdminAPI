using FastAdminAPI.Common.BASE;
using FastAdminAPI.Core.Models.UserPermission;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.IServices
{
    /// <summary>
    /// 用户功能权限
    /// </summary>
    public interface IUserPermissionService
    {
        /// <summary>
        /// 获取所有用户功能权限列表
        /// </summary>
        /// <param name="pageSearch"></param>
        /// <returns></returns>
        Task<ResponseModel> GetUserPermissions(UserPermssionPageSearch pageSearch);
    }
}
