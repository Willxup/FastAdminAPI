using FastAdminAPI.Business.Models.Region;
using FastAdminAPI.Core.Models.TokenPass;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.IServices
{
    /// <summary>
    /// 令牌过滤
    /// </summary>
    public interface ITokenPassService
    {

        #region 字典
        /// <summary>
        /// 获取一组字典列表
        /// </summary>
        /// <param name="code">分组代号</param>
        /// <returns></returns>
        Task<List<CodeModel>> GetCodeList(string code);
        /// <summary>
        /// 获取多组字典列表(单层)
        /// </summary>
        /// <param name="codeList">分组代号</param>
        /// <returns></returns>
        Task<Dictionary<string, List<CodeModel>>> GetMultiCodeList(List<string> codeList);
        /// <summary>
        /// 获取一组字典树(多层)
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<string> GetCodeTree(string code);
        #endregion

        #region 区域
        /// <summary>
        /// 获取地区信息
        /// </summary>
        /// <returns></returns>
        Task<RegionStructureModel> GetRegions();
        #endregion

    }
}
