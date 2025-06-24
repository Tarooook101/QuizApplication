using FluentValidation;
using QuizApp.Application.Auth.Commands;
using QuizApp.Application.Common.Constants;

namespace QuizApp.Application.Auth.Commands.Validators;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(ApplicationConstants.Validation.MaxNameLength);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(ApplicationConstants.Validation.MaxNameLength);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(ApplicationConstants.Validation.MaxEmailLength);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(ApplicationConstants.Validation.MinPasswordLength)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$")
            .WithMessage("Password must contain at least one lowercase letter, one uppercase letter, and one digit");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .Equal(x => x.Password)
            .WithMessage("Passwords do not match");
    }
}