using AS2023Env;
using AS2023Env.Data;
using AS2023Env.Models;
using BackgroundService = AS2023Env.BackgroundService;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddHostedService<BackgroundService>()
    .AddSingleton<IStorage<Position>, PositionStorage>()
    .AddSingleton<IStorage<Employee>, EmployeeStorage>()
    .AddSingleton<IStorage<StaffUnit>, StaffUnitStorage>();

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

// порядок инициализации важен
await app.Services.GetRequiredService<IStorage<Position>>().Init();
await app.Services.GetRequiredService<IStorage<Employee>>().Init();
await app.Services.GetRequiredService<IStorage<StaffUnit>>().Init();

app.Run();