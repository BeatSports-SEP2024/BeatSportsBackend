using BeatSportsAPI.Application.Common.Ultilities;
using BeatSportsAPI.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace BeatSportsAPI.Application.Common.Middlewares;
public class CustomAuthorizeAttribute : AuthorizeAttribute
{
    //Custom Authorization
    //Use in controller:     [CustomAuthorize(RoleEnums.Customer)]
    public CustomAuthorizeAttribute(params RoleEnums[] role)
    {
        var allowRoles = role.Select(x => x.GetDescriptionFromEnum());
        Roles = string.Join(", ", allowRoles);
    }
}