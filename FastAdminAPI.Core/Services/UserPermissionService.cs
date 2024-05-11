using FastAdminAPI.Common.BASE;
using FastAdminAPI.Common.Enums;
using FastAdminAPI.Core.IServices;
using FastAdminAPI.Core.Models.UserPermission;
using FastAdminAPI.Core.Services.BASE;
using FastAdminAPI.Framework.Entities;
using FastAdminAPI.Framework.Extensions;
using Microsoft.AspNetCore.Http;
using SqlSugar;
using System.Linq;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.Services
{
    /// <summary>
    /// 用户功能权限
    /// </summary>
    public class UserPermissionService : BaseService, IUserPermissionService
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="httpContext"></param>
        public UserPermissionService(ISqlSugarClient dbContext, IHttpContextAccessor httpContext) : base(dbContext, httpContext) { }


        /// <summary>
        /// 获取所有用户功能权限列表
        /// </summary>
        /// <param name="pageSearch"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetUserPermissions(UserPermssionPageSearch pageSearch)
        {
            if (!pageSearch.EmployeeIds?.Any() ?? true && pageSearch.DepartIds?.Count > 0)
            {
                pageSearch.EmployeeIds = await _dbContext.Queryable<S08_EmployeePost>()
                    .Where(S08 => S08.S08_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                                  pageSearch.DepartIds.Contains(S08.S05_DepartId))
                    .Select(S08 => S08.S07_EmployeeId).Distinct().ToListAsync();
            }

            //获取角色权限
            var rolePermssions = _dbContext.Queryable<S01_User>()
                .InnerJoin<S09_UserPermission>((S01, S09) => S09.S01_UserId == S01.S01_UserId && S09.S09_PermissionType == (byte)BusinessEnums.PermissionType.Role)
                .InnerJoin<S07_Employee>((S01, S09, S07) => S07.S01_UserId == S01.S01_UserId && S07.S07_IsDelete == (byte)BaseEnums.TrueOrFalse.False)
                .InnerJoin<S03_Role>((S01, S09, S07, S03) => S03.S03_RoleId == S09.S09_CommonId && S03.S03_IsDelete == (byte)BaseEnums.TrueOrFalse.False)
                .InnerJoin<S04_RolePermission>((S01, S09, S07, S03, S04) => S04.S03_RoleId == S03.S03_RoleId)
                .Where((S01, S09, S07, S03, S04) => S01.S01_IsDelete == (byte)BaseEnums.TrueOrFalse.False)
                .WhereIF(pageSearch.ModuleIds?.Count > 0, (S01, S09, S07, S03, S04) => pageSearch.ModuleIds.Contains(S04.S02_ModuleId))
                .WhereIF(pageSearch.EmployeeIds?.Count > 0, (S01, S09, S07, S03, S04) => pageSearch.EmployeeIds.Contains(S07.S07_EmployeeId))
                .Select((S01, S09, S07, S03, S04) => new UserPermssionPageResult
                {
                    ModuleId = S04.S02_ModuleId,
                    ModuleName = SqlFunc.Subqueryable<S02_Module>()
                                        .Where(S02 => S02.S02_ModuleId == S04.S02_ModuleId)
                                        .Select(S02 => S02.S02_ModuleName),
                    DepartName = SqlFunc.Subqueryable<S05_Department>()
                                        .InnerJoin<S08_EmployeePost>((S05, S08) => S05.S05_DepartId == S08.S05_DepartId)
                                        .Where((S05, S08) => S08.S08_IsDelete == (byte)BaseEnums.TrueOrFalse.False && 
                                                             S08.S08_IsMainPost == (byte)BaseEnums.TrueOrFalse.True && 
                                                             S08.S07_EmployeeId == S07.S07_EmployeeId)
                                        .Select((S05, S08) => S05.S05_DepartName),
                    PostName = SqlFunc.Subqueryable<S06_Post>()
                                        .InnerJoin<S08_EmployeePost>((S06, S08) => S06.S06_PostId == S08.S06_PostId)
                                        .Where((S06, S08) => S08.S08_IsDelete == (byte)BaseEnums.TrueOrFalse.False && 
                                                             S08.S08_IsMainPost == (byte)BaseEnums.TrueOrFalse.True &&
                                                             S08.S07_EmployeeId == S07.S07_EmployeeId)
                                        .Select((S06, S08) => S06.S06_PostName),
                    EmployeeName = S07.S07_Name,
                    Contact = S07.S07_Phone
                });

            //获取用户权限
            var userPermssions = _dbContext.Queryable<S01_User>()
                 .InnerJoin<S09_UserPermission>((S01, S09) => S09.S01_UserId == S01.S01_UserId && S09.S09_PermissionType == (byte)BusinessEnums.PermissionType.User)
                 .InnerJoin<S07_Employee>((S01, S09, S07) => S07.S01_UserId == S01.S01_UserId && S07.S07_IsDelete == (byte)BaseEnums.TrueOrFalse.False)
                 .Where((S01, S09, S07) => S01.S01_IsDelete == (byte)BaseEnums.TrueOrFalse.False)
                 .WhereIF(pageSearch.ModuleIds?.Count > 0, (S01, S09, S07) => pageSearch.ModuleIds.Contains(S09.S09_CommonId))
                 .WhereIF(pageSearch.EmployeeIds?.Count > 0, (S01, S09, S07) => pageSearch.EmployeeIds.Contains(S07.S07_EmployeeId))
                .Select((S01, S09, S07) => new UserPermssionPageResult
                {
                    ModuleId = S09.S09_CommonId,
                    ModuleName = SqlFunc.Subqueryable<S02_Module>()
                                        .Where(S02 => S02.S02_ModuleId == S09.S09_CommonId)
                                        .Select(S02 => S02.S02_ModuleName),
                    DepartName = SqlFunc.Subqueryable<S05_Department>()
                                        .InnerJoin<S08_EmployeePost>((S05, S08) => S05.S05_DepartId == S08.S05_DepartId)
                                        .Where((S05, S08) => S08.S08_IsDelete == (byte)BaseEnums.TrueOrFalse.False && 
                                                             S08.S08_IsMainPost == (byte)BaseEnums.TrueOrFalse.True &&
                                                             S08.S07_EmployeeId == S07.S07_EmployeeId)
                                        .Select((S05, S08) => S05.S05_DepartName),
                    PostName = SqlFunc.Subqueryable<S06_Post>()
                                        .InnerJoin<S08_EmployeePost>((S06, S08) => S06.S06_PostId == S08.S06_PostId)
                                        .Where((S06, S08) => S08.S08_IsDelete == (byte)BaseEnums.TrueOrFalse.False && 
                                                             S08.S08_IsMainPost == (byte)BaseEnums.TrueOrFalse.True &&
                                                             S08.S07_EmployeeId == S07.S07_EmployeeId)
                                        .Select((S06, S08) => S06.S06_PostName),
                    EmployeeName = S07.S07_Name,
                    Contact = S07.S07_Phone
                });

            return await _dbContext.Union(rolePermssions, userPermssions).ToListResultAsync(pageSearch.Index, pageSearch.Size);

        }
    }
}
