using Task_Management_App.DTO;
using Task_Management_App.Models;

namespace Task_Management_App.Services;

public interface IAuthenticationService
{
    Task<SuccessResponse> RegisterUserAsync(RegisterDto registerDto); 
    Task<AuthResponse> LoginAsync(LoginDto loginDto); 
}