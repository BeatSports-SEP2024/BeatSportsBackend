using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Application.Common.Response;
public class CourtSubSettingResponse
{
    public Guid CourtSubSettingId { get; set; }
    public Guid SportCategoryId { get; set; }
    public string? CourtType { get; set; }
    public string? SportCategoryName { get; set; }
}