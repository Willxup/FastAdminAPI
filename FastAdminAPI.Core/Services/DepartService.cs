using System.Linq;
using System.Threading.Tasks;
using FastAdminAPI.Business.Interfaces;
using FastAdminAPI.Business.Utilities;
using FastAdminAPI.Common.Attributes;
using FastAdminAPI.Common.BASE;
using FastAdminAPI.Common.Enums;
using FastAdminAPI.Common.Tree;
using FastAdminAPI.Core.IServices;
using FastAdminAPI.Core.Models.Depart;
using FastAdminAPI.Core.Services.BASE;
using FastAdminAPI.Framework.Entities;
using FastAdminAPI.Framework.Extensions;
using Microsoft.AspNetCore.Http;
using SqlSugar;

namespace FastAdminAPI.Core.Services
{
    /// <summary>
    /// 部门设置
    /// </summary>
    public class DepartService : BaseService, IDepartService
    {
        /// <summary>
        /// 数据权限Service
        /// </summary>
        private readonly IDataPermissionService _dataPermissionService;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="httpContext"></param>
        /// <param name="dataPermissionService"></param>
        public DepartService(ISqlSugarClient dbContext, IHttpContextAccessor httpContext,
            IDataPermissionService dataPermissionService) : base(dbContext, httpContext)
        {
            _dataPermissionService = dataPermissionService;
        }

        /// <summary>
        /// 获取部门树
        /// </summary>
        /// <param name="departName">部门名称</param>
        /// <returns></returns>
        public async Task<string> GetDepartmentTree(string departName = null)
        {
            var departList = await _dbContext.Queryable<S05_Department>()
                .Where(S05 => S05.S05_IsDelete == (byte)BaseEnums.TrueOrFalse.False)
                .Select(S05 => new DepartInfoModel
                {
                    Id = S05.S05_DepartId,
                    Name = S05.S05_DepartName,
                    ParentId = S05.S05_ParentDepartId,
                    Priority = S05.S05_Priority ?? 0,
                    Property = S05.S05_Property,
                    PropertyName = SqlFunc.Subqueryable<S99_Code>()
                        .Where(S99 => S99.S99_CodeId == S05.S05_Property).Select(S99 => S99.S99_Name),
                    Label = S05.S05_Label,
                    ParentName = SqlFunc.Subqueryable<S05_Department>()
                        .Where(a => a.S05_DepartId == S05.S05_ParentDepartId).Select(a => a.S05_DepartName),
                    CornerMark = S05.S05_CornerMark,
                    DirectordName = SqlFunc.Subqueryable<S07_Employee>()
                        .LeftJoin<S08_EmployeePost>((S07, S08) => S07.S07_EmployeeId == S08.S07_EmployeeId)
                        .LeftJoin<S06_Post>((S07, S08, S06) => S08.S06_PostId == S06.S06_PostId)
                        .Where((S07, S08, S06) => S07.S07_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                                                  S08.S08_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                                                  S08.S08_IsMainPost == (byte)BaseEnums.TrueOrFalse.True &&
                                                  S08.S05_DepartId == S05.S05_DepartId &&
                                                  S06.S06_ParentPostId == null &&
                                                  S06.S06_IsDelete == (byte)BaseEnums.TrueOrFalse.False)
                        .Select((S07, S08, S06) => S07.S07_Name)
                }).ToListAsync();

            return SortedBaseTree<DepartInfoModel>.BuildJsonTree(departList, departName);
        }
        /// <summary>
        /// 新增部门
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddDepartment(AddDepartModel model)
        {
            if (model.DepartLabelList?.Count < 0)
                throw new UserOperationException("部门标签不能为空!");

            if (model.DepartLabelList?.Count > 4)
                throw new UserOperationException("部门标签不能多超过三个!");

            model.Label = string.Join(",", model.DepartLabelList);

            model.OperationId = _employeeId;
            model.OperationName = _employeeName;
            model.OperationTime = _dbContext.GetDate();
            model.CornerMark = await CornerMarkGenerator.GetCornerMark(_dbContext, "S05_Department", "S05_DepartId",
                "S05_CornerMark", "S05_ParentDepartId", model.ParentDepartId.ToString());

            var result = await _dbContext.InsertResultAsync<AddDepartModel, S05_Department>(model);

            if (result?.Code == ResponseCode.Success)
            {
                //释放数据权限
                await _dataPermissionService.Release();
            }

            return result;
        }
        /// <summary>
        /// 编辑部门
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseModel> EditDepartment(EditDepartModel model)
        {
            if (model.DepartLabelList?.Count > 0)
                model.Label = string.Join(",", model.DepartLabelList);

            model.OperationId = _employeeId;
            model.OperationName = _employeeName;
            model.OperationTime = _dbContext.GetDate();

            var result = await _dbContext.UpdateResultAsync<EditDepartModel, S05_Department>(model);

            if (result?.Code == ResponseCode.Success)
            {
                //释放数据权限
                await _dataPermissionService.Release();
            }

            return result;
        }
        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="departId">部门Id</param>
        /// <returns></returns>
        public async Task<ResponseModel> DelDepartment(long departId)
        {
            //校验子部门
            bool isExistChild = await _dbContext.Queryable<S05_Department>()
                .Where(S05 => S05.S05_IsDelete == (byte)BaseEnums.TrueOrFalse.False && S05.S05_ParentDepartId == departId)
                .AnyAsync();
            if (isExistChild)
                throw new UserOperationException("该部门下存在子部门，无法删除!");

            //校验岗位
            bool isExist = await _dbContext.Queryable<S06_Post>()
                .Where(S06 => S06.S06_IsDelete == (byte)BaseEnums.TrueOrFalse.False && S06.S05_DepartId == departId)
                .AnyAsync();
            if (isExist)
                throw new UserOperationException("该部门已存在岗位，请移除岗位后删除!");

            var result = await _dbContext.Deleteable<S05_Department>()
                .Where(S05 => S05.S05_DepartId == departId)
                .SoftDeleteAsync(S05 => new S05_Department
                {
                    S05_IsDelete = (byte)BaseEnums.TrueOrFalse.True,
                    S05_DeleteId = _employeeId,
                    S05_DeleteBy = _employeeName,
                    S05_DeleteTime = SqlFunc.GetDate()
                });

            if (result?.Code == ResponseCode.Success)
            {
                //释放数据权限
                await _dataPermissionService.Release();
            }

            return result;
        }
        /// <summary>
        /// 获取部门岗位编制
        /// </summary>
        /// <param name="cornerMark">角标</param>
        /// <returns></returns>
        public async Task<ResponseModel> GetDepartMaxEmployeeNums(string cornerMark)
        {
            ResponseModel response = ResponseModel.Success();

            //查询部门角标获取该部门下全部下级部门
            var departIds = await _dbContext.Queryable<S05_Department>()
                .Where(S05 => S05.S05_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                              S05.S05_CornerMark.StartsWith(cornerMark))
                .Select(S05 => S05.S05_DepartId)
                .ToListAsync();

            if (departIds?.Count >= 1)
            {
                //查询岗位信息
                var postList = await _dbContext.Queryable<S06_Post>()
                    .Where(S06 => S06.S06_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                                  departIds.Contains(S06.S05_DepartId))
                    .Select(S06 => new
                    {
                        S06.S06_PostId,
                        S06.S06_MaxEmployeeNums
                    })
                    .ToListAsync();

                //查询员工岗位信息
                var employeePostList = await _dbContext.Queryable<S08_EmployeePost>()
                    .Where(S08 => S08.S08_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                                  departIds.Contains(S08.S05_DepartId))
                    .Select(S08 => new
                    {
                        S08.S05_DepartId,
                        S08.S06_PostId
                    })
                    .ToListAsync();

                DepartMaxEmployeeNumsModel maxEmployeeNums = new()
                {
                    DepartNums = departIds?.Count - 1,
                    PostSum = postList?.Count,
                    MaxEmployeeNums = $"{employeePostList?.Count}/{postList.Sum(c => c.S06_MaxEmployeeNums)}"
                };

                response.Data = maxEmployeeNums;
                return response;
            }

            return response;
        }
    }
}
