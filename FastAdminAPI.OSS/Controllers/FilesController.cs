using FastAdminAPI.Common.BASE;
using FastAdminAPI.OSS.Controllers.BASE;
using FastAdminAPI.OSS.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace FastAdminAPI.OSS.Controllers
{
    /// <summary>
    /// 文件
    /// </summary>
    public class FilesController : BaseController
    {
        /// <summary>
        /// 文件Service
        /// </summary>
        private readonly IFilesService _files;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="files"></param>
        public FilesController(IFilesService files)
        {
            _files = files;
        }

        /// <summary>
        /// 获取服务器域名
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseModel> GetServerDomain()
        {
            return Success(await _files.GetServerDomain());
        }
        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="file">文件</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseModel> Upload(IFormFile file)
        {
            return await _files.Upload(file);
        }
        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="filepath">文件地址</param>
        /// <param name="filename">文件名(可选)</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Download(string filepath, string filename = null)
        {
            var buffer = await _files.Download(filepath);

            if (buffer != null)
            {
                filename ??= filepath.Replace(Path.DirectorySeparatorChar + "", "")[22..];

                return File(buffer, "application/octet-stream", filename);
            }
            else
            {
                return NotFound();
            }
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filepath">文件地址</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ResponseModel> Delete(string filepath)
        {
            return await _files.Delete(filepath);
        }
    }
}
