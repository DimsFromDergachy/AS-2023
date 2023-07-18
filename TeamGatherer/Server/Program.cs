using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.Filters;

using TeamGatherer.Server;
using TeamGatherer.Server.Data;
using TeamGatherer.Server.Models;
using TeamGatherer.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddStaffApiClient();

builder.Services.AddHostedService<VacancyCloserBackgroundService>();
builder.Services.AddHostedService<InterviewRejectorBackgroundService>();

builder.Services.AddHttpClient("StaffApi");

builder.Services.AddRazorPages();

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddAuthentication(builder.Configuration)
    .AddAdapters()
    .AddRepositories();

builder.Services.Configure<StaffConfig>(builder.Configuration.GetSection("StaffConfig"));

var app = builder.Build();
await SeedDatabaseAsync(app);


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();


static async Task SeedDatabaseAsync(WebApplication app)
{
    await using var scope = app.Services.CreateAsyncScope();
    await using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();

    var comminication = new SkillGroup() { Name = "Коммуникационные", Skills = new() };
    var command = new SkillGroup() { Name = "Навыки работы в комманде", Skills = new() };
    var timemanagement = new SkillGroup() { Name = "Навыки управления временем", Skills = new() };
    var tech = new SkillGroup() { Name = "Технические навыки", Skills = new() };

    var anyAdded = false;
    await Add(comminication);
    await Add(command);
    await Add(timemanagement);
    await Add(tech);

    if (anyAdded)
        await context.SaveChangesAsync();

    async Task Add(SkillGroup sg)
    {
        if (!context.SkillGroups.Any(s => s.Name == sg.Name))
        {
            await context.SkillGroups.AddAsync(sg);
            anyAdded = true;
        }
    }
}