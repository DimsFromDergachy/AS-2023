using Microsoft.AspNetCore.Mvc;
using TeamGatherer.Shared;
using TeamGatherer.Shared.DTOs;
using TeamGatherer.Shared.ServerAdapterViewModels;
using TeamGatherer.Shared.ViewModels;

namespace TeamGatherer.Server.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class StaffController : ControllerBase
{
    private readonly StaffClientAdapter _staffClientAdapter;

    public StaffController(StaffClientAdapter staffClientAdapter)
    {
        _staffClientAdapter = staffClientAdapter;
    }

    [HttpGet]
    public async Task<EmployeeViewModel> GetEmployeeByIdAsync(string employeeId)
    {
        var adaptedEmployee = await _staffClientAdapter.GetAdapedEmployeeById(employeeId);

        if (adaptedEmployee is not { })
            throw new Exception("Сотрудник не найден");

        var result = new EmployeeViewModel()
        {
            Email = adaptedEmployee?.Email,
            FirstName = adaptedEmployee?.FirstName,
            Id = adaptedEmployee?.Id,
            LastName = adaptedEmployee?.LastName,
            Position = GetPosition(adaptedEmployee?.Position),
            StaffUnit = GetStaffUnit(adaptedEmployee?.StaffUnit),
        };

        return result;
    }

    [HttpGet]
    public async Task<List<EmployeeViewModel>> GetEmployeesAsync()
    {
        return await _staffClientAdapter.GetEmployeesAsync();
    }

    [HttpGet]
    public async Task<List<StaffUnitDto>> GetStaffUnitsAsync()
    {
        return await _staffClientAdapter.GetStaffUnitsAsync();
    }

    [HttpGet]
    public async Task<List<StaffUnitDto>> GetOpenStaffUnitsAsync()
    {
        return await _staffClientAdapter.GetStaffUnitsByStatusAsync("Opened");
    }

    [HttpGet]
    [Route("{staffUnitId?}")]
    public async Task<StaffUnitAdapterViewModel> GetStaffUnitByIdAsync([FromRoute] string staffUnitId)
    {
        return await _staffClientAdapter.GetAdaptedStaffUnitByIdAsync(staffUnitId);
    }

    [HttpGet]
    [Route("{positionId?}")]
    public async Task<PositionViewModel> GetPositionByIdAsync([FromRoute] string positionId)
    {
        var position = await _staffClientAdapter.GetPositionByIdAsync(positionId);
        return new PositionViewModel() { Id = position.Id, Name = position.Name };
    }

    [HttpGet]
    public async Task<List<PositionViewModel>> GetPositionsAsync()
    {
        return await _staffClientAdapter.GetPositionsAsync();
    }

    [HttpGet]
    public async Task<List<EmployeeViewModel>> GetHrsAsync()
    {
        return await _staffClientAdapter.GetHrEmployees();
    }


    static StaffUnitViewModel GetStaffUnit(StaffUnitAdapterViewModel staffUnit)
    {
        return staffUnit is null ? null
        : new StaffUnitViewModel()
        {
            Id = staffUnit?.Id,
            CloseTime = staffUnit?.CloseTime,
            EmployeeId = staffUnit?.EmployeeId,
            PositionId = staffUnit?.PositionId,
            Status = staffUnit?.Status.ToString(),
        };
    }

    static PositionViewModel GetPosition(PositionAdapterViewModel p)
    {

        return p is null ? null
        : new PositionViewModel()
        {
            Id = p?.Id,
            Name = p?.Name
        };
    }


}