using ActivityClub.Contracts.DTOs.Auth;
using ActivityClub.Web.Services.Interfaces;
using ActivityClub.Web.ViewModels.Auth;

namespace ActivityClub.Web.Services.Implementations
{
    public sealed class AuthUiService : IAuthUiService
    {
        private const string JwtCookieName = "ac_jwt";
        private readonly IAuthApiClient _authApiClient;

        public AuthUiService(IAuthApiClient authApiClient)
        {
            _authApiClient = authApiClient;
        }

        public async Task<bool> LoginAsync(LoginVm vm, HttpResponse response, CancellationToken ct = default)
        {
            var dto = new LoginRequestDto
            {
                Email = vm.Email,
                Password = vm.Password
            };

            var auth = await _authApiClient.LoginAsync(dto, ct);

            if (auth is null)
                return false;

            WriteJwtCookie(response, auth.Token, auth.ExpiresAtUtc);
            return true;
        }

        public async Task RegisterAsync(RegisterVm vm, HttpResponse response, CancellationToken ct = default)
        {
            if (vm.GenderLookupId is null)
                throw new InvalidOperationException("Gender is required.");

            var dto = new RegisterRequestDto
            {
                Name = vm.Name,
                Email = vm.Email,
                Password = vm.Password,
                DateOfBirth = vm.DateOfBirth,
                GenderLookupId = vm.GenderLookupId.Value
            };

            var auth = await _authApiClient.RegisterAsync(dto, ct);

            // API returns token (auto-login) ✅
            if (auth is not null)
                WriteJwtCookie(response, auth.Token, auth.ExpiresAtUtc);
        }

        public void Logout(HttpResponse response)
        {
            response.Cookies.Delete(JwtCookieName);
        }

        public bool IsSignedIn(HttpRequest request)
        {
            return request.Cookies.ContainsKey(JwtCookieName);
        }

        private static void WriteJwtCookie(HttpResponse response, string token, DateTime expiresAtUtc)
        {
            response.Cookies.Append(JwtCookieName, token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,              // https localhost
                SameSite = SameSiteMode.Lax,
                Expires = expiresAtUtc
            });
        }
    }
}