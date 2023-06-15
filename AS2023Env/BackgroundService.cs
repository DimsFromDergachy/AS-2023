using AS2023Env.Models;

namespace AS2023Env;

public class BackgroundService : IHostedService
{
    private readonly IStorage<StaffUnit> _staffUnitStorage;
    private readonly IStorage<Employee> _employeeStorage;
    private readonly ILogger<BackgroundService> _logger;

    private Timer _fireEmployeeTimer = null;
    private Timer _resetStatusesTimer = null;

    public BackgroundService(IStorage<StaffUnit> staffUnitStorage, IStorage<Employee> employeeStorage, ILogger<BackgroundService> logger)
    {
        _staffUnitStorage = staffUnitStorage;
        _employeeStorage = employeeStorage;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _fireEmployeeTimer = new Timer(FireEmployee, null, Constants.FireEmployeeDelay, Constants.FireEmployeeDelay);
        _resetStatusesTimer = new Timer(ResetStatuses, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        _logger.LogInformation("Фоновый сервис стартовал");
        return Task.CompletedTask;
    }

    private async void ResetStatuses(object state)
    {
        List<StaffUnit> pendingStaffUnits = await _staffUnitStorage
            .GetList(s => s.Status == StaffUnitStatus.Pending && s.CloseTime != null && s.CloseTime <= DateTime.Now);

        foreach (StaffUnit staffUnit in pendingStaffUnits)
        {
            staffUnit.SetClosed();
            _logger.LogInformation(
                "Для штатной единицы {Id} установлен статус {Status}",
                staffUnit.Id,
                staffUnit.Status.ToString()
            );

            await _staffUnitStorage.Update(staffUnit);
        }
    }

    private async void FireEmployee(object _)
    {
        (Employee employee, StaffUnit staffUnit) = await Constants.FireEmployee(_staffUnitStorage, _employeeStorage);
        _logger.LogInformation(
            "{EmployeeId} уволен, штатная единица {StaffUnitId} освобождена",
            employee.Id,
            staffUnit?.Id
        );
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _fireEmployeeTimer.Change(Timeout.Infinite, 0);
        _resetStatusesTimer.Change(Timeout.Infinite, 0);
        _logger.LogInformation("Фоновый сервис останавливается");
        return Task.CompletedTask;
    }
}