using System;
using System.Collections.Generic;
using System.Text;

namespace UserManagement.Domain
{
    public class InstitutionModel
    {
        public int InstitutionId { get; set; }
        public string Name { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string ReferenceNumber { get; set; }
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public int DistrictId { get; set; }
        public int CityId { get; set; }
        public string PinCode { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string ImagePath { get; set; }
        public int InstitutionTypeId { get; set; }
        public string Fax { get; set; }
        public int SourceId { get; set; }
        public bool IsActive { get; set; }
        public int StatusId { get; set; }
        public string CreatedDate { get; set; }
    }
    public class LoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ReferenceId { get; set; }
        public bool IsActive { get; set; }
        public string SourceId { get; set; }
    }
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
    public class MemberInstitutionModel
    {
        public string MemberId { get; set; }
        public string InstitutionId { get; set; }
        public bool IsActive { get; set; }
        public string SourceId { get; set; }
    }
}
