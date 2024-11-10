using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Authentication_Authorisation.Models;

public class User : IdentityUser
{
    [StringLength(20)] // enforce limit in DB and avoid increase memory.
    public string FullName { get; set; } = string.Empty; //initialize with empty string because should always have a value
    //public string? FullName { get; set; } = string.Empty; // can be null . 
    //No need to redefine Email or Password
    [StringLength(20)]
    public string? AvatarUrl { get; set; }

}