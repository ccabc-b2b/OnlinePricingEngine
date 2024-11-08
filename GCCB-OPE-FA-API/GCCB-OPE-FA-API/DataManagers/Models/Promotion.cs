using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCCB_OPE_FA_API.DataManagers.Models
{
    public class Promotion
    {
        public string PromotionID { get; set; }
        public string CustomerNumber { get; set; }
        public string PromotionDescription { get; set; }
        public string PromotionType { get; set; }
        public string ConditionType { get; set; }
        public DateTime? AgreementValidFromDate { get; set; }
        public DateTime? AgreementValidToDate { get; set; }
        public string MinQty { get; set; }
        public string MinValue { get; set; }
        public string MaxQty { get; set; }
        public string MaxValue { get; set; }
        public string RequirementId { get; set; }
        public string RequirementQty { get; set; }
        public string RequirementValue { get; set; }
        public string MaterialNumber { get; set; }
        public string MaterialGroupID { get; set; }
        public string PromoRewardID { get; set; }
        public string RewardQty { get; set; }
        public string RewardValue { get; set; }
        public string RewardPercentage { get; set; }
        public int IsSlab { get; set; }
        public string FromQTY { get; set; }
        public string ToQTY { get; set; }
        public DateTime? ActiveFrom { get; set; }
        public DateTime? ActiveTo { get; set; }
        public string DiscountType { get; set; }
        public string FreeGoodQTY { get; set; }

        }
    }
