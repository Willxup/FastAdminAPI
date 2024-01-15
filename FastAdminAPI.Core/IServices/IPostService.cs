using FastAdminAPI.Common.BASE;
using FastAdminAPI.Core.Models.Posts;
using System.Threading.Tasks;

namespace FastAdminAPI.Core.IServices
{
    /// <summary>
    /// 岗位设置
    /// </summary>
    public interface IPostService
    {
        /// <summary>
        /// 获取岗位树
        /// </summary>
        /// <param name="departId">部门Id</param>
        /// <returns></returns>
        Task<string> GetPostTree(long departId);
        /// <summary>
        /// 新增岗位
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResponseModel> AddPost(AddPostModel model);
        /// <summary>
        /// 编辑岗位
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResponseModel> EditPost(EditPostModel model);
        /// <summary>
        /// 删除岗位
        /// </summary>
        /// <param name="postId">岗位Id</param>
        /// <returns></returns>
        Task<ResponseModel> DelPost(long postId);
    }
}
