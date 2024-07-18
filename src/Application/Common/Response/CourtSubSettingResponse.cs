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
    public string? TypeSize { get; set; }
    public string? SportCategoryName { get; set; }
}

public class CourtSubSettingV2
{
    public Guid CourtSubSettingId { get; set; }
    public Guid SportCategoryId { get; set; }
    public string? TypeSize { get; set; }
    public string? SportCategoryName { get; set; }
    public List<CourtSubdivisionV4>? CourtSubdivision { get; set; }
}