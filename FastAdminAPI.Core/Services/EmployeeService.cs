using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastAdminAPI.Business.Interfaces;
using FastAdminAPI.Common.Attributes;
using FastAdminAPI.Common.BASE;
using FastAdminAPI.Common.Enums;
using FastAdminAPI.Core.IServices;
using FastAdminAPI.Core.Models.Employee;
using FastAdminAPI.Core.Services.BASE;
using FastAdminAPI.Framework.Entities;
using FastAdminAPI.Framework.Extensions;
using Microsoft.AspNetCore.Http;
using SqlSugar;
using SqlSugar.Attributes.Extension.Extensions;

namespace FastAdminAPI.Core.Services
{
    /// <summary>
    /// 员工信息
    /// </summary>
    public class EmployeeService : BaseService, IEmployeeService
    {
        /// <summary>
        /// 数据权限Service
        /// </summary>
        private readonly IDataPermissionService _dataPermission;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="httpContext"></param>
        /// <param name="dataPermission"></param>
        public EmployeeService(ISqlSugarClient dbContext, IHttpContextAccessor httpContext,
            IDataPermissionService dataPermission) : base(dbContext, httpContext)
        {
            _dataPermission = dataPermission;
        }

        #region 员工

        #region 内部方法
        /// <summary>
        /// 新增用户功能权限
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userId">用户Id</param>
        /// <param name="isDelPermission">是否删除角色权限</param>
        /// <returns></returns>
        private async Task<ResponseModel> AddUserPermission(EmployeeBaseInfoModel model, long userId, bool isDelPermission = false)
        {
            ResponseModel result = ResponseModel.Success();

            //创建用户角色
            if (model.RoleIds?.Count <= 0)
                throw new UserOperationException("角色不能为空!");

            if (isDelPermission)
            {
                //删除该用户的角色与权限
                result = await _dbContext.Deleteable<S09_UserPermission>().Where(S09 => S09.S01_UserId == userId).ExecuteAsync(false);
            }

            //创建List储存功能权限
            List<S09_UserPermission> permissionList = new();

            //数据库时间
            DateTime dbTime = _dbContext.GetDate();

            //循环获取角色
            model.RoleIds.ForEach(item =>
            {
                S09_UserPermission permission = new()
                {
                    S01_UserId = userId,
                    S09_PermissionType = (byte)BusinessEnums.PermissionType.Role,
                    S09_CommonId = item,

                    S09_CreateId = _employeeId,
                    S09_CreateBy = _employeeName,
                    S09_CreateTime = dbTime
                };

                //添加到List
                permissionList.Add(permission);
            });

            //创建用户模块
            if (model.ModuleIds?.Count > 0)
            {
                //去重
                model.ModuleIds = model.ModuleIds.Distinct().ToList();
                //循环获取模块
                model.ModuleIds.ForEach(item =>
                {
                    S09_UserPermission permission = new()
                    {
                        S01_UserId = userId,
                        S09_PermissionType = (byte)BusinessEnums.PermissionType.User,
                        S09_CommonId = item,

                        S09_CreateId = _employeeId,
                        S09_CreateBy = _employeeName,
                        S09_CreateTime = dbTime
                    };
                    permissionList.Add(permission);
                });
            }

            //判断list是否存在功能权限
            if (result?.Code == ResponseCode.Success && permissionList?.Count > 0)
            {
                result = await _dbContext.Insertable(permissionList).ExecuteAsync();
            }

            return result;
        }
        #endregion

        /// <summary>
        /// 按部门Ids获取员工简要列表(不含子部门)
        /// </summary>
        /// <param name="departIds">部门Ids</param>
        /// <param name="isMainPost">是否获取员工主岗位</param>
        /// <returns></returns>
        public async Task<List<EmployeeSimpleModel>> GetEmployeeListByDepartIds(List<long> departIds, bool isMainPost)
        {
            return await _dbContext.Queryable<S07_Employee>()
                .Where(S07 => S07.S07_IsDelete == (byte)BaseEnums.TrueOrFalse.False)
                .WhereIF(departIds?.Count > 0, S07 => SqlFunc.Subqueryable<S08_EmployeePost>()
                                                             .Where(S08 => S08.S08_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                                                                           S08.S07_EmployeeId == S07.S07_EmployeeId &&
                                                                           departIds.Contains(S08.S05_DepartId))
                                                             .WhereIF(isMainPost, S08 => S08.S08_IsMainPost == (byte)BaseEnums.TrueOrFalse.True)
                                                             .Any())
                .OrderBy(S07 => S07.S07_Name)
                .Select(S07 => new EmployeeSimpleModel
                {
                    EmployeeId = S07.S07_EmployeeId,
                    EmployeeName = S07.S07_Name,
                    Phone = S07.S07_Phone
                }).ToListAsync();
        }
        /// <summary>
        /// 按岗位Ids获取员工简要列表(不含子岗位)
        /// </summary>
        /// <param name="postIds">岗位Ids</param>
        /// <param name="isMainPost">是否获取员工主岗位</param>
        /// <returns></returns>
        public async Task<List<EmployeeSimpleModel>> GetEmployeeListByPostIds(List<long> postIds, bool isMainPost)
        {
            return await _dbContext.Queryable<S07_Employee>()
                .Where(S07 => S07.S07_IsDelete == (byte)BaseEnums.TrueOrFalse.False)
                .WhereIF(postIds?.Count > 0, S07 => SqlFunc.Subqueryable<S08_EmployeePost>()
                                                             .Where(S08 => S08.S08_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                                                                           S08.S07_EmployeeId == S07.S07_EmployeeId &&
                                                                           postIds.Contains(S08.S06_PostId))
                                                             .WhereIF(isMainPost, S08 => S08.S08_IsMainPost == (byte)BaseEnums.TrueOrFalse.True)
                                                             .Any())
                .OrderBy(S07 => S07.S07_Name)
                .Select(S07 => new EmployeeSimpleModel
                {
                    EmployeeId = S07.S07_EmployeeId,
                    EmployeeName = S07.S07_Name,
                    Phone = S07.S07_Phone
                }).ToListAsync();
        }
        /// <summary>
        /// 获取下属员工简要列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<EmployeeSimpleModel>> GetSubordinateEmployeeList()
        {
            //数据权限
            var dataPermission = await _dataPermission.Get();

            return await _dbContext.Queryable<S07_Employee>()
                .Where(S07 => S07.S07_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                              dataPermission.Contains(S07.S07_EmployeeId))
                .OrderBy(S07 => S07.S07_Name)
                .Select(S07 => new EmployeeSimpleModel
                {
                    EmployeeId = S07.S07_EmployeeId,
                    EmployeeName = S07.S07_Name,
                    Phone = S07.S07_Phone
                }).ToListAsync();
        }
        /// <summary>
        /// 获取全部员工简要列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<EmployeeSimpleModel>> GetAllEmployeeList()
        {
            return await _dbContext.Queryable<S07_Employee>()
                .Where(S07 => S07.S07_IsDelete == (byte)BaseEnums.TrueOrFalse.False)
                .OrderBy(S07 => S07.S07_Name)
                .Select(S07 => new EmployeeSimpleModel
                {
                    EmployeeId = S07.S07_EmployeeId,
                    EmployeeName = S07.S07_Name,
                    Phone = S07.S07_Phone
                }).ToListAsync();
        }
        /// <summary>
        /// 通过部门Id获取员工(主岗位)列表(包含子部门)
        /// </summary>
        /// <param name="departId">部门Id</param>
        /// <returns></returns>
        public async Task<ResponseModel> GetEmployeeListByDepartId(long departId)
        {
            return await GetEmployeeList(new EmployeePageSearch()
            {
                CornerMark = await _dbContext.Queryable<S05_Department>()
                             .Where(S05 => S05.S05_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                                  S05.S05_DepartId == departId)
                             .Select(S05 => S05.S05_CornerMark)
                             .FirstAsync(),
                Status = { (byte)BusinessEnums.EmployeeStatus.Practice, (byte)BusinessEnums.EmployeeStatus.Formal }
            });
        }
        /// <summary>
        /// 获取员工列表
        /// </summary>
        /// <param name="pageSearch"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetEmployeeList(EmployeePageSearch pageSearch)
        {
            List<long> departIds = new();

            //部门查询
            if (!string.IsNullOrEmpty(pageSearch.CornerMark))
            {
                //查询部门角标获取该部门下全部下级部门
                departIds = await _dbContext.Queryable<S05_Department>()
                    .Where(S05 => S05.S05_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                                  S05.S05_CornerMark.StartsWith(pageSearch.CornerMark))
                    .Select(S05 => S05.S05_DepartId)
                    .ToListAsync();
            }

            var result = await _dbContext.Queryable<S07_Employee>()
                .LeftJoin<S08_EmployeePost>((S07, S08) => S08.S08_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                                                         S08.S08_IsMainPost == (byte)BaseEnums.TrueOrFalse.True && //查询主岗位
                                                         S07.S07_EmployeeId == S08.S07_EmployeeId)
                .WhereIF(!pageSearch.IsAll, (S07, S08) => S07.S07_IsDelete == (byte)BaseEnums.TrueOrFalse.False)
                .WhereIF(departIds?.Count > 0, (S07, S08) => departIds.Contains(S08.S05_DepartId))
                .ToListResultAsync(pageSearch, new EmployeePageResult());

            if (result?.Code == ResponseCode.Success)
            {
                var list = result.ToConvertData<List<EmployeePageResult>>();
                if (list?.Count > 0)
                {
                    //查询员工角色
                    var permissionList = await _dbContext.Queryable<S09_UserPermission>()
                        .Where(S09 => S09.S09_PermissionType == (byte)BusinessEnums.PermissionType.Role)
                        .Select(S09 => new
                        {
                            UserId = S09.S01_UserId,
                            RoleId = S09.S09_CommonId,
                            RoleName = SqlFunc.Subqueryable<S03_Role>()
                                       .Where(S03 => S03.S03_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                                                     S03.S03_RoleId == S09.S09_CommonId)
                                       .Select(S03 => S03.S03_RoleName)
                        }).ToListAsync();

                    //循环获取用户角色
                    list.ForEach(item =>
                    {
                        if (item.UserId != null)
                        {
                            //角色
                            item.Roles = permissionList?.Where(c => c.UserId == item.UserId)
                                         .Select(c => new EmployeeRoleModel
                                         {
                                             RoleId = c.RoleId,
                                             RoleName = c.RoleName
                                         }).ToList();
                        }
                    });
                    result.Data = list;
                }
            }

            return result;
        }
        /// <summary>
        /// 获取员工信息
        /// </summary>
        /// <param name="employeeId">员工Id</param>
        /// <returns></returns>
        public async Task<EmployeeInfoModel> GetEmployeeInfo(long employeeId)
        {
            //获取员工信息
            var employee = await _dbContext.Queryable<S07_Employee>()
                .LeftJoin<S08_EmployeePost>((S07, S08) => S07.S07_EmployeeId == S08.S07_EmployeeId &&
                                                          S08.S08_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                                                          S08.S08_IsMainPost == (byte)BaseEnums.TrueOrFalse.True) //查询主岗位
                .Where((S07, S08) => S07.S07_IsDelete == (byte)BaseEnums.TrueOrFalse.False && S07.S07_EmployeeId == employeeId)
                .Select((S07, S08) => new EmployeeInfoModel
                {
                    EmployeeId = S07.S07_EmployeeId,
                    UserId = S07.S01_UserId,
                    CompanyId = S07.S10_CompanyId,
                    CompanyName = null,
                    DepartId = S08.S05_DepartId,
                    DepartName = SqlFunc.Subqueryable<S05_Department>()
                                            .Where(S05 => S05.S05_DepartId == S08.S05_DepartId)
                                            .Select(S05 => S05.S05_DepartName),
                    PostId = S08.S06_PostId,
                    PostName = SqlFunc.Subqueryable<S06_Post>()
                                      .Where(S06 => S06.S06_PostId == S08.S06_PostId)
                                      .Select(S06 => S06.S06_PostName),
                    QyUserId = S07.S07_QyUserId,
                    Name = S07.S07_Name,
                    Phone = S07.S07_Phone,
                    Gender = S07.S07_Gender,
                    Avatar = S07.S07_Avatar,
                    Email = S07.S07_Email,
                    Kind = S07.S07_Kind,
                    Status = S07.S07_Status,
                    Bio = S07.S07_Bio,
                    HireDate = S07.S07_HireDate,
                    TrialPeriodDate = S07.S07_TrialPeriodDate,
                    ConfirmationDate = S07.S07_ConfirmationDate,
                    TerminationDate = S07.S07_TerminationDate,
                })
                .FirstAsync();

            if (employee != null)
            {
                //判断是否存在用户Id
                if (employee.UserId != null)
                {
                    //查询用户
                    employee.Account = await _dbContext.Queryable<S01_User>()
                        .Where(S01 => S01.S01_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                                      S01.S01_UserId == employee.UserId)
                        .Select(c => new EmployeeAccountModel
                        {
                            Account = c.S01_Account,
                            Password = c.S01_Password,
                            AccountStatus = c.S01_AccountStatus
                        }).FirstAsync();

                    //查询员工功能权限
                    var permissionList = await _dbContext.Queryable<S09_UserPermission>()
                        .Where(S09 => S09.S01_UserId == employee.UserId)
                        .Select(S09 => new
                        {
                            S09.S01_UserId,
                            S09.S09_PermissionType,
                            S09.S09_CommonId
                        }).ToListAsync();

                    //员工角色
                    var roleIds = permissionList?.Where(c => c.S09_PermissionType == (byte)BusinessEnums.PermissionType.Role)
                                                 .Select(c => c.S09_CommonId)
                                                 .ToList();

                    if (roleIds?.Count > 0)
                    {
                        employee.Roles = await _dbContext.Queryable<S03_Role>()
                            .Where(S03 => S03.S03_IsDelete == (byte)BaseEnums.TrueOrFalse.False && roleIds.Contains(S03.S03_RoleId))
                            .Select(S03 => new EmployeeRoleModel
                            {
                                RoleId = S03.S03_RoleId,
                                RoleName = S03.S03_RoleName
                            }).ToListAsync();
                    }

                    //员工模块
                    var moduleIds = permissionList?.Where(c => c.S09_PermissionType == (byte)BusinessEnums.PermissionType.User)
                                                   .Select(c => c.S09_CommonId)
                                                   .ToList();

                    if (moduleIds?.Count > 0)
                    {
                        employee.Permissions = await _dbContext.Queryable<S02_Module>()
                             .Where(S02 => S02.S02_IsDelete == (byte)BaseEnums.TrueOrFalse.False && moduleIds.Contains(S02.S02_ModuleId))
                             .Select(c => new EmployeePermissionModel
                             {
                                 ModuleId = c.S02_ModuleId,
                                 ModuleName = c.S02_ModuleName,
                                 Kind = c.S02_Kind
                             }).ToListAsync();
                    }
                }
            }

            return employee;
        }
        /// <summary>
        /// 新增员工
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddEmployee(AddEmployeeInfoModel model)
        {
            //允许编辑的状态
            byte[] allowStatus = new byte[] { (byte)BusinessEnums.EmployeeStatus.Formal, (byte)BusinessEnums.EmployeeStatus.Practice };

            //校验状态
            if (!allowStatus.Contains((byte)model.Status))
                throw new UserOperationException("状态有误!");

            //名称重复校验
            bool isExist = await _dbContext.Queryable<S07_Employee>()
                .Where(S07 => S07.S07_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                              S07.S07_Phone == model.Phone)
                .AnyAsync();
            if (isExist) throw new UserOperationException("当前联系方式已存在!");

            //开启事务
            return await _dbContext.TransactionAsync(async () =>
            {
                ResponseModel result = ResponseModel.Success();

                //用户Id
                long? userId = null;

                //用户相关 判断是否创建用户
                if (model.AccountStatus)
                {
                    if (model.Account == null)
                        throw new UserOperationException("账号不能为空!");

                    if (model.Password == null)
                        throw new UserOperationException("密码不能为空!");

                    //名称重复校验
                    bool isExist = await _dbContext.Queryable<S01_User>()
                        .Where(S01 => S01.S01_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                                      S01.S01_Account == model.Account)
                        .AnyAsync();
                    if (isExist) throw new UserOperationException("账号已存在!");

                    //创建用户
                    S01_User user = new()
                    {
                        S01_Account = model.Account,
                        S01_Password = model.Password,
                        S01_AccountStatus = (byte)BusinessEnums.AccountStatus.Enable,

                        S01_IsDelete = (byte)BaseEnums.TrueOrFalse.False,
                        S01_CreateId = _employeeId,
                        S01_CreateBy = _employeeName,
                        S01_CreateTime = _dbContext.GetDate()
                    };

                    result = await _dbContext.Insertable(user).ExecuteAsync();

                    //创建用户角色权限
                    if (result?.Code == ResponseCode.Success)
                    {
                        userId = Convert.ToInt64(result.Data);

                        result = await AddUserPermission(model, (long)userId, false);
                    }
                }

                //员工相关
                if (result?.Code == ResponseCode.Success)
                {
                    //创建员工
                    S07_Employee employee = new()
                    {
                        S01_UserId = userId,
                        S10_CompanyId = model.CompanyId,
                        S07_Name = model.Name,
                        S07_Phone = model.Phone,
                        S07_Gender = model.Gender,
                        S07_Avatar = model.Avatar,
                        S07_Email = model.Email,
                        S07_Kind = model.Kind,
                        S07_Status = model.Status,
                        S07_Bio = model.Bio,
                        S07_HireDate = model.HireDate,
                        S07_TrialPeriodDate = model.TrialPeriodDate,
                        S07_ConfirmationDate = model.ConfirmationDate,
                        S07_TerminationDate = model.TerminationDate,

                        S07_IsDelete = (byte)BaseEnums.TrueOrFalse.False,
                        S07_CreateId = _employeeId,
                        S07_CreateBy = _employeeName,
                        S07_CreateTime = _dbContext.GetDate()
                    };

                    //创建员工返回主键
                    result = await _dbContext.Insertable(employee).ExecuteAsync();

                    if (result?.Code == ResponseCode.Success)
                    {
                        //员工Id
                        long employeeId = Convert.ToInt64(result.Data);

                        //创建部门岗位
                        S08_EmployeePost employeePost = new()
                        {
                            S07_EmployeeId = employeeId,
                            S05_DepartId = (long)model.DepartmentId,
                            S06_PostId = (long)model.PostId,
                            S08_IsMainPost = (byte)BaseEnums.TrueOrFalse.True,

                            S08_IsDelete = (byte)BaseEnums.TrueOrFalse.False,
                            S08_CreateId = _employeeId,
                            S08_CreateBy = _employeeName,
                            S08_CreateTime = _dbContext.GetDate()
                        };

                        result = await _dbContext.Insertable(employeePost).ExecuteAsync();
                    }
                }

                //清除数据权限
                if (result?.Code == ResponseCode.Success)
                {
                    await _dataPermission.Release();
                }

                return result;
            });
        }
        /// <summary>
        /// 编辑员工
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseModel> EditEmployee(EditEmployeeInfoModel model)
        {
            //允许编辑的状态
            byte[] allowStatus = new byte[] { (byte)BusinessEnums.EmployeeStatus.Formal, (byte)BusinessEnums.EmployeeStatus.Practice };

            //校验状态
            if (!allowStatus.Contains((byte)model.Status))
                throw new UserOperationException("状态有误!");

            return await _dbContext.TransactionAsync(async () =>
            {
                //编辑员工
                var result = await _dbContext.Updateable<S07_Employee>()
                            .SetColumns(S07 => new S07_Employee
                            {
                                S10_CompanyId = model.CompanyId,
                                S07_Name = model.Name,
                                S07_Phone = model.Phone,
                                S07_Gender = model.Gender,
                                S07_Avatar = model.Avatar,
                                S07_Email = model.Email,
                                S07_Kind = model.Kind,
                                S07_Status = model.Status,
                                S07_Bio = model.Bio,
                                S07_HireDate = model.HireDate,
                                S07_TrialPeriodDate = model.TrialPeriodDate,
                                S07_ConfirmationDate = model.ConfirmationDate,
                                S07_TerminationDate = model.TerminationDate,

                                S07_ModifyId = _employeeId,
                                S07_ModifyBy = _employeeName,
                                S07_ModifyTime = SqlFunc.GetDate()
                            })
                            .Where(S07 => S07.S07_EmployeeId == model.EmployeeId)
                            .ExecuteAsync();

                if (result?.Code == ResponseCode.Success)
                {
                    //密码进行解密
                    //model.Password = UserPasswordCryptionHelper.DecryptPassword(model.Password);

                    //查询该员工是否存在用户账号
                    var userId = await _dbContext.Queryable<S07_Employee>()
                                 .Where(S07 => S07.S07_EmployeeId == model.EmployeeId)
                                 .Select(S07 => S07.S01_UserId)
                                 .FirstAsync();

                    //启用账号
                    if (model.AccountStatus)
                    {
                        if (model.Account == null)
                            throw new UserOperationException("账号不能为空!");

                        if (model.Password == null)
                            throw new UserOperationException("密码不能为空!");

                        //用户不存在时 新增用户
                        if (userId == null)
                        {
                            //名称重复校验
                            bool isExist = await _dbContext.Queryable<S01_User>()
                                .Where(S01 => S01.S01_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                                              S01.S01_Account == model.Account)
                                .AnyAsync();
                            if (isExist) throw new UserOperationException("账号已存在!");

                            //创建用户
                            S01_User user = new()
                            {
                                S01_Account = model.Account,
                                S01_Password = model.Password,
                                S01_AccountStatus = (byte)BusinessEnums.AccountStatus.Enable,

                                S01_IsDelete = (byte)BaseEnums.TrueOrFalse.False,
                                S01_CreateId = _employeeId,
                                S01_CreateBy = _employeeName,
                                S01_CreateTime = _dbContext.GetDate()
                            };

                            result = await _dbContext.Insertable(user).ExecuteAsync();

                            //创建用户角色权限
                            if (result?.Code == ResponseCode.Success)
                            {
                                userId = Convert.ToInt64(result.Data);

                                result = await AddUserPermission(model, (long)userId, false); //新增用户，无需删除角色权限
                            }
                        }
                        //用户存在时 修改账号状态为启用并清除用户权限重新添加新传入的用户权限
                        else
                        {
                            //名称重复校验
                            bool isExist = await _dbContext.Queryable<S01_User>()
                                .Where(S01 => S01.S01_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                                              S01.S01_Account == model.Account &&
                                              S01.S01_UserId != userId)
                                .AnyAsync();
                            if (isExist) throw new UserOperationException("账号已存在!");

                            //修改用户账号及密码
                            result = await _dbContext.Updateable<S01_User>()
                                .SetColumns(S01 => new S01_User
                                {
                                    S01_Account = model.Account,
                                    S01_Password = model.Password,
                                    S01_AccountStatus = (byte)BusinessEnums.AccountStatus.Enable,

                                    S01_ModifyId = _employeeId,
                                    S01_ModifyBy = _employeeName,
                                    S01_ModifyTime = SqlFunc.GetDate()
                                })
                                .Where(S01 => S01.S01_UserId == userId)
                                .ExecuteAsync();

                            if (result?.Code == ResponseCode.Success)
                            {
                                //先删除已存在的用户角色权限，在新增用户角色权限
                                result = await AddUserPermission(model, (long)userId, true); //编辑用户，需要删除角色权限
                            }
                        }
                    }
                    //禁用账号
                    else
                    {
                        if (userId != null)
                        {
                            result = await _dbContext.Updateable<S01_User>()
                                .SetColumns(S01 => new S01_User
                                {
                                    S01_AccountStatus = (byte)BusinessEnums.AccountStatus.Disable,
                                    S01_ModifyId = _employeeId,
                                    S01_ModifyBy = _employeeName,
                                    S01_ModifyTime = SqlFunc.GetDate()
                                })
                                .Where(S01 => S01.S01_UserId == userId)
                                .ExecuteAsync();
                        }

                    }
                }

                //清除数据权限
                if (result?.Code == ResponseCode.Success)
                {
                    await _dataPermission.Release();
                }

                return result;
            });
        }
        /// <summary>
        /// 离职员工
        /// </summary>
        /// <param name="employeeId">员工Id</param>
        /// <returns></returns>
        public async Task<ResponseModel> DimissionEmployee(long employeeId)
        {
            return await _dbContext.TransactionAsync(async () =>
            {
                //更改员工状态为离职
                var result = await _dbContext.Updateable<S07_Employee>()
                            .SetColumns(S07 => new S07_Employee
                            {
                                S07_Status = (byte)BusinessEnums.EmployeeStatus.Dimission,
                                S07_IsDelete = (byte)BusinessEnums.EmployeeStatus.Dimission, //将离职员工标记为已删除，但区别于删除的员工
                                S07_ModifyId = _employeeId,
                                S07_ModifyBy = _employeeName,
                                S07_ModifyTime = SqlFunc.GetDate()
                            })
                            .Where(S07 => S07.S07_EmployeeId == employeeId)
                            .ExecuteAsync();

                //查询该员工是否存在用户账号，存在则禁用账号
                var userId = await _dbContext.Queryable<S07_Employee>()
                             .Where(S07 => S07.S07_EmployeeId == employeeId)
                             .Select(S07 => S07.S01_UserId)
                             .FirstAsync();

                //禁用员工账号
                if (userId != null && result?.Code == ResponseCode.Success)
                {

                    result = await _dbContext.Updateable<S01_User>()
                        .SetColumns(S01 => new S01_User
                        {
                            S01_AccountStatus = (byte)BusinessEnums.AccountStatus.Disable,
                            S01_IsDelete = (byte)BaseEnums.TrueOrFalse.True, //离职员工将账号删除
                            S01_ModifyId = _employeeId,
                            S01_ModifyBy = _employeeName,
                            S01_ModifyTime = SqlFunc.GetDate()
                        })
                        .Where(S01 => S01.S01_UserId == userId)
                        .ExecuteAsync();

                    //删除权限
                    if (result?.Code == ResponseCode.Success)
                    {
                        result = await _dbContext.Deleteable<S09_UserPermission>()
                            .Where(S09 => S09.S01_UserId == userId)
                            .ExecuteAsync(false);
                    }
                }

                //无效员工岗位
                if (result?.Code == ResponseCode.Success)
                {
                    result = await _dbContext.Updateable<S08_EmployeePost>()
                           .SetColumns(S08 => new S08_EmployeePost
                           {
                               S08_IsDelete = (byte)BaseEnums.TrueOrFalse.True,
                               S08_DeleteId = _employeeId,
                               S08_DeleteBy = _employeeName,
                               S08_DeleteTime = SqlFunc.GetDate()
                           })
                           .Where(S08 => S08.S07_EmployeeId == employeeId)
                           .ExecuteAsync();
                }

                //清除数据权限
                if (result?.Code == ResponseCode.Success)
                {
                    await _dataPermission.Release();
                }

                return result;
            });
        }
        /// <summary>
        /// 删除员工
        /// </summary>
        /// <param name="employeeId">员工Id</param>
        /// <returns></returns>
        public async Task<ResponseModel> DelEmployee(long employeeId)
        {
            return await _dbContext.TransactionAsync(async () =>
            {
                var result = await _dbContext.Updateable<S07_Employee>()
                            .SetColumns(S07 => new S07_Employee
                            {
                                S07_IsDelete = (byte)BaseEnums.TrueOrFalse.True,
                                S07_DeleteId = _employeeId,
                                S07_DeleteBy = _employeeName,
                                S07_DeleteTime = SqlFunc.GetDate()
                            })
                            .Where(S07 => S07.S07_EmployeeId == employeeId)
                            .ExecuteAsync();

                //查询该员工是否存在用户账号，存在则进行无效
                var userId = await _dbContext.Queryable<S07_Employee>()
                             .Where(S07 => S07.S07_EmployeeId == employeeId)
                             .Select(S07 => S07.S01_UserId)
                             .FirstAsync();

                //无效员工账号
                if (result?.Code == ResponseCode.Success && userId != null)
                {
                    result = await _dbContext.Updateable<S01_User>()
                                .SetColumns(S01 => new S01_User
                                {
                                    S01_AccountStatus = (byte)BusinessEnums.AccountStatus.Disable,
                                    S01_IsDelete = (byte)BaseEnums.TrueOrFalse.True,
                                    S01_DeleteId = _employeeId,
                                    S01_DeleteBy = _employeeName,
                                    S01_DeleteTime = SqlFunc.GetDate()
                                })
                                .Where(S01 => S01.S01_UserId == userId)
                                .ExecuteAsync();

                    //删除权限
                    if (result?.Code == ResponseCode.Success)
                    {
                        result = await _dbContext.Deleteable<S09_UserPermission>()
                            .Where(S09 => S09.S01_UserId == userId)
                            .ExecuteAsync(false);
                    }

                }

                //无效员工岗位
                if (result?.Code == ResponseCode.Success)
                {
                    result = await _dbContext.Updateable<S08_EmployeePost>()
                           .SetColumns(S08 => new S08_EmployeePost
                           {
                               S08_IsDelete = (byte)BaseEnums.TrueOrFalse.True,
                               S08_DeleteId = _employeeId,
                               S08_DeleteBy = _employeeName,
                               S08_DeleteTime = SqlFunc.GetDate()
                           })
                           .Where(S08 => S08.S07_EmployeeId == employeeId)
                           .ExecuteAsync();
                }


                //清除数据权限
                if (result?.Code == ResponseCode.Success)
                {
                    await _dataPermission.Release();
                }

                return result;
            });
        }
        #endregion

        #region 员工岗位
        /// <summary>
        /// 获取员工岗位列表
        /// </summary>
        /// <param name="employeeId">员工Id</param>
        /// <param name="index">页数</param>
        /// <param name="size">行数</param>
        /// <returns></returns>
        public async Task<ResponseModel> GetEmployeePostList(long employeeId, int? index, int? size)
        {
            return await _dbContext.Queryable<S08_EmployeePost>()
                   .Where(S08 => S08.S08_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                                 S08.S07_EmployeeId == employeeId)
                   .Select(S08 => new EmployeePostResult
                   {
                       EmployeePostId = S08.S08_EmployeePostId,
                       DepartmentId = S08.S05_DepartId,
                       DepartmentName = SqlFunc.Subqueryable<S05_Department>()
                                                       .Where(S05 => S05.S05_DepartId == S08.S05_DepartId)
                                                       .Select(S05 => S05.S05_DepartName),
                       PostId = S08.S06_PostId,
                       PostName = SqlFunc.Subqueryable<S06_Post>()
                                                 .Where(S06 => S06.S06_PostId == S08.S06_PostId)
                                                 .Select(S06 => S06.S06_PostName),
                       IsMainPost = S08.S08_IsMainPost
                   })
                   .ToListResultAsync(index, size);
        }
        /// <summary>
        /// 新增员工岗位
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddEmployeePost(AddEmployeePostModel model)
        {
            //部门岗位重复校验
            bool isExist = await _dbContext.Queryable<S08_EmployeePost>()
                .Where(S08 => S08.S08_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                              S08.S07_EmployeeId == model.EmployeeId &&
                              S08.S05_DepartId == model.DepartId &&
                              S08.S06_PostId == model.PostId)
                .AnyAsync();
            if (isExist) throw new UserOperationException("该岗位已存在!");

            bool isExistPost = await _dbContext.Queryable<S08_EmployeePost>()
                .Where(S08 => S08.S08_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                              S08.S07_EmployeeId == model.EmployeeId)
                .AnyAsync();

            //不存在岗位的员工默认设置的第一条为主岗位
            if (!isExistPost)
            {
                model.IsMainPost = (byte)BaseEnums.TrueOrFalse.True;
            }

            model.OperationId = _employeeId;
            model.OperationName = _employeeName;
            model.OperationTime = _dbContext.GetDate();

            var result = await _dbContext.InsertResultAsync<AddEmployeePostModel, S08_EmployeePost>(model);

            //释放数据权限
            if (result?.Code == ResponseCode.Success)
            {

                await _dataPermission.Release();
            }

            return result;
        }
        /// <summary>
        /// 设置员工主岗位
        /// </summary>
        /// <param name="employeePostId">员工岗位Id</param>
        /// <returns></returns>
        public async Task<ResponseModel> SetEmployeeMainPost(long employeePostId)
        {
            //部门岗位是否为主账号
            var employeePostInfo = await _dbContext.Queryable<S08_EmployeePost>()
                .Where(S08 => S08.S08_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                              S08.S08_EmployeePostId == employeePostId)
                .Select(S08 => new
                {
                    S08.S07_EmployeeId,
                    S08.S08_IsMainPost
                })
                .FirstAsync() ?? throw new UserOperationException("暂未找到该员工岗位!");

            if (employeePostInfo.S08_IsMainPost == (byte)BaseEnums.TrueOrFalse.True)
                throw new UserOperationException("当前岗位已是主岗位!");

            //开启事务
            return await _dbContext.TransactionAsync(async () =>
            {
                //更新该员工其他岗位为副岗位
                var result = await _dbContext.Updateable<S08_EmployeePost>()
                            .SetColumns(S08 => new S08_EmployeePost
                            {
                                S08_IsMainPost = (byte)BaseEnums.TrueOrFalse.False,
                                S08_ModifyId = _employeeId,
                                S08_ModifyBy = _employeeName,
                                S08_ModifyTime = SqlFunc.GetDate()
                            })
                            .Where(S08 => S08.S07_EmployeeId == employeePostInfo.S07_EmployeeId && S08.S08_EmployeePostId != employeePostId)
                            .ExecuteAsync();

                //更新当前岗位为主岗位
                if (result?.Code == ResponseCode.Success)
                {
                    result = await _dbContext.Updateable<S08_EmployeePost>()
                                .SetColumns(S08 => new S08_EmployeePost
                                {
                                    S08_IsMainPost = (byte)BaseEnums.TrueOrFalse.True,
                                    S08_ModifyId = _employeeId,
                                    S08_ModifyBy = _employeeName,
                                    S08_ModifyTime = SqlFunc.GetDate()
                                })
                                .Where(S08 => S08.S08_EmployeePostId == employeePostId)
                                .ExecuteAsync();
                }

                //释放数据权限
                if (result?.Code == ResponseCode.Success)
                {
                    await _dataPermission.Release();
                }

                return result;
            });

        }
        /// <summary>
        /// 删除员工岗位
        /// </summary>
        /// <param name="employeePostId">员工岗位Id</param>
        /// <returns></returns>
        public async Task<ResponseModel> DelEmployeePost(long employeePostId)
        {
            //部门岗位是否为主账号
            var employeePostInfo = await _dbContext.Queryable<S08_EmployeePost>()
                .Where(S08 => S08.S08_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                              S08.S08_EmployeePostId == employeePostId)
                .Select(S08 => new
                {
                    S08.S07_EmployeeId,
                    S08.S08_IsMainPost
                })
                .FirstAsync() ?? throw new UserOperationException("暂未找到该员工岗位!");

            if (employeePostInfo.S08_IsMainPost == (byte)BaseEnums.TrueOrFalse.True)
                throw new UserOperationException("当前为主岗位,不允许删除!");

            //软删除员工岗位
            var result = await _dbContext.Deleteable<S08_EmployeePost>()
                .Where(S08 => S08.S08_EmployeePostId == employeePostId)
                .SoftDeleteAsync(S08 => new S08_EmployeePost
                {
                    S08_IsDelete = (byte)BaseEnums.TrueOrFalse.True,
                    S08_DeleteId = _employeeId,
                    S08_DeleteBy = _employeeName,
                    S08_DeleteTime = SqlFunc.GetDate()
                });

            //释放数据权限
            if (result?.Code == ResponseCode.Success)
            {
                await _dataPermission.Release();
            }

            return result;
        }
        #endregion
    }
}
