using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BeatSportsAPI.Application.Common.Ultilities;
public static class StringExtraction
{
    // Phương thức tách và trả về chỉ phần văn bản từ chuỗi đầu vào
    public static string ExtractText(string input)
    {
        Regex regex = new Regex(@"(.+?)(\d+)$");
        Match match = regex.Match(input);

        if (match.Success)
        {
            return match.Groups[1].Value.Trim();
        }
        else
        {
            throw new InvalidOperationException("No match found for text part.");
        }
    }

    // Phương thức tách và trả về chỉ phần số từ chuỗi đầu vào
    public static string ExtractNumber(string input)
    {
        Regex regex = new Regex(@"(.+?)(\d+)$");
        Match match = regex.Match(input);

        if (match.Success)
        {
            return match.Groups[2].Value;
        }
        else
        {
            throw new InvalidOperationException("No match found for number part.");
        }
    }
}