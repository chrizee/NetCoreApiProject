using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreApiProject.Cache;
using CoreApiProject.Contracts.V1;
using CoreApiProject.Contracts.V1.Requests;
using CoreApiProject.Contracts.V1.Requests.Queries;
using CoreApiProject.Contracts.V1.Responses;
using CoreApiProject.Domain;
using CoreApiProject.Extensions;
using CoreApiProject.Helpers;
using CoreApiProject.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreApiProject.Controllers.V1
{
    //[Route("api/v1/[controller]")]
    [ApiController, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostsController : Controller
    {
        private readonly IPostService _PostService;
        private readonly IMapper _mapper;
        private readonly IUriService _UriService;

        public PostsController(IPostService postService, IMapper mapper, IUriService uriService)
        {
            _PostService = postService;
            _mapper = mapper;
            _UriService = uriService;
        }

        [HttpGet(ApiRoutes.Posts.GetAll)]
        [Cached(600)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllPostsQuery query, [FromQuery] PaginationQuery paginationQuery)
        {
            var paginationFilter = _mapper.Map<PaginationFilter>(paginationQuery);
            var filter = _mapper.Map<GetAllPostsFilter>(query);
            var posts = await _PostService.GetPostsAsync(filter, paginationFilter);
            var postResponse = _mapper.Map<List<PostResponse>>(posts);

            if(paginationFilter is null || paginationFilter.PageSize < 1 || paginationFilter.PageNumber < 1)
            {
                return Ok(new PagedResponse<PostResponse>(postResponse));
            }

            var paginatedResponse = PaginationHelpers.CreatePaginatedResponse(_UriService, paginationFilter, postResponse);
            return Ok(paginatedResponse);
        }

        [HttpGet(ApiRoutes.Posts.Get)]
        [Cached(600)]
        public async Task<IActionResult> Get([FromRoute] Guid postId)
        {
            var post = await _PostService.GetByIdAsync(postId);
            if (post != null)
                return Ok(new Response<PostResponse>(_mapper.Map<PostResponse>(post)));
            else
                return NotFound();
        }

        [HttpDelete(ApiRoutes.Posts.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid postId)
        {
            bool userOwnsPost = await _PostService.UserOwnsPostAsync(postId, HttpContext.GetUserId());
            if (!userOwnsPost)
            {
                return BadRequest(new { Error = "User doesn't own post" });
            }

            var deleted = await _PostService.DeletePostAsync(postId);
            if (deleted)
                return NoContent();
            else
                return NotFound();
        }

        [HttpPut(ApiRoutes.Posts.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid postId, [FromBody] UpdatePostRequest request)
        {
            bool userOwnsPost =await  _PostService.UserOwnsPostAsync(postId, HttpContext.GetUserId());
            if(!userOwnsPost)
            {
                return BadRequest(new { Error = "User doesn't own post" });
            }

            var post = await _PostService.GetByIdAsync(postId);
            post.Name = request.Name;

            var updated = await _PostService.UpdatePostAsync(post);
            if (updated)
                return Ok(new Response<PostResponse>(_mapper.Map<PostResponse>(post)));
            else
                return NotFound();
        }

        [HttpPost(ApiRoutes.Posts.Create)]
        public async Task<IActionResult> Create([FromBody] CreatePostRequest postRequest)
        {
            var postId = Guid.NewGuid();
            var post = new Post { Id = postId, Name = postRequest.Name, UserId = HttpContext.GetUserId(), Tags = postRequest.Tags.Select(x => new PostTag { PostId = postId, TagName = x }).ToList() };

            await _PostService.CreatePostAsync(post);

            var location = _UriService.GetPostUri(post.Id.ToString());


            var response = new PostResponse { Id = post.Id, Name = post.Name, Tags = post.Tags.Select(x => new TagResponse {Name = x.TagName })};
            return Created(location, new Response<PostResponse>(_mapper.Map<PostResponse>(post)));
        }
    }
}