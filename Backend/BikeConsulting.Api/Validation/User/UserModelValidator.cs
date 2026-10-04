using BikeConsulting.Api.Model.User;
using FluentValidation;

namespace BikeConsulting.Api.Validation.User
{
    public class UserModelValidator : AbstractValidator<UserModel>
    {
        public UserModelValidator()
        {
            RuleFor(user => user.Username).NotEmpty().WithMessage("User Name is required.");
            RuleFor(user => user.Email).NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(user => user.PasswordHash).NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
        }
    }
}
