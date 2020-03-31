using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebApplication.Models;
using WebApplication.Utilities;

namespace WebApplication.Filters
{
    public class CommonStartupFilter : IStartupTask
    {
        private readonly IServiceProvider _serviceProvider;

        public CommonStartupFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            await Task.Run(() =>
            {
                using IServiceScope scope = _serviceProvider.CreateScope();
                var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                var settings = configuration.GetSection("Settings").Get<Settings>();
                Tools.SetUtilsProviderConfiguration(settings);
            }, cancellationToken);
        }

    }
}
