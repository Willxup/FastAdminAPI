namespace FastAdminAPI.Core.Models.Users
{
    public class UserModel
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public long? UserId { get; set; }
        /// <summary>
        /// 企业微信UserId
        /// </summary>
        public string QyUserId { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 员工Id
        /// </summary>
        public long? EmployeeId { get; set; }
        /// <summary>
        /// 员工名称
        /// </summary>
        public string EmployeeName { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }
    }
}
