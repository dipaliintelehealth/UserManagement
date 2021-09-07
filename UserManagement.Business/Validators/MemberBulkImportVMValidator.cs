using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using UserManagement.Domain.ViewModel;
using UserManagement.Contract.User;
using System.Globalization;

namespace UserManagement.Business.Validators
{
    public class MemberBulkImportVMValidator : AbstractValidator<MemberBulkImportVM>
    {
        public MemberBulkImportVMValidator()
        {
            DateTime dt;
            RuleFor(x => x.HFName)
                .NotEmpty()
                .WithMessage("HFName can not be blank !");

            RuleFor(x => x.HFEmail)
                .EmailAddress()
                .When(x => !string.IsNullOrWhiteSpace(x.HFEmail))
                .WithMessage("Invalid HF Email !");

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("User First Name can not be blank !");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("User Last Name can not be blank !");

            RuleFor(x => x.UserMobile)
                .NotEmpty()
                .WithMessage("User Mobile can not be blank !");

            RuleFor(x => x.UserEmail)
                .NotEmpty()
                .WithMessage("User Email can not be blank !");

            RuleFor(x => x.UserEmail)
                .EmailAddress()
                .When(x=>!string.IsNullOrWhiteSpace(x.UserEmail))
                .WithMessage("Invalid User Email !");

            RuleFor(x => x.UserMobile)
                .Matches("^[0-9]{10}$")
                .When(x=>!string.IsNullOrWhiteSpace(x.UserMobile))
                .WithMessage("Invalid User Mobile !");

            RuleFor(x => x.StateId)
                .NotEmpty()
                .WithMessage("Invalid HF State Name !");

            RuleFor(x => x.DistrictId)
                .NotEmpty()
                .WithMessage("Invalid HF District Name !");

            RuleFor(x => x.CityId)
                .NotEmpty()
                .WithMessage("Invalid HF City Name !");

            RuleFor(x => x.UserStateId)
                .NotEmpty()
                .WithMessage("Invalid User State Name !");

            RuleFor(x => x.UserDistrictId)
                .NotEmpty()
                .WithMessage("Invalid User District Name !");

            RuleFor(x => x.UserCityId)
                .NotEmpty()
                .WithMessage("Invalid User City Name !");

            RuleFor(x => x.UserRole)
                .NotEmpty()
                .WithMessage("Invalid Role!");

            RuleFor(x => x.UserAvilableDay)
                .NotEmpty()
                .WithMessage("Invalid Day and Time (Availability)!");

            RuleFor(x => x.UserAvilableFromTime)
                .NotEmpty()
                .WithMessage("Invalid Availability From Time !");

            RuleFor(x => x.UserAvilableToTime)
                .NotEmpty()
                .WithMessage("Invalid Availability To Time !");

            RuleFor(x => x.DOB)
                .Must(x=>DateTime.TryParseExact(x,"dd/MM/yyyy", CultureInfo.InvariantCulture,DateTimeStyles.None, out dt))
                .When(x=> !string.IsNullOrWhiteSpace(x.DOB))
                .WithMessage("Invalid Date of Birth!");

        }
    }
}
