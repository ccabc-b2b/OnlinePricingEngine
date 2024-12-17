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
    public class OrderPricing
        {
        private readonly ILogger _logger;
        private readonly ConnectionManager _connectionManager;
        public OrderPricingRequest _orderpricingRequest;
        private readonly Connection _connection;
        public OrderPricing(ILogger<OrderPricing> logger, ConnectionManager connectionManager, Connection connection)
            {
            _logger = logger;
            _connectionManager = connectionManager;
            _connection = connection;
            }
        public OrderPricingResponse ProcessOrderPricing(OrderPricingRequest orderPricingRequest)
            {
            //var key = $"{CountryCode}_{ConditionType}_{variablekey}"; 
            _logger.LogInformation("Process order pricing");

            _orderpricingRequest = orderPricingRequest;

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

            var materialGroupParameter = new SqlParameter[]
            {
                new SqlParameter("@MaterialNumber",string.Join(",", orderPricingRequest.Items.Select(x => x.ProductId).ToList()))
            };
            var materialGrps = Util.DataTabletoList<MaterialGroups>(_connectionManager.ExecuteStoredProcedure("MaterialGroupFetch", materialGroupParameter));

            var filteredMaterialGrps = materialGrps
             .Where(mg => !string.IsNullOrEmpty(mg.MaterialGroup))
             .ToList();

            // Step 2: Update the Quantity from the Item object
            var updatedMaterialGrps = filteredMaterialGrps
                .Select(mg =>
                {
                    var item = orderPricingRequest.Items.FirstOrDefault(i => i.ProductId == mg.MaterialNumber);
                    if (item != null)
                        {
                        mg.Quantity = item.Quantity;
                        }
                    return mg;
                })
                .ToList();

            var response = new OrderPricingResponse();

            response.Status = (int)HttpStatusCode.OK;
            response.Message = Constants.SuccessMessage;
            var result = new Result();
            result.SyncDate = DateTime.Now;//Todo        
            result.PricingDetails = CalculatePricePromo(orderPricingRequest, customer, materials, updatedMaterialGrps);
            result.SubTotalPrice = result.PricingDetails.Sum(x => x.SubTotalPrice * x.quantity);
            result.Rewards = result.PricingDetails.Sum(x => x.Rewards * (x.freegoodqty > 0 ? x.freegoodqty : x.quantity));
            result.Discount = result.PricingDetails.Sum(x => x.Discount * (x.freegoodqty > 0 ? x.freegoodqty : x.quantity));
            result.NetPrice = result.PricingDetails.Sum(x => x.NetPrice * x.quantity);
            result.TotalPrice = result.PricingDetails.Sum(x => x.TotalPrice * x.quantity);
            result.TotalTax = result.PricingDetails.Sum(x => x.TotalTax * x.quantity);
            result.SalesOrg = customer.SalesOrg;
            result.SalesRoute = customer.SalesRoute;
            response.Result = result;

            return response;
            }

        public List<PricingDetails> CalculatePricePromo(OrderPricingRequest orderPricingRequest, Customer customer, List<Material> Materials, List<MaterialGroups> materialGroups)
            {
            List<PricingDetails> lstPricingDetails = new List<PricingDetails>();
            List<PromotionUtil> promotionsApplied = new List<PromotionUtil>();

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
                    var pricingDetails = CalculatePricing(orderPricingRequest, item, customer, conditionItems, keyMappings, promotions, materialGroups);
                    lstPricingDetails.Add(pricingDetails);
                    }
                }


            if (materialGroups.Where(m => m.GroupType == "REQ").Count() > 0)
                {
                var focpromoappliedgrouplevel = ApplyFOCPromotiontMaterialGroupLevel(orderPricingRequest, materialGroups, lstPricingDetails);
                promotionsApplied.AddRange(focpromoappliedgrouplevel);

                var filteredmaterialgroups = from materialgroup in materialGroups
                                             join orderpricing1 in _orderpricingRequest.Items
                                             on materialgroup.MaterialNumber equals orderpricing1.ProductId
                                             where orderpricing1.isFreeGood == false
                                             select materialgroup;

                List<MaterialGroups> materialgrps = filteredmaterialgroups.ToList<MaterialGroups>();

                if (materialgrps != null && materialgrps.Count() > 0)
                    {
                    var promoappliedgrouplevel = ApplyPromotiontMaterialGroupLevel(_orderpricingRequest, materialgrps, lstPricingDetails);
                    promotionsApplied.AddRange(promoappliedgrouplevel);
                    }
                }

            foreach (var item in _orderpricingRequest.Items.Where(x => x.isFreeGood == false).ToList())
                {
                var baseprice = lstPricingDetails.Where(x => x.product.ToString() == item.ProductId).Select(x => x.SubTotalPrice).FirstOrDefault();
                var promoapplieditemlevel = ApplyPromotiontItemLevel(_orderpricingRequest, promotions, item, baseprice);
                promotionsApplied.AddRange(promoapplieditemlevel);
                }

            var bestRewards = promotionsApplied
           .GroupBy(p => new { p.MaterialNumber, p.PromotionType })
           .Select(g => g.OrderByDescending(p => p.CashDiscount).First()).ToList();

            var totalRewards = bestRewards
            .GroupBy(p => p.MaterialNumber)
            .Select(g => new
                {
                MaterialNumber = g.Key,
                FreeGoodQty = g.Max(p => p.FreeGoodQty),
                TotalRewardValue = g.Sum(p => p.CashDiscount),
                PromotionIds = g.Select(p => p.PromotionID).ToList()
                })
            .ToList();

            foreach (var item in lstPricingDetails)
                {
                foreach (var reward in totalRewards)
                    {
                    if (item.product.ToString() == reward.MaterialNumber)
                        {
                        item.Rewards = Convert.ToDecimal(reward.TotalRewardValue);
                        item.freegoodqty = reward.FreeGoodQty;
                        item.promotionsApplied = reward.PromotionIds;
                        item.Discount += item.Rewards;
                        if (item.freegoodqty == 0)
                            {
                            item.NetPrice -= item.Rewards;
                            }
                        item.TotalPrice = item.NetPrice;
                        var vat = (customer.TaxClassification.Equals("1") ? 5 * item.TotalPrice / 100 : 0);
                        item.TotalTax = vat;
                        item.TotalPrice += item.TotalTax;
                        item.isFreeGoods = (item.TotalPrice) == 0 ? true : false;
                        item.quantity -= reward.FreeGoodQty;
                        }
                    }

                }
            return lstPricingDetails;
            }
        public PricingDetails CalculatePricing(OrderPricingRequest orderPricingRequest, Item item, Customer customer, List<ConditionItems> conditionItems, List<PricingMatrix> keyMappings, List<Promotion> promotions, List<MaterialGroups> materialGroups)
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
            return pricingDetails;
            }
        public List<PromotionUtil> ApplyPromotiontItemLevel(OrderPricingRequest orderPricingRequest, List<Promotion> promotions, Item item, decimal baseprice)
            {
            var filteredpromotions = new List<Promotion>();
            var promotionsapplied = new List<PromotionUtil>();
            RuleHandler ruleHandler = new RuleHandler();
            promotions = promotions.Where(x => x.MaterialNumber.Equals(item.ProductId)).ToList();
            filteredpromotions = ruleHandler.CheckPromotionRuleAtItemLevel(orderPricingRequest, promotions, item.Quantity);
            promotionsapplied = filteredpromotions
                             .Where(p => string.IsNullOrEmpty(p.RewardQty) || p.RewardQty == Constants.DefaultQuantity)
                             .GroupBy(p => p.PromotionType)
                             .Select(g =>
                             {
                                 var maxRewardPromotion = g.OrderByDescending(p => float.Parse(p.RewardValue)).First();
                                 var cashDiscount = !string.IsNullOrEmpty(maxRewardPromotion.RewardPercentage) && maxRewardPromotion.RewardPercentage != Constants.DefaultRewardPercentage
                                                    ? Math.Round(decimal.Parse(maxRewardPromotion.RewardPercentage) * baseprice / 100, 2)
                                                    : decimal.Parse(maxRewardPromotion.RewardValue);
                                 return new PromotionUtil
                                     {
                                     PromotionID = maxRewardPromotion.PromotionID,
                                     MaterialNumber = maxRewardPromotion.MaterialNumber,
                                     MaterialGroup_ID = maxRewardPromotion.RewardMaterialGroupID,
                                     Quantity = item.Quantity,
                                     CashDiscount = cashDiscount,
                                     PromotionType = maxRewardPromotion.PromotionType,
                                     };
                             }
                             ).ToList();

            return promotionsapplied;
            }
        public List<PromotionUtil> ApplyPromotiontMaterialGroupLevel(OrderPricingRequest orderPricingRequest, List<MaterialGroups> materialGroups, List<PricingDetails> lstPricingDetails)
            {
            var promotionsapplied = new List<PromotionUtil>();
            RuleHandler ruleHandler = new RuleHandler();

            var parameter = new SqlParameter[]
            {
                new SqlParameter("@CustomerNumber",orderPricingRequest.SoldToCustomerId),
                new SqlParameter("@MaterialGroup",string.Join(",", materialGroups.Where(x=>x.GroupType=="REQ").Select(x => x.MaterialGroup).ToList()))
            };
            var promotions = Util.DataTabletoList<Promotion>(_connectionManager.ExecuteStoredProcedure("ItemPromotionGroup", parameter));

            var materialQuantityList = new List<(string ProductId, int Quantity)>();

            foreach (var item in orderPricingRequest.Items.Where(x => x.isFreeGood == false).ToList())
                {
                materialQuantityList.Add((item.ProductId, item.Quantity));
                }

            var materialPricingList = new List<(string ProductId, decimal Pricing)>();

            foreach (var pricingDetails in lstPricingDetails)
                {
                materialPricingList.Add((pricingDetails.product.ToString(), pricingDetails.SubTotalPrice));
                }

            var material_REQ_Group = materialGroups
                                .Where(m => m.GroupType == "REQ")
                                .GroupBy(m => m.MaterialGroup)
                                .Select(g => new
                                    {
                                    Group = g.Key,
                                    GroupType = g.Select(m => m.GroupType).Distinct().ToList(),
                                    Materials = g.Select(m => m.MaterialNumber).ToList(),
                                    TotalQuantity = g.Sum(m => m.Quantity)
                                    })
                                .ToList();

            var material_RWD_Group = materialGroups
                                    .Where(m => m.GroupType == "REW")
                                    .GroupBy(m => m.MaterialGroup)
                                    .Select(g => new
                                        {
                                        Group = g.Key,
                                        Materials = g.Select(m => m.MaterialNumber).ToList(),
                                        })
                                    .ToList();

            foreach (var materialgroup in material_REQ_Group)
                {
                List<Promotion> applicablePromotions = new List<Promotion>();
                if (materialgroup.Materials.Count() >= 1)
                    {
                    applicablePromotions = promotions
                   .Where(p => p.RequirementMaterialGroupID == materialgroup.Group)
                   .ToList();
                    }

                var filteredpromotions = ruleHandler.CheckPromotionRuleAtGroupLevel(orderPricingRequest, applicablePromotions, materialgroup.TotalQuantity);


                foreach (var material in materialgroup.Materials)
                    {
                    foreach (var rewgrp in material_RWD_Group)
                        {
                        if (rewgrp.Materials.Contains(material))
                            {
                            foreach (var promotion in filteredpromotions)
                                {
                                if (promotion.RewardMaterialGroupID == rewgrp.Group)
                                    {
                                    if (!string.IsNullOrEmpty(promotion.RewardPercentage) && promotion.RewardPercentage != Constants.DefaultRewardPercentage)
                                        {
                                        var promotionapplied = new PromotionUtil
                                            {
                                            PromotionID = promotion.PromotionID,
                                            MaterialNumber = material,
                                            MaterialGroup_ID = promotion.RequirementMaterialGroupID,
                                            MaterialRewGrp = promotion.RewardMaterialGroupID,
                                            Quantity = materialQuantityList.Where(m => m.ProductId == material).Select(m => m.Quantity).FirstOrDefault(),
                                            CashDiscount = decimal.Parse(promotion.RewardPercentage) * (materialPricingList.Where(x => x.ProductId == material).Select(x => x.Pricing).FirstOrDefault()) / 100,//decimal.Parse(promotion.RewardValue),
                                            PromotionType = promotion.PromotionType,
                                            };
                                        promotionsapplied.Add(promotionapplied);
                                        }
                                    else if (!string.IsNullOrEmpty(promotion.RewardValue) && promotion.RewardValue != Constants.DefaultRewardValue)
                                        {
                                        var promotionapplied = new PromotionUtil
                                            {
                                            PromotionID = promotion.PromotionID,
                                            MaterialNumber = material,
                                            MaterialGroup_ID = promotion.RequirementMaterialGroupID,
                                            MaterialRewGrp = promotion.RewardMaterialGroupID,
                                            Quantity = materialQuantityList.Where(m => m.ProductId == material).Select(m => m.Quantity).FirstOrDefault(),
                                            CashDiscount = decimal.Parse(promotion.RewardValue),
                                            PromotionType = promotion.PromotionType,
                                            };
                                        promotionsapplied.Add(promotionapplied);
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
            return promotionsapplied;
            }

        public List<PromotionUtil> ApplyFOCPromotiontMaterialGroupLevel(OrderPricingRequest orderPricingRequest, List<MaterialGroups> materialGroups, List<PricingDetails> lstPricingDetails)
            {
            var promotionsapplied = new List<PromotionUtil>();
            RuleHandler ruleHandler = new RuleHandler();

            var parameter = new SqlParameter[]
            {
                new SqlParameter("@CustomerNumber",orderPricingRequest.SoldToCustomerId),
                new SqlParameter("@MaterialGroup",string.Join(",", materialGroups.Where(x=>x.GroupType=="REQ").Select(x => x.MaterialGroup).ToList()))
            };
            var promotions = Util.DataTabletoList<Promotion>(_connectionManager.ExecuteStoredProcedure("ItemPromotionGroup", parameter));

            var materialQuantityList = new List<(string ProductId, int Quantity)>();

            foreach (var item in orderPricingRequest.Items)
                {
                materialQuantityList.Add((item.ProductId, item.Quantity));
                }

            var materialPricingList = new List<(string ProductId, decimal Pricing)>();

            foreach (var pricingDetails in lstPricingDetails)
                {
                materialPricingList.Add((pricingDetails.product.ToString(), pricingDetails.SubTotalPrice));
                }

            var material_REQ_Group = materialGroups
                                .Where(m => m.GroupType == "REQ")
                                .GroupBy(m => m.MaterialGroup)
                                .Select(g => new
                                    {
                                    Group = g.Key,
                                    GroupType = g.Select(m => m.GroupType).Distinct().ToList(),
                                    Materials = g.Select(m => m.MaterialNumber).ToList(),
                                    TotalQuantity = g.Sum(m => m.Quantity)
                                    })
                                .ToList();

            var material_RWD_Group = materialGroups
                                    .Where(m => m.GroupType == "REW")
                                    .GroupBy(m => m.MaterialGroup)
                                    .Select(g => new
                                        {
                                        Group = g.Key,
                                        Materials = g.Select(m => m.MaterialNumber).ToList(),
                                        })
                                    .ToList();

            foreach (var materialgroup in material_REQ_Group)
                {
                List<Promotion> applicablePromotions = new List<Promotion>();
                if (materialgroup.Materials.Count() >= 1)
                    {
                    applicablePromotions = promotions
                   .Where(p => p.RequirementMaterialGroupID == materialgroup.Group)
                   .ToList();
                    }

                var filteredpromotions = ruleHandler.CheckFOCPromotionRuleAtGroupLevel(orderPricingRequest, applicablePromotions, materialgroup.TotalQuantity);



                foreach (var promotion in filteredpromotions)
                    {
                    if (material_RWD_Group.Select(x => x.Group).Contains(promotion.RewardMaterialGroupID))
                        {
                        var filteredMaterials = material_RWD_Group
                            .Where(x => x.Group == promotion.RewardMaterialGroupID)
                            .SelectMany(x => x.Materials)
                            .ToList();

                        // Find common product IDs between materialQuantityList and filteredMaterials
                        var commonProductsWithQuantity = materialQuantityList
                            .Where(m => filteredMaterials.Contains(m.ProductId))
                            .Select(m => new { m.ProductId, m.Quantity })
                            .ToList();

                        var totalQuantity = commonProductsWithQuantity.Sum(m => m.Quantity);

                        decimal cashdiscount = 0;
                        bool freegoodqty = false;
                        if (filteredMaterials[0] == materialgroup.Materials[0])//reward material==req material grp
                            {
                            foreach (var material in commonProductsWithQuantity.Select(x => x.ProductId).ToList())
                                {
                                if (decimal.Parse(promotion.FromQTY) + decimal.Parse(promotion.FreeGoodQTY) == decimal.Parse(totalQuantity.ToString()))//freegoodqty+from qty ==total quantity
                                    {
                                    var quantity = Convert.ToInt32(decimal.Parse(promotion.FreeGoodQTY));
                                    cashdiscount = materialPricingList.Where(p => p.ProductId == material).Select(p => p.Pricing).FirstOrDefault();
                                    freegoodqty = true;

                                    var promotionapplied = new PromotionUtil
                                        {
                                        PromotionID = promotion.PromotionID,
                                        MaterialNumber = material,
                                        MaterialGroup_ID = promotion.RequirementMaterialGroupID,
                                        MaterialRewGrp = promotion.RewardMaterialGroupID,
                                        Quantity = totalQuantity - quantity,
                                        CashDiscount = cashdiscount,
                                        IsFreeGoodQty = freegoodqty,
                                        PromotionType = promotion.PromotionType,
                                        FreeGoodQty = quantity
                                        };
                                    promotionsapplied.Add(promotionapplied);



                                    foreach (var item in materialgroup.Materials)
                                        {
                                        foreach (var orderitem in _orderpricingRequest.Items)
                                            {
                                            if (item == orderitem.ProductId)
                                                {
                                                orderitem.isFreeGood = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        else if ((!string.IsNullOrEmpty(promotion.FreeGoodQTY) || promotion.FreeGoodQTY != Constants.DefaultQuantity) && decimal.Parse(promotion.FreeGoodQTY) == decimal.Parse(totalQuantity.ToString()))
                            {
                            foreach (var material in commonProductsWithQuantity.Select(x => x.ProductId).ToList())
                                {
                                var quantity = materialQuantityList.Where(m => m.ProductId == material).Select(m => m.Quantity).FirstOrDefault();
                                cashdiscount = materialPricingList.Where(p => p.ProductId == material).Select(p => p.Pricing).FirstOrDefault();
                                freegoodqty = true;
                                var promotionapplied = new PromotionUtil
                                    {
                                    PromotionID = promotion.PromotionID,
                                    MaterialNumber = material,
                                    MaterialGroup_ID = promotion.RequirementMaterialGroupID,
                                    MaterialRewGrp = promotion.RewardMaterialGroupID,
                                    Quantity = quantity,
                                    CashDiscount = cashdiscount,
                                    IsFreeGoodQty = freegoodqty,
                                    PromotionType = promotion.PromotionType,
                                    };
                                promotionsapplied.Add(promotionapplied);

                                foreach (var item in _orderpricingRequest.Items)
                                    {
                                    if (item.ProductId == material)
                                        {
                                        item.isFreeGood = true;
                                        }
                                    }
                                }
                            foreach (var item in materialgroup.Materials)
                                {
                                foreach (var orderitem in _orderpricingRequest.Items)
                                    {
                                    if (item == orderitem.ProductId)
                                        {
                                        orderitem.isFreeGood = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            return promotionsapplied;
            }

        private List<PricingMatrix> VariableKeyMapping(Customer customer, Material material, string country)
            {
            try
                {
                var parameter = new SqlParameter[] {
                new SqlParameter("@Country",country)
            };
                List<PricingMatrix> lstPricingMatrix = Util.DataTabletoList<PricingMatrix>(_connectionManager.ExecuteStoredProcedure("PricingMatrixFetch", parameter));
                throw new NullReferenceException();
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
            catch(Exception ex)
                {
                return null;
                }
            }
        public Task WarmUp()
            {
            return TestSqlConnectionAsync(_connection.ConnectionString);
            }

        private async Task<bool> TestSqlConnectionAsync(string connectionString)
            {
            try
                {
                using (var connection = new SqlConnection(connectionString))
                    {
                    await connection.OpenAsync();
                    // Connection successful
                    return true;
                    }
                }
            catch (Exception ex)
                {
                // Handle or log the exception
                return false;
                }
            }
        }
    }
