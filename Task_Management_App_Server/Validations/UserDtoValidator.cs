using System.Data;
using FluentValidation;
using Task_Management_App.DTO;

namespace Task_Management_App.Validations;

public class UserDtoValidator : AbstractValidator<UserDto>
{
    public UserDtoValidator()
    {
        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(user => user.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowerCase letter")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number.");
        RuleFor(user => user.FullName)
            .NotEmpty().WithMessage("Full name is required.");
        
    }
}