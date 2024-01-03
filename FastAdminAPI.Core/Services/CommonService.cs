﻿using FastAdminAPI.Common.Attributes;
using FastAdminAPI.Common.Enums;
using FastAdminAPI.Common.Logs;
using FastAdminAPI.Core.IServices;
using FastAdminAPI.Core.Services.BASE;
using FastAdminAPI.Framework.Entities;
using FastAdminAPI.NPOI.Extensions;
using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.Services
{
    public class CommonService : BaseService, ICommonService
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="httpContext"></param>
        public CommonService(ISqlSugarClient dbContext, IHttpContextAccessor httpContext) : base(dbContext, httpContext) { }

        #region 通用
        /// <summary>
        /// 导出EXCEL模板
        /// </summary>
        /// <returns></returns>
        /// <exception cref="UserOperationException"></exception>
        public async Task<byte[]> ExportExcelTemplate()
        {
            //创建Excel
            XSSFWorkbook workbook = new();
            XSSFSheet sheet = (XSSFSheet)workbook.CreateSheet($"导入模板");

            #region 设置标题单元格样式
            //设置标题字体大小
            var titleFontStyle = NPOIExcelHelper.GetFontStyle(workbook, "Arial", 10, true);
            //设置标题样式
            var titleStyle = NPOIExcelHelper.GetTitleCellStyle(workbook, titleFontStyle);
            var titleStyleWithColor = NPOIExcelHelper.GetTitleCellStyle(workbook, titleFontStyle, new XSSFColor(new byte[] { 255, 192, 0 }));
            #endregion

            #region 设置标题
            //标题单元格信息
            ICellStyle[] cellStyleList = new ICellStyle[2] { titleStyle, titleStyleWithColor }; //0不带颜色得标题 1带颜色的标题
            Dictionary<string, int> cellInfoDic = new()
            {
                { "测试名称1", 1 },{ "测试名称2", 0 },{ "测试名称3", 1 },{ "测试名称4", 1 },{ "测试名称5", 1 },{ "测试名称6", 0 }
            };
            //设置标题
            NPOIExcelHelper.SetTitleRow(sheet, NPOIExcelHelper.GetExcelTemplateTitleRow(cellInfoDic, cellStyleList), NPOIExcelHelper.GetDefaultStyle(workbook));
            #endregion

            #region 设置下拉数据源
            //设置下拉数据源(数据大小大于255长度)
            List<string> schoolList = await _dbContext.Queryable<S99_Code>()
                .Where(code => code.S99_IsValid == (byte)BaseEnums.IsValid.Valid)
                .Select(code => code.S99_Name)
                .ToListAsync();
            NPOIExcelHelper.SetMoreDropDownList(sheet, schoolList, 0, "测试下拉数据源");

            //设置下拉数据(数据大小小于255长度)
            List<string> enrollTypeList = new()
            { "测试1", "测试2", "测试3" };
            NPOIExcelHelper.SetLessDropDownList(sheet, enrollTypeList.ToArray(), 4);
            #endregion

            try
            {
                if (workbook != null)
                {
                    byte[] buffer = new byte[1024 * 2];
                    using (MemoryStream ms = new())
                    {
                        workbook.Write(ms);
                        buffer = ms.ToArray();
                        ms.Close();
                    }
                    return buffer;
                }
                else
                    throw new UserOperationException("没有可以导出的数据!");
            }
            catch (Exception ex)
            {
                NLogHelper.Error($"院校专业模板-导出数据失败!{ex.Message}", ex);
                throw new UserOperationException($"导出数据失败!");
            }
        }
        #endregion
    }
}
