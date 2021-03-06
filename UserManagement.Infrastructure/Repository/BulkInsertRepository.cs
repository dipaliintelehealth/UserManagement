using System;
using Microsoft.Extensions.Logging;
using System.Data;
using Dapper;
using UserManagement.Contract.User;
using UserManagement.Contract.Factory;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UserManagement.Domain;
using System.Threading.Tasks;
using UserManagement.Contract.Repository;
using MySqlConnector;
using System.IO;
using System.Linq;
using UserManagement.Domain.ViewModel;

namespace UserManagement.Infrastructure.Repository
{
    public class BulkInsertRepository : RepositoryBase, IMemberBulkInsertRepository
    {
        private readonly ILogger<IMemberBulkInsertRepository> Logger;
        private MySqlConnection bulkLoaderConnection;
        public BulkInsertRepository(IConnectionFactory connectionFactory, ILogger<IMemberBulkInsertRepository> logger) : base(connectionFactory)
        {
            // string connString = this.Configuration.GetConnectionString("MyConn");
            Logger = logger;
            Logger.LogInformation("BulkInsertRepository initialized");
            bulkLoaderConnection = new MySqlConnection($"{Connection.ConnectionString};AllowLoadLocalInfile=True");
        }

        public async Task<int> BulkInsertInstitution(Stream stream)
        {
            MySqlBulkLoader bulkLoader = new MySqlBulkLoader(bulkLoaderConnection)
            {
                Columns = {
                    "Name",
                    "AddressLine1",
                    "AddressLine2",
                    "ReferenceNumber",
                    "CountryId",
                    "StateId",
                    "DistrictId",
                    "CityId",
                    "PinCode",
                    "Mobile",
                    "Email",
                    "ImagePath",
                    "InstitutionTypeId",
                    "IsActive",
                    "Fax",
                    "CreatedDate",
                    "SourceId",
                    "StatusId"
                }
            };

            bulkLoader.Local = true;
            bulkLoader.TableName = "md_institution";
            bulkLoader.FieldTerminator = ",";
            bulkLoader.LineTerminator = "\n";
            //bulkLoader.FieldQuotationCharacter = '"';
            bulkLoader.SourceStream = stream;
            bulkLoader.NumberOfLinesToSkip = 1;


            return await bulkLoader.LoadAsync();
        }

        public async Task<int> BulkInsertMembers(Stream stream)
        {
            MySqlBulkLoader bulkLoader = new MySqlBulkLoader(bulkLoaderConnection)
            {
                Columns = {
                    "FirstName",
                    "MiddleName",
                    "LastName",
                    "AgeType",
                    "DOB",
                    "Age",
                    "Mobile",
                    "Email",
                    "CreatedBy",
                    "CreatedDate",
                    "IsActive",
                    "GenderId",
                    "RegistrationNumber",
                    "AddressLine1",
                    "AddressLine2",
                    "StateId",
                    "DistrictId",
                    "CityId",
                    "SpecializationId",
                    "QualificationId",
                    "PinCode",
                    "Fax",
                    "LoginOTP",
                    "IsLoginOTPActive",
                    "SignaturePath",
                    "CountryId",
                    "StatusId",
                    "RatingMasterId",
                    "SourceId",
                    "IsMaster",
                    "ImagePath",
                    "FileName",
                    "FileFlag",
                    "IsAvailable",
                    "Prefix",
                    "CreationRole"
                }
            };
            bulkLoader.Local = true;
            bulkLoader.TableName = "md_members";
            bulkLoader.FieldTerminator = ",";
            bulkLoader.LineTerminator = "\n";
            bulkLoader.FieldQuotationCharacter = '"';
            bulkLoader.SourceStream = stream;
            bulkLoader.NumberOfLinesToSkip = 1;
            return await bulkLoader.LoadAsync();
        }
        public async Task<int> BulkInsertLogin(Stream stream)
        {
            MySqlBulkLoader bulkLoader = new MySqlBulkLoader(bulkLoaderConnection)
            {
                Columns = {
                    "UserName",
                    "Password",
                    "ReferenceId",
                    "IsActive",
                    "SourceId"
                }
            };
            bulkLoader.Local = true;
            bulkLoader.TableName = "md_login";
            bulkLoader.FieldTerminator = ",";
            bulkLoader.LineTerminator = "\n";
            bulkLoader.FieldQuotationCharacter = '"';
            bulkLoader.SourceStream = stream;
            bulkLoader.NumberOfLinesToSkip = 1;
            return await bulkLoader.LoadAsync();
        }
        public async Task<int> BulkInsertMemberSlot(Stream stream)
        {
            MySqlBulkLoader bulkLoader = new MySqlBulkLoader(bulkLoaderConnection)
            {
                Columns = {
                    "MemberId",
                    "Day",
                    "SlotTo",
                    "SlotFrom",
                    "CreatedDate",
                    "IsActive",
                    "SourceId"
                }
            };
            bulkLoader.Local = true;
            bulkLoader.TableName = "md_members_slot";
            bulkLoader.FieldTerminator = ",";
            bulkLoader.LineTerminator = "\n";
            bulkLoader.FieldQuotationCharacter = '"';
            bulkLoader.SourceStream = stream;
            bulkLoader.NumberOfLinesToSkip = 1;
            return await bulkLoader.LoadAsync();
        }

        public async Task<int> BulkInsertMemberInstitution(Stream stream)
        {
            MySqlBulkLoader bulkLoader = new MySqlBulkLoader(bulkLoaderConnection)
            {
                Columns = {
                   "MemberId",
                    "InstitutionId",
                    "IsActive",
                    "SourceId"
                }
            };
            bulkLoader.Local = true;
            bulkLoader.TableName = "mp_member_institution";
            bulkLoader.FieldTerminator = ",";
            bulkLoader.LineTerminator = "\n";
            bulkLoader.FieldQuotationCharacter = '"';
            bulkLoader.SourceStream = stream;
            bulkLoader.NumberOfLinesToSkip = 1;
            return await bulkLoader.LoadAsync();
        }

        public async Task<IEnumerable<string>> FindEmails(IEnumerable<string> emails)
        {
            var singleQuotedEmails = emails.Select(x => { return $"'{x}'"; });
            var emailString = string.Join(',', singleQuotedEmails);
            var sql = "Select Email from md_members where Email IN( " + emailString + ")";
            var result = await Connection.QueryAsync<string>(sql);
            return result;
        }
        public async Task<IEnumerable<string>> FindMobiles(IEnumerable<string> mobiles)
        {
            var singleQuotedMobiles = mobiles.Select(x => { return $"'{x}'"; });
            var mobileString = string.Join(',', singleQuotedMobiles);
            var sql = "Select Mobile from md_members where Mobile IN( " + mobileString + ")";
            var result = await Connection.QueryAsync<string>(sql);
            return result;
        }
        public async Task<IEnumerable<string>> FindUsers(IEnumerable<string> users)
        {
            var singleQuotedUsers = users.Select(x => { return $" UserName like '%{x}%'"; });
            var userString = string.Join(" OR ", singleQuotedUsers);
            var sql = $"Select UserName from md_login Where {userString }";
            var result = await Connection.QueryAsync<string>(sql);
            return result;
        }

        public async Task<IEnumerable<InstitutionModel>> GetInstituations(int minId, int maxId)
        {
            var sql = $"SELECT InstitutionId,Name " +
                 $" FROM md_institution WHERE InstitutionId >= {minId} AND InstitutionId <= {maxId};";
            var result = await Connection.QueryAsync<InstitutionModel>(sql);
            return result;
        }
        public async Task<IEnumerable<MembersModel>> GetMembers(int minId, int maxId)
        {
            var sql = $"SELECT * FROM md_members where MemberId >= {minId} and MemberId <= {maxId};";
            var result = await Connection.QueryAsync<MembersModel>(sql);
            return result;
        }

        public async Task<IEnumerable<MembersModel>> FindMembers(IEnumerable<string> emails)
        {
            //var sql = $"SELECT * FROM md_members where MemberId >= {minId} and MemberId <= {maxId};";
            var singleQuotedEmails = emails.Select(x => { return $"'{x}'"; });
            var emailString = string.Join(',', singleQuotedEmails);
            var sql = "Select MemberId,Email from md_members where Email IN ( " + emailString + ")";
            var result = await Connection.QueryAsync<MembersModel>(sql);
            return result;
        }

        public async Task<IEnumerable<InstitutionModel>> FindInstitutions(IEnumerable<string> emails, IEnumerable<string> mobiles, IEnumerable<string> hfNameWithDistrict)
        {
            var singleQuotedEmails = emails.Select(x => { return $"'{x}'"; });
            var emailString = string.Join(',', singleQuotedEmails);
            var singleQuotedMobiles = mobiles.Select(x => { return $"'{x}'"; });
            var mobileString = string.Join(',', singleQuotedMobiles);
            var singleQuotedhfNames = hfNameWithDistrict.Select(x => { return $"'{x.Replace("'","''")}'"; });
            var hfnameString = string.Join(',', singleQuotedhfNames);
            var sql = "SELECT InstitutionId,Name,AddressLine1, " +
                "AddressLine2,ReferenceNumber," +
                "CountryId,StateId,DistrictId,CityId,PinCode," +
                "Email,Mobile,ImagePath,InstitutionTypeId," +
                "Fax,SourceId,IsActive,StatusId " +
                "FROM md_institution where  Name IN ( " + hfnameString + ") OR Email IN ( " + emailString + ") OR Mobile IN ( " + mobileString + ");";
            var result = await Connection.QueryAsync<InstitutionModel>(sql);
            return result;
        }
        public async Task<IEnumerable<InstitutionModel>> FindInstitutions(IEnumerable<string> hfNameWithDistrict)
        {
           
            var singleQuotedhfNames = hfNameWithDistrict.Select(x => { return $"'{x.Replace("'","''")}'"; });
            var hfnameString = string.Join(',', singleQuotedhfNames);
            var sql = "SELECT InstitutionId,Name,AddressLine1, " +
                "AddressLine2,ReferenceNumber," +
                "CountryId,StateId,DistrictId,CityId,PinCode," +
                "Email,Mobile,ImagePath,InstitutionTypeId," +
                "Fax,SourceId,IsActive,StatusId " +
                "FROM md_institution where  Name IN ( " + hfnameString + ");";
            var result = await Connection.QueryAsync<InstitutionModel>(sql);
            return result;
        }

        public Task<int> GetMaxInstituteId()
        {
            var sql = "SELECT Max(InstitutionId) FROM md_institution;";
            var result = Connection.QueryFirst<int>(sql);
            return Task.FromResult(result);
        }
        public Task<int> GetMaxMemberId()
        {
            var sql = "SELECT Max(MemberId) FROM md_members;";
            var result = Connection.QueryFirst<int>(sql);
            return Task.FromResult(result);
        }
        public async Task<IEnumerable<QualificationModel>> GetQualification()
        {
            var sql = "SELECT QualificationId,QualificationName FROM md_qualification;";
            var result = await Connection.QueryAsync<QualificationModel>(sql);
            return result;
        }
        public async Task<IEnumerable<KeyValue<string, string>>> GetQualifications()
        {
            var sql = "SELECT QualificationId AS Id, QualificationName AS Value FROM md_qualification Order by QualificationName;";
            var result = await Connection.QueryAsync<KeyValue<string, string>>(sql);
            return result;
        }
        public async Task<IEnumerable<StateDistrictCity>> GetStateDistrictCities()
        {
            var sql = "SELECT S.StateId, S.StateName,S.StateCode " +
                        ", D.DistrictId, D.DistrictName " +
                        ", (SELECT ShortDistCode FROM lgd_dist AS L WHERE L.DistrictNameE = D.DistrictName LIMIT 1) AS DistrictShortCode " +
                        ", C.CityId, C.CityName, C.CityCode " +
                        " FROM md_state AS S " +
                        " JOIN md_district AS D ON S.StateId = D.StateId " +
                        " JOIN md_city AS C ON C.DistrictId = D.DistrictId " +
                        " WHERE S.CountryId = 1; ";
            var result = await Connection.QueryAsync<StateDistrictCity>(sql);
            return result;
        }
       
        public async Task<int> BulkInsertMemberMenu(Stream stream)
        {
            MySqlBulkLoader bulkLoader = new MySqlBulkLoader(bulkLoaderConnection)
            {
                Columns = {
                    "RoleId",
                    "MemberId",
                    "MenuMappingId",
                    "IsActive",
                    "InstitutionId",
                    "SourceId"
                }
            };
            bulkLoader.Local = true;
            bulkLoader.TableName = "mp_member_menu";
            bulkLoader.FieldTerminator = ",";
            bulkLoader.LineTerminator = "\n";
            bulkLoader.FieldQuotationCharacter = '"';
            bulkLoader.SourceStream = stream;
            bulkLoader.NumberOfLinesToSkip = 1;
            return await bulkLoader.LoadAsync();
        }

        public async Task<IEnumerable<SubMenuModel>> GetSubMenu()
        {
            // var sql = "SELECT SubMenuId,SubMenuName FROM md_submenu;";
            var sql = "select * from (SELECT " +
                           " md_submenu.SubMenuId, " +
                           " md_submenu.SubMenuName, " +
                           " mp_menu_submenu.MenuMappingId, " +
                           " mp_menu_submenu.menuid " +
                           " FROM md_submenu " +
                           " INNER JOIN mp_menu_submenu " +
                           " ON md_submenu.SubMenuId = mp_menu_submenu.SubMenuId " +
                           " WHERE md_submenu.IsActive = TRUE " +
                           " AND mp_menu_submenu.IsActive = TRUE)  A where A.menuid = 2; ";
            var result = await Connection.QueryAsync<SubMenuModel>(sql);
            return result;
        }
        public async Task<int> BulkInsertAuditTrail(Stream stream)
        {
            MySqlBulkLoader bulkLoader = new MySqlBulkLoader(bulkLoaderConnection)
            {
                Columns = {
                    "Message",
                    "CreatedDate",
                    "IconPath",
                    "MemberId",
                    "ModuleId",
                    "EventId",
                    "AccessType",
                    "LocationIPAddress",
                    "SourceId",
                    "UserTypeId"
                }
            };
            bulkLoader.Local = true;
            bulkLoader.TableName = "md_audittrail";
            bulkLoader.FieldTerminator = ",";
            bulkLoader.LineTerminator = "\n";
            bulkLoader.FieldQuotationCharacter = '"';
            bulkLoader.SourceStream = stream;
            bulkLoader.NumberOfLinesToSkip = 1;
            return await bulkLoader.LoadAsync();
        }
        public async Task<IEnumerable<KeyValue<string, string>>> GetStates()
        {
            var sql = "SELECT S.StateId AS Id, S.StateName AS Value" +
                         " FROM md_state AS S " +
                         " WHERE S.CountryId = 1 Order by S.StateName; ";
            var result = await Connection.QueryAsync<KeyValue<string, string>>(sql);
            return result;
        }

        public async Task<IEnumerable<KeyValue<string, string>>> GetDistrict(string stateId)
        {
            int stateID = string.IsNullOrWhiteSpace(stateId) ? 0 : Convert.ToInt32(stateId);
            var sql = "SELECT D.DistrictId AS Id, D.DistrictName AS Value " +
                         " FROM  md_district AS D " +
                         $" WHERE D.StateId = {stateID} Order by D.DistrictName; ";
            var result = await Connection.QueryAsync<KeyValue<string, string>>(sql);
            return result;
        }

        public async Task<IEnumerable<KeyValue<string, string>>> GetCities(string stateId, string districtId)
        {
            int districtID = string.IsNullOrWhiteSpace(districtId) ? 0 : Convert.ToInt32(districtId);
            var sql = "SELECT C.CityId AS Id, C.CityName AS Value " +
                        " FROM md_city AS C " +
                        $" WHERE C.DistrictId = {districtID} Order by C.CityName; ";
            var result = await Connection.QueryAsync<KeyValue<string, string>>(sql);
            return result;
        }

        public async Task<IEnumerable<SpecializationModel>> GetSpecialities()
        {
            var sql = "SELECT SpecialityId,SpecialityName FROM md_speciality Order by SpecialityName;";
            var result = await Connection.QueryAsync<SpecializationModel>(sql);
            return result;
        }

        public async Task<IEnumerable<KeyValue<string, string>>> GetSpecility()
        {
            var sql = "SELECT SpecialityId AS Id, SpecialityName AS Value FROM md_speciality Order by SpecialityName;";
            var result = await Connection.QueryAsync<KeyValue<string, string>>(sql);
            return result;
        }

        public async Task<int> SetMasterMember(IEnumerable<string> instituteIds)
        {
            var instituteString = string.Join(',', instituteIds);

            var sql = "Update staggingdblive_jan_aws.md_members as md join " +
                      " (SELECT m.IsMaster, min(m.MemberId) as FirstUser, mp.InstitutionId " +
                      " FROM staggingdblive_jan_aws.md_members as m " +
                      " INNER JOIN staggingdblive_jan_aws.mp_member_institution as mp " +
                      " ON m.MemberId = mp.MemberId " +
                      $" where mp.InstitutionId IN ( " + instituteString + ")" +
                      "  group by mp.InstitutionId,m.IsMaster having m.IsMaster=0) as t " +
                      " on md.MemberId = t.FirstUser " +
                      " set md.IsMaster=1;";
            var result = await Connection.ExecuteAsync(sql);
            return result;
        }

        public async Task<IEnumerable<KeyValue<string, string>>> GetHFTypes()
        {
            var sql = "Select InstitutionTypeId AS Id, TypeName AS Value from md_institutiontype;";
            var result = await Connection.QueryAsync<KeyValue<string, string>>(sql);
            return result;
        }

        public async Task<int> RemoveInstitutions(IEnumerable<string> emails, IEnumerable<string> mobiles)
        {
            var singleQuotedEmails = emails.Select(x => { return $"'{x}'"; });
            var emailString = string.Join(',', singleQuotedEmails);
            var singleQuotedMobiles = mobiles.Select(x => { return $"'{x}'"; });
            var mobileString = string.Join(',', singleQuotedMobiles);
            var sql = "DELETE " +
                "FROM md_institution where Email IN (" + emailString + ") AND Mobile IN (" + mobileString + ");";
            var result = await Connection.ExecuteAsync(sql);
            return result;
        }

        public async Task<int> RemoveMembers(IEnumerable<string> emails, IEnumerable<string> mobiles)
        {
            var singleQuotedEmails = emails.Select(x => { return $"'{x}'"; });
            var emailString = string.Join(',', singleQuotedEmails);
            var singleQuotedMobiles = mobiles.Select(x => { return $"'{x}'"; });
            var mobileString = string.Join(',', singleQuotedMobiles);
            var sql = "DELETE " +
                "FROM md_members where Email IN (" + emailString + ") AND Mobile IN (" + mobileString + ");";
            var result = await Connection.ExecuteAsync(sql);
            return result;
        }

        public async Task<int> RemoveMemberMenus(IEnumerable<string> memberIds)
        {
            var memberIdString = string.Join(',', memberIds);
            var sql = "DELETE " +
                "FROM mp_member_menu where MemberId IN (" + memberIdString + ");";
            var result = await Connection.ExecuteAsync(sql);
            return result;
        }

        public async Task<int> RemoveMemberSlots(IEnumerable<string> memberIds)
        {
            var memberIdString = string.Join(',', memberIds);
            var sql = "DELETE " +
                "FROM md_members_slot where MemberId IN (" + memberIdString + ");";
            var result = await Connection.ExecuteAsync(sql);
            return result;
        }

        public async Task<int> RemoveLogins(IEnumerable<string> memberIds)
        {
            var memberIdString = string.Join(',', memberIds);
            var sql = "DELETE " +
                "FROM md_login where ReferenceId IN (" + memberIdString + ");";
            var result = await Connection.ExecuteAsync(sql);
            return result;
        }

        public async Task<int> RemoveMemberInstitution(IEnumerable<string> instituteIds, IEnumerable<string> memberIds)
        {
            var memberIdString = string.Join(',', memberIds);
            var instituteIdString = string.Join(',', instituteIds);
            var sql = "DELETE " +
                "FROM mp_member_institution where MemberId IN (" + memberIdString + ") OR  InstitutionId IN (" + instituteIdString + ");";
            var result = await Connection.ExecuteAsync(sql);
            return result;
        }
    }
}
