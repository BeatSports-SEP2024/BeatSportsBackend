using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Application.Models.Authentication;
public class LoginModelResponse
{
    public string? Id { get; set; }
    public string? UserName { get; set; } 
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Role { get; set; }
    public string? Token { get; set; }
}
