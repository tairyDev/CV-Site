using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GitHubService.Services
{
    public static class Extenstions
    {
        public static void AddGitHubIntegretions(this IServiceCollection services,Action<GitHubIntegretionOption> configur)
        {
            services.Configure(configur);
            services.AddScoped<IGitHubService,GitHubService>();
        }
    }
}
