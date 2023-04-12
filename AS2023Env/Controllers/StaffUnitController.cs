using AS2023Env.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AS2023Env.Controllers;

[ApiController]
[Route("/staffUnit")]
public class StaffUnitController : ControllerBase
{
    private readonly IStorage<StaffUnit> _storage;

    public StaffUnitController(IStorage<StaffUnit> storage)
    {
        _storage = storage;
    }

    [HttpGet("GetStaffUnitList")]
    [SwaggerOperation("Получить список штатных единиц")]
    public async Task<List<StaffUnit>> GetStaffUnitListAsync(bool onlyActive)
    {
        if (!onlyActive)
        {
            return await _storage.GetList();
        }
        else
        {
            return await _storage.GetList(s => s.Active);
        }
    }
}