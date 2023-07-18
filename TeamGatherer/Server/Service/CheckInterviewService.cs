using TeamGatherer.Shared.ViewModels;
using TeamGatherer.Shared;
using TeamGatherer.Server.Data;

using Microsoft.EntityFrameworkCore;
using TeamGatherer.Server;


namespace TeamGatherer.Server.Service;

public class CheckInterviewService
{
    private readonly IServiceScopeFactory _sp;

    public CheckInterviewService(IServiceScopeFactory sp)
    {
        _sp = sp;
    }

    // Вернём тех сотрудников у кого пересечение по другим интервью
    public async Task<IEnumerable<EmployeeViewModel>> CheckInterviewDateTime()
    {
        await using (var scope = _sp.CreateAsyncScope())
        {
            var _adapter = scope.ServiceProvider.GetRequiredService<StaffClientAdapter>();
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var interviews = await _context.Interviews.ToListAsync();

            var employees = await _adapter.GetEmployeesAsync();
        }

        return Enumerable.Empty<EmployeeViewModel>();
    }
}
