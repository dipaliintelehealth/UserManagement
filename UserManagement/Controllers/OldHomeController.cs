using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using UserManagement.Models;
using UserManagement.Contract.User;
using OfficeOpenXml;
using UserManagement.Domain.User;
using System.Data.OleDb;
using UserTest.Models;
using Microsoft.Extensions.Configuration;

namespace UserManagement.Controllers
{
    public class OldHomeController : Controller
    {
        private IConfiguration Configuration;
        private readonly IUserRepository _UserRepository;
        [Obsolete]
        private IHostingEnvironment _hostingEnvironment;

        // GET: /<controller>/
        [Obsolete]
        public OldHomeController(IUserRepository UserRepository, IHostingEnvironment hostingEnvironment, IConfiguration _configuration)
        {
            _hostingEnvironment = hostingEnvironment;
            _UserRepository = UserRepository;
            Configuration = _configuration;
        }
        public IActionResult UserTest()
        {
            return View();
        }
        [HttpPost]
        public IActionResult UploadUserFile()
        {
            string result = "";
            string strError = "";
            string strUsersList = "<table id='tblCustomers'  class='table table-hover'><tr><th>HF Name</th><th>User Name</th></tr>";
            string re = "";
            List<HealthFacility> HFlist = new List<HealthFacility>();
            try
            {
                long size = 0;
                var file = Request.Form.Files;
                var filename = ContentDispositionHeaderValue
                                .Parse(file[0].ContentDisposition)
                                .FileName
                                .Trim('"');

             //string FilePath = _hostingEnvironment.WebRootPath + $@"\InputExcel\{ filename}";
             string FilePath = "/var/netcore/wwwroot/InputExcel/{filename}";
                
                size += file[0].Length;
                using (FileStream fs = System.IO.File.Create(FilePath))
                {
                    file[0].CopyTo(fs);
                    fs.Flush();
                }
                DataTable dt11 = GetDataTableFromExcel(FilePath);
                DataTable dtUsers = new DataTable();
                dtUsers.Columns.Add("HF Name");
                dtUsers.Columns.Add("User Name");
                string excelCS = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=Excel 8.0", FilePath);
                DataTable dt = new DataTable();
                dt = dt11;
                //using (OleDbConnection con = new OleDbConnection(excelCS))
                //{
                //    OleDbCommand cmd = new OleDbCommand("select * from [eSanjeevaniAB$]", con);

                //    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                //    da.Fill(dt);

                    if (dt.Rows.Count >= 2)
                    {
                        for (int i = 1; i < dt.Rows.Count; i++)
                        {
                            HealthFacility HFNew = new HealthFacility();
                            HFNew.strHFName = dt.Rows[i][1].ToString();
                            HFNew.strHFNameActual = dt.Rows[i][1].ToString();
                            HFNew.strHFPhone = dt.Rows[i][2].ToString();
                            HFNew.strHFType = dt.Rows[i][3].ToString();
                            HFNew.strNIN = dt.Rows[i][4].ToString();
                            HFNew.strHFEmail = dt.Rows[i][5].ToString();
                            HFNew.strState = dt.Rows[i][6].ToString();
                            HFNew.strDistrict = dt.Rows[i][7].ToString();
                            HFNew.strCity = dt.Rows[i][8].ToString();
                            HFNew.strAddress = dt.Rows[i][9].ToString();
                            HFNew.strPIN = dt.Rows[i][10].ToString();
                            HFNew.strFName = dt.Rows[i][11].ToString();
                            HFNew.strLname = dt.Rows[i][12].ToString();
                            HFNew.strUserMobile = dt.Rows[i][13].ToString();
                            HFNew.strGender = dt.Rows[i][14].ToString();
                            HFNew.strQualification = dt.Rows[i][15].ToString();
                            HFNew.strExperience = dt.Rows[i][16].ToString();
                            HFNew.strDRRegNo = dt.Rows[i][17].ToString();
                            HFNew.strUserEmail = dt.Rows[i][18].ToString();
                            HFNew.strDesgination = dt.Rows[i][19].ToString();
                            HFNew.strDOB = dt.Rows[i][20].ToString();

                            HFNew.UserState = dt.Rows[i][21].ToString();
                            HFNew.UserDist = dt.Rows[i][22].ToString();
                            HFNew.UserCity = dt.Rows[i][23].ToString();
                            HFNew.UserAddress = dt.Rows[i][24].ToString();
                            HFNew.UserPin = dt.Rows[i][25].ToString();
                            HFNew.UserPrefix = dt.Rows[i][26].ToString();

                            HFNew.UserAvilableDay = dt.Rows[i][27].ToString();
                            HFNew.UserAvilableFromTime = dt.Rows[i][28].ToString();
                            HFNew.UserAvilableToTime = dt.Rows[i][29].ToString();
                            HFNew.UserRole = dt.Rows[i][30].ToString();

                            HFNew.AssignedHFType = dt.Rows[i][31].ToString();
                            HFNew.AssignHF = dt.Rows[i][32].ToString();

                        string strHFNameFull = dt.Rows[i][1].ToString() + " " + HFNew.strDistrict; ;

                        HFNew.strHFName = HFNew.strHFName.Replace("HSC", "").Replace("PHC","") + " " + HFNew.strDistrict;
                        if (_UserRepository.CheckDist(HFNew.strDistrict) == "")
                        {

                            if (_UserRepository.CheckState(HFNew.strState) == "")
                            {



                                string ValidateMessage = _UserRepository.ValidateData(HFNew.strHFName, HFNew.strHFEmail, HFNew.strUserMobile, HFNew.strUserEmail, HFNew.strFName, HFNew.strLname);
                                if (ValidateMessage.Trim() == "")
                                {

                                    try
                                    {
                                        string gender = "1";
                                        if (HFNew.strGender == "Female")
                                        {
                                            gender = "2";
                                        }
                                        else if (HFNew.strGender == "Male")
                                        {
                                            gender = "1";
                                        }
                                        else
                                        {
                                            gender = "3";
                                        }

                                        string stateID = _UserRepository.GetStateID(HFNew.strState);

                                        string distID = _UserRepository.GetDistID(HFNew.strDistrict, stateID);
                                        string cityid = _UserRepository.GetCityID(HFNew.strCity, stateID, distID);

                                        string InstituteID = _UserRepository.GetSubCenterID(HFNew.strHFName);

                                        string UserStateID = _UserRepository.GetStateID(HFNew.UserState);
                                        string UserDistID = _UserRepository.GetDistID(HFNew.UserDist, UserStateID);
                                        string UserCityID = _UserRepository.GetCityID(HFNew.UserCity, UserStateID, UserDistID);

                                        string AssignedInstituteID = _UserRepository.GetSubCenterID(HFNew.AssignHF);

                                        string strTypeID = "";
                                        if (HFNew.strHFType.Replace(" ", "").Replace("-", "").ToLower() == "hub")
                                        {
                                            strTypeID = "1";
                                        }
                                        else if (HFNew.strHFType.Replace(" ", "").Replace("-", "").ToLower() == "phc")
                                        {
                                            strTypeID = "2";
                                        }
                                        else
                                        {
                                            strTypeID = "3";

                                        }


                                        if (stateID == "")
                                        {
                                            // result = "Mudassar Khan";
                                            result = result + "<br/>" + (i - 1).ToString() + ". Invalid State !";
                                        }

                                        else if (distID == "")
                                        {
                                            result = result + "<br/>" + (i - 1).ToString() + ".Invalid District !";
                                        }
                                        else if (cityid == "")
                                        {
                                            result = result + "<br/>" + (i - 1).ToString() + ".Invalid City !";
                                        }
                                        else if (UserStateID == "")
                                        {
                                            result = result + "<br/>" + (i - 1).ToString() + ".Invalid User State !";
                                        }
                                        else if (UserDistID == "")
                                        {
                                            result = result + "<br/>" + (i - 1).ToString() + ".Invalid User District !";
                                        }

                                        else if (UserCityID == "")
                                        {
                                            result = result + "<br/>" + (i - 1).ToString() + ".Invalid User City !";
                                        }
                                        //else if (AssignedInstituteID == "")
                                        //{
                                        //    result = result + "<br/>" + (i - 1).ToString() + ".Invalid Assigned HUB or PHC !";
                                        //}
                                        else if (HFNew.UserRole == "")
                                        {
                                            result = result + "<br/>" + (i - 1).ToString() + ".Invalid Role!";
                                        }
                                        else if (HFNew.UserAvilableDay == "")
                                        {
                                            result = result + "<br/>" + (i - 1).ToString() + ".Invalid Day and Time (Availability)!";
                                        }
                                        else if (HFNew.UserAvilableFromTime == "")
                                        {
                                            result = result + "<br/>" + (i - 1).ToString() + ".Invalid Availability From Time !";
                                        }
                                        else if (HFNew.UserAvilableToTime == "")
                                        {
                                            result = result + "<br/>" + (i - 1).ToString() + ".Invalid Availability To Time !";
                                        }
                                        else
                                        {
                                            if (InstituteID == "")
                                            {
                                                InstituteID = _UserRepository.CreateServiceProvider(HFNew.strHFName, HFNew.strAddress, "", HFNew.strNIN, "1", stateID, distID, cityid, HFNew.strPIN, HFNew.strHFPhone, HFNew.strHFEmail, strTypeID);
                                            }


                                            string QualificatioID = _UserRepository.GetQualificationID(HFNew.strQualification);
                                            string UserName = _UserRepository.CreateUsersName(HFNew.strState, HFNew.strDistrict, HFNew.strHFNameActual, HFNew.strHFType).Replace(" ", "");
                                            string MemberId = _UserRepository.CreateMember(HFNew.strFName, "", HFNew.strLname, "1", HFNew.strDOB, "0", HFNew.strUserMobile, HFNew.strUserEmail, "", "1", DateTime.Now.ToString("yyyy-MM-dd"), "1", gender, HFNew.strDRRegNo, HFNew.strAddress, "", UserStateID, UserDistID, UserCityID, "0", QualificatioID, HFNew.UserPin, "", "0", "0", "", "1", "1", "0", "0", "0", "", HFNew.UserRole);
                                            string LoginID = _UserRepository.CreateLogin(UserName, MemberId);
                                            // string MappedNetworkId = MappedNetwork(AssignedInstituteID, InstituteID);
                                            string SlotID = _UserRepository.MemberSlot(MemberId, HFNew.UserAvilableDay, HFNew.UserAvilableToTime, HFNew.UserAvilableFromTime);
                                            //AssignMemberMenu(UserRole, MemberId, InstituteID);
                                            string MemberIstitutionMappingID = _UserRepository.MemberInstitutionMapping(MemberId, InstituteID);
                                            // result  = result  + "<br/>" + (i - 1).ToString() + ".Success !" + UserName.ToLower() + " Login ID:" + LoginID;

                                            // string   AuditTrailID=_UserRepository.AuditLog()


                                            dtUsers.Rows.Add(strHFNameFull, UserName.ToLower());
                                            strUsersList = strUsersList + "<tr><td>" + strHFNameFull + "</td><td>" + UserName.ToLower() + "</td></tr>";
                                            // need to do
                                            //  Session["Udata"] = dtUsers;
                                            // Button2.Visible = true;
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception(ex.Message);
                                    }
                                    finally
                                    {

                                    }
                                }
                                else
                                {
                                    result = result + "<br/>" + (i - 1).ToString() + "." + ValidateMessage + " </br>";
                                }
                            }

                            else
                            {
                                result = result + "<br/>" + (i - 1).ToString() + "." + "Invalid State Name !" + " </br>";

                            }
                        }
                        else
                        {
                            result = result + "<br/>" + (i - 1).ToString() + "." + "Invalid District Name !" + " </br>";

                        }
                        HFlist.Add(HFNew);
                        }
                        string AuditTrailID = _UserRepository.AuditLog("Bulk Institution and Member Added Successfully", "", "1", "11", "14", "", "", "0", "2");
                    }
                //}

                //        //getting data from excel to a data table
                     
                //DataView dvview = new DataView(dt);
                ////checking the columns available or not before starting operation for getting data from the data table
                //if (dt.Columns.Contains("Name") && dt.Columns.Contains("EmailId"))
                //{
                //    User ObjUser = new User();
                //    for (int i = 0; i < dt.Rows.Count; i++)
                //    {
                //        if (dt.Rows[i]["Name"].ToString() != "")
                //        {

                //            ObjUser.Name = dt.Rows[i]["Name"].ToString();
                //            ObjUser.EmailId = dt.Rows[i]["EmailId"].ToString();
                //            ObjUser.Mobile = dt.Rows[i]["Mobile"].ToString();
                //            ObjUser.Password = dt.Rows[i]["Password"].ToString();
                //            re = _UserRepository.INSERTUSER(ObjUser);
                //        }
                //    }
                //}
                //else 
                //{
                //    ViewBag.ErrorMessage = "Invalid Template"; 
                //}
                //if(re=="1")
                //{
                //    result = "Data Upoaded and Inserted Successfuly";
                //}
                //else
                //{
                //    result = "System Facing some issue to insert data";
                //}

                strUsersList = strUsersList + "</table>";
                result = strUsersList + result;
                // result = JsonConvert.SerializeObject(HFlist);
                return Ok(result);
            }
            catch (Exception ex)
            {
                result = "Error Occur!" + ex.Message;
                return Ok(result);
            }

        }
        private static DataTable GetDataTableFromExcel(string path, bool hasHeader = true)
        {
           ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var pck = new OfficeOpenXml.ExcelPackage())
            {
                using (var stream = System.IO.File.OpenRead(path))
                {
                    pck.Load(stream);
                }
                var ws = pck.Workbook.Worksheets.First();
                DataTable tbl = new DataTable();
                foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                {
                    tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                }
                var startRow = hasHeader ? 2 : 1;
                for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    DataRow row = tbl.Rows.Add();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                }
                return tbl;
            }
        }

    }
}
