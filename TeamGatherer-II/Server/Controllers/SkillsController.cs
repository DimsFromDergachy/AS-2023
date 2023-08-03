using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using TeamGatherer.Server.Data;
using TeamGatherer.Shared.ViewModels;

namespace TeamGatherer.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SkillsController : ControllerBase
    {
        [HttpGet]
        public async Task<List<SkillViewModel>> GetSkillsAsync([FromServices] ApplicationDbContext context)
        {
                return await context.Skills.Select(s => new SkillViewModel()
                {
                    Id = s.Id,
                    Name = s.Name,
                    SkillGroupId = s.SkillGroupId,
                    IsDeleted = s.IsDeleted,
                }).AsNoTracking().ToListAsync();
        }

        [HttpGet]
        public async Task<SkillViewModel> GetSkillAsync(int id, [FromServices] ApplicationDbContext context)
        {
            var skill = await context.Skills.FindAsync(id);

            if (skill is not { })
            {
                throw new Exception("Навык не найден!");
            }

            return new()
            {
                Id = skill.Id,
                Name = skill.Name,
                SkillGroupId = skill.SkillGroupId,
                IsDeleted = skill.IsDeleted,
            };
        }

        [HttpPost]
        public async Task AddSkillToGroupAsync(SkillViewModel request, [FromServices] ApplicationDbContext context)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new Exception("Не задано имя компетенции!");

            if (request.SkillGroupId is null)
                throw new Exception("Не задана группа в которую добавлять!");

            await context.Skills.AddAsync(new Models.Skill() { Name = request.Name, SkillGroupId = request.SkillGroupId.GetValueOrDefault(), MaxScore = request.MaxScore });

            await context.SaveChangesAsync();
        }

        [HttpPost]
        public async Task UpdateSkillAsync(SkillViewModel request, [FromServices] ApplicationDbContext context)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new Exception("Не задано имя навыка!");

            if (request.SkillGroupId is null)
                throw new Exception("Не задана группа в какую добавить скилл!");

            var skill = await context.Skills.FindAsync(request.Id);

            if(skill is not { })
            {
                throw new Exception("Навык не найден!");
            }

            skill.Name = request.Name;
            skill.SkillGroupId = request.SkillGroupId.GetValueOrDefault();

            await context.SaveChangesAsync();
        }

        
        [HttpPost]
        public async Task ArchiveSkillAsync(SkillViewModel request, [FromServices] ApplicationDbContext context)
        {
            var skill = await context.Skills.FindAsync(request.Id);

            if (skill is not { })
            {
                throw new Exception("Навык не найден!");
            }

            skill.IsDeleted = !skill.IsDeleted;

            await context.SaveChangesAsync();
        }
    }
}
