using AS2023Env.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AS2023Env.Controllers;

[ApiController]
[Route("/staffUnits")]
[SwaggerTag("Штатные единицы")]
public class StaffUnitsController : BaseController<StaffUnit>
{
    public StaffUnitsController(IStorage<StaffUnit> storage, AuthService authService) : base(storage, authService) { }

    [HttpGet("List/{status}")]
    [SwaggerOperation("Получить список штатных единиц по статусу")]
    public async Task<List<StaffUnit>> StaffUnitListAsync(string status)
    {
        if (!CheckAuth()) return null;
        if (!Enum.TryParse(status, out StaffUnitStatus st))
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return null;
        }

        return await Storage.GetList(s => s.Status == st);
    }
}