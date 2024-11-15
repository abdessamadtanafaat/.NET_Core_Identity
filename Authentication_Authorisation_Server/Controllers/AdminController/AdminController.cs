using Authentication_Authorisation.DTO;
using Authentication_Authorisation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Authentication_Authorisation.Controllers;

[Authorize(Roles = Roles.Admin)]
[Route("api/admin")]
[ApiController]
public class AdminController : ControllerBase
{
    [HttpGet("info")]
    public IActionResult GetAdminInfo()
    {
        
        return Ok("This is an endpoint for the admin."); 
    }
}