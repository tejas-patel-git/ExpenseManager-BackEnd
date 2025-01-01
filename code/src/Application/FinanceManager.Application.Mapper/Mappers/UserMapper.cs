using FinanceManager.Data.Models;
using FinanceManager.Domain.Abstraction.Mappers;
using FinanceManager.Domain.Models;
using FinanceManager.Models.Request;

namespace FinanceManager.Application.Mapper.Mappers
{
    public class UserRegistrationRequestToDomainMapper : BaseMapper<UserRegistrationRequest, UserDomain>
    {
        public UserRegistrationRequestToDomainMapper()
            : base(source => new()
            {
                UserId = source.UserId,
                Email = source.Email,
                PhoneNumber = source.PhoneNumber,
                CreatedAt = source.CreatedAt,
                IsEmailVerified = source.IsEmailVerified,
                FamilyName = source.FamilyName,
                GivenName = source.GivenName,
                LastPasswordReset = source.LastPasswordReset,
                FullName = source.FullName,
                Nickname = source.Nickname,
                IsPhoneVerified = source.IsPhoneVerified,
                PictureUrl = source.PictureUrl,
                UpdatedAt = source.UpdatedAt,
                AppMetadata = source.AppMetadata,
                UserMetadata = source.UserMetadata
            })
        {
        }
    }

    public class UserDomainToEntityMapper : BaseMapper<UserDomain, User>
    {
        public UserDomainToEntityMapper()
            : base(source => new()
            {
                Auth0UserId = source.UserId,
                Email = source.Email,
                PhoneNumber = source.PhoneNumber,
                CreatedAt = source.CreatedAt,
                IsEmailVerified = source.IsEmailVerified,
                FamilyName = source.FamilyName,
                GivenName = source.GivenName,
                LastPasswordReset = source.LastPasswordReset,
                FullName = source.FullName,
                Nickname = source.Nickname,
                IsPhoneVerified = source.IsPhoneVerified,
                PictureUrl = source.PictureUrl,
                UpdatedAt = source.UpdatedAt,
                AppMetadata = source.AppMetadata,
                UserMetadata = source.UserMetadata
            })
        {
        }
    }

    public class UserEntityToDomainMapper : BaseMapper<User, UserDomain>
    {
        public UserEntityToDomainMapper()
            : base(source => new()
            {
                Id = source.Id,
                Email = source.Email,
                PhoneNumber = source.PhoneNumber,
                CreatedAt = source.CreatedAt,
                IsEmailVerified = source.IsEmailVerified,
                FamilyName = source.FamilyName,
                GivenName = source.GivenName,
                LastPasswordReset = source.LastPasswordReset,
                FullName = source.FullName,
                Nickname = source.Nickname,
                IsPhoneVerified = source.IsPhoneVerified,
                PictureUrl = source.PictureUrl,
                UpdatedAt = source.UpdatedAt,
                AppMetadata = source.AppMetadata,
                UserMetadata = source.UserMetadata
            })
        {
        }
    }
}
