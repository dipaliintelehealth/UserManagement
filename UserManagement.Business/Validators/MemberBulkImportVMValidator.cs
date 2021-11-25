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
            //HF Validation
            RuleFor(x => x.HFName)
                .NotEmpty()
                .WithMessage("HF Name can not be blank !");

            RuleFor(x => x.HFPhone)
                .NotEmpty()
                .WithMessage("HF Phone can not be blank !");

            RuleFor(x => x.HFType)
                .NotEmpty()
                .WithMessage("HF Type can not be blank !");

            RuleFor(x => x.NIN)
                .NotEmpty()
                .WithMessage("HF NIN can not be blank !");

            RuleFor(x => x.HFEmail)
                .NotEmpty()
                .WithMessage("HF Email can not be blank !");

            RuleFor(x => x.HFEmail)
                .EmailAddress()
                .WithMessage("Invalid HF Email !");

            RuleFor(x => x.StateId)
                .NotEmpty()
                .WithMessage("Invalid HF State Name !");

            RuleFor(x => x.State)
                .NotEmpty()
                .WithMessage("HF State can not be blank !");

            RuleFor(x => x.DistrictId)
                .NotEmpty()
                .When(x=> x.StateId != default)
                .WithMessage("Invalid HF District Name !");

            RuleFor(x => x.District)
                .NotEmpty()
                .WithMessage("HF District can not be blank !");

            RuleFor(x => x.CityId)
                .NotEmpty()
                .When(x=> x.DistrictId != default)
                .WithMessage("Invalid HF City Name !");

            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage("HF City can not be blank !");

            RuleFor(x => x.Address)
                            .NotEmpty()
                            .WithMessage("HF Address can not be blank !");

            RuleFor(x => x.PIN)
                .NotEmpty()
                .WithMessage("HF PIN can not be blank !");

            //User Validation 
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("User First Name can not be blank !");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("User Last Name can not be blank !");

            /*RuleFor(x => x.UserMobile)
                .NotEmpty()
                .WithMessage("User Mobile can not be blank !");*/

            RuleFor(x => x.UserMobile)
                .Matches("^[0-9]{10}$")
                .WithMessage("Invalid User Mobile !");

            RuleFor(x => x.Gender)
                .NotEmpty()
                .WithMessage("User Gender can not be blank!");

            RuleFor(x => x.Qualification)
                .NotEmpty()
                .WithMessage("User Qualification can not be blank!");

            /*RuleFor(x => x.UserEmail)
                .NotEmpty()
                .WithMessage("User Email can not be blank !");*/

            RuleFor(x => x.UserEmail)
                .EmailAddress()
                .WithMessage("Invalid User Email !");

            RuleFor(x => x.Designation)
                .NotEmpty()
                .WithMessage("User Specialization / Designation can not be blank!");

            RuleFor(x => x.DOB)
                .NotEmpty()
                .WithMessage("User Date of Birth can not be blank !");

            RuleFor(x => x.DOB)
                .Must(x => DateTime.TryParseExact(x, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                .When(x => !string.IsNullOrWhiteSpace(x.DOB))
                .WithMessage("Invalid Date of Birth it should be in DD-MM-YYYY !");

            RuleFor(x => x.UserStateId)
                .NotEmpty()
                .WithMessage("Invalid User State Name !");

            RuleFor(x => x.UserState)
               .NotEmpty()
               .WithMessage("User State can not be blank !");

            RuleFor(x => x.UserDistrictId)
                .NotEmpty()
                .When(x => x.UserStateId != default)
                .WithMessage("Invalid User District Name !");

            RuleFor(x => x.UserDistrict)
               .NotEmpty()
               .WithMessage("User District can not be blank !");

            RuleFor(x => x.UserCityId)
                .NotEmpty()
                .When(x => x.UserDistrictId != default)
                .WithMessage("Invalid User City Name !");

            RuleFor(x => x.UserCity)
               .NotEmpty()
               .WithMessage("User City can not be blank !");

            RuleFor(x => x.UserAddress)
               .NotEmpty()
               .WithMessage("User Address can not be blank !");

            RuleFor(x => x.UserPin)
                .NotEmpty()
                .WithMessage("User PIN can not be blank !");

            RuleFor(x => x.UserAvilableDay)
                .NotEmpty()
                .WithMessage("Invalid Day and Time (Availability)!");

            RuleFor(x => x.UserAvilableFromTime)
                .NotEmpty()
                .WithMessage("Invalid Availability From Time !");

            RuleFor(x => x.UserAvilableToTime)
                .NotEmpty()
                .WithMessage("Invalid Availability To Time !");

            RuleFor(x => x.UserRole)
                .NotEmpty()
                .WithMessage("Invalid Role!");

            //RuleFor(x => x.SubMenuID)
            //   .NotEmpty()
            //   .WithMessage("Sub Menu Id can not be blank !");

            //RuleFor(x => x.SubMenuID)
            //    .Matches("^[5-9](;[5-9])*$")
            //    .When(x => !string.IsNullOrWhiteSpace(x.SubMenuID))
            //    .WithMessage("Invalid Sub Menu Id!");
        }
    }
}