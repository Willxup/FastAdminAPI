using System.Collections.Generic;
using System.Threading.Tasks;
using FastAdminAPI.Business.Models.Region;

namespace FastAdminAPI.Business.Interfaces
{
    /// <summary>
    /// 区域服务
    /// </summary>
    public interface IRegionService
    {
        /// <summary>
        /// 将区域信息存入redis(自动重试)
        /// </summary>
        /// <param name="RetryTimes">重试次数，默认2次</param>
        /// <returns></returns>
        Task<List<RegionModel>> Get(int RetryTimes = 2);

        /// <summary>
        /// 按区县代号获取完整区域信息
        /// </summary>
        /// <param name="regionCode">地区/县code</param>
        /// <returns></returns>
        Task<Dictionary<string, string>> GetFullRegion(string regionCode);
        /// <summary>
        /// 获取完整区域信息
        /// </summary>
        /// <param name="provinceCode">省份</param>
        /// <param name="cityCode">城市</param>
        /// <param name="regionCode">地区/县</param>
        /// <returns></returns>
        Task<Dictionary<string, string>> GetFullRegion(string provinceCode, string cityCode, string regionCode);
        /// <summary>
        /// 获取区域名称
        /// </summary>
        /// <param name="regionCode">区域代码</param>
        /// <returns></returns>
        Task<string> GetRegionName(string regionCode);
    }
}
