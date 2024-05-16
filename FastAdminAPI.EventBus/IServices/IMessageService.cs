using System.Threading.Tasks;

namespace FastAdminAPI.EventBus.IServices
{
    /// <summary>
    /// 信息中心
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="msg">信息</param>
        /// <returns></returns>
        Task SendMessage(string msg);
    }
}
