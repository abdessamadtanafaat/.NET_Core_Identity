using Authentication_Authorisation.DTO;
using Authentication_Authorisation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Authentication_Authorisation.Controllers;

[Authorize(Roles = Roles.User)]
[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{

    [HttpGet("info")]
    public IActionResult GetUserInfo()
    {
        return Ok("This is an endpoint for the user"); 
    }
}