using Microsoft.EntityFrameworkCore;

using TeamGatherer.Server.Data;
using TeamGatherer.Shared;
using TeamGatherer.Shared.Extensions;

namespace TeamGatherer.Server
{
    public class InterviewRejectorBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _sp;

        public InterviewRejectorBackgroundService(IServiceScopeFactory sp)
        {
            _sp = sp;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true && !stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await using (var scope = _sp.CreateAsyncScope())
                    {
                        var _adapter = scope.ServiceProvider.GetRequiredService<StaffClientAdapter>();
                        var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                        var interviews = await _context.Interviews.ToListAsync();

                        var employees = await _adapter.GetEmployeesAsync();

                        foreach (var interview in interviews)
                        {
                            var hasHrs = employees.Any(h => interview.HrIds.Contains(h.Id));
                            var hasExperts = employees.Any(h => interview.ExpertIds.Contains(h.Id));

                            if (!hasHrs || !hasExperts)
                            {
                                interview.Status = InterviewStatus.Rejected;
                            }

                            if(interview.EndTimestamp < DateTime.UtcNow.ToUnixTimestamp())
                            {
                                interview.Status = InterviewStatus.Finished;
                            }
                        }

                        await _context.SaveChangesAsync();
                    }
                }
                catch
                {
                }
                finally
                {

                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
        }
    }
}
