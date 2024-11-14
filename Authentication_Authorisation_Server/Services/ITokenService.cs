using System.Security.Claims;
using Authentication_Authorisation.Models;

namespace Authentication_Authorisation.Services;

public interface ITokenService
{
    Task<TokenResponse> RefreshToken(RefreshTokenRequest refreshTokenRequest);
    Task<string> GenerateJwtToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token); 

}