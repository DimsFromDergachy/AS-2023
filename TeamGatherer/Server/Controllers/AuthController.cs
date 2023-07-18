using Microsoft.AspNetCore.Mvc;
using TeamGatherer.Client.Models;
using TeamGatherer.Server.Adapters;

namespace TeamGatherer.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationAdapter _authenticationAdapter;

    public AuthController(IAuthenticationAdapter authenticationAdapter)
    {
        _authenticationAdapter = authenticationAdapter;
    }

    [HttpPost("register")]
    public async Task<string> Register(RegisterRequest request) => await _authenticationAdapter.RegisterUser(request);

    [HttpPost("login")]
    public async Task<string> Login(LoginRequest request) => await _authenticationAdapter.Login(request);
}