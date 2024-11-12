using System.Security.Claims;
using Authentication_Authorisation.Exceptions;
using Authentication_Authorisation.Models;
using Authentication_Authorisation.Utils;
using Microsoft.AspNetCore.Identity;

namespace Authentication_Authorisation.Services;

public class TokenService : ITokenService
{
    private readonly TokenUtils _tokenUtils;
    private readonly UserManager<User> _userManager; 
 
    public TokenService(TokenUtils tokenUtils, UserManager<User> userManager)
    {
        _tokenUtils = tokenUtils;
        _userManager = userManager; 
    }
    
    public async Task<TokenResponse> RefreshToken(RefreshTokenRequest refreshTokenRequest)
    {
        if (string.IsNullOrEmpty(refreshTokenRequest.Token) || string.IsNullOrEmpty(refreshTokenRequest.RefreshToken))
        {
            throw new InvalidTokenException("Token cannot be empty.");
        }
        ClaimsPrincipal principal = null;

        try
        { 
            principal = _tokenUtils.GetPrincipalFromExpiredToken(refreshTokenRequest.Token);
        }
        catch (Exception ex)
        {
            throw new InvalidTokenException("Invalid token provided.");
        }
        
        if (principal == null)
        {
            throw new InvalidTokenException("Invalid access token or refresh token");
        }

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            throw new InvalidTokenException("User Not Found"); 
        }

        // Commented out: no need to verify the refresh token with the two-factor provider for now
        /*var isValidRefreshToken =
            await _userManager.VerifyUserTokenAsync(user, "Default", "RefreshToken", refreshTokenRequest.RefreshToken);
        if (!isValidRefreshToken)
        {
            throw new InvalidTokenException("Invalid refreshToken");
        }*/
        
        var newAccessToken = _tokenUtils.GenerateJwtToken(user);
        var newRefreshToken = _tokenUtils.GenerateRefreshToken();

        await _userManager.SetAuthenticationTokenAsync(user, "Default", "RefreshToken", newRefreshToken);
        return new TokenResponse(await newAccessToken,  newRefreshToken,"Token refreshed successfully.");

    }

}