namespace GCCB_OPE_FA_API.DataManagers.Models
    {
    public class PromotionUtil
        {
        public string PromotionID { get; set; }
        public string MaterialNumber { get; set; }
        public string MaterialGroup_ID { get; set; }
        public string MaterialRewGrp {  get; set; }
        public int Quantity { get; set; }
        public decimal CashDiscount { get; set; }
        public bool IsFreeGoodQty { get; set; }
        public string PromotionType { get; set;}
        public int FreeGoodQty { get; set; }// add freegoodqty int 
        }
    }
