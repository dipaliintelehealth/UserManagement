using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using UserManagement.Business.Services;
using UserManagement.Business.Validators;
using UserManagement.Contract;
using UserManagement.Contract.Factory;
using UserManagement.Contract.Repository;
using UserManagement.Contract.User;
using UserManagement.Contract.Utility;
using UserManagement.Contract.Validator;
using UserManagement.Domain.ViewModel;

namespace UserManagement.Business
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusiness(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IBulkDataImportService<MemberBulkImportVM>, MemberBulkDataImportService>();
            services.AddScoped<IBulkInsertValidator<MemberBulkImportVM>, MemberBulkInsertValidator>();
            return services;
        }
    }
}
