using System.Collections.Generic;
using UserManagement.Models;

namespace UserManagement.Domain
{
    public class ResultModel<T> where T :class
    {
        public T Value { get; set; }
        public bool IsSuccess { get; set; } = true;
        public List<string> Messages { get; set; } = new List<string>();

        public static ResultModel<T> Success(T model)
        {
            return new ResultModel<T>()
            {
                Value = model,
                IsSuccess = true
            };
        }
        public static ResultModel<T> Failure(T model, string message)
        {
            var result = new ResultModel<T>()
            {
                Value = model,
                IsSuccess = true
            };
            result.Messages.Add(message);
            return result;
        }
    }
    public static class ResultModelExtentions
    {
        public static MemberBulkInvalid ToMemberBulkInvalid(this ResultModel<MemberBulkValid> model)
        {
           return new MemberBulkInvalid()
            {
                Address = model.Value.Address,
                AssignedHFType = model.Value.AssignedHFType,
                AssignedInstituteID = model.Value.AssignedInstituteID,
                AssignHF = model.Value.AssignHF,
                Designation = model.Value.Designation,
                DOB = model.Value.DOB,
                DRRegNo = model.Value.DRRegNo,
                ErrorMessage = string.Join(",", model.Messages),
                Experience = model.Value.Experience,
                FirstName = model.Value.FirstName,
                Gender = model.Value.Gender,
                HFCities = model.Value.HFCities,
                HFCity = model.Value.HFCity,
                HFDistrict = model.Value.HFDistrict,
                HFDistricts = model.Value.HFDistricts,
                HFEmail = model.Value.HFEmail,
                HFName = model.Value.HFName,
                HFPhone = model.Value.HFPhone,
                HFState = model.Value.HFState,
                HFType = model.Value.HFType,
                HFTypeId = model.Value.HFTypeId,
                InstituteID = model.Value.InstituteID,
                LastName = model.Value.LastName,
                MemberId = model.Value.MemberId,
                NIN = model.Value.NIN,
                PIN = model.Value.PIN,
                Qualification = model.Value.Qualification,
                QualificationId = model.Value.QualificationId,
                SelectedHFCityId = model.Value.SelectedHFCityId,
                SelectedHFDistrictId = model.Value.SelectedHFCityId,
                SelectedHFStateId = model.Value.SelectedHFCityId,
                SelectedSpecialityId = model.Value.SelectedSpecialityId,
                SelectedUserCityId = model.Value.SelectedUserCityId,
                SelectedUserDistrictId = model.Value.SelectedUserDistrictId,
                SelectedUserStateId = model.Value.SelectedUserStateId,
                SpecialityId = model.Value.SpecialityId,
                SubMenuName = model.Value.SubMenuName,
                UserAddress = model.Value.UserAddress,
                UserAvailableDay = model.Value.UserAvailableDay,
                UserAvailableFromTime = model.Value.UserAvailableFromTime,
                UserAvailableToTime = model.Value.UserAvailableToTime,
                UserCities = model.Value.UserCities,
                UserCity = model.Value.UserCity,
                UserDistrict = model.Value.UserDistrict,
                UserDistricts = model.Value.UserDistricts,
                UserDistrictShortCode = model.Value.UserDistrictShortCode,
                UserEmail = model.Value.UserEmail,
                UserMobile = model.Value.UserMobile,
                UserName = model.Value.UserName,
                UserPin = model.Value.UserPin,
                UserPrefix = model.Value.UserPrefix,
                UserRole = model.Value.UserRole,
                UserState = model.Value.UserState,
            };
        }
    }
}
