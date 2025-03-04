using FinanceManager.Models.Request;
using FluentValidation;

namespace FinanceManager.Application.Validator
{
    public class SavingsRequestValidator : AbstractValidator<SavingsRequest>
    {
        public SavingsRequestValidator()
        {
            RuleFor(s => s.Goal).NotEmpty().WithMessage("Goal cannot be empty.");

            RuleFor(s => s.TargetAmount).NotEmpty()
                .GreaterThan(0).WithMessage("{PropertyName} should be greater than 0");

            RuleFor(s => s.InitialBalance).NotEmpty()
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} should be greater than equal 0");

            RuleFor(s => s.TargetAmount).NotEmpty()
                .GreaterThan(0).WithMessage("{PropertyName} should be greater than 0");
        }
    }
}
