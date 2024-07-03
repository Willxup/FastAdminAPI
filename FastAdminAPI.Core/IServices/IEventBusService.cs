using System.Threading.Tasks;

namespace FastAdminAPI.Core.IServices
{
    public interface IEventBusService
    {
        /// <summary>
        /// 发布信息
        /// </summary>
        /// <param name="msg">信息</param>
        /// <returns></returns>
        Task PublishMessage(string msg);
    }
}
