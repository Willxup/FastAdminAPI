using System;

namespace FastAdminAPI.Common.Authentications
{
    /// <summary>
    /// 令牌
    /// </summary>
    public class JwtTokenModel
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public long UserId { get; set; } = -1;
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 员工Id
        /// </summary>
        public long EmployeeId { get; set; } = -1;
        /// <summary>
        /// 员工名称
        /// </summary>
        public string EmployeeName { get; set; }
        /// <summary>
        /// 岗位Ids
        /// </summary>
        public string PostIds { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }
        /// <summary>
        /// 设备 0PC 1企业微信
        /// </summary>
        public int Device { get; set; } = -1;
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime Expires { get; set; }

    }
}
