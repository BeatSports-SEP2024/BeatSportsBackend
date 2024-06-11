using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Application.Common.Ultilities;
public class TimeUtils
{
    public static IEnumerable<(TimeSpan, TimeSpan)> GenerateTimeSlots(TimeSpan startTime, TimeSpan endTime)
    {
        TimeSpan start = startTime;
        TimeSpan end = endTime;

        while (start < end)
        {
            TimeSpan nextTime = start.Add(TimeSpan.FromMinutes(30));
            if (nextTime > end)
                nextTime = end;

            yield return (start, nextTime);
            start = nextTime;
        }
    }
}