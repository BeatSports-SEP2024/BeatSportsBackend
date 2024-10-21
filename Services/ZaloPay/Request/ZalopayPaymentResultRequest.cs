using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Utils.Helpers;

namespace Services.ZaloPay.Request;
public class ZalopayPaymentResultRequest
{
    //amount=50000&
    //appid=554&
    //apptransid=240705_ac497ec2-ff41-4a3b-8e74-6b5be1aa6950&
    //bankcode=&
    //checksum=a1a9fde0924a248be2c1d845f6cc9755728a8659a38cfc4f4b30cb5ea28a5d3b&
    //discountamount=0&
    //pmcid=38&
    //status=1
    

    [JsonProperty("appid")]
    public int? appid { get; set; }

    [JsonProperty("apptransid")]
    public string apptransid { get; set; } = string.Empty;

    [JsonProperty("pmcid")]
    public int? pmcid { get; set; }

    [JsonProperty("bankcode")]
    public string? bankcode { get; set; }

    [JsonProperty("amount")]
    public long? amount { get; set; }

    [JsonProperty("discountamount")]
    public long? discountamount { get; set; }

    [JsonProperty("status")]
    public int? status { get; set; }

    [JsonProperty("checksum")]
    public string? checksum { get; set; }

   /* [JsonProperty("embeddata")]
    public string embeddata { get; set; } = string.Empty;

    [JsonProperty("apptime")]
    public long? apptime { get; set; }

    [JsonProperty("appuser")]
    public string appuser { get; set; } = string.Empty;

    [JsonProperty("item")]
    public string item { get; set; } = string.Empty;

    [JsonProperty("zptransid")]
    public long? zptransid { get; set; } 

    [JsonProperty("servertime")]
    public long? servertime { get; set; }

    [JsonProperty("channel")]
    public int? channel { get; set; } 

    [JsonProperty("merchantuserid")]
    public string merchantuserid { get; set; } = string.Empty;

    [JsonProperty("userfeeamount")]
    public long userfeeamount { get; set; }*/


    public bool IsValidSignature(string key2)
    {
        var checksumData = $"{this.appid}|{this.apptransid}|{this.pmcid}|{this.bankcode}|{this.amount}|{this.discountamount}|{this.status}";
        var mac = HashHelper.HmacSHA256(checksumData, key2);
        return mac.Equals(checksum);
    }
}
