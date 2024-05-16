namespace FastAdminAPI.Network.Wechat.Models
{
#pragma warning disable IDE1006 // 命名样式
    public class WeChatOfficialAccountsModel
    {
        /// <summary>
        /// 微信公众号内部序号 1韩流姐微信公众号(默认) 2菲律宾留学通 3学在白石 4韩腾教育
        /// </summary>
        public int WechatOfficialAccountNo { get; set; }
        /// <summary>
        /// 微信公众号名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// AppId
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// AppSecret
        /// </summary>
        public string AppSecret { get; set; }
    }
    public class WechatWebAuthorizationModel
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public int errcode { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string errmsg { get; set; }
        /// <summary>
        /// 网页授权接口调用凭证
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// access_token接口调用凭证超时时间，单位（秒）
        /// </summary>
        public int expires_in { get; set; }
        /// <summary>
        /// 用户刷新access_token
        /// </summary>
        public string refresh_token { get; set; }
        /// <summary>
        /// 用户唯一标识，请注意，在未关注公众号时，用户访问公众号的网页，也会产生一个用户和公众号唯一的OpenID
        /// </summary>
        public string openid { get; set; }
        /// <summary>
        /// 用户授权的作用域，使用逗号（,）分隔
        /// </summary>
        public string scope { get; set; }
        /// <summary>
        /// 是否为快照页模式虚拟账号，只有当用户是快照页模式虚拟账号时返回，值为1
        /// </summary>
        public int is_snapshotuser { get; set; }
        /// <summary>
        /// 用户统一标识（针对一个微信开放平台账号下的应用，同一用户的 unionid 是唯一的），只有当scope为"snsapi_userinfo"时返回
        /// </summary>
        public string unionid { get; set; }
    }
#pragma warning restore IDE1006 // 命名样式
}
