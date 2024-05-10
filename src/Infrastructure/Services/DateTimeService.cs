using BeatSportsAPI.Application.Common.Interfaces;

namespace BeatSportsAPI.Infrastructure.Services;
public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}
