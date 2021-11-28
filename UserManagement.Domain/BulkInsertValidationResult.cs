using System.Collections.Generic;

namespace UserManagement.Domain.Validator
{
    public class BulkInsertValidationResult
    {
        public BulkInsertValidationResult()
        {
        }
        public BulkInsertValidationResult(IList<BulkInsertValidationFailure> Errors)
        {
            this.Errors = Errors;
            IsValid = Errors.Count == 0;
        }
        public bool IsValid { get; private set; } = true;
        public IList<BulkInsertValidationFailure> Errors { get; private set; }
    }
    public class BulkInsertValidationFailure
    {
        public string ErrorCode { get; set; }
        public int Index { get; set; }
        public string ErrorMessage { get; set; }
        public string PropertyName { get; set; }
    }
}