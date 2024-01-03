using FastAdminAPI.Core.Models.Users;
using FastAdminAPI.Common.Attributes;
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
    }
}
