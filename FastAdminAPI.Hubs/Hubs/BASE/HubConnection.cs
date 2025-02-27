namespace FastAdminAPI.Hubs.Hubs.BASE
{
    public class HubConnection
    {
        /// <summary>
        /// 客户端连接Id
        /// </summary>
        public string ConnectionId { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 员工Id
        /// </summary>
        public long EmployeeId { get; set; }
        /// <summary>
        /// 员工名称
        /// </summary>
        public string EmployeeName { get; set; }
    }
}
