using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCCB_OPE_FA_API.BLL.Models
{
    public class OrderPricingRequest
    {
        public string SoldToCustomerId { get; set; }
        public string Currency { get; set; }
        public string DeliveryDate { get; set; }
        public int ApplyPromotion { get; set; }
        public bool TaxExempt { get; set; }
        public List<Item> Items { get; set; }
    }
    public class Item
    {
        public string ProductId { get; set; }
        public int Sequence { get; set; }
        public string UOMId { get; set; }
        public int Quantity { get; set; }
        public List<string> Promotions { get; set; }
    }
}
