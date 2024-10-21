using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Domain.Enums;
public enum LevelEnums
{
    [Description("Trung Bình")]
    Medium,
    [Description("Tập sự")]
    Newbie,
    [Description("Chuyên gia")]
    Expert
}