using FastAdminAPI.Core.Controllers.BASE;
using FastAdminAPI.Email.Extensions;
using FastAdminAPI.Network.Models.Email;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.Controllers
{
    /// <summary>
    /// EmailAPI
    /// </summary>
    public class EmailApiController : BaseController
    {
        /// <summary>
        /// 构造
        /// </summary>
        public EmailApiController() { }

        /// <summary>
        /// 发送Smtp邮件
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task SendSmtpEmail([FromBody] SendSmtpEmailModel model)
        {
            await EmailTool.SendSmtpEmailAsync(model);
        }
        /// <summary>
        /// 发送smtp邮件(支持多个收件人及抄送人)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task SendMultipleSmtpEmail([FromBody] SendMultipleSmtpEmailModel model)
        {
            await EmailTool.SendMultipleSmtpEmailAsync(model);
        }
        /// <summary>
        /// 默认配置发送邮件
        /// </summary>
        /// <param name="subject">邮件主题</param>
        /// <param name="body">邮件内容</param>
        /// <returns></returns>
        [HttpGet]
        public async Task SendEmailByDefault([FromQuery][Required(ErrorMessage = "邮件主题不能为空!")] string subject, 
            [FromQuery][Required(ErrorMessage = "邮件内容不能为空!")] string body)
        {
            await EmailTool.SendEmailByDefaultAsync(subject, body);
        }
    }
}
