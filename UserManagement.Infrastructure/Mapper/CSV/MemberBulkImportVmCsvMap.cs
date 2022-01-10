using CsvHelper.Configuration;
using UserManagement.Domain.ViewModel;
using UserManagement.Models;

namespace UserManagement.Infrastructure.Mapper.CSV
{
    public class MemberBulkImportVmCsvMap : ClassMap<MemberBulkImportVM>
    {
        public MemberBulkImportVmCsvMap()
        {
            Map(m => m.HFName).Index(0);
            Map(m => m.HFPhone).Index(1);
            Map(m => m.HFType).Index(2);
            Map(m => m.NIN).Index(3);
            Map(m => m.HFEmail).Index(4);
            Map(m => m.HFState).Index(5);
            Map(m => m.HFDistrict).Index(6);
            Map(m => m.HFCity).Index(7);
            Map(m => m.Address).Index(8);
            Map(m => m.PIN).Index(9);
            Map(m => m.FirstName).Index(10);
            Map(m => m.LastName).Index(11);
            Map(m => m.UserMobile).Index(12);
            Map(m => m.Gender).Index(13);
            Map(m => m.Qualification).Index(14);
            Map(m => m.Experience).Index(15);
            Map(m => m.DRRegNo).Index(16);
            Map(m => m.UserEmail).Index(17);
            Map(m => m.Designation).Index(18);
            Map(m => m.DOB).Index(19);
            Map(m => m.UserState).Index(20);
            Map(m => m.UserDistrict).Index(21);
            Map(m => m.UserCity).Index(22);
            Map(m => m.UserAddress).Index(23);
            Map(m => m.UserPin).Index(24);
            Map(m => m.UserPrefix).Index(25);
            Map(m => m.UserAvailableDay).Index(26);
            Map(m => m.UserAvailableFromTime).Index(27);
            Map(m => m.UserAvailableToTime).Index(28);
            Map(m => m.UserRole).Index(29);
            Map(m => m.AssignedHFType).Index(30);
            Map(m => m.AssignHF).Index(31);
            Map(m => m.SelectedHFStateId).Index(32);
            Map(m => m.SelectedHFDistrictId).Index(33);
            Map(m => m.SelectedHFCityId).Index(34);
            Map(m => m.SelectedUserStateId).Index(35);
            Map(m => m.SelectedUserDistrictId).Index(36);
            Map(m => m.SelectedUserCityId).Index(37);
            Map(m => m.QualificationId).Index(38);
            Map(m => m.GenderId).Index(39);
            Map(m => m.HFTypeId).Index(40);
            Map(m => m.AssignedInstituteID).Index(41);
            Map(m => m.InstituteID).Index(42);
            Map(m => m.MemberId).Index(43);
            Map(m => m.UserDistrictShortCode).Index(44);
            Map(m => m.UserName).Index(45);
            Map(m => m.HFNameWithDistrictName).Index(46);
            Map(m => m.SubMenuName).Index(47);
            Map(m => m.SpecialityId).Index(48);
            Map(m => m.SelectedSpecialityId).Index(49);
            Map(m => m.HFDistricts).Ignore();
            Map(m => m.HFCities).Ignore();
            Map(m => m.UserDistricts).Ignore();
            Map(m => m.UserCities).Ignore();

        }
    }
    public class MemberBulkValidCsvMap : ClassMap<MemberBulkValid>
    {
        public MemberBulkValidCsvMap()
        {
            Map(m => m.HFName).Index(0);
            Map(m => m.HFPhone).Index(1);
            Map(m => m.HFType).Index(2);
            Map(m => m.NIN).Index(3);
            Map(m => m.HFEmail).Index(4);
            Map(m => m.HFState).Index(5);
            Map(m => m.HFDistrict).Index(6);
            Map(m => m.HFCity).Index(7);
            Map(m => m.Address).Index(8);
            Map(m => m.PIN).Index(9);
            Map(m => m.FirstName).Index(10);
            Map(m => m.LastName).Index(11);
            Map(m => m.UserMobile).Index(12);
            Map(m => m.Gender).Index(13);
            Map(m => m.Qualification).Index(14);
            Map(m => m.Experience).Index(15);
            Map(m => m.DRRegNo).Index(16);
            Map(m => m.UserEmail).Index(17);
            Map(m => m.Designation).Index(18);
            Map(m => m.DOB).Index(19);
            Map(m => m.UserState).Index(20);
            Map(m => m.UserDistrict).Index(21);
            Map(m => m.UserCity).Index(22);
            Map(m => m.UserAddress).Index(23);
            Map(m => m.UserPin).Index(24);
            Map(m => m.UserPrefix).Index(25);
            Map(m => m.UserAvailableDay).Index(26);
            Map(m => m.UserAvailableFromTime).Index(27);
            Map(m => m.UserAvailableToTime).Index(28);
            Map(m => m.UserRole).Index(29);
            Map(m => m.AssignedHFType).Index(30);
            Map(m => m.AssignHF).Index(31);
            Map(m => m.SelectedHFStateId).Index(32);
            Map(m => m.SelectedHFDistrictId).Index(33);
            Map(m => m.SelectedHFCityId).Index(34);
            Map(m => m.SelectedUserStateId).Index(35);
            Map(m => m.SelectedUserDistrictId).Index(36);
            Map(m => m.SelectedUserCityId).Index(37);
            Map(m => m.QualificationId).Index(38);
            Map(m => m.GenderId).Index(39);
            Map(m => m.HFTypeId).Index(40);
            Map(m => m.AssignedInstituteID).Index(41);
            Map(m => m.InstituteID).Index(42);
            Map(m => m.MemberId).Index(43);
            Map(m => m.UserDistrictShortCode).Index(44);
            Map(m => m.UserName).Index(45);
            Map(m => m.HFNameWithDistrictName).Index(46);
            Map(m => m.SubMenuName).Index(47);
            Map(m => m.SpecialityId).Index(48);
            Map(m => m.SelectedSpecialityId).Index(49);
            Map(m => m.HFDistricts).Ignore();
            Map(m => m.HFCities).Ignore();
            Map(m => m.UserDistricts).Ignore();
            Map(m => m.UserCities).Ignore();

        }
    }
    public class MemberBulkInvalidCsvMap : ClassMap<MemberBulkInvalid>
    {
        public MemberBulkInvalidCsvMap()
        {
            Map(m => m.HFName).Index(0);
            Map(m => m.HFPhone).Index(1);
            Map(m => m.HFType).Index(2);
            Map(m => m.NIN).Index(3);
            Map(m => m.HFEmail).Index(4);
            Map(m => m.HFState).Index(5);
            Map(m => m.HFDistrict).Index(6);
            Map(m => m.HFCity).Index(7);
            Map(m => m.Address).Index(8);
            Map(m => m.PIN).Index(9);
            Map(m => m.FirstName).Index(10);
            Map(m => m.LastName).Index(11);
            Map(m => m.UserMobile).Index(12);
            Map(m => m.Gender).Index(13);
            Map(m => m.Qualification).Index(14);
            Map(m => m.Experience).Index(15);
            Map(m => m.DRRegNo).Index(16);
            Map(m => m.UserEmail).Index(17);
            Map(m => m.Designation).Index(18);
            Map(m => m.DOB).Index(19);
            Map(m => m.UserState).Index(20);
            Map(m => m.UserDistrict).Index(21);
            Map(m => m.UserCity).Index(22);
            Map(m => m.UserAddress).Index(23);
            Map(m => m.UserPin).Index(24);
            Map(m => m.UserPrefix).Index(25);
            Map(m => m.UserAvailableDay).Index(26);
            Map(m => m.UserAvailableFromTime).Index(27);
            Map(m => m.UserAvailableToTime).Index(28);
            Map(m => m.UserRole).Index(29);
            Map(m => m.AssignedHFType).Index(30);
            Map(m => m.AssignHF).Index(31);
            Map(m => m.SelectedHFStateId).Index(32);
            Map(m => m.SelectedHFDistrictId).Index(33);
            Map(m => m.SelectedHFCityId).Index(34);
            Map(m => m.SelectedUserStateId).Index(35);
            Map(m => m.SelectedUserDistrictId).Index(36);
            Map(m => m.SelectedUserCityId).Index(37);
            Map(m => m.QualificationId).Index(38);
            Map(m => m.GenderId).Index(39);
            Map(m => m.HFTypeId).Index(40);
            Map(m => m.AssignedInstituteID).Index(41);
            Map(m => m.InstituteID).Index(42);
            Map(m => m.MemberId).Index(43);
            Map(m => m.UserDistrictShortCode).Index(44);
            Map(m => m.UserName).Index(45);
            Map(m => m.HFNameWithDistrictName).Index(46);
            Map(m => m.SubMenuName).Index(47);
            Map(m => m.SpecialityId).Index(48);
            Map(m => m.SelectedSpecialityId).Index(49);
            Map(m => m.ErrorMessage).Index(50);
            Map(m => m.HFDistricts).Ignore();
            Map(m => m.HFCities).Ignore();
            Map(m => m.UserDistricts).Ignore();
            Map(m => m.UserCities).Ignore();

        }
    }
}