using Microsoft.EntityFrameworkCore;

using TeamGatherer.Server.Data;
using TeamGatherer.Shared;

namespace TeamGatherer.Server
{
    public class VacancyCloserBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _sp;

        public VacancyCloserBackgroundService(IServiceScopeFactory sp)
        {
            _sp = sp;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true && !stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await using AsyncServiceScope scope = _sp.CreateAsyncScope();
                    StaffClientAdapter _adapter = scope.ServiceProvider.GetRequiredService<StaffClientAdapter>();
                    ApplicationDbContext _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    List<Models.Vacancy> vacancies = await _context.Vacancies.Where(v => !v.IsClosed).ToListAsync();

                    foreach (Models.Vacancy vacancy in vacancies)
                    {
                        Shared.ServerAdapterViewModels.StaffUnitAdapterViewModel stuffUnit = await _adapter.GetAdaptedStaffUnitByIdAsync(vacancy.StaffUnitId);

                        if (stuffUnit is null or { Status: "Closed" })
                        {
                            vacancy.IsClosed = true;
                        }
                    }

                    _ = await _context.SaveChangesAsync();
                }
                catch
                {
                }
                finally
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }
    }
}
