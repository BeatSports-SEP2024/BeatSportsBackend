using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace BeatSportsAPI.Infrastructure.Common;
public class GetJsonInAppSettingsExtension
{
    public static string? GetJson(string key)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        return configuration[key];
    }
}