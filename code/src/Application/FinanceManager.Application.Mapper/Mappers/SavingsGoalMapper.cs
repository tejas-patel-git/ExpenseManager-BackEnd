using FinanceManager.Data.Models;
using FinanceManager.Domain.Abstraction.Mappers;
using FinanceManager.Domain.Models;
using FinanceManager.Models.Request;
using FinanceManager.Models.Response;

namespace FinanceManager.Application.Mapper.Mappers
{
    public class SavingsRequestToDomainMapper : BaseMapper<SavingsRequest, SavingsGoalDomain>
    {
        public SavingsRequestToDomainMapper()
            : base(source => new()
            {
                Goal = source.Goal,
                CurrentBalance = source.CurrentBalance,
                InitialBalance = source.InitialBalance,
                TargetAmount = source.TargetAmount
            })
        {
        }
    }

    public class SavingsGoalDomainToEntityMapper : BaseMapper<SavingsGoalDomain, SavingsGoal>
    {
        public SavingsGoalDomainToEntityMapper()
            : base(source => new SavingsGoal
            {
                Id = source.Id,
                UserId = source.UserId,
                Goal = source.Goal,
                CurrentBalance = source.CurrentBalance,
                InitialBalance = source.InitialBalance,
                TargetAmount = source.TargetAmount
            })
        {
        }
    }

    public class SavingsGoalToDomainMapper : BaseMapper<SavingsGoal, SavingsGoalDomain>
    {
        public SavingsGoalToDomainMapper()
            : base(source => new SavingsGoalDomain
            {
                Id = source.Id,
                UserId = source.UserId,
                Goal = source.Goal,
                CurrentBalance = source.CurrentBalance,
                InitialBalance = source.InitialBalance,
                TargetAmount = source.TargetAmount,
            })
        {
        }
    }

    public class SavingsDomainToResponseMapper : BaseMapper<SavingsGoalDomain, SavingsResponse>
    {
        public SavingsDomainToResponseMapper()
            : base(source => new()
            {
                Id = source.Id,
                Goal = source.Goal,
                CurrentBalance = source.CurrentBalance,
                InitialBalance = source.InitialBalance,
                TargetAmount = source.TargetAmount
            })
        {
        }
    }
}
