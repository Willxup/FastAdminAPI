namespace FastAdminAPI.Network.QyWechat.Config
{
    public class BaseQyWechatConfiguration
    {
        /// <summary>
        /// 企业ID
        /// </summary>
        internal virtual string Corpid { get; set; }
        /// <summary>
        /// 应用的凭证密钥
        /// </summary>
        internal virtual string CorpSecret { get; set; }
        /// <summary>
        /// 企业应用的id
        /// </summary>
        internal virtual int Agentid { get; set; }
        /// <summary>
        /// 基础URL
        /// </summary>
        public const string BaseUrl = "https://qyapi.weixin.qq.com";
        /// <summary>
        /// 获取访问令牌
        /// </summary>
        public const string GetAccessToken = $" /cgi-bin/gettoken";
        /// <summary>
        /// 发送应用消息
        /// </summary>
        public const string SendMessage = $" /cgi-bin/message/send";
        /// <summary>
        /// 获取企业微信UserId
        /// </summary>
        public const string GetQyWechatUserId = "/cgi-bin/user/getuserinfo";
    }
}
