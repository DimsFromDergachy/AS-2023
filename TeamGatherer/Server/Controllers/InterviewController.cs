using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using TeamGatherer.Client.Pages;
using TeamGatherer.Server.Data;
using TeamGatherer.Server.Models;
using TeamGatherer.Shared;
using TeamGatherer.Shared.Extensions;
using TeamGatherer.Shared.Requests;
using TeamGatherer.Shared.ServerAdapterViewModels;
using TeamGatherer.Shared.ViewModels;

namespace TeamGatherer.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class InterviewController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly StaffClientAdapter _adapter;

        public InterviewController(ApplicationDbContext context, StaffClientAdapter adapter)
        {
            _context = context;
            _adapter = adapter;
        }

        [HttpGet]
        [Route("{interviewId?}")]
        public async Task<InterviewViewModel> GetInterviewByIdAsync(int interviewId)
        {
            Interview interview = await _context.Interviews.FirstOrDefaultAsync(f => f.Id == interviewId);
            if (interview is null)
            {
                throw new Exception("Не найдено собеседование");
            }

            var empIds = interview.HrIds.Union(interview.ExpertIds).ToArray();
            List<EmployeeAdapterViewModel> employees = await _adapter.GetAdaptedEmployeesByIdsAsync(empIds);

            InterviewViewModel res = BuildinterviewVM(employees, interview);

            return res;
        }

        [HttpGet]
        [Route("{interviewId?}")]
        public async Task<bool> CancelInterviewByIdAsync([FromRoute] int interviewId)
        {
            Interview interview = await _context.Interviews.FirstOrDefaultAsync(f => f.Id == interviewId);
            if (interview is null)
            {
                throw new Exception("Не найдено собеседование");
            }

            interview.Status = InterviewStatus.Rejected;
            _ = await _context.SaveChangesAsync();

            return true;
        }

        [HttpGet]
        [Route("{vacancyId?}")]
        public async Task<List<InterviewViewModel>> GetInterviewForVacancyIdAsync([FromRoute] int vacancyId)
        {
            List<Interview> interviews = await _context.Interviews.Where(f => f.VacancyId == vacancyId).ToListAsync();

            var empIds = interviews.SelectMany(i => i.HrIds).Union(interviews.SelectMany(i => i.ExpertIds)).ToArray();

            List<EmployeeAdapterViewModel> employees = await _adapter.GetAdaptedEmployeesByIdsAsync(empIds);

            List<InterviewViewModel> resultVms = new();
            foreach (Interview interview in interviews)
            {
                InterviewViewModel result = BuildinterviewVM(employees, interview);

                resultVms.Add(result);
            }

            return resultVms;
        }

        private static InterviewViewModel BuildinterviewVM(List<EmployeeAdapterViewModel> employees, Interview interview)
        {
            IEnumerable<EmployeeViewModel> hrs = employees.Where(e => e is not null && interview.HrIds.Contains(e.Id)).Select(e => new EmployeeViewModel()
            {
                Id = e.Id,
                Email = e.Email,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Position = GetPosition(e?.Position),
                StaffUnit = GetStaffUnit(e?.StaffUnit),
            });

            IEnumerable<EmployeeViewModel> experts = employees.Where(e => e is not null && interview.ExpertIds.Contains(e.Id)).Select(e => new EmployeeViewModel()
            {
                Id = e.Id,
                Email = e.Email,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Position = GetPosition(e?.Position),
                StaffUnit = GetStaffUnit(e?.StaffUnit),
            });

            InterviewViewModel result = new()
            {
                Candidate = new CandidateViewModel()
                {
                    Email = interview?.Candidate?.Email,
                    FIO = interview?.Candidate?.FIO,

                },
                Experts = experts.ToList(),
                Hrs = hrs.ToList(),
                EndTime = interview.EndTimestamp.FromUnixTimestamp(),
                Id = interview.Id,
                Location = interview.Location,
                StartTime = interview.StartTimestamp.FromUnixTimestamp(),
                Status = interview.Status.ToString(),
                VacancyId = interview.VacancyId,
            };
            return result;
        }

        [HttpPost]
        public async Task<int> CreateInterview(CreateInterviewRequest request)
        {
            Interview interview = new()
            {
                Candidate = new() { Email = request.Candidate.Email, FIO = request.Candidate.Email },
                ExpertIds = request.ExpertIds,
                HrIds = request.HrIds,
                EndTimestamp = request.EndTime.ToUnixTimestamp(),
                Location = request.Location,
                StartTimestamp = request.StartTime.ToUnixTimestamp(),
                Status = InterviewStatus.Created,
                VacancyId = request.VacancyId,
            };

            _ = await _context.Interviews.AddAsync(interview);
            _ = await _context.SaveChangesAsync();

            return interview.Id;
        }

        [HttpPost]
        public async Task UpdateInterviewAsync(UpdateInterviewRequest request)
        {
            Interview interview = await _context.Interviews.FirstOrDefaultAsync(f => f.Id == request.InterviewId);
            if (interview is null)
            {
                throw new Exception("Не найдено собеседование");
            }

            interview.Location = request.Location;
            interview.Candidate = new() { Email = request.Candidate.Email, FIO = request.Candidate.Email };
            interview.ExpertIds = request.ExpertIds;
            interview.HrIds = request.HrIds;
            interview.EndTimestamp = request.EndTime.ToUnixTimestamp();
            interview.StartTimestamp = request.StartTime.ToUnixTimestamp();
            interview.Status = Enum.TryParse<InterviewStatus>(request.Status, out InterviewStatus res) ? res : InterviewStatus.Created;

            _ = await _context.SaveChangesAsync();
        }

        [HttpPost]
        public async Task<int> CreateInterviewResult(CreateInterviewResultRequest request)
        {
            var result = await _context.InterviewResults
                .FirstOrDefaultAsync(i => i.InterviewId == request.InterviewId && i.ExpertId == request.ExpertId);

            if (result == null)
            {
                InterviewResult interviewResult = new()
                {
                    Comment = request.Comment,
                    InterviewId = request.InterviewId,
                    ExpertId = request.ExpertId,
                    Estimations = request.Estimations.Select(e => new SkillEstimation() { Estimation = e.Estimation, SkillId = e.SkillId }).ToList()
                };

                _ = await _context.InterviewResults.AddAsync(interviewResult);
                _ = await _context.SaveChangesAsync();

                return interviewResult.Id;
            }
            else
            {
                result.Estimations = request.Estimations.Select(e => new SkillEstimation() { Estimation = e.Estimation, SkillId = e.SkillId }).ToList();
            }

            _ = await _context.SaveChangesAsync();

            return result.Id;
        }
        
        [HttpGet]
        [Route("{interviewId?}")]
        public async Task<List<InterviewResultViewModel>> GetInterviewResultsForInterview(int interviewId)
        {
            var interview = await _context.Interviews.AsNoTracking().FirstOrDefaultAsync(i => i.Id == interviewId);

            if (interview is null)
                throw new Exception("Не найдено собеседование");

            var interviewResults = _context.InterviewResults.AsNoTracking().Where(i => i.InterviewId == interviewId).ToList();
            var needExperts = interviewResults.Select(ir => ir.ExpertId).Distinct().ToArray();

            List<EmployeeAdapterViewModel> experts = await _adapter.GetAdaptedEmployeesByIdsAsync(needExperts);


            IEnumerable<EmployeeViewModel> expertVms = experts.Select(e => new EmployeeViewModel()
            {
                Id = e.Id,
                Email = e.Email,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Position = GetPosition(e.Position),
                StaffUnit = GetStaffUnit(e.StaffUnit),
            });

            var skillIds = interviewResults.SelectMany(ir => ir.Estimations).Select(e => e.SkillId).Distinct().ToList();

            List<SkillViewModel> skills = await _context.Skills.Where(s => skillIds.Contains(s.Id)).Select(s => new SkillViewModel()
            {
                Id = s.Id,
                Name = s.Name,
                MaxScore = s.MaxScore,
                IsDeleted = s.IsDeleted,
            }).ToListAsync();


            var vacancy = await _context.Vacancies.FirstOrDefaultAsync(v => v.Id == interview.VacancyId);
            var vacancyWeights = await _context.Criteries.Where(c => vacancy.Criterias.Contains(c.Id)).ToListAsync();

            List<InterviewResultViewModel> result = interviewResults.Select(ir =>
            {
                var estimations = ir.Estimations.Select(e => new SkillEstimationViewModel()
                {
                    Skill = skills.FirstOrDefault(s => s.Id == e.SkillId),
                    Estimation = e.Estimation
                }).ToList();

                return new InterviewResultViewModel()
                {
                    Id = ir.Id,
                    Comment = ir.Comment,
                    Expert = expertVms.FirstOrDefault(e => e.Id == ir.ExpertId),
                    InterviewId = ir.InterviewId,
                    Estimations = estimations,
                    OverallScore = GetOverallScore(estimations, vacancyWeights)
                };
            }).ToList();

            return result;

        }

        static long GetOverallScore(List<SkillEstimationViewModel> estimations, List<Criteria> weights)
        {
            long summ = 0;
            foreach (var estimation in estimations)
            {
                summ += estimation.Estimation * (weights.FirstOrDefault(w => w.SkillId == estimation.Skill.Id)?.Weight).GetValueOrDefault();
            }
            return summ;
        }

        [HttpGet]
        public async Task<InterviewResultViewModel> GetInterviewResultById(int interviewResultId)
        {
            var ir = await _context.InterviewResults.FirstOrDefaultAsync(ir => ir.Id == interviewResultId);

            if (ir is null)
                throw new Exception("Не найдено собеседование");


            var adaptedEmployee = await _adapter.GetAdapedEmployeeById(ir.ExpertId);

            var skillIds = ir.Estimations.Select(e => e.SkillId).Distinct().ToList();

            List<SkillViewModel> skills = await _context.Skills.Where(s => skillIds.Contains(s.Id)).Select(s => new SkillViewModel()
            {
                Id = s.Id,
                Name = s.Name,
                MaxScore = s.MaxScore,
                IsDeleted = s.IsDeleted,
            }).ToListAsync();

            var vacancy = await _context.Vacancies.FirstOrDefaultAsync(v => v.Id == interviewResultId);
            var vacancyWeights = await _context.Criteries.Where(c => vacancy.Criterias.Contains(c.Id)).ToListAsync();

            List<SkillEstimationViewModel> skillEstimationViewModels = ir.Estimations.Select(e => new SkillEstimationViewModel()
            {
                Skill = skills.FirstOrDefault(s => s.Id == e.SkillId),
                Estimation = e.Estimation
            }).ToList();

            var result = new InterviewResultViewModel()
            {
                Id = ir.Id,
                Comment = ir.Comment,
                Expert = new EmployeeViewModel()
                {
                    Email = adaptedEmployee.Email,
                    FirstName = adaptedEmployee.FirstName,
                    Id = adaptedEmployee.FirstName,
                    LastName = adaptedEmployee.LastName,
                    Position = GetPosition(adaptedEmployee.Position),
                    StaffUnit = GetStaffUnit(adaptedEmployee.StaffUnit)
                },
                InterviewId = ir.InterviewId,
                Estimations = skillEstimationViewModels,
                OverallScore = GetOverallScore(skillEstimationViewModels, vacancyWeights)
            };

            return result;
        }

        private static StaffUnitViewModel GetStaffUnit(StaffUnitAdapterViewModel staffUnit)
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

        private static PositionViewModel GetPosition(PositionAdapterViewModel p)
        {

            return p is null ? null
            : new PositionViewModel()
            {
                Id = p?.Id,
                Name = p?.Name,
            };
        }
    }
}
