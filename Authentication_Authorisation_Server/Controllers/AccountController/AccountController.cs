using System.Text;
using Authentication_Authorisation.Models;
using Authentication_Authorisation.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Authentication_Authorisation.Controllers.AccountController;

[ApiController]
[Route("api/")]
public class AccountController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IAccountService _accountService; 

    public AccountController(UserManager<User> userManager, IAccountService accountService)
    {
        _userManager = userManager;
        _accountService = accountService; 
    }

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        var response = await _accountService.ConfirmEmail(userId, token);
        return Ok(response); 
        
    }
}