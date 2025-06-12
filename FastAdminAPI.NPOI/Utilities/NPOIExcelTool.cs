using System.Collections.Generic;
using System.Reflection;
using FastAdminAPI.NPOI.Models;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace FastAdminAPI.NPOI.Utilities
{
    public static class NPOIExcelTool
    {
        /// <summary>
        /// 获取默认样式
        /// </summary>
        /// <param name="workbook"></param>
        /// <returns></returns>
        public static XSSFCellStyle GetDefaultStyle(IWorkbook workbook)
        {
            XSSFCellStyle style = (XSSFCellStyle)workbook.CreateCellStyle();

            // 设置单元格样式
            style.Alignment = HorizontalAlignment.Justify; // 两端自动对齐（自动换行）
            style.VerticalAlignment = VerticalAlignment.Center; // 垂直居中对齐

            // 设置单元格格式
            IDataFormat dataformat = workbook.CreateDataFormat(); 
            style.DataFormat = dataformat.GetFormat("@"); // dataformat.GetFormat("test") 两种方式都可以

            // 设置单元格字体
            style.SetFont(GetFontStyle(workbook, "Arial", 10, true));

            return style;
        }
        /// <summary>
        /// 获取字体样式
        /// </summary>
        /// <param name="workbook">工作簿</param>
        /// <param name="fontName">字体名称</param>
        /// <param name="point">字体大小</param>
        /// <param name="isBold">是否加粗</param>
        /// <returns></returns>
        public static IFont GetFontStyle(IWorkbook workbook, string fontName, short point, bool isBold = false)
        {
            XSSFFont font = (XSSFFont)workbook.CreateFont();
            font.FontName = fontName; // 字体名称
            font.FontHeightInPoints = point; // 字体大小
            if (isBold)
                font.IsBold = true; // 是否加粗
            return font;
        }
        /// <summary>
        /// 获取标题单元格样式
        /// </summary>
        /// <param name="workbook">工作簿</param>
        /// <param name="fontStyle">字体样式</param>
        /// <param name="color">颜色</param>
        /// <returns></returns>
        public static XSSFCellStyle GetTitleCellStyle(IWorkbook workbook, IFont fontStyle, XSSFColor color = null)
        {
            XSSFCellStyle titleStyle = (XSSFCellStyle)workbook.CreateCellStyle();
            
            // 设置单元格样式
            titleStyle.Alignment = HorizontalAlignment.Center; // 两端居中对齐
            titleStyle.VerticalAlignment = VerticalAlignment.Center; // 垂直居中对齐

            // 设置单元格格式
            IDataFormat dataformat = workbook.CreateDataFormat();
            titleStyle.DataFormat = dataformat.GetFormat("@"); // dataformat.GetFormat("text") 两种方式都可以

            // 设置单元格字体
            titleStyle.SetFont(fontStyle);

            // 设置填充颜色
            if (color != null)
            {
                titleStyle.FillForegroundColorColor = color;
                titleStyle.FillPattern = FillPattern.SolidForeground;
            }

            return titleStyle;
        }
        /// <summary>
        /// 获取普通单元格样式
        /// </summary>
        /// <param name="workbook">工作簿</param>
        /// <param name="font">字体</param>
        /// <returns></returns>
        public static XSSFCellStyle GetNormalCellStyle(IWorkbook workbook, IFont font)
        {
            XSSFCellStyle cellStyle = (XSSFCellStyle)workbook.CreateCellStyle();

            // 设置单元格样式
            cellStyle.Alignment = HorizontalAlignment.Justify; // 两端自动对齐（自动换行）
            cellStyle.VerticalAlignment = VerticalAlignment.Center; // 垂直居中对齐

            // 设置单元格格式
            IDataFormat dataformat = workbook.CreateDataFormat();
            cellStyle.DataFormat = dataformat.GetFormat("@"); // dataformat.GetFormat("text") 两种方式都可以

            // 设置单元格字体
            cellStyle.SetFont(font);

            return cellStyle;
        }


        /// <summary>
        /// 获取模板标题行
        /// </summary>
        /// <param name="cellInfoDic">单元格内容字典(单元格内容：样式索引)</param>
        /// <param name="cellStyleList">样式列表</param>
        /// <returns></returns>
        public static List<NPOIExcelTitleCellModel> GetExcelTemplateTitleRow(Dictionary<string, int> cellInfoDic, ICellStyle[] cellStyleList)
        {
            // 标题行列表
            List<NPOIExcelTitleCellModel> titleCellList = null;

            if (cellInfoDic?.Count > 0 && cellStyleList?.Length > 0)
            {
                titleCellList = new List<NPOIExcelTitleCellModel>();

                foreach (var cellInfo in cellInfoDic)
                {
                    NPOIExcelTitleCellModel cell = new()
                    {
                        TitleCellName = cellInfo.Key, // key为单元格内容
                        TitleCellStyle = cellStyleList[cellInfo.Value] // value为单元格样式
                    };

                    titleCellList.Add(cell);
                }
            }
            return titleCellList;
        }
        /// <summary>
        /// 设置标题行
        /// </summary>
        /// <param name="sheet">表</param>
        /// <param name="cellInfoList">标题单元格</param>
        /// <param name="columnStyle">列样式</param>
        /// <param name="heightInPoint">单元格高度(默认27)</param>
        /// <param name="columnWidth">单元格宽度(默认30 * 256[15个字符])</param>
        public static void SetTitleRow(ISheet sheet, List<NPOIExcelTitleCellModel> cellInfoList, ICellStyle columnStyle, float heightInPoint = 27, int columnWidth = 15)
        {
            // 创建行
            IRow title = sheet.CreateRow(0);
            // 设置行高
            title.HeightInPoints = heightInPoint;

            // 循环设置标题
            if (cellInfoList?.Count > 0)
            {
                for (int i = 0; i < cellInfoList.Count; i++)
                {
                    // 获取单元格设置
                    var cellInfo = cellInfoList[i];

                    // 设置列默认样式
                    sheet.SetDefaultColumnStyle(i, columnStyle);

                    // 创建单元格并赋值
                    title.CreateCell(i).SetCellValue(cellInfo.TitleCellName);

                    // 设置单元格样式
                    title.GetCell(i).CellStyle = cellInfo.TitleCellStyle;

                    // 包含项目，加大单元格宽度
                    if (cellInfo.TitleCellName.Contains("项目"))
                    {
                        sheet.SetColumnWidth(i, (columnWidth + 7) * 256);
                    }
                    // 长度大于10字符，按照每超过3字符加5字符单元格宽度 不足3字符当作3字符计算
                    else if (cellInfo.TitleCellName.Length > 10)
                    {
                        sheet.SetColumnWidth(i, (columnWidth + ((cellInfo.TitleCellName.Length - 10) / 3 + 1) * 5) * 256);
                    }
                    // 其他
                    else
                    {
                        sheet.SetColumnWidth(i, columnWidth * 256);
                    }
                }
            }
        }


        /// <summary>
        /// 设置内容行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sheet">表</param>
        /// <param name="contentList">单元格内容</param>
        /// <param name="cellStyle">单元格样式</param>
        public static void SetContentRows<T>(ISheet sheet, List<T> contentList, ICellStyle cellStyle) where T : class, new()
        {
            if (contentList?.Count > 0)
            {
                // 获取泛型的所有属性
                PropertyInfo[] propertyInfos = typeof(T).GetProperties();

                // 行号，从1开始（0为标题）
                int rowNum = 1;
                foreach (var item in contentList)
                {
                    // 创建行
                    IRow row = sheet.CreateRow(rowNum);
                    // 单元格序号
                    int cellNum = 0;

                    // 循环反射赋值
                    foreach (PropertyInfo p in propertyInfos)
                    {
                        // 创建单元格，设置单元格内容
                        row.CreateCell(cellNum).SetCellValue(p.GetValue(item).ToString());

                        // 设置单元格样式
                        row.GetCell(cellNum).CellStyle = cellStyle;

                        cellNum++;
                    }

                    rowNum++;
                }
            }
        }


        /// <summary>
        /// 设置下拉列表(少于255)
        /// </summary>
        /// <param name="sheet">当前表</param>
        /// <param name="dropDownList">下拉列表</param>
        /// <param name="column">需要下拉列表的单元格所在列/param>
        /// <param name="startRow">需要下拉列表的单元格起始行(默认1)</param>
        /// <param name="lastRow">需要下拉列表的单元格结束行(默认65535)</param>
        public static void SetLessDropDownList(XSSFSheet sheet, string[] dropDownList, int column, int startRow = 1, int lastRow = 65535)
        {
            // 数据校验帮助类
            XSSFDataValidationHelper dvHelper = new(sheet);

            // 创建需要校验的数据
            XSSFDataValidationConstraint dvConstraint = (XSSFDataValidationConstraint)dvHelper.CreateExplicitListConstraint(dropDownList);

            // 被数据校验的单元格范围
            CellRangeAddressList addressList = new(startRow, lastRow, column, column);

            // 创建数据校验
            XSSFDataValidation validation = (XSSFDataValidation)dvHelper.CreateValidation(dvConstraint, addressList);

            // 添加数据校验
            sheet.AddValidationData(validation);
        }
        /// <summary>
        /// 设置下拉列表(多于255)
        /// </summary>
        /// <param name="sheet">当前表</param>
        /// <param name="dropDownList">下拉列表</param>
        /// <param name="column">需要下拉列表的单元格所在列/param>
        /// <param name="sheetName">表名(默认HiddenSheet)</param>
        /// <param name="startRow">需要下拉列表的单元格起始行(默认1)</param>
        /// <param name="lastRow">需要下拉列表的单元格结束行(默认65535)</param>
        public static void SetMoreDropDownList(XSSFSheet sheet, List<string> dropDownList, int column, string sheetName = "HiddenSheet", int startRow = 1, int lastRow = 65535)
        {
            if (sheet == null) return;

            IWorkbook workbook = sheet.Workbook;

            // 创建 数据校验sheet
            ISheet hiddenSheet = workbook.CreateSheet(sheetName); ;

            // 循环将数据填入 数据校验sheet 中
            for (int i = 0, length = dropDownList.Count; i < length; i++)
            {
                string value = dropDownList[i];
                IRow row = hiddenSheet.CreateRow(i);
                ICell cell = row.CreateCell(0);
                cell.SetCellValue(value);
            }

            // 创建单元格范围
            IName namedCell = workbook.CreateName();
            namedCell.NameName = sheetName;
            namedCell.RefersToFormula = sheetName + "!$A$1:$A$" + dropDownList.Count;

            // 数据校验帮助类
            XSSFDataValidationHelper dvHelper = new(sheet);

            // 创建需要校验的数据
            XSSFDataValidationConstraint dvConstraint = (XSSFDataValidationConstraint)dvHelper.CreateFormulaListConstraint(sheetName);

            // 被数据校验的单元格范围
            CellRangeAddressList addressList = new(startRow, lastRow, column, column);

            // 创建数据校验
            XSSFDataValidation validation = (XSSFDataValidation)dvHelper.CreateValidation(dvConstraint, addressList);

            // 获取 数据校验sheet 的索引
            int hiddenSheetIndex = workbook.GetSheetIndex(hiddenSheet);

            // 设置该 数据校验sheet 为隐藏sheet
            workbook.SetSheetHidden(hiddenSheetIndex, SheetState.Hidden);

            // 添加数据校验
            sheet.AddValidationData(validation);
        }
    }
}
