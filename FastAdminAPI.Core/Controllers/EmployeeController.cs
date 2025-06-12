using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FastAdminAPI.Common.BASE;
using FastAdminAPI.Core.Controllers.BASE;
using FastAdminAPI.Core.IServices;
using FastAdminAPI.Core.Models.Employee;
using Microsoft.AspNetCore.Mvc;

namespace FastAdminAPI.Core.Controllers
{
    /// <summary>
    /// 员工信息
    /// </summary>
    public class EmployeeController: BaseController
    {
        /// <summary>
        /// 员工Service
        /// </summary>
        private readonly IEmployeeService _employeeService;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="employeeService"></param>
        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        #region 员工
        /// <summary>
        /// 按部门Ids获取员工简要列表(不含子部门)
        /// </summary>
        /// <param name="departIds">部门Ids</param>
        /// <param name="isMainPost">是否获取员工主岗位(默认主子岗位都获取)</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<EmployeeSimpleModel>), 200)]
        public async Task<ResponseModel> GetEmployeeListByDepartIds([FromQuery] List<long> departIds, bool isMainPost = false)
        {
            return Success(await _employeeService.GetEmployeeListByDepartIds(departIds, isMainPost));
        }
        /// <summary>
        /// 按岗位Ids获取员工简要列表(不含子岗位)
        /// </summary>
        /// <param name="postIds">岗位Ids</param>
        /// <param name="isMainPost">是否获取员工主岗位(默认主子岗位都获取)</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<EmployeeSimpleModel>), 200)]
        public async Task<ResponseModel> GetEmployeeListByPostIds([FromQuery] List<long> postIds, [FromQuery] bool isMainPost = false)
        {
            return Success(await _employeeService.GetEmployeeListByPostIds(postIds, isMainPost));
        }
        /// <summary>
        /// 获取下属员工简要列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<EmployeeSimpleModel>), 200)]
        public async Task<ResponseModel> GetSubordinateEmployeeList()
        {
            return Success(await _employeeService.GetSubordinateEmployeeList());
        }
        /// <summary>
        /// 获取全部员工简要列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<EmployeeSimpleModel>), 200)]
        public async Task<ResponseModel> GetAllEmployeeList()
        {
            return Success(await _employeeService.GetAllEmployeeList());
        }
        /// <summary>
        /// 通过部门Id获取员工(只能获取主岗位)列表(包含子部门)
        /// </summary>
        /// <param name="departId">部门Id</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<EmployeePageResult>), 200)]
        public async Task<ResponseModel> GetEmployeeListByDepartId([FromQuery][Required(ErrorMessage = "岗位Id不能为空!")] long? departId)
        {
            return await _employeeService.GetEmployeeListByDepartId((long)departId);
        }
        /// <summary>
        /// 获取员工信息列表
        /// </summary>
        /// <param name="pageSearch"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(List<EmployeePageResult>), 200)]
        public async Task<ResponseModel> GetEmployeeList([FromBody] EmployeePageSearch pageSearch)
        {
            return await _employeeService.GetEmployeeList(pageSearch);
        }
        /// <summary>
        /// 获取员工信息
        /// </summary>
        /// <param name="employeeId">员工Id</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(EmployeeInfoModel), 200)]
        public async Task<ResponseModel> GetEmployeeInfo([FromQuery][Required(ErrorMessage = "员工Id不能为空!")] long? employeeId)
        {
            return Success(await _employeeService.GetEmployeeInfo((long)employeeId));
        }
        /// <summary>
        /// 新增员工
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseModel> AddEmployee([FromBody] AddEmployeeInfoModel model)
        {
            return await _employeeService.AddEmployee(model);
        }
        /// <summary>
        /// 编辑员工
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ResponseModel> EditEmployee([FromBody] EditEmployeeInfoModel model)
        {
            return await _employeeService.EditEmployee(model);
        }
        /// <summary>
        /// 离职员工
        /// </summary>
        /// <param name="employeeId">员工Id</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ResponseModel> DimissionEmployee([FromQuery][Required(ErrorMessage = "员工Id不能为空!")] long? employeeId)
        {
            return await _employeeService.DimissionEmployee((long)employeeId);
        }
        /// <summary>
        /// 删除员工
        /// </summary>
        /// <param name="employeeId">员工Id</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ResponseModel> DelEmployee([FromQuery][Required(ErrorMessage = "员工Id不能为空!")] long? employeeId)
        {
            return await _employeeService.DelEmployee((long)employeeId);
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
        [HttpGet]
        [ProducesResponseType(typeof(EmployeePostResult), 200)]
        public async Task<ResponseModel> GetEmployeePostList([FromQuery][Required(ErrorMessage = "员工Id不能为空!")] long? employeeId, int? index, int? size)
        {
            return await _employeeService.GetEmployeePostList((long)employeeId, index, size);
        }
        /// <summary>
        /// 新增员工岗位
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseModel> AddEmployeePost([FromBody] AddEmployeePostModel model)
        {
            return await _employeeService.AddEmployeePost(model);
        }
        /// <summary>
        /// 设置员工主岗位
        /// </summary>
        /// <param name="employeePostId">员工岗位Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseModel> SetEmployeeMainPost([FromQuery][Required(ErrorMessage = "员工岗位Id不能为空!")] long? employeePostId)
        {
            return await _employeeService.SetEmployeeMainPost((long)employeePostId);
        }
        /// <summary>
        /// 删除员工岗位
        /// </summary>
        /// <param name="employeePostId">员工岗位Id</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ResponseModel> DelEmployeePost([FromQuery][Required(ErrorMessage = "员工岗位Id不能为空!")] long? employeePostId)
        {
            return await _employeeService.DelEmployeePost((long)employeePostId);
        } 
        #endregion
    }
}
