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
            List<Promotion> PromotionType1 = new List<Promotion>();
            List<Promotion> PromotionType2 = new List<Promotion>();
            List<Promotion> PromotionType3 = new List<Promotion>();
            var pricingDetails = new PricingDetails();
            if (promotions.Count > 0)
                {
                float val1;
                float val2;
                int val3;

                filteredPromotion = promotions.Where(x =>
                    ((x.MinQty != null && x.MinQty != Constants.DefaultQuantity) &&
                    (x.AgreementValidFromDate.HasValue && x.AgreementValidToDate.HasValue) &&
                    item.Quantity >= (float.TryParse(x.MinQty.Trim(), out val1) ? val1 : 0) &&
                    item.Quantity <= (float.Parse(((x.MaxQty == null || x.MaxQty.Trim().Equals("") || x.MaxQty == Constants.DefaultQuantity) ? int.MaxValue.ToString() : x.MaxQty).Trim())) &&   // check promotion slab
                    deliveryDate >= x.AgreementValidFromDate && deliveryDate <= x.AgreementValidToDate) //||
                                                                                                        //(x.MaxQty == null || x.MaxQty == Constants.DefaultQuantity) && item.Quantity > (int.TryParse(x.MinQty.Trim(), out val3) ? val3 : 0) // new condition
                ).ToList();
                }

                float rewardValue = filteredPromotion.GroupBy(p => p.PromotionType).Select(p => new { PromotionType = p.Key, reward = p.Max(q => float.Parse(q.RewardValue)) }).ToList().Sum(T => T.reward);

                pricingDetails.Rewards = Convert.ToDecimal(rewardValue);
                

                pricingDetails.isFreeGoods = filteredPromotion.Any(x => x.PromotionType.Equals(((int)Enumerators.PromotionType.FOC).ToString()));
                
                
                
            return pricingDetails;

            }
    }
}
