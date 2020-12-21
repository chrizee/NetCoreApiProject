using CoreApiProject.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApiProject.Services
{
    public interface IPostService
    {
        Task<bool> CreatePostAsync(Post post);
        Task<List<Post>> GetPostsAsync(GetAllPostsFilter filter = null, PaginationFilter paginationFilter = null);

        Task<Post> GetByIdAsync(Guid id);

        Task<bool> UpdatePostAsync(Post updatedPost);

        Task<bool> DeletePostAsync(Guid postId);
        Task<bool> UserOwnsPostAsync(Guid postId, string userId);
        Task<List<Tag>> GetAllTagsAsync();
        Task<bool> CreatTagAsync(Tag tag);
        Task<Tag> GetTagByNameAsync(string name);
        Task<bool> DeleteTagAsync(string tagName);
    }
}
