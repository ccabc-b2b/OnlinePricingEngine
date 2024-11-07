using GCCB_OPE_FA_API.BLL;
using System;
using System.Collections.Generic;

public static class Constants
{
    public static readonly Dictionary<string, string> CurrencyToCountry = new()
    {
        {"AED","UAE" },
        {"OMR","OM" },
        {"QAR","QA" },
        {"BHD","BH" }
    };
    public const string SuccessMessage = "Data listed successfully!";
    public const string InvalidApiKey = "Invalid API Key";
    public const string InvalidDeliveryDate = "Invalid Delivery Date , does not accept null or empty value";
    public const string ErrorMessage = "An error occurred while processing the request.";
    public const string AED = "AED";
    public const string OMR = "OMR";
    public const string QAR = "QAR";
    public const string BHD = "BHD";

    public const string Customer = "Customer";
    public const string Material = "Material";
    public const string PriceList = "PriceList";
    public const string SalesOrg = "SalesOrg";
    public const string SalesGroup = "SalesGroup";
    public const string MaterialGroup = "MaterialGroup";
    public const string KeyAccount = "KeyAccount";
    public const string Plant = "Plant";
    public const string PaymentTerms = "PaymentTerms";
    public const string BottlerTr = "BottlerTr";
    public const string SubTrade = "SubTrade";
    public const string POType = "POType";
    public const string SubTradeChannel = "SubTradeChannel";
    public const string DefaultQuantity = "0.000";
    public static Dictionary<string, Func<string, bool>> ConditionTableRule = new()
    {
        {
            "YPR0+924",materialGroup=>!Util.isBetween(materialGroup,"14000","15999") && materialGroup!="71000" &&
            materialGroup!="71010" && materialGroup!="71020" && materialGroup!="70300"
        },
        {
            "YBAJ+980",materialGroup=>!Util.isBetween(materialGroup,"14000","15999") && materialGroup!="71000" &&
            materialGroup!="71010" && materialGroup!="71020" && materialGroup!="70300"
        },
        {
            "YBUY+980",materialGroup=>!Util.isBetween(materialGroup,"14000","15999") && materialGroup!="71000" &&
            materialGroup!="71010" && materialGroup!="71020" && materialGroup!="70300"
        },
        {
            "YTDN+980",materialGroup=>!Util.isBetween(materialGroup,"14000","15999")&&materialGroup!="71000" &&
            materialGroup!="71010" && materialGroup!="71020" &&materialGroup!="70300"
        },
        {
            "YRPO+503",materialGroup=>!Util.isBetween(materialGroup,"14000","15999")&&materialGroup!="71000" &&
            materialGroup!="71010" && materialGroup!="71020" &&materialGroup!="70300"
        },
        {
            "YELP+980",materialGroup=>!Util.isBetween(materialGroup,"14000","15999")&&materialGroup!="71000" &&
            materialGroup!="71010" && materialGroup!="71020" && materialGroup!="70300"
        },
        {
            "YPDN+980",materialGroup=>!Util.isBetween(materialGroup,"14000","15999")&&materialGroup!="71000" &&
            materialGroup!="71010" && materialGroup!="71020" && materialGroup!="70300"
        },
        {
            "YPN2+980",materialGroup=>!Util.isBetween(materialGroup,"14000","15999")&&materialGroup!="71000" &&
            materialGroup!="71010" && materialGroup!="71020" && materialGroup!="70300"
        }
    };
}