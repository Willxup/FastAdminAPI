using FastAdminAPI.Common.Attributes;
using FastAdminAPI.Common.BASE;
using FastAdminAPI.Common.Logs;
using FastAdminAPI.Common.Utilities;
using FastAdminAPI.Configuration.BASE;
using FastAdminAPI.OSS.IServices;
using FastAdminAPI.OSS.Services.BASE;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SqlSugar;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FastAdminAPI.OSS.Services
{
    /// <summary>
    /// 文件
    /// </summary>
    public class FilesService : BaseService, IFilesService
    {
        /// <summary>
        /// 服务域名
        /// </summary>
        private readonly string SERVER_DOMAIN;
        /// <summary>
        /// 文件存储目录
        /// </summary>
        private readonly string STORAGE_PATH = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", Define.STORAGE_DIRECTORY);
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="httpContext"></param>
        /// <param name="configuration"></param>
        public FilesService(ISqlSugarClient dbContext, IHttpContextAccessor httpContext, IConfiguration configuration) : base(dbContext, httpContext)
        {
            SERVER_DOMAIN = configuration.GetValue<string>("FastAdminAPI.OSS.Domain");
        }

        #region 私有方法
        /// <summary>
        /// 格式化url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string FormatUrl(string url)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                string path = url.Replace(SERVER_DOMAIN, "");
                int questionMarkIndex = path.IndexOf('?');
                if (questionMarkIndex != -1)
                {
                    path = path[..questionMarkIndex];
                }

                return path;
            }

            return null;
        }
        #endregion

        /// <summary>
        /// 获取服务器域名
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetServerDomain()
        {
            return await Task.FromResult(SERVER_DOMAIN);
        }
        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="directory">目录名</param>
        /// <param name="file">文件</param>
        /// <returns></returns>
        /// <exception cref="UserOperationException"></exception>
        public async Task<ResponseModel> Upload(IFormFile file, string directory = "files")
        {

            var content = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
            var fileName = content.FileName.Trim('"');
            string[] splits = fileName.Split(".");
            if (splits == null || splits.Length < 2)
                throw new Exception("获取文件格式失败");
            string type = splits[1];
            string name = string.Join("", splits[..^1]).Replace("-", "").Replace(":", "").Replace(Path.DirectorySeparatorChar + "", "");

            fileName = $"{GuidConverter.GenerateShortGuid()}{name}.{type}";
            string filePath = STORAGE_PATH + Path.DirectorySeparatorChar + directory + Path.DirectorySeparatorChar +
                DateTime.Now.ToString("yyyyMM") + Path.DirectorySeparatorChar;

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            string fileFullName = filePath + fileName;
            using FileStream fs = File.Create(fileFullName);

            try
            {
                await file.CopyToAsync(fs);
                await fs.FlushAsync();

                return ResponseModel.Success(fileFullName.Replace(STORAGE_PATH, ""));
            }
            catch (Exception ex)
            {
                NLogHelper.Error($"文件上传失败，{ex.Message}", ex);
                throw new UserOperationException("文件上传失败!");
            }
            finally
            {
                fs.Close();
            }
        }
        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="filepath">文件地址</param>
        /// <returns></returns>
        public async Task<byte[]> Download(string filepath)
        {
            filepath = FormatUrl(filepath);

            string path = STORAGE_PATH + filepath;

            if (!File.Exists(path))
                return null;

            return await File.ReadAllBytesAsync(path);
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filepath">文件地址</param>
        /// <returns></returns>
        public async Task<ResponseModel> Delete(string filepath)
        {
            filepath = FormatUrl(filepath);
            string path = STORAGE_PATH + filepath;

            if (!File.Exists(path))
            {
                File.Delete(path);
            }

            return await Task.FromResult(ResponseModel.Success());
        }
    }
}
