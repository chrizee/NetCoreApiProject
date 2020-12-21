using CoreApiProject.Contracts.V1.Requests;
using CoreApiProject.Contracts.V1.Responses;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoreApiProject.Sdk
{
    public interface IIdentityApi
    {
        [Post("/api/v1/identity/register")]
        Task<ApiResponse<AuthsuccessResponse>> RegisterAsync([Body] UserRegistrationRequest request);

        [Post("/api/v1/identity/login")]
        Task<ApiResponse<AuthsuccessResponse>> LoginAsync([Body] UserLoginRequest request);

        [Post("/api/v1/identity/refresh")]
        Task<ApiResponse<AuthsuccessResponse>> RefreshAsync([Body] RefreshTokenRequest request);
    }
}
