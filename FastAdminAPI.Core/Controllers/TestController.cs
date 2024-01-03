using FastAdminAPI.Core.Controllers.BASE;
using FastAdminAPI.Core.IServices;
using FastAdminAPI.Core.Models.Test;
using FastAdminAPI.Common.BASE;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.Controllers
{
    /// <summary>
    /// 测试
    /// </summary>
    [Route("api/tokenpass/[controller]/[action]")]
    #if !DEBUG
    [NonController]
    #endif
    public class TestController : BaseController
    {
        /// <summary>
        /// 测试Service
        /// </summary>
        private readonly ITestService _testService;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="testService"></param>
        public TestController(ITestService testService)
        {
            _testService = testService;
        }

        /// <summary>
        /// 获取code列表 mapster方式
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<CodeMapsterModel>), 200)]
        public async Task<ResponseModel> GetCodeListWithMapster([FromQuery] string Code)
        {
            return Success(await _testService.GetCodeListWithMapster(Code));
        }
        /// <summary>
        /// 获取code列表 AutoBox方式
        /// </summary>
        /// <param name="Search"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(List<CodePageResult>), 200)]
        public async Task<ResponseModel> GetCodeListWithAutoBox([FromBody] CodePageSearch Search)
        {
            return await _testService.GetCodeListWithAutoBox(Search);
        }
        /// <summary>
        /// 新增Code
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseModel> AddCode([FromBody] AddCodeModel model)
        {
            return await _testService.AddCode(model);
        }
        /// <summary>
        /// 编辑Code
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ResponseModel> EditCode([FromBody] EditCodeModel model)
        {
            return await _testService.EditCode(model);
        }
        /// <summary>
        /// 删除Code
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ResponseModel> DelCode([FromBody] DelCodeModel model)
        {
            return await _testService.DelCode(model);
        }
        /// <summary>
        /// 通过Id删除Code
        /// </summary>
        /// <param name="CodeId">字典Id</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ResponseModel> DelCodeById([FromQuery][Required(ErrorMessage = "字典Id不能为空!")] long? CodeId)
        {
            return await _testService.DelCodeById((long)CodeId);
        }
    }
}
