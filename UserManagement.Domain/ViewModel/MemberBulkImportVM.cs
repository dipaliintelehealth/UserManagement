using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace UserManagement.Domain.ViewModel
{
    public class MemberBulkImportVM
    {
        
        [Display(Name = "HF Name")]
        public string HFName { get; set; }
        public string HFNameActual { get; set; }
        [Display(Name = "HF Phone")]
        public string HFPhone { get; set; }
        [Display(Name = "HF Type")]
        public string HFType { get; set; }
        public string NIN { get; set; }
        [Display(Name = "HF Email")]
        public string HFEmail { get; set; }
        [Display(Name = "State")]
        public string State { get; set; }
        [Display(Name = "District")]
        public string District { get; set; }
        [Display(Name = "City")]
        public string City { get; set; }
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
        public string UserAvilableDay { get; set; }
        [Display(Name = "FromTime")]
        public string UserAvilableFromTime { get; set; }
        [Display(Name = "To Time")]
        public string UserAvilableToTime { get; set; }
        [Display(Name = "Role")]
        public string UserRole { get; set; }
        [Display(Name = "Assign Type")]
        public string AssignedHFType { get; set; }
        [Display(Name = "Assign PHC Or Hub")]
        public string AssignHF { get; set; }
        public int StateId { get; set; }
        public int DistrictId { get; set; }
        public int CityId { get; set; }
        public int UserStateId { get; set; }
        public int UserDistrictId { get; set; }
        public int UserCityId { get; set; }
        public int QualificationId { get; set; }
        public int GenderId
        {
            get

            {
                if (this.Gender == "Female")
                {
                    return 2;
                }
                else if (this.Gender == "Male")
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
            get
            {
                if (HFType.Replace(" ", "").Replace("-", "").ToLower() == "hub")
                {
                    return 1;
                }
                else if (HFType.Replace(" ", "").Replace("-", "").ToLower() == "phc")
                {
                    return 2;
                }
                return 3;
            }
        }
        public string AssignedInstituteID { get; set; }
        public string InstituteID { get; set; }
        public string MemberId { get; set; }
        public string UserDistrictShortCode { get; set; }
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        public string ComputedHFName
        {
            get
            {
                return $"{this.HFName} {this.District}";
            }
        }
        [Display(Name = "HF Name")]
        public string HFNameWithDistrictName
        {
            get
            {
                return $"{this.HFName?.Trim()} {this.District?.Trim()}";
            }
        }
    }
    public class CompareHFNameWithDistrictName : IEqualityComparer<MemberBulkImportVM>
    {
        public bool Equals(MemberBulkImportVM x, MemberBulkImportVM y)
        {
           return x.HFNameWithDistrictName.Trim().ToLower().Equals(y.HFNameWithDistrictName.Trim().ToLower());
            
        }

        public int GetHashCode(MemberBulkImportVM obj)
        {
            return obj.HFNameWithDistrictName.Trim().ToLower().GetHashCode();
        }
    }
}