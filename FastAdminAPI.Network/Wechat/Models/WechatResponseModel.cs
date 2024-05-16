namespace FastAdminAPI.Network.Wechat.Models
{
#pragma warning disable IDE1006 // 命名样式
    public class WechatTokenModel
    {
        /// <summary>
        /// 错误消息
        /// </summary>
        public string errmsg { get; set; }
        /// <summary>
        /// 错误代码
        /// </summary>
        public int errcode { get; set; }
        /// <summary>
        /// token
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public int expires_in { get; set; }
    }

    public class WechatTicketModel
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
        public string ticket { get; set; }
        public int expires_in { get; set; }
    }
#pragma warning restore IDE1006 // 命名样式
}
