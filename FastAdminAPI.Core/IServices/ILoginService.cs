using FastAdminAPI.Common.Attributes;
using FastAdminAPI.Core.Models.Users;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.IServices
{
    /// <summary>
    /// 登录
    /// </summary>
    public interface ILoginService
    {
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="qyUserId">企业微信UserId</param>
        /// <returns></returns>
        /// <exception cref="UserOperationException"></exception>
        Task<UserModel> GetUser(string qyUserId);
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        /// <exception cref="UserOperationException"></exception>
        Task<UserModel> GetUser(string account, string password);
        /// <summary>
        /// 记录用户登录
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="employeeId">员工Id</param>
        /// <param name="device">登录设备</param>
        /// <returns></returns>
        Task RecordLogin(long userId, long employeeId, int device);
    }
}
