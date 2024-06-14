using System;
using System.Collections.Generic;

namespace GCCB_OPE_FA_API.BLL.Models
{
    public class CatalogueResponse

    {
        public int Status { get; set; }
        public string Message { get; set; }
        public Results Result { get; set; }

    }
    public class Results
    {
        public string Currency { get; set; }
        public string TransactionDate { get; set; }
        public string SourceTransactionId { get; set; }
        public List<Items> Items { get; set; }
    }

    public class Items
    {
        public string ProductId { get; set; }
        public string BasePrice { get; set; }
    }

}
