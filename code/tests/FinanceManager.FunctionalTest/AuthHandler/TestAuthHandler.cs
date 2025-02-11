using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace FinanceManager.FunctionalTest.AuthHandler
{
    public class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    AuthClaimsProvider claimsProvider) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        public static string SchemeName = "Test";
        private readonly IList<Claim> _claims = claimsProvider.Claims;

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var identity = new ClaimsIdentity(_claims, SchemeName);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SchemeName);

            var result = AuthenticateResult.Success(ticket);

            return Task.FromResult(result);
        }
    }
}
