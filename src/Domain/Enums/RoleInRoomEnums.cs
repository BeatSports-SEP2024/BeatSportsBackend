using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Domain.Enums;
public enum RoleInRoomEnums
{
    [Description("Chủ phòng")]
    Master,
    [Description("Thành viên")]
    Member
}