using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCCB_OPE_FA_API.DataManagers.Models
{
    public class PricingMatrix
    {
        public int SequenceNumber { get; set; }
        public string Country { get; set;}
        public string ConditionType { get; set;}
        public string VariableKeyFetch { get; set;}
        public string ConditionTableName { get; set;}
        public string VariableKeyValue { get; set;}
        public string ConditionRecordNumber { get; set;}
        public decimal YPR0 { get; set; }
        public decimal YBAJ { get; set; }
        public decimal YBUY { get; set; }
        public decimal YTDN { get; set; }
        public decimal YRPO { get; set; }
        public decimal YELP { get; set; }
        public decimal YPDN { get; set; }
        public decimal YPN2 { get; set; }
        public decimal YAC1 { get; set; }
        public decimal YAC4 { get; set; }
        public decimal YAC2 { get; set; }
        public decimal YAC3 { get; set; }
        public decimal YAC5 { get; set; }
        public decimal YAC6 { get; set; }
        public decimal MWST { get; set; }
    }
}
