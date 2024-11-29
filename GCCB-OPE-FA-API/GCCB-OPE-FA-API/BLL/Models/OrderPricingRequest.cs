using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace GCCB_OPE_FA_API.BLL.Models
{
    public class OrderPricingRequest
    {
        [Required, NotNull]
        public string SoldToCustomerId { get; set; }
        [Required, NotNull]
        public string Currency { get; set; }
        [Required, NotNull]
        public string DeliveryDate { get; set; }
        [Required, NotNull]
        public List<Item> Items { get; set; }
    }
    public class Item
    {
        [Required, NotNull]
        public string ProductId { get; set; }
        public int Sequence { get; set; }
        public string UOMId { get; set; }
        [Required, NotNull]
        public int Quantity { get; set; }
        public bool isFreeGood { get; set; }
    }
}
