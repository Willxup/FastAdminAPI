using System.Collections.Generic;

namespace FastAdminAPI.Network.QyWechat.Model
{
#pragma warning disable IDE1006 // 命名样式
    public class QyUserInfoModel
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
        public string UserId { get; set; }
        public string DeviceId { get; set; }
    }
#pragma warning restore IDE1006 // 命名样式
}
