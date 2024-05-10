using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Domain.Entities;

namespace BeatSportsAPI.Application.TodoLists.Queries.ExportTodos;
public class TodoItemRecord : IMapFrom<TodoItem>
{
    public string? Title { get; set; }

    public bool Done { get; set; }
}
