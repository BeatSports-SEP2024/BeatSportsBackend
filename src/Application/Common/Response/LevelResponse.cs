using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Domain.Entities.Room;

namespace BeatSportsAPI.Application.Common.Response;
public class LevelResponse : IMapFrom<Level>
{
    public Guid LevelId { get; set; }
    public string? LevelName { get; set; }
    public string? LevelDescription { get; set; }
}
