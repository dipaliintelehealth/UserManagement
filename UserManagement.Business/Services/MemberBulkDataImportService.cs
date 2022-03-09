using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using UserManagement.Contract;
using UserManagement.Contract.Repository;
using UserManagement.Contract.Utility;
using UserManagement.Contract.Validator;
using UserManagement.Domain;
using UserManagement.Domain.ViewModel;
using UserManagement.Infrastructure.Files;
using UserManagement.Models;

namespace UserManagement.Business.Services
{
    public class MemberBulkDataImportService : IBulkDataImportService<MemberBulkImportVM>
    {
        private readonly IExcelFileUtility<MemberBulkImportVM> _excelFileUtility;
        private readonly IMemberBulkInsertRepository _bulkInsertRepository;
        private readonly IBulkInsertValidator<MemberBulkImportVM> _bulkInsertValidator;
        private MemberBulkImportVM _obj = new MemberBulkImportVM();
        private readonly ExcelConfiguration _excelConfiguration;
        private string _pathForCsv = string.Empty;
        public MemberBulkDataImportService(IExcelFileUtility<MemberBulkImportVM> excelFileUtility, IMemberBulkInsertRepository bulkInsertRepository, IBulkInsertValidator<MemberBulkImportVM> bulkInsertValidator)
        {
            this._excelFileUtility = excelFileUtility;
            this._bulkInsertRepository = bulkInsertRepository;
            this._bulkInsertValidator = bulkInsertValidator;
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
            var specializations = await _bulkInsertRepository.GetSpecialities();
            var institutions = await _bulkInsertRepository.FindInstitutions(models.Select(x => x.HFEmail), models.Select(x => x.HFPhone), models.Select(x => x.HFNameWithDistrictName.Trim()));
            var hfTypes = await _bulkInsertRepository.GetHFTypes();

            var data = MapDuplicateInstituteData(models, states, institutions, hfTypes);
            foreach (var x in data)
            {
                x.HFTypeId = GetHFtypeId(hfTypes, x.HFType);
                x.HFState = states.FirstOrDefault(s => s.StateId == x.SelectedHFStateId)?.StateName;
                x.HFDistrict = states.FirstOrDefault(s => s.DistrictId == x.SelectedHFDistrictId)?.DistrictName;
                x.HFCity = states.FirstOrDefault(s => s.CityId == x.SelectedHFCityId)?.CityName;
                x.UserState = states.FirstOrDefault(s => s.StateId == x.SelectedUserStateId)?.StateName;
                x.UserDistrict = states.FirstOrDefault(s => s.DistrictId == x.SelectedUserDistrictId)?.DistrictName;
                x.UserCity = states.FirstOrDefault(s => s.CityId == x.SelectedUserCityId)?.CityName;
                x.Designation = specializations.FirstOrDefault(d => d.SpecialityId == x.SelectedSpecialityId)?.SpecialityName;
                x.UserDistrictShortCode = GetDistrictShortCode(states, x.UserState, x.UserDistrict);
                x.UserName = GetUsersName(states, x.UserState, x.UserDistrict, x.HFName, x.HFType);
                x.QualificationId = GetQualificationId(qualifications, x.Qualification);
                x.SpecialityId = GetSpecializationId(specializations, x.Designation);
                x.InstituteID = GetInstitutionId(institutions, x.HFNameWithDistrictName);
                x.AssignedInstituteID = GetInstitutionId(institutions, x.AssignHF);
            }
            var (resultValid, resultInvalid) = await GetValidInvalidData(data);
            WriteToCSV(new MemberBulkValidCsvUtility(sessionIdInString), folderPath, resultValid);
            WriteToCSV(new MemberBulkInvalidCsvUtility(sessionIdInString), folderPath, resultInvalid);
            return Result.Success(sessionIdInString);
        }

        private List<MemberBulkImportVM> MapDuplicateInstituteData(IEnumerable<MemberBulkImportVM> models, IEnumerable<StateDistrictCity> states, IEnumerable<InstitutionModel> institutions, IEnumerable<KeyValue<string, string>> hfTypes)
        {
            var data = models.ToList();
            var distinctinstitutes = data?.Distinct(new CompareOnHFEmailMemberBulkImportVM())
                                         ?.Distinct(new CompareOnHFPhoneMemberBulkImportVM())
                                         ?.Distinct(new CompareOnHFNameWithDistrictMemberBulkImportVM())
                                         ?.Where(x => string.IsNullOrWhiteSpace(x.InstituteID));
            foreach (var item in distinctinstitutes)
            {
                var found = institutions.FirstOrDefault(t => string.Equals(item.HFEmail?.Trim().ToLower(), t.Email?.Trim().ToLower())
                                && string.Equals(item.HFPhone?.Trim(), t.Mobile?.Trim())
                                && string.Equals(item.HFNameWithDistrictName?.Trim().ToLower(), t.Name?.Trim().ToLower()));
                if (found != null)
                {
                    item.InstituteID = Convert.ToString(found.InstitutionId);
                    item.Address = found.AddressLine1;
                    item.HFName = found.Name.Substring(0, found.Name.LastIndexOf(" ")); // HF name
                    item.SelectedHFStateId = found.StateId;
                    item.SelectedHFDistrictId = found.DistrictId;
                    item.SelectedHFCityId = found.CityId;
                    item.HFEmail = found.Email;
                    item.HFPhone = found.Mobile;
                    item.HFTypeId = found.InstitutionTypeId;
                    item.HFType = hfTypes?.FirstOrDefault(t => t.Id == found.InstitutionTypeId.ToString())?.Value;
                    item.NIN = found.ReferenceNumber;
                }

            }
            foreach (var item in data)
            {
                var found = distinctinstitutes.FirstOrDefault(t => string.Equals(item.HFEmail?.Trim().ToLower(), t.HFEmail?.Trim().ToLower())
                            && string.Equals(item.HFPhone?.Trim(), t.HFPhone?.Trim())
                            && string.Equals(item.HFNameWithDistrictName?.Trim().ToLower(), t.HFNameWithDistrictName?.Trim().ToLower()));
                if (found != null)
                {
                    item.InstituteID = Convert.ToString(found.InstituteID);
                    item.HFName = found.HFName; // HF name
                    item.SelectedHFStateId = found.SelectedHFStateId;
                    item.SelectedHFDistrictId = found.SelectedHFDistrictId;
                    item.SelectedHFCityId = found.SelectedHFCityId;
                    item.HFEmail = found.HFEmail;
                    item.HFPhone = found.HFPhone;
                    item.HFTypeId = found.HFTypeId;
                    item.AssignedHFType = found.AssignedHFType;
                    item.AssignHF = found.AssignHF;
                    item.NIN = found.NIN;
                    item.Address = found.Address;
                }

            }
            return data;
        }

        private void WriteToCSV<T>(ICsvFileUtility<T> csvFileUtility, string folderPath, IList<T> data)
        {

            csvFileUtility.Configure(new CsvConfiguration() { CsvLogPath = folderPath });
            csvFileUtility.Write(data);
        }

        private async Task<(IList<MemberBulkValid>, IList<MemberBulkInvalid>)> GetValidInvalidData(IList<MemberBulkImportVM> data)
        {
            var result = await _bulkInsertValidator.ValidateAsync(data);
            var validData = new List<MemberBulkValid>();
            var invalidData = new List<MemberBulkInvalid>();
            for (int i = 0; i < data.Count; i++)
            {
                if (result.Errors.Any(t => t.Index == i))
                {
                    invalidData.Add(GetInValidData(data[i], result.Errors.Where(t => t.Index == i).Select(x => x.ErrorMessage).ToList()));
                }
                else
                {
                    validData.Add(GetValidData(data[i]));
                }
            }
            return (validData, invalidData);
        }
        private MemberBulkValid GetValidData(MemberBulkImportVM data)
        {
            return new MemberBulkValid()
            {
                HFName = data.HFName,
                Address = data.Address,
                AssignedHFType = data.AssignedHFType,
                AssignedInstituteID = data.AssignedInstituteID,
                AssignHF = data.AssignHF,
                Designation = data.Designation,
                DOB = data.DOB,
                DRRegNo = data.DRRegNo,
                Experience = data.Experience,
                FirstName = data.FirstName,
                Gender = data.Gender,
                HFCities = data.HFCities,
                HFCity = data.HFCity,
                HFDistrict = data.HFDistrict,
                HFDistricts = data.HFDistricts,
                HFEmail = data.HFEmail,
                HFPhone = data.HFPhone,
                HFState = data.HFState,
                HFType = data.HFType,
                HFTypeId = data.HFTypeId,
                InstituteID = data.InstituteID,
                LastName = data.LastName,
                MemberId = data.MemberId,
                NIN = data.NIN,
                PIN = data.PIN,
                Qualification = data.Qualification,
                QualificationId = data.QualificationId,
                SelectedHFCityId = data.SelectedHFCityId,
                SelectedHFDistrictId = data.SelectedHFDistrictId,
                SelectedHFStateId = data.SelectedHFStateId,
                SelectedSpecialityId = data.SelectedSpecialityId,
                SelectedUserCityId = data.SelectedUserCityId,
                SpecialityId = data.SpecialityId,
                SelectedUserDistrictId = data.SelectedUserDistrictId,
                SelectedUserStateId = data.SelectedUserStateId,
                SubMenuName = data.SubMenuName,
                UserAddress = data.UserAddress,
                UserAvailableDay = data.UserAvailableDay,
                UserAvailableFromTime = data.UserAvailableFromTime,
                UserAvailableToTime = data.UserAvailableToTime,
                UserCities = data.UserCities,
                UserCity = data.UserCity,
                UserDistrict = data.UserDistrict,
                UserDistricts = data.UserDistricts,
                UserDistrictShortCode = data.UserDistrictShortCode,
                UserEmail = data.UserEmail,
                UserMobile = data.UserMobile,
                UserName = data.UserName,
                UserPin = data.UserPin,
                UserPrefix = data.UserPrefix,
                UserRole = data.UserRole,
                UserState = data.UserState
            };
        }
        private MemberBulkInvalid GetInValidData(MemberBulkImportVM data, IList<string> errors)
        {
            return new MemberBulkInvalid()
            {
                ErrorMessage = string.Join(",", errors),
                HFName = data.HFName,
                Address = data.Address,
                AssignedHFType = data.AssignedHFType,
                AssignedInstituteID = data.AssignedInstituteID,
                AssignHF = data.AssignHF,
                Designation = data.Designation,
                DOB = data.DOB,
                DRRegNo = data.DRRegNo,
                Experience = data.Experience,
                FirstName = data.FirstName,
                Gender = data.Gender,
                HFCities = data.HFCities,
                HFCity = data.HFCity,
                HFDistrict = data.HFDistrict,
                HFDistricts = data.HFDistricts,
                HFEmail = data.HFEmail,
                HFPhone = data.HFPhone,
                HFState = data.HFState,
                HFType = data.HFType,
                HFTypeId = data.HFTypeId,
                InstituteID = data.InstituteID,
                LastName = data.LastName,
                MemberId = data.MemberId,
                NIN = data.NIN,
                PIN = data.PIN,
                Qualification = data.Qualification,
                QualificationId = data.QualificationId,
                SelectedHFCityId = data.SelectedHFCityId,
                SelectedHFDistrictId = data.SelectedHFDistrictId,
                SelectedHFStateId = data.SelectedHFStateId,
                SelectedSpecialityId = data.SelectedSpecialityId,
                SelectedUserCityId = data.SelectedUserCityId,
                SpecialityId = data.SpecialityId,
                SelectedUserDistrictId = data.SelectedUserDistrictId,
                SelectedUserStateId = data.SelectedUserStateId,
                SubMenuName = data.SubMenuName,
                UserAddress = data.UserAddress,
                UserAvailableDay = data.UserAvailableDay,
                UserAvailableFromTime = data.UserAvailableFromTime,
                UserAvailableToTime = data.UserAvailableToTime,
                UserCities = data.UserCities,
                UserCity = data.UserCity,
                UserDistrict = data.UserDistrict,
                UserDistricts = data.UserDistricts,
                UserDistrictShortCode = data.UserDistrictShortCode,
                UserEmail = data.UserEmail,
                UserMobile = data.UserMobile,
                UserName = data.UserName,
                UserPin = data.UserPin,
                UserPrefix = data.UserPrefix,
                UserRole = data.UserRole,
                UserState = data.UserState
            };
        }

        public async Task<IEnumerable<MemberBulkImportVM>> CreateModels(Stream stream)
        {
            _excelFileUtility.Configure(_excelConfiguration);
            var models = _excelFileUtility.Read(stream);
            models = await GetModels(models);
            return models;
        }

        public async Task<IEnumerable<MemberBulkImportVM>> GetModels(IEnumerable<MemberBulkImportVM> models)
        {
            var institutions = await _bulkInsertRepository.FindInstitutions(models.Select(x => x.HFEmail), models.Select(x => x.HFPhone), models.Select(x => x.HFNameWithDistrictName.Trim()));
            var states = await _bulkInsertRepository.GetStateDistrictCities();
            models = await GetModelsWithStateDistrictAndCityId(models, states, institutions);
            return models;
        }

        public async Task<IEnumerable<ResultModel<MemberBulkValid>>> ImportData(IEnumerable<MemberBulkValid> models, string pathForCsvLog)
        {
            this._pathForCsv = pathForCsvLog;
            var result = Enumerable.Empty<ResultModel<MemberBulkValid>>();
            if (!models.Any())
            {
                return Enumerable.Empty<ResultModel<MemberBulkValid>>();
            }
            var institutions = await _bulkInsertRepository.FindInstitutions(models.Select(x => x.HFEmail), models.Select(x => x.HFPhone), models.Select(x => x.HFNameWithDistrictName.Trim()));
            var states = await _bulkInsertRepository.GetStateDistrictCities();
            var users = await _bulkInsertRepository.FindUsers(models.Select(x => GetHFNameForLogin(x.HFName)).Distinct());
            var subMenu = await _bulkInsertRepository.GetSubMenu();

            result = await this.CreateUserName(models, users, states);
            try
            {
                result = await this.CreateInstitutes(result, institutions);
                result = await this.CreateMember(result);
                result = await this.CreateLogin(result);
                result = await this.CreateMemberSlot(result);
                result = await this.CreateMemberInstitution(result);
                result = await this.CreateMemberMenu(result, subMenu);
                result = await this.CreateAuditTrail(result);
             
            }
            catch(Exception ex)
            {
                await RevertInsertedData(result.Select(t => t.Value));
                throw ex;
            }
            return result;
        }
        public Task<IEnumerable<ResultModel<MemberBulkValid>>> CreateUserName(IEnumerable<MemberBulkValid> validatedModels, IEnumerable<string> users, IEnumerable<StateDistrictCity> states)
        {
            var modelReturns = new List<ResultModel<MemberBulkValid>>();
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
                    var numberToincrement = users.Where(x => Regex.IsMatch(x, pattern))?.Select(x => { var number = x.Replace(firstpart, string.Empty).Replace(secondpart, string.Empty); return (!string.IsNullOrWhiteSpace(number) ? int.Parse(number) : 0); }).OrderByDescending(x => x).FirstOrDefault();
                    initialCount = numberToincrement != null ? Convert.ToInt32(numberToincrement) + 1 : initialCount;
                }
                foreach (var item in duplicateUserGroup)
                {
                    if (initialCount > 0)
                    {
                        item.UserName = $"{firstpart}{initialCount}{secondpart}";
                    }
                    initialCount++;
                    modelReturns.Add(ResultModel<MemberBulkValid>.Success(item));
                }
            }

            return Task.FromResult(modelReturns.AsEnumerable());
        }

        private async Task<IEnumerable<MemberBulkImportVM>> GetModelsWithStateDistrictAndCityId(IEnumerable<MemberBulkImportVM> bulkImportVMs, IEnumerable<StateDistrictCity> states, IEnumerable<InstitutionModel> institutions)
        {
            var qualifications = await _bulkInsertRepository.GetQualification();
            var specializations = await _bulkInsertRepository.GetSpecialities();
            var hfTypes = await _bulkInsertRepository.GetHFTypes();
            var models = new List<MemberBulkImportVM>();
            foreach (var model in bulkImportVMs)
            {
                if (model.SelectedHFStateId != 0)
                {
                    model.HFState = states.FirstOrDefault(s => s.StateId == model.SelectedHFStateId)?.StateName;
                }
                else
                {
                    model.SelectedHFStateId = GetStateId(states, model.HFState);
                }
                if (model.SelectedHFDistrictId != 0)
                {
                    model.HFDistrict = states.FirstOrDefault(s => s.DistrictId == model.SelectedHFDistrictId)?.DistrictName;
                }
                else
                {
                    model.SelectedHFDistrictId = GetDistrictId(states, model.HFState, model.HFDistrict);
                }
                if (model.SelectedHFCityId != 0)
                {
                    model.HFCity = states.FirstOrDefault(s => s.CityId == model.SelectedHFCityId)?.CityName;
                }
                else
                {
                    model.HFCity = GetCityName(states, model.HFState, model.HFDistrict, model.HFCity);
                    model.SelectedHFCityId = GetCityId(states, model.HFState, model.HFDistrict, model.HFCity);
                }

                if (model.SelectedUserStateId != 0)
                {
                    model.UserState = states.FirstOrDefault(s => s.StateId == model.SelectedUserStateId)?.StateName;
                }
                else
                {
                    model.SelectedUserStateId = GetStateId(states, model.UserState);
                }
                if (model.SelectedUserDistrictId != 0)
                {
                    model.UserDistrict = states.FirstOrDefault(s => s.DistrictId == model.SelectedUserDistrictId)?.DistrictName;
                }
                else
                {
                    model.SelectedUserDistrictId = GetDistrictId(states, model.UserState, model.UserDistrict);
                }
                if (model.SelectedUserCityId != 0)
                {
                    model.UserCity = states.FirstOrDefault(s => s.CityId == model.SelectedUserCityId)?.CityName;
                }
                else
                {
                    model.UserCity = GetCityName(states, model.UserState, model.UserDistrict, model.UserCity);
                    model.SelectedUserCityId = GetCityId(states, model.UserState, model.UserDistrict, model.UserCity);
                }
                if (model.SelectedSpecialityId != 0)
                {
                    model.Designation = specializations?.FirstOrDefault(x => x.SpecialityId == model.SelectedSpecialityId)?.SpecialityName;
                }
                else
                {
                    model.SelectedSpecialityId = GetSpecializationId(specializations, model.Designation);
                }
                model.HFTypeId = GetHFtypeId(hfTypes, model.HFType);
                model.Designation = specializations.FirstOrDefault(d => d.SpecialityId == model.SelectedSpecialityId)?.SpecialityName;
                model.UserDistrictShortCode = GetDistrictShortCode(states, model.UserState, model.UserDistrict);
                model.UserName = GetUsersName(states, model.UserState, model.UserDistrict, model.HFName, model.HFType);
                model.QualificationId = GetQualificationId(qualifications, model.Qualification);
                model.SpecialityId = GetSpecializationId(specializations, model.Designation);
                model.InstituteID = GetInstitutionId(institutions, model.HFName);
                model.AssignedInstituteID = GetInstitutionId(institutions, model.AssignHF);
                model.HFDistricts = GetDistricts(states, model.HFState);
                model.UserDistricts = GetDistricts(states, model.UserState);
                model.HFCities = GetCities(states, model.HFState, model.HFDistrict);
                model.UserCities = GetCities(states, model.UserState, model.UserDistrict);


                //model.SelectedHFStateId = GetStateId(states, model.HFState);
                //model.SelectedSpecialityId = GetSpecialityId(specializations, model.Designation);
                //model.SelectedHFDistrictId = GetDistrictId(states, model.HFState, model.HFDistrict);
                //model.SelectedHFCityId = GetCityId(states, model.HFState, model.HFDistrict, model.HFCity);
                //model.HFCity = GetCityName(states, model.HFState, model.HFDistrict, model.HFCity);
                //model.SelectedUserStateId = GetStateId(states, model.UserState);
                //model.SelectedUserDistrictId = GetDistrictId(states, model.UserState, model.UserDistrict);
                //model.SelectedUserCityId = GetCityId(states, model.UserState, model.UserDistrict, model.UserCity);
                //model.UserCity = GetCityName(states, model.UserState, model.UserDistrict, model.UserCity);
                //model.UserDistrictShortCode = GetDistrictShortCode(states, model.UserState, model.UserDistrict);
                //model.UserName = GetUsersName(states, model.UserState, model.UserDistrict, model.HFName, model.HFType);
                //model.QualificationId = GetQualificationId(qualifications, model.Qualification);
                //model.SpecialityId = GetSpecializationId(specializations, model.Designation);
                //model.InstituteID = GetInstitutionId(institutions, model.HFName);
                //model.AssignedInstituteID = GetInstitutionId(institutions, model.AssignHF);
                //model.HFDistricts = GetDistricts(states, model.HFState);
                //model.UserDistricts = GetDistricts(states, model.UserState);
                //model.HFCities = GetCities(states, model.HFState, model.HFDistrict);
                //model.UserCities = GetCities(states, model.UserState, model.UserDistrict);

                models.Add(model);
            }
            return models;
        }
        private int GetHFtypeId(IEnumerable<KeyValue<string, string>> hfTypes, string hfType)
        {
            var tempHfType = hfType?.Replace(" ", "")?.Replace("-", "")?.ToLower();
            var found = hfTypes.FirstOrDefault(t => t.Value.Replace(" ", "")?.Replace("-", "").ToLower() == tempHfType);
            int hfTypeId = 3;
            if (found != null)
            {
                hfTypeId = Convert.ToInt32(found.Id);
            }
            return hfTypeId;
        }

        private IEnumerable<KeyValue<string, string>> GetCities(IEnumerable<StateDistrictCity> states, string state, string district)
        {
            var cities = states
                .Where(x => x.StateName.ToUpper() == state?.Trim().ToUpper() && x.DistrictName.ToUpper() == district?.Trim().ToUpper())
                .Select(s => new KeyValue<string, string> { Id = s.CityId.ToString(), Value = s.CityName });

            return cities.Distinct(new KeyValueStringComparer()).OrderBy(x => x.Value);
        }
        private IEnumerable<KeyValue<string, string>> GetDistricts(IEnumerable<StateDistrictCity> states, string state)
        {
            var districts = states
                .Where(x => x.StateName.ToUpper() == state?.Trim().ToUpper())
                .Select(s => new KeyValue<string, string> { Id = s.DistrictId.ToString(), Value = s.DistrictName });

            return districts.Distinct(new KeyValueStringComparer()).OrderBy(x => x.Value);
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
            if (Type?.Trim().ToUpper() == "SPOKE")
            {
                strTypeShortCode = "S";
            }
            else if (Type?.Trim().ToUpper() == "HUB")
            {
                strTypeShortCode = "H";
            }
            else if (Type?.Trim().ToUpper() == "SPOKE CUM HUB")
            {
                strTypeShortCode = "SCH";
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
                "SPOKE Cum HUB","SPOKE","HUB"
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
                var trimmedDistrict = districtName.Replace(" ", "");


                if (trimmedDistrict.Length > 2)
                {
                    distShortCode = trimmedDistrict.Substring(0, trimmedDistrict.Length - 1);
                }
                else
                {
                    distShortCode = trimmedDistrict;
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

            if (!string.IsNullOrEmpty(StateName) && StateAndCodes.ContainsKey(StateName.Trim().ToUpper()))
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
                var firstCity = cities?.OrderBy(x => x.CityName).FirstOrDefault()?.CityId;
                cityID = firstCity ?? 0;
            }

            return cityID;
        }
        private int GetSpecialityId(IEnumerable<SpecializationModel> specilizations, string specialityName)
        {
            return specilizations.Where(x => x.SpecialityName.ToUpper() == specialityName?.Trim().ToUpper())
                .Select(specilization => specilization.SpecialityId)
                .FirstOrDefault();
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
        private int GetSpecializationId(IEnumerable<SpecializationModel> specializations, string specializationName)
        {
            var spl = specializations
                .Where(x => x.SpecialityName.ToUpper() == specializationName?.Trim().ToUpper())
                .Select(specialization => specialization.SpecialityId)
                .FirstOrDefault();

            return spl;
        }
        private async Task<IEnumerable<ResultModel<MemberBulkValid>>> CreateInstitutes(IEnumerable<ResultModel<MemberBulkValid>> models, IEnumerable<InstitutionModel> institutions)
        {
            var allValidModels = models?.Where(x => x.IsSuccess).Select(x => x.Value);
            if (allValidModels.Count() == 0)
            {
                return models;
            }
            var invalidResults = models?.Where(t => !t.IsSuccess);
            
            var NotInDBModels = allValidModels?.Where(x => !institutions.Any(t => string.Equals(x.HFEmail?.Trim().ToLower(), t.Email?.Trim().ToLower())
                               || string.Equals(x.HFPhone?.Trim(), t.Mobile?.Trim())
                               || string.Equals(x.HFNameWithDistrictName?.Trim().ToLower(), t.Name?.Trim().ToLower())));


            var institutesToInsert = NotInDBModels?.Distinct(new CompareOnHFEmail())?
                .Distinct(new CompareOnHFPhone())?
                .Distinct(new CompareOnHFNameWithDistrict())
                .Where(x => string.IsNullOrWhiteSpace(x.InstituteID))
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
                }).ToList();
            var records = 0;
            if (institutesToInsert != null && institutesToInsert.Count() > 0)
            {
                var csvUtility = new InstitutionModelCsvUtility();
                csvUtility.Configure(new CsvConfiguration()
                {
                    CsvLogPath = this._pathForCsv
                });
                var stream = csvUtility.Write(institutesToInsert);
                records = await _bulkInsertRepository.BulkInsertInstitution(stream);
            }
            var tempInstitutions = await _bulkInsertRepository.FindInstitutions(allValidModels.Select(x => x.HFEmail), NotInDBModels.Select(x => x.HFPhone), NotInDBModels.Select(x => x.HFNameWithDistrictName.Trim()));


            var results = new List<ResultModel<MemberBulkValid>>();
            foreach (var item in allValidModels)
            {
                var find = tempInstitutions
                            .FirstOrDefault(r => r.Email?.Trim().ToLower() == item.HFEmail?.Trim().ToLower()
                                             && r.Mobile?.Trim() == item.HFPhone?.Trim()
                                             && r.Name?.Trim().ToLower() == item.HFNameWithDistrictName?.Trim().ToLower());
                if (find != null)
                {
                    item.InstituteID = Convert.ToString(find.InstitutionId);
                    item.IsInstituteInserted = institutesToInsert.Any(t => t.Email?.Trim().ToLower() == item.HFEmail?.Trim().ToLower()
                                                                        && t.Mobile?.Trim() == item.HFPhone?.Trim());
                    results.Add(ResultModel<MemberBulkValid>.Success(item));
                }
                else
                {
                    results.Add(ResultModel<MemberBulkValid>.Failure(item, "Database Error For Institute"));
                }
            }
            if (institutesToInsert != null && institutesToInsert.Count() > 0 && results.Count > 0)
            {
                var insertedInstitutions = results
                                            .Where(r => r.IsSuccess && r.Value.IsInstituteInserted)
                                            .Select(r => r.Value);

                var result = await CreateMasterMember(insertedInstitutions);
                var loginResult = await CreateMasterLogin(result.Value);
                await CreateMemberInstitution(loginResult);
            }
            results.AddRange(invalidResults);
            return results;
        }

        private async Task<ResultModel<Dictionary<int, MemberBulkValid>>> CreateMasterMember(IEnumerable<MemberBulkValid> models)
        {
            Dictionary<int, MemberBulkValid> mdl = new Dictionary<int, MemberBulkValid>();
            if (models == null || models.Count() == 0) { return ResultModel<Dictionary<int, MemberBulkValid>>.Success(mdl); }
            var data = models.ToList();
            var dob = DateTime.Now.AddYears(-18).ToString("yyyy-MM-dd HH:mm:ss");
            var createdDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var members = data.Select(x => new MembersModelForCsv()
            {

                FirstName = x.HFNameWithDistrictName,
                MiddleName = "MiddleName",
                LastName = "LastName",
                AgeType = 1,
                DOB = dob,
                Age = 0,
                Mobile = x.HFPhone,
                Email = x.HFEmail,
                ImagePath = string.Empty,
                CreatedBy = 10,
                CreatedDate = createdDate,
                IsActive = 1,
                GenderId = 4,
                RegistrationNumber = string.Empty,
                AddressLine1 = string.Empty,
                AddressLine2 = string.Empty,
                StateId = null,
                DistrictId = null,
                CityId = null,
                SpecializationId = null,
                QualificationId = null,
                PinCode = string.Empty,
                Fax = string.Empty,
                LoginOTP = 0,
                IsLoginOTPActive = "",
                SignaturePath = string.Empty,
                CountryId = 0,
                StatusId = 2,
                RatingMasterId = 1,
                SourceId = 99,
                IsMaster = "1",
                Prefix = null,
                CreationRole = "1"
            });

            var csvUtility = new MembersModelForCsvUtility();
            csvUtility.Configure(new CsvConfiguration()
            {
                CsvLogPath = this._pathForCsv
            });
            var stream = csvUtility.Write(members);
            var records = await _bulkInsertRepository.BulkInsertMembers(stream);
            var dbRecords = await _bulkInsertRepository.FindMembers(members.Select(x => x.Email));

            foreach (var item in data)
            {
                var find = dbRecords.FirstOrDefault(r => r.Email == item.HFEmail);
                if (find != null && !mdl.ContainsKey(find.MemberId))
                {
                    mdl.Add(find.MemberId, item);
                }
            }
            return ResultModel<Dictionary<int, MemberBulkValid>>.Success(mdl);
        }
        private async Task<bool> RevertInsertedData(IEnumerable<MemberBulkValid> models)
        {
            await this.RevertMemberSlotData(models);
            await this.RevertMemberMenuData(models);
            await this.RevertMemberInstituteData(models);
            await this.RevertLoginData(models);
            await this.RevertMemberData(models);
            await this.RevertInstituteData(models);
            return true;
        }
        private async Task<bool> RevertInstituteData(IEnumerable<MemberBulkValid> models)
        {
            if (models == null || models.Count() == 0) return true;
            var hfemails =  models?.Select(t => t.HFEmail);
            var hfMobiles = models?.Select(t => t.HFPhone);
            await _bulkInsertRepository.RemoveInstitutions(hfemails, hfMobiles);
            return true;
        }
        private async Task<bool> RevertMemberData(IEnumerable<MemberBulkValid> models)
        {
            if (models == null || models.Count() == 0) { return true; }
            var hfemails = models.Where(t=> t.IsInstituteInserted)?.Select(t => t.HFEmail);
            var hfMobiles = models.Where(t => t.IsInstituteInserted).Select(t => t.HFPhone);
            var userMobiles = models.Select(t => t.UserMobile);
            var userEmails = models.Select(t => t.UserEmail);
            userEmails = userEmails.Concat(hfemails);
            userMobiles = userMobiles.Concat(hfMobiles);
            await _bulkInsertRepository.RemoveMembers(userEmails, userMobiles);
            return true;
        }
        private async  Task<bool> RevertMemberInstituteData(IEnumerable<MemberBulkValid> models)
        {
            if (models == null || models.Count() == 0) return true;
            var memberIds = models?.Where(t => !string.IsNullOrWhiteSpace(t.MemberId)).Select(t => t.MemberId);
            var instituteIds = models?.Where(t => t.IsInstituteInserted && !string.IsNullOrWhiteSpace(t.InstituteID)).Select(t => t.InstituteID);
            if (memberIds == null || memberIds.Count() == 0 || instituteIds == null || instituteIds.Count() == 0) return true;
            await _bulkInsertRepository.RemoveMemberInstitution(instituteIds, memberIds);
            return true;
        }
        private async Task<bool> RevertLoginData(IEnumerable<MemberBulkValid> models)
        {
            if (models == null || models.Count() == 0) return true;
            var memberIds = models?.Where(t => !string.IsNullOrWhiteSpace(t.MemberId)).Select(t => t.MemberId);
            if (memberIds == null || memberIds.Count() == 0) return true;
            await _bulkInsertRepository.RemoveLogins(memberIds);
            return true;
        }
        private async Task<bool> RevertMemberMenuData(IEnumerable<MemberBulkValid> models)
        {
            if (models == null || models.Count() == 0) return true;
            var memberIds = models?.Where(t => !string.IsNullOrWhiteSpace(t.MemberId)).Select(t => t.MemberId);
            if (memberIds == null || memberIds.Count() == 0) return true;
            await _bulkInsertRepository.RemoveMemberMenus(memberIds);
            return true;
        }
        private async Task<bool> RevertMemberSlotData(IEnumerable<MemberBulkValid> models)
        {
            if (models == null || models.Count() == 0) return true;
            var memberIds = models?.Where(t => !string.IsNullOrWhiteSpace(t.MemberId)).Select(t => t.MemberId);
            if (memberIds ==  null || memberIds.Count() == 0) return true;
            await _bulkInsertRepository.RemoveMemberSlots(memberIds);
            return true;
        }
        private async Task<List<ResultModel<MemberBulkValid>>> CreateMasterLogin(Dictionary<int, MemberBulkValid> models)
        {
            if (models == null || models.Count() == 0) { return (new List<ResultModel<MemberBulkValid>>()); }
            var logins = new List<LoginModelForCsv>();
            var result = new List<ResultModel<MemberBulkValid>>();
            foreach (var item in models)
            {
                var lg = new LoginModelForCsv()
                {
                    UserName = GetMasterUserName(item.Value),
                    Password = "ba3253876aed6bc22d4a6ff53d8406c6ad864195ed144ab5c87621b6c233b548baeae6956df346ec8c17f5ea10f35ee3cbc514797ed7ddd3145464e2a0bab413",
                    ReferenceId = Convert.ToString(item.Key),
                    IsActive = 1,
                    SourceId = "99"
                };
                logins.Add(lg);
                var mdl = new MemberBulkValid()
                {
                    InstituteID = item.Value.InstituteID,
                    MemberId = Convert.ToString(item.Key)
                };
                result.Add(ResultModel<MemberBulkValid>.Success(mdl));
            }

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
            return result;
        }

        private string GetMasterUserName(MemberBulkValid value)
        {
            var userName = "User" + (value.HFTypeId == 1 ? "HUB" : (value.HFTypeId == 2 ? "SPOKECumHUB" : "SPOKE")) + value.InstituteID;
            return userName;
        }

        private async Task<IEnumerable<ResultModel<MemberBulkValid>>> CreateMember(IEnumerable<ResultModel<MemberBulkValid>> models)
        {
            var allValidModels = models?.Where(x => x.IsSuccess).Select(x => x.Value);
            if (allValidModels.Count() == 0)
            {
                return models;
            }
            var invalidResults = models?.Where(x => !x.IsSuccess);
         
            var members = allValidModels.Select(x => new MembersModelForCsv()
            {

                FirstName = x.FirstName,
                MiddleName = string.Empty,
                LastName = x.LastName,
                AgeType = 1,
                DOB = DateTime.ParseExact(x.DOB, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm:ss"),
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
                SpecializationId = x.SelectedSpecialityId,
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
                Prefix = x.UserPrefix,
                CreationRole = x.UserRole
            });

            var csvUtility = new MembersModelForCsvUtility();
            csvUtility.Configure(new CsvConfiguration()
            {
                CsvLogPath = this._pathForCsv
            });
            var stream = csvUtility.Write(members);
            var records = await _bulkInsertRepository.BulkInsertMembers(stream);

            var dbRecords = await _bulkInsertRepository.FindMembers(allValidModels.Select(x => x.UserEmail));
            var results = new List<ResultModel<MemberBulkValid>>();
            foreach (var item in allValidModels)
            {
                var find = dbRecords.FirstOrDefault(r => r.Email == item.UserEmail);
                if (find != null)
                {
                    item.MemberId = Convert.ToString(find.MemberId);
                    results.Add(ResultModel<MemberBulkValid>.Success(item));
                }
                else
                {
                    results.Add(ResultModel<MemberBulkValid>.Failure(item, "Database Error for Member"));
                }
            }
            results.AddRange(invalidResults);
            throw new ArgumentNullException();
            return results;
        }

        private async Task<IEnumerable<ResultModel<MemberBulkValid>>> CreateLogin(IEnumerable<ResultModel<MemberBulkValid>> models)
        {
            var allValidModels = models?.Where(x => x.IsSuccess).Select(x => x.Value);
            if (allValidModels.Count() == 0)
            {
                return models;
            }
            var inValidResults = models?.Where(x => !x.IsSuccess);
           
            var logins = allValidModels.Select(x => new LoginModelForCsv()
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
            /// to do check which logins are not created

            var results = new List<ResultModel<MemberBulkValid>>();
            foreach (var item in allValidModels)
            {
                results.Add(ResultModel<MemberBulkValid>.Success(item));
            }
            results.AddRange(inValidResults);
            return results;
        }
        private async Task<IEnumerable<ResultModel<MemberBulkValid>>> CreateMemberSlot(IEnumerable<ResultModel<MemberBulkValid>> models)
        {
            var allValidModels = models?.Where(x => x.IsSuccess).Select(x => x.Value);
            if (allValidModels.Count() == 0)
            {
                return models;
            }

            var inValidResults = models?.Where(x => !x.IsSuccess);

            var memberSlots = allValidModels?.Select(x => new MemberSlotModelForCsv()
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

            var results = new List<ResultModel<MemberBulkValid>>();
            foreach (var item in allValidModels)
            {
                results.Add(ResultModel<MemberBulkValid>.Success(item));
            }
            results.AddRange(inValidResults);
            return results;
        }
        private async Task<IEnumerable<ResultModel<MemberBulkValid>>> CreateMemberInstitution(IEnumerable<ResultModel<MemberBulkValid>> models)
        {
            var allValidModels = models?.Where(x => x.IsSuccess).Select(x => x.Value);
            if (allValidModels.Count() == 0)
            {
                return models;
            }

            var inValidResults = models?.Where(x => !x.IsSuccess);
          

            var memberInstitutions = allValidModels?.Select(x => new MemberInstitutionModel()
            {
                InstitutionId = x.InstituteID,
                MemberId = x.MemberId,
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
                await _bulkInsertRepository.BulkInsertMemberInstitution(stream);
            }

            var results = new List<ResultModel<MemberBulkValid>>();
            foreach (var item in allValidModels)
            {
                results.Add(ResultModel<MemberBulkValid>.Success(item));
            }
            results.AddRange(inValidResults);
            return results;
        }

        private async Task<IEnumerable<ResultModel<MemberBulkValid>>> CreateMemberMenu(IEnumerable<ResultModel<MemberBulkValid>> models, IEnumerable<SubMenuModel> subMenu)
        {
            var allValidModels = models?.Where(x => x.IsSuccess).Select(x => x.Value);
            if (allValidModels.Count() == 0)
            {
                return models;
            }

            var inValidResults = models?.Where(x => !x.IsSuccess);
        
            IEnumerable<MemberMenuModelForCsv> memberMenus = GetMemberMenus(allValidModels, subMenu);

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

            var results = new List<ResultModel<MemberBulkValid>>();
            foreach (var item in allValidModels)
            {
                results.Add(ResultModel<MemberBulkValid>.Success(item));
            }
            results.AddRange(inValidResults);
            return results;
        }

        public IEnumerable<MemberMenuModelForCsv> GetMemberMenus(IEnumerable<MemberBulkValid> models, IEnumerable<SubMenuModel> subMenus)
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
        private async Task<IEnumerable<ResultModel<MemberBulkValid>>> CreateAuditTrail(IEnumerable<ResultModel<MemberBulkValid>> models)
        {
            var allValidModels = models?.Where(x => x.IsSuccess).Select(x => x.Value);
            if(allValidModels.Count() == 0)
            {
                return models;
            }
            var inValidResults = models?.Where(x => !x.IsSuccess);
           

            var members = allValidModels?.Select(x => new AuditTrailModel()
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

            var results = new List<ResultModel<MemberBulkValid>>();
            foreach (var item in allValidModels)
            {
                results.Add(ResultModel<MemberBulkValid>.Success(item));
            }
            results.AddRange(inValidResults);
            return results;
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

        public async Task<IEnumerable<KeyValue<string, string>>> GetSpecialities()
        {
            return await _bulkInsertRepository.GetSpecility();
        }

        public async Task<Result<BulkInsertValidInvalidVM>> AddDataFromTemporaryStorage(string sessionID)
        {
            const string folderPath = "Logs/Csv";
            var csvUtility = new MemberBulkValidCsvUtility(sessionID);
            csvUtility.Configure(new CsvConfiguration() { CsvLogPath = folderPath });
            var validData = csvUtility.Read(null);
            var inValidCSVUtility = new MemberBulkInvalidCsvUtility(sessionID);
            inValidCSVUtility.Configure(new CsvConfiguration() { CsvLogPath = folderPath });
            var invalidData = inValidCSVUtility.Read(null);
            var message = "No data found to import";
            var validModels = validData?.ToList();
            var inValidModels = invalidData?.ToList();

            var bulkModel = new BulkInsertValidInvalidVM();
           

            if (validModels.Count == 0 && inValidModels.Count == 0)
            {
                return Result.Failure<BulkInsertValidInvalidVM>(message);
            }

            if (validModels.Count > 0)
            {
                var result = await this.ImportData(validModels, folderPath);
                if (result.Count() == 0)
                {
                    return Result.Failure<BulkInsertValidInvalidVM>(message);
                }
                else
                {
                    message = string.Empty;
                    bulkModel.ValidModels = result.Where(t =>t.IsSuccess).Select(t =>t.Value);
                    var invalidImports = result.Where(t => !t.IsSuccess).Select(t => t.ToMemberBulkInvalid());
                    if (invalidImports.Count() > 0)
                    {
                        inValidModels.AddRange(invalidImports);
                    }
                }
            }
            bulkModel.InValidModels = inValidModels;
            csvUtility.CompleteTask();
            inValidCSVUtility.CompleteTask();
            return Result.Success(bulkModel);
        }
    }
}
