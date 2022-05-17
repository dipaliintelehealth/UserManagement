using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using UserManagement.Domain.ViewModel;
using UserManagement.Contract.User;
using System.Globalization;
using System.Text.RegularExpressions;

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
            
            RuleFor(x => x.HFName)
                .Must(t => Regex.IsMatch(t, @"^[\w\s]*$"))
                .WithMessage("Invalid character in HF name!");

            RuleFor(x => x.HFShortName)
                .NotEmpty()
                .WithMessage("HF Short Name can not be blank !");

            RuleFor(x => x.HFShortName)
             .Must(t => Regex.IsMatch(t, @"^[a-zA-Z\s]*$"))
             .When(t => !string.IsNullOrWhiteSpace(t.HFShortName))
             .WithMessage("Invalid character in HF Short Name !");

            RuleFor(x => x.HFPhone)
                .NotEmpty()
                .WithMessage("HF Phone can not be blank !");

            RuleFor(x => x.HFPhone)
                .NotEqual(x => x.UserMobile)
                .WithMessage("HF Phone can not be same as User Mobile !");

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

            RuleFor(x => x.HFEmail)
                .NotEqual(x => x.UserEmail)
                .WithMessage("HF Email can not be same as User Email !");

            RuleFor(x => x.SelectedHFStateId)
                .NotEmpty()
                .WithMessage("Invalid HF State Name !");

           
            RuleFor(x => x.SelectedHFDistrictId)
                .NotEmpty()
                .WithMessage("Invalid HF District Name !");

            RuleFor(x => x.SelectedHFCityId)
                .NotEmpty()
                .WithMessage("Invalid HF City Name !");

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

            RuleFor(x => x.UserMobile)
                .NotEmpty()
                .WithMessage("User Mobile can not be blank !");

            RuleFor(x => x.UserMobile)
                .Matches("^[0-9]{10}$")
                .WithMessage("Invalid User Mobile !");

            RuleFor(x => x.Gender)
                .NotEmpty()
                .WithMessage("User Gender can not be blank!");

            RuleFor(x => x.Qualification)
                .NotEmpty()
                .WithMessage("User Qualification can not be blank!");

            RuleFor(x => x.UserEmail)   
                .NotEmpty()
                .WithMessage("User Email can not be blank !");

            RuleFor(x => x.UserEmail)
                .EmailAddress()
                .WithMessage("Invalid User Email !");

            //RuleFor(x => x.Designation)
            //    .NotEmpty()
            //    .WithMessage("User Specialization / Designation can not be blank!");

            RuleFor(x => x.DOB)
                .NotEmpty()
                .WithMessage("User Date of Birth can not be blank !");

            RuleFor(x => x.DOB)
                .Must(x => DateTime.TryParseExact(x, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                .When(x => !string.IsNullOrWhiteSpace(x.DOB))
                .WithMessage("Invalid Date of Birth it should be in DD-MM-YYYY !");

            RuleFor(x => x.SelectedUserStateId)
                .NotEmpty()
                .WithMessage("Invalid User State Name !");

            RuleFor(x => x.SelectedUserDistrictId)
              .NotEmpty()
              .WithMessage("Invalid User District Name !");

            RuleFor(x => x.SelectedUserCityId)
                .NotEmpty()
                .WithMessage("Invalid User City Name !");

            RuleFor(x => x.SelectedSpecialityId)
                 .NotEmpty()
                 .WithMessage("Invalid User Specialization / Designation !");

            RuleFor(x => x.UserAddress)
               .NotEmpty()
               .WithMessage("User Address can not be blank !");

            RuleFor(x => x.UserPin)
                .NotEmpty()
                .WithMessage("User PIN can not be blank !");

            RuleFor(x => x.UserAvailableDay)
                .NotEmpty()
                .WithMessage("Invalid Day and Time (Availability)!");

            RuleFor(x => x.UserAvailableFromTime)
                .NotEmpty()
                .WithMessage("Invalid Availability From Time !");

            RuleFor(x => x.UserAvailableToTime)
                .NotEmpty()
                .WithMessage("Invalid Availability To Time !");

            RuleFor(x => x.UserRole)
                .NotEmpty()
                .WithMessage("Invalid Role!");
        }
    }
}