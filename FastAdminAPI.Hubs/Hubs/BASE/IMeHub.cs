using System.Threading.Tasks;

namespace FastAdminAPI.Hubs.Hubs.BASE
{
    /// <summary>
    /// MeHub 客户端方法
    /// </summary>
    public interface IMeHub
    {
        /// <summary>
        /// 设置未读消息数
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public Task SetUnreadMessageCount(int count);
    }
}
