using FastAdminAPI.Business.Models.Region;
using FastAdminAPI.Common.BASE;
using FastAdminAPI.Core.Controllers.BASE;
using FastAdminAPI.Core.IServices;
using FastAdminAPI.Core.Models.TokenPass;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FastAdminAPI.Common.Tree;

namespace FastAdminAPI.Core.Controllers
{
    /// <summary>
    /// 令牌过滤
    /// </summary>
    public class TokenPassController : BaseController
    {
        /// <summary>
        /// 令牌过滤Service
        /// </summary>
        private readonly ITokenPassService _tokenPassService;
        
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="tokenPassService"></param>
        public TokenPassController(ITokenPassService tokenPassService) 
        {
            _tokenPassService = tokenPassService;
        }

        #region 字典
        /// <summary>
        /// 获取一组字典列表(单层)
        /// </summary>
        /// <param name="code">分组代号</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<CodeModel>), 200)]
        public async Task<ResponseModel> GetCodeList([FromQuery][Required(ErrorMessage = "分组代号不能为空!")] string code)
        {
            return Success(await _tokenPassService.GetCodeList(code));
        }
        /// <summary>
        /// 获取多组字典列表(单层)
        /// </summary>
        /// <param name="codeList">分组代号</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(Dictionary<string, List<CodeModel>>), 200)]
        public async Task<ResponseModel> GetMultiCodeList([FromQuery] List<string> codeList)
        {
            return Success(await _tokenPassService.GetMultiCodeList(codeList));
        }
        /// <summary>
        /// 获取一组字典树(多层)
        /// </summary>
        /// <param name="code">分组代号</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<SortedBaseTree>), 200)]
        public async Task<ResponseModel> GetCodeTree([FromQuery][Required(ErrorMessage = "分组代号不能为空!")] string code)
        {
            return Success(await _tokenPassService.GetCodeTree(code));
        }
        #endregion

        #region 区域
        /// <summary>
        /// 获取地区信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(RegionStructureModel), 200)]
        public async Task<ResponseModel> GetRegions()
        {
            return Success(await _tokenPassService.GetRegions());
        }
        #endregion

    }
}
