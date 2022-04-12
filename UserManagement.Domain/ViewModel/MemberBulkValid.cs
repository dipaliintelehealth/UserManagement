using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Domain.ViewModel;

namespace UserManagement.Models
{
    public class MemberBulkValid
    {

        [Display(Name = "HF Name")]
        public string HFName { get; set; }

        [Display(Name = "HF Phone")]
        public string HFPhone { get; set; }
        [Display(Name = "HF Type")]
        public string HFType { get; set; }
        public string NIN { get; set; }
        [Display(Name = "HF Email")]
        public string HFEmail { get; set; }
        [Display(Name = "State")]
        public string HFState { get; set; }
        [Display(Name = "District")]
        public string HFDistrict { get; set; }
        [Display(Name = "City")]
        public string HFCity { get; set; }
        [Display(Name = "Address")]
        public string Address { get; set; }
        [Display(Name = "PIN")]
        public string PIN { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "User Mobile")]
        public string UserMobile { get; set; }
        [Display(Name = "Gender")]
        public string Gender { get; set; }
        [Display(Name = "Qualification")]
        public string Qualification { get; set; }
        [Display(Name = "Experience (in yrs)")]
        public string Experience { get; set; }
        [Display(Name = "Dr Reg No")]
        public string DRRegNo { get; set; }
        [Display(Name = "User Email")]
        public string UserEmail { get; set; }
        [Display(Name = "Specialization / Designation")]
        public string Designation { get; set; }
        [Display(Name = "Date of Birth DD-MM-YYYY")]
        public string DOB { get; set; }
        [Display(Name = "User State")]
        public string UserState { get; set; }
        [Display(Name = "User District")]
        public string UserDistrict { get; set; }
        [Display(Name = "User City")]
        public string UserCity { get; set; }
        [Display(Name = "User Address")]
        public string UserAddress { get; set; }
        [Display(Name = "User PIN")]
        public string UserPin { get; set; }
        [Display(Name = "User Prefix")]
        public string UserPrefix { get; set; }
        [Display(Name = "Day and Time (Availability)")]
        public string UserAvailableDay { get; set; }
        [Display(Name = "FromTime")]
        public string UserAvailableFromTime { get; set; }
        [Display(Name = "To Time")]
        public string UserAvailableToTime { get; set; }
        [Display(Name = "Role")]
        public string UserRole { get; set; }
       /* [Display(Name = "Assign Type")]
        public string AssignedHFType { get; set; }
        [Display(Name = "Assign PHC Or Hub")]
        public string AssignHF { get; set; }*/
        public IEnumerable<KeyValue<string, string>> HFDistricts { get; set; }
        public IEnumerable<KeyValue<string, string>> HFCities { get; set; }
        public IEnumerable<KeyValue<string, string>> UserDistricts { get; set; }
        public IEnumerable<KeyValue<string, string>> UserCities { get; set; }
        public int SelectedHFStateId { get; set; }
        public int SelectedHFDistrictId { get; set; }
        public int SelectedHFCityId { get; set; }
        public int SelectedUserStateId { get; set; }
        public int SelectedUserDistrictId { get; set; }
        public int SelectedUserCityId { get; set; }
        public int SelectedSpecialityId { get; set; }
        public int QualificationId { get; set; }
        public int SpecialityId { get; set; }
        public int GenderId
        {
            get

            {
                if (this.Gender?.ToLower() == "Female".ToLower())
                {
                    return 2;
                }
                else if (this.Gender?.ToLower() == "Male".ToLower())
                {
                    return 1;
                }
                else
                {
                    return 3;
                }
            }
        }

        public int HFTypeId
        {
            get; set;
         
        }
        public string AssignedInstituteID { get; set; }
        public string InstituteID { get; set; }
        public string MemberId { get; set; }
        public string UserDistrictShortCode { get; set; }
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Display(Name = "HF Name")]
        public string HFNameWithDistrictName
        {
            get
            {
                return $"{this.HFName?.Trim()} {this.HFDistrict?.Trim()}";
            }
        }
        [Display(Name = "Sub Menu")]
        public string SubMenuName { get; set; }
        public bool IsInstituteInserted { get; set; }
    }
}
