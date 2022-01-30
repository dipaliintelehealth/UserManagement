using System;
using System.Collections.Generic;
using UserManagement.Domain.Validator;
using UserManagement.Domain.ViewModel;

namespace UserManagement.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }

    public class BulkImportWithValidationErrorVM
    {
        public IList<BulkInsertValidationFailure> Errors { get; set; }
        public List<MemberBulkImportVM> Data { get; set; }
    }
}