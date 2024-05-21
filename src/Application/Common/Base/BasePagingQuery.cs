using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BeatSportsAPI.Application.Common.Base;
public class BasePagingQuery
{
    [JsonProperty("Criteria")]
    public string? Criteria { get; set; } = string.Empty;

    [JsonProperty("PageSize")]
    public int? PageSize { get; set; } = 20;

    [JsonProperty("PageIndex")]
    public int? PageIndex { get; set; } = 1;
}
