using FastAdminAPI.Common.BASE;
using FastAdminAPI.Core.Controllers.BASE;
using FastAdminAPI.Core.IServices;
using FastAdminAPI.Core.Models.Depart;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.Controllers
{
    /// <summary>
    /// 部门设置
    /// </summary>
    public class DepartController : BaseController
    {
        /// <summary>
        /// 部门Service
        /// </summary>
        private readonly IDepartService _departmentSercvice;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="departmentSercvice"></param>
        public DepartController(IDepartService departmentSercvice)
        {
            _departmentSercvice = departmentSercvice;
        }

        /// <summary>
        /// 获取部门树
        /// </summary>
        /// <param name="departName">部门名称</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(DepartInfoModel), 200)]
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
        public async Task<ResponseModel> AddDepartment([FromBody] AddDepartModel model)
        {
            return await _departmentSercvice.AddDepartment(model);
        }
        /// <summary>
        /// 编辑部门
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ResponseModel> EditDepartment([FromBody] EditDepartModel model)
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
        [ProducesResponseType(typeof(DepartMaxEmployeeNumsModel), 200)]
        public async Task<ResponseModel> GetDepartMaxEmployeeNums([FromQuery][Required(ErrorMessage = "角标不能为空!")] string cornerMark)
        {
            return await _departmentSercvice.GetDepartMaxEmployeeNums(cornerMark);
        }
    }
}
