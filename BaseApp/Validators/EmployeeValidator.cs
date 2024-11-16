using BaseApp.DTO;
using BaseApp.Models;
using FluentValidation;
namespace BaseApp.Validators
{
    public class EmployeeValidator : AbstractValidator<RequestEmployeeDTO>
    {

        public EmployeeValidator()
        {

            RuleFor(emp => emp.Email)
                .EmailAddress()
                .WithMessage("Mail must be an email address");

            // vietnam or japan phone number regex
            string regex = regex = @"^(?:\+84|0)(3[2-9]|5[6|8|9]|7[0|6-9]|8[1-6|8|9]|9[0-9])[0-9]{7}$|^(?:\+81|0)\d{1,4}\d{1,4}\d{4}$";

            RuleFor(emp => emp.PhoneNumber)
                .Matches(regex)
                .WithMessage("Phone number must matches vietnam or japan phone number");

            RuleFor(emp => emp.FullName)
                .MinimumLength(2)
                .WithMessage("Name must be at least 2 char");

            RuleFor(emp => emp.GithubName)
                .MinimumLength(2)
                .WithMessage("Github name must be at least 2 char");
        }

    }
}
