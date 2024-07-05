using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Helpers;

namespace Services.ZaloPay.Response;
public class CallbackOrder
{
    public string Type { get; set; }
    public string Mac { get; set; }
    public string Data { get; set; }

    public CallbackOrderData? toCallbackOrderData()
    {
        return JsonConvertHelper.DeserializeObject<CallbackOrderData>(Data);
    }

    public bool IsValidMac(string key2)
    {
        var mac = HashHelper.HmacSHA256(this.Data, key2);
        return mac.Equals(this.Mac);
    }
}

public class CallbackOrderData
{
    public int AppId { get; set; }
    public string AppTransId { get; set; }
    public long AppTime { get; set; }
    public string AppUser { get; set; }
    public long Amount { get; set; }
    public string EmbedData { get; set; }
    public string Item { get; set; }
    public long ZpTransId { get; set; }
    public long ServerTime { get; set; }
    public int Channel { get; set; }
    public string MerchantUserId { get; set; }
    public long UserFeeAmount { get; set; }
    public long DiscountAmount { get; set; }
}
