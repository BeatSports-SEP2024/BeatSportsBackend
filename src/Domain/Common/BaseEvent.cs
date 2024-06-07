using MediatR;

namespace BeatSportsAPI.Domain.Common;
public abstract class BaseEvent : INotification
{
    public Guid Id { get; set; }
}
