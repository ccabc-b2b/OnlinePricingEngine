using System.Collections.Generic;

public static class Constants
{
    public const int MaxPageSize = 35;

    public static readonly Dictionary<string, string> CurrencyToCountry = new()
    {
        {"AED","UAE" },
        {"OMR","OM" },
        {"QAR","QA" },
        {"BHD","BH" }
    };
    public const string SuccessMessage = "Data listed successfully!";
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


}
