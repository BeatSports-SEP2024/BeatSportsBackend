using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Application.Common.Attributes;
public class NormalizeAttribute : Attribute
{
    public string Normalize(string input)
    {
        return input?.ToLowerInvariant() ?? string.Empty;
    }
}