using System.Net.Http.Headers;
using System.Text;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using RestApiClient;

namespace TeamGatherer.Shared
{
    public static class Inject
    {
        public static void AddStaffApiClient(this IServiceCollection services)
        {
            services.AddScoped((sp) =>
            {
                var opts = sp.GetRequiredService<IOptions<StaffConfig>>();
                var o = opts.Value;

                if (o.Url is null)
                    throw new Exception("Не задан адррес системы Кадры");

                if (string.IsNullOrWhiteSpace(o.Login))
                    throw new Exception("Не задано имя пользователя для авторизации в системе Кадры");

                if (string.IsNullOrWhiteSpace(o.Password))
                    throw new Exception("Не задан пароль пользователя для авторизации в системе Кадры");

                Console.Out.WriteLine($"DT: ConfigUrl: {o.Url}");

                var c = sp.GetRequiredService<IHttpClientFactory>().CreateClient("StaffApi");
                
                var string64 = Convert.ToBase64String(UTF8Encoding.Default.GetBytes($"{o.Login}:{o.Password}"));

                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", string64);
                return new StaffClientAdapter(new Client(o.Url.ToString(), c), c, opts);
            });
        }
    }
}
