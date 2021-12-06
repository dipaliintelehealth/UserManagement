using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UserManagement.Business.Validators;
using UserManagement.Contract.Repository;
using UserManagement.Contract.Utility;
using UserManagement.Domain;
using UserManagement.Domain.ViewModel;
using Xunit;

namespace UserManagement.Business.Tests
{
    public class MemberBulkInsertValidatorTest
    {
        [Fact]
        public async System.Threading.Tasks.Task ImportData_Should_Provide_InvalidUsers()
        {
            #region Arrange

            var repoMock = new Mock<IMemberBulkInsertRepository>();
            repoMock.Setup(bl => bl.GetStateDistrictCities().Result).Returns(new List<StateDistrictCity>
            {
                new StateDistrictCity()
                {
                    StateId =123,
                    StateName ="Maharashtra",
                    DistrictId = 123,
                    DistrictName ="Pune",
                    DistrictShortCode ="PN",
                    CityId = 1,
                    CityName ="Pune"
                }
            });
            repoMock.Setup(bl => bl.FindEmails(It.IsAny<IEnumerable<string>>()).Result).Returns(new List<string>() {
              "test1@gmail.com",
               "test2@gmail.com",
               "test3@gmail.com"
            });
            repoMock.Setup(bl => bl.FindMobiles(It.IsAny<IEnumerable<string>>()).Result).Returns(new List<string>() {
              "1234567890",
               "45678901234",
               "2345678901"
            });

            repoMock.Setup(bl => bl.GetSubMenu().Result).Returns(new List<SubMenuModel>() {

                new SubMenuModel()
                {
                    SubMenuId = "5",
                    SubMenuName = "User Dashboard",
                    MenuMappingId = "33"
                },
                new SubMenuModel()
                {
                    SubMenuId = "6",
                    SubMenuName = "Patient List",
                    MenuMappingId = "34"
                },
                 new SubMenuModel()
                {
                    SubMenuId = "7",
                    SubMenuName = "Add Patient",
                    MenuMappingId = "35"
                }
            });

            var data = new List<MemberBulkImportVM>
            {
                 new MemberBulkImportVM()
                 {
                     UserEmail = "test1@gmail.com",
                     UserMobile = "2424242424",
                     UserDistrict = "Pune",
                     SelectedUserDistrictId =123,
                     UserCity ="Pune",
                     SelectedUserCityId=1,
                     UserState ="Maharashtra",
                     SelectedUserStateId =123,
                     HFDistrict = "Pune",
                     SelectedHFDistrictId = 123,
                     HFCity ="Pune",
                     SelectedHFCityId=1,
                     HFState ="Maharashtra",
                     SelectedHFStateId = 123,
                     SubMenuName = "User Dashboard"
                 },
                  new MemberBulkImportVM()
                 {
                     UserEmail = "xyz@gmail.com",
                     UserMobile = "45678901234",
                    UserDistrict = "Test",
                     SelectedUserDistrictId =0,
                     UserCity ="Pune",
                     SelectedUserCityId=1,
                     UserState ="Maharashtra",
                     SelectedUserStateId =123,
                     HFDistrict = "Pune",
                     SelectedHFDistrictId = 123,
                     HFCity ="Pune",
                     SelectedHFCityId=1,
                     HFState ="Maharashtra",
                     SelectedHFStateId = 123,
                     SubMenuName = "Patient List"
                 },
                 new MemberBulkImportVM()
                 {
                     UserEmail = "abc@gmail.com",
                     UserMobile = "2424242424",
                    UserDistrict = "Pune",
                     SelectedUserDistrictId =123,
                     UserCity ="Pune",
                     SelectedUserCityId=1,
                     UserState ="Maharashtra",
                     SelectedUserStateId =123,
                     HFDistrict = "Pune",
                     SelectedHFDistrictId = 123,
                     HFCity ="Pune",
                     SelectedHFCityId=1,
                     HFState ="Maharashtra",
                     SelectedHFStateId = 123,
                     SubMenuName = "Add Patient"
                 },
                  new MemberBulkImportVM()
                 {
                     UserEmail = "abc1@gmail.com",
                   UserDistrict = "Pune",
                     SelectedUserDistrictId =123,
                     UserCity ="Pune",
                     SelectedUserCityId=1,
                     UserState ="Maharashtra",
                     SelectedUserStateId =123,
                     HFDistrict = "Pune",
                     SelectedHFDistrictId = 123,
                     HFCity ="Pune",
                     SelectedHFCityId=1,
                     HFState ="HR",
                     SelectedHFStateId = 0,
                     SubMenuName = ""
                 }
            };
            var validator = new MemberBulkInsertValidator(new MemberBulkImportVMValidator(), repoMock.Object);

            #endregion

            #region Act

            var result = await validator.ValidateAsync(data);

            #endregion

            #region Assert

            Assert.True(result != null, "result is not empty");
             //Assert.True(result.Errors.Count == 4, "total invalid count should match");
             Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("Duplicate User")).Count() == 2, "duplicate user count should match");
             Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("Invalid User District Name !")).Count() == 1, "invalid district count should match");
             Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("Invalid HF State Name !")).Count() == 1, "invalid state count should match");
             Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("Invalid HF District Name !")).Count() == 0, "invalid district count should match");
             Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("Invalid HF City Name !")).Count() == 0, "invalid city count should match");
            #endregion
        }

        [Fact]
        public async System.Threading.Tasks.Task ImportData_Should_Provide_InvalidUsers_WithProperErrorMessage()
        {
            #region Arrange

            var repoMock = new Mock<IMemberBulkInsertRepository>();
            repoMock.Setup(bl => bl.GetStateDistrictCities().Result).Returns(new List<StateDistrictCity>
            {
                new StateDistrictCity()
                {
                    StateId =123,
                    StateName ="Maharashtra",
                    DistrictId = 123,
                    DistrictName ="Pune",
                    DistrictShortCode ="PN",
                    CityId = 1,
                    CityName ="Bhor"
                }
            });
            repoMock.Setup(bl => bl.FindEmails(It.IsAny<IEnumerable<string>>()).Result).Returns(new List<string>() {
              "test1@gmail.com",
               "test2@gmail.com",
               "test3@gmail.com"
            });
            repoMock.Setup(bl => bl.FindMobiles(It.IsAny<IEnumerable<string>>()).Result).Returns(new List<string>() {
              "1234567890",
               "45678901234",
               "2345678901"
            });


            repoMock.Setup(bl => bl.GetSubMenu().Result).Returns(new List<SubMenuModel>() {

                new SubMenuModel()
                {
                    SubMenuId = "5",
                    SubMenuName = "User Dashboard",
                    MenuMappingId = "33"
                },
                new SubMenuModel()
                {
                    SubMenuId = "6",
                    SubMenuName = "Patient List",
                    MenuMappingId = "34"
                },
                 new SubMenuModel()
                {
                    SubMenuId = "7",
                    SubMenuName = "Add Patient",
                    MenuMappingId = "35"
                }
            });

            
            var data = new List<MemberBulkImportVM>
            {
                 new MemberBulkImportVM()
                 {
                     HFName = "Hub Ramesh Mulani",
                     HFPhone = "1446875567",
                     HFType = "HUB",
                     NIN = "4877253429",
                     HFEmail = "rameshmulani@gmail.com",
                     HFState = "Maharashtra",
                     HFDistrict = "Pune",
                     HFCity = "Pune",
                     Address = "SC Ramesh Pune",
                     PIN = "497335",
                     FirstName = "Rose",
                     LastName = "Merry",
                     UserMobile = "9913104637",
                     Gender="Female",
                     Qualification="Other",
                     Experience = "1.7",
                     DRRegNo = "4545",
                     UserEmail = "rose.merry@gmail.com",
                     Designation= "CHO",
                     DOB = "12/13/1988",
                     UserState = "Maharashtra",
                     UserDistrict = "Pune",
                     UserCity = "Pune",
                     UserAddress = "Rose Merry Pune",
                     UserPin = "412001",
                     UserPrefix = "Ms",
                     UserAvailableDay = "Monday,Saturday",
                     UserAvailableFromTime = "9:00 AM",
                     UserAvailableToTime = "4:00 PM",
                     UserRole="2",
                     SubMenuName="User Dashboard"
                 }
            };
           
            var validator = new MemberBulkInsertValidator(new MemberBulkImportVMValidator(), repoMock.Object);
            #endregion

            #region Act
            
            var result = await validator.ValidateAsync(data);

            #endregion

            #region Assert

            Assert.True(result != null, "result is not empty");
         
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("Invalid Date of Birth it should be in DD-MM-YYYY !")).Count() == 1, "invalid DOB count should match");
            Assert.True(!result.IsValid, "result should be invalid");
            
            #endregion
        }

        [Fact]
        public async System.Threading.Tasks.Task GetMemberMenus_When_SubMenuId_Is_Present()
        {
            var validatedModels = new List<MemberBulkImportVM>()
            {


                    new MemberBulkImportVM()
                    {
                        UserName="pbaboharfazsc",
                        UserState="PUNJAB",
                        UserDistrict="FAZILKA",
                        HFName="HSC ABOHAR",
                        HFType="SubCentre",
                        SubMenuName="User Dashboard,Patient List"

                    }

            };

            var subMenus = new List<SubMenuModel>()
            {
                new SubMenuModel()
                {
                    SubMenuId = "5",
                    SubMenuName = "User Dashboard",
                    MenuMappingId = "33"
                },
                new SubMenuModel()
                {
                    SubMenuId = "6",
                    SubMenuName = "Patient List",
                    MenuMappingId = "34"
                },
                 new SubMenuModel()
                {
                    SubMenuId = "7",
                    SubMenuName = "Add Patient",
                    MenuMappingId = "35"
                }
            };
            var repoMock = new Mock<IMemberBulkInsertRepository>();
            repoMock.Setup(bl => bl.GetSubMenu().Result).Returns(subMenus);

            var validator = new MemberBulkInsertValidator(new MemberBulkImportVMValidator(), repoMock.Object);
            var result = await validator.ValidateAsync(validatedModels);

            Assert.NotNull(result);

            //Assert.Contains(result, x => x..MenuMappingId == "33");
            //Assert.Contains(result, x => x.MenuMappingId == "34");

        }

        [Fact]
        public async System.Threading.Tasks.Task CheckSubMenu_When_SubMenu_Is_Invalid()
        {
            var validatedModels = new List<MemberBulkImportVM>()
            {


                    new MemberBulkImportVM()
                    {
                        UserName="pbaboharfazsc",
                        UserState="PUNJAB",
                        UserDistrict="FAZILKA",
                        HFName="HSC ABOHAR",
                        HFType="SubCentre",
                        SubMenuName="Doctor List"

                    }

            };

            var subMenus = new List<SubMenuModel>()
            {
               new SubMenuModel()
                {
                    SubMenuId = "5",
                    SubMenuName = "User Dashboard",
                    MenuMappingId = "33"
                },
                new SubMenuModel()
                {
                    SubMenuId = "6",
                    SubMenuName = "Patient List",
                    MenuMappingId = "34"
                },
                 new SubMenuModel()
                {
                    SubMenuId = "7",
                    SubMenuName = "Add Patient",
                    MenuMappingId = "35"
                }
            };
            var repoMock = new Mock<IMemberBulkInsertRepository>();
            repoMock.Setup(bl => bl.GetSubMenu().Result).Returns(subMenus);

            var validator = new MemberBulkInsertValidator(new MemberBulkImportVMValidator(), repoMock.Object);
            var result = await validator.ValidateAsync(validatedModels);

            Assert.NotNull(result);
            Assert.True(!result.IsValid, "Validation should fail");
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("Invalid Sub Menu !")).Count() == 1, "invalid sub menu id count should match");
        }

        [Fact]
        public async System.Threading.Tasks.Task CheckSubMenu_When_SubMenuId_Is_Empty()
        {
            var validatedModels = new List<MemberBulkImportVM>()
            {


                    new MemberBulkImportVM()
                    {
                        UserName="pbaboharfazsc",
                        UserState="PUNJAB",
                        UserDistrict="FAZILKA",
                        HFName="HSC ABOHAR",
                        HFType="SubCentre",
                        SubMenuName=" "


                },


                    new MemberBulkImportVM()
                    {
                        UserName="pbaboharfazsc",
                        UserState="PUNJAB",
                        UserDistrict="FAZILKA",
                        HFName="HSC ABOHAR",
                        HFType="SubCentre",
                        SubMenuName="User Dashboard"

                    }

            };

            var subMenus = new List<SubMenuModel>()
            {
                new SubMenuModel()
                {
                    SubMenuId = "5",
                    SubMenuName = "User Dashboard"
                },
                new SubMenuModel()
                {
                    SubMenuId = "6",
                    SubMenuName = "Patient List"
                },
                 new SubMenuModel()
                {
                    SubMenuId = "7",
                    SubMenuName = "Add Patient"
                }
            };
            var repoMock = new Mock<IMemberBulkInsertRepository>();
            repoMock.Setup(bl => bl.GetSubMenu().Result).Returns(subMenus);
            var validator = new MemberBulkInsertValidator(new MemberBulkImportVMValidator(), repoMock.Object);
            var result = await validator.ValidateAsync(validatedModels);

            Assert.NotNull(result);
            Assert.True(!result.IsValid,"Validation should be invalid");
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("Invalid Sub Menu !")).Count() == 1, "invalid sub menu id count should match");
            
        }
    }
}
