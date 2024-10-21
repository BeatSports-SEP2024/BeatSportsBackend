using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Application.Common.Base;
public class BaseError
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
