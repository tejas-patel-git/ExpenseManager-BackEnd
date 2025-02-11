using System.Security.Claims;

namespace FinanceManager.FunctionalTest.AuthHandler
{
    public class AuthClaimsProvider
    {
        public IList<Claim> Claims { get; } = [];
        public static readonly string UserId = Guid.NewGuid().ToString();

        public AuthClaimsProvider(IList<Claim> claims)
        {
            Claims = claims;
        }

        public AuthClaimsProvider()
        {
            Claims.Add(new Claim(ClaimTypes.NameIdentifier, UserId));
        }
    }
}
