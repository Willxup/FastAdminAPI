﻿using System.ComponentModel.DataAnnotations;

namespace FastAdminAPI.Network.Models.Wechat
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
}
