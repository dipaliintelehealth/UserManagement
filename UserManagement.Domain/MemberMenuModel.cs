using System;

namespace UserManagement.Domain
{
    public class MemberMenuModelForCsv
    {
        public string RoleId { get; set; }
        public string MemberId { get; set; }
        public string MenuMappingId { get; set; }
        public string IsActive { get; set; }
        public string InstitutionId { get; set; }
        public string SourceId { get; set; }
    }
}
