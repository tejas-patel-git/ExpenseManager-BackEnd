using FinanceManager.Domain.Enums;
using FinanceManager.Models.Request;
using FluentValidation;

namespace FinanceManager.Application.Validator
{
    public class TransactionRequestValidator : AbstractValidator<TransactionRequest>
    {
        public TransactionRequestValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0m)
                .WithMessage($"{nameof(TransactionRequest.Amount)} must be greater than 0.");

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .When(x => x.Description != null)
                .WithMessage($"{nameof(TransactionRequest.Description)} cannot exceed 500 characters.");

            RuleFor(x => x.Date)
                .NotEmpty()
                .WithMessage($"{nameof(TransactionRequest.Date)} is required.");

            RuleFor(x => x.IsExpense)
                .NotEmpty()
                .WithMessage($"{nameof(TransactionRequest.IsExpense)} is required.");

            RuleFor(x => x.Type)
                .IsInEnum()
                .NotEqual(TransactionType.Undefined)
                .WithMessage($"A valid transaction {nameof(TransactionRequest.Type)} is required.");

            // Payments validation: required for non-Savings, must match Amount
            RuleFor(x => x.Payments)
                // required
                .NotNull()
                .WithMessage("{PropertyName} is required for non-Savings transactions.")
                // rules for Payments model properties
                .ChildRules(payment =>
                {
                    payment.RuleFor(p => p.Accounts).NotEmpty().WithMessage("{PropertyName} should have atleast one account.");
                    payment.RuleForEach(p => p.Accounts)
                           .ChildRules(account =>
                           {
                               account.RuleFor(a => a.Amount).GreaterThan(0).WithMessage("{PropertyName} should be greater than 0");
                           });
                })
                // sum of payments account should be equal to transaction amount
                .Must((t, p) => p.Accounts.Sum(a => a.Amount) == t.Amount)
                .WithMessage("The sum of payment account amounts must match the transaction amount.")
                .When(x => x.Type != TransactionType.Savings);

            // SavingGoal validation: required for Savings
            RuleFor(x => x.SavingGoal)
                .NotEmpty()
                .When(x => x.Type == TransactionType.Savings)
                .WithMessage($"{nameof(TransactionRequest.SavingGoal)} is required for Savings transactions.")
                .MaximumLength(50)
                .When(x => x.SavingGoal != null)
                .WithMessage($"{nameof(TransactionRequest.SavingGoal)} cannot exceed 50 characters.");
        }
    }
}
