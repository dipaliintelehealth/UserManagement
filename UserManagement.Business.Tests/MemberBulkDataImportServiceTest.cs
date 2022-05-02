using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UserManagement.Business.Services;
using UserManagement.Contract.Repository;
using UserManagement.Contract.Utility;
using UserManagement.Contract.Validator;
using UserManagement.Domain;
using UserManagement.Domain.ViewModel;
using UserManagement.Models;
using Xunit;

namespace UserManagement.Business.Tests
{
    public class MemberBulkDataImportServiceTest
    {
       
        [Theory]
        [InlineData("Hub Vantmuri MCH", "vantmurimch")]
        [InlineData("HUB Vantmuri MCH", "vantmurimch")]
        [InlineData("SPOKE Vantmuri MCH", "vantmurimch")]
        [InlineData("SPOKE Vantmuri MCH", "vantmurimch")]
        [InlineData("SPOKE Vantmuri MCH", "vantmurimch")]
        [InlineData("SPOKE Vantmuri MCH", "vantmurimch")]
        [InlineData("SPOKE Vantmuri MCH", "vantmurimch")]
        [InlineData(" SPOKE Vantmuri MCH ", "vantmurimch")]
        [InlineData("Vantmuri MCH", "vantmurimch")]
        public void GetHFNameForLogin_Should_Give_ProperName(string hfName, string expected)
        {
            var result = MemberBulkDataImportService.GetHFNameForLogin(hfName);
            Assert.Equal(expected, result);
        }
        [Theory]
        [InlineData("Maharashtra", "Pune", "SPOKE Siddapura", "SPOKE", "mhsiddapurapns")]
        [InlineData("Maharashtra", "Pune", "SPOKE Siddapura", " SPOKE", "mhsiddapurapns")]
        [InlineData("Maharashtra", "Pune", "SPOKE Siddapura", "SPOKE ", "mhsiddapurapns")]
        [InlineData("Maharashtra", "Pune", "SPOKE Siddapura", " SPOKE ", "mhsiddapurapns")]
        [InlineData("Maharashtra", "Pune", " SPOKE Siddapura", " SPOKE ", "mhsiddapurapns")]
        [InlineData("Maharashtra", "Pune", "SPOKE Siddapura ", " SPOKE ", "mhsiddapurapns")]
        [InlineData("Maharashtra", "Pune", " SPOKE Siddapura ", " SPOKE ", "mhsiddapurapns")]
        [InlineData("Haryana", "Palwal ", "SPOKE Rampur Khor", "SPOKE", "hrrampurkhorpwls")]
        [InlineData("Haryana", "Palwal", "SPOKE Rampur Khor", "SPOKE", "hrrampurkhorpwls")]
        [InlineData("Haryana ", "Palwal", "SPOKE Rampur Khor", "SPOKE", "hrrampurkhorpwls")]
        [InlineData("Haryana ", "Palwal ", "SPOKE Rampur Khor", "SPOKE", "hrrampurkhorpwls")]
        [InlineData(" Haryana ", " Palwal ", "SPOKE Rampur Khor", "SPOKE", "hrrampurkhorpwls")]
        [InlineData("PUNJAB", "FAZILKA", "SPOKE ABOHAR1", "SPOKE", "pbabohar1fazs")]
        [InlineData("", "FAZILKA", "SPOKE ABOHAR1", "SPOKE", "abohar1fazs")]
        [InlineData("", "", "SPOKE ABOHAR1", "SPOKE", "abohar1s")]
        [InlineData("", "", "", "SPOKE", "s")]
        [InlineData("", "", "", "", "")]

        public void GetUserName_Should_Give_Proper_UserName(string state, string district, string hfname, string type, string expected)
        {
            var repoMock = new Mock<IMemberBulkInsertRepository>();
            var excelMock = new Mock<IExcelFileUtility<MemberBulkImportVM>>();
            var repoValidator = new Mock<IBulkInsertValidator<MemberBulkImportVM>>();
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
            var service = new MemberBulkDataImportService(excelMock.Object, repoMock.Object,repoValidator.Object);


            var result = service.GetUsersName(states, state, district, hfname, type);

            Assert.Equal(expected, result);
        }

        [Fact]
        public async System.Threading.Tasks.Task CreateUserName_When_One_User_Exists_inDB_Should_Create_UserName_With_IncrementAsync()
        {
            var validatedModels = new List<MemberBulkValid>()
            {
                
                {
                    new MemberBulkValid()
                    {
                        UserName="pbaboharfazs",
                        HFState="PUNJAB",
                        HFDistrict="FAZILKA",
                        HFShortName="SPOKE ABOHAR",
                        HFType="SPOKE"

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
                "pbaboharfazs",
                "mhsiddapurapns"
            };
            var repoMock = new Mock<IMemberBulkInsertRepository>();
            var excelMock = new Mock<IExcelFileUtility<MemberBulkImportVM>>();
            var repoValidator = new Mock<IBulkInsertValidator<MemberBulkImportVM>>();
            var service = new MemberBulkDataImportService(excelMock.Object, repoMock.Object,repoValidator.Object);

            var result =  await service.CreateUserName(validatedModels, users, states);

            Assert.NotNull(result);
            Assert.Contains(result, x => x.Value.UserName == "pbabohar1fazs");

        }
        [Fact]
        public async System.Threading.Tasks.Task CreateUserName_When_Multiple_User_Exists_inDB_And_MultipleUsers_Are_In_Excel_Then_Should_Create_UserName_With_IncrementAsync()
        {
            var validatedModels = new List<MemberBulkValid>()
            {
                
                {
                    new MemberBulkValid()
                    {
                        UserName="pbaboharfazs",
                        HFState="PUNJAB",
                        HFDistrict="FAZILKA",
                        HFShortName="SPOKE ABOHAR",
                        HFType="SPOKE"

                    }
                },
                 
                {
                    new MemberBulkValid()
                    {
                        UserName="pbaboharfazs",
                        HFState="PUNJAB",
                        HFDistrict="FAZILKA",
                        HFShortName="SPOKE ABOHAR",
                        HFType="SPOKE"

                    }
                },
                  
                {
                    new MemberBulkValid()
                    {
                        UserName="pbaboharfazs",
                        HFState="PUNJAB",
                        HFDistrict="FAZILKA",
                        HFShortName="ABOHAR",
                        HFType="SPOKE"

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
                "pbaboharfazs",
                "pbaboharfazs",
                "pbaboharfazs",
                "pbabohar3fazs",
                "pbabohar20fazs",
                "mhsiddapurapns"
            };
            var repoMock = new Mock<IMemberBulkInsertRepository>();
            var excelMock = new Mock<IExcelFileUtility<MemberBulkImportVM>>();
            var repoValidator = new Mock<IBulkInsertValidator<MemberBulkImportVM>>();
            var service = new MemberBulkDataImportService(excelMock.Object, repoMock.Object,repoValidator.Object);

            var result = await service.CreateUserName(validatedModels, users, states);

            Assert.NotNull(result);
            Assert.Contains(result, x => x.Value.UserName == "pbabohar21fazs");
            Assert.Contains(result, x => x.Value.UserName == "pbabohar22fazs");
            Assert.Contains(result, x => x.Value.UserName == "pbabohar23fazs");
           
        }
        [Fact]
        public async System.Threading.Tasks.Task CreateUserName_When_One_User_Exists_inDB_And_MultipleUsers_Are_In_Excel_Then_Should_Create_UserName_With_IncrementAsync()
        {
            var validatedModels = new List<MemberBulkValid>()
            {
                
                
                    new MemberBulkValid()
                    {
                        UserName="pbaboharfazs",
                        HFState="PUNJAB",
                        HFDistrict="FAZILKA",
                        HFShortName="SPOKE ABOHAR",
                        HFType="SPOKE"

                    
                },
                 
                
                    new MemberBulkValid()
                    {
                        UserName="pbaboharfazs",
                        HFState="PUNJAB",
                        HFDistrict="FAZILKA",
                        HFShortName="SPOKE ABOHAR",
                        HFType="SPOKE"


                },
                  
                
                    new MemberBulkValid()
                    {
                        UserName="pbaboharfazs",
                        HFState="PUNJAB",
                        HFDistrict="FAZILKA",
                        HFShortName="SPOKE ABOHAR",
                        HFType="SPOKE"


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
                "pbaboharfazs",
                "mhsiddapurapns"
            };
            var repoMock = new Mock<IMemberBulkInsertRepository>();
            var excelMock = new Mock<IExcelFileUtility<MemberBulkImportVM>>();
            var repoValidator = new Mock<IBulkInsertValidator<MemberBulkImportVM>>();
            var service = new MemberBulkDataImportService(excelMock.Object, repoMock.Object,repoValidator.Object);

            var result = await service.CreateUserName(validatedModels, users, states);

            Assert.NotNull(result);
            Assert.Contains(result, x => x.Value.UserName == "pbabohar1fazs");
            Assert.Contains(result, x => x.Value.UserName == "pbabohar2fazs");
            Assert.Contains(result, x => x.Value.UserName == "pbabohar3fazs");

        }
        [Fact]
        public async System.Threading.Tasks.Task CreateUserName_When_User_Not_Exists_inDB_Should_Create_UserName_Without_IncrementAsync()
        {
            var validatedModels = new List<MemberBulkValid>()
            {
                
                
                    new MemberBulkValid()
                    {
                        UserName="pbaboharfazs",
                        HFState="PUNJAB",
                        HFDistrict="FAZILKA",
                        HFName="SPOKE ABOHAR",
                        HFType="SPOKE"

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
                "pbaboharfazh",
                "pbaboharfazsch",
                "mhaboharfazs",
                "pbrameshfazs",
                "pbkapilfazs",
                "mhsiddapurapns"
            };
            var repoMock = new Mock<IMemberBulkInsertRepository>();
            var excelMock = new Mock<IExcelFileUtility<MemberBulkImportVM>>();
            var repoValidator = new Mock<IBulkInsertValidator<MemberBulkImportVM>>();
            var service = new MemberBulkDataImportService(excelMock.Object, repoMock.Object,repoValidator.Object);

            var result = await service.CreateUserName(validatedModels, users, states);

            Assert.NotNull(result);
            Assert.Contains(result, x => x.Value.UserName == "pbaboharfazs");

        }
        [Fact]
        public async System.Threading.Tasks.Task CreateUserName_When_One_User_Not_Exists_inDB_And_duplicateUsers_Are_In_Excel_Then_Should_Create_UserName_With_IncrementAsync()
        {
            var validatedModels = new List<MemberBulkValid>()
            {
                
               
                    new MemberBulkValid()
                    {
                        UserName="pbaboharfazs",
                        HFState="PUNJAB",
                        HFDistrict="FAZILKA",
                        HFShortName="SPOKE ABOHAR",
                        HFType="SPOKE"

                    },
                 
               
                    new MemberBulkValid()
                    {
                        UserName="pbaboharfazs",
                        HFState="PUNJAB",
                        HFDistrict="FAZILKA",
                        HFShortName="SPOKE ABOHAR",
                        HFType="SPOKE"


                },
                  
               
                    new MemberBulkValid()
                    {
                        UserName="pbaboharfazs",
                        HFState="PUNJAB",
                        HFDistrict="FAZILKA",
                        HFName="SPOKE ABOHAR",
                        HFType="SPOKE"

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
               "pbaboharfazh",
                "pbaboharfazsch",
                "mhaboharfazs",
                "pbrameshfazs",
                "pbkapilfazs",
                "mhsiddapurapns"
            };
            var repoMock = new Mock<IMemberBulkInsertRepository>();
            var excelMock = new Mock<IExcelFileUtility<MemberBulkImportVM>>();
            var repoValidator = new Mock<IBulkInsertValidator<MemberBulkImportVM>>();
            var service = new MemberBulkDataImportService(excelMock.Object, repoMock.Object,repoValidator.Object);

            var result = await service.CreateUserName(validatedModels, users, states);

            Assert.NotNull(result);
            Assert.Contains(result, x => x.Value.UserName == "pbaboharfazs");
            Assert.Contains(result, x => x.Value.UserName == "pbabohar1fazs");
            Assert.Contains(result, x => x.Value.UserName == "pbabohar2fazs");

        }
        [Fact]
        public async System.Threading.Tasks.Task CreateUserName_When_Multiple_User_Exists_inDB_Should_Create_UserName_With_IncrementAsync()
        {
            var validatedModels = new List<MemberBulkValid>()
            {
                
               
                    new MemberBulkValid()
                    {
                        UserName="pbaboharfazs",
                        HFState="PUNJAB",
                        HFDistrict="FAZILKA",
                        HFShortName="SPOKE ABOHAR",
                        HFType="SPOKE"

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
                "pbaboharfazs",
                "pbabohar1fazs",
                "pbabohar2fazs",
                "pbabohar3fazs",
                "pbabohar5fazs",
                "mhsiddapurapns"
            };
            var repoMock = new Mock<IMemberBulkInsertRepository>();
            var excelMock = new Mock<IExcelFileUtility<MemberBulkImportVM>>();
            var repoValidator = new Mock<IBulkInsertValidator<MemberBulkImportVM>>();
            var service = new MemberBulkDataImportService(excelMock.Object, repoMock.Object,repoValidator.Object);

            var result = await service.CreateUserName(validatedModels, users, states);

            Assert.NotNull(result);
            Assert.Contains(result, x => x.Value.UserName == "pbabohar6fazs");

        }

      
    }
}
