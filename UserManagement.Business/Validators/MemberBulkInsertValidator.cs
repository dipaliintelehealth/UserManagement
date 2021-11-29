using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Contract.Repository;
using UserManagement.Contract.Validator;
using UserManagement.Domain.Validator;
using UserManagement.Domain.ViewModel;

namespace UserManagement.Business.Validators
{
    public class MemberBulkInsertValidator : IBulkInsertValidator<MemberBulkImportVM>
    {
        private readonly IValidator<MemberBulkImportVM> _validator;
        private readonly IMemberBulkInsertRepository _repository;
        private IEnumerable<string> _mobiles= Enumerable.Empty<string>();
        private IEnumerable<string> _emails= Enumerable.Empty<string>();
        private IEnumerable<string> _menus = Enumerable.Empty<string>();

        public MemberBulkInsertValidator(IValidator<MemberBulkImportVM> validator, IMemberBulkInsertRepository repository)
        {
            this._validator = validator;
            this._repository = repository;
        }
        
        #region Private Methods
        private async Task SetEmailsForValidation(IList<MemberBulkImportVM> models)
        {
            this._emails = await _repository.FindEmails(models.Select(model => model.UserEmail));
        }
        private async  Task SetMobilesForValidation(IList<MemberBulkImportVM> models)
        {
            this._mobiles = await _repository.FindMobiles(models.Select(model => model.UserMobile));
        }
        private async Task SetMenusForValidation()
        {
            var memberMenus = await _repository.GetSubMenu();
            _menus = memberMenus?.Select(x => x.SubMenuName);
        }
        private bool IsDuplicateMobile(string mobile)
        {
            return _mobiles.Contains(mobile);
        }
        private bool IsDuplicateEmail(string email)
        {
            return _emails.Contains(email);
        }
        private bool IsContainsInValidMenu(string menuString)
        {
            var userMenus = menuString?.Split(',')?.ToList();
            return userMenus == null || !userMenus.Any() || userMenus.Any(t => !_menus.Contains(t.Trim()));
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
        private async Task<BulkInsertValidationResult> ValidateAsync(MemberBulkImportVM model,int index=0)
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
            if(IsDuplicateEmail(model.UserEmail))
            {
                var error = GetBulkInsertValidationFailure(index, "Duplicate User Email !", string.Empty,
                    nameof(model.UserEmail));
                errors.Add(error);
            }
            if (IsDuplicateMobile(model.UserMobile))
            {
                var error = GetBulkInsertValidationFailure(index, "Duplicate User Mobile !", string.Empty,
                    nameof(model.UserMobile));
                errors.Add(error);
            }
            if (IsContainsInValidMenu(model.SubMenuName))
            {
                var error = GetBulkInsertValidationFailure(index, "Invalid Sub Menu !", string.Empty,
                    nameof(model.SubMenuName));
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
            await SetMenusForValidation();

            for (var i = 0; i < models.Count; i++)
            {
                var validationResult = await ValidateAsync(models[i],i);
                if (validationResult.IsValid) continue;
                errors.AddRange(validationResult.Errors);
            }
            return new BulkInsertValidationResult(errors);
        }
        public BulkInsertValidationResult Validate(IList<MemberBulkImportVM> models)
        {
            throw new NotImplementedException();
        }
    }
}
