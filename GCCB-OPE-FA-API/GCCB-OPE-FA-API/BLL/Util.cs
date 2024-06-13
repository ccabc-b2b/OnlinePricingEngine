using GCCB_OPE_FA_API.DataManagers.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace GCCB_OPE_FA_API.BLL
{
    public static class Util
    {
        /// <summary>
        /// Convert datatable to list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> DataTabletoList<T>(DataTable dt) where T : new()
        {
            try
            {
                List<T> list = new List<T>();
                foreach (DataRow row in dt.Rows)
                {
                    T obj = new T();
                    foreach (DataColumn col in dt.Columns)
                    {
                        var property = obj.GetType().GetProperty(col.ColumnName);
                        if (property != null && row[col] != DBNull.Value)
                        {
                            object value = Convert.ChangeType(row[col], property.PropertyType);
                            property.SetValue(obj, row[col]);
                        }
                    }
                    list.Add(obj);
                }
                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static int CharLengthMapping(string varKey)
        {
            var length = varKey switch
            {
                Constants.Customer => 10,
                Constants.Material => 8,
                Constants.PriceList => 2,
                Constants.SalesOrg => 4,
                Constants.SalesGroup => 3,
                Constants.MaterialGroup => 9,
                Constants.KeyAccount => 4,
                Constants.Plant => 4,
                Constants.PaymentTerms => 4,
                Constants.BottlerTr => 2,
                Constants.SubTrade => 2,
                Constants.POType => 3,
                Constants.SubTradeChannel => 3,
                _ => 0,
            };
            return length;
        }
        public static bool isBetween(string value, string lower, string upper)
        {
            return string.Compare(value, lower) >= 0 && string.Compare(value, upper) <= 0;
        }
        public static string GetMarketCode(string code)
        {
            string countryCode = Constants.CurrencyToCountry.ContainsKey(code) ? Constants.CurrencyToCountry[code] : code;
            return $"{countryCode}{DateTime.Now.ToString("MMddyyyyHHmmss")}";
        }
        //TODO: Remove
        public static List<ConditionItems> GetSampleConditionItemsData()
        {
            List<ConditionItems> lstConditionItems = new List<ConditionItems>()
            {
                new ConditionItems()
                {
                     ConditionRecordNumber ="0000218972",
                     ConditionType ="YPR0",
                     VariableKey ="861729743210151900",
                     ValidFrom =Convert.ToDateTime("2013-12-04"),
                     ValidTo =Convert.ToDateTime("9999-12-31"),
                     ConditionAmountOrPercentageRate ="0",
                     CurrencyOrPercentageRateUnit ="AED",
                     ConditionPricingUnit ="1",
                     ConditionUnit ="EA",
                     ConditionAmountLowerLimit ="0.00",
                     ConditionAmountUpperLimit ="0.00",
                     AccrualAmount ="0.00"
                },
                new ConditionItems()
                {
                     ConditionRecordNumber ="0000218972",
                     ConditionType ="YPR0",
                     VariableKey ="512 28910151900",
                     ValidFrom =Convert.ToDateTime("2013-12-04"),
                     ValidTo =Convert.ToDateTime("9999-12-31"),
                     ConditionAmountOrPercentageRate ="2.00-",
                     CurrencyOrPercentageRateUnit ="AED",
                     ConditionPricingUnit ="1",
                     ConditionUnit ="EA",
                     ConditionAmountLowerLimit ="0.00",
                     ConditionAmountUpperLimit ="0.00",
                     AccrualAmount ="0.00"
                },
                new ConditionItems()
                {
                     ConditionRecordNumber ="0000218972",
                     ConditionType ="YBAJ",
                     VariableKey ="861729743210151900",
                     ValidFrom =Convert.ToDateTime("2013-12-04"),
                     ValidTo =Convert.ToDateTime("9999-12-31"),
                     ConditionAmountOrPercentageRate ="1.50",
                     CurrencyOrPercentageRateUnit ="AED",
                     ConditionPricingUnit ="1",
                     ConditionUnit ="EA",
                     ConditionAmountLowerLimit ="0.00",
                     ConditionAmountUpperLimit ="0.00",
                     AccrualAmount ="0.00"
                }
            };
            return lstConditionItems;
        }
        public static List<Promotion> GetSamplePromotionsData()
        {
            List<Promotion> lstConditionItems = new List<Promotion>()
            {
                new Promotion()
                {
                     PromotionID  ="0000018382",
                     MaterialNumber ="10151900",
                     CustomerNumber ="8617297432",
                     PromotionType="1",
                     MinQty="1.000",
                     MaxQty="5.000"

                }
            };
            return lstConditionItems;
        }
    }
}
