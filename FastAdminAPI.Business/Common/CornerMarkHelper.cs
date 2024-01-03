using FastAdminAPI.Common.Attributes;
using FastAdminAPI.Common.Logs;
using SqlSugar;
using System;
using System.Threading.Tasks;

namespace FastAdminAPI.Business.Common
{
    public static class CornerMarkHelper
    {
        /// <summary>
        /// 角标补零为4位数字字符串
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string PaddingCornerMark(int num) => num.ToString("D4");
        /// <summary>
        /// 按旧角标获取新角标
        /// </summary>
        /// <param name="oldCornerMark"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string GetNewCornerMarkByOld(string oldCornerMark)
        {
            string prefix = "";
            string suffix = oldCornerMark;
            if (oldCornerMark.Length > 4)
            {
                prefix = oldCornerMark[..^4];
                suffix = oldCornerMark[^4..];
            }
            if (int.TryParse(suffix, out int number))
            {
                suffix = PaddingCornerMark(number + 1);
            }
            else
            {
                throw new Exception("无法转换角标!");
            }
            return prefix + suffix;
        }
        /// <summary>
        /// 获取角标
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName">表名</param>
        /// <param name="primaryFieldName">主键</param>
        /// <param name="cornerMarkFieldName">角标字段名</param>
        /// <param name="parentFieldName">父级字段名</param>
        /// <param name="parentFieldValue">父级字段值</param>
        /// <returns></returns>
        /// <exception cref="UserOperationException"></exception>
        public static async Task<string> GetCornerMark(SqlSugarScope db, string tableName, string primaryFieldName, string cornerMarkFieldName,
            string parentFieldName, string parentFieldValue)
        {
            try
            {
                //拼接sql
                string sql = $"SELECT MAX({cornerMarkFieldName}) FROM {tableName} WHERE {parentFieldName} ";
                if (!string.IsNullOrEmpty(parentFieldValue))
                {
                    sql += $"= {parentFieldValue}";
                }
                else
                {
                    sql += $" IS NULL";
                }

                //获取sql执行结果的获取首行首列
                string cornerMark = await db.Ado.GetStringAsync(sql);

                //如果角标不为空，说明已有角标，进行累加
                if (!string.IsNullOrEmpty(cornerMark))
                {
                    return GetNewCornerMarkByOld(cornerMark);
                }
                //角标为空，说明是第一个角标 (需要判断是否有父级)
                else
                {
                    //如果父级的值不为空，说明存在父级，找到父级，将父级的角标与当前的角标相加
                    if (!string.IsNullOrEmpty(parentFieldValue))
                    {
                        sql = $"SELECT {cornerMarkFieldName} FROM {tableName} WHERE {primaryFieldName} = {parentFieldValue}";
                        string parentCornerMark = await db.Ado.GetStringAsync(sql);
                        if (!string.IsNullOrEmpty(parentCornerMark))
                        {
                            return parentCornerMark + PaddingCornerMark(1);
                        }
                        else
                        {
                            throw new UserOperationException("获取父级角标失败!");
                        }
                    }
                    //如果父级的值为空，说明当前为1级，直接返回4位数字即可
                    else
                    {
                        return PaddingCornerMark(1);
                    }
                }

            }
            catch (Exception ex)
            {
                NLogHelper.Error($"获取【{tableName}】角标失败，{ex.Message}，" +
                    $"具体参数为：【primaryFieldName = {primaryFieldName}】【cornerMarkFieldName = {cornerMarkFieldName}】" +
                    $"【parentFieldName = {parentFieldName}】【parentFieldValue = {parentFieldValue}】", ex);
                throw new UserOperationException("获取角标失败，请重试!");
            }
        }
    }
}
