using System.Net;
using AS2023Env.Models;
using Microsoft.AspNetCore.Mvc;

namespace AS2023Env.Controllers;

[ApiController]
[Route("/employee")]
public class EmployeeController : ControllerBase
{
    private readonly IStorage<Employee> _employeeStorage;
    private readonly IStorage<StaffUnit> _staffUnitStorage;

    public EmployeeController(IStorage<Employee> employeeStorage, IStorage<StaffUnit> staffUnitStorage)
    {
        _employeeStorage = employeeStorage;
        _staffUnitStorage = staffUnitStorage;
    }

    [HttpGet("GetEmployeeList")]
    public async Task<List<Employee>> GetEmployeeListAsync(string positionId)
    {
        if (string.IsNullOrEmpty(positionId))
        {
            return await _employeeStorage.GetList();
        }
        else
        {
            return await _employeeStorage.GetList(e => e.PositionId == positionId);
        }
    }

    [HttpPost("RegisterEmployee")]
    public async Task<RegisterEmployeeResult> RegisterEmployeeAsync(RegisterEmployeeRequest request)
    {
        StaffUnit unit = (await _staffUnitStorage.GetList(u => u.Id == request.StaffUnitId)).FirstOrDefault();
        if (unit == null)
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return new RegisterEmployeeResult(false, $"Штатная единица с id = {request.StaffUnitId} не найдена");
        }
        if (!unit.Active)
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return new RegisterEmployeeResult(false, $"Штатная единица с id = {request.StaffUnitId} занята");
        }

        var newEmp = new Employee
        {
            Id = Guid.NewGuid().ToString(),
            PositionId = unit.PositionId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email
        };
        await _employeeStorage.Add(newEmp);
        unit.EmployeeId = newEmp.Id;
        await _staffUnitStorage.Update(unit);

        return new RegisterEmployeeResult(true, newEmp.Id);
    }
}