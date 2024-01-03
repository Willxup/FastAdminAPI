namespace FastAdminAPI.CAP.Subscribes
{
    public class SystemSubscriber
    {
        #region 内部使用
        /// <summary>
        /// 前缀
        /// </summary>
        private const string PREFIX = "FastAdminAPI_EventBus:System."; 
        #endregion

        /// <summary>
        /// 通知消息
        /// </summary>
        public const string NOTIFY_MESSAGE = $"{PREFIX}Notify.Message";
    }
}
