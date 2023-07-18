using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

using Npgsql.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;

using TeamGatherer.Server.Data;
using TeamGatherer.Shared;
using TeamGatherer.Shared.Requests;
using TeamGatherer.Shared.ServerAdapterViewModels;
using TeamGatherer.Shared.ViewModels;

namespace TeamGatherer.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class VacancyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly StaffClientAdapter _adapter;

        public VacancyController(ApplicationDbContext context, StaffClientAdapter adapter)
        {
            _context = context;
            _adapter = adapter;
        }

        [HttpGet]
        [Route("{vacancyId?}")]
        public async Task<VacancyViewModel> GetVacancyByIdAsync([FromRoute] int vacancyId)
        {
            Models.Vacancy vacancy = await _context.Vacancies.FirstOrDefaultAsync(v => v.Id == vacancyId);

            if (vacancy is not { })
            {
                throw new Exception("вакансия не найдена или вакансия закрыта");
            }

            var criteria = await _context.Criteries.Where(c => vacancy.Criterias.Contains(c.Id)).ToListAsync();
            var skills = await _context.Skills.Where(s => criteria.Select(c => c.SkillId).Contains(s.Id)).Select(s => new SkillViewModel()
            {
                Id = s.Id,
                Name = s.Name,
                SkillGroupId = s.SkillGroupId,
                IsDeleted = s.IsDeleted,
            }).AsNoTracking().ToListAsync();

            var criteriaVm = criteria.Select(c => new CriteriaViewModel()
            {
                Id = c.Id,
                Skill = skills.FirstOrDefault(s => s.Id == c.SkillId),
                Weight = c.Weight
            }).ToList();

            var employees = await _adapter.GetAdaptedEmployeesByIdsAsync(vacancy.HrIds.ToArray());

            var hrs = employees.Where(e => vacancy.HrIds.Contains(e.Id));
            var hrsvm = hrs.Select(e =>
            {
                return new EmployeeViewModel()
                {
                    Id = e.Id,
                    Email = e.Email,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Position = GetPosition(e?.Position),
                    StaffUnit = GetStaffUnit(e?.StaffUnit),
                };

            }).ToList();

            var position = await _adapter.GetAdaptedPositionByIdAsync(vacancy.PositionId);
            var staffUnit = await _adapter.GetAdaptedStaffUnitByIdAsync(vacancy.StaffUnitId);

            var result = new VacancyViewModel()
            {
                Id = vacancy.Id,
                Hrs = hrsvm,
                Name = vacancy.Name,
                Position = GetPosition(position),
                StaffUnit = GetStaffUnit(staffUnit),
                Requrements = vacancy.Requrements,
                Criteria = criteriaVm,
                Responsibilities = vacancy.Responsibilities,
                WorkingConditions = vacancy.WorkingConditions,
                IsClosed = vacancy.IsClosed,
            };

            return result;

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

        [HttpGet]
        public async Task<List<VacancyListItemViewModel>> GetVacanciesAsync()
        {
            var vacancies = await _context.Vacancies.ToListAsync();

            var result = vacancies.Select(v => new VacancyListItemViewModel()
            {
                Id = v.Id,
                IsClosed = v.IsClosed,
                Name = v.Name
            }).ToList();

            return result;
        }

        [HttpGet]
        [Route("{staffId?}")]
        public async Task<int> GetVacancyByStaffIdAsync(string staffId)
        {
            var vacancy = await _context.Vacancies.FirstOrDefaultAsync(it => it.StaffUnitId == staffId);
            return vacancy?.Id ?? 0;
        }

        [HttpPost]
        public async Task<int> CreateVacancyAsync(CreateVacancyRequest request)
        {
            bool criteriasExists = request.Criterias.Count > 0;
            if (!criteriasExists)
            {
                throw new Exception("Нельзя создать вакансию без критериев оценки!");
            }

            IEnumerable<Criteria> criteriasToCreate = request.Criterias.Select(c => new Criteria() { SkillId = c.Skill.Id, Weight = c.Weight });

            await _context.Criteries.AddRangeAsync(criteriasToCreate);

            _ = await _context.SaveChangesAsync();

            IEnumerable<int> criteriasIds = criteriasToCreate.Select(c => c.Id);

            Models.Vacancy vacancy = new()
            {
                Criterias = criteriasIds.ToList(),
                PositionId = request.PositionId,
                StaffUnitId = request.StateUnitId,
                HrIds = request.HRs,
                Name = request.Name,
                Requrements = request.Requrements,
                Responsibilities = request.Responsibilities,
                WorkingConditions = request.WorkingConditions
            };
            _ = await _context.Vacancies.AddAsync(vacancy);

            _ = await _context.SaveChangesAsync();

            return vacancy.Id;
        }

        [HttpPost]
        public async Task UpdateVacancyAsync(UpdateVacancyRequest request)
        {
            Models.Vacancy vacancy = await _context.Vacancies.FirstOrDefaultAsync(v => v.Id == request.Id && !v.IsClosed);

            if (vacancy is not { })
            {
                throw new Exception("вакансия не найдена или вакансия закрыта");
            }

            vacancy.PositionId = request.PositionId;
            vacancy.Name = request.Name;
            vacancy.Requrements = request.Requrements;
            vacancy.Responsibilities = request.Responsibilities;
            vacancy.WorkingConditions = request.WorkingConditions;

            _ = await _context.SaveChangesAsync();
        }

        [HttpPost]
        public async Task DeleteVacancyAsync(DeleteVacancyRequest request)
        {
            Models.Vacancy vacancy = await _context.Vacancies.FirstOrDefaultAsync(v => v.Id == request.Id && !v.IsClosed);

            if (vacancy is not { })
            {
                throw new Exception("вакансия не найдена или вакансия закрыта");
            }

            vacancy.IsClosed = true;

            _ = await _context.SaveChangesAsync();
        }

        [HttpPost]
        public async Task<List<int>> AddCriteriaToVacancyAsync(AddCriteriaToVacancyRequest request)
        {
            Models.Vacancy vacancy = await _context.Vacancies.FirstOrDefaultAsync(v => v.Id == request.VacancyId && !v.IsClosed);

            if (vacancy is not { })
            {
                throw new Exception("вакансия не найдена или вакансия закрыта");
            }

            IEnumerable<Criteria> criteriasToCreate = request.Criterias.Select(c => new Criteria() { SkillId = c.Skill.Id, Weight = c.Weight });

            var criteriaIds = new List<int>();

            foreach (var critera in criteriasToCreate)
            {
                await _context.Criteries.AddAsync(critera);
                _ = await _context.SaveChangesAsync();
                criteriaIds.Add(critera.Id);
            }

            vacancy.Criterias.AddRange(criteriaIds);
            _ = await _context.SaveChangesAsync();

            return criteriaIds;
        }

        [HttpPost]
        public async Task RemoveCriteriaFromVacancyAsync(RemoveCriteriaFromVacancyRequest request)
        {
            Models.Vacancy vacancy = await _context.Vacancies.FirstOrDefaultAsync(v => v.Id == request.VacancyId && !v.IsClosed);

            if (vacancy is not { })
            {
                throw new Exception("вакансия не найдена или вакансия закрыта");
            }

            List<Criteria> criteriasToRemove = _context.Criteries.Where(c => request.CriteriaIds.Contains(c.Id)).ToList();

            foreach (Criteria item in criteriasToRemove)
            {
                if (vacancy.Criterias.Contains(item.Id))
                {
                    _ = vacancy.Criterias.Remove(item.Id);
                }
            }

            _context.Criteries.RemoveRange(criteriasToRemove);

            _ = await _context.SaveChangesAsync();
        }

        [HttpPost]
        public async Task AddHrToVacancyAsync(AddHrToVacancyRequest request)
        {
            Models.Vacancy vacancy = await _context.Vacancies.FirstOrDefaultAsync(v => v.Id == request.VacancyId && !v.IsClosed);

            if (vacancy is not { })
            {
                throw new Exception("вакансия не найдена или вакансия закрыта");
            }

            vacancy.HrIds.AddRange(request.HrIds);

            _ = await _context.SaveChangesAsync();
        }

        [HttpPost]
        public async Task RemoveHrFromVacancyAsync(RemoveHrFromVacancyRequest request)
        {
            Models.Vacancy vacancy = await _context.Vacancies.FirstOrDefaultAsync(v => v.Id == request.VacancyId && !v.IsClosed);

            if (vacancy is not { })
            {
                throw new Exception("вакансия не найдена или вакансия закрыта");
            }

            foreach (var item in request.HrIds)
            {
                if (vacancy.HrIds.Contains(item))
                {
                    _ = vacancy.HrIds.Remove(item);
                }
            }

            _ = await _context.SaveChangesAsync();
        }
    }
}
