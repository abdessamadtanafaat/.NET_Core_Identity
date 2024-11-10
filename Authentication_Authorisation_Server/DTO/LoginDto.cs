using System.ComponentModel.DataAnnotations;

namespace Authentication_Authorisation.DTO;

public class LoginDto
{
    [Required, MaxLength(50)] 
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}