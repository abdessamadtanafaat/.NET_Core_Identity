namespace Authentication_Authorisation.Models;

public class TokenResponse
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public string Message { get; set; }

    public TokenResponse(string token, string refreshToken ,string message)
    {
        Token = token;
        RefreshToken = refreshToken;
        Message = message;
    }
}