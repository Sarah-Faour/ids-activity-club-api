using System.Net.Http.Headers;

namespace ActivityClub.Web.Handlers
{
    public sealed class JwtForwardingHandler : DelegatingHandler
    {
        private const string CookieName = "ac_jwt";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtForwardingHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null &&
                httpContext.Request.Cookies.TryGetValue(CookieName, out var token) &&
                !string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
