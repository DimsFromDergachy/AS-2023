using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Swashbuckle.AspNetCore.Annotations;

namespace AS2023Env.Controllers;

[ApiController]
public abstract class BaseController<TEntity> : ControllerBase where TEntity: class, new()
{
    protected readonly IStorage<TEntity> Storage;
    private readonly AuthService _authService;

    protected BaseController(IStorage<TEntity> storage, AuthService authService)
    {
        Storage = storage;
        _authService = authService;
    }
    
    [HttpGet("List")]
    [SwaggerOperation("Получить полный список")]
    public async Task<List<TEntity>> List()
    {
        if (!CheckAuth()) return null;
        return await Storage.GetList();
    }
    
    [HttpGet("{id}")]
    [SwaggerOperation("Получить один элемент")]
    public async Task<TEntity> Id(string id)
    {
        if (!CheckAuth()) return null;
        TEntity result = await Storage.ById(id);
        if (result == null)
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
        }

        return result;
    }

    protected bool CheckAuth()
    {
        bool success = Request.Headers.TryGetValue("Authorization", out StringValues auth) && _authService.Check(auth);
        if (!success)
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
        }

        return success;
    }
}