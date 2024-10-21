using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Domain.Enums;

namespace BeatSportsAPI.Application.Common.Ultilities;

public static class SportCategoryMapper
{
    public static readonly Dictionary<string, SportCategoriesEnums> SportCategoryMapping = new Dictionary<string, SportCategoriesEnums>
    {
        { "Bóng đá", SportCategoriesEnums.Soccer },
        { "Cầu lông", SportCategoriesEnums.Badminton },
        { "Bóng chuyền", SportCategoriesEnums.Volleyball }
    };
}