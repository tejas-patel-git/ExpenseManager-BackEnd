using FinanceManager.FunctionalTest.Configuration;
using System.Security.Claims;

namespace FinanceManager.FunctionalTest.AuthHandler
{
    public class AuthClaimsProvider
    {
        public IList<Claim> Claims { get; } = [];

        public AuthClaimsProvider(IList<Claim> claims)
        {
            Claims = claims;
        }

        public AuthClaimsProvider()
        {
            Claims.Add(new Claim(ClaimTypes.NameIdentifier, TestConstants.UserId));
        }
    }
}
