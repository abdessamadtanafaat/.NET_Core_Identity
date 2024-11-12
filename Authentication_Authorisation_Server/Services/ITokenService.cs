using Authentication_Authorisation.Models;

namespace Authentication_Authorisation.Services;

public interface ITokenService
{
    Task<TokenResponse> RefreshToken(RefreshTokenRequest refreshTokenRequest);
}