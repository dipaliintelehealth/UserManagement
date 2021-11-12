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

namespace UserManagement.Infrastructure.Repository
{
    public class BulkInsertRepository : RepositoryBase, IMemberBulkInsertRepository
    {
        private readonly ILogger<IMemberBulkInsertRepository> Logger;
        public BulkInsertRepository(IConnectionFactory connectionFactory, ILogger<IMemberBulkInsertRepository> logger) : base(connectionFactory)
        {
            // string connString = this.Configuration.GetConnectionString("MyConn");
            Logger = logger;
            Logger.LogInformation("BulkInsertRepository initialized");
        }

        public async Task<int> BulkInsertInstitution(Stream stream)
        {
            var connection = new MySqlConnection($"{Connection.ConnectionString};AllowLoadLocalInfile=True");
            MySqlBulkLoader bulkLoader = new MySqlBulkLoader(connection)
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
            var connection = new MySqlConnection($"{Connection.ConnectionString};AllowLoadLocalInfile=True");
            MySqlBulkLoader bulkLoader = new MySqlBulkLoader(connection)
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
            var connection = new MySqlConnection($"{Connection.ConnectionString};AllowLoadLocalInfile=True");
            MySqlBulkLoader bulkLoader = new MySqlBulkLoader(connection)
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
            var connection = new MySqlConnection($"{Connection.ConnectionString};AllowLoadLocalInfile=True");
            MySqlBulkLoader bulkLoader = new MySqlBulkLoader(connection)
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
            var connection = new MySqlConnection($"{Connection.ConnectionString};AllowLoadLocalInfile=True");
            MySqlBulkLoader bulkLoader = new MySqlBulkLoader(connection)
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
            var sql = $"SELECT * FROM md_institution where InstitutionId >= {minId} and InstitutionId <= {maxId};";
            var result = await Connection.QueryAsync<InstitutionModel>(sql);
            return result;
        }
        public async Task<IEnumerable<MembersModel>> GetMembers(int minId, int maxId)
        {
            var sql = $"SELECT * FROM md_members where MemberId >= {minId} and MemberId <= {maxId};";
            var result = await Connection.QueryAsync<MembersModel>(sql);
            return result;
        }

        public async Task<IEnumerable<InstitutionModel>> GetInstitution()
        {
            var sql = "SELECT InstitutionId,Name FROM md_institution;";
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
        public async Task<string> AddAuditLog()
        {
            var sql = "INSERT INTO  `md_audittrail` ( " +
                 "`Message`, `CreatedDate`, `IconPath`, `MemberId`," +
                 " `ModuleId`, `EventId`, `AccessType`, `LocationIPAddress`," +
                  "`SourceId`,`UserTypeId` )" +
                  "VALUES ( " +
                  $"'Bulk Institution and Member Added Successfully' , '{DateTime.Now.ToString("yyyy-MM-dd") }', " +
                  "'', '1', '11', '14', '','','99','2' ); SELECT LAST_INSERT_ID();";
            var result = await Connection.QueryFirstAsync<string>(sql);
            return result;
        }

        public async Task<int> BulkInsertMemberMenu(Stream stream)
        {
            var connection = new MySqlConnection($"{Connection.ConnectionString};AllowLoadLocalInfile=True");
            MySqlBulkLoader bulkLoader = new MySqlBulkLoader(connection)
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
    }
}
