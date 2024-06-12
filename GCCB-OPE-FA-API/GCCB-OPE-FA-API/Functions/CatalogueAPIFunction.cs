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
    public class CatalogueAPIFunction
    {
        private readonly CataloguePrice _cataloguePrice;
        //private readonly CacheManager _cacheManager;
        public CatalogueAPIFunction(CataloguePrice cataloguePrice)
        {
            _cataloguePrice = cataloguePrice;
        }

        [FunctionName("CataloguePrice")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            //var t = _cacheManager.KeyExists("key");
            log.LogInformation("C# HTTP trigger function processed a request.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);
            //name = name ?? data?.name;
            var catalogueRequest = JsonConvert.DeserializeObject<CatalogueRequest>(requestBody);
            var response = await _cataloguePrice.ProcessCataloguePrice(catalogueRequest);


            return new OkObjectResult(response);
        }
    }
}
