using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCCB_OPE_FA_API.BLL.Models
{
    public class PricingCache
    {
        public string Rate { get; set; }    
        public string ConditionTableName { get; set; }
        public DateTime ValidFromDate { get; set; }
        public DateTime ValidToDate { get; set; }
    }
}
