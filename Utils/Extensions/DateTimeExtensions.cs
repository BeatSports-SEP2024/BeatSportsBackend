﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Extensions;
public static class DateTimeExtensions
{
    public static long GetTimeStamp(this DateTime date)
    {
        return (long)(date.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
    }

    public static long GetTimeStamp()
    {
        return GetTimeStamp(DateTime.Now);
    }
}
