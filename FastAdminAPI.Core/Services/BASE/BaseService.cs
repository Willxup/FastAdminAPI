using Microsoft.AspNetCore.Http;
using SqlSugar;
using System;
using System.Linq;

namespace FastAdminAPI.Core.Services.BASE
{
    /// <summary>
    /// 服务层公用部分
    /// </summary>
    public class BaseService
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        protected readonly long _userId;
        /// <summary>
        /// 账号
        /// </summary>
        protected readonly string _account;
        /// <summary>
        /// 员工Id
        /// </summary>
        protected readonly long _employeeId;
        /// <summary>
        /// 员工名称
        /// </summary>
        protected readonly string _employeeName;
        /// <summary>
        /// 头像
        /// </summary>
        protected readonly string _avatar;
        /// <summary>
        /// SugarScope
        /// </summary>
        protected SqlSugarScope _dbContext;

        public BaseService() { }

        public BaseService(ISqlSugarClient dbContext)
        {
            _dbContext = dbContext as SqlSugarScope;
        }

        public BaseService(ISqlSugarClient dbContext, IHttpContextAccessor httpContext)
        {
            _dbContext = dbContext as SqlSugarScope;

            _userId = Convert.ToInt64(httpContext.HttpContext.User.Claims.First(c => c.Type == "UserId").Value);
            _account = httpContext.HttpContext.User.Claims.First(c => c.Type == "Account").Value;
            _employeeId = Convert.ToInt64(httpContext.HttpContext.User.Claims.First(c => c.Type == "EmployeeId").Value);
            _employeeName = httpContext.HttpContext.User.Claims.First(c => c.Type == "EmployeeName").Value;
            _avatar = httpContext.HttpContext.User.Claims.First(c => c.Type == "Avatar").Value;
        }
    }
}
