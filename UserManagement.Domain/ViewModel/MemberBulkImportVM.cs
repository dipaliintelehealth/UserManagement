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
        public string HFPhone { get; set; }
        public string HFType { get; set; }
        public string NIN { get; set; }
        [Display(Name = "HF Email")]
        public string HFEmail { get; set; }
        public string State { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string PIN { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserMobile { get; set; }
        public string Gender { get; set; }
        public string Qualification { get; set; }
        public string Experience { get; set; }
        public string DRRegNo { get; set; }
        public string UserEmail { get; set; }
        public string Designation { get; set; }
        public string DOB { get; set; }
        public string UserState { get; set; }
        public string UserDistrict { get; set; }
        public string UserCity { get; set; }
        public string UserAddress { get; set; }
        public string UserPin { get; set; }
        public string UserPrefix { get; set; }
        public string UserAvilableDay { get; set; }
        public string UserAvilableFromTime { get; set; }
        public string UserAvilableToTime { get; set; }
        public string UserRole { get; set; }
        public string AssignedHFType { get; set; }
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
    }
}