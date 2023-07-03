using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using WeatherApi.Data;

namespace WeatherApi.Authentication
{
    public class BasicAuthenticationOptions: AuthenticationSchemeOptions
    {
    }
    public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        private readonly UserProvider _userProvider;
        private record UserCredentials(string login, string password);
        
        public BasicAuthenticationHandler(
            UserProvider userProvider,
            IOptionsMonitor<BasicAuthenticationOptions> options, 
            ILoggerFactory logger, 
            UrlEncoder encoder, 
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _userProvider = userProvider;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var creds = RetrieveCredentials();
            if (creds == null)
                return AuthenticateResult.Fail("No credentials");

            var userData = _userProvider.GetUser(creds.login, creds.password);
            if (userData == null)
                return AuthenticateResult.Fail("No such user");

            if (userData.Password != creds.password)
                return AuthenticateResult.Fail("Invalid password");

            var principal = CreatePrincipal(userData);
            var ticket = new AuthenticationTicket(principal, "Basic");

            return AuthenticateResult.Success(ticket);
        }

        private UserCredentials RetrieveCredentials()
        {
            if (Context.Request.Headers.Authorization.Count == 0)
                return null;

            var basedValue = Context.Request.Headers.Authorization[0];
            if (basedValue.StartsWith("Basic "))
                basedValue = basedValue.Remove(0, "Basic ".Length);
            else
                return null;

            var byteData = Convert.FromBase64String(basedValue);
            var credsData = Encoding.UTF8.GetString(byteData);

            var credValues = credsData.Split(':');
            if (credValues == null || credValues.Length != 2)
                return null;

            return new UserCredentials(credValues[0], credValues[1]);
        }

        private ClaimsPrincipal CreatePrincipal(UserData user)
        {
            ClaimsIdentity identity = new ClaimsIdentity("Basic");
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));

            return new ClaimsPrincipal(identity);
        }
    }
}
