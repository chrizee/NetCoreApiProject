using CoreApiProject.Domain;
using Cosmonaut;
using Cosmonaut.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApiProject.Services
{
    public class CosmosPostService : IPostService
    {
        private readonly ICosmosStore<CosmosPost> _CosmosStore;

        public CosmosPostService(ICosmosStore<CosmosPost> cosmosStore)
        {
            _CosmosStore = cosmosStore;
        }

        public async Task<bool> CreatePostAsync(Post post)
        {
            var cosmosPost = new CosmosPost { Id = Guid.NewGuid().ToString(), Name = post.Name };
            var response = await _CosmosStore.AddAsync(cosmosPost);
            post.Id = Guid.Parse(response.Entity.Value.Id);
            return response.IsSuccess;
        }

        public Task<bool> CreatTagAsync(Tag tag)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeletePostAsync(Guid postId)
        {
            var response = await _CosmosStore.RemoveByIdAsync(postId.ToString(), new Microsoft.Azure.Cosmos.PartitionKey(postId.ToString()));
            return response.IsSuccess;
        }

        public Task<bool> DeleteTagAsync(string tagName)
        {
            throw new NotImplementedException();
        }

        public Task<List<Tag>> GetAllTagsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Post> GetByIdAsync(Guid id)
        {
            var result = await _CosmosStore.FindAsync(id.ToString(), new Microsoft.Azure.Cosmos.PartitionKey(id.ToString()));
            if (result == null) return null;
            return new Post { Id = Guid.Parse(result.Id), Name = result.Name };
        }

        public  async Task<List<Post>> GetPostsAsync()
        {
            var posts = await _CosmosStore.Query().ToListAsync();

            return posts.Select(x => new Post { Id = Guid.Parse(x.Id), Name = x.Name }).ToList();
        }

        public Task<List<Post>> GetPostsAsync(PaginationFilter paginationFilter)
        {
            throw new NotImplementedException();
        }

        public Task<List<Post>> GetPostsAsync(GetAllPostsFilter filter = null, PaginationFilter paginationFilter = null)
        {
            throw new NotImplementedException();
        }

        public Task<Tag> GetTagByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdatePostAsync(Post updatedPost)
        {
            var cosmosPost = new CosmosPost { Id = updatedPost.Id.ToString(), Name = updatedPost.Name };
            var response = await _CosmosStore.UpdateAsync(cosmosPost);
            return response.IsSuccess;
        }

        public Task<bool> UserOwnsPostAsync(Guid postId, string userId)
        {
            throw new NotImplementedException();
        }
    }
}
