using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using GCCB_OPE_FA_API.DataManagers;
using GCCB_OPE_FA_API.BLL;
using GCCB_OPE_FA_API.BLL.Models;
using Microsoft.AspNetCore.Routing;

namespace GCCB_OPE_FA_API.Functions
{
    public class OrderPricingAPIFunction
    {
        private readonly OrderPricing _orderPricing;
        //private readonly CacheManager _cacheManager;
        public OrderPricingAPIFunction(OrderPricing orderPricing) 
        {           
            _orderPricing = orderPricing;
        }

        [FunctionName("OrderPrice")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
           //var t = _cacheManager.KeyExists("key");
            log.LogInformation("C# HTTP trigger function processed a request.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);
            //name = name ?? data?.name;
            var orderPricingRequest = JsonConvert.DeserializeObject<OrderPricingRequest>(requestBody);
            var response =await  _orderPricing.ProcessOrderPricing(orderPricingRequest);
           

            return new OkObjectResult(response);
        }
    }
}
