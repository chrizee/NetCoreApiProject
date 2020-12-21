using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreApiProject.Contracts.V1;
using CoreApiProject.Contracts.V1.Requests;
using CoreApiProject.Contracts.V1.Responses;
using CoreApiProject.Domain;
using CoreApiProject.Extensions;
using CoreApiProject.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreApiProject.Controllers.V1
{
    [ApiController, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    public class TagsController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IMapper _mapper;

        public TagsController(IPostService postService, IMapper mapper)
        {
            _postService = postService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all tags in the store
        /// </summary>
        /// <remarks>
        ///     You can add markdown remarks here
        /// </remarks>
        /// <response code="200">Returns all the tags in the database</response>
        [HttpGet(ApiRoutes.Tags.GetAll)]
        [Authorize(Policy = "TagViewer")]
        public async Task<IActionResult> GetAll()
        {
            var tags = await _postService.GetAllTagsAsync();
            return Ok(_mapper.Map<List<TagResponse>>(tags));
        }

        [HttpGet(ApiRoutes.Tags.Get)]
        public async Task<IActionResult> Get([FromRoute] string tagName)
        {
            Tag tag = await _postService.GetTagByNameAsync(tagName);
            if (tag == null) return NotFound();
            return Ok(_mapper.Map<TagResponse>(tag));
        }

        [HttpDelete(ApiRoutes.Tags.Delete)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] string tagName)
        {
            bool deleted = await _postService.DeleteTagAsync(tagName);
            if (deleted) return NoContent();
            return NotFound();
        }

        /// <summary>
        /// Creates a new tag in the db
        /// </summary>        
        /// <param name="request"></param>
        /// <response code="201">Creates tag successfully</response>
        /// <response code="400">Unable to create tag due to validation errors</response>
        [HttpPost(ApiRoutes.Tags.Create)]
        [Authorize(Policy = "HasGmail")]
        [ProducesResponseType(typeof(TagResponse), 201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> Create([FromBody]CreateTagRequest request)
        {
            var tag = new Tag
            {
                Name = request.Name,
                CreatorId = HttpContext.GetUserId(),
                CreatedOn = DateTime.UtcNow
            };

            bool created = await _postService.CreatTagAsync(tag);
            if(!created)
            {
                return BadRequest(new ErrorResponse { Errors = new List<ErrorModel> { new ErrorModel { Message = "Error creating tag" } } });
            }

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUrl + "/" + ApiRoutes.Tags.Get.Replace("{tagName}", tag.Name);
            return Created(locationUri, _mapper.Map<TagResponse>(tag));
        }
    }
}