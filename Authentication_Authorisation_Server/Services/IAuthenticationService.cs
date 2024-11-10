using Authentication_Authorisation.DTO;
using Authentication_Authorisation.Models;

namespace Authentication_Authorisation.Services;

public interface IAuthenticationService
{
    Task<SuccessResponse> RegisterUserAsync(RegisterDto registerDto); 
    Task<AuthResponse> LoginAsync(LoginDto loginDto); 
}