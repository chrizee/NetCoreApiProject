using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApiProject.Contracts.V1;
using CoreApiProject.Contracts.V1.Requests;
using CoreApiProject.Contracts.V1.Responses;
using CoreApiProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreApiProject.Controllers.V1
{
    //[Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _IdentityService;

        public IdentityController(IIdentityService identityService)
        {
            _IdentityService = identityService;
        }

        [HttpPost(ApiRoutes.Identity.Register)]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(y => y.ErrorMessage))
                });
            }
            var authReponse = await _IdentityService.RegisterAsync(request.Email, request.Password);

            if(!authReponse.Success)
            {
                return BadRequest(new AuthFailedResponse { 
                    Errors = authReponse.Errors
                });
            }
            return Ok(new AuthsuccessResponse { 
                Token = authReponse.Token,
                RefreshToken = authReponse.RefreshToken
            });
        }

        [HttpPost(ApiRoutes.Identity.Login)]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            var authReponse = await _IdentityService.LoginAsync(request.Email, request.Password);

            if (!authReponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authReponse.Errors
                });
            }
            return Ok(new AuthsuccessResponse
            {
                Token = authReponse.Token,
                RefreshToken = authReponse.RefreshToken
            });
        }

        [HttpPost(ApiRoutes.Identity.Refresh)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var authReponse = await _IdentityService.RefreshTokenAsync(request.Token, request.RefreshToken);

            if (!authReponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authReponse.Errors
                });
            }
            return Ok(new AuthsuccessResponse
            {
                Token = authReponse.Token,
                RefreshToken = authReponse.RefreshToken
            });
        }
    }

}