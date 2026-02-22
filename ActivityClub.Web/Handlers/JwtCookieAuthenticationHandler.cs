using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace ActivityClub.Web.Handlers
{
    public sealed class JwtCookieAuthenticationHandler
        : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private const string CookieName = "ac_jwt";
        private readonly IConfiguration _configuration;

        public JwtCookieAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory loggerFactory,
            System.Text.Encodings.Web.UrlEncoder encoder,
            IConfiguration configuration)
            : base(options, loggerFactory, encoder)
        {
            _configuration = configuration;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // 1) Read token from cookie
            if (!Request.Cookies.TryGetValue(CookieName, out var token) || string.IsNullOrWhiteSpace(token))
                return Task.FromResult(AuthenticateResult.NoResult());

            // 2) Read JWT settings
            var jwtSection = _configuration.GetSection("Jwt");
            var key = jwtSection["Key"];
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];

            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
                return Task.FromResult(AuthenticateResult.Fail("JWT settings missing in ActivityClub.Web appsettings.json"));

            // 3) Validate token
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),

                ValidateIssuer = true,
                ValidIssuer = issuer,

                ValidateAudience = true,
                ValidAudience = audience,

                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30)
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            catch
            {
                // token expired/invalid => treat as logged out
                return Task.FromResult(AuthenticateResult.NoResult());
            }
        }
    }
}