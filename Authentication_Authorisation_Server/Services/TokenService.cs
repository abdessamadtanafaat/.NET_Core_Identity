using System.Security.Claims;
using Authentication_Authorisation.Exceptions;
using Authentication_Authorisation.Models;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Authentication_Authorisation.Services;

public class TokenService : ITokenService
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration; 
    private readonly RoleManager<IdentityRole> _roleManager; 

 
    public TokenService(
        UserManager<User> userManager,
        IConfiguration configuration,
        RoleManager<IdentityRole> roleManager
        )
    {
        _userManager = userManager;
        _configuration = configuration;
        _roleManager = roleManager;

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
            principal = GetPrincipalFromExpiredToken(refreshTokenRequest.Token);
        }
        catch (Exception ex)
        {
            throw new InvalidTokenException("Invalid token provided.");
        }
        
        if (principal == null)
        {
            throw new InvalidTokenException("Invalid access token");
        }

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            throw new InvalidTokenException("User Not Found"); 
        }
        
        var existingRefreshToken = await _userManager.GetAuthenticationTokenAsync(user, "Default", "RefreshToken");
        if (existingRefreshToken != refreshTokenRequest.RefreshToken)
        {
            throw new InvalidTokenException("Invalid Refresh Token"); 
        }
        

        // check if the refresh token has expired.

        var refreshTokenExpiryDays = int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"]);
        var tokenCreatedDate = DateTime.UtcNow.AddMinutes(-refreshTokenExpiryDays);
        if (tokenCreatedDate > DateTime.UtcNow)
        {
            throw new InvalidTokenException("Refresh token has expired."); 
        }

        // invalidate the old refresh token be deleting it.
        await _userManager.RemoveAuthenticationTokenAsync(user, "Default", "RefreshToken"); 

        // generate new access and refresh token .
        var newAccessToken = await GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();

        await _userManager.SetAuthenticationTokenAsync(user, "Default", "RefreshToken", newRefreshToken);
        return new TokenResponse( newAccessToken,  newRefreshToken,"Token refreshed successfully.");

    }

    public async Task<string> GenerateJwtToken(User user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user); 
        var userRoles = await _userManager.GetRolesAsync(user);
        
        var roleClaims = new List<Claim>();
        foreach (var role in userRoles)
        {
            roleClaims.Add(new Claim(ClaimTypes.Role, role));
            
            var identityRole = await _roleManager.FindByNameAsync(role);
            roleClaims.AddRange(await _roleManager.GetClaimsAsync(identityRole));
        }
        var authClaims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),

        };
        authClaims.AddRange(userClaims);
        authClaims.AddRange(roleClaims);
        
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"])),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
        }

        return Convert.ToBase64String(randomNumber); 
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");
        return principal; 
    }
    
}