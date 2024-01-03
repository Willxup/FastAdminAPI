using System.Threading.Tasks;

namespace FastAdminAPI.EventBus.IServices
{
    /// <summary>
    /// 系统
    /// </summary>
    public interface ISystemService
    {
        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="msg">信息</param>
        /// <returns></returns>
        Task SendMessage(string msg);
    }
}
