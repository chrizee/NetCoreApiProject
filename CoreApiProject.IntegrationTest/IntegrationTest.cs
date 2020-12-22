using CoreApiProject.Contracts.V1;
using CoreApiProject.Contracts.V1.Requests;
using CoreApiProject.Contracts.V1.Responses;
using CoreApiProject.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CoreApiProject.IntegrationTest
{
    public class IntegrationTest : IDisposable
    {
        protected readonly HttpClient _client;
        protected readonly IServiceProvider serviceProvider;
        protected IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(x => {
                    x.ConfigureServices(services => {

                        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                        services.Remove(descriptor);

                        //services.RemoveAll(typeof(ApplicationDbContext));
                        services.AddDbContext<ApplicationDbContext>(options => {
                            options.UseInMemoryDatabase("Testdb");
                        });

                        //var sp = services.BuildServiceProvider();

                        //using(var scope = sp.CreateScope())
                        //{
                        //    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        //    dbContext.Database.EnsureCreated();
                        //}
                    });
                });
            serviceProvider = appFactory.Server.Services;
            _client = appFactory.CreateClient();
        }

        public void Dispose()
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                context.Database.EnsureDeleted();
            }
        }

        protected async Task AuthenticateAsync()
        {
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", await GetJwtAsync());
        }

        protected async Task<Response<PostResponse>> CreatePostAsync(CreatePostRequest request)
        {
            var response = await _client.PostAsJsonAsync(ApiRoutes.Posts.Create, request);
            
            var far = await response.Content.ReadAsStringAsync();

            return await response.Content.ReadAsAsync<Response<PostResponse>>();
        }

        private async Task<string> GetJwtAsync()
        {
            var response = await _client.PostAsJsonAsync(ApiRoutes.Identity.Register, new UserRegistrationRequest { 
                Email = "Test123@gmail.com",
                Password = "P@ssw0rd"
            });

            var far = await response.Content.ReadAsStringAsync();

            var registrationResponse = await response.Content.ReadAsAsync<AuthsuccessResponse>();
            return registrationResponse.Token;
        }
    }
}
