using GCCB_OPE_FA_API.BLL.Models;
using GCCB_OPE_FA_API.DataManagers.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GCCB_OPE_FA_API.BLL
{
    //TODO
    public class BasicRuleHandler
    {
        public List<PricingMatrix> CheckPricingRule(List<ConditionItems> lstConditionItems, List<PricingMatrix> lstKeyMappings)
        {
            var groupByConditionType = lstKeyMappings.GroupBy(x => x.ConditionType).ToList();
            foreach (var groupCondition in groupByConditionType)
            {
                groupCondition.OrderBy(x => x.SequenceNumber).ToList();

                var basePrices = (from keyMappings in groupCondition
                                  join conditionItems in lstConditionItems on new { JobID = keyMappings.ConditionType, prodCode = keyMappings.VariableKeyValue }
                                  equals new { JobID = conditionItems.ConditionType, prodCode = conditionItems.VariableKey }
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
                        lstKeyMappings.ForEach(x => x.YPR0 = Convert.ToDecimal(rate.ConditionAmountOrPercentageRate));
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
                        lstKeyMappings.ForEach(x => x.YBAJ = Convert.ToDecimal(rate.ConditionAmountOrPercentageRate));
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
                        lstKeyMappings.ForEach(x => x.YBUY = Convert.ToDecimal(rate.ConditionAmountOrPercentageRate));
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
                        lstKeyMappings.ForEach(x => x.YTDN = Convert.ToDecimal(rate.ConditionAmountOrPercentageRate));
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
                        lstKeyMappings.ForEach(x => x.YRPO = Convert.ToDecimal(rate.ConditionAmountOrPercentageRate));
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
                        lstKeyMappings.ForEach(x => x.YELP = Convert.ToDecimal(rate.ConditionAmountOrPercentageRate));
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
                        lstKeyMappings.ForEach(x => x.YPDN = Convert.ToDecimal(rate.ConditionAmountOrPercentageRate));
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
                        lstKeyMappings.ForEach(x => x.YPN2 = Convert.ToDecimal(rate.ConditionAmountOrPercentageRate));
                        lstKeyMappings.Where(x => x.ConditionType.Equals(Enumerators.ConditionTypeCode.YPN2.ToString()) && x.VariableKeyValue.Equals(rate.VariableKey)).ToList().ForEach(x => x.ConditionRecordNumber = rate.ConditionRecordNumber);
                    }
                }
            }
            return lstKeyMappings;
        }
        public PricingDetails CheckPromotionRule(OrderPricingRequest orderPricingRequest, List<Promotion> promotions, Item item)
        {
            List<Promotion> lstAppliedPromotion = new List<Promotion>();
            //TODO
            if (promotions != null)
            {
                //promotions = promotions.ForEach(x => x.AgreementValidFromDate)
                foreach(var promotion in promotions)
                {
                    if (promotion.AgreementValidFromDate != null && promotion.AgreementValidToDate != null)
                    {

                    }
                }

            }
            return null;
        }
    }
}
