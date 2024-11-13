using Authentication_Authorisation.Models;
using Authentication_Authorisation.Services;
using Microsoft.AspNetCore.Mvc;

namespace Authentication_Authorisation.Controllers;

[Route("api")]
[ApiController]
public class TokenController : ControllerBase
{
    
    private readonly ITokenService _tokenService; 
    public TokenController(ITokenService tokenService)
    {
        _tokenService = tokenService; 
    }
    
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest)
    {
        var response = await _tokenService.RefreshToken(refreshTokenRequest);
        return Ok(response);
    }
}