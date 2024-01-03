using System;

namespace FastAdminAPI.Common.SystemUtilities
{
    public static class EnvironmentHelper
    {
        /// <summary>
        /// 环境变量
        /// </summary>
        private static readonly string _env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        #region 内部使用
        /// <summary>
        /// 是否为开发环境
        /// </summary>
        /// <returns></returns>
        private static bool IsDev() => _env.ToLower().Contains("develop");
        /// <summary>
        /// 是否为生产环境
        /// </summary>
        /// <returns></returns>
        private static bool IsPro() => _env.ToLower().Contains("product");
        #endregion

        /// <summary>
        /// 是否为开发环境
        /// </summary>
        public static readonly bool IsDevelop = IsDev();
        /// <summary>
        /// 是否为生产环境
        /// </summary>
        public static readonly bool IsProduction = IsPro();

        /// <summary>
        /// 获取环境变量 ASPNETCORE_ENVIRONMENT
        /// </summary>
        /// <returns></returns>
        public static string GetEnv()
        {
            return _env;
        }
        /// <summary>
        /// 获取指定环境变量
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public static string GetEnvironmentVariable(string variable)
        {
            return Environment.GetEnvironmentVariable(variable);
        }
        /// <summary>
        /// 设置环境变量
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        public static void SetEnviromentVariable(string variable, string value)
        {
            Environment.SetEnvironmentVariable(variable, value);
        }
    }
}
