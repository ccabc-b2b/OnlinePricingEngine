using GCCB_OPE_FA_API.BLL.Models;
using GCCB_OPE_FA_API.DataManagers.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GCCB_OPE_FA_API.BLL
{
    //TODO
    public class RuleHandler
    {
        public List<PricingMatrix> CheckPricingRule(List<ConditionItems> lstConditionItems, List<PricingMatrix> lstKeyMappings)
        {
            var groupByConditionType = lstKeyMappings.GroupBy(x => x.ConditionType).ToList();
            foreach (var groupCondition in groupByConditionType)
            {
                groupCondition.OrderBy(x => x.SequenceNumber).ToList();

                var basePrices = (from keyMappings in groupCondition
                                  join conditionItems in lstConditionItems on new { Type = keyMappings.ConditionType, Variable = keyMappings.VariableKeyValue }
                                  equals new { Type = conditionItems.ConditionType, Variable = conditionItems.VariableKey }
                                  select new { keyMappings, conditionItems }).ToList();


                if (groupCondition.Key == Enumerators.ConditionTypeCode.YPR0.ToString())
                {
                    var rate = basePrices.Where(x => x.conditionItems.ConditionAmountOrPercentageRate != null
                    && x.conditionItems.ConditionAmountOrPercentageRate != "0").Select(x => new
                    {
                        x.conditionItems.ConditionAmountOrPercentageRate,
                        x.conditionItems.ConditionRecordNumber,
                        x.conditionItems.VariableKey
                    }).FirstOrDefault();
                    if (rate != null)
                    {
                        lstKeyMappings.ForEach(x => x.YPR0 = Convert.ToDecimal(rate.ConditionAmountOrPercentageRate.Replace("-", "")));
                        lstKeyMappings.Where(x => x.ConditionType.Equals(Enumerators.ConditionTypeCode.YPR0.ToString()) && x.VariableKeyValue.Equals(rate.VariableKey)).ToList().ForEach(x => x.ConditionRecordNumber = rate.ConditionRecordNumber);
                    }
                }
                if (groupCondition.Key == Enumerators.ConditionTypeCode.YBAJ.ToString())
                {
                    var rate = basePrices.Where(x => x.conditionItems.ConditionAmountOrPercentageRate != null
                    && x.conditionItems.ConditionAmountOrPercentageRate != "0").Select(x => new
                    {
                        x.conditionItems.ConditionAmountOrPercentageRate,
                        x.conditionItems.ConditionRecordNumber,
                        x.conditionItems.VariableKey
                    }).FirstOrDefault();
                    if (rate != null)
                    {
                        lstKeyMappings.ForEach(x => x.YBAJ = Convert.ToDecimal(rate.ConditionAmountOrPercentageRate.Replace("-", "")));
                        lstKeyMappings.Where(x => x.ConditionType.Equals(Enumerators.ConditionTypeCode.YBAJ.ToString()) && x.VariableKeyValue.Equals(rate.VariableKey)).ToList().ForEach(x => x.ConditionRecordNumber = rate.ConditionRecordNumber);
                    }
                }
                if (groupCondition.Key == Enumerators.ConditionTypeCode.YBUY.ToString())
                {
                    var rate = basePrices.Where(x => x.conditionItems.ConditionAmountOrPercentageRate != null
                    && x.conditionItems.ConditionAmountOrPercentageRate != "0").Select(x => new
                    {
                        x.conditionItems.ConditionAmountOrPercentageRate,
                        x.conditionItems.ConditionRecordNumber,
                        x.conditionItems.VariableKey
                    }).FirstOrDefault();
                    if (rate != null)
                    {
                        lstKeyMappings.ForEach(x => x.YBUY = Convert.ToDecimal(rate.ConditionAmountOrPercentageRate.Replace("-", "")));
                        lstKeyMappings.Where(x => x.ConditionType.Equals(Enumerators.ConditionTypeCode.YBUY.ToString()) && x.VariableKeyValue.Equals(rate.VariableKey)).ToList().ForEach(x => x.ConditionRecordNumber = rate.ConditionRecordNumber);
                    }
                }
                if (groupCondition.Key == Enumerators.ConditionTypeCode.YTDN.ToString())
                {
                    var rate = basePrices.Where(x => x.conditionItems.ConditionAmountOrPercentageRate != null
                    && x.conditionItems.ConditionAmountOrPercentageRate != "0").Select(x => new
                    {
                        x.conditionItems.ConditionAmountOrPercentageRate,
                        x.conditionItems.ConditionRecordNumber,
                        x.conditionItems.VariableKey
                    }).FirstOrDefault();
                    if (rate != null)
                    {
                        lstKeyMappings.ForEach(x => x.YTDN = Convert.ToDecimal(rate.ConditionAmountOrPercentageRate.Replace("-", "")));
                        lstKeyMappings.Where(x => x.ConditionType.Equals(Enumerators.ConditionTypeCode.YTDN.ToString()) && x.VariableKeyValue.Equals(rate.VariableKey)).ToList().ForEach(x => x.ConditionRecordNumber = rate.ConditionRecordNumber);
                    }
                }
                if (groupCondition.Key == Enumerators.ConditionTypeCode.YRPO.ToString())
                {
                    var rate = basePrices.Where(x => x.conditionItems.ConditionAmountOrPercentageRate != null
                    && x.conditionItems.ConditionAmountOrPercentageRate != "0").Select(x => new
                    {
                        x.conditionItems.ConditionAmountOrPercentageRate,
                        x.conditionItems.ConditionRecordNumber,
                        x.conditionItems.VariableKey
                    }).FirstOrDefault();
                    if (rate != null)
                    {
                        lstKeyMappings.ForEach(x => x.YRPO = Convert.ToDecimal(rate.ConditionAmountOrPercentageRate.Replace("-", "")));
                        lstKeyMappings.Where(x => x.ConditionType.Equals(Enumerators.ConditionTypeCode.YRPO.ToString()) && x.VariableKeyValue.Equals(rate.VariableKey)).ToList().ForEach(x => x.ConditionRecordNumber = rate.ConditionRecordNumber);
                    }
                }
                if (groupCondition.Key == Enumerators.ConditionTypeCode.YELP.ToString())
                {
                    var rate = basePrices.Where(x => x.conditionItems.ConditionAmountOrPercentageRate != null
                    && x.conditionItems.ConditionAmountOrPercentageRate != "0").Select(x => new
                    {
                        x.conditionItems.ConditionAmountOrPercentageRate,
                        x.conditionItems.ConditionRecordNumber,
                        x.conditionItems.VariableKey
                    }).FirstOrDefault();
                    if (rate != null)
                    {
                        lstKeyMappings.ForEach(x => x.YELP = Convert.ToDecimal(rate.ConditionAmountOrPercentageRate.Replace("-", "")));
                        lstKeyMappings.Where(x => x.ConditionType.Equals(Enumerators.ConditionTypeCode.YELP.ToString()) && x.VariableKeyValue.Equals(rate.VariableKey)).ToList().ForEach(x => x.ConditionRecordNumber = rate.ConditionRecordNumber);
                    }
                }
                if (groupCondition.Key == Enumerators.ConditionTypeCode.YPDN.ToString())
                {
                    var rate = basePrices.Where(x => x.conditionItems.ConditionAmountOrPercentageRate != null
                    && x.conditionItems.ConditionAmountOrPercentageRate != "0").Select(x => new
                    {
                        x.conditionItems.ConditionAmountOrPercentageRate,
                        x.conditionItems.ConditionRecordNumber,
                        x.conditionItems.VariableKey
                    }).FirstOrDefault();
                    if (rate != null)
                    {
                        lstKeyMappings.ForEach(x => x.YPDN = Convert.ToDecimal(rate.ConditionAmountOrPercentageRate.Replace("-", "")));
                        lstKeyMappings.Where(x => x.ConditionType.Equals(Enumerators.ConditionTypeCode.YPDN.ToString()) && x.VariableKeyValue.Equals(rate.VariableKey)).ToList().ForEach(x => x.ConditionRecordNumber = rate.ConditionRecordNumber);
                    }
                }
                if (groupCondition.Key == Enumerators.ConditionTypeCode.YPN2.ToString())
                {
                    var rate = basePrices.Where(x => x.conditionItems.ConditionAmountOrPercentageRate != null
                    && x.conditionItems.ConditionAmountOrPercentageRate != "0").Select(x => new
                    {
                        x.conditionItems.ConditionAmountOrPercentageRate,
                        x.conditionItems.ConditionRecordNumber,
                        x.conditionItems.VariableKey
                    }).FirstOrDefault();
                    if (rate != null)
                    {
                        lstKeyMappings.ForEach(x => x.YPN2 = Convert.ToDecimal(rate.ConditionAmountOrPercentageRate.Replace("-", "")));
                        lstKeyMappings.Where(x => x.ConditionType.Equals(Enumerators.ConditionTypeCode.YPN2.ToString()) && x.VariableKeyValue.Equals(rate.VariableKey)).ToList().ForEach(x => x.ConditionRecordNumber = rate.ConditionRecordNumber);
                    }
                }
            }
            return lstKeyMappings;
        }
        public List<PricingMatrix> CheckConditionTableRule(Material material, List<PricingMatrix> lstPricingMatrix)
        {
            foreach (var condition in lstPricingMatrix)
            {
                string key = $"{condition.ConditionType}+{condition.ConditionTableName}";
                if (Constants.ConditionTableRule.ContainsKey(key) && !Constants.ConditionTableRule[key](material.MaterialGroup.ToString()))
                {
                    lstPricingMatrix.Remove(condition);
                }
            }
            return lstPricingMatrix;
        }
        public PricingDetails CheckPromotionRule(OrderPricingRequest orderPricingRequest, List<Promotion> promotions, Item item)
        {
            DateTime deliveryDate = Convert.ToDateTime(orderPricingRequest.DeliveryDate);
            List<Promotion> filteredPromotion = new List<Promotion>();
            var pricingDetails = new PricingDetails();
            if (promotions.Count > 0)
            {
                int val1;
                int val2;
                int val3;
                filteredPromotion = promotions.Where(x =>
                    (x.MinQty != null && x.MinQty != Constants.DefaultQuantity && x.MaxQty != null && x.MaxQty != Constants.DefaultQuantity ||
                    x.AgreementValidFromDate.HasValue && x.AgreementValidToDate.HasValue &&
                    item.Quantity >= (int.TryParse(x.MinQty.Trim(), out val1) ? val1 : 0) && item.Quantity <= (int.TryParse(x.MaxQty.Trim(), out val2) ? val2 : 0) || // check promotion slab
                    deliveryDate >= x.AgreementValidFromDate && deliveryDate <= x.AgreementValidToDate) ||
                    (x.MaxQty == null || x.MaxQty == Constants.DefaultQuantity) && item.Quantity > (int.TryParse(x.MinQty.Trim(), out val3) ? val3 : 0) // new condition
                ).ToList();

                ////Filter promotions with delivery date
                //promotions = promotions.Where(x => x.AgreementValidFromDate.HasValue
                //&& x.AgreementValidToDate.HasValue && deliveryDate >= x.AgreementValidFromDate
                //&& deliveryDate <= x.AgreementValidToDate).ToList();

                ////Check promtion slab
                //promotions.OrderByDescending(x => x.MaxQty);
                //promotions.Where(x => x.MinQty != null && x.MinQty != Constants.DefaultQuantity
                //&& x.MaxQty != null && x.MaxQty != Constants.DefaultQuantity &&
                //item.Quantity >= Convert.ToInt32(x.MinQty) && item.Quantity <= Convert.ToInt32(x.MaxQty)).ToList();

                }
            List<string> appliedPromotions = filteredPromotion?.Select(x => x.PromotionID).ToList();
            pricingDetails.promotionsApplied = appliedPromotions;
            if (filteredPromotion.Count > 0)
                pricingDetails.Rewards = filteredPromotion.Select(x => string.IsNullOrEmpty(x.RewardValue) ? "0" : x.RewardValue)
                    .Select(decimal.Parse).Max();
            pricingDetails.isFreeGoods = filteredPromotion.Any(x => x.PromotionType.Equals(((int)Enumerators.PromotionType.FOC).ToString()));
            return pricingDetails;
        }
    }
}
