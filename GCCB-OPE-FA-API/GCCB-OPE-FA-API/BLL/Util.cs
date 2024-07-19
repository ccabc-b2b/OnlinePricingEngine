using GCCB_OPE_FA_API.DataManagers.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace GCCB_OPE_FA_API.BLL
{
    public static class Util
    {
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
    }
}
