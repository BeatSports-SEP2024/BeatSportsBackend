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

        if (startTime < TimeSpan.Zero || endTime > new TimeSpan(23, 59, 59) || startTime >= endTime)
        {
            throw new ArgumentOutOfRangeException("Thời gian bắt đầu và kết thúc phải nằm trong khoảng từ 00:00 đến 23:59 và thời gian bắt đầu phải nhỏ hơn thời gian kết thúc.");
        }

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