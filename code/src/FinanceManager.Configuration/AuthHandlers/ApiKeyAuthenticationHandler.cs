using FinanceManager.Configuration.Models;
using FinanceManager.Models.Response;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace FinanceManager.Configuration.AuthHandlers
{
    internal class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private readonly string _predefinedApiKey;

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<ApiKeyAuthenticationOptions> options,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            IConfiguration configuration) // Inject configuration for API key
            : base(options, loggerFactory, encoder)
        {
            _predefinedApiKey = configuration.GetValue<string>("Auth0Provider:ValidApiKey")
                                ?? throw new ArgumentNullException("Auth0Provider:ValidApiKey", "API key is not configured.");
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            Logger.LogDebug("Starting API Key authentication.");

            // Check if the API key header is present
            if (!Request.Headers.TryGetValue(Options.ApiKeyHeaderName, out var apiKeyHeaderValues))
            {
                Logger.LogDebug("API Key header '{HeaderName}' is missing.", Options.ApiKeyHeaderName);
                return Task.FromResult(AuthenticateResult.Fail("API Key header is missing."));
            }

            var providedApiKey = apiKeyHeaderValues.FirstOrDefault();
            if (string.IsNullOrEmpty(providedApiKey))
            {
                Logger.LogDebug("API Key is empty in the request.");
                return Task.FromResult(AuthenticateResult.Fail("API Key is empty."));
            }

            // Validate the API key
            if (!providedApiKey.Equals(_predefinedApiKey))
            {
                Logger.LogDebug("Invalid API Key provided: {ApiKey}", providedApiKey);
                return Task.FromResult(AuthenticateResult.Fail("Invalid API Key."));
            }

            Logger.LogDebug("API Key authentication successful.");

            // Create claims
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "Auth0User"),
                new Claim(ClaimTypes.Role, "ApiKeyUser"),
                new Claim("AuthenticatedWith", "ApiKey")
            };

            var identity = new ClaimsIdentity(claims, Options.Scheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Options.Scheme);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Logger.LogDebug("Authentication challenge triggered.");
            var authResult = HandleAuthenticateOnceAsync();

            Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            var response = new Response
            {
                Success = false,
                ErrorMessage = authResult.Result.Failure?.Message ?? "Authentication failed."
            };

            return Response.WriteAsJsonAsync(response);
        }
    }
}
