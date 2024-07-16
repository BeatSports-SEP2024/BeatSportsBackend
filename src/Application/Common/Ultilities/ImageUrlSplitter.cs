using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Application.Common.Ultilities;
public static class ImageUrlSplitter
{
    public static List<string> SplitImageUrls(string imageUrls)
    {
        if (string.IsNullOrEmpty(imageUrls))
            return new List<string>();

        var urls = imageUrls.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        var trimmedUrls = new List<string>();

        foreach (var url in urls)
        {
            trimmedUrls.Add(url.Trim());
        }

        return trimmedUrls;
    }

    public static string SplitAndGetFirstImageUrls(string imageUrls)
    {
        if (string.IsNullOrEmpty(imageUrls))
            return string.Empty;

        var urls = imageUrls.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(url => url.Trim())
                        .ToList();

        return urls.FirstOrDefault() ?? string.Empty;
    }
}