﻿using CoreApiProject.Contracts.V1;
using CoreApiProject.Contracts.V1.Responses;
using CoreApiProject.Domain;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CoreApiProject.IntegrationTest
{
    public class PostControllerTest : IntegrationTest
    {
        [Fact]
        public async Task GetAll_WithoutAnyPost_ReturnsEmptyResponse()
        {
            //Arrange
            await AuthenticateAsync();
            //Act

            var response = await _client.GetAsync(ApiRoutes.Posts.GetAll);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadAsAsync<PagedResponse<PostResponse>>()).Data.Should().BeEmpty();
        }

        [Fact]
        public async Task Get_ReturnsPost_WhenPostExistInDatabase()
        {
            //Arrange
            await AuthenticateAsync();
            var createdPost = await CreatePostAsync(new Contracts.V1.Requests.CreatePostRequest { Name = "Test Post" });

            //Act
            var response = await _client.GetAsync(ApiRoutes.Posts.Get.Replace("{postId}", createdPost.Data.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var returnedPost = await response.Content.ReadAsAsync<Response<PostResponse>>();
            returnedPost.Data.Id.Should().Be(createdPost.Data.Id);
            returnedPost.Data.Name.Should().Be("Test Post");
        }
    }
}
