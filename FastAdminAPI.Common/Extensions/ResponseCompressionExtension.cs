using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Compression;

namespace FastAdminAPI.Common.Extensions
{
    /// <summary>
    /// 响应压缩
    /// </summary>
    public static class ResponseCompressionExtension
    {
        /// <summary>
        /// 压缩Response
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCompressResponse(this IServiceCollection services)
        {
            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest; //压缩应该尽快完成，即使生成的输出未以最佳方式压缩。
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest; //压缩应该尽快完成，即使生成的输出未以最佳方式压缩。
            });

            //返回压缩
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<BrotliCompressionProvider>(); //br压缩
                options.Providers.Add<GzipCompressionProvider>(); //gzip压缩
            });

            return services;
        }
    }
}
