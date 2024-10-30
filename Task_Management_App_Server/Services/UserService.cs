using System.Diagnostics;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Task_Management_App.DTO;
using Task_Management_App.Exceptions;
using Task_Management_App.Models;
using ValidationException = FluentValidation.ValidationException;

namespace Task_Management_App.Services;

public class UserService
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private readonly IValidator<UserDto> _userDtoValidator;

    public UserService(UserManager<User> userManager, IMapper mapper, IValidator<UserDto> userDtoValidator)
    {
        _userManager = userManager;
        _mapper = mapper;
        _userDtoValidator = userDtoValidator; 
    }

    public async Task<(bool Success, IEnumerable<string> Errors)> RegisterUserAsync(UserDto userDto)
    {
        // validate the icomming DTO
        ValidationResult validationResult = await _userDtoValidator.ValidateAsync(userDto);
        if (!validationResult.IsValid)
        {
            throw new ValidationException("User registration failed", validationResult.Errors);
            //return (false, validationResult.Errors.Select(e=> e.ErrorMessage));
        }

        var user = _mapper.Map<User>(userDto);
        if (string.IsNullOrEmpty(user.UserName))
        {
            user.UserName = user.Email;
        }
        
        
        var result = await _userManager.CreateAsync(user, userDto.Password);
        if (result.Succeeded)
        {
            return (true, Enumerable.Empty<string>());
        }
        if (!result.Succeeded)
        {
            throw new ValidationException("User Creation Failed",
                result.Errors.Select(e => new ValidationFailure("", e.Description)));
        }
        throw new NotFoundException("User not found");
        //return (false, result.Errors.Select(e => e.Description));
        
    }
}