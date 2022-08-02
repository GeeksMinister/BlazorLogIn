using BlazorLogIn.Models;

namespace BlazorLogIn.Authentication
{
    public interface IAuthenticationService
    {
        Task<AuthenticatedUserModel> Login(AuthenticationUserModel userForAuthentication);
        Task Logout();
    }
}