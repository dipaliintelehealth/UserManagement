using System;
using System.Collections.Generic;
using System.IO;
using UserManagement.Domain;
using UserManagement.Domain.ViewModel;
using UserManagement.Contract;
using UserManagement.Contract.User;
using UserManagement.Contract.Utility;
using UserManagement.Contract.Repository;
using UserManagement.Business.Validators;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Infrastructure.Files;
using System.Globalization;
using System.Text.RegularExpressions;

namespace UserManagement.Business
{
    public class MemberBulkDataImportService : IBulkDataImportService<MemberBulkImportVM>
    {
        private readonly IExcelFileUtility<MemberBulkImportVM> excelFileUtility;
        private readonly IMemberBulkInsertRepository bulkInsertRepository;
        private MemberBulkImportVM obj = new MemberBulkImportVM();
        private ExcelConfiguration excelConfiguration;
        private string _pathForCsv;

        public MemberBulkDataImportService(IExcelFileUtility<MemberBulkImportVM> excelFileUtility, IMemberBulkInsertRepository bulkInsertRepository)
        {
            this.excelFileUtility = excelFileUtility;
            this.bulkInsertRepository = bulkInsertRepository;
            excelConfiguration = new ExcelConfiguration()
            {
                ColumnPropertyMapping = new Dictionary<string, string>()
                {
                    { "HF Name", nameof(this.obj.HFName)},
                    { "HF Phone", nameof(obj.HFPhone)},
                    { "HF Type", nameof(obj.HFType)},
                    { "NIN", nameof(obj.NIN)},
                    { "HF Email", nameof(obj.HFEmail)},
                    { "State", nameof(obj.State)},
                    { "District", nameof(obj.District)},
                    { "City", nameof(obj.City)},
                    { "Address", nameof(obj.Address)},
                    { "PIN", nameof(obj.PIN)},
                    { "First Name", nameof(obj.FirstName)},
                    { "Last Name",nameof(obj.LastName)},
                    { "User Mobile", nameof(obj.UserMobile)},
                    { "Gender", nameof(obj.Gender)},
                    { "Qualification",nameof(obj.Qualification)},
                    { "Experience (in yrs)", nameof(obj.Experience)},
                    { "Dr Reg No", nameof(obj.DRRegNo)},
                    { "User Email", nameof(obj.UserEmail)},
                    { "Specialization / Designation", nameof(obj.Designation)},
                    { "Date of Birth DD-MM-YYYY", nameof(obj.DOB)},
                    { "User State", nameof(obj.UserState)},
                    { "User District", nameof(obj.UserDistrict)},
                    { "User City", nameof(obj.UserCity)},
                    { "User Address ", nameof(obj.UserAddress) },
                    { "User PIN", nameof(obj.UserPin) },
                    { "User Prefix", nameof(obj.UserPrefix) },
                    { "Day and Time (Availability)", nameof(obj.UserAvilableDay) },
                    { "FromTime", nameof(obj.UserAvilableFromTime) },
                    { "To Time", nameof(obj.UserAvilableToTime) },
                    { "Role", nameof(obj.UserRole)},
                    { "Assign Type", nameof(obj.AssignedHFType) },
                    { "Assign PHC Or Hub", nameof(obj.AssignHF) },
                    { "Sub Menu", nameof (obj.SubMenuName)}
                }
                ,
                DateTimeFormat = "dd-MM-yyyy"
            };
        }
        public async Task<IEnumerable<MemberBulkImportVM>> CreateModels(Stream stream)
        {
            excelFileUtility.Configure(excelConfiguration);
            var models = excelFileUtility.Read(stream);
            var results = Enumerable.Empty<ResultModel<MemberBulkImportVM>>();
            var validator = new MemberBulkImportVMValidator();
            var institutions = await bulkInsertRepository.GetInstitution();
            var states = await bulkInsertRepository.GetStateDistrictCities();
            models = await GetModelsWithStateDistrictAndCityId(models, states, institutions);
            return models;
        }
        
        public async Task<IEnumerable<ResultModel<MemberBulkImportVM>>> ImportData(Stream stream, string pathForCsvLog)
        {
            this._pathForCsv = pathForCsvLog;
            excelFileUtility.Configure(excelConfiguration);
            var models = excelFileUtility.Read(stream);
            var results = Enumerable.Empty<ResultModel<MemberBulkImportVM>>();
            var validator = new MemberBulkImportVMValidator();
            var institutions = await bulkInsertRepository.GetInstitution();
            var states = await bulkInsertRepository.GetStateDistrictCities();
            models = await GetModelsWithStateDistrictAndCityId(models, states, institutions);
            var subMenu = await bulkInsertRepository.GetSubMenu();
            results = await CheckUserDuplicate(models);
            results = await CheckSubMenu(results, subMenu);
            foreach (var result in results)
            {
                var validationResult = validator.Validate(result.Model);
                if (!validationResult.IsValid)
                {
                    result.Success = false;
                    result.Messages.AddRange(validationResult.Errors.Select(x => x.ErrorMessage));
                }
            }
            var validatedModels = results.Where(x => x.Success);
            if (validatedModels.Count() > 0)
            {
                var users = await bulkInsertRepository.FindUsers(validatedModels.Select(x => GetHFNameForLogin(x.Model.HFName)).Distinct());
                validatedModels = await this.CreateUserName(validatedModels, users, states);

                validatedModels = await this.CreateServiceProvider(validatedModels,institutions);
                validatedModels = await this.CreateMember(validatedModels);
                validatedModels = await this.CreateLogin(validatedModels);
                validatedModels = await this.CreateMemberSlot(validatedModels);
                validatedModels = await this.CreateMemberInstitution(validatedModels);
                validatedModels = await this.CreateMemberMenu(validatedModels, subMenu);
                validatedModels = await this.CreateAuditTrail(validatedModels);
            }
            return results;
        }

        public Task<IEnumerable<ResultModel<MemberBulkImportVM>>> CreateUserName(IEnumerable<ResultModel<MemberBulkImportVM>> validatedModels, IEnumerable<string> users, IEnumerable<StateDistrictCity> states)
        {
            var modelReturns = validatedModels;
            var duplicateUsersGroups = modelReturns.GroupBy(x => x.Model.UserName);
            foreach (var duplicateUserGroup in duplicateUsersGroups)
            {
                var initialCount = 0;
                var user = duplicateUserGroup.FirstOrDefault();
                string stateShortCode = GetStateShortCode(user.Model.UserState);

                string distShortCode = GetDistrictShortCode(states, user.Model.UserDistrict);
                string strHFname = GetHFNameForLogin(user.Model.HFName);
                var strTypeShortCode = GetHFTypeCode(user.Model.HFType);
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
                        item.Model.UserName = $"{firstpart}{initialCount}{secondpart}";
                    }
                    initialCount++;
                }
            }
            return Task.FromResult(modelReturns);
        }

        private async Task<IEnumerable<ResultModel<MemberBulkImportVM>>> CheckUserDuplicate(IEnumerable<MemberBulkImportVM> models)
        {
            var results = new List<ResultModel<MemberBulkImportVM>>();
            var emails = await bulkInsertRepository.FindEmails(models.Select(mdel => mdel.UserEmail));
            var mobiles = await bulkInsertRepository.FindMobiles(models.Select(mdel => mdel.UserMobile));
            // var users = await bulkInsertRepository.FindUsers(models.Select(mdel => mdel.UserName));

            var invalidModels = models.Where(model => emails.Contains(model.UserEmail) || mobiles.Contains(model.UserMobile));
            var validModels = models.Where(model => !(emails.Contains(model.UserEmail) || mobiles.Contains(model.UserMobile)));

            results.AddRange(invalidModels.Select(model => new ResultModel<MemberBulkImportVM>()
            {
                Model = model,
                Success = false,
                Messages = new List<string> { "Duplicate User" }
            }));
            results.AddRange(validModels.Select(model => new ResultModel<MemberBulkImportVM>()
            {
                Model = model,
            }));
            return results;
        }
        public Task<IEnumerable<ResultModel<MemberBulkImportVM>>> CheckSubMenu(IEnumerable<ResultModel<MemberBulkImportVM>> models ,IEnumerable<SubMenuModel> subMenus)
        {
           
            var subMenuNames = subMenus.Select(x => x.SubMenuName.Trim());
            var results = new List<ResultModel<MemberBulkImportVM>>();
            foreach (var item in models)
            {
                var memberMenus = item.Model.SubMenuName.Trim()?.Split(",")?.ToList();
                
                if (memberMenus == null || memberMenus.Count() == 0 || memberMenus.Any(t => !subMenuNames.Contains(t.Trim())))
                {
                    item.Success = false;
                    item.Messages.Add("Invalid Sub Menu !");
                }
                results.Add(item);
            }
                          
            return Task.FromResult(results.AsEnumerable());
        }

        private async Task<IEnumerable<MemberBulkImportVM>> GetModelsWithStateDistrictAndCityId(IEnumerable<MemberBulkImportVM> bulkImportVMs, IEnumerable<StateDistrictCity> states, IEnumerable<InstitutionModel> institutions)
        {
            var qualifications = await bulkInsertRepository.GetQualification();

            var models = bulkImportVMs.Select(x =>
            {
                x.StateId = GetStateId(states, x.State);
                x.DistrictId = GetDistrictId(states, x.State, x.District);
                x.CityId = GetCityId(states, x.State, x.District, x.City);
                x.City = GetCityName(states, x.State, x.District, x.City);
                x.UserStateId = GetStateId(states, x.UserState);
                x.UserDistrictId = GetDistrictId(states, x.UserState, x.UserDistrict);
                x.UserCityId = GetCityId(states, x.UserState, x.UserDistrict, x.UserCity);
                x.UserCity= GetCityName(states, x.UserState, x.UserDistrict, x.UserCity);
                x.UserDistrictShortCode = GetDistrictShortCode(states, x.UserState, x.UserDistrict);
                x.UserName = GetUsersName(states, x.UserState, x.UserDistrict, x.HFName, x.HFType);
                x.QualificationId = GetQualificationId(qualifications, x.Qualification);
                x.InstituteID = GetInstitutionId(institutions, x.HFName);
                x.AssignedInstituteID = GetInstitutionId(institutions, x.AssignHF);
                return x;
            });
            return models;
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
        private async Task<IEnumerable<ResultModel<MemberBulkImportVM>>> CreateServiceProvider(IEnumerable<ResultModel<MemberBulkImportVM>> models,IEnumerable<InstitutionModel> institutions)
        {

            var modelReturns = models;
            var bulkimports = models.Select(x => x.Model);
            var validModels = bulkimports?.Where(x => !institutions.Any(i => i.Name.ToLower().Equals(x.HFNameWithDistrictName?.ToLower())));
            var institutes = validModels?.Distinct(new CompareHFNameWithDistrictName()).Where(x => x.InstituteID == default(string))
                 .Select(x => new InstitutionModelForCsv()
                 {

                     Name = x.HFNameWithDistrictName,
                     AddressLine1 = x.Address,
                     AddressLine2 = string.Empty,
                     ReferenceNumber = x.NIN,
                     CountryId = 1,
                     StateId = x.StateId,
                     DistrictId = x.DistrictId,
                     CityId = x.CityId,
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
                records = await bulkInsertRepository.BulkInsertInstitution(stream);

            }
            var maxInstituteId = await bulkInsertRepository.GetMaxInstituteId();
            var max = Math.Max((maxInstituteId - records) + 1, maxInstituteId);
            var min = Math.Min((maxInstituteId - records) + 1, maxInstituteId);
            var results = await bulkInsertRepository.GetInstituations(min,max);
            var tempInstitutions = institutions.Concat(results);
            modelReturns = modelReturns.Select(x =>
            {
                var find = tempInstitutions.FirstOrDefault(r => r.Name?.Trim().ToLower() == x.Model.HFNameWithDistrictName?.Trim().ToLower());
                if (find != null)
                {
                    x.Model.InstituteID = Convert.ToString(find.InstitutionId);
                }
                return x;
            });

            return modelReturns;
        }


        private async Task<IEnumerable<ResultModel<MemberBulkImportVM>>> CreateMember(IEnumerable<ResultModel<MemberBulkImportVM>> models)
        {
            var modelReturns = models;
            var members = models.Select(x => new MembersModelForCsv()
            {

                FirstName = x.Model.FirstName,
                MiddleName = string.Empty,
                LastName = x.Model.LastName,
                AgeType = 1,
                DOB = x.Model.DOB,
                Age = 0,
                Mobile = x.Model.UserMobile,
                Email = x.Model.UserEmail,
                ImagePath = string.Empty,
                CreatedBy = 1,
                CreatedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                IsActive = 1,
                GenderId = x.Model.GenderId,
                RegistrationNumber = x.Model.DRRegNo,
                AddressLine1 = x.Model.Address,
                AddressLine2 = string.Empty,
                StateId = x.Model.UserStateId,
                DistrictId = x.Model.UserDistrictId,
                CityId = x.Model.UserCityId,
                SpecializationId = 0,
                QualificationId = x.Model.QualificationId,
                PinCode = x.Model.UserPin,
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
                CreationRole = x.Model.UserRole
            });
            if (members != null && members.Count() > 0)
            {
                var csvUtility = new MembersModelForCsvUtility();
                csvUtility.Configure(new CsvConfiguration()
                {
                    CsvLogPath = this._pathForCsv
                });
                var stream = csvUtility.Write(members);

                var records = await bulkInsertRepository.BulkInsertMembers(stream);
                var maxMemberId = await bulkInsertRepository.GetMaxMemberId();
                var results = await bulkInsertRepository.GetMembers((maxMemberId - records) + 1, maxMemberId);

                var result = modelReturns.Select(x =>
                {
                    var find = results.FirstOrDefault(r => r.Email == x.Model.UserEmail);
                    if (find != null)
                    {
                        x.Model.MemberId = Convert.ToString(find.MemberId);
                    }
                    return x;
                });
                return result;
            }
            return modelReturns;
        }

        private async Task<IEnumerable<ResultModel<MemberBulkImportVM>>> CreateLogin(IEnumerable<ResultModel<MemberBulkImportVM>> models)
        {
            var modelReturns = models;
            var logins = models.Select(x => new LoginModelForCsv()
            {
                UserName = x.Model.UserName,
                Password = "ba3253876aed6bc22d4a6ff53d8406c6ad864195ed144ab5c87621b6c233b548baeae6956df346ec8c17f5ea10f35ee3cbc514797ed7ddd3145464e2a0bab413",
                ReferenceId = x.Model.MemberId,
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
                var records = await bulkInsertRepository.BulkInsertLogin(stream);
            }
            return modelReturns;
        }
        private async Task<IEnumerable<ResultModel<MemberBulkImportVM>>> CreateMemberSlot(IEnumerable<ResultModel<MemberBulkImportVM>> models)
        {
            var modelReturns = models;
            var memberSlots = models.Select(x => new MemberSlotModelForCsv()
            {
                MemberId = x.Model.MemberId,
                Day = x.Model.UserAvilableDay,
                SlotTo = x.Model.UserAvilableToTime,
                SlotFrom = x.Model.UserAvilableFromTime,
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
                var records = await bulkInsertRepository.BulkInsertMemberSlot(stream);
            }
            return modelReturns;
        }
        private async Task<IEnumerable<ResultModel<MemberBulkImportVM>>> CreateMemberInstitution(IEnumerable<ResultModel<MemberBulkImportVM>> models)
        {
            var modelReturns = models;
            var memberInstitutions = models.Select(x => new MemberInstitutionModel()
            {
                InstitutionId = x.Model.InstituteID,
                MemberId = x.Model.MemberId,
                IsActive = true,
                SourceId = "99"
            });

            if (memberInstitutions != null && memberInstitutions.Count() > 0)
            {
                var csvUtility = new MemberInstitutionModelCsvUtility();
                csvUtility.Configure(new CsvConfiguration()
                {
                    CsvLogPath = this._pathForCsv
                });
                var stream = csvUtility.Write(memberInstitutions);
                var records = await bulkInsertRepository.BulkInsertMemberInstitution(stream);
            }
            return modelReturns;
        }

        private async Task<IEnumerable<ResultModel<MemberBulkImportVM>>> CreateMemberMenu(IEnumerable<ResultModel<MemberBulkImportVM>> models, IEnumerable<SubMenuModel> subMenu)
        {
            var modelReturns = models;
            IEnumerable<MemberMenuModelForCsv> memberMenus = GetMemberMenus(models, subMenu);

            if (memberMenus != null && memberMenus.Count() > 0)
            {
                var csvUtility = new MemberMenuModelCsvUtility();
                csvUtility.Configure(new CsvConfiguration()
                {
                    CsvLogPath = this._pathForCsv
                });
                var stream = csvUtility.Write(memberMenus);
                var records = await bulkInsertRepository.BulkInsertMemberMenu(stream);
            }
            return modelReturns;
        }

        public IEnumerable<MemberMenuModelForCsv> GetMemberMenus(IEnumerable<ResultModel<MemberBulkImportVM>> models, IEnumerable<SubMenuModel> subMenus)
        {
            return models.SelectMany(x =>
            {
                var memberMenus = x.Model.SubMenuName.Trim().Split(',').ToList();
               
                var listMenu = new List<MemberMenuModelForCsv>();
                foreach (var item in memberMenus)
                {
                    var menuMapID = subMenus.FirstOrDefault(t => t.SubMenuName == item.Trim())?.MenuMappingId;
                    var menu = new MemberMenuModelForCsv()
                    {
                        RoleId = x.Model.UserRole,
                        MemberId = x.Model.MemberId,
                        MenuMappingId = menuMapID,
                        IsActive = "1",
                        InstitutionId = x.Model.InstituteID,
                        SourceId = "99"
                    };
                    listMenu.Add(menu);
                }
                return listMenu;
            });
        }
      private async Task<IEnumerable<ResultModel<MemberBulkImportVM>>> CreateAuditTrail(IEnumerable<ResultModel<MemberBulkImportVM>> models)
        {
            var modelReturns = models;
            var members = models.Select(x => new AuditTrailModelForCsv()
            {
                Message = "Bulk Institution and Member Added Successfully",
                CreatedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                IconPath = " ",
                MemberId = x.Model.MemberId,
                ModuleId = "11",
                EventId = "14",
                AccessType = " ",
                LocationIPAddress = " ",
                SourceId = "99",
                UserTypeId = "2"
            });

            if (members != null && members.Count() > 0)
            {
                var csvUtility = new AuditTrailModelCsvUtility();
                csvUtility.Configure(new CsvConfiguration()
                {
                    CsvLogPath = this._pathForCsv
                });
                var stream = csvUtility.Write(members);
                var records = await bulkInsertRepository.BulkInsertAuditTrail(stream);
            }

            return modelReturns;
        }
    }
}
