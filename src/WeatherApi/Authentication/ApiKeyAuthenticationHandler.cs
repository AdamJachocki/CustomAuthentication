using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;
using System.Text.Encodings.Web;
using WeatherApi.Data;

namespace WeatherApi.Authentication
{
    class ApiKeyAuthenticationDefaults
    {
        public const string SchemeName = "ApiKey";
    }
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public int ApiKeyLength { get; set; }
        public bool CheckApiKeyLength { get; set; }
    }

    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private readonly ApiKeyClientProvider _clientProvider;
        public ApiKeyAuthenticationHandler(
            ApiKeyClientProvider clientProvider,
            IOptionsMonitor<ApiKeyAuthenticationOptions> options, 
            ILoggerFactory logger, 
            UrlEncoder encoder, 
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _clientProvider = clientProvider;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var apiKey = GetApiKey();
            if (string.IsNullOrWhiteSpace(apiKey))
                return AuthenticateResult.NoResult();

            if (Options.CheckApiKeyLength)
            {
                if (apiKey.Length != Options.ApiKeyLength)
                    return AuthenticateResult.Fail("Invalid API key");
            }

            var client = _clientProvider.GetClient(apiKey);
            if (client == null)
                return AuthenticateResult.Fail("Invalid API key");

            var principal = CreatePrincipal(client);

            AuthenticationTicket ticket = new AuthenticationTicket(principal, "ApiKey");
            return AuthenticateResult.Success(ticket);
        }

        //protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        //{
        //    Response.Headers.WWWAuthenticate = new StringValues("X-API-KEY");
        //    return Task.CompletedTask;
        //}

        private string GetApiKey()
        {
            StringValues keyValue;
            if (!Context.Request.Headers.TryGetValue("X-API-KEY", out keyValue))
                return null;

            if (!keyValue.Any())
                return null;

            return keyValue.ElementAt(0);
        }

        private ClaimsPrincipal CreatePrincipal(ApiKeyClient client)
        {
            ClaimsIdentity identity = new ClaimsIdentity("ApiKey");
            identity.AddClaim(new Claim(ClaimTypes.Email, client.Email));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, client.Id.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Name, client.Name));

            return new ClaimsPrincipal(identity);
        }
    }
}
