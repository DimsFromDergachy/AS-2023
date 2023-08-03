using TeamGatherer.Client.Models;

namespace TeamGatherer.Client.Services;

public interface IAuthenticationService
{
    Task<string> Login(LoginRequest loginRequest);
    Task Logout();
    Task<string> Register(RegisterRequest registerRequest);
}