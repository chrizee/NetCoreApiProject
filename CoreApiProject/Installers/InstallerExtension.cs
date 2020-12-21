using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApiProject.Installers
{
    public static class InstallerExtension
    {
        public static void Install(this IServiceCollection services, IConfiguration configuration)
        {
            var installers = typeof(Startup).Assembly.GetExportedTypes().Where(x => typeof(IInstaller).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(Activator.CreateInstance).Cast<IInstaller>().ToList();

            installers.ForEach(installer => installer.Install(services, configuration));
        }
    }
}
