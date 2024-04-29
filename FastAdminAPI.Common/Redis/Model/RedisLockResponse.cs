namespace FastAdminAPI.Common.Redis.Model
{
    public class RedisLockResponse<T>
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; } = false;
        /// <summary>
        /// 返回数据
        /// </summary>
        public T Data { get; set; } = default;
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
