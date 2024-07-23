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

namespace GCCB_OPE_FA_API.BLL
{
    public class OrderPricing
    {
        private readonly ILogger _logger;
        private readonly ConnectionManager _connectionManager;
        public OrderPricing(ILogger<OrderPricing> logger, ConnectionManager connectionManager)
        {
            _logger = logger;
            _connectionManager = connectionManager;
        }
        public OrderPricingResponse ProcessOrderPricing(OrderPricingRequest orderPricingRequest)
        {
            //var key = $"{CountryCode}_{ConditionType}_{variablekey}"; 
            _logger.LogInformation("Process order pricing");

            var customerParameter = new SqlParameter[] {
                new SqlParameter("@CustomerNumber",orderPricingRequest.SoldToCustomerId)
            };
            var customer = Util.DataTabletoList<Customer>(_connectionManager.ExecuteStoredProcedure("CustomerFetch", customerParameter)).FirstOrDefault();
            //string value = "'"+string.Join("','", productIds)+"'";
            var materialParameter = new SqlParameter[]
            {
                new SqlParameter("@MaterialNumber",string.Join(",", orderPricingRequest.Items.Select(x => x.ProductId).ToList()))
            };
            var materials = Util.DataTabletoList<Material>(_connectionManager.ExecuteStoredProcedure("MaterialFetch", materialParameter));

            var response = new OrderPricingResponse();

            response.Status = (int)HttpStatusCode.OK;
            response.Message = Constants.SuccessMessage;
            var result = new Result();
            result.SyncDate = DateTime.Now;//Todo        
            result.PricingDetails = CalculatePricePromo(orderPricingRequest, customer, materials);
            result.SubTotalPrice = result.PricingDetails.Sum(x => x.SubTotalPrice * x.quantity);
            result.Rewards = result.PricingDetails.Sum(x => x.Rewards * x.quantity);
            result.Discount = result.PricingDetails.Sum(x => x.Discount * x.quantity);
            result.NetPrice = result.PricingDetails.Sum(x => x.NetPrice * x.quantity);
            result.TotalPrice = result.PricingDetails.Sum(x => x.TotalPrice * x.quantity);
            result.TotalTax = result.PricingDetails.Sum(x => x.TotalTax * x.quantity);
            result.SalesOrg = customer.SalesOrg;
            result.SalesRoute = customer.SalesRoute;
            response.Result = result;

            return response;
        }

        public List<PricingDetails> CalculatePricePromo(OrderPricingRequest orderPricingRequest, Customer customer, List<Material> Materials)
        {
            List<PricingDetails> lstPricingDetails = new List<PricingDetails>();

            var parameter = new SqlParameter[]
            {
                new SqlParameter("@CustomerNumber",orderPricingRequest.SoldToCustomerId),
                new SqlParameter("@MaterialNumber",string.Join(",", orderPricingRequest.Items.Select(x => x.ProductId).ToList()))
            };
            var promotions = Util.DataTabletoList<Promotion>(_connectionManager.ExecuteStoredProcedure("ItemPromotion", parameter));
            foreach (var item in orderPricingRequest.Items)
            {
                var material = Materials.Where(x => x.MaterialNumber.Equals(item.ProductId)).FirstOrDefault();
                if (material != null)
                {
                    var keyMappings = VariableKeyMapping(customer, material, Constants.CurrencyToCountry[orderPricingRequest.Currency]);

                    RuleHandler ruleHandler = new RuleHandler();
                    keyMappings = ruleHandler.CheckConditionTableRule(material, keyMappings);

                    var conditionItemsParameter = new SqlParameter[]
                    {
                      new SqlParameter("@VariableKey",string.Join(",", keyMappings.Select(x=>x.VariableKeyValue).ToList()))
                    };
                    var conditionItems = Util.DataTabletoList<ConditionItems>(_connectionManager.ExecuteStoredProcedure("ConditionItemsFetch", conditionItemsParameter));
                    conditionItems.ForEach(x => x.ConditionAmountOrPercentageRate.Replace("-", ""));
                    var pricingDetails = CalculatePricing(orderPricingRequest, item, customer, conditionItems, keyMappings, promotions);

                    lstPricingDetails.Add(pricingDetails);
                }
            }
            return lstPricingDetails;
        }
        public PricingDetails CalculatePricing(OrderPricingRequest orderPricingRequest, Item item, Customer customer, List<ConditionItems> conditionItems, List<PricingMatrix> keyMappings, List<Promotion> promotions)
        {
            var pricingDetails = new PricingDetails();
            pricingDetails.product = Convert.ToInt32(item.ProductId);
            pricingDetails.quantity = item.Quantity;
            RuleHandler basicRule = new RuleHandler();
            var rules = basicRule.CheckPricingRule(conditionItems, keyMappings);
            decimal MWST = 0;

            if (orderPricingRequest.Currency == Constants.AED)
            {
                var grossValue = rules.Select(x => x.YPR0).FirstOrDefault() - rules.Select(x => x.YBAJ).FirstOrDefault();//YPR0 - YBAJ;
                var totalTradeDiscount = rules.Select(x => x.YBUY).FirstOrDefault() + rules.Select(x => x.YTDN).FirstOrDefault() + rules.Select(x => x.YRPO).FirstOrDefault();//YBUY + YTDN + YRPO;
                var netofTradeDiscount = grossValue - totalTradeDiscount;
                var netofEDLP = netofTradeDiscount - rules.Select(x => x.YELP).FirstOrDefault();
                var netValue = netofEDLP - rules.Select(x => x.YPDN).FirstOrDefault() - rules.Select(x => x.YPN2).FirstOrDefault();
                //var consumerPromotion = YAC1 + YAC4 + YAC2 + YAC3 + YAC5 + YAC6;
                //totaltax Mwst = 5 % of Netvalue
                MWST = (customer.TaxClassification.Equals("1") ? 5 * netValue / 100 : 0);
                var total = netValue + MWST;

                pricingDetails.SubTotalPrice = grossValue;
                pricingDetails.Discount = totalTradeDiscount;
                pricingDetails.NetPrice = netValue;
                pricingDetails.TotalPrice = total;
                pricingDetails.TotalTax = MWST;
            }

            if (orderPricingRequest.Currency == Constants.OMR)
            {
                var grossValue = (rules.Select(x => x.YPR0).FirstOrDefault() < rules.Select(x => x.YBAJ).FirstOrDefault()) ? rules.Select(x => x.YPR0).FirstOrDefault() : rules.Select(x => x.YBAJ).FirstOrDefault();//YPR0 less YBAJ;
                var totalTradeDiscount = rules.Select(x => x.YBUY).FirstOrDefault() + rules.Select(x => x.YTDN).FirstOrDefault() + rules.Select(x => x.YRPO).FirstOrDefault();//YBUY + YTDN + YRPO;
                var netofTradeDiscount = grossValue - totalTradeDiscount;
                var netofEDLP = netofTradeDiscount - rules.Select(x => x.YELP).FirstOrDefault();
                var netValue = netofEDLP - rules.Select(x => x.YPDN).FirstOrDefault() - rules.Select(x => x.YPN2).FirstOrDefault();

                //totaltax Mwst = 5 % of Netvalue
                MWST = (customer.TaxClassification.Equals("1") ? 5 * netValue / 100 : 0);
                var total = netValue + MWST;

                pricingDetails.SubTotalPrice = grossValue;
                pricingDetails.Discount = totalTradeDiscount;
                pricingDetails.NetPrice = netValue;
                pricingDetails.TotalPrice = total;
                pricingDetails.TotalTax = MWST;
            }
            if (orderPricingRequest.Currency == Constants.QAR)
            {
                var grossValue = (rules.Select(x => x.YPR0).FirstOrDefault() < rules.Select(x => x.YBAJ).FirstOrDefault()) ? rules.Select(x => x.YPR0).FirstOrDefault() : rules.Select(x => x.YBAJ).FirstOrDefault();//YPR0 less YBAJ;
                var totalTradeDiscount = rules.Select(x => x.YBUY).FirstOrDefault() + rules.Select(x => x.YTDN).FirstOrDefault() + rules.Select(x => x.YRPO).FirstOrDefault();//YBUY + YTDN + YRPO;
                var netofTradeDiscount = grossValue - totalTradeDiscount;
                var netofEDLP = netofTradeDiscount - rules.Select(x => x.YELP).FirstOrDefault();
                var netValue = netofEDLP - rules.Select(x => x.YPDN).FirstOrDefault() - rules.Select(x => x.YPN2).FirstOrDefault();

                //totaltax Mwst = 5 % of Netvalue
                MWST = (customer.TaxClassification.Equals("1") ? 5 * netValue / 100 : 0);
                var total = netValue + MWST;

                pricingDetails.SubTotalPrice = grossValue;
                pricingDetails.Discount = totalTradeDiscount;
                pricingDetails.NetPrice = netValue;
                pricingDetails.TotalPrice = total;
                pricingDetails.TotalTax = MWST;

            }
            if (orderPricingRequest.Currency == Constants.BHD)
            {
                var grossValue = (rules.Select(x => x.YPR0).FirstOrDefault() < rules.Select(x => x.YBAJ).FirstOrDefault()) ? rules.Select(x => x.YPR0).FirstOrDefault() : rules.Select(x => x.YBAJ).FirstOrDefault();//YPR0 less YBAJ;
                var totalTradeDiscount = rules.Select(x => x.YBUY).FirstOrDefault() + rules.Select(x => x.YTDN).FirstOrDefault() + rules.Select(x => x.YRPO).FirstOrDefault();//YBUY + YTDN + YRPO;
                var netofTradeDiscount = grossValue - totalTradeDiscount;
                var netofEDLP = netofTradeDiscount - rules.Select(x => x.YELP).FirstOrDefault();
                var netValue = netofEDLP - rules.Select(x => x.YPDN).FirstOrDefault() - rules.Select(x => x.YPN2).FirstOrDefault();

                //totaltax Mwst = 5 % of Netvalue
                MWST = (customer.TaxClassification.Equals("1") ? 5 * netValue / 100 : 0);
                var total = netValue + MWST;

                pricingDetails.SubTotalPrice = grossValue;
                pricingDetails.Discount = totalTradeDiscount;
                pricingDetails.NetPrice = netValue;
                pricingDetails.TotalPrice = total;
                pricingDetails.TotalTax = MWST;
            }
            //TODO: loop rules to assign pricing components
            rules = rules.Where(x => x.ConditionRecordNumber != null).ToList();
            List<PricingComponent> lstPricingComponents = new List<PricingComponent>();
            foreach (var pricing in rules)
            {
                var pricingComponent = new PricingComponent
                {
                    ConditionType = pricing.ConditionType,
                    ConditionRecordNumber = pricing.ConditionRecordNumber,
                    Rate = Convert.ToDecimal(pricing.GetType().GetProperty(pricing.ConditionType).GetValue(pricing, null)),
                    TableName = pricing.ConditionTableName,
                    VariableKey = pricing.VariableKeyValue
                };
                lstPricingComponents.Add(pricingComponent);
            }
            pricingDetails.PricingComponents = lstPricingComponents;
            var promo = ApplyPromotion(orderPricingRequest, promotions, item);
            if (promo != null)
            {
                pricingDetails.Rewards = promo.Rewards;
                pricingDetails.isFreeGoods = promo.isFreeGoods;
                pricingDetails.promotionsApplied = promo.promotionsApplied;
            }
            return pricingDetails;
        }
        public PricingDetails ApplyPromotion(OrderPricingRequest orderPricingRequest, List<Promotion> promotions, Item item)
        {
            var pricingDetail = new PricingDetails();
            RuleHandler ruleHandler = new RuleHandler();
            promotions = promotions.Where(x => x.MaterialNumber.Equals(item.ProductId)).ToList();
            pricingDetail = ruleHandler.CheckPromotionRule(orderPricingRequest, promotions, item);
            return pricingDetail;
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
