using GCCB_OPE_FA_API.BLL.Models;
using GCCB_OPE_FA_API.DataManagers;
using GCCB_OPE_FA_API.DataManagers.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GCCB_OPE_FA_API.BLL
{
    public class CataloguePrice
    {
        private readonly ILogger _logger;
        private readonly ConnectionManager _connectionManager;
        private readonly OrderPricing _orderPricing;
        public CataloguePrice(ILogger<CataloguePrice> logger, ConnectionManager connectionManager,OrderPricing orderPricing)
        {
            _logger = logger;
            _connectionManager = connectionManager;
            _orderPricing = orderPricing;
        }
        public async Task<CatalogueResponse> ProcessCataloguePrice(CatalogueRequest catalogueRequest)
        {
            //var key = $"{CountryCode}_{ConditionType}_{variablekey}"; //Todo
            _logger.LogInformation("Process catalog pricing");

            var customerParameter = new SqlParameter[] {
                new SqlParameter("@CustomerNumber",catalogueRequest.SoldToCustomerId)
            };
            var customer = Util.DataTabletoList<Customer>(_connectionManager.ExecuteStoredProcedure("CustomerFetch", customerParameter)).FirstOrDefault();
            List<string> materiallist = _connectionManager.FetchMaterial(catalogueRequest.SoldToCustomerId);
            var materialParameter = new SqlParameter[]
            {
                new SqlParameter("@MaterialNumber",string.Join(",", materiallist.ToList()))
            };
            var materials = Util.DataTabletoList<Material>(_connectionManager.ExecuteStoredProcedure("MaterialFetch", materialParameter));

            var response = new CatalogueResponse();

            response.Status = (int)HttpStatusCode.OK;
            response.Message = Constants.SuccessMessage;
            var result = new Results();
            result.Currency = catalogueRequest.Currency;
            result.TransactionDate=catalogueRequest.TransactionDate;
            result.SourceTransactionId = Guid.NewGuid().ToString();
            result.Items = FetchMaterialBasePrice(catalogueRequest, customer, materials);
            response.Result = result;

            return response;
        }
        public List<Items> FetchMaterialBasePrice(CatalogueRequest orderPricingRequest, Customer customer, List<Material> Materials)
        {
            List<Items> lstItems = new List<Items>();

            //get the db details --TODO
            foreach (var material in Materials)
            { 
                var keyMapping =VariableKeyMapping(customer, material, Constants.CurrencyToCountry[orderPricingRequest.Currency]);

                var conditionItemsParameter = new SqlParameter[]
                {
                new SqlParameter("@VariableKey",string.Join(",", keyMapping.Select(x=>x.VariableKeyValue).ToList()))
                };
                var conditionItems = Util.DataTabletoList<ConditionItems>(_connectionManager.ExecuteStoredProcedure("ConditionItemsFetch", conditionItemsParameter));

                Items item = new Items();
                item.ProductId = material.MaterialNumber;
                item.BasePrice = conditionItems[0].ConditionAmountOrPercentageRate;  
                lstItems.Add(item);
            }
            return lstItems;
        }
        private List<PricingMatrix> VariableKeyMapping(Customer customer, Material material, string country)
        {
            var parameter = new SqlParameter[] {
                new SqlParameter("@Country",country)
            };
            List<PricingMatrix> lstPricingMatrix = Util.DataTabletoList<PricingMatrix>(_connectionManager.ExecuteStoredProcedure("PricingMatrixFetch", parameter));
            foreach (var pricingMatrix in lstPricingMatrix)
            {
                var variableKey = "";
                List<string> splitKeys = pricingMatrix.VariableKeyFetch.Split('+').ToList();
                foreach (var splitKey in splitKeys)
                {
                    switch (splitKey)
                    {
                        case Constants.Customer:
                            variableKey += (customer.CustomerNumber != null) ? customer.CustomerNumber.PadRight(Util.CharLengthMapping(Constants.Customer), ' ') : "";
                            //variableKey += "0000005000";
                            break;
                        case Constants.Material:
                            //variableKey += "10151900";
                            variableKey += (material.MaterialNumber != null) ? material.MaterialNumber.PadRight(Util.CharLengthMapping(Constants.Material), '0') : "";
                            break;
                        case Constants.PriceList:
                            variableKey += (customer.PriceListType != null) ? customer.PriceListType.PadRight(Util.CharLengthMapping(Constants.PriceList), ' ') : "";
                            break;
                        case Constants.SalesOrg:
                            variableKey += (customer.SalesOrg != null) ? customer.SalesOrg.PadRight(Util.CharLengthMapping(Constants.SalesOrg), ' ') : "";
                            break;
                        case Constants.SalesGroup:
                            variableKey += (customer.SalesGroup != null) ? customer.SalesGroup.PadRight(Util.CharLengthMapping(Constants.SalesGroup), ' ') : "";
                            break;
                        case Constants.MaterialGroup:
                            variableKey += (material.MaterialGroup != null) ? material.MaterialGroup.PadRight(Util.CharLengthMapping(Constants.MaterialGroup), ' ') : "";
                            break;
                        case Constants.KeyAccount:
                            variableKey += (customer.IndustryCode1 != null) ? customer.IndustryCode1.PadRight(Util.CharLengthMapping(Constants.KeyAccount), ' ') : "";
                            break;
                        case Constants.Plant:
                            variableKey += (material.PlantID != null) ? material.PlantID.PadRight(Util.CharLengthMapping(Constants.Plant), ' ') : "";
                            break;
                        case Constants.PaymentTerms:
                            variableKey += (customer.PaymentTerms != null) ? customer.PaymentTerms.PadRight(Util.CharLengthMapping(Constants.PaymentTerms), ' ') : "";
                            break;
                        case Constants.BottlerTr:
                            variableKey += (customer.BottlerTr != null) ? customer.BottlerTr.PadRight(Util.CharLengthMapping(Constants.BottlerTr), ' ') : "";
                            break;
                        case Constants.SubTrade:
                            variableKey += (customer.BottlerTr != null) ? customer.BottlerTr.PadRight(Util.CharLengthMapping(Constants.SubTrade), ' ') : "";
                            break;
                        case Constants.POType:
                            variableKey += (customer.POType != null) ? customer.POType.PadRight(Util.CharLengthMapping(Constants.POType), ' ') : "";
                            break;
                        case Constants.SubTradeChannel:
                            variableKey += (customer.CustomerSubTrade != null) ? customer.CustomerSubTrade.PadRight(Util.CharLengthMapping(Constants.SubTradeChannel), ' ') : "";
                            break;
                        default:
                            break;
                    }
                }
                pricingMatrix.VariableKeyValue = variableKey;
            }
            //var lst1 = lst.Distinct().ToList();
            return lstPricingMatrix;
        }
    }
}
