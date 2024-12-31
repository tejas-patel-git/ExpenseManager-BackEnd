using FinanceManager.Configuration.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace FinanceManager.Configuration.AuthHandlers
{
    internal class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private const string PredefinedApiKey = "YOUR_SECURE_API_KEY";

        public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthenticationOptions> options,
                                           ILoggerFactory logger,
                                           UrlEncoder encoder,
                                           ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(Options.ApiKeyHeaderName, out var apiKeyHeaderValues))
            {
                return Task.FromResult(AuthenticateResult.Fail("API Key was not provided."));
            }

            var providedApiKey = apiKeyHeaderValues.FirstOrDefault();
            if (providedApiKey == null || !providedApiKey.Equals(PredefinedApiKey))
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid API Key."));
            }

            var claims = new[] { new Claim(ClaimTypes.Name, "Auth0User") };
            var identity = new ClaimsIdentity(claims, Options.Scheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Options.Scheme);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
