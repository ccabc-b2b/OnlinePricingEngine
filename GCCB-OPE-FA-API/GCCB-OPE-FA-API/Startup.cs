using Azure.Identity;
using AzureFunctions.Extensions.Swashbuckle;
using AzureFunctions.Extensions.Swashbuckle.Settings;
using GCCB_OPE_FA_API.BLL;
using GCCB_OPE_FA_API.DataManagers;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Reflection;

[assembly: FunctionsStartup(typeof(GCCB_OPE_FA_API.Startup))]
namespace GCCB_OPE_FA_API
{
    public class Startup : FunctionsStartup
    {

        public override void Configure(IFunctionsHostBuilder builder)
        {
            //ConfigurationBuilder _builder = new ConfigurationBuilder();
            //_builder.AddAzureKeyVault(new Uri(Environment.GetEnvironmentVariable("KeyVaultURI")), new DefaultAzureCredential());
            //IConfiguration configuration = _builder.Build();

            builder.Services.AddLogging();
            
            builder.Services.AddSingleton(provider =>
            {
                var connectionString = Environment.GetEnvironmentVariable("DatabaseConnectionString");
                return new Connection { ConnectionString = connectionString };
            });

            //Environment.SetEnvironmentVariable("OrderPricingAPIKey", configuration["OrderPricingAPIKey"]);
            //Environment.SetEnvironmentVariable("CatalogueAPIKey", configuration["CatalogueAPIKey"]);

            builder.Services.AddScoped<OrderPricing>();
            builder.Services.AddScoped<ConnectionManager>();
            builder.Services.AddScoped<CataloguePrice>();

            builder.AddSwashBuckle(Assembly.GetExecutingAssembly(), opts =>
            {
                opts.AddCodeParameter = false;
                opts.Documents = new[] {
                    new SwaggerDocument {
                        Name = "v1",
                            Title = "B2B API",
                            Description = "B2B Swagger UI",
                            Version = "v1"
                    }
                };
                opts.ConfigureSwaggerGen = x =>
                {
                    x.CustomOperationIds(apiDesc =>
                    {
                        return apiDesc.TryGetMethodInfo(out MethodInfo mInfo) ? mInfo.Name : default(Guid).ToString();
                    });
                };
            });
        }
    }
}
