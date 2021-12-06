using System;

namespace UserManagement.Domain
{
    public class InstitutionMembersModel
    {
        public string FirstName { get; set; }
        public DateTime DOB { get; set; }
        public int StatusId { get; set; }
        public bool IsActive { get; set; }
        public bool IsMaster { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public int GenderId { get; set; }
        public int InstitutionId { get; set; }
        public string UserName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int SourceId { get; set; }
        public string Prefix { get; set; }
    }

}
