using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastAdminAPI.Network.QyWechat.Model
{
#pragma warning disable IDE1006 // 命名样式
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
#pragma warning restore IDE1006 // 命名样式
}
