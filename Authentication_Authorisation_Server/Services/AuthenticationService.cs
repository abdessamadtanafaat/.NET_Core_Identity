using System.Diagnostics;
using System.Security.Claims;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Authentication_Authorisation.DTO;
using Authentication_Authorisation.Models;
using Authentication_Authorisation.Exceptions;
using Authentication_Authorisation.Utils;
using TaskValidationException = Authentication_Authorisation.Exceptions.ValidationException;
using ValidationFailure = FluentValidation.Results.ValidationFailure;


namespace Authentication_Authorisation.Services;

public class AuthenticationService : IAuthenticationService
{
    
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private readonly IValidator<RegisterDto> _userDtoValidator;
    private readonly TokenUtils _tokenUtils;
    private readonly IConfiguration _configuration;

    public AuthenticationService(
        UserManager<User> userManager,
        IMapper mapper,
        IValidator<RegisterDto> userDtoValidator,
        TokenUtils tokenUtils,
        IConfiguration configuration
        )
    {
        _userManager = userManager;
        _mapper = mapper;
        _userDtoValidator = userDtoValidator;
        _tokenUtils = tokenUtils;
        _configuration = configuration;
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
        
        var adminEmailDomain = _configuration["AdminSettings:AdminEmailDomain"];
        if (user.Email.EndsWith($"@{adminEmailDomain}"))
        {
            await _userManager.AddToRoleAsync(user, "admin"); 
        }
        else
        {
            await _userManager.AddToRoleAsync(user, "user"); 
        }
        
        if (result.Succeeded)
        {
            return new SuccessResponse("User Registered Successfully!", 201);
        }
        
        // if Creation Failed, Extract the error messages and throw a Validation Exception.
        var errorMessages = result.Errors.Select(e => e.Description);
        throw new TaskValidationException("User creation failed", errorMessages.Select(errorMessage => new ValidationFailure(String.Empty, errorMessage)));
        
    }

    public async Task<TokenResponse> LoginAsync(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email); 
        
        if (user == null)
        {
            throw new AuthenticationException("Invalid Email or password."); 
        }

        int maxFailedAttempts = int.Parse(_configuration["Security:MaxFailedAccessAttempts"]);
        int lockoutDurationMinutes = int.Parse(_configuration["Security:LockoutDurationMinutes"]); 
        if (await _userManager.IsLockedOutAsync(user))
        {
            throw new AccountLockedException(
                "Your account is locked after several unsuccessful attempts. Please try again later."); 
        }

        if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            await _userManager.AccessFailedAsync(user);

            if (user.AccessFailedCount >= maxFailedAttempts)
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddMinutes(lockoutDurationMinutes)); 
                throw new AccountLockedException(
                    "Your account is locked after several unsuccessful attempts. Please try again later.");
            }
            throw new AuthenticationException("Invalid Email or password.");

        }

        await _userManager.ResetAccessFailedCountAsync(user);
        
        var token = _tokenUtils.GenerateJwtToken(user);
        var refreshToken = _tokenUtils.GenerateRefreshToken();
        var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"])); 
        
        return new TokenResponse(await token, refreshToken, "Login successful."); 
    }


}
