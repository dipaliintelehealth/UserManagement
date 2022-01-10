using System;
using System.Collections.Generic;
using System.Text;
using UserManagement.Models;

namespace UserManagement.Domain.ViewModel
{
    public class BulkInsertValidInvalidVM
    {
        public IEnumerable<MemberBulkValid> ValidModels { get; set; }
        public IEnumerable<MemberBulkInvalid> InValidModels { get; set; }
    }
}
