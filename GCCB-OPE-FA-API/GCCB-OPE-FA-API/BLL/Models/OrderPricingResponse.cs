using System;
using System.Collections.Generic;

namespace GCCB_OPE_FA_API.BLL.Models
{
    public class OrderPricingResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public Result Result { get; set; }

    }
    public class Result
    {
        public DateTime SyncDate { get; set; }
        public List<PricingDetails> PricingDetails { get; set; }
        public decimal SubTotalPrice { get; set; }
        public decimal Rewards { get; set; }
        public decimal Discount { get; set; }
        public decimal NetPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalTax { get; set; }
        public string SalesRoute { get; set; }
        public string SalesOrg { get; set; }
    }
    public class PricingDetails
    {
        public bool isFreeGoods { get; set; }
        public int product { get; set; }
        public int quantity { get; set; }
        public decimal SubTotalPrice { get; set; }
        public decimal Rewards { get; set; }
        public decimal Discount { get; set; }
        public decimal NetPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalTax { get; set; }
        public int freegoodqty { get; set; }
        public List<string> promotionsApplied { get; set; }
        public List<PricingComponent> PricingComponents { get; set; }

    }
    public class PricingComponent
    {
        public string ConditionType { get; set; }
        public decimal Rate { get; set; }
        public string ConditionRecordNumber { get; set; }
        public string TableName { get; set; }
        public string VariableKey { get; set; }
    }
    //public class CalculatedResults
    //{
    //    public string SubTotalPrice { get; set; }
    //    public string SubToRewardstalPrice { get; set; }
    //    public string Discount { get; set; }
    //    public string NetPrice { get; set; }
    //    public string TotalPrice { get; set; }
    //    public string TotalTax { get; set; }
    //    public string SalesRoute { get; set; }
    //    public string SalesOrg { get; set; }

    //}

}
