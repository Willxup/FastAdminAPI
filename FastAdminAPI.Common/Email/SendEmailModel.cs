using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FastAdminAPI.Common.Email
{
    #region 内部使用
    public enum EmailTextFormat
    {
        /// <summary>
        /// 纯文本格式
        /// </summary>
        Plain,
        /// <summary>
        /// 纯文本格式的别名(相当于Plain)
        /// </summary>
        Text,
        /// <summary>
        /// 流文本格式
        /// </summary>
        Flowed,
        /// <summary>
        /// HTML文本格式
        /// </summary>
        Html,
        /// <summary>
        /// 富文本格式ETF
        /// </summary>
        Enriched,
        /// <summary>
        /// 压缩的富文本格式
        /// </summary>
        CompressedRichText,
        /// <summary>
        /// 富文本格式RTF
        /// </summary>
        RichText,
    }
    #endregion

    #region Apollo配置发送
    public class SendEmailByApolloConfigModel
    {
        /// <summary>
        /// 邮件主题
        /// </summary>
        [Required(ErrorMessage = "邮件主题不能为空!")]
        public string Subject { get; set; }
        /// <summary>
        /// 邮件内容
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// 文本格式(默认Plain)
        /// </summary>
        public EmailTextFormat Format { get; set; } = EmailTextFormat.Plain;
    }
    #endregion

    #region 单发
    public class SendEmailModel
    {
        /// <summary>
        /// 发件人账户
        /// </summary>
        public string FromAccount { get; set; }

        /// <summary>
        /// 发件人密码
        /// </summary>
        public string FromPassword { get; set; }

        /// <summary>
        /// 发件人邮箱
        /// </summary>
        public string FromAddress { get; set; }

        /// <summary>
        /// 收件人邮箱
        /// </summary>
        public string ToAddress { get; set; }

        /// <summary>
        /// 抄送人邮箱
        /// </summary>
        public string CCAddress { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 发件人名称
        /// </summary>
        public string FromName { get; set; }

        /// <summary>
        /// 收件人名称
        /// </summary>
        public string ToName { get; set; }

        /// <summary>
        /// 抄送人名称
        /// </summary>
        public string CCName { get; set; }
    }
    public class SendSmtpEmailModel : SendEmailModel
    {
        /// <summary>
        /// smtp服务地址
        /// </summary>
        public string SmtpServer { get; set; }
        /// <summary>
        /// smtp服务端口号
        /// </summary>
        public int? SmtpPort { get; set; }
        /// <summary>
        /// 文本格式(默认Plain)
        /// </summary>
        public EmailTextFormat Format { get; set; } = EmailTextFormat.Plain;
    }
    #endregion

    #region 多发
    public class SendEmailMultipleModel
    {
        /// <summary>
        /// 发件人账户
        /// </summary>
        public string FromAccount { get; set; }

        /// <summary>
        /// 发件人密码
        /// </summary>
        public string FromPassword { get; set; }

        /// <summary>
        /// 发件人邮箱
        /// </summary>
        public string FromAddress { get; set; }
        /// <summary>
        /// 发件人名称
        /// </summary>
        public string FromName { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// 收件人
        /// </summary>
        public List<SendEmailReceiveModel> ReceiverList { get; set; }
        /// <summary>
        /// 抄送人
        /// </summary>
        public List<SendEmailReceiveModel> CCReceiverList { get; set; }
    }
    public class SendEmailReceiveModel
    {
        /// <summary>
        /// 收件人名称
        /// </summary>
        public string ToName { get; set; } = string.Empty;
        /// <summary>
        /// 收件人邮箱
        /// </summary>
        public string ToAddress { get; set; }
    }
    public class SendMultipleSmtpEmailModel : SendEmailMultipleModel
    {
        /// <summary>
        /// smtp服务地址
        /// </summary>
        public string SmtpServer { get; set; }
        /// <summary>
        /// smtp服务端口号
        /// </summary>
        public int? SmtpPort { get; set; }
        /// <summary>
        /// 文本格式(默认Plain)
        /// </summary>
        public EmailTextFormat Format { get; set; } = EmailTextFormat.Plain;
    }
    #endregion
}
