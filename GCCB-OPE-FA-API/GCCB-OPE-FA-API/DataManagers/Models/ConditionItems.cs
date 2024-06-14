using System;

namespace GCCB_OPE_FA_API.DataManagers.Models
{
    public class ConditionItems
    {
        public string ConditionRecordNumber { get; set; }
        public string ConditionType { get; set; }
        public string VariableKey { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public string ConditionAmountOrPercentageRate { get; set; }
        public string CurrencyOrPercentageRateUnit { get; set; }
        public string ConditionPricingUnit { get; set; }
        public string ConditionUnit { get; set; }
        public string ConditionAmountLowerLimit { get; set; }
        public string ConditionAmountUpperLimit { get; set; }
        public string AccrualAmount { get; set; }
    }
}
