using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UserManagement.Contract.Repository;
using UserManagement.Contract.Utility;
using UserManagement.Domain;
using UserManagement.Domain.ViewModel;
using Xunit;

namespace UserManagement.Business.Tests
{
    public class MemberBulkDataImportServiceTest
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
            var excelMock = new Mock<IExcelFileUtility<MemberBulkImportVM>>();
            excelMock.Setup(ex => ex.Read(It.IsAny<Stream>())).Returns(new List<MemberBulkImportVM>
            {
                 new MemberBulkImportVM()
                 {
                     UserEmail = "test1@gmail.com",
                     UserMobile = "2424242424",
                     UserDistrict = "Pune",
                     UserCity ="Pune",
                     UserState ="Maharashtra"
                 },
                  new MemberBulkImportVM()
                 {
                     UserEmail = "xyz@gmail.com",
                     UserMobile = "45678901234",
                     UserDistrict = "test",
                     UserCity ="Pune",
                     UserState ="Maharashtra"
                 },
                 new MemberBulkImportVM()
                 {
                     UserEmail = "abc@gmail.com",
                     UserMobile = "2424242424",
                     UserDistrict = "Pune",
                     UserCity ="Pune",
                     UserState ="Maharashtra"
                 }
            });
            var service = new MemberBulkDataImportService(excelMock.Object, repoMock.Object);

            #endregion

            #region Act

            var result = await service.ImportData(new MemoryStream(),string.Empty);

            #endregion

            #region Assert

            Assert.True(result != null && result.Count() > 0, "result is not empty");
            var notScussess = result.Where(x => !x.Success);
            Assert.True(notScussess.Count() == 3, "total invalid count should match");
            Assert.True(notScussess.Where(x => x.Messages.Contains("Duplicate User")).Count() == 2, "duplicate user count should match");
            Assert.True(notScussess.Where(x => x.Messages.Contains("Invalid User District Name !")).Count() == 1, "invalid district count should match");
            Assert.True(result.Where(x => x.Success).Count() == 0, "Nobody is valid");

            #endregion
        }
        [Theory]
        [InlineData("Maharashtra","Pune", "SC Siddapura", "SubCentre", "mhsiddapurapnsc")]
        [InlineData("PUNJAB", "FAZILKA", "HSC ABOHAR1", "SubCentre", "pbabohar1fazsc")]
        public void GetUserName_Should_Give_Proper_UserName(string state,string district,string hfname,string type,string expected)
        {
            var repoMock = new Mock<IMemberBulkInsertRepository>();
            var excelMock = new Mock<IExcelFileUtility<MemberBulkImportVM>>();
            var states = new List<StateDistrictCity>
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
                },
                 new StateDistrictCity()
                 {
                    StateId =12,
                    StateName ="PUNJAB",
                    DistrictId = 12,
                    DistrictName ="FAZILKA",
                    DistrictShortCode ="FAZ",
                    CityId = 2,
                    CityName ="FAZILKA"
                }

            };
            var service = new MemberBulkDataImportService(excelMock.Object, repoMock.Object);


            var result = service.GetUsersName(states, state,district,hfname,type);

            Assert.Equal(expected, result);
        }
    }
}
