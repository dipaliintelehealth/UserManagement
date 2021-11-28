using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Domain.Validator;

namespace UserManagement.Contract.Validator
{
    public interface IBulkInsertValidator<T> where T : class
    {
        BulkInsertValidationResult Validate(IList<T> models);
        BulkInsertValidationResult Validate(T model);
        Task<BulkInsertValidationResult> ValidateAsync(IList<T> models);
        Task<BulkInsertValidationResult> ValidateAsync(T model);
    }
}
