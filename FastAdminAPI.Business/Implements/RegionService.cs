﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastAdminAPI.Business.Interfaces;
using FastAdminAPI.Business.Models.Region;
using FastAdminAPI.Common.Redis;
using FastAdminAPI.Framework.Entities;
using SqlSugar;

namespace FastAdminAPI.Business.Implements
{
    /// <summary>
    /// 区域服务
    /// </summary>
    internal class RegionService : IRegionService
    {
        /// <summary>
        /// SugarScope
        /// </summary>
        protected SqlSugarScope _dbContext;
        /// <summary>
        /// Redis
        /// </summary>
        private readonly IRedisHelper _redis;

        public RegionService(ISqlSugarClient dbContext, IRedisHelper redis) 
        {
            _dbContext = dbContext as SqlSugarScope;
            _redis = redis;
        }

        #region 通用方法

        #region 私有方法
        /// <summary>
        /// 将区域信息存入redis
        /// </summary>
        /// <returns></returns>
        private async Task<bool> Set()
        {
            var regionList = await _dbContext.Queryable<S98_RegionInfo>()
                .Select(S98 => new RegionModel
                {
                    RegionId = S98.S98_REGION_ID,
                    ParentId = S98.S98_PARENT_ID,
                    RegionCode = S98.S98_REGION_CODE,
                    RegionName = S98.S98_REGION_NAME
                }).ToListAsync();
            return await _redis.StringSetAsync("Common:RegionInfo", regionList);
        } 
        #endregion

        /// <summary>
        /// 将区域信息存入redis(自动重试)
        /// </summary>
        /// <param name="RetryTimes">重试次数，默认2次</param>
        /// <returns></returns>
        public async Task<List<RegionModel>> Get(int RetryTimes = 2)
        {
            var regionInfo = await _redis.StringGetAsync<List<RegionModel>>("Common:RegionInfo");

            if (!regionInfo?.Any() ?? true)
            {
                regionInfo = null;//默认值
                for (int i = 0; i < RetryTimes; i++)
                {
                    bool isSuccuss = await Set();

                    if (isSuccuss)
                    {
                        regionInfo = await _redis.StringGetAsync<List<RegionModel>>("Common:RegionInfo");
                        break;
                    }
                }
            }
            return regionInfo;
        }
        #endregion

        #region 扩展方法
        /// <summary>
        /// 按区县代号获取完整区域信息
        /// </summary>
        /// <param name="regionCode">地区/县</param>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> GetFullRegion(string regionCode)
        {
            // 获取区域信息
            var positionInfo = await Get();
            
            if (positionInfo == null)
                return null;

            Dictionary<string, string> position = new();

            // 获取区域
            var region = positionInfo.Where(r => r.RegionCode == regionCode)
                                    .Select(c => new { c.RegionCode, c.RegionName, c.ParentId })
                                    .FirstOrDefault();
            if (region == null)
                return null;

            // 获取城市
            var city = positionInfo.Where(r => r.RegionId == region.ParentId)
                                 .Select(c => new { c.RegionCode, c.RegionName, c.ParentId })
                                 .FirstOrDefault();
            if (city == null)
                return null;

            // 获取省份
            var province = positionInfo.Where(c => c.RegionId == city.ParentId)
                                     .Select(c => new { c.RegionCode, c.RegionName })
                                     .FirstOrDefault();
            if (province == null)
                return null;

            // 省份
            position.Add(province.RegionCode, province.RegionName);

            // 城市
            position.Add(city.RegionCode, city.RegionName);

            // 区域
            position.Add(region.RegionCode, region.RegionName);

            return position;
        }
        /// <summary>
        /// 获取完整区域信息
        /// </summary>
        /// <param name="provinceCode">省份</param>
        /// <param name="cityCode">城市</param>
        /// <param name="regionCode">地区/县</param>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> GetFullRegion(string provinceCode, string cityCode, string regionCode)
        {
            // 获取区域信息
            var positionInfo = await Get();

            // 获取省份
            var province = positionInfo?.Where(c => c.RegionCode == provinceCode).Select(c => new { c.RegionCode, c.RegionName }).FirstOrDefault();

            // 获取城市
            var city = positionInfo?.Where(c => c.RegionCode == cityCode).Select(c => new { c.RegionCode, c.RegionName }).FirstOrDefault();

            // 获取区域
            var region = positionInfo?.Where(c => c.RegionCode == regionCode).Select(c => new { c.RegionCode, c.RegionName }).FirstOrDefault();

            // 返回省市区
            if (province != null && city != null && region != null)
            {
                Dictionary<string, string> position = new()
                {
                    { province.RegionCode, province.RegionName },
                    { city.RegionCode, city.RegionName },
                    { region.RegionCode, region.RegionName }
                };
                return position;
            }

            return null;
        }
        /// <summary>
        /// 获取区域名称
        /// </summary>
        /// <param name="regionCode">区域代码</param>
        /// <returns></returns>
        public async Task<string> GetRegionName(string regionCode)
        {
            string regionName = string.Empty;

            var regionInfo = await Get();

            if (regionInfo?.Count > 0)
            {
                regionName = regionInfo.Where(c => c.RegionCode == regionCode).Select(c => c.RegionName).FirstOrDefault();
            }

            return regionName;
        }
        #endregion
    }
}
