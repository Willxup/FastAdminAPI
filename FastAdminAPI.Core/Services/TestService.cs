using FastAdminAPI.Common.BASE;
using FastAdminAPI.Common.Enums;
using FastAdminAPI.Core.IServices;
using FastAdminAPI.Core.Models.Test;
using FastAdminAPI.Core.Services.BASE;
using FastAdminAPI.Framework.Entities;
using FastAdminAPI.Framework.Extensions;
using Mapster;
using SqlSugar;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.Services
{
    /// <summary>
    /// 测试
    /// </summary>
    public class TestService : BaseService, ITestService
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="dbContext"></param>
        public TestService(ISqlSugarClient dbContext) : base(dbContext) { }

        /// <summary>
        /// 获取code列表 mapster方式
        /// </summary>
        /// <returns></returns>
        public async Task<List<CodeMapsterModel>> GetCodeListWithMapster(string code)
        {
            var list = await _dbContext.Queryable<S99_Code>()
                .Where(c => c.S99_IsDelete == (byte)BaseEnums.TrueOrFalse.False &&
                            c.S99_GroupCode == code)
                .ToListAsync();

            //实体映射为dto
            var result = list.Adapt<List<CodeMapsterModel>>();

            return result;
        }
        /// <summary>
        /// 获取code列表 AutoBox方式
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetCodeListWithAttributes(CodePageSearch search)
        {
            return await _dbContext.Queryable<S99_Code>()
                .InnerJoin<S99_Code>((a, b) => a.S99_CodeId == b.S99_CodeId)
                .ToListResultAsync(search, new CodePageResult()); //查询条件 - 查询结果
        }
        /// <summary>
        /// 新增Code
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddCode(AddCodeModel model)
        {
            model.OperationId = -1;
            model.OperationName = "test";
            model.OperationTime = _dbContext.GetDate();
            return await _dbContext.InsertResultAsync<AddCodeModel, S99_Code>(model);
        }
        /// <summary>
        /// 编辑Code
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseModel> EditCode(EditCodeModel model)
        {
            model.OperationId = -1;
            model.OperationName = "test";
            model.OperationTime = _dbContext.GetDate();
            return await _dbContext.UpdateResultAsync<EditCodeModel, S99_Code>(model);
        }
        /// <summary>
        /// 删除Code
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseModel> DelCode(DelCodeModel model)
        {
            model.OperationId = -1;
            model.OperationName = "test";
            model.OperationTime = _dbContext.GetDate();
            return await _dbContext.SoftDeleteAsync<DelCodeModel, S99_Code>(model);
        }
        /// <summary>
        /// 通过Id删除Code
        /// </summary>
        /// <param name="codeId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> DelCodeById(long codeId)
        {
            return await _dbContext.Deleteable<S99_Code>()
                .Where(c => c.S99_CodeId == codeId)
                .SoftDeleteAsync(c => new S99_Code 
                {
                    S99_IsDelete = (byte)BaseEnums.TrueOrFalse.True,
                    S99_DeleteId = -1,
                    S99_DeleteBy = "test",
                    S99_DeleteTime = SqlFunc.GetDate()
                });
        }

    }
}
