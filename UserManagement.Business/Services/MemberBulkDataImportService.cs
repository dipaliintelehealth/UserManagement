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
                    { "Assign PHC Or Hub", nameof(obj.AssignHF) }
                }
                ,
                DateTimeFormat = "dd-MM-yyyy"
            };
        }
        public async Task<IEnumerable<ResultModel<MemberBulkImportVM>>> ImportData(Stream stream,string pathForCsvLog)
        {
            this._pathForCsv = pathForCsvLog;
            excelFileUtility.Configure(excelConfiguration);
            var models = excelFileUtility.Read(stream);
            var results = Enumerable.Empty<ResultModel<MemberBulkImportVM>>();
            var validator = new MemberBulkImportVMValidator();
            models = await GetModelsWithStateDistrictAndCityId(models);
            results = await CheckDuplicate(models);
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
                validatedModels = await this.CreateServiceProvider(validatedModels);
                validatedModels = await this.CreateMember(validatedModels);
                validatedModels = await this.CreateLogin(validatedModels);
                validatedModels = await this.CreateMemberSlot(validatedModels);
                validatedModels = await this.CreateMemberInstitution(validatedModels);
                await bulkInsertRepository.AddAuditLog();
            }
            return results;
        }
        private async Task<IEnumerable<ResultModel<MemberBulkImportVM>>> CheckDuplicate(IEnumerable<MemberBulkImportVM> models)
        {
            var results = new List<ResultModel<MemberBulkImportVM>>();
            var emails = await bulkInsertRepository.FindEmails(models.Select(mdel => mdel.UserEmail));
            var mobiles = await bulkInsertRepository.FindMobiles(models.Select(mdel => mdel.UserMobile));
            var users = await bulkInsertRepository.FindUsers(models.Select(mdel => mdel.UserName));

            var invalidModels = models.Where(model => emails.Contains(model.UserEmail) || mobiles.Contains(model.UserMobile) || users.Contains(model.UserName));
            var validModels = models.Where(model => !(emails.Contains(model.UserEmail) || mobiles.Contains(model.UserMobile) || users.Contains(model.UserName)));

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
        private async Task<IEnumerable<MemberBulkImportVM>> GetModelsWithStateDistrictAndCityId(IEnumerable<MemberBulkImportVM> bulkImportVMs)
        {
            var states = await bulkInsertRepository.GetStateDistrictCities();
            var qualifications = await bulkInsertRepository.GetQualification();
            var institutions = await bulkInsertRepository.GetInstitution();
            var models = bulkImportVMs.Select(x =>
            {
                x.StateId = GetStateId(states, x.State);
                x.DistrictId = GetDistrictId(states, x.State, x.District);
                x.CityId = GetCityId(states, x.State, x.District, x.City);
                x.UserStateId = GetStateId(states, x.UserState);
                x.UserDistrictId = GetDistrictId(states, x.UserState, x.UserDistrict);
                x.UserCityId = GetCityId(states, x.UserState, x.UserDistrict, x.UserCity);
                x.UserDistrictShortCode = GetDistrictShortCode(states, x.UserState, x.UserDistrict);
                x.UserName = GetUsersName(states, x.UserState, x.UserDistrict, x.HFName, x.HFType);
                x.QualificationId = GetQualificationId(qualifications, x.Qualification);
                x.InstituteID = GetInstitutionId(institutions, x.HFName);
                x.AssignedInstituteID = GetInstitutionId(institutions, x.AssignHF);
                return x;
            });
            return models;
        }

        public string GetUsersName(IEnumerable<StateDistrictCity> states, string StateName, string DistrictName, string HFName, string Type)
        {
            //State Code (2 alphabet)______Name of HF_____District Code (3 alphabet)______Type of HF (hub/phc/uphc/sc)
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
            if (StateAndCodes.ContainsKey(StateName.ToUpper()))
            {
                StateShortCode = StateAndCodes[StateName.ToUpper()];
            }
            else
            {
                StateShortCode = StateName?.ToUpper().Substring(0, 2);
            }

            var DistShortCode = states.FirstOrDefault(x => x.DistrictName == DistrictName)?.DistrictShortCode;
            if (!string.IsNullOrWhiteSpace(DistrictName) && (string.IsNullOrEmpty(DistShortCode) || DistShortCode.Length < 2))
            {
                if (DistrictName.Replace(" ", "").Length > 2)
                {
                    DistShortCode = DistrictName.Replace(" ", "").Substring(0, DistrictName.Length - 1);
                }
                else
                {
                    DistShortCode = DistrictName.Replace(" ", "");
                }
            }
            var hf = new List<string>()
            {
                "UPHC","HSC","PHC","HUB","SC"
            };

            string strHFname = HFName;
            hf.ForEach((item) =>
            {
                strHFname = strHFname.Replace(item, "");
            });
            strHFname = strHFname.Trim();
            var strTypeShortCode = "";
            string stRes = "";

            if (Type == "SubCentre")
            {
                strTypeShortCode = "SC";
            }
            else
            {
                strTypeShortCode = Type;
            }
            stRes = (StateShortCode + strHFname + DistShortCode + strTypeShortCode).ToLower();
            //strHFname
            return stRes;
        }
        private int GetStateId(IEnumerable<StateDistrictCity> states, string stateName)
        {
            return states.Where(x => x.StateName.ToUpper() == stateName?.ToUpper())
                .Select(state => state.StateId)
                .FirstOrDefault();
        }
        private int GetDistrictId(IEnumerable<StateDistrictCity> states, string stateName, string districtName)
        {
            return states.Where(x => x.StateName.ToUpper() == stateName?.ToUpper() && x.DistrictName.ToUpper() == districtName?.ToUpper())
                .Select(state => state.DistrictId)
                .FirstOrDefault();
        }
        private int GetCityId(IEnumerable<StateDistrictCity> states, string stateName, string districtName, string cityName)
        {
            return states
                .Where(x => x.StateName.ToUpper() == stateName?.ToUpper() && x.DistrictName.ToUpper() == districtName?.ToUpper() && x.CityName.ToUpper() == cityName?.ToUpper())
                .Select(state => state.CityId)
                .FirstOrDefault();
        }
        private string GetDistrictShortCode(IEnumerable<StateDistrictCity> states, string stateName, string districtName)
        {
            return states
                .Where(x => x.StateName.ToUpper() == stateName?.ToUpper() && x.DistrictName.ToUpper() == districtName?.ToUpper())
                .Select(state => state.DistrictShortCode)
                .FirstOrDefault();
        }
        private string GetInstitutionId(IEnumerable<InstitutionModel> institutions, string institutionName)
        {
            var result =institutions
                .Where(x => x.Name.ToUpper() == institutionName?.ToUpper())
                .Select(institution => Convert.ToString(institution.InstitutionId))
                .FirstOrDefault();
            return result;
        }
        private int GetQualificationId(IEnumerable<QualificationModel> qualifications, string qualificationName)
        {
            return qualifications
                .Where(x => x.QualificationName.ToUpper() == qualificationName?.ToUpper())
                .Select(qualification => qualification.QualificationId)
                .FirstOrDefault();
        }
        private async Task<IEnumerable<ResultModel<MemberBulkImportVM>>> CreateServiceProvider(IEnumerable<ResultModel<MemberBulkImportVM>> models)
        {
            var modelReturns = models;
            var institutes = models.Where(x => x.Model.InstituteID == default(string))
                 .Select(x => new InstitutionModelForCsv()
                 {

                     Name = x.Model.HFName,
                     AddressLine1 = x.Model.Address,
                     AddressLine2 = string.Empty,
                     ReferenceNumber = x.Model.NIN,
                     CountryId = 1,
                     StateId = x.Model.StateId,
                     DistrictId = x.Model.DistrictId,
                     CityId = x.Model.CityId,
                     PinCode = x.Model.PIN,
                     Mobile = x.Model.HFPhone,
                     Email = x.Model.HFEmail,
                     InstitutionTypeId = x.Model.HFTypeId,
                     IsActive = true,
                     CreatedDate = DateTime.Now.ToString("yyyy-MM-dd"),
                     SourceId = 99,
                     StatusId = 1
                 });
            if (institutes != null && institutes.Count() > 0)
            {
                var csvUtility = new InstitutionModelCsvUtility();
                csvUtility.Configure(new CsvConfiguration()
                {
                    CsvLogPath = this._pathForCsv
                });
                var stream =csvUtility.Write(institutes);
                var records = await bulkInsertRepository.BulkInsertInstitution(stream);
                var maxInstituteId = await bulkInsertRepository.GetMaxInstituteId();
                var results = await bulkInsertRepository.GetInstituations((maxInstituteId-records)+1, maxInstituteId);

                var result = modelReturns.Select(x =>
                  {
                      var find = results.FirstOrDefault(r => r.Name == x.Model.HFName);
                      if (find != null)
                      {
                          x.Model.InstituteID = Convert.ToString(find.InstitutionId);
                      }
                      return x;
                  });
                return result;
            }
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
                CreatedDate = DateTime.Now.ToString("yyyy-MM-dd"),
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
                IsLoginOTPActive = 0,
                SignaturePath = string.Empty,
                CountryId = 1,
                StatusId = 1,
                RatingMasterId = 0,
                SourceId = 0,
                IsMaster = 0,
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
                var results = await bulkInsertRepository.GetMembers((maxMemberId -records) + 1, maxMemberId );

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
                IsActive = false,
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
                CreatedDate = DateTime.Now.ToString("yyyy-MM-dd"),
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
    }
}
