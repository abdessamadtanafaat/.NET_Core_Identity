using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Task_Management_App.DTO;

public class RegisterDto
{
    public string Email { get; set; } = string.Empty;
    [PasswordPropertyText]
    public string Password { get; set; } = string.Empty;
    [MaxLength(20)]
    public string FullName { get; set; } = string.Empty;
    
}