using Microsoft.Extensions.DependencyInjection;

namespace Template.Core.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            return services;
        }
    }
}
