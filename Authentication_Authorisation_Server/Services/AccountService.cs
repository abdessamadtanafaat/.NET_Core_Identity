using Authentication_Authorisation.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Authentication_Authorisation.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<User> _userManager;

    public AccountService(UserManager<User> userManager)
    {
        _userManager = userManager; 
    }
    public async Task<string> ConfirmEmail(string userId, string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
        {
            return "User ID et token requires.";
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return "user not found"; 
        }

        //var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token)); 
        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
        {
            return "Email confirmed."; 
            //return new SuccessResponse("Email confirmed ", 200);
        }
        return "Error confirming email."; 
        //return new ErrorResponse("Email Not confirmed ", 400);
    }
}