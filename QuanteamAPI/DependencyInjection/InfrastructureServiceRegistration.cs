using QuanteamAPI.Abstracts;
using QuanteamAPI.Models;

namespace QuanteamAPI.DependencyInjection
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<BaseUrls>(configuration.GetSection("BaseUrls"));
            services.AddHttpClient();
            services.AddMemoryCache();
            services.AddScoped<IBestStory, BestStoryService>();

            return services;
        }
    }
}
