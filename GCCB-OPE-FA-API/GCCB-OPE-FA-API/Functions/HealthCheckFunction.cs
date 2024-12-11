
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Web.Http;
using GCCB_OPE_FA_API.BLL;
using Microsoft.Identity.Client.Extensibility;

namespace LoyaltyProgram.Functions
    {
    public class HealthCheckFunctions
        {
        private readonly OrderPricing orderPricing;

        public HealthCheckFunctions(
            OrderPricing orderPricing)
            {
            this.orderPricing = orderPricing;
            }

        [FunctionName("Ping")]
        public IActionResult Ping(
            [HttpTrigger(AuthorizationLevel.Function, "get", "head", Route = null)] HttpRequest req,
            ILogger log)
            {
            return new NoContentResult();
            }

        [FunctionName("Warmup")]
        public async Task Warmup([WarmupTrigger] WarmupContext ctx)
            {
            await CheckExternalDependencies().ConfigureAwait(false);
            }

        [FunctionName("HealthCheck")]
        public async Task<IActionResult> HealthCheck(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log
        )
            {
            try
                {
                await CheckExternalDependencies().ConfigureAwait(false);
                }
            catch (Exception e)
                {
                log.LogError(e, "Error checking external dependencies");
                return new ExceptionResult(e, includeErrorDetail: true);
                }
            return new OkResult();
            }

        private Task CheckExternalDependencies()
            {
            return Task.WhenAll(
                orderPricing.WarmUp()
            );
            }
        }
    }
