using FastAdminAPI.Common.Attributes;
using FastAdminAPI.Common.BASE;
using FastAdminAPI.Common.Enums;
using FastAdminAPI.Common.Logs;
using FastAdminAPI.Core.IServices;
using FastAdminAPI.Core.Models.Users;
using FastAdminAPI.Core.Services.BASE;
using FastAdminAPI.Framework.Entities;
using FastAdminAPI.Framework.Extensions;
using SqlSugar;
using System;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.Services
{
    /// <summary>
    /// 登录
    /// </summary>
    public class LoginService : BaseService, ILoginService
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="dbContext"></param>
        public LoginService(ISqlSugarClient dbContext) : base(dbContext) { }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="qyUserId">企业微信UserId</param>
        /// <returns></returns>
        /// <exception cref="UserOperationException"></exception>
        public async Task<UserModel> GetUser(string qyUserId)
        {
            //获取员工信息
            var employee = await _dbContext.Queryable<S07_Employee>()
                .Where(S07 => S07.S07_IsDelete == (byte)BaseEnums.TrueOrFalse.False && S07.S07_QyUserId == qyUserId)
                .Select(S07 => new UserModel
                {
                    UserId = S07.S01_UserId,
                    QyUserId = S07.S07_QyUserId,
                    EmployeeId = S07.S07_EmployeeId,
                    EmployeeName = S07.S07_Name,
                    Avatar = S07.S07_Avatar
                }).FirstAsync() ?? throw new UserOperationException("获取用户信息失败!");

            //获取用户信息
            var user = await _dbContext.Queryable<S01_User>()
                .Where(S01 => S01.S01_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                              S01.S01_AccountStatus == (byte)BusinessEnums.AccountStatus.Enable &&
                              S01.S01_UserId == employee.UserId)
                .Select(S01 => new
                {
                    UserId = S01.S01_UserId,
                    Account = S01.S01_Account
                })
                .FirstAsync() ?? throw new UserOperationException("用户不存在或已禁用!");

            employee.UserId = user.UserId;
            employee.Account = user.Account;
            return employee;
        }
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        /// <exception cref="UserOperationException"></exception>
        public async Task<UserModel> GetUser(string account, string password)
        {
            //密码进行解密
            //password = UserPasswordCryptionHelper.DecryptPassword(password);

            //校验用户及获取用户信息
            var user = await _dbContext.Queryable<S01_User>()
                .Where(S01 => S01.S01_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                              S01.S01_AccountStatus == (byte)BusinessEnums.AccountStatus.Enable &&
                              S01.S01_Account == account &&
                              S01.S01_Password == password)
                .Select(S01 => new
                {
                    UserId = S01.S01_UserId,
                    Account = S01.S01_Account
                })
                .FirstAsync() ?? throw new UserOperationException("账号或密码错误!");

            //获取员工信息
            var employee = await _dbContext.Queryable<S07_Employee>()
                .Where(S07 => S07.S07_IsDelete == (byte)BaseEnums.TrueOrFalse.False && S07.S01_UserId == user.UserId)
                .Select(S07 => new UserModel
                {
                    QyUserId = S07.S07_QyUserId,
                    EmployeeId = S07.S07_EmployeeId,
                    EmployeeName = S07.S07_Name,
                    Avatar = S07.S07_Avatar
                }).FirstAsync() ?? throw new UserOperationException("获取用户信息失败!");

            employee.UserId = user.UserId;
            employee.Account = user.Account;
            return employee;
        }
        /// <summary>
        /// 记录用户登录
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="employeeId">员工Id</param>
        /// <param name="device">登录设备</param>
        /// <returns></returns>
        public async Task RecordLogin(long userId, long employeeId, int device)
        {
            try
            {
                S14_LoginRecords login = new()
                {
                    S01_UserId = userId,
                    S07_EmployeeId = employeeId,
                    S14_Device = device,
                    S14_Time = _dbContext.GetDate()
                };

                var result = await _dbContext.Insertable(login).ExecuteAsync();

                if (result.Code != ResponseCode.Success)
                {
                    NLogHelper.Error($"记录用户登录失败!{result.Message}");
                }
            }
            catch (Exception ex)
            {
                NLogHelper.Error($"记录用户登录出错!{ex.Message}", ex);
            }
        }
    }
}
