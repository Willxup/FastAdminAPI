using FastAdminAPI.Network.Models.Email;
using Refit;
using System.Threading.Tasks;

namespace FastAdminAPI.Network.Interfaces
{
    public interface IEmailApi
    {
        /// <summary>
        /// 发送Smtp邮件
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Post("/api/EmailApi/SendSmtpEmail")]
        Task SendSmtpEmail([Body] SendSmtpEmailModel model);
        /// <summary>
        /// 发送Smtp邮件
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Post("/api/EmailApi/SendMultipleSmtpEmail")]
        Task SendMultipleSmtpEmail([Body] SendMultipleSmtpEmailModel model);
        /// <summary>
        /// 发送Smtp邮件
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Get("/api/EmailApi/SendEmailByDefault")]
        Task SendEmailByDefault([Query] string subject, [Query] string body);
    }
}
