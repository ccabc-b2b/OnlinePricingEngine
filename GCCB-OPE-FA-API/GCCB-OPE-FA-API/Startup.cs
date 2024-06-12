using GCCB_OPE_FA_API.DataManagers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using GCCB_OPE_FA_API.BLL;

[assembly: FunctionsStartup(typeof(GCCB_OPE_FA_API.Startup))]
namespace GCCB_OPE_FA_API
{
    public class Startup : FunctionsStartup
    {

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();
            // builder.Services.AddHttpClient();
            builder.Services.AddSingleton(provider =>
            {
                //var configuration = provider.GetRequiredService<IConfiguration>();
                var connectionString = Environment.GetEnvironmentVariable("DatabaseConnectionString");
                return new Connection { ConnectionString = connectionString };
            });
           // builder.Services.AddSingleton(proviider => new CacheManager(Environment.GetEnvironmentVariable("RedisConnectionString")));
           
            builder.Services.AddScoped<OrderPricing>();          
            builder.Services.AddScoped<ConnectionManager>();
            builder.Services.AddScoped<CataloguePrice>();
        }
    }
}
