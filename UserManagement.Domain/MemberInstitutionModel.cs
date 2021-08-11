namespace UserManagement.Domain
{
    public class MemberInstitutionModel
    {
        public string MemberId { get; set; }
        public string InstitutionId { get; set; }
        public bool IsActive { get; set; }
        public string SourceId { get; set; }
    }
    public class MemberInstitutionModelForCsv
    {
        public string MemberId { get; set; }
        public string InstitutionId { get; set; }
        public bool IsActive { get; set; }
        public string SourceId { get; set; }
    }
}