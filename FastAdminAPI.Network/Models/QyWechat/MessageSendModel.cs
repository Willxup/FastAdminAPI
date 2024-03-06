using FastAdminAPI.Network.QyWechat.Model;

namespace FastAdminAPI.Network.Models.QyWechat
{
#pragma warning disable IDE1006 // 命名样式

    public class MessageSendModel : MessageSendBaseModel
    {
        /// <summary>
        /// 消息类型:text image voice video file
        /// </summary>
        public string msgtype { get; set; } = "text";
        /// <summary>
        /// 消息内容，最长不超过2048个字节，超过将截断（支持id转译）
        /// </summary>
        public Content text { get; set; } = new Content();
    }
    public class Content
    {
        public string content { get; set; }
    }
    public class CardMsgSendModel : MessageSendBaseModel
    {
        /// <summary>
        /// 消息类型:text image voice video file
        /// </summary>
        public string msgtype { get; set; } = "textcard";
        /// <summary>
        /// 消息内容，最长不超过2048个字节，超过将截断（支持id转译）
        /// </summary>
        public Textcard textcard { get; set; } = new Textcard();
    }
    public class Textcard
    {
        public string title { get; set; }
        public string description { get; set; }
        /// <summary>
        /// 点击后跳转的链接
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 按钮文字。 默认为“详情”， 不超过4个文字，超过自动截断。
        /// </summary>
        public string btntxt { get; set; }
    }
#pragma warning restore IDE1006 // 命名样式
}
