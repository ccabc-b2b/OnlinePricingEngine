using AzureFunctions.Extensions.Swashbuckle.Attribute;
using GCCB_OPE_FA_API.BLL;
using GCCB_OPE_FA_API.BLL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace GCCB_OPE_FA_API.Functions
{
    public class OrderPricingAPIFunction
    {
        private readonly OrderPricing _orderPricing;
        public OrderPricingAPIFunction(OrderPricing orderPricing)
        {
            _orderPricing = orderPricing;
        }

        [FunctionName("OrderPrice")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)][RequestBodyType(typeof(OrderPricingRequest), "")] HttpRequest req,
            ILogger log)
        {
            try
            {
                var apiKey = req.Headers["x-api-key"];

                if (apiKey != Environment.GetEnvironmentVariable("OrderPricingAPIKey"))
                {
                    var response = new OrderPricingResponse
                    {
                        Status = (int)HttpStatusCode.Unauthorized,
                        Message = Constants.InvalidApiKey
                    };
                    return new UnauthorizedObjectResult(response);
                }
                else
                {
                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                    var orderPricingRequest = JsonConvert.DeserializeObject<OrderPricingRequest>(requestBody);
                    log.LogInformation($"{Util.GetMarketCode(orderPricingRequest.Currency)} :OrderPrice function processed a request.");
                    log.LogInformation($"{Util.GetMarketCode(orderPricingRequest.Currency)} :Request Payload {JsonConvert.SerializeObject(orderPricingRequest)}");
                    var response = _orderPricing.ProcessOrderPricing(orderPricingRequest);
                    log.LogInformation($"{Util.GetMarketCode(orderPricingRequest.Currency)} :Response Payload {JsonConvert.SerializeObject(response)}");
                    return new OkObjectResult(response);
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Exception :{ex}");
                var response = new OrderPricingResponse
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Message = Constants.ErrorMessage
                };
                return new ObjectResult(response)
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
