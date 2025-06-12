using System.Threading.Tasks;
using FastAdminAPI.Core.Controllers.BASE;
using FastAdminAPI.Core.IServices;
using Microsoft.AspNetCore.Mvc;

namespace FastAdminAPI.Core.Controllers
{
    /// <summary>
    /// 导出Excel
    /// </summary>
    public class ExportExcelController: BaseController
    {
        /// <summary>
        /// 导出Excel服务
        /// </summary>
        private readonly IExportExcelService _commonService;

        public ExportExcelController(IExportExcelService commonService)
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
