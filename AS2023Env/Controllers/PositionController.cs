using AS2023Env.Models;
using Microsoft.AspNetCore.Mvc;

namespace AS2023Env.Controllers;

[ApiController]
[Route("/position")]
public class PositionController : ControllerBase
{
    private readonly IStorage<Position> _storage;

    public PositionController(IStorage<Position> storage)
    {
        _storage = storage;
    }
    
    [HttpGet("GetPositionList")]
    public Task<List<Position>> GetPositionList()
    {
        return _storage.GetList();
    }
}