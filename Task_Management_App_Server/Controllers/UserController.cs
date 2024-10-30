using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Task_Management_App.DTO;
using Task_Management_App.Models;
using Task_Management_App.Services;

namespace Task_Management_App.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private readonly IValidator<UserDto> _userDtoValidator;

    public readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService; 
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserDto userDto)
    {
        var (success, errors) = await _userService.RegisterUserAsync(userDto);
        if (success)
        {
            return Ok(new { message = "User registered successfully!" });
        }
        return BadRequest(errors);
    }
}
