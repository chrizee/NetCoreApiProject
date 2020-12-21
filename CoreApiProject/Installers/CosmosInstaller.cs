using CoreApiProject.Domain;
using Cosmonaut;
using Cosmonaut.Extensions.Microsoft.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApiProject.Installers
{
    public class CosmosInstaller : IInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration)
        {
            var cosmosDbSetting = new CosmosStoreSettings(configuration["CosmosSettings:DatabaseName"], configuration["CosmosSettings:AccountUri"], configuration["CosmosSettings:AccountKey"]);

            services.AddCosmosStore<CosmosPost>(cosmosDbSetting);
        }
    }
}
