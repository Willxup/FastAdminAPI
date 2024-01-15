using FastAdminAPI.Common.BASE;
using FastAdminAPI.Core.Controllers.BASE;
using FastAdminAPI.Core.IServices;
using FastAdminAPI.Core.Models.Departments;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.Controllers
{
    /// <summary>
    /// 部门设置
    /// </summary>
    public class DepartmentController : BaseController
    {
        /// <summary>
        /// 部门Service
        /// </summary>
        private readonly IDepartmentService _departmentSercvice;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="departmentSercvice"></param>
        public DepartmentController(IDepartmentService departmentSercvice)
        {
            _departmentSercvice = departmentSercvice;
        }

        /// <summary>
        /// 获取部门树
        /// </summary>
        /// <param name="departName">部门名称</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(DepartmentInfoModel), 200)]
        public async Task<ResponseModel> GetDepartmentTree([FromQuery] string departName = null)
        {
            return Success(await _departmentSercvice.GetDepartmentTree(departName));
        }
        /// <summary>
        /// 新增部门
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseModel> AddDepartment([FromBody] AddDepartmentModel model)
        {
            return await _departmentSercvice.AddDepartment(model);
        }
        /// <summary>
        /// 编辑部门
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ResponseModel> EditDepartment([FromBody] EditDepartmentModel model)
        {
            return await _departmentSercvice.EditDepartment(model);
        }
        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="departId">部门Id</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ResponseModel> DelDepartment([FromQuery] [Required(ErrorMessage = "部门Id不能为空!")] long? departId)
        {
            return await _departmentSercvice.DelDepartment((long)departId);
        }
        /// <summary>
        /// 获取部门岗位编制
        /// </summary>
        /// <param name="cornerMark">角标</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(DepartmentPostStaffingModel), 200)]
        public async Task<ResponseModel> GetDepartmentPostStaffing([FromQuery][Required(ErrorMessage = "角标不能为空!")] string cornerMark)
        {
            return await _departmentSercvice.GetDepartmentPostStaffing(cornerMark);
        }
    }
}
