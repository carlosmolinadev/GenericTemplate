using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Data.Common;
using Template.Core.Application.Contracts.Persistence;
using Template.Infrastructure.Persistance.Dapper.Repositories;

namespace Template.Infrastructure.Persistance.Dapper
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services/*, IConfiguration configuration*/)
        {
            //services.AddTransient((sp) => new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            return services;
        }
    }
}

//services.AddTransient((sp) => new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")));