using System.Collections.Generic;

namespace FastAdminAPI.Common.Utilities
{
    /// <summary>
    /// 扩展工具
    /// </summary>
    public static class ExtensionTool
    {
        #region List<T>
        /// <summary>
        /// 列表是否为空或null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this List<T> list)
        {
            bool isNullOrEmpty = false;

            // linq方式
            //if (!list?.Any() ?? true)
            //    isNullOrEmpty = true;

            if (list == null || list?.Count <= 0)
                isNullOrEmpty = true;

            return isNullOrEmpty;
        } 
        #endregion
    }
}
