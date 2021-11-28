using FluentValidation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Contract;
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
        private IEnumerable<string> mobiles;
        private IEnumerable<string> emails;
        private IEnumerable<string> menus;

        public MemberBulkInsertValidator(IValidator<MemberBulkImportVM> validator, IMemberBulkInsertRepository repository)
        {
            this._validator = validator;
            this._repository = repository;
        }
        public BulkInsertValidationResult Validate(IList<MemberBulkImportVM> models)
        {
            throw new NotImplementedException();
        }
        private async Task SetEmailsForValidation(IList<MemberBulkImportVM> models)
        {
            this.emails = await _repository.FindEmails(models.Select(mdel => mdel.UserEmail));
        }
        private async  Task SetMobilesForValidation(IList<MemberBulkImportVM> models)
        {
            this.mobiles = await _repository.FindMobiles(models.Select(mdel => mdel.UserMobile));
        }
        private async Task SetMenusForValidation(IList<MemberBulkImportVM> models)
        {
            var memberMenus = await _repository.GetSubMenu();
            menus = memberMenus?.Select(x => x.SubMenuName);
        }
        private bool IsDuplicateMobile(string mobile)
        {
            return mobiles.Contains(mobile);
        }
        private bool IsDuplicateEmail(string email)
        {
            return emails.Contains(email);
        }
        private bool IsContainsInValidMenu(string menuString)
        {
            var userMenus = menuString?.Split(',')?.ToList();
            return userMenus == null || userMenus.Count() == 0 || userMenus.Any(t => !menus.Contains(t.Trim()));
        }
        public BulkInsertValidationResult Validate(MemberBulkImportVM model)
        {
            throw new NotImplementedException();
        }

        public async Task<BulkInsertValidationResult> ValidateAsync(IList<MemberBulkImportVM> models)
        {
            IList<BulkInsertValidationFailure> errors = new List<BulkInsertValidationFailure>();

            await SetEmailsForValidation(models);
            await SetMobilesForValidation(models);
            await SetMenusForValidation(models);

            for (int i = 0; i < models.Count; i++)
            {
                var validationResult = _validator.Validate(models[i]);
                if (!validationResult.IsValid)
                {
                    foreach (var fluentError in validationResult.Errors)
                    {
                        var error = new BulkInsertValidationFailure()
                        {
                            Index = i,
                            ErrorMessage = fluentError?.ErrorMessage,
                            ErrorCode = fluentError?.ErrorCode,
                            PropertyName = fluentError?.PropertyName
                        };
                        errors.Add(error);
                    }
                }
                if(IsDuplicateEmail(models[i].UserEmail))
                {
                    var error = new BulkInsertValidationFailure()
                    {
                        Index = i,
                        ErrorMessage = "Duplicate User Email",
                        ErrorCode = string.Empty,
                        PropertyName = "UserEmail"
                    };
                    errors.Add(error);
                }
                if (IsDuplicateMobile(models[i].UserMobile))
                {
                    var error = new BulkInsertValidationFailure()
                    {
                        Index = i,
                        ErrorMessage = "Duplicate User Mobile",
                        ErrorCode = string.Empty,
                        PropertyName = "UserMobile"
                    };
                    errors.Add(error);
                }
                if (IsContainsInValidMenu(models[i].SubMenuName))
                {
                    var error = new BulkInsertValidationFailure()
                    {
                        Index = i,
                        ErrorMessage = "Invalid Sub Menu !",
                        ErrorCode = string.Empty,
                        PropertyName = "SubMenuName"
                    };
                    errors.Add(error);
                }
            }
            return new BulkInsertValidationResult(errors);
        }

        public Task<BulkInsertValidationResult> ValidateAsync(MemberBulkImportVM model)
        {
            throw new NotImplementedException();
        }
    }
}
