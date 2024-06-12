using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCCB_OPE_FA_API.BLL.Models
{
    public class CatalogueRequest
    {
        public string SoldToCustomerId { get; set; }
        public string Currency { get; set; }
        public string TransactionDate { get; set; }
        public string SourceName { get; set; }
        public string CountryCode { get; set; }
    }
}
