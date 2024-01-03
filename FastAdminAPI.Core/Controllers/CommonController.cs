using FastAdminAPI.Core.Controllers.BASE;
using FastAdminAPI.Core.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.Controllers
{
    /// <summary>
    /// 公共
    /// </summary>
    public class CommonController: BaseController
    {
        /// <summary>
        /// 公共服务
        /// </summary>
        private readonly ICommonService _commonService;

        public CommonController(ICommonService commonService)
        {
            _commonService = commonService;
        }

        #region 通用
        /// <summary>
        /// 导出Excel模板
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IActionResult), 200)]
        public async Task<IActionResult> ExportExcelTemplate()
        {
            return File(await _commonService.ExportExcelTemplate(), "application/ms-excel", $"导入模板.xlsx");
        }
        #endregion
    }
}
