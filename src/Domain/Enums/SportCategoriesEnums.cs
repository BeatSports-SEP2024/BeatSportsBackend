using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Domain.Enums;
public enum SportCategoriesEnums
{
    [Description("Bóng đá")]
    Soccer,
    [Description("Cầu lông")]
    Badminton,
    [Description("Bóng chuyền")]
    Volleyball,
    All
}
