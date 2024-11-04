using System.Diagnostics;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Task_Management_App.DTO;
using Task_Management_App.Models;
using Task_Management_App.Exceptions;
using Task_Management_App.Utils;
using TaskValidationException = Task_Management_App.Exceptions.ValidationException;


namespace Task_Management_App.Services;

public class AuthenticationService : IAuthenticationService
{
    
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private readonly IValidator<RegisterDto> _userDtoValidator;
    private readonly AuthUtils _authUtils;

    public AuthenticationService(
        UserManager<User> userManager,
        IMapper mapper,
        IValidator<RegisterDto> userDtoValidator,
        AuthUtils authUtils
        )
    {
        _userManager = userManager;
        _mapper = mapper;
        _userDtoValidator = userDtoValidator;
        _authUtils = authUtils; 
    }
    
    public async Task<SuccessResponse> RegisterUserAsync(RegisterDto registerDto)
    {
        // validate the icomming user DTO using FluentValidation.
        ValidationResult validationResult = await _userDtoValidator.ValidateAsync(registerDto);       
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new ValidationFailure(e.PropertyName, e.ErrorMessage));
            throw new TaskValidationException("User registration failed", errors);
        }
        // Map the userDTO to the User Model using AutoMapper
        var user = _mapper.Map<User>(registerDto);
        user.UserName ??= user.Email; // set Username to Email if UserName is not provided.
        
        // Create the user in the system asynchronously.
        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (result.Succeeded)
        {
            return new SuccessResponse("User Registered Successfully!", 201);
        }
        
        // if Creation Failed, Extract the error messages and throw a Validation Exception.
        var errorMessages = result.Errors.Select(e => e.Description);
        throw new TaskValidationException("User creation failed", errorMessages.Select(errorMessage => new ValidationFailure(String.Empty, errorMessage)));
        
    }

    public async Task<AuthResponse> LoginAsync(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            throw new AuthenticationException("Invalid Email or password."); 
        }

        var token = _authUtils.GenerateJwtToken(user);
        return new AuthResponse(token, "Login successful."); 
    }
    
}
