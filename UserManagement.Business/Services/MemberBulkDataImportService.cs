using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using UserManagement.Contract;
using UserManagement.Contract.Repository;
using UserManagement.Contract.Utility;
using UserManagement.Domain;
using UserManagement.Domain.ViewModel;
using UserManagement.Infrastructure.Files;

namespace UserManagement.Business.Services
{
    public class MemberBulkDataImportService : IBulkDataImportService<MemberBulkImportVM>
    {
        private readonly IExcelFileUtility<MemberBulkImportVM> _excelFileUtility;
        private readonly IMemberBulkInsertRepository _bulkInsertRepository;
        private MemberBulkImportVM _obj = new MemberBulkImportVM();
        private readonly ExcelConfiguration _excelConfiguration;
        private string _pathForCsv = string.Empty;

        public MemberBulkDataImportService(IExcelFileUtility<MemberBulkImportVM> excelFileUtility, IMemberBulkInsertRepository bulkInsertRepository)
        {
            this._excelFileUtility = excelFileUtility;
            this._bulkInsertRepository = bulkInsertRepository;
            _excelConfiguration = new ExcelConfiguration()
            {
                ColumnPropertyMapping = new Dictionary<string, string>()
                {
                    { "HF Name", nameof(this._obj.HFName)},
                    { "HF Phone", nameof(_obj.HFPhone)},
                    { "HF Type", nameof(_obj.HFType)},
                    { "NIN", nameof(_obj.NIN)},
                    { "HF Email", nameof(_obj.HFEmail)},
                    { "State", nameof(_obj.HFState)},
                    { "District", nameof(_obj.HFDistrict)},
                    { "City", nameof(_obj.HFCity)},
                    { "Address", nameof(_obj.Address)},
                    { "PIN", nameof(_obj.PIN)},
                    { "First Name", nameof(_obj.FirstName)},
                    { "Last Name",nameof(_obj.LastName)},
                    { "User Mobile", nameof(_obj.UserMobile)},
                    { "Gender", nameof(_obj.Gender)},
                    { "Qualification",nameof(_obj.Qualification)},
                    { "Experience (in yrs)", nameof(_obj.Experience)},
                    { "Dr Reg No", nameof(_obj.DRRegNo)},
                    { "User Email", nameof(_obj.UserEmail)},
                    { "Specialization / Designation", nameof(_obj.Designation)},
                    { "Date of Birth DD-MM-YYYY", nameof(_obj.DOB)},
                    { "User State", nameof(_obj.UserState)},
                    { "User District", nameof(_obj.UserDistrict)},
                    { "User City", nameof(_obj.UserCity)},
                    { "User Address ", nameof(_obj.UserAddress) },
                    { "User PIN", nameof(_obj.UserPin) },
                    { "User Prefix", nameof(_obj.UserPrefix) },
                    { "Day and Time (Availability)", nameof(_obj.UserAvailableDay) },
                    { "FromTime", nameof(_obj.UserAvailableFromTime) },
                    { "To Time", nameof(_obj.UserAvailableToTime) },
                    { "Role", nameof(_obj.UserRole)},
                    { "Assign Type", nameof(_obj.AssignedHFType) },
                    { "Assign PHC Or Hub", nameof(_obj.AssignHF) },
                    { "Sub Menu", nameof (_obj.SubMenuName)}
                }
                ,
                DateTimeFormat = "dd-MM-yyyy"
            };
        }

        public async Task<Result<string>> AddToTemporaryStorage(IEnumerable<MemberBulkImportVM> models)
        {
            var sessionId = Guid.NewGuid();
            var sessionIdInString = Convert.ToString(sessionId);
            const string folderPath = "Logs/Csv";
            var states = await _bulkInsertRepository.GetStateDistrictCities();
            var qualifications = await _bulkInsertRepository.GetQualification();
            var institutions = await _bulkInsertRepository.GetInstitution();
            var data = models.Select(x =>
            {
                x.HFState = states.FirstOrDefault(s => s.StateId == x.SelectedHFStateId)?.StateName;
                x.HFDistrict = states.FirstOrDefault(s => s.DistrictId == x.SelectedHFDistrictId)?.DistrictName;
                x.HFCity = states.FirstOrDefault(s => s.CityId == x.SelectedHFCityId)?.CityName;
                x.UserState = states.FirstOrDefault(s => s.StateId == x.SelectedUserStateId)?.StateName;
                x.UserDistrict = states.FirstOrDefault(s => s.DistrictId == x.SelectedUserDistrictId)?.DistrictName;
                x.UserCity = states.FirstOrDefault(s => s.CityId == x.SelectedUserCityId)?.CityName;
                x.UserDistrictShortCode = GetDistrictShortCode(states, x.UserState, x.UserDistrict);
                x.UserName = GetUsersName(states, x.UserState, x.UserDistrict, x.HFName, x.HFType);
                x.QualificationId = GetQualificationId(qualifications, x.Qualification);
                x.InstituteID = GetInstitutionId(institutions, x.HFName);
                x.AssignedInstituteID = GetInstitutionId(institutions, x.AssignHF);
                return x;
            });
            
            var csvUtility = new MemberBulkImportVmCsvUtility(Convert.ToString(sessionIdInString));
            csvUtility.Configure(new CsvConfiguration() { CsvLogPath = folderPath});
            csvUtility.Write(data);
            return Result.Success(sessionIdInString);
        }

        public async Task<IEnumerable<MemberBulkImportVM>> CreateModels(Stream stream)
        {
            _excelFileUtility.Configure(_excelConfiguration);
            var models = _excelFileUtility.Read(stream);
            var results = Enumerable.Empty<ResultModel<MemberBulkImportVM>>();
            var institutions = await _bulkInsertRepository.GetInstitution();
            var states = await _bulkInsertRepository.GetStateDistrictCities();
            models = await GetModelsWithStateDistrictAndCityId(models, states, institutions);
            return models;
        }

        public async Task<Result<IEnumerable<MemberBulkImportVM>>> ImportData(IEnumerable<MemberBulkImportVM> models, string pathForCsvLog)
        {
            this._pathForCsv = pathForCsvLog;
            if (models.Any())
            {
                var institutions = await _bulkInsertRepository.GetInstitution();
                var states = await _bulkInsertRepository.GetStateDistrictCities();
                var users = await _bulkInsertRepository.FindUsers(models.Select(x => GetHFNameForLogin(x.HFName)).Distinct());
                var subMenu = await _bulkInsertRepository.GetSubMenu();

                var result = await this.CreateUserName(models, users, states);
                result = await this.CreateServiceProvider(result.Value, institutions);
                result = await this.CreateMember(result.Value);
                result = await this.CreateLogin(result.Value);
                result = await this.CreateMemberSlot(result.Value);
                result = await this.CreateMemberInstitution(result.Value);
                result = await this.CreateMemberMenu(result.Value, subMenu);
                result = await this.CreateAuditTrail(result.Value);
                 return result;
                 

               /* return await this.CreateUserName(models, users, states)
                     .Check(t => this.CreateServiceProvider(t, institutions))
                     .Check(t => this.CreateMember(t))
                     .Check(t => this.CreateLogin(t))
                     .Check(t => this.CreateMemberSlot(t))
                     .Check(t => this.CreateMemberInstitution(t))
                     .Check(t => this.CreateMemberMenu(t, subMenu))
                     .Check(t => this.CreateAuditTrail(t));*/
                    


            }
            return Result.Failure<IEnumerable<MemberBulkImportVM>>("No data to import");
        }
        public Task<Result<IEnumerable<MemberBulkImportVM>>> CreateUserName(IEnumerable<MemberBulkImportVM> validatedModels, IEnumerable<string> users, IEnumerable<StateDistrictCity> states)
        {
            var modelReturns = new List<MemberBulkImportVM>();
            var duplicateUsersGroups = validatedModels.GroupBy(x => x.UserName);
            foreach (var duplicateUserGroup in duplicateUsersGroups)
            {
                var initialCount = 0;
                var user = duplicateUserGroup.FirstOrDefault();
                string stateShortCode = GetStateShortCode(user.UserState);

                string distShortCode = GetDistrictShortCode(states, user.UserDistrict);
                string strHFname = GetHFNameForLogin(user.HFName);
                var strTypeShortCode = GetHFTypeCode(user.HFType);
                var firstpart = $"{stateShortCode}{strHFname}";
                var secondpart = $"{distShortCode}{strTypeShortCode}";

                var pattern = $"{firstpart}[0-9]+{secondpart}";

                if (users.Contains(duplicateUserGroup.Key))
                {
                    initialCount = 1;
                    var lastFounduser = users.Where(x => Regex.IsMatch(x, pattern))?.OrderByDescending(x => x).FirstOrDefault();
                    if (!string.IsNullOrEmpty(lastFounduser))
                    {
                        var numberToincrement = lastFounduser.Replace(firstpart, string.Empty).Replace(secondpart, string.Empty);
                        initialCount = string.IsNullOrEmpty(numberToincrement) ? initialCount : int.Parse(numberToincrement) + 1;
                    }
                }
                foreach (var item in duplicateUserGroup)
                {
                    if (initialCount > 0)
                    {
                        item.UserName = $"{firstpart}{initialCount}{secondpart}";
                    }
                    initialCount++;
                    modelReturns.Add(item);
                }
            }

            return Task.FromResult(Result.Success(modelReturns.AsEnumerable()));
        }

        private async Task<IEnumerable<MemberBulkImportVM>> GetModelsWithStateDistrictAndCityId(IEnumerable<MemberBulkImportVM> bulkImportVMs, IEnumerable<StateDistrictCity> states, IEnumerable<InstitutionModel> institutions)
        {
            var qualifications = await _bulkInsertRepository.GetQualification();
            var models = new List<MemberBulkImportVM>();
            foreach (var model in bulkImportVMs)
            {
                model.SelectedHFStateId = GetStateId(states, model.HFState);
                model.SelectedHFDistrictId = GetDistrictId(states, model.HFState, model.HFDistrict);
                model.SelectedHFCityId = GetCityId(states, model.HFState, model.HFDistrict, model.HFCity);
                model.HFCity = GetCityName(states, model.HFState, model.HFDistrict, model.HFCity);
                model.SelectedUserStateId = GetStateId(states, model.UserState);
                model.SelectedUserDistrictId = GetDistrictId(states, model.UserState, model.UserDistrict);
                model.SelectedUserCityId = GetCityId(states, model.UserState, model.UserDistrict, model.UserCity);
                model.UserCity = GetCityName(states, model.UserState, model.UserDistrict, model.UserCity);
                model.UserDistrictShortCode = GetDistrictShortCode(states, model.UserState, model.UserDistrict);
                model.UserName = GetUsersName(states, model.UserState, model.UserDistrict, model.HFName, model.HFType);
                model.QualificationId = GetQualificationId(qualifications, model.Qualification);
                model.InstituteID = GetInstitutionId(institutions, model.HFName);
                model.AssignedInstituteID = GetInstitutionId(institutions, model.AssignHF);
                model.HFDistricts = GetDistricts(states, model.HFState);
                model.UserDistricts = GetDistricts(states, model.UserState);
                model.HFCities = GetCities(states, model.HFState, model.HFDistrict);
                model.UserCities = GetCities(states, model.UserState, model.UserDistrict);

                models.Add(model);
            }
            return models;
        }

        private IEnumerable<KeyValue<string,string>> GetCities(IEnumerable<StateDistrictCity> states, string state, string district)
        {
            var cities = states
                .Where(x => x.StateName.ToUpper() == state?.Trim().ToUpper() && x.DistrictName.ToUpper() == district?.Trim().ToUpper())
                .Select(s => new KeyValue<string,string> {Id = s.CityId.ToString(), Value = s.CityName });

            return cities.Distinct();
        } 
        private IEnumerable<KeyValue<string, string>> GetDistricts(IEnumerable<StateDistrictCity> states, string state)
        {
            var districts = states
                .Where(x => x.StateName.ToUpper() == state?.Trim().ToUpper())
                .Select(s => new KeyValue<string, string> { Id = s.DistrictId.ToString(), Value = s.DistrictName });

            return districts.Distinct();
        }
        private string GetCityName(IEnumerable<StateDistrictCity> states, string state, string district, string city)
        {
            var cities = states
                .Where(x => x.StateName.ToUpper() == state?.Trim().ToUpper() && x.DistrictName.ToUpper() == district?.Trim().ToUpper())
                .Select(s => new { CityId = s.CityId, CityName = s.CityName });

            var cityName = city;
            if (!cities.Any(x => x.CityName.ToUpper() == city?.Trim().ToUpper()))
            {
                var firstCity = cities?.OrderBy(x => x.CityName).FirstOrDefault()?.CityName;
                cityName = firstCity ?? city;
            }

            return cityName;
        }

        public string GetUsersName(IEnumerable<StateDistrictCity> states, string StateName, string DistrictName, string HFName, string Type)
        {
            //State Code (2 alphabet)______Name of HF_____District Code (3 alphabet)______Type of HF (hub/phc/uphc/sc)
            string StateShortCode = GetStateShortCode(StateName);

            string DistShortCode = GetDistrictShortCode(states, DistrictName);
            string strHFname = GetHFNameForLogin(HFName);
            var strTypeShortCode = "";
            string stRes = "";

            strTypeShortCode = GetHFTypeCode(Type);
            var firstpart = $"{StateShortCode}{strHFname}";
            var secondpart = $"{DistShortCode}{strTypeShortCode}";

            var userName = $"{firstpart}{secondpart}";
            //strHFname
            return userName;
        }

        private static string GetHFTypeCode(string Type)
        {
            string strTypeShortCode;
            if (Type?.Trim().ToUpper() == "SUBCENTRE")
            {
                strTypeShortCode = "SC";
            }
            else
            {
                strTypeShortCode = Type;
            }

            return strTypeShortCode?.ToLower();
        }

        public static string GetHFNameForLogin(string HFName)
        {
            var hf = new List<string>()
            {
                "UPHC","HSC","PHC","HUB","SC"
            };

            string strHFname = HFName;
            strHFname = strHFname?.Trim().ToLower();
            hf.ForEach((item) =>
            {
                strHFname = strHFname?.Replace(item.ToLower(), "");
            });

            if (string.IsNullOrEmpty(strHFname))
            {
                return string.Empty;
            }
            string strHFnametrimmed = String.Concat(strHFname?.Where(c => !Char.IsWhiteSpace(c)));
            return strHFnametrimmed;
        }

        private static string GetDistrictShortCode(IEnumerable<StateDistrictCity> states, string districtName)
        {
            var distShortCode = states.FirstOrDefault(x => x.DistrictName.ToUpper() == districtName?.Trim().ToUpper())?.DistrictShortCode;
            if (!string.IsNullOrWhiteSpace(districtName) && (string.IsNullOrEmpty(distShortCode) || distShortCode.Length < 2))
            {
                if (districtName.Replace(" ", "").Length > 2)
                {
                    distShortCode = districtName.Replace(" ", "").Substring(0, districtName.Length - 1);
                }
                else
                {
                    distShortCode = districtName.Replace(" ", "");
                }
            }

            return distShortCode?.ToLower() ?? string.Empty;
        }

        private static string GetStateShortCode(string StateName)
        {
            Dictionary<string, string> StateAndCodes = new Dictionary<string, string>()
            {
                    {"ANDHRA PRADESH","AP"},
                    {"ARUNACHAL PRADESH","AR"},
                    {"ASSAM","AS"},
                    {"BIHAR","BR"},
                    {"CHHATTISGARH","CG"},
                    {"DELHI","DL"},
                    {"GOA","GA"},
                    {"GUJARAT","GJ"},
                    {"HARYANA","HR"},
                    {"HIMACHAL PRADESH","HP"},
                    {"JAMMU & KASHMIR","JK"},
                    {"JHARKHAND","JS"},
                    {"KARNATAKA","KA"},
                    {"KERALA","KL"},
                    {"MADHYA PRADESH","MP"},
                    {"MAHARASHTRA","MH"},
                    {"MANIPUR","MN"},
                    {"MEGHALAYA","ML"},
                    {"MIZORAM","MZ"},
                    {"NAGALAND","NL"},
                    {"ORISSA","OR"},
                    {"PUNJAB","PB"},
                    {"RAJASTHAN","RJ"},
                    {"SIKKIM","SK"},
                    {"TAMIL NADU","TN"},
                    {"TRIPURA","TR"},
                    {"UTTARAKHAND","UK"},
                    {"UTTAR PRADESH","UP"},
                    {"WEST BENGAL","WB"},
                    {"ANDAMAN & NICOBAR","AN"},
                    {"CHANDIGARH","CH"},
                    {"DADRA AND NAGAR HAVELI","DN"},
                    {"DAMAN & DIU","DD"},
                    {"LAKSHADWEEP","LD"},
                    {"PONDICHERRY","PY"},
                    {"PUDUCHERRY","PY"},
                    {"ODISHA","OD"}

            };

            string StateShortCode = "";

            if (StateAndCodes.ContainsKey(StateName.Trim().ToUpper()))
            {
                StateShortCode = StateAndCodes[StateName.Trim().ToUpper()];
            }
            else if (!string.IsNullOrEmpty(StateName) && StateName.Length > 2)
            {
                StateShortCode = StateName?.Trim().ToUpper().Substring(0, 2);
            }

            return StateShortCode?.ToLower();
        }

        private int GetStateId(IEnumerable<StateDistrictCity> states, string stateName)
        {
            return states.Where(x => x.StateName.ToUpper() == stateName?.Trim().ToUpper())
                .Select(state => state.StateId)
                .FirstOrDefault();
        }
        private int GetDistrictId(IEnumerable<StateDistrictCity> states, string stateName, string districtName)
        {
            return states.Where(x => x.StateName.ToUpper() == stateName?.Trim().ToUpper() && x.DistrictName.ToUpper() == districtName?.Trim().ToUpper())
                .Select(state => state.DistrictId)
                .FirstOrDefault();
        }
        private int GetCityId(IEnumerable<StateDistrictCity> states, string stateName, string districtName, string cityName)
        {

            var cities = states
                .Where(x => x.StateName.ToUpper() == stateName?.Trim().ToUpper() && x.DistrictName.ToUpper() == districtName?.Trim().ToUpper())
                .Select(state => new { CityId = state.CityId, CityName = state.CityName });

            var cityID = 0;
            if (cities.Any(x => x.CityName.ToUpper() == cityName?.Trim().ToUpper()))
            {
                cityID = cities.Where(x => x.CityName.ToUpper() == cityName?.Trim().ToUpper())
                                .Select(x => x.CityId)
                                .FirstOrDefault();
            }
            else
            {
                var firstCity = cities?.OrderBy(x=>x.CityName).FirstOrDefault()?.CityId;
                cityID = firstCity ?? 0;
            }

            return cityID;
        }

        private string GetDistrictShortCode(IEnumerable<StateDistrictCity> states, string stateName, string districtName)
        {
            return states
                .Where(x => x.StateName.ToUpper() == stateName?.Trim().ToUpper() && x.DistrictName.ToUpper() == districtName?.Trim().ToUpper())
                .Select(state => state.DistrictShortCode)
                .FirstOrDefault();
        }
        private string GetInstitutionId(IEnumerable<InstitutionModel> institutions, string institutionName)
        {
            var result = institutions
                .Where(x => x.Name.ToUpper() == institutionName?.Trim().ToUpper())
                .Select(institution => Convert.ToString(institution.InstitutionId))
                .FirstOrDefault();
            return result;
        }
        private int GetQualificationId(IEnumerable<QualificationModel> qualifications, string qualificationName)
        {
            return qualifications
                .Where(x => x.QualificationName.ToUpper() == qualificationName?.Trim().ToUpper())
                .Select(qualification => qualification.QualificationId)
                .FirstOrDefault();
        }
        private async Task<Result<IEnumerable<MemberBulkImportVM>>> CreateServiceProvider(IEnumerable<MemberBulkImportVM> models, IEnumerable<InstitutionModel> institutions)
        {
            if (models == null) throw new ArgumentNullException(nameof(models));

            var validModels = models?.Where(x => !institutions.Any(i => i.Name.ToLower().Equals(x.HFNameWithDistrictName?.ToLower())));
            var institutes = validModels?.Distinct(new CompareHFNameWithDistrictName()).Where(x => string.IsNullOrWhiteSpace(x.InstituteID))
                 .Select(x => new InstitutionModelForCsv()
                 {

                     Name = x.HFNameWithDistrictName,
                     AddressLine1 = x.Address,
                     AddressLine2 = string.Empty,
                     ReferenceNumber = x.NIN,
                     CountryId = 1,
                     StateId = x.SelectedHFStateId,
                     DistrictId = x.SelectedHFDistrictId,
                     CityId = x.SelectedHFCityId,
                     PinCode = x.PIN,
                     Mobile = x.HFPhone,
                     Email = x.HFEmail,
                     InstitutionTypeId = x.HFTypeId,
                     IsActive = true,
                     CreatedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                     SourceId = 99,
                     StatusId = 1
                 });
            var records = 0;
            if (institutes != null && institutes.Count() > 0)
            {
                var csvUtility = new InstitutionModelCsvUtility();
                csvUtility.Configure(new CsvConfiguration()
                {
                    CsvLogPath = this._pathForCsv
                });
                var stream = csvUtility.Write(institutes);
                records = await _bulkInsertRepository.BulkInsertInstitution(stream);

            }
            var maxInstituteId = await _bulkInsertRepository.GetMaxInstituteId();
            var max = Math.Max((maxInstituteId - records) + 1, maxInstituteId);
            var min = Math.Min((maxInstituteId - records) + 1, maxInstituteId);
            var dbInstitutions = await _bulkInsertRepository.GetInstituations(min,max);
            var tempInstitutions = institutions.Concat(dbInstitutions);
            var results = models.Select(x =>
            {
                var find = tempInstitutions.FirstOrDefault(r => r.Name?.Trim().ToLower() == x.HFNameWithDistrictName?.Trim().ToLower());
                if (find != null)
                {
                    x.InstituteID = Convert.ToString(find.InstitutionId);
                }
                return x;
            });

            return Result.Success(results);
        }


        private async Task<Result<IEnumerable<MemberBulkImportVM>>> CreateMember(IEnumerable<MemberBulkImportVM> models)
        {
            var members = models.Select(x => new MembersModelForCsv()
            {

                FirstName = x.FirstName,
                MiddleName = string.Empty,
                LastName = x.LastName,
                AgeType = 1,
                DOB = x.DOB,
                Age = 0,
                Mobile = x.UserMobile,
                Email = x.UserEmail,
                ImagePath = string.Empty,
                CreatedBy = 1,
                CreatedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                IsActive = 1,
                GenderId = x.GenderId,
                RegistrationNumber = x.DRRegNo,
                AddressLine1 = x.Address,
                AddressLine2 = string.Empty,
                StateId = x.SelectedUserStateId,
                DistrictId = x.SelectedUserDistrictId,
                CityId = x.SelectedUserCityId,
                SpecializationId = 0,
                QualificationId = x.QualificationId,
                PinCode = x.UserPin,
                Fax = string.Empty,
                LoginOTP = 0,
                IsLoginOTPActive = "",
                SignaturePath = string.Empty,
                CountryId = 1,
                StatusId = 1,
                RatingMasterId = 0,
                SourceId = 99,
                IsMaster = "",
                Prefix = string.Empty,
                CreationRole = x.UserRole
            });
           
                var csvUtility = new MembersModelForCsvUtility();
                csvUtility.Configure(new CsvConfiguration()
                {
                    CsvLogPath = this._pathForCsv
                });
                var stream = csvUtility.Write(members);
                var records = await _bulkInsertRepository.BulkInsertMembers(stream);
                
                var maxMemberId = await _bulkInsertRepository.GetMaxMemberId();
                //need to refactor this as we need to get members by email rather than by records.
                var dbRecords = await _bulkInsertRepository.GetMembers((maxMemberId - records) + 1, maxMemberId);

                var results = models.Select(x =>
                {
                    var find = dbRecords.FirstOrDefault(r => r.Email == x.UserEmail);
                    if (find != null)
                    {
                        x.MemberId = Convert.ToString(find.MemberId);
                    }
                    return x;
                });
                return Result.Success(results);
       }

        private async Task<Result<IEnumerable<MemberBulkImportVM>>> CreateLogin(IEnumerable<MemberBulkImportVM> models)
        {
            var modelReturns = models;
            var logins = models.Select(x => new LoginModelForCsv()
            {
                UserName = x.UserName,
                Password = "ba3253876aed6bc22d4a6ff53d8406c6ad864195ed144ab5c87621b6c233b548baeae6956df346ec8c17f5ea10f35ee3cbc514797ed7ddd3145464e2a0bab413",
                ReferenceId = x.MemberId,
                IsActive = 1,
                SourceId = "99"
            });

            if (logins != null && logins.Count() > 0)
            {
                var csvUtility = new LoginModelCsvUtility();
                csvUtility.Configure(new CsvConfiguration()
                {
                    CsvLogPath = this._pathForCsv
                });
                var stream = csvUtility.Write(logins);
                var records = await _bulkInsertRepository.BulkInsertLogin(stream);
            }
            return Result.Success(modelReturns);
        }
        private async Task<Result<IEnumerable<MemberBulkImportVM>>> CreateMemberSlot(IEnumerable<MemberBulkImportVM> models)
        {
            if (models == null) throw new ArgumentNullException(nameof(models));
            
            var modelReturns = models;
            var memberSlots = models.Select(x => new MemberSlotModelForCsv()
            {
                MemberId = x.MemberId,
                Day = x.UserAvailableDay,
                SlotTo = x.UserAvailableToTime,
                SlotFrom = x.UserAvailableFromTime,
                CreatedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                IsActive = true,
                SourceId = "99"
            });

            if (memberSlots != null && memberSlots.Count() > 0)
            {
                var csvUtility = new MemberSlotModelCsvUtility();
                csvUtility.Configure(new CsvConfiguration()
                {
                    CsvLogPath = this._pathForCsv
                });
                var stream = csvUtility.Write(memberSlots);
                var records = await _bulkInsertRepository.BulkInsertMemberSlot(stream);
            }
            return Result.Success(modelReturns);
        }
        private async Task<Result<IEnumerable<MemberBulkImportVM>>> CreateMemberInstitution(IEnumerable<MemberBulkImportVM> models)
        {
            if (models == null) throw new ArgumentNullException(nameof(models));
            
            var modelReturns = models;
            var memberInstitutions = models.Select(x => new MemberInstitutionModel()
            {
                InstitutionId = x.InstituteID,
                MemberId = x.MemberId,
                IsActive = true,
                SourceId = "99"
            });

            var csvUtility = new MemberInstitutionModelCsvUtility();
            csvUtility.Configure(new CsvConfiguration()
            {
                CsvLogPath = this._pathForCsv
            });
            var stream = csvUtility.Write(memberInstitutions);
            await _bulkInsertRepository.BulkInsertMemberInstitution(stream);
            return Result.Success(modelReturns);
        }

        private async Task<Result<IEnumerable<MemberBulkImportVM>>> CreateMemberMenu(IEnumerable<MemberBulkImportVM> models, IEnumerable<SubMenuModel> subMenu)
        {
            var modelReturns = models;
            IEnumerable<MemberMenuModelForCsv> memberMenus = GetMemberMenus(models, subMenu);

            if (memberMenus != null && memberMenus.Any())
            {
                var csvUtility = new MemberMenuModelCsvUtility();
                csvUtility.Configure(new CsvConfiguration()
                {
                    CsvLogPath = this._pathForCsv
                });
                var stream = csvUtility.Write(memberMenus);
                await _bulkInsertRepository.BulkInsertMemberMenu(stream);
            }
            return Result.Success(modelReturns);
        }

        public IEnumerable<MemberMenuModelForCsv> GetMemberMenus(IEnumerable<MemberBulkImportVM> models, IEnumerable<SubMenuModel> subMenus)
        {
            return models.SelectMany(x =>
            {
                var memberMenus = x.SubMenuName.Trim().Split(',').ToList();
               
                var listMenu = new List<MemberMenuModelForCsv>();
                foreach (var item in memberMenus)
                {
                    var menuMapID = subMenus.FirstOrDefault(t => t.SubMenuName == item.Trim())?.MenuMappingId;
                    var menu = new MemberMenuModelForCsv()
                    {
                        RoleId = x.UserRole,
                        MemberId = x.MemberId,
                        MenuMappingId = menuMapID,
                        IsActive = "1",
                        InstitutionId = x.InstituteID,
                        SourceId = "99"
                    };
                    listMenu.Add(menu);
                }
                return listMenu;
            });
        }
      private async Task<Result<IEnumerable<MemberBulkImportVM>>> CreateAuditTrail(IEnumerable<MemberBulkImportVM> models)
        {
            var modelReturns = models;
            var members = models.Select(x => new AuditTrailModel()
            {
                Message = "Bulk Institution and Member Added Successfully",
                CreatedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                IconPath = " ",
                MemberId = x.MemberId,
                ModuleId = "11",
                EventId = "14",
                AccessType = " ",
                LocationIPAddress = " ",
                SourceId = "99",
                UserTypeId = "2"
            });

            if (members != null && members.Any())
            {
                var csvUtility = new AuditTrailModelCsvUtility();
                csvUtility.Configure(new CsvConfiguration()
                {
                    CsvLogPath = this._pathForCsv
                });
                var stream = csvUtility.Write(members);
                await _bulkInsertRepository.BulkInsertAuditTrail(stream);
            }

            return Result.Success(modelReturns);
        }

        public async Task<IEnumerable<KeyValue<string, string>>> GetStates()
        {
            return await _bulkInsertRepository.GetStates();
        }

        public async Task<IEnumerable<KeyValue<string, string>>> GetDistrict(string stateId)
        {
            return await _bulkInsertRepository.GetDistrict(stateId);
        }

        public async Task<IEnumerable<KeyValue<string, string>>> GetCities(string stateId, string districtId)
        {
            return await _bulkInsertRepository.GetCities(stateId, districtId);
        }
    }
}
