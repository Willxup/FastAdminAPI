namespace FastAdminAPI.Network.QyWechat.Models
{
#pragma warning disable IDE1006 // 命名样式
    public class AccessTokenModel
    {
        public int errcode { get; set; }

        public string errmsg { get; set; }

        public string access_token { get; set; }
        public int expires_in { get; set; }
    }
#pragma warning restore IDE1006 // 命名样式
}
