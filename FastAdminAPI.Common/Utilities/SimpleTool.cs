namespace FastAdminAPI.Common.Utilities
{
    public static class SimpleTool
    {
        /// <summary>
        /// 手机号过滤
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static string PhoneFormatFilter(string phone)
        {
            return phone.Replace("\n", "")
                        .Replace(" ", "")
                        .Replace("\t", "")
                        .Replace("\r", "");
        }
    }
}
