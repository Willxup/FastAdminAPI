using System.Collections.Generic;

namespace FastAdminAPI.Core.Models.UserPermission
{
    /// <summary>
    /// 查询
    /// </summary>
    public class UserPermssionPageSearch
    {
        /// <summary>
        /// 模块Id
        /// </summary>
        public List<long> ModuleIds { get; set; }
        /// <summary>
        /// 部门Id
        /// </summary>
        public List<long> DepartIds { get; set; }
        /// <summary>
        /// 员工Id
        /// </summary>
        public List<long> EmployeeIds { get; set; }
        /// <summary>
        /// 页数
        /// </summary>
        public int? Index { get; set; }
        /// <summary>
        /// 行数
        /// </summary>
        public int? Size { get; set; }
    }
    /// <summary>
    /// 用户权限返回结果
    /// </summary>
    public class UserPermssionPageResult
    {
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartName { get; set; }
        /// <summary>
        /// 岗位名称
        /// </summary>
        public string PostName { get; set; }
        /// <summary>
        /// 员工姓名
        /// </summary>
        public string EmployeeName { get; set; }
        /// <summary>
        /// 联系方式
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleName { get; set; }
        /// <summary>
        /// 模块id
        /// </summary>
        public long ModuleId { get; set; }
    }
}
