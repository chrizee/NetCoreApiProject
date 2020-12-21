using CoreApiProject.Data;
using CoreApiProject.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApiProject.Services
{
    public class PostService : IPostService
    {       
        private readonly ApplicationDbContext _DbContext;

        public PostService(ApplicationDbContext dbContext)
        {           
            _DbContext = dbContext;
        }

        public async Task<bool> CreatePostAsync(Post post)
        {
            post.Tags?.ForEach(tag => tag.TagName = tag.TagName.ToLower());

            await AddNewTags(post);
            await _DbContext.Posts.AddAsync(post);
            var saved = await _DbContext.SaveChangesAsync();
            return saved > 0;
        }

        private async Task AddNewTags(Post post)
        {
            foreach(var tag in post.Tags)
            {
                var existingTag = await _DbContext.Tags.SingleOrDefaultAsync(x => x.Name == tag.TagName);
                if (existingTag != null) continue;
                await _DbContext.Tags.AddAsync(new Tag { Name = tag.TagName, CreatedOn = DateTime.UtcNow, CreatorId = post.UserId });
            }
        }

        public async Task<bool> DeletePostAsync(Guid postId)
        {
            var post = await GetByIdAsync(postId);

            if (post is null) return false;
            _DbContext.Posts.Remove(post);
            var deleted = await _DbContext.SaveChangesAsync();
            return deleted > 0;
        }

        public async Task<List<Tag>> GetAllTagsAsync()
        {
            return await _DbContext.Tags.AsNoTracking().ToListAsync();
        }

        public async Task<Post> GetByIdAsync(Guid id)
        {
            return await _DbContext.Posts.Include(x => x.Tags).SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Post>> GetPostsAsync(GetAllPostsFilter filter = null,  PaginationFilter paginationFilter = null)
        {
            var query = _DbContext.Posts.AsQueryable();

            if(!string.IsNullOrEmpty(filter?.UserId))
            {
                query = query.Where(x => x.UserId == filter.UserId);
            }
            if(paginationFilter is null)
                return await query.Include(x => x.Tags).ToListAsync();


            return await query.Include(x => x.Tags).Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize).Take(paginationFilter.PageSize).ToListAsync();
        }

        public async Task<bool> UpdatePostAsync(Post updatedPost)
        {
            _DbContext.Posts.Update(updatedPost);
            int updateCount = await _DbContext.SaveChangesAsync();
            return updateCount > 0;
        }

        public async Task<bool> UserOwnsPostAsync(Guid postId, string userId)
        {
            var post = await _DbContext.Posts.AsNoTracking().SingleOrDefaultAsync(x => x.Id == postId);

            if (post == null) return false;
            return post.UserId == userId;
        }

        public async Task<bool> CreatTagAsync(Tag tag)
        {
            tag.Name = tag.Name.ToLower();
            var existing = await _DbContext.Tags.AsNoTracking().SingleOrDefaultAsync(x => x.Name == tag.Name);
            if (existing != null) return true;

            await _DbContext.Tags.AddAsync(tag);
            var created = await _DbContext.SaveChangesAsync();
            return created > 0;
        }

        public async Task<Tag> GetTagByNameAsync(string name)
        {
            return await _DbContext.Tags.AsNoTracking().SingleOrDefaultAsync(x => x.Name == name.ToLower());
        }

        public async Task<bool> DeleteTagAsync(string tagName)
        {
            var tag = await _DbContext.Tags.AsNoTracking().SingleOrDefaultAsync(x => x.Name == tagName.ToLower());
            if (tag == null) return true;

            var postTags = await _DbContext.PostTags.AsNoTracking().Where(x => x.TagName == tag.Name).ToListAsync();
            _DbContext.PostTags.RemoveRange(postTags);
            _DbContext.Tags.Remove(tag);
            return await _DbContext.SaveChangesAsync() > postTags.Count();
        }
    }
}
