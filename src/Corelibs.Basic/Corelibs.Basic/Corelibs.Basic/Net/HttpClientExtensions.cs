using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Corelibs.Basic.Net;

public static class HttpClientExtensions
{
    public static IHttpClientBuilder AddHttpClient(
        this IServiceCollection services, string name, IConfiguration configuration)
    {
        return services.AddHttpClient(name, client =>
        {
            var baseAddress = configuration.GetSection("ApiUrl").Value;
            client.BaseAddress = new Uri(baseAddress);
        });
    }
}
