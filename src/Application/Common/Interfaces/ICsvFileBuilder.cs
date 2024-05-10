using BeatSportsAPI.Application.TodoLists.Queries.ExportTodos;

namespace BeatSportsAPI.Application.Common.Interfaces;
public interface ICsvFileBuilder
{
    byte[] BuildTodoItemsFile(IEnumerable<TodoItemRecord> records);
}
