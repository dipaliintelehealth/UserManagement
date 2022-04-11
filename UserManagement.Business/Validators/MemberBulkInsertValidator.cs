using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Contract.Repository;
using UserManagement.Contract.Validator;
using UserManagement.Domain;
using UserManagement.Domain.Validator;
using UserManagement.Domain.ViewModel;

namespace UserManagement.Business.Validators
{
    public class MemberBulkInsertValidator : IBulkInsertValidator<MemberBulkImportVM>
    {
        private readonly IValidator<MemberBulkImportVM> _validator;
        private readonly IMemberBulkInsertRepository _repository;
        private IEnumerable<string> _mobiles = Enumerable.Empty<string>();
        private IEnumerable<string> _emails = Enumerable.Empty<string>();
        private IEnumerable<KeyValue<string, string>> _hfTypes = Enumerable.Empty<KeyValue<string, string>>();
        private IEnumerable<string> _menus = Enumerable.Empty<string>();
        private IEnumerable<StateDistrictCity> _stateDistrictCities = Enumerable.Empty<StateDistrictCity>();
        private IEnumerable<InstitutionModel> _institutions = Enumerable.Empty<InstitutionModel>();

        public MemberBulkInsertValidator(IValidator<MemberBulkImportVM> validator, IMemberBulkInsertRepository repository)
        {
            this._validator = validator;
            this._repository = repository;
        }

        #region Private Methods
        private async Task SetEmailsForValidation(IList<MemberBulkImportVM> models)
        {
            var userEmails = models.Select(model => model.UserEmail);
            var hfEmails = models.Select(model => model.HFEmail);
            var allEmails = Enumerable.Concat(userEmails, hfEmails);
            this._emails = await _repository.FindEmails(allEmails);
        }
        private async Task SetInstitutionsForValidation(IList<MemberBulkImportVM> models)
        {
            var hfEmails = models.Select(model => model.HFEmail);
            var hfMobiles = models.Select(model => model.HFPhone);
            var hfnames = models.Select(model => model.HFNameWithDistrictName);
            this._institutions = await _repository.FindInstitutions(hfEmails, hfMobiles, hfnames);
        }
        private async Task SetHFTypesForValidation()
        {
            this._hfTypes = await _repository.GetHFTypes();
        }
        private async Task SetMobilesForValidation(IList<MemberBulkImportVM> models)
        {
            var userMobiles = models.Select(model => model.UserMobile);
            var hfMobiles = models.Select(model => model.HFPhone);
            var allMobiles = Enumerable.Concat(userMobiles, hfMobiles);
            this._mobiles = await _repository.FindMobiles(allMobiles);
        }
        private async Task SetMenusForValidation()
        {
            var memberMenus = await _repository.GetSubMenu();
            _menus = memberMenus?.Select(x => x.SubMenuName);
        }
        private async Task SetStateDistrictsForValidation()
        {
            _stateDistrictCities = await _repository.GetStateDistrictCities();
        }
        private bool IsDuplicateMobile(string mobile)
        {
            return _mobiles.Contains(mobile);
        }
        private bool IsDuplicateMobile(string mobile, IEnumerable<string> mobiles)
        {
            return mobiles?.Where(x => x == mobile).Count() > 1;
        }
        private bool IsDuplicateEmail(string email)
        {
            return _emails.Contains(email);
        }
        private bool IsContainInValidHFType(string hfType)
        {
            return !_hfTypes.Any(x => string.Equals(x.Value,hfType?.Trim(),StringComparison.InvariantCultureIgnoreCase));
        }
        private bool IsContainInValidHFName(string hfName)
        {
            if(string.IsNullOrEmpty(hfName))
            {
                return false;
            }
            return !_hfTypes.Any(x =>  hfName.Trim().ToLower().Contains(x.Value.ToLower()));
        }
        private bool AreHFNameAndHFTypeDifferent(string hfName,string hfType)
        {
            if (!string.IsNullOrEmpty(hfName))
            {
                return !hfName.ToLower().Contains(hfType.ToLower());
            }
            return true;
        }
        private bool IsDuplicateEmail(string email, IEnumerable<string> emails)
        {
            return emails?.Where(x => x == email).Count() > 1;
        }
        private bool IsDuplicateInstituteEmail(MemberBulkImportVM model, IEnumerable<MemberBulkImportVM> list)
        {
            var hfEmailsFromList = list.Select(x => x.HFEmail);
            var hfEmailsFromDb = this._institutions.Select(x => x.Email);
            var hfEmails = Enumerable.Concat(hfEmailsFromList, hfEmailsFromDb);
            return IsDuplicateEmail(model.HFEmail, hfEmails) && !IsDuplicateHfName(model.HFNameWithDistrictName, list.Select(x => x.HFNameWithDistrictName));
        }
        private bool IsDuplicateHfName(string hfName, IEnumerable<string> hfNames)
        {
            var hfNamesFromDb = this._institutions?.Select(x => x.Name);
            var allHfNames = Enumerable.Concat(hfNamesFromDb, hfNames);
            return allHfNames?.Where(x => x == hfName).Count() > 1;
        }
        private bool IsDuplicateInstituteMobile(MemberBulkImportVM model, IEnumerable<MemberBulkImportVM> list)
        {
            var hfMobilesFromList = list.Select(x => x.HFPhone);
            var hfMobilesFromDb = this._institutions.Select(x => x.Mobile);
            var hfMobiles = Enumerable.Concat(hfMobilesFromList, hfMobilesFromDb);
            return IsDuplicateMobile(model.HFPhone, hfMobiles) && !IsDuplicateHfName(model.HFNameWithDistrictName, list.Select(x => x.HFNameWithDistrictName));
        }
        private bool IsInvalidHFNameWithDistrict(MemberBulkImportVM model, IEnumerable<MemberBulkImportVM> list)
        {
            var isInvalid = false;
            //var hfEmailsFromList = list.Select(x => x.HFEmail);
            //var hfEmailsFromDb = this._institutions.Select(x => x.Email);
            //var hfEmails = Enumerable.Concat(hfEmailsFromList, hfEmailsFromDb);
            //var hfMobilesFromList = list.Select(x => x.HFPhone);
            //var hfMobilesFromDb = this._institutions.Select(x => x.Mobile);
            //var hfMobiles = Enumerable.Concat(hfMobilesFromList, hfMobilesFromDb);

            //return !IsDuplicateMobile(model.HFPhone, hfMobiles) && !IsDuplicateEmail(model.HFEmail, hfEmails) && IsDuplicateHfName(model.HFNameWithDistrictName, list.Select(x => x.HFNameWithDistrictName));
             if(list.Any(t => String.Equals(t.HFNameWithDistrictName.Trim(), model.HFNameWithDistrictName) 
                    && (!string.Equals(t.HFPhone, model.HFPhone) 
                    || !string.Equals(t.HFEmail.Trim().ToLower(), model.HFEmail.Trim().ToLower()))))
            {
                isInvalid = true;
            }
            
             if(this._institutions.Any(t => String.Equals(t.Name.Trim(), model.HFNameWithDistrictName)
                    && (!string.Equals(t.Mobile, model.HFPhone)
                    || !string.Equals(t.Email.Trim().ToLower(), model.HFEmail.Trim().ToLower()))))
            {
                isInvalid = true;
            }

            return isInvalid;
        }
        private bool IsInvalidInstituteMobile(MemberBulkImportVM model, IEnumerable<MemberBulkImportVM> list)
        { 
            // check mobile in md_member for master member creation
            return IsDuplicateMobile(model.HFPhone) && !IsDuplicateHfName(model.HFNameWithDistrictName, list.Select(x => x.HFNameWithDistrictName));
        }
        private bool IsInvalidInstituteEmail(MemberBulkImportVM model, IEnumerable<MemberBulkImportVM> list)
        {
            // check mobile in md_member for master member creation
            return IsDuplicateEmail(model.HFEmail) && !IsDuplicateHfName(model.HFNameWithDistrictName, list.Select(x => x.HFNameWithDistrictName));
        }

        private bool IsContainsInValidMenu(string menuString)
        {
            var userMenus = menuString?.Split(',')?.ToList();
            return userMenus == null || !userMenus.Any() || userMenus.Any(t => !_menus.Contains(t.Trim()));
        }
        private bool IsContainDistrictShortCode(MemberBulkImportVM model)
        {
            if (model.SelectedUserDistrictId == 0) return true;
            return _stateDistrictCities.Any(x => x.StateId == model.SelectedUserStateId && x.DistrictId == model.SelectedUserDistrictId && !string.IsNullOrWhiteSpace(x.DistrictShortCode));
        }
        private static BulkInsertValidationFailure GetBulkInsertValidationFailure(int index, string errorMessage,
            string errorCode, string propertyName)
        {
            var error = new BulkInsertValidationFailure()
            {
                Index = index,
                ErrorMessage = errorMessage,
                ErrorCode = errorCode,
                PropertyName = propertyName
            };
            return error;
        }
        private async Task<BulkInsertValidationResult> ValidateAsync(MemberBulkImportVM model, int index = 0)
        {
            IList<BulkInsertValidationFailure> errors = new List<BulkInsertValidationFailure>();
            var validationResult = await _validator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                foreach (var fluentError in validationResult.Errors)
                {
                    var error = GetBulkInsertValidationFailure(index, fluentError?.ErrorMessage,
                        fluentError?.ErrorCode, fluentError?.PropertyName);
                    errors.Add(error);
                }
            }
            if (IsDuplicateEmail(model.UserEmail))
            {
                var error = GetBulkInsertValidationFailure(index, "Duplicate User Email !", string.Empty,
                    nameof(model.UserEmail));
                errors.Add(error);
            }
            /* if (IsDuplicateEmail(model.HFEmail))
            {
                var error = GetBulkInsertValidationFailure(index, "Institution Email already exist!", string.Empty,
                    nameof(model.HFEmail));
                errors.Add(error);
            } */
            if (IsDuplicateMobile(model.UserMobile))
            {
                var error = GetBulkInsertValidationFailure(index, "Duplicate User Mobile !", string.Empty,
                    nameof(model.UserMobile));
                errors.Add(error);
            }
           /* if (IsDuplicateMobile(model.HFPhone))
            {
                var error = GetBulkInsertValidationFailure(index, "Institution Mobile Number already exist !", string.Empty,
                    nameof(model.HFPhone));
                errors.Add(error);
            }*/
            if (IsContainsInValidMenu(model.SubMenuName))
            {
                var error = GetBulkInsertValidationFailure(index, "Invalid Sub Menu !", string.Empty,
                    nameof(model.SubMenuName));
                errors.Add(error);
            }
            if(IsContainInValidHFType(model.HFType))
            {
                var error = GetBulkInsertValidationFailure(index, "Invalid HF Type !", string.Empty,
                   nameof(model.HFType));
                errors.Add(error);
            }
            /*if(IsContainInValidHFName(model.HFName))
            {
                var error = GetBulkInsertValidationFailure(index, "Invalid HF Name !", string.Empty,
                  nameof(model.HFName));
                errors.Add(error);
            }
            if(AreHFNameAndHFTypeDifferent(model.HFName,model.HFType))
            {
                var error = GetBulkInsertValidationFailure(index, "Different institution type found in HF Type and HF Name !", string.Empty,
                  nameof(model.HFName));
                var error1 = GetBulkInsertValidationFailure(index, "Different institution type found in HF Type and HF Name !", string.Empty,
                  nameof(model.HFType));
                errors.Add(error);
                errors.Add(error1);
            }*/
            if (!IsContainDistrictShortCode(model))
            {
                var districtName = _stateDistrictCities.FirstOrDefault(x => x.DistrictId == model.SelectedUserDistrictId)?.DistrictName;
                var error = GetBulkInsertValidationFailure(index, $"No District code found for {districtName}. Please Contact the Admin !", string.Empty,
                    nameof(model.SelectedUserDistrictId));
                errors.Add(error);
            }

            return new BulkInsertValidationResult(errors);
        }
        #endregion

        public async Task<BulkInsertValidationResult> ValidateAsync(IList<MemberBulkImportVM> models)
        {
            var errors = new List<BulkInsertValidationFailure>();

            await SetEmailsForValidation(models);
            await SetMobilesForValidation(models);
            await SetInstitutionsForValidation(models);
            await SetMenusForValidation();
            await SetStateDistrictsForValidation();
            await SetHFTypesForValidation();

            var emails = models.Select(x => x.UserEmail);
            var mobiles = models.Select(x => x.UserMobile);
            for (var i = 0; i < models.Count; i++)
            {
                var validationResult = await ValidateAsync(models[i], i);
                var model = models[i];
                if (IsDuplicateInstituteEmail(model, models))
                {
                    var error = GetBulkInsertValidationFailure(i, "Another institution with same email address exist !", string.Empty,
                            nameof(model.HFEmail));
                    errors.Add(error);
                    //Institution Email Address or Mobile Number already exist
                }
                if (IsDuplicateInstituteMobile(model, models))
                {
                    var error = GetBulkInsertValidationFailure(i, "Another institution with same mobile number exist !", string.Empty,
                            nameof(model.HFPhone));
                    errors.Add(error);
                    //Institution Email Address or Mobile Number already exist
                }
                if (IsInvalidInstituteEmail(model, models))
                {
                    var error = GetBulkInsertValidationFailure(i, "Institution Email Address already exist !", string.Empty,
                            nameof(model.HFEmail));
                    errors.Add(error);
                    //Institution Email Address or Mobile Number already exist
                }
                if (IsInvalidInstituteMobile(model, models))
                {
                    var error = GetBulkInsertValidationFailure(i, "Institution Mobile Number already exist !", string.Empty,
                            nameof(model.HFPhone));
                    errors.Add(error);
                    //Institution Email Address or Mobile Number already exist
                }
               /* if (IsInvalidHFNameWithDistrict(model, models))
                {
                    var error1 = GetBulkInsertValidationFailure(i, "Duplicate HF Name with District !", string.Empty,
                          nameof(model.HFName));
                    var error2 = GetBulkInsertValidationFailure(i, "Duplicate HF Name with District !", string.Empty,
                          nameof(model.HFDistrict));
                    errors.Add(error1);
                    errors.Add(error2);
                }*/
                if (IsDuplicateEmail(models[i].UserEmail, emails))
                {
                    
                    var error = GetBulkInsertValidationFailure(i, "Duplicate User Email !", string.Empty,
                            nameof(model.UserEmail));
                    errors.Add(error);
                }
                if (IsDuplicateMobile(models[i].UserMobile,mobiles))
                {
                    var error = GetBulkInsertValidationFailure(i, "Duplicate User Mobile !", string.Empty,
                        nameof(model.UserMobile));
                    errors.Add(error);
                }
                if (!validationResult.IsValid)
                {
                    errors.AddRange(validationResult.Errors);
                }
            }
            return new BulkInsertValidationResult(errors);
        }
        public BulkInsertValidationResult Validate(IList<MemberBulkImportVM> models)
        {
            throw new NotImplementedException();
        }
    }
}
