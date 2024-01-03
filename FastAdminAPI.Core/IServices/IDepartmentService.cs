using FastAdminAPI.Core.Models.Departments;
using FastAdminAPI.Common.BASE;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.IServices
{
    /// <summary>
    /// 部门设置
    /// </summary>
    public interface IDepartmentService
    {
        /// <summary>
        /// 获取部门树
        /// </summary>
        /// <param name="departName">部门名称</param>
        /// <returns></returns>
        Task<string> GetDepartmentTree(string departName = null);
        /// <summary>
        /// 新增部门
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResponseModel> AddDepartment(AddDepartmentModel model);
        /// <summary>
        /// 编辑部门
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResponseModel> EditDepartment(EditDepartmentModel model);
        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResponseModel> DelDepartment(DelDepartmentModel model);
        /// <summary>
        /// 获取部门岗位编制
        /// </summary>
        /// <param name="cornerMark">角标</param>
        /// <returns></returns>
        Task<ResponseModel> GetDepartmentPostStaffing(string cornerMark);
    }
}
