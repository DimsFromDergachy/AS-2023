using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using TeamGatherer.Server.Data;
using TeamGatherer.Shared.ViewModels;
using TeamGatherer.Server.Models;

namespace TeamGatherer.Server.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class SkillGroupsController : ControllerBase
{
    [HttpGet]
    public async Task<List<SkillGroupViewModel>> GetSkillGroupsAsync([FromServices] ApplicationDbContext context)
    {
        return await context.SkillGroups
            .Select(s => new SkillGroupViewModel()
            {
                Id = s.Id,
                Name = s.Name,
                IsDeleted = s.IsDeleted,
            }).AsNoTracking().ToListAsync();
    }

    [HttpGet]
    public async Task<SkillGroupViewModel> GetSkillGroupAsync(int id, [FromServices] ApplicationDbContext context)
    {
        var group = await context.SkillGroups.FindAsync(id);

        if (group is not { })
        {
            throw new Exception("Группа навыков не найдена!");
        }

        return new()
        {
            Id = group.Id,
            Name = group.Name,
            IsDeleted = group.IsDeleted,
        };
    }

    [HttpPost]
    public async Task AddGroupAsync(SkillGroupViewModel request, [FromServices] ApplicationDbContext context)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new Exception("Не задано имя для группы навыков!");

        await context.SkillGroups.AddAsync(new Models.SkillGroup() { Name = request.Name, Skills = new List<Skill>()});

        await context.SaveChangesAsync();
    }

    [HttpPost]
    public async Task UpdateGroupAsync(SkillGroupViewModel request, [FromServices] ApplicationDbContext context)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new Exception("Не задано имя для группы навыков!");

        var group = await context.SkillGroups.FindAsync(request.Id);

        if (group is not { })
        {
            throw new Exception("Группа навыков не найдена!");
        }

        group.Name = request.Name;

        await context.SaveChangesAsync();
    }

    [HttpPost]
    public async Task ArchiveSkillGroupAsync(SkillGroupViewModel request, [FromServices] ApplicationDbContext context)
    {
        var group = await context.SkillGroups.FindAsync(request.Id);

        if (group is not { })
        {
            throw new Exception("Группа навыков не найдена!");
        }

        group.IsDeleted = !group.IsDeleted;

        await context.SaveChangesAsync();
    }
}