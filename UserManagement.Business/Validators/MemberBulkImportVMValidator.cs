using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using UserManagement.Domain.ViewModel;
using UserManagement.Contract.User;

namespace UserManagement.Business.Validators
{
    public class MemberBulkImportVMValidator : AbstractValidator<MemberBulkImportVM>
    {
        public MemberBulkImportVMValidator()
        {
            RuleFor(x => x.HFName).NotEmpty().WithMessage("HFName can not be blank !");
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("User First Name can not be blank !");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("User Last Name can not be blank !");
            RuleFor(x => x.UserMobile).NotEmpty().WithMessage("User Mobile can not be blank !");
            RuleFor(x => x.UserEmail).NotEmpty().WithMessage("User Email can not be blank !");
            RuleFor(x => x.UserEmail).EmailAddress().WithMessage("Invalid User Email !");
            RuleFor(x => x.UserMobile).Matches("^[0-9]{10}$").WithMessage("Invalid User Mobile !");
            RuleFor(x => x.StateId).NotEmpty().WithMessage("Invalid State Name !");
            RuleFor(x => x.DistrictId).NotEmpty().WithMessage("Invalid District Name !");
            RuleFor(x => x.CityId).NotEmpty().WithMessage("Invalid City Name !");
            RuleFor(x => x.UserStateId).NotEmpty().WithMessage("Invalid User State Name !");
            RuleFor(x => x.UserDistrictId).NotEmpty().WithMessage("Invalid User District Name !");
            RuleFor(x => x.UserCityId).NotEmpty().WithMessage("Invalid User City Name !");
            RuleFor(x => x.UserRole).NotEmpty().WithMessage("Invalid Role!");
            RuleFor(x => x.UserAvilableDay).NotEmpty().WithMessage("Invalid Day and Time (Availability)!");
            RuleFor(x => x.UserAvilableFromTime).NotEmpty().WithMessage("Invalid Availability From Time !");
            RuleFor(x => x.UserAvilableToTime).NotEmpty().WithMessage("Invalid Availability To Time !");

        }
    }
}
