using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FastAdminAPI.Common.BASE;
using FastAdminAPI.Core.Controllers.BASE;
using FastAdminAPI.Core.IServices;
using FastAdminAPI.Core.Models.Posts;
using Microsoft.AspNetCore.Mvc;

namespace FastAdminAPI.Core.Controllers
{
    /// <summary>
    /// 岗位设置
    /// </summary>
    public class PostController : BaseController
    {
        /// <summary>
        /// 岗位Service
        /// </summary>
        private readonly IPostService _postService;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="postService"></param>
        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        /// <summary>
        /// 获取岗位树
        /// </summary>
        /// <param name="departId">部门Id</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PostInfoModel), 200)]
        public async Task<ResponseModel> GetPostTree([FromQuery][Required(ErrorMessage = "部门Id不能为空!")] long? departId)
        {
            return Success(await _postService.GetPostTree((long)departId));
        }
        /// <summary>
        /// 获取多个岗位树
        /// </summary>
        /// <param name="departIds">部门Ids</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PostInfoModel), 200)]
        public async Task<ResponseModel> GetMultiPostTree([FromQuery] long[] departIds)
        {
            return Success(await _postService.GetMultiPostTree(departIds));
        }
        /// <summary>
        /// 获取岗位信息
        /// </summary>
        /// <param name="postId">岗位Id</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PostInfoModel), 200)]
        public async Task<ResponseModel> GetPostById([FromQuery][Required(ErrorMessage = "岗位Id不能为空!")] long postId)
        {
            return Success(await GetPostById(postId));
        }
        /// <summary>
        /// 新增岗位
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseModel> AddPost([FromBody] AddPostModel model)
        {
            return await _postService.AddPost(model);
        }
        /// <summary>
        /// 编辑岗位
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ResponseModel> EditPost([FromBody] EditPostModel model)
        {
            return await _postService.EditPost(model);
        }
        /// <summary>
        /// 删除岗位
        /// </summary>
        /// <param name="postId">岗位Id</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ResponseModel> DelPost([FromQuery][Required(ErrorMessage = "岗位Id不能为空!")] long? postId)
        {
            return await _postService.DelPost((long)postId);
        }
    }
}
