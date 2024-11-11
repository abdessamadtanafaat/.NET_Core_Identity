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
using Microsoft.IdentityModel.Tokens;
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
        if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            throw new AuthenticationException("Invalid Email or password."); 
        }

        var token = _tokenUtils.GenerateJwtToken(user);
        var refreshToken = _tokenUtils.GenerateRefreshToken();
        var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"])); 
        
        return new TokenResponse(token, refreshToken, "Login successful."); 
    }

    public async Task<TokenResponse> RefreshToken(RefreshTokenRequest refreshTokenRequest)
    {
        if (string.IsNullOrEmpty(refreshTokenRequest.Token) || string.IsNullOrEmpty(refreshTokenRequest.RefreshToken))
        {
            throw new InvalidTokenException("Token cannot be empty.");
        }
        ClaimsPrincipal principal = null;

        try
        { 
            principal = _tokenUtils.GetPrincipalFromExpiredToken(refreshTokenRequest.Token);
        }
        catch (Exception ex)
        {
            throw new InvalidTokenException("Invalid token provided.");
        }
        
        if (principal == null)
        {
            throw new InvalidTokenException("Invalid access token or refresh token");
        }

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            throw new InvalidTokenException("User Not Found"); 
        }

        // Commented out: no need to verify the refresh token with the two-factor provider for now
        /*var isValidRefreshToken =
            await _userManager.VerifyUserTokenAsync(user, "Default", "RefreshToken", refreshTokenRequest.RefreshToken);
        if (!isValidRefreshToken)
        {
            throw new InvalidTokenException("Invalid refreshToken");
        }*/
        
        var newAccessToken = _tokenUtils.GenerateJwtToken(user);
        var newRefreshToken = _tokenUtils.GenerateRefreshToken();

        await _userManager.SetAuthenticationTokenAsync(user, "Default", "RefreshToken", newRefreshToken);
        return new TokenResponse(newAccessToken,  newRefreshToken,"Token refreshed successfully.");

    }

}
