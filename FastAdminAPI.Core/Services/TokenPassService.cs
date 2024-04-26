using FastAdminAPI.Business.Interfaces;
using FastAdminAPI.Business.Models.Region;
using FastAdminAPI.Common.Enums;
using FastAdminAPI.Common.JsonTree;
using FastAdminAPI.Core.IServices;
using FastAdminAPI.Core.Models.TokenPass;
using FastAdminAPI.Core.Services.BASE;
using FastAdminAPI.Framework.Entities;
using SqlSugar;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.Services
{
    /// <summary>
    /// 令牌过滤
    /// </summary>
    public class TokenPassService : BaseService, ITokenPassService
    {
        /// <summary>
        /// 区域服务
        /// </summary>
        private readonly IRegionService _regionService;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="regionService"></param>
        public TokenPassService(ISqlSugarClient dbContext, IRegionService regionService) : base(dbContext)
        {
            _regionService = regionService;
        }

        #region 字典
        /// <summary>
        /// 获取一组字典列表(单层)
        /// </summary>
        /// <param name="code">分组代号</param>
        /// <returns></returns>
        public async Task<List<CodeModel>> GetCodeList(string code)
        {
            return await _dbContext.Queryable<S99_Code>()
                .Where(S99 => S99.S99_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                              S99.S99_GroupCode == code &&
                              S99.S99_ParentCodeId == null) //一级
                .OrderBy(S99 => S99.S99_SeqNo)
                .OrderByDescending(S99 => S99.S99_CodeId)
                .Select(S99 => new CodeModel
                {
                    CodeId = S99.S99_CodeId,
                    CodeName = S99.S99_Name
                }).ToListAsync();
        }
        /// <summary>
        /// 获取多组字典列表(单层)
        /// </summary>
        /// <param name="codeList">分组代号</param>
        /// <returns></returns>
        public async Task<Dictionary<string, List<CodeModel>>> GetMultiCodeList(List<string> codeList)
        {
            Dictionary<string, List<CodeModel>> result = new();
            if (codeList?.Count > 0)
            {
                var list = await _dbContext.Queryable<S99_Code>()
                .Where(S99 => S99.S99_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                              codeList.Contains(S99.S99_GroupCode) &&
                              S99.S99_ParentCodeId == null)
                .Select(S99 => new
                {
                    CodeId = S99.S99_CodeId,
                    CodeName = S99.S99_Name,
                    GroupCode = S99.S99_GroupCode,
                    Priority = S99.S99_SeqNo
                }).ToListAsync();

                //对列表进行循环拆分
                if (list?.Count > 0)
                {
                    codeList.ForEach(item =>
                    {
                        result.Add(item, list.Where(c => c.GroupCode == item).OrderBy(c => c.Priority).ThenByDescending(c => c.CodeId)
                            .Select(c => new CodeModel
                            {
                                CodeId = c.CodeId,
                                CodeName = c.CodeName
                            }).ToList());
                    });
                }
            }
            return result;
        }
        /// <summary>
        /// 获取一组字典树(多层)
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<string> GetCodeTree(string code)
        {
            return SortedJsonTree.CreateJsonTrees(await _dbContext.Queryable<S99_Code>()
                .Where(S99 => S99.S99_IsDelete == (byte)BaseEnums.TrueOrFalse.False && S99.S99_GroupCode == code)
                .Select(S99 => new SortedJsonTree
                {
                    Id = S99.S99_CodeId,
                    Name = S99.S99_Name,
                    ParentId = S99.S99_ParentCodeId,
                    Priority = S99.S99_SeqNo ?? 0
                })
                .ToListAsync());
        }

        #endregion

        #region 区域
        /// <summary>
        /// 获取地区信息
        /// </summary>
        /// <returns></returns>
        public async Task<RegionStructureModel> GetRegions()
        {
            RegionStructureModel regionStruc = new();

            var regionList = await _regionService.GetRegion();//从redis取出所有区域信息
            if (regionList?.Count <= 0 || regionList == null)
            {
                return regionStruc;
            }

            //所有省份
            var provinceList = regionList?.Where(r => r.ParentId == 1)
                                         .Select(r => new { r.RegionCode, r.RegionName, r.RegionId }).ToList();
            //省份Id 用来查询城市
            var provinceIds = provinceList?.Select(c => c.RegionId).ToList();
            if (provinceIds?.Count <= 0 || provinceIds == null)
            {
                return regionStruc;
            }

            //所有城市
            var cityList = regionList?.Where(r => provinceIds.Contains((long)r.ParentId))
                                     .Select(r => new { r.RegionCode, r.RegionName, r.RegionId }).ToList();
            //城市Id 用来查询区县
            var cityIds = cityList?.Select(c => c.RegionId).ToList();
            if (cityIds?.Count <= 0 || cityIds == null)
            {
                return regionStruc;
            }

            //所有区县
            var areaList = regionList?.Where(r => cityIds.Contains((long)r.ParentId))
                                        .Select(r => new { r.RegionCode, r.RegionName }).ToList();

            //将所有列表加入字典
            foreach (var item in provinceList)
            {
                regionStruc.Province.Add(item.RegionCode, item.RegionName);
            }
            foreach (var item in cityList)
            {
                regionStruc.City.Add(item.RegionCode, item.RegionName);
            }
            foreach (var item in areaList)
            {
                regionStruc.Region.Add(item.RegionCode, item.RegionName);
            }

            return regionStruc;
        }
        #endregion
    }
}
