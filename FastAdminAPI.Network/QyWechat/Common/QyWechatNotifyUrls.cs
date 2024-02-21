namespace FastAdminAPI.Network.QyWechat.Common
{
    /// <summary>
    /// 企业微信通知地址
    /// </summary>
    public class QyWechatNotifyUrls
    {

        /// <summary>
        /// 企业微信客户端域名
        /// </summary>
        private const string QYWECHAT_CLIENT_DOMAIN = "http://localhost:9000";

        /// <summary>
        /// 获取URL
        /// </summary>
        /// <param name="route">路由(当前类的常量)</param>
        /// <returns></returns>
        public static string Get(string route) => QYWECHAT_CLIENT_DOMAIN + route;

        /// <summary>
        /// 测试通知地址
        /// </summary>
        public const string NOTIFY_TEST_URL = "/notice";
    }
}
