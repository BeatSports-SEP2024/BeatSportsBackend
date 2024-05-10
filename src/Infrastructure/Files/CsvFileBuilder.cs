using System.Globalization;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.TodoLists.Queries.ExportTodos;
using BeatSportsAPI.Infrastructure.Files.Maps;
using CsvHelper;

namespace BeatSportsAPI.Infrastructure.Files;
public class CsvFileBuilder : ICsvFileBuilder
{
    public byte[] BuildTodoItemsFile(IEnumerable<TodoItemRecord> records)
    {
        using var memoryStream = new MemoryStream();
        using (var streamWriter = new StreamWriter(memoryStream))
        {
            using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

            csvWriter.Configuration.RegisterClassMap<TodoItemRecordMap>();
            csvWriter.WriteRecords(records);
        }

        return memoryStream.ToArray();
    }
}
