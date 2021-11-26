using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Domain;
using UserManagement.Domain.ViewModel;

namespace UserManagement.Contract.Repository
{
    public interface IMemberBulkInsertRepository :IRepository
    {
        Task<IEnumerable<StateDistrictCity>> GetStateDistrictCities();
        Task<int> BulkInsertInstitution(Stream stream);
        Task<int> BulkInsertMembers(Stream stream);
        Task<int> BulkInsertLogin(Stream stream);
        Task<int> BulkInsertMemberSlot(Stream stream);
        Task<int> BulkInsertMemberInstitution(Stream stream);
        Task<IEnumerable<string>> FindEmails(IEnumerable<string> emails);
        Task<IEnumerable<string>> FindMobiles(IEnumerable<string> mobiles);
        Task<IEnumerable<string>> FindUsers(IEnumerable<string> users);
        Task<IEnumerable<InstitutionModel>> GetInstitution();
        Task<IEnumerable<QualificationModel>> GetQualification();
        Task<int> GetMaxInstituteId();
        Task<IEnumerable<InstitutionModel>> GetInstituations(int minId, int maxId);
        Task<int> GetMaxMemberId();
        Task<IEnumerable<MembersModel>> GetMembers(int minId, int maxId);
        
        Task<int> BulkInsertMemberMenu(Stream stream);
        Task<int> BulkInsertAuditTrail(Stream stream);
        Task<IEnumerable<SubMenuModel>> GetSubMenu();
        Task<IEnumerable<KeyValue<string, string>>> GetStates();
        Task<IEnumerable<KeyValue<string, string>>> GetDistrict(string stateId);

        Task<IEnumerable<KeyValue<string, string>>> GetCities(string stateId, string districtId);
    }
}
