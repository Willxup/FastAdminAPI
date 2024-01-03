using FastAdminAPI.Common.BASE;
using FastAdminAPI.Core.Models.Employee;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.IServices
{
    public interface IEmployeeService
    {
        #region 员工
        /// <summary>
        /// 按部门Ids获取员工简要列表(不含子部门)
        /// </summary>
        /// <param name="departIds">部门Ids</param>
        /// <param name="isMainPost">是否获取员工主岗位</param>
        /// <returns></returns>
        Task<List<EmployeeSimpleModel>> GetEmployeeListByDepartIds(List<long> departIds, bool isMainPost);
        /// <summary>
        /// 按岗位Ids获取员工简要列表(不含子岗位)
        /// </summary>
        /// <param name="postIds">岗位Ids</param>
        /// <param name="isMainPost">是否获取员工主岗位</param>
        /// <returns></returns>
        Task<List<EmployeeSimpleModel>> GetEmployeeListByPostIds(List<long> postIds, bool isMainPost);
        /// <summary>
        /// 获取下属员工简要列表
        /// </summary>
        /// <returns></returns>
        Task<List<EmployeeSimpleModel>> GetSubordinateEmployeeList();
        /// <summary>
        /// 获取全部员工简要列表
        /// </summary>
        /// <returns></returns>
        Task<List<EmployeeSimpleModel>> GetAllEmployeeList();
        /// <summary>
        /// 通过部门Id获取员工(主岗位)列表(包含子部门)
        /// </summary>
        /// <param name="departId"></param>
        /// <returns></returns>
        Task<ResponseModel> GetEmployeeListByDepartId(long departId);
        /// <summary>
        /// 获取员工信息列表
        /// </summary>
        /// <param name="pageSearch"></param>
        /// <returns></returns>
        Task<ResponseModel> GetEmployeeList(EmployeePageSearch pageSearch);
        /// <summary>
        /// 获取员工信息
        /// </summary>
        /// <param name="employeeId">员工Id</param>
        /// <returns></returns>
        Task<EmployeeInfoModel> GetEmployeeInfo(long employeeId);
        /// <summary>
        /// 新增员工
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResponseModel> AddEmployee(AddEmployeeInfoModel model);
        /// <summary>
        /// 编辑员工
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResponseModel> EditEmployee(EditEmployeeInfoModel model);
        /// <summary>
        /// 离职员工
        /// </summary>
        /// <param name="employeeId">员工Id</param>
        /// <returns></returns>
        Task<ResponseModel> DimissionEmployee(long employeeId);
        /// <summary>
        /// 删除员工
        /// </summary>
        /// <param name="employeeId">员工Id</param>
        /// <returns></returns>
        Task<ResponseModel> DelEmployee(long employeeId);
        #endregion

        #region 员工岗位
        /// <summary>
        /// 获取员工岗位列表
        /// </summary>
        /// <param name="employeeId">员工Id</param>
        /// <returns></returns>
        Task<ResponseModel> GetEmployeePostList(long employeeId);
        /// <summary>
        /// 新增员工岗位
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResponseModel> AddEmployeePost(AddEmployeePostModel model);
        /// <summary>
        /// 设置员工主岗位
        /// </summary>
        /// <param name="employeePostId">员工岗位Id</param>
        /// <returns></returns>
        Task<ResponseModel> SetEmployeeMainPost(long employeePostId);
        /// <summary>
        /// 删除员工岗位
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResponseModel> DelEmployeePost(DelEmployeePostModel model); 
        #endregion
    }
}
