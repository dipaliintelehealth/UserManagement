using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UserManagement.Business.Services;
using UserManagement.Contract.Repository;
using UserManagement.Contract.Utility;
using UserManagement.Domain;
using UserManagement.Domain.ViewModel;
using Xunit;

namespace UserManagement.Business.Tests
{
    public class MemberBulkDataImportServiceTest
    {
       
        [Theory]
        [InlineData("Hub Vantmuri MCH", "vantmurimch")]
        [InlineData("HUB Vantmuri MCH", "vantmurimch")]
        [InlineData("Uphc Vantmuri MCH", "vantmurimch")]
        [InlineData("hsc Vantmuri MCH", "vantmurimch")]
        [InlineData("Phc Vantmuri MCH", "vantmurimch")]
        [InlineData("PHC Vantmuri MCH", "vantmurimch")]
        [InlineData("Sc Vantmuri MCH", "vantmurimch")]
        [InlineData(" Sc Vantmuri MCH ", "vantmurimch")]
        [InlineData("Vantmuri MCH", "vantmurimch")]
        public void GetHFNameForLogin_Should_Give_ProperName(string hfName, string expected)
        {
            var result = MemberBulkDataImportService.GetHFNameForLogin(hfName);
            Assert.Equal(expected, result);
        }
        [Theory]
        [InlineData("Maharashtra", "Pune", "SC Siddapura", "SubCentre", "mhsiddapurapnsc")]
        [InlineData("Maharashtra", "Pune", "SC Siddapura", " SubCentre", "mhsiddapurapnsc")]
        [InlineData("Maharashtra", "Pune", "SC Siddapura", "SubCentre ", "mhsiddapurapnsc")]
        [InlineData("Maharashtra", "Pune", "SC Siddapura", " SubCentre ", "mhsiddapurapnsc")]
        [InlineData("Maharashtra", "Pune", " SC Siddapura", " SubCentre ", "mhsiddapurapnsc")]
        [InlineData("Maharashtra", "Pune", "SC Siddapura ", " SubCentre ", "mhsiddapurapnsc")]
        [InlineData("Maharashtra", "Pune", " SC Siddapura ", " SubCentre ", "mhsiddapurapnsc")]
        [InlineData("Haryana", "Palwal ", "SC Rampur Khor", "SC", "hrrampurkhorpwlsc")]
        [InlineData("Haryana", "Palwal", "SC Rampur Khor", "SC", "hrrampurkhorpwlsc")]
        [InlineData("Haryana ", "Palwal", "SC Rampur Khor", "SC", "hrrampurkhorpwlsc")]
        [InlineData("Haryana ", "Palwal ", "SC Rampur Khor", "SC", "hrrampurkhorpwlsc")]
        [InlineData(" Haryana ", " Palwal ", "SC Rampur Khor", "SC", "hrrampurkhorpwlsc")]
        [InlineData("PUNJAB", "FAZILKA", "HSC ABOHAR1", "SubCentre", "pbabohar1fazsc")]
        [InlineData("", "FAZILKA", "HSC ABOHAR1", "SubCentre", "abohar1fazsc")]
        [InlineData("", "", "HSC ABOHAR1", "SubCentre", "abohar1sc")]
        [InlineData("", "", "", "SubCentre", "sc")]
        [InlineData("", "", "", "", "")]

        public void GetUserName_Should_Give_Proper_UserName(string state, string district, string hfname, string type, string expected)
        {
            var repoMock = new Mock<IMemberBulkInsertRepository>();
            var excelMock = new Mock<IExcelFileUtility<MemberBulkImportVM>>();
            var states = new List<StateDistrictCity>
            {
                 new StateDistrictCity()
                {
                    StateId =1234,
                    StateName ="Haryana",
                    DistrictId = 1234,
                    DistrictName ="Palwal",
                    DistrictShortCode ="PWL",
                    CityId = 1,
                    CityName ="Palwal"
                },
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


            var result = service.GetUsersName(states, state, district, hfname, type);

            Assert.Equal(expected, result);
        }

        [Fact]
        public async System.Threading.Tasks.Task CreateUserName_When_One_User_Exists_inDB_Should_Create_UserName_With_IncrementAsync()
        {
            var validatedModels = new List<MemberBulkImportVM>()
            {
                
                {
                    new MemberBulkImportVM()
                    {
                        UserName="pbaboharfazsc",
                        UserState="PUNJAB",
                        UserDistrict="FAZILKA",
                        HFName="HSC ABOHAR",
                        HFType="SubCentre"

                    }
                }
            };
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
            var users = new List<string>()
            {
                "pbaboharfazsc",
                "mhsiddapurapnsc"
            };
            var repoMock = new Mock<IMemberBulkInsertRepository>();
            var excelMock = new Mock<IExcelFileUtility<MemberBulkImportVM>>();
            var service = new MemberBulkDataImportService(excelMock.Object, repoMock.Object);

            var result =  await service.CreateUserName(validatedModels, users, states);

            Assert.NotNull(result.Value);
            Assert.Contains(result.Value, x => x.UserName == "pbabohar1fazsc");

        }
        [Fact]
        public async System.Threading.Tasks.Task CreateUserName_When_Multiple_User_Exists_inDB_And_MultipleUsers_Are_In_Excel_Then_Should_Create_UserName_With_IncrementAsync()
        {
            var validatedModels = new List<MemberBulkImportVM>()
            {
                
                {
                    new MemberBulkImportVM()
                    {
                        UserName="pbaboharfazsc",
                        UserState="PUNJAB",
                        UserDistrict="FAZILKA",
                        HFName="HSC ABOHAR",
                        HFType="SubCentre"

                    }
                },
                 
                {
                    new MemberBulkImportVM()
                    {
                        UserName="pbaboharfazsc",
                        UserState="PUNJAB",
                        UserDistrict="FAZILKA",
                        HFName="HSC ABOHAR",
                        HFType="SubCentre"

                    }
                },
                  
                {
                    new MemberBulkImportVM()
                    {
                        UserName="pbaboharfazsc",
                        UserState="PUNJAB",
                        UserDistrict="FAZILKA",
                        HFName="HSC ABOHAR",
                        HFType="SubCentre"

                    }
                }
            };
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
            var users = new List<string>()
            {
                "pbaboharfazsc",
                "pbabohar1fazsc",
                "pbabohar2fazsc",
                "pbabohar3fazsc",
                "pbabohar5fazsc",
                "mhsiddapurapnsc"
            };
            var repoMock = new Mock<IMemberBulkInsertRepository>();
            var excelMock = new Mock<IExcelFileUtility<MemberBulkImportVM>>();
            var service = new MemberBulkDataImportService(excelMock.Object, repoMock.Object);

            var result = await service.CreateUserName(validatedModels, users, states);

            Assert.NotNull(result.Value);
            Assert.Contains(result.Value, x => x.UserName == "pbabohar6fazsc");
            Assert.Contains(result.Value, x => x.UserName == "pbabohar7fazsc");
            Assert.Contains(result.Value, x => x.UserName == "pbabohar8fazsc");
           
        }
        [Fact]
        public async System.Threading.Tasks.Task CreateUserName_When_One_User_Exists_inDB_And_MultipleUsers_Are_In_Excel_Then_Should_Create_UserName_With_IncrementAsync()
        {
            var validatedModels = new List<MemberBulkImportVM>()
            {
                
                
                    new MemberBulkImportVM()
                    {
                        UserName="pbaboharfazsc",
                        UserState="PUNJAB",
                        UserDistrict="FAZILKA",
                        HFName="HSC ABOHAR",
                        HFType="SubCentre"

                    
                },
                 
                
                    new MemberBulkImportVM()
                    {
                        UserName="pbaboharfazsc",
                        UserState="PUNJAB",
                        UserDistrict="FAZILKA",
                        HFName="HSC ABOHAR",
                        HFType="SubCentre"

                    
                },
                  
                
                    new MemberBulkImportVM()
                    {
                        UserName="pbaboharfazsc",
                        UserState="PUNJAB",
                        UserDistrict="FAZILKA",
                        HFName="HSC ABOHAR",
                        HFType="SubCentre"

                    
                }
            };
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
            var users = new List<string>()
            {
                "pbaboharfazsc",
                "mhsiddapurapnsc"
            };
            var repoMock = new Mock<IMemberBulkInsertRepository>();
            var excelMock = new Mock<IExcelFileUtility<MemberBulkImportVM>>();
            var service = new MemberBulkDataImportService(excelMock.Object, repoMock.Object);

            var result = await service.CreateUserName(validatedModels, users, states);

            Assert.NotNull(result.Value);
            Assert.Contains(result.Value, x => x.UserName == "pbabohar1fazsc");
            Assert.Contains(result.Value, x => x.UserName == "pbabohar2fazsc");
            Assert.Contains(result.Value, x => x.UserName == "pbabohar3fazsc");

        }
        [Fact]
        public async System.Threading.Tasks.Task CreateUserName_When_User_Not_Exists_inDB_Should_Create_UserName_Without_IncrementAsync()
        {
            var validatedModels = new List<MemberBulkImportVM>()
            {
                
                
                    new MemberBulkImportVM()
                    {
                        UserName="pbaboharfazsc",
                        UserState="PUNJAB",
                        UserDistrict="FAZILKA",
                        HFName="HSC ABOHAR",
                        HFType="SubCentre"

                    }
                
            };
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
            var users = new List<string>()
            {
                "pbaboharfazhub",
                "pbaboharfazuphc",
                "mhaboharfazsc",
                "pbrameshfazsc",
                "pbkapilfazsc",
                "mhsiddapurapnsc"
            };
            var repoMock = new Mock<IMemberBulkInsertRepository>();
            var excelMock = new Mock<IExcelFileUtility<MemberBulkImportVM>>();
            var service = new MemberBulkDataImportService(excelMock.Object, repoMock.Object);

            var result = await service.CreateUserName(validatedModels, users, states);

            Assert.NotNull(result.Value);
            Assert.Contains(result.Value, x => x.UserName == "pbaboharfazsc");

        }
        [Fact]
        public async System.Threading.Tasks.Task CreateUserName_When_One_User_Not_Exists_inDB_And_duplicateUsers_Are_In_Excel_Then_Should_Create_UserName_With_IncrementAsync()
        {
            var validatedModels = new List<MemberBulkImportVM>()
            {
                
               
                    new MemberBulkImportVM()
                    {
                        UserName="pbaboharfazsc",
                        UserState="PUNJAB",
                        UserDistrict="FAZILKA",
                        HFName="HSC ABOHAR",
                        HFType="SubCentre"

                    },
                 
               
                    new MemberBulkImportVM()
                    {
                        UserName="pbaboharfazsc",
                        UserState="PUNJAB",
                        UserDistrict="FAZILKA",
                        HFName="HSC ABOHAR",
                        HFType="SubCentre"

                   
                },
                  
               
                    new MemberBulkImportVM()
                    {
                        UserName="pbaboharfazsc",
                        UserState="PUNJAB",
                        UserDistrict="FAZILKA",
                        HFName="HSC ABOHAR",
                        HFType="SubCentre"

                    }
                
            };
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
            var users = new List<string>()
            {
               "pbaboharfazhub",
                "pbaboharfazuphc",
                "mhaboharfazsc",
                "pbrameshfazsc",
                "pbkapilfazsc",
                "mhsiddapurapnsc"
            };
            var repoMock = new Mock<IMemberBulkInsertRepository>();
            var excelMock = new Mock<IExcelFileUtility<MemberBulkImportVM>>();
            var service = new MemberBulkDataImportService(excelMock.Object, repoMock.Object);

            var result = await service.CreateUserName(validatedModels, users, states);

            Assert.NotNull(result.Value);
            Assert.Contains(result.Value, x => x.UserName == "pbaboharfazsc");
            Assert.Contains(result.Value, x => x.UserName == "pbabohar1fazsc");
            Assert.Contains(result.Value, x => x.UserName == "pbabohar2fazsc");

        }
        [Fact]
        public async System.Threading.Tasks.Task CreateUserName_When_Multiple_User_Exists_inDB_Should_Create_UserName_With_IncrementAsync()
        {
            var validatedModels = new List<MemberBulkImportVM>()
            {
                
               
                    new MemberBulkImportVM()
                    {
                        UserName="pbaboharfazsc",
                        UserState="PUNJAB",
                        UserDistrict="FAZILKA",
                        HFName="HSC ABOHAR",
                        HFType="SubCentre"

                    }
               
            };
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
            var users = new List<string>()
            {
                "pbaboharfazsc",
                "pbabohar1fazsc",
                "pbabohar2fazsc",
                "pbabohar3fazsc",
                "pbabohar5fazsc",
                "mhsiddapurapnsc"
            };
            var repoMock = new Mock<IMemberBulkInsertRepository>();
            var excelMock = new Mock<IExcelFileUtility<MemberBulkImportVM>>();
            var service = new MemberBulkDataImportService(excelMock.Object, repoMock.Object);

            var result = await service.CreateUserName(validatedModels, users, states);

            Assert.NotNull(result.Value);
            Assert.Contains(result.Value, x => x.UserName == "pbabohar6fazsc");

        }

      
    }
}
