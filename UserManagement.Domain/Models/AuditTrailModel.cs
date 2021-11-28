namespace UserManagement.Domain
{
    public class AuditTrailModel
    {
        public string Message { get; set; }
        public string CreatedDate { get; set; }
        public string IconPath { get; set; }
        public string MemberId { get; set; }
        public string ModuleId { get; set; }
        public string EventId { get; set; }
        public string AccessType { get; set; }
        public string LocationIPAddress { get; set; }
        public string SourceId { get; set; }
        public string UserTypeId { get; set; }
    }
}
