using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Application.Common.Ultilities;
public static class ParseTimeExtension
{
    public static string GetFormattedTime(TimeSpan timeSpan)
    {
        if (timeSpan.TotalHours < 1)
        {
            return $"{timeSpan.TotalMinutes} minutes ago";
        }
        else if (timeSpan.TotalDays < 1)
        {
            return $"{(int)timeSpan.TotalHours} hours ago";
        }
        else
        {
            return $"{(int)timeSpan.TotalDays} days ago";
        }
    }
}