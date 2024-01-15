using System.Threading.Tasks;

namespace FastAdminAPI.Core.IServices
{
    /// <summary>
    /// 导出Excel
    /// </summary>
    public interface IExportExcelService
    {
        #region 通用审批
        /// <summary>
        /// 导出EXCEL模板
        /// </summary>
        /// <returns></returns>
        Task<byte[]> ExportExcelTemplate();
        #endregion
    }
}
