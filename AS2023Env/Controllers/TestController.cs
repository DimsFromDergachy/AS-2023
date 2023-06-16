using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AS2023Env.Controllers;

[ApiController]
[Route("/test")]
[SwaggerTag(Constants.TestControllerDescription)]
public class TestController : ControllerBase
{
    private readonly AuthService _authService;
    private static string _data = "Hello, world!";

    public TestController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpGet("GetData")]
    public string GetData()
    {
        if (!CheckAuth()) return null;
        return _data;
    }

    [HttpPost("PostData")]
    public string PostData(string data)
    {
        if (!CheckAuth()) return null;
        _data = data ?? "";
        return "ok";
    }

    private bool CheckAuth()
    {
        return Constants.CheckAuth(Request, Response, _authService, true);
    }
}