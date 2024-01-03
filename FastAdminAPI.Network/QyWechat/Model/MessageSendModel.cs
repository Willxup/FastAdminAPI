namespace FastAdminAPI.Network.QyWechat.Model
{
#pragma warning disable IDE1006 // 命名样式
    public class Content
    {
        public string content { get; set; }
    }
    public class MessageSendBaseModel
    {
        /// <summary>
        /// 指定接收消息的成员，成员ID列表（多个接收者用‘|’分隔，最多支持1000个）。
        /// 特殊情况：指定为”@all”，则向该企业应用的全部成员发送
        /// </summary>
        public string touser { get; set; }
        /// <summary>
        /// 指定接收消息的部门，部门ID列表，多个接收者用‘|’分隔，最多支持100个。
        /// 当touser为”@all”时忽略本参数
        /// </summary>
        public string toparty { get; set; }
        /// <summary>
        /// 指定接收消息的标签，标签ID列表，多个接收者用‘|’分隔，最多支持100个。 
        /// 当touser为”@all”时忽略本参数
        /// </summary>
        public string totag { get; set; }

        /// <summary>
        /// 企业应用的id，整型。企业内部开发，可在应用的设置页面查看
        /// </summary>
        public int agentid { get; set; }
        /// <summary>
        /// 表示是否是保密消息，0表示否，1表示是，默认0
        /// </summary>
        public int safe { get; set; } = 0;
        /// <summary>
        /// 表示是否开启id转译，0表示否，1表示是，默认0
        /// </summary>
        public int enable_id_trans { get; set; } = 0;
        /// <summary>
        /// 表示是否开启重复消息检查，0表示否，1表示是，默认0
        /// </summary>
        public int enable_duplicate_check { get; set; } = 0;
        /// <summary>
        /// 表示是否重复消息检查的时间间隔，默认1800s，最大不超过4小时
        /// </summary>
        public int duplicate_check_interval { get; set; } = 1800;
    }
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
    public class MessageSendResultModel
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
        /// <summary>
        /// 返回无效的部分
        /// </summary>
        public string invaliduser { get; set; }
        public string invalidparty { get; set; }
        public string invalidtag { get; set; }
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
