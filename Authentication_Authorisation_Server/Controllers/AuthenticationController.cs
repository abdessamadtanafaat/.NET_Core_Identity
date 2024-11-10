﻿using Microsoft.AspNetCore.Mvc;
using Authentication_Authorisation.DTO;
using Authentication_Authorisation.Models;
using Authentication_Authorisation.Services;
using ValidationException = Authentication_Authorisation.Exceptions.ValidationException;

namespace Authentication_Authorisation.Controllers;

[Route("api/users")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authService; 
    
    public AuthenticationController(IAuthenticationService authService)
    {
        _authService = authService; 
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            var successResponse = await _authService.RegisterUserAsync(registerDto);
            return Ok(successResponse);
        }
        catch(ValidationException ex)
        {
            var errorResponse = new ErrorResponse(ex.Errors.Select(e => e.ErrorMessage), 400, "Validation Failure");
            return BadRequest(errorResponse); 
        }
        catch
        {
            return StatusCode(500, new ErrorResponse(new[] { "An unexpected error occurred." }, 500,"An Unexpected error occured.")); 
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
            var response = await _authService.LoginAsync(loginDto);
            return Ok(response); 
    }
    /*Example : async {
        "email": "raja@gmail.com",
        "password": "Raja2015"
    }*/
}