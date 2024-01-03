using System.Threading.Tasks;

namespace FastAdminAPI.Core.IServices
{
    public interface ICommonService
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
