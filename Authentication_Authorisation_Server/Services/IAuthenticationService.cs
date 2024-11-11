using Authentication_Authorisation.DTO;
using Authentication_Authorisation.Models;

namespace Authentication_Authorisation.Services;

public interface IAuthenticationService
{
    Task<SuccessResponse> RegisterUserAsync(RegisterDto registerDto); 
    Task<TokenResponse> LoginAsync(LoginDto loginDto);
    Task<TokenResponse> RefreshToken(RefreshTokenRequest refreshTokenRequest);
}