namespace Task_Management_App.Models;

public class AuthResponse
{
    public string Token { get; set; }
    public string Message { get; set; }

    public AuthResponse(string token, string message)
    {
        Token = token;
        Message = message;
    }
}