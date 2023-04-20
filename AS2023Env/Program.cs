using AS2023Env;
using AS2023Env.Data;
using AS2023Env.Models;
using Microsoft.OpenApi.Models;
using BackgroundService = AS2023Env.BackgroundService;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    
    c.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
    {
        Description = "Basic auth added to authorization header",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "basic",
        Type = SecuritySchemeType.Http
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Basic" }
            },
            new List<string>()
        }
    });
});

builder.Services
    .AddHostedService<BackgroundService>()
    .AddSingleton<AuthService>()
    .AddSingleton<IStorage<Position>, PositionStorage>()
    .AddSingleton<IStorage<Employee>, EmployeeStorage>()
    .AddSingleton<IStorage<StaffUnit>, StaffUnitStorage>();

builder.Logging.ClearProviders().AddConsole();
builder.Logging.AddFilter((_, category, logLevel) =>
{
#if DEBUG == false    
    if (category.StartsWith("Microsoft."))
    {
        return logLevel > LogLevel.Information;
    }
#endif
    return logLevel >= LogLevel.Information;
});

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

// порядок инициализации важен
await app.Services.GetRequiredService<IStorage<Position>>().Init();
await app.Services.GetRequiredService<IStorage<Employee>>().Init();
await app.Services.GetRequiredService<IStorage<StaffUnit>>().Init();

app.Run();