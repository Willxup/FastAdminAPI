﻿namespace FastAdminAPI.Network.Models.Wechat
{
    public class WeChatSignModel
    {
        /// <summary>
        /// Appid
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// JSticket
        /// </summary>
        public string JsApiTicket { get; set; }
        /// <summary>
        /// 随机字符串
        /// </summary>
        public string Noncestr { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        public string Timestamp { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string Sign { get; set; }
    }
    public class WeChatSignRequestModel
    {
        /// <summary>
        /// 请求完整地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 微信公众号内部序号 1韩流姐微信公众号(默认) 2菲律宾留学通 3学在白石 4韩腾教育
        /// </summary>
        public int? WechatOfficialAccountNo { get; set; } = 1;
    }
}