using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Services.ZaloPay.Response;
using Utils.Helpers;

namespace Services.ZaloPay.Request;
public class CreateZalopayPayRequest
{
    public CreateZalopayPayRequest(int appId, string appUser, long appTime,
            long amount, string appTransId, string bankCode, string description, object embedData/*, string[]? items*/, string callbackUrl)
    {
        AppId = appId;
        AppUser = appUser;
        AppTime = appTime;
        Amount = amount;
        AppTransId = appTransId;
        BankCode = bankCode;
        Description = description;
        EmbedData = embedData;
        /*Items = items;*/
        CallbackUrl = callbackUrl;
    }
    public int AppId { get; set; }
    public string AppUser { get; set; } = string.Empty;
    public long AppTime { get; set; }
    public long Amount { get; set; }
    public string AppTransId { get; set; } = string.Empty;
    public string ReturnUrl { get; }
    public object EmbedData { get; set; } = string.Empty;
    /*public object[]? Items { get; set; } = new string[0];*/
    public string Mac { get; set; } = string.Empty;
    public string BankCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CallbackUrl { get; set; } = string.Empty;

    public void MakeSignature(string key)
    {
        var data = AppId + "|" + AppTransId + "|" + AppUser + "|" + Amount + "|"
          + AppTime + "|" + JsonConvert.SerializeObject(EmbedData) + "|" /* + JsonConvert.SerializeObject(Items)*/;

        this.Mac = HashHelper.HmacSHA256(data, key);
    }

    public Dictionary<string, string> GetContent()
    {
        Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

        keyValuePairs.Add("appid", AppId.ToString());
        keyValuePairs.Add("appuser", AppUser);
        keyValuePairs.Add("apptime", AppTime.ToString());
        keyValuePairs.Add("amount", Amount.ToString());
        keyValuePairs.Add("apptransid", AppTransId);
        keyValuePairs.Add("embeddata", JsonConvert.SerializeObject(EmbedData));
        /*keyValuePairs.Add("item", JsonConvert.SerializeObject(Items));*/
        keyValuePairs.Add("description", Description);
        keyValuePairs.Add("bankcode", "zalopayapp");
        keyValuePairs.Add("mac", Mac);
        keyValuePairs.Add("callback_url", CallbackUrl);

        return keyValuePairs;
    }

    public (bool, string) GetLink(string paymentUrl)
    {
        using var client = new HttpClient();
        var content = new FormUrlEncodedContent(GetContent());
        var response = client.PostAsync(paymentUrl, content).Result;

        /*var content = new FormUrlEncodedContent(GetContent());
        var request = new HttpRequestMessage(HttpMethod.Post, paymentUrl)
        {
            Content = content
        };

        using var client = new HttpClient();
        var response = await client.SendAsync(request);*/

        if (response.IsSuccessStatusCode)
        {
            var responseContent = response.Content.ReadAsStringAsync().Result;
            var responseData = JsonConvert
                .DeserializeObject<CreateZalopayPayResponse>(responseContent);
            if (responseData.returnCode == 1)
            {
                return (true, responseData.orderUrl);
            }
            else
            {
                return (false, responseData.returnMessage);
            }

        }
        else
        {
            return (false, response.ReasonPhrase ?? string.Empty);
        }
    }
}
