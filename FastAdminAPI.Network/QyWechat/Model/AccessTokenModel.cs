namespace FastAdminAPI.Network.QyWechat.Model
{
#pragma warning disable IDE1006 // ������ʽ
    public class AccessTokenModel
    {
        public int errcode { get; set; }

        public string errmsg { get; set; }

        public string access_token { get; set; }
        public int expires_in { get; set; }
    }
#pragma warning restore IDE1006 // ������ʽ
}
