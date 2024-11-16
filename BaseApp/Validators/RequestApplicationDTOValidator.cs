using BaseApp.DTO;
using FluentValidation;

namespace BaseApp.Validators
{
    public class RequestApplicationDTOValidator : AbstractValidator<RequestApplicationDTO>
    {

        public RequestApplicationDTOValidator()
        {
            RuleFor(a => a.Title)
                .MinimumLength(2)
                .MaximumLength(100)
                .WithMessage("Application's name must be from 2 to 100 characters");

            RuleFor(a => a.Note)
                .MinimumLength(2)
                .WithMessage("Application's note must be 2 characters minimum");

            RuleFor(a => a.StartedDate)
                .GreaterThan(DateTime.Now)
                .LessThan(a => a.EndedDate)
                .WithMessage("Start date must be greater than current date and less than end date");
        }
    }
}
