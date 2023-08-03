using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using TeamGatherer.Server.Adapters;
using TeamGatherer.Server.Data;

namespace TeamGatherer.Server;

public static class Inject
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ServiceOptions>(configuration);
        var options = configuration.Get<ServiceOptions>();

        Console.WriteLine($"KA: ConnectonString: {options.ConnectionString}");

        services.AddDbContextFactory<ApplicationDbContext>(o =>
        {
            var connectionString = options?.ConnectionString ?? string.Empty;
            connectionString = connectionString.TrimEnd(';');

            _ = o.UseNpgsql(connectionString);
        });

        return services;
    }

    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication().AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("AppSettings:Token").Value!))
            };
        });

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services;
    }

    public static IServiceCollection AddAdapters(this IServiceCollection services)
    {
        services.AddScoped<IAuthenticationAdapter, AuthenticationAdapter>();
        return services;
    }
}