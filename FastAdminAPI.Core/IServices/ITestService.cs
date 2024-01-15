using FastAdminAPI.Common.BASE;
using FastAdminAPI.Core.Models.Test;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.IServices
{
    /// <summary>
    /// 测试
    /// </summary>
    public interface ITestService
    {
        /// <summary>
        /// 获取code列表 mapster方式
        /// </summary>
        /// <returns></returns>
        Task<List<CodeMapsterModel>> GetCodeListWithMapster(string code);
        /// <summary>
        /// 获取code列表 Extension方式
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        Task<ResponseModel> GetCodeListWithAutoBox(CodePageSearch search);
        /// <summary>
        /// 新增Code
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResponseModel> AddCode(AddCodeModel model);
        /// <summary>
        /// 编辑Code
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResponseModel> EditCode(EditCodeModel model);
        /// <summary>
        /// 删除Code
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResponseModel> DelCode(DelCodeModel model);
        /// <summary>
        /// 通过Id删除Code
        /// </summary>
        /// <param name="codeId"></param>
        /// <returns></returns>
        Task<ResponseModel> DelCodeById(long codeId);
    }
}
