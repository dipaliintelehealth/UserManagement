using System;

namespace UserManagement.Domain
{
    public class MembersModel
    {
        public int MemberId { get; set; }
        public string Day { get; set; }
        public string SlotFrom { get; set; }
        public string SlotTo { get; set; }
        public int SourceId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int AgeType { get; set; }
        public string DOB { get; set; }
        public int Age { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string ImagePath { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public int IsActive { get; set; }
        public int GenderId { get; set; }
        public string RegistrationNumber { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public int StateId { get; set; }
        public int DistrictId { get; set; }
        public int CityId { get; set; }
        public int SpecializationId { get; set; }
        public int QualificationId { get; set; }
        public string PinCode { get; set; }
        public string Fax { get; set; }
        public int LoginOTP { get; set; }
        public string IsLoginOTPActive { get; set; }
        public string SignaturePath { get; set; }
        public int CountryId { get; set; }
        
        public int StatusId { get; set; }
        
        public int RatingMasterId { get; set; }
        public string IsMaster { get; set; }
        public string CreationRole { get; set; }
        public string UserName { get; set; }
        public int InstitutionId { get; set; }
        public string Prefix { get; set; }
    }

    public class MembersModelForCsv
    {
        public string Day { get; set; }
        public string SlotFrom { get; set; }
        public string SlotTo { get; set; }
        public int SourceId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int AgeType { get; set; }
        public string DOB { get; set; }
        public int Age { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string ImagePath { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public int IsActive { get; set; }
        public int GenderId { get; set; }
        public string RegistrationNumber { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public int StateId { get; set; }
        public int DistrictId { get; set; }
        public int CityId { get; set; }
        public int SpecializationId { get; set; }
        public int QualificationId { get; set; }
        public string PinCode { get; set; }
        public string Fax { get; set; }
        public int LoginOTP { get; set; }
        public string IsLoginOTPActive { get; set; }
        public string SignaturePath { get; set; }
        public int CountryId { get; set; }

        public int StatusId { get; set; }

        public int RatingMasterId { get; set; }
        public string IsMaster { get; set; }
        public string CreationRole { get; set; }
        public string UserName { get; set; }
        public int InstitutionId { get; set; }
        public string Prefix { get; set; }
        public string FileName { get; set; }
        public string FileFlag { get; set; }
        public bool IsAvailable { get; set; }
    }
}
