using AS2023Env.Models;
using Microsoft.AspNetCore.Mvc;

namespace AS2023Env.Controllers;

[ApiController]
[Route("/utils")]
public class UtilsController : ControllerBase
{
    private readonly IStorage<StaffUnit> _staffUnitStorage;
    private readonly IStorage<Employee> _employeeStorage;

    public UtilsController(IStorage<StaffUnit> staffUnitStorage, IStorage<Employee> employeeStorage)
    {
        _staffUnitStorage = staffUnitStorage;
        _employeeStorage = employeeStorage;
    }

    [HttpPost("FireEmployees")]
    public async Task<string> FireEmployee()
    {
        if (!Constants.IsAdmin)
        {
            Response.StatusCode = StatusCodes.Status403Forbidden;
            return null;
        }

        var continueFire = true;
        while (continueFire)
        {
            (Employee employee, StaffUnit staffUnit) = await Constants.FireEmployee(_staffUnitStorage, _employeeStorage);
            continueFire = employee != null && staffUnit != null;
        }
        return "Уволены!";
    }

    [HttpPost("CloseAllPending")]
    public async Task<string> CloseAllPending()
    {
        if (!Constants.IsAdmin)
        {
            Response.StatusCode = StatusCodes.Status403Forbidden;
            return null;
        }
        
        List<StaffUnit> pendingStaffUnits = await _staffUnitStorage.GetList(s => s.Status == StaffUnitStatus.Pending);
        foreach (StaffUnit staffUnit in pendingStaffUnits)
        {
            staffUnit.SetClosed();
        }

        return $"Закрыто {pendingStaffUnits.Count}";
    }
}