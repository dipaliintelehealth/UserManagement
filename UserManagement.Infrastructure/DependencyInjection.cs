using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using UserManagement.Contract.Factory;
using UserManagement.Contract.Repository;
using UserManagement.Contract.User;
using UserManagement.Contract.Utility;
using UserManagement.Infrastructure.Factory;
using UserManagement.Infrastructure.Files;
using UserManagement.Infrastructure.Repository;

namespace UserManagement.Repository
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IConnectionFactory, ConnectionFactory>(opt => 
                new ConnectionFactory(configuration.GetConnectionString("UTConnectionString"))
            );
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IMemberBulkInsertRepository, BulkInsertRepository>();

           // services.AddScoped(typeof(ICsvFileUtility<>), typeof(InstitutionModelCsvUtility<>));
            services.AddScoped(typeof(IExcelFileUtility<>), typeof(EPPlusExcelUtility<>));
            return services;
        }
    }
}
