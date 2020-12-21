using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApiProject.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CoreApiProject
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using(var scope = host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
                await dbContext.Database.MigrateAsync();

                //create default roles
                var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
                if(! await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
                }

                if (!await roleManager.RoleExistsAsync("Poster"))
                {
                    await roleManager.CreateAsync(new IdentityRole { Name = "Poster" });
                }
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();                    
                });
    }
}
