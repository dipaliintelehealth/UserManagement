using CsvHelper.Configuration;
using UserManagement.Domain;

namespace UserManagement.Infrastructure.Mapper.CSV
{
    public sealed class MembersModelForCsvMap : ClassMap<MembersModelForCsv>
    {
        public MembersModelForCsvMap()
        {
            Map(m => m.FirstName).Index(1);
            Map(m => m.MiddleName).Index(2);
            Map(m => m.LastName).Index(3);
            Map(m => m.AgeType).Index(4);
            Map(m => m.DOB).Index(5);

            Map(m => m.Age).Index(6);
            Map(m => m.Mobile).Index(7);
            Map(m => m.Email).Index(8);
            Map(m => m.PinCode).Index(9);
            Map(m => m.Mobile).Index(10);
            Map(m => m.Email).Index(11);
            Map(m => m.CreatedBy).Index(12);

            Map(m => m.CreatedDate).Index(13);
            Map(m => m.IsActive).Index(14);

            Map(m => m.GenderId).Index(15);
            Map(m => m.RegistrationNumber).Index(16);
            Map(m => m.AddressLine1).Index(17);
            Map(m => m.AddressLine2).Index(18);

            Map(m => m.StateId).Index(19);
            Map(m => m.DistrictId).Index(20);
            Map(m => m.CityId).Index(21);
            Map(m => m.SpecializationId).Index(22);

            Map(m => m.QualificationId).Index(23);
            Map(m => m.PinCode).Index(24);
            Map(m => m.Fax).Index(25);
            Map(m => m.LoginOTP).Index(26);

            Map(m => m.IsLoginOTPActive).Index(27);
            Map(m => m.SignaturePath).Index(28);
            Map(m => m.CountryId).Index(29);
            Map(m => m.StatusId).Index(30);

            Map(m => m.RatingMasterId).Index(31);
            Map(m => m.SourceId).Index(32);
            Map(m => m.IsMaster).Index(33);
            Map(m => m.ImagePath).Index(34);

            Map(m => m.FileName).Index(35);
            Map(m => m.FileFlag).Index(36);
            Map(m => m.IsAvailable).Index(37);
            Map(m => m.Prefix).Index(38);
            Map(m => m.CreationRole).Index(39);
        }
    }

}
