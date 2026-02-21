using ActivityClub.Web.ViewModels.Auth;

namespace ActivityClub.Web.Services.Interfaces
{
    public interface IAuthUiService
    {
        Task<bool> LoginAsync(LoginVm vm, HttpResponse response, CancellationToken ct = default);
        Task RegisterAsync(RegisterVm vm, HttpResponse response, CancellationToken ct = default);
        void Logout(HttpResponse response);
        bool IsSignedIn(HttpRequest request);
    }
}
