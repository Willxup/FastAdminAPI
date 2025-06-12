using System.Threading.Tasks;
using FastAdminAPI.Common.BASE;
using Microsoft.AspNetCore.Http;

namespace FastAdminAPI.OSS.IServices
{
    /// <summary>
    /// 文件
    /// </summary>
    public interface IFilesService
    {
        /// <summary>
        /// 获取服务器域名
        /// </summary>
        /// <returns></returns>
        Task<string> GetServerDomain();
        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="directory">目录名</param>
        /// <param name="file">文件</param>
        /// <returns></returns>
        Task<ResponseModel> Upload(IFormFile file, string directory = "files");
        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="filepath">文件地址</param>
        /// <returns></returns>
        Task<byte[]> Download(string filepath);
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filepath">文件地址</param>
        /// <returns></returns>
        Task<ResponseModel> Delete(string filepath);
    }
}
