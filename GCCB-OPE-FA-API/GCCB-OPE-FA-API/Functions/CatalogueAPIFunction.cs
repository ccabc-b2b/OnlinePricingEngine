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
using System.Net;
using AzureFunctions.Extensions.Swashbuckle.Attribute;

namespace GCCB_OPE_FA_API.Functions
{
    public class CatalogueAPIFunction
    {
        private readonly CataloguePrice _cataloguePrice;
        
        public CatalogueAPIFunction(CataloguePrice cataloguePrice)
        {
            _cataloguePrice = cataloguePrice;
        }

        [FunctionName("CataloguePrice")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)][RequestBodyType(typeof(CatalogueRequest), "")] HttpRequest req,
            ILogger log)
        {
            try
            {
                var apiKey = req.Headers["x-api-key"];

                if (apiKey != Environment.GetEnvironmentVariable("CatalogueAPIKey"))
                {
                    var response = new CatalogueResponse
                    {
                        Status = (int)HttpStatusCode.Unauthorized,
                        Message = Constants.InvalidApiKey
                    };
                    return new UnauthorizedObjectResult(response);
                }
                else
                {
                    log.LogInformation("C# HTTP trigger function processed a request.");
                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                    var catalogueRequest = JsonConvert.DeserializeObject<CatalogueRequest>(requestBody);
                    var response = await _cataloguePrice.ProcessCataloguePrice(catalogueRequest);
                    return new OkObjectResult(response);
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Exception :{ex}");
                var response = new CatalogueResponse
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
