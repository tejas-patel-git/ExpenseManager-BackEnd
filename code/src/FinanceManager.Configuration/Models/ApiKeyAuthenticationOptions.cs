using Microsoft.AspNetCore.Authentication;

namespace FinanceManager.Configuration.Models
{
    internal class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "ApiKey";
        public string Scheme { get; set; } = DefaultScheme;
        public string ApiKeyHeaderName { get; set; } = "X-Api-Key";
    }
}
