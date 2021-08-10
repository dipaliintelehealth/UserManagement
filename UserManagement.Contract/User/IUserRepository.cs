using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Domain;

namespace UserManagement.Contract.User
{
    public interface IUserRepository : IDisposable, IRepository
    {
        string INSERTUSER(UserManagement.Domain.User.User Model);
        string GetCity(string DistID);
        string GetSubCenterID(string strHFName);
        string GetQualificationID(string strQfName);
        string GetStateID(string strStateName);
        string GetDistID(string strDistName, string stateID);
        string GetCityID(string strCityName, string stateID, string districtID);
        string CreateUsersName(string StateName, string DistrictName, string HFName, string Type);
        string ExeQueryOutput(string sql);
        bool ValidateEmail(string email);
        bool IsWholeNumber(string strNumber);
        bool ValidateMobile(string strMobile);
        bool CheckDuplicate(string strUserEmail, string strUserMobile);
        string ValidateData(string strHFName, string strHFEmail, string strMobile, string strUserEmail, string strUserFName, string strUserLName);
        DataTable GetMenuMappingID(string RoleID);
        string MemberInstitutionMapping(string memberid, string institutionid);
        string AssignMemberMenu(string RoleID, string MemberID, string InstituteID);
        string MemberSlot(string MemberID, string Day, string SlotTo, string SlotFrom);
        string GetInstutionID(string InstutionName);
        string MappedNetwork(string ParentInstitutionId, string ChildInstitutionID);
        string CreateServiceProvider(string HFNanme, string Addres1, string Addres2, string RefNumber, string CountryId, string StateID, string DistId, string CityId, string Pin, string HfMobile, string HfEmail, string TypeId);

        string CreateMember
       (string pFirstName,
        string pMiddleName,
        string pLastName,
        string pAgeType,
        string pDOB,
        string pAge,
        string pMobile,
        string pEmail,
        string pImagePath,
        string pCreatedBy,
        string pCreatedDate,
        string pIsActive,
        string pGenderId,
        string pRegistrationNumber,
        string pAddressLine1,
        string pAddressLine2,
        string pStateId,
        string pDistrictId,
        string pCityId,
        string pSpecializationId,
        string pQualificationId,
        string pPinCode,
        string pFax,
        string pLoginOTP,
        string pIsLoginOTPActive,
        string pSignaturePath,
        string pCountryId,
        string pStatusId,
        string pRatingMasterId,
        string pSourceId,
        string pIsMaster,
        string pPrefix,
        string pCreationRole
       );

        string CreateLogin(string UserName, string RefernceId);
        string AuditLog(string Message, string IconPath,string MemberID,string ModuleId,string EventId,string AccessType,string IP,string Source,string UserType);
        string CheckDist(string DistName);
        string CheckState(string StateName);
        Task<IEnumerable<StateDistrictCity>> GetStateDistrictCities();
    }
}
