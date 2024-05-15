using FastAdminAPI.Common.Attributes;
using FastAdminAPI.Common.Authentications;
using FastAdminAPI.Common.BASE;
using FastAdminAPI.Common.Logs;
using FastAdminAPI.Common.Redis;
using FastAdminAPI.Common.Utilities;
using FastAdminAPI.Core.Controllers.BASE;
using FastAdminAPI.Core.IServices;
using FastAdminAPI.Core.Models.Login;
using FastAdminAPI.Network.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.Controllers
{
    /// <summary>
    /// 登录
    /// </summary>
    public class LoginController : BaseController
    {
        /// <summary>
        /// Redis
        /// </summary>
        private readonly IRedisHelper _redis;
        /// <summary>
        /// 配置
        /// </summary>
        private readonly IConfiguration _configuration;
        /// <summary>
        /// 登录Service
        /// </summary>
        private readonly ILoginService _loginService;
        /// <summary>
        /// 企业微信API
        /// </summary>
        private readonly IQyWechatApi _qyWechatApi;


        /// <summary>
        /// 登录许可Key
        /// </summary>
        private readonly string LOGIN_PERMIT_KEY;
        /// <summary>
        /// 登录许可有效期
        /// </summary>
        private readonly int LOGIN_PERMIT_EXPIRES;
        /// <summary>
        /// 登录许可IP白名单
        /// </summary>
        private readonly List<string> LOGIN_PERMIT_IP_WHITE_LIST;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="redis"></param>
        /// <param name="configuration"></param>
        /// <param name="loginService"></param>
        /// <param name="qyWechatApi"></param>
        public LoginController(IRedisHelper redis, IConfiguration configuration, ILoginService loginService, IQyWechatApi qyWechatApi)
        {
            _redis = redis;
            _configuration = configuration;
            _loginService = loginService;
            _qyWechatApi = qyWechatApi;
            LOGIN_PERMIT_KEY = configuration.GetValue<string>("Redis.LoginPermit.Key");
            LOGIN_PERMIT_EXPIRES = configuration.GetValue<int>("Redis.LoginPermit.Expires");
            LOGIN_PERMIT_IP_WHITE_LIST = configuration.GetValue<string>("Login.IPAddress.WhiteList")?.Split(",")?.ToList();
        }

        /// <summary>
        /// 是否为被许可的IP地址
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <returns>true/false</returns>
        [HttpGet]
        public async Task<ResponseModel> IsPermittedIPAddress([FromQuery][Required(ErrorMessage = "IP地址不能为空!")] string ip)
        {
            bool isPermit = false;
            //验证密码登录时域名限制
            if (EnvTool.IsProduction)
            {
                //如果IP不为空并且在白名单中
                if (!string.IsNullOrEmpty(ip) && (LOGIN_PERMIT_IP_WHITE_LIST?.Contains(ip) ?? false))
                {
                    isPermit = true;
                }
            }
            else
            {
                isPermit = true;
            }
            return await Task.FromResult(Success(isPermit));
        }

        /// <summary>
        /// 密码登录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="UserOperationException"></exception>
        [HttpPost]
        public async Task<ResponseModel> LoginByPassword([FromBody] LoginByPasswordModel model)
        {
            try
            {
                var user = await _loginService.GetUser(model.Account, model.Password);

                JwtTokenModel jwt = new()
                {
                    UserId = (long)user.UserId,
                    Account = user.Account,
                    EmployeeId = (long)user.EmployeeId,
                    EmployeeName = user.EmployeeName,
                    Avatar = user.Avatar,
                    Device = model.Device
                };

                //生成token
                string token = JwtHelper.IssueJwt(jwt, _configuration);

                //登录许可RedisKey
                string key = LOGIN_PERMIT_KEY + user.UserId + ":" + model.Device;

                //保存在Redis中
                bool isSuccess = await _redis.StringSetAsync(key, token, TimeSpan.FromSeconds(LOGIN_PERMIT_EXPIRES));
                if (!isSuccess)
                    throw new Exception("登录失败，redis缓存失败!");

                //登录记录
                await _loginService.RecordLogin(jwt.UserId, jwt.EmployeeId, jwt.Device);

                return Success(token);
            }
            catch (UserOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                NLogHelper.Error($"登录失败，{ex.Message}", ex);
                throw new UserOperationException("登录失败，请稍后再试!");
            }
        }
        /// <summary>
        /// 企业微信登录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="UserOperationException"></exception>
        [HttpPost]
        [ProducesResponseType(typeof(LoginByQyWechatResponseModel), 200)]
        public async Task<ResponseModel> LoginByQyWechat([FromBody] LoginByQyWechatModel model)
        {
            try
            {
                var res = await _qyWechatApi.GetUserId(model.Code);
                if (res?.Code == ResponseCode.Success)
                {
                    //企业微信UserId
                    string qyUserId = res.Data.ToString();

                    if (!string.IsNullOrEmpty(qyUserId))
                    {
                        var user = await _loginService.GetUser(qyUserId);

                        JwtTokenModel jwt = new()
                        {
                            UserId = (long)user.UserId,
                            Account = user.Account,
                            EmployeeId = (long)user.EmployeeId,
                            EmployeeName = user.EmployeeName,
                            Avatar = user.Avatar,
                            Device = model.Device
                        };

                        //生成token
                        string token = JwtHelper.IssueJwt(jwt, _configuration);

                        //登录许可RedisKey
                        string key = LOGIN_PERMIT_KEY + user.UserId + ":" + model.Device;

                        //保存在Redis中
                        bool isSuccess = await _redis.StringSetAsync(key, token, TimeSpan.FromSeconds(LOGIN_PERMIT_EXPIRES));
                        if (!isSuccess)
                            throw new Exception("企业微信登录失败，redis缓存失败!");

                        //登录记录
                        await _loginService.RecordLogin(jwt.UserId, jwt.EmployeeId, jwt.Device);

                        //返回体
                        LoginByQyWechatResponseModel result = new()
                        {
                            Token = token,
                            QyUserId = user.QyUserId
                        };

                        //返回token
                        return Success(result);

                    }
                    throw new Exception($"企业微信登录失败，企业微信userid为空!");
                }
                throw new Exception($"企业微信登录失败，{res.Message}");
            }
            catch (UserOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                NLogHelper.Error($"企业微信登录失败，{ex.Message}", ex);
                throw new UserOperationException("登录失败，请稍后再试!");
            }
        }
    }
}
