using Microsoft.EntityFrameworkCore;

using TeamGatherer.Server.Models;

namespace TeamGatherer.Server.Data;

public class ApplicationDbContext : DbContext
{

    public DbSet<Vacancy> Vacancies { get; set; }
    public DbSet<Criteria> Criteries { get; set; }

    public DbSet<Interview> Interviews { get; set; }
    public DbSet<InterviewResult> InterviewResults { get; set; }

    public DbSet<Skill> Skills { get; set; }
    public DbSet<SkillGroup> SkillGroups { get; set; }
    public DbSet<User> Users { get; set; } = null!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

    }
}
