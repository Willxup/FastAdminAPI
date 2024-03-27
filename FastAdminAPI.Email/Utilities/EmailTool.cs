using FastAdminAPI.Common.Logs;
using FastAdminAPI.Network.Models.Email;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastAdminAPI.Email.Extensions
{
    public static class EmailTool
    {
        /// <summary>
        /// SMTP服务器
        /// </summary>
        private const string SMTP_SERVER = "smtp.exmail.qq.com";
        /// <summary>
        /// SMTP服务器端口号
        /// </summary>
        private const int SMTP_PORT = 465;
        /// <summary>
        /// 默认账号
        /// </summary>
        private const string DEFAULT_ACCOUNT = "service@willxup.top";
        /// <summary>
        /// 默认密码
        /// </summary>
        private const string DEFAULT_PASSWORD = "123456";
        /// <summary>
        /// 默认地址
        /// </summary>
        private const string DEFAULT_ADDRESS = "service@willxup.top";
        /// <summary>
        /// 默认接收人
        /// </summary>
        private const string DEFAULT_RECEIVERS = "test1@willxup.top,test2@willxup.top";

        #region 内部使用
        /// <summary>
        /// 转换文本格式
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static TextFormat ConvertTextFormat(EmailTextFormat format)
        {
            //将文本模式进行转换，转换为MimeKit的枚举类型
            return format switch
            {
                EmailTextFormat.Plain => TextFormat.Plain,
                EmailTextFormat.Text => TextFormat.Text,
                EmailTextFormat.Flowed => TextFormat.Flowed,
                EmailTextFormat.Html => TextFormat.Html,
                EmailTextFormat.Enriched => TextFormat.Enriched,
                EmailTextFormat.CompressedRichText => TextFormat.CompressedRichText,
                EmailTextFormat.RichText => TextFormat.RichText,
                _ => throw new Exception("文本格式有误!")
            };
        }
        /// <summary>
        /// 发送Smtp邮件
        /// </summary>
        /// <param name="emailModel">邮件实体</param>
        /// <param name="format">文本格式</param>
        private static void SendSmtpEmail(SendSmtpEmailModel emailModel)
        {
            try
            {
                MimeMessage message = new();

                //发送人
                message.From.Add(new MailboxAddress(emailModel.FromName, emailModel.FromAddress));

                //接收人
                message.To.Add(new MailboxAddress(emailModel.ToName, emailModel.ToAddress));

                //抄送人
                if (!string.IsNullOrEmpty(emailModel.CCAddress))
                {
                    message.Cc.Add(new MailboxAddress(emailModel.CCName, emailModel.CCAddress));
                }

                //主题
                message.Subject = emailModel.Subject;

                //内容
                message.Body = new TextPart(ConvertTextFormat(emailModel.Format))
                {
                    Text = @emailModel.Body
                };

                using SmtpClient client = new();

                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect(emailModel.SmtpServer, (int)emailModel.SmtpPort, true);

                client.Authenticate(emailModel.FromAccount, emailModel.FromPassword);

                client.Send(message);

                client.Disconnect(true);
            }
            catch (Exception ex)
            {
                NLogHelper.Error($"发送邮件失败! {ex.Message}", ex);
            }
        }
        /// <summary>
        /// 发送smtp邮件(支持多个收件人及抄送人)
        /// </summary>
        /// <param name="emailModel"></param>
        private static void SendMultipleSmtpEmail(SendMultipleSmtpEmailModel emailModel)
        {
            if (emailModel.ReceiverList?.Count > 0)
            {
                try
                {
                    MimeMessage message = new();

                    //发送人
                    message.From.Add(new MailboxAddress(emailModel.FromName, emailModel.FromAddress));

                    //接收人
                    emailModel.ReceiverList.ForEach(item =>
                    {
                        message.To.Add(new MailboxAddress(item.ToName, item.ToAddress));
                    });

                    //抄送人
                    if (emailModel.CCReceiverList?.Count > 0)
                    {
                        emailModel.CCReceiverList.ForEach(item =>
                        {
                            message.Cc.Add(new MailboxAddress(item.ToName, item.ToAddress));
                        });
                    }

                    //主题
                    message.Subject = emailModel.Subject;

                    //内容
                    message.Body = new TextPart(ConvertTextFormat(emailModel.Format))
                    {
                        Text = @emailModel.Body
                    };

                    using SmtpClient client = new();

                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    client.Connect(emailModel.SmtpServer, (int)emailModel.SmtpPort, true);

                    client.Authenticate(emailModel.FromAccount, emailModel.FromPassword);

                    client.Send(message);

                    client.Disconnect(true);
                }
                catch (Exception ex)
                {
                    NLogHelper.Error($"发送邮件失败(多发)! {ex.Message}", ex);
                }
            }
            else
                throw new Exception("发送邮件失败，请添加收件人!");

        }
        #endregion

        /// <summary>
        /// 发送smtp邮件
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="emailModel"></param>
        public static async Task SendSmtpEmailAsync(SendSmtpEmailModel emailModel)
        {

            emailModel.SmtpServer = !string.IsNullOrWhiteSpace(emailModel.SmtpServer) ? emailModel.SmtpServer : SMTP_SERVER;
            emailModel.SmtpPort = emailModel.SmtpPort != null ? emailModel.SmtpPort : SMTP_PORT;

            //同步转异步
            await Task.Run(() =>
            {
                SendSmtpEmail(emailModel);
            });

        }
        /// <summary>
        /// 发送smtp邮件(支持多个收件人及抄送人)
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="emailModel"></param>
        public static async Task SendMultipleSmtpEmailAsync(SendMultipleSmtpEmailModel emailModel)
        {
            emailModel.SmtpServer = !string.IsNullOrWhiteSpace(emailModel.SmtpServer) ? emailModel.SmtpServer : SMTP_SERVER;
            emailModel.SmtpPort = emailModel.SmtpPort != null ? emailModel.SmtpPort : SMTP_PORT;

            //同步转异步
            await Task.Run(() =>
            {
                SendMultipleSmtpEmail(emailModel);
            });

        }
        /// <summary>
        /// 默认配置发送邮件
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="type">服务器类型 1smtp 2pop 3imap</param>
        /// <param name="format">文本格式</param>
        public static async Task SendEmailByDefaultAsync(string subject, string body, EmailTextFormat format = EmailTextFormat.Plain)
        {
            try
            {
                SendMultipleSmtpEmailModel emailModel = new()
                {
                    Subject = subject,
                    Body = @body,
                    SmtpServer = SMTP_SERVER,
                    SmtpPort = SMTP_PORT,
                    Format = format,

                    FromAccount = DEFAULT_ACCOUNT,
                    FromPassword = DEFAULT_PASSWORD,
                    FromAddress = DEFAULT_ADDRESS
                };

                var ToAllAddress = DEFAULT_RECEIVERS?.Split(",").ToList();
                List<SendEmailReceiveModel> receivers = new();
                ToAllAddress.ForEach(item => { receivers.Add(new SendEmailReceiveModel { ToAddress = item }); });
                emailModel.ReceiverList = receivers;

                //同步转异步
                await Task.Run(() =>
                {
                    SendMultipleSmtpEmail(emailModel);
                });
            }
            catch (Exception ex)
            {
                NLogHelper.Error($"发送邮件失败! {ex.Message}", ex);
            }
        }
    }
}
