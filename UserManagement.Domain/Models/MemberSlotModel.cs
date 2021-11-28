namespace UserManagement.Domain
{
    public class MemberSlotModel
    {
        public string MemberId { get; set; }
        public string Day { get; set; }
        public string SlotTo { get; set; }
        public string SlotFrom { get; set; }
        public string CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public string SourceId { get; set; }
    }
    public class MemberSlotModelForCsv
    {
        public string MemberId { get; set; }
        public string Day { get; set; }
        public string SlotTo { get; set; }
        public string SlotFrom { get; set; }
        public string CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public string SourceId { get; set; }
    }
}
