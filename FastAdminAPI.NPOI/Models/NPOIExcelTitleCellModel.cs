using NPOI.SS.UserModel;

namespace FastAdminAPI.NPOI.Models
{
    public class NPOIExcelTitleCellModel
    {
        /// <summary>
        /// 标题单元格名称
        /// </summary>
        public string TitleCellName { get; set; }
        /// <summary>
        /// 标题单元格样式
        /// </summary>
        public ICellStyle TitleCellStyle { get; set; }
    }
}
