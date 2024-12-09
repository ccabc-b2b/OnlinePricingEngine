using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace GCCB_OPE_FA_API
    {
    public class HealthCheckFunction
        {
        private readonly HealthCheckService _healthCheckService;

        public HealthCheckFunction(HealthCheckService healthCheckService)
            {
            _healthCheckService = healthCheckService;
            }

        [FunctionName("HealthCheck")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
            {
            var report = await _healthCheckService.CheckHealthAsync();
            var result = report.Status == HealthStatus.Healthy ? "Healthy" : "Unhealthy";

            return new OkObjectResult(result);
            }
        }
    }