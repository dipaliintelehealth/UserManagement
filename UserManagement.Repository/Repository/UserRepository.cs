using System;
using Microsoft.Extensions.Logging;
using System.Data;
using Dapper;
using UserManagement.Contract.User;
using UserManagement.Contract.Factory;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UserManagement.Domain;
using System.Threading.Tasks;
using UserManagement.Infrastructure.Extension;

namespace UserManagement.Infrastructure.Repository
{
    public class UserRepository : RepositoryBase, IUserRepository
    {

        //  public MySqlConnection con = new MySqlConnection("Data Source=localhost;Initial Catalog=HWC;Persist Security Info=True;User ID=root;password=Shine@9338;");
        public MySqlConnection con = new MySqlConnection("Data Source=127.0.0.1;Initial Catalog=staggingdblive_jan_aws;Persist Security Info=True;User ID=root;password=password;");
        private readonly ILogger<UserRepository> Logger;
        //   private IConfiguration Configuration;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionFactory">Connection Factory</param>
        /// See also <see cref="UserRepository.Repository.Contract.Factory.IConnectionFactory" />
        public UserRepository(IConnectionFactory connectionFactory, ILogger<UserRepository> logger) : base(connectionFactory)
        {
            // string connString = this.Configuration.GetConnectionString("MyConn");
            Logger = logger;
            Logger.LogInformation("UserRepository initialized");
        }
        //public string INSERTUSER(UserManagement.Domain.User.User Model)
        //{
        //    try
        //    {
        //        var p = new OracleDynamicParameters();
        //        p.Add("P_Action", OracleDbType.Char, ParameterDirection.Input, "A");
        //        p.Add("P_Name", OracleDbType.Varchar2, ParameterDirection.Input, Model.Name);
        //        p.Add("P_EmailId", OracleDbType.Varchar2, ParameterDirection.Input, Model.EmailId);
        //        p.Add("P_Mobile", OracleDbType.Varchar2, ParameterDirection.Input, Model.Mobile);
        //        p.Add("P_Password", OracleDbType.Varchar2, ParameterDirection.Input, Model.Password);
        //        p.Add("P_Msg", OracleDbType.RefCursor, direction: ParameterDirection.Output);
        //        var query = "USP_usermanagement";
        //        Connection.Query(query, p, commandType: CommandType.StoredProcedure);
        //        return "1";
        //    }
        //    catch (Exception ex)
        //    {
        //        return "0";
        //    }
        //}
        public string INSERTUSER(UserManagement.Domain.User.User Model)
        {
            int res = 0;
            try
            {
                var p = new SqlDynamicParameters();
                p.Add("@P_Action", SqlDbType.Char, ParameterDirection.Input, "A");
                p.Add("@P_Name", SqlDbType.VarChar, ParameterDirection.Input, Model.Name);
                p.Add("@P_EmailId", SqlDbType.VarChar, ParameterDirection.Input, Model.EmailId);
                p.Add("@P_Mobile", SqlDbType.VarChar, ParameterDirection.Input, Model.Mobile);
                p.Add("@P_Password", SqlDbType.VarChar, ParameterDirection.Input, Model.Password);
                p.Add("@P_Msg", SqlDbType.Int, ParameterDirection.Output);
                var query = "USP_usermanagement";
                Connection.Query(query, p, commandType: CommandType.StoredProcedure);
                //var r = p.GetOutParamValue("@P_Msg");
                res = 1;
                return res.ToString();
            }
            catch (Exception ex)
            {
                return res.ToString();
            }
        }
        public string GetCity(string DistId)
        {
            return "BBSR";
        }
        public string GetSubCenterID(string strHFName)
        {
            string strResult = "";
            try
            {
                string stSQL = "SELECT InstitutionId FROM md_institution  where  Name='" + strHFName + "';";

                MySqlCommand cmd = new MySqlCommand(stSQL, con);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                MySqlDataAdapter MySqlDataAdapter = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                da.Fill(dt);


                if (dt.Rows.Count > 0)
                {
                    strResult = dt.Rows[0][0].ToString();
                }
            }
            catch
            {

            }
            //else
            //{
            //    string strQ = "";
            //    strResult = ExeQueryOutput(strQ);
            //}
            return strResult;
        }
        public string GetQualificationID(string strQfName)
        {
            string strResult = "";
            try
            {

                string stSQL = "SELECT QualificationId FROM md_qualification  where  QualificationName='" + strQfName + "';";

                MySqlCommand cmd = new MySqlCommand(stSQL, con);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                MySqlDataAdapter MySqlDataAdapter = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                da.Fill(dt);


                if (dt.Rows.Count > 0)
                {
                    strResult = dt.Rows[0][0].ToString();
                }
                else
                {
                    strResult = "4"; // For Others
                }
            }
            catch
            {

            }
            //else
            //{
            //    string strQ = "";
            //    strResult = ExeQueryOutput(strQ);
            //}
            return strResult;
        }
        public string GetStateID(string strStateName)
        {
            string strResult = "";
            try
            {
                string stSQL = "SELECT StateID FROM md_state where  StateName='" + strStateName + "';";

                MySqlCommand cmd = new MySqlCommand(stSQL, con);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                MySqlDataAdapter MySqlDataAdapter = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                da.Fill(dt);


                if (dt.Rows.Count > 0)
                {
                    strResult = dt.Rows[0][0].ToString();
                }
            }
            catch
            { }
            return strResult;
        }
        public string GetDistID(string strDistName, string stateID)
        {
            string strResult = "";
            try
            {
                string stSQL = "SELECT DistrictId FROM md_district where StateID=" + stateID + " and  DistrictName='" + strDistName + "';";
                MySqlCommand cmd = new MySqlCommand(stSQL, con);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                MySqlDataAdapter MySqlDataAdapter = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                da.Fill(dt);


                if (dt.Rows.Count > 0)
                {
                    strResult = dt.Rows[0][0].ToString();
                }
            }
            catch
            {

            }
            return strResult;
        }
        public string GetCityID(string strCityName, string stateID, string districtID)
        {
            string strResult = "";

            try
            {
                string stSQL = "SELECT CityId FROM md_city where StateId=" + stateID + " and DistrictId=" + districtID + " and CityName='" + strCityName + "';";
                MySqlCommand cmd = new MySqlCommand(stSQL, con);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                MySqlDataAdapter MySqlDataAdapter = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                da.Fill(dt);


                if (dt.Rows.Count > 0)
                {
                    strResult = dt.Rows[0][0].ToString();
                }
            }
            catch
            {
            }
            return strResult;
        }
        public string CreateUsersName(string StateName, string DistrictName, string HFName, string Type)
        {
            //State Code (2 alphabet)______Name of HF_____District Code (3 alphabet)______Type of HF (hub/phc/uphc/sc)

            string strTypeShortCode = "";
            string StateShortCode = StateName.Substring(0, 2);

            if (StateName.Replace(' ', '-').ToUpper() == "Andhra Pradesh".ToUpper())
            {
                StateShortCode = "AP";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "Arunachal Pradesh".ToUpper())
            {
                StateShortCode = "AR";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "Assam".ToUpper())
            {
                StateShortCode = "AS";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "BIHAR")
            {
                StateShortCode = "BR";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "Chhattisgarh".ToUpper())
            {
                StateShortCode = "CG";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "DELHI")
            {
                StateShortCode = "DL";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "GOA")
            {
                StateShortCode = "GA";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "GUJARAT")
            {
                StateShortCode = "GJ";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "HARYANA")
            {
                StateShortCode = "HR";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "HARIYANA")
            {
                StateShortCode = "HR";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "Himachal Pradesh".ToUpper())
            {
                StateShortCode = "HP";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "Jammu And Kashmir".ToUpper())
            {
                StateShortCode = "JK";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "Jharkhand".ToUpper())
            {
                StateShortCode = "JS";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "Karnataka".ToUpper())
            {
                StateShortCode = "KA";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "KERALA")
            {
                StateShortCode = "KL";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "Madhya Pradesh".ToUpper())
            {
                StateShortCode = "MP";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "Maharashtra".ToUpper())
            {
                StateShortCode = "MH";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "MANIPUR")
            {
                StateShortCode = "MN";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "Meghalaya".ToUpper())
            {
                StateShortCode = "ML";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "MIZORAM")
            {
                StateShortCode = "MZ";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "Nagaland".ToUpper())
            {
                StateShortCode = "NL";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "ORISSA")
            {
                StateShortCode = "OR";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "ODISHA")
            {
                StateShortCode = "OD";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "PUNJAB")
            {
                StateShortCode = "PB";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "RAJASTHAN")
            {
                StateShortCode = "RJ";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "SIKKIM")
            {
                StateShortCode = "SK";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "Tamil Nadu".ToUpper())
            {
                StateShortCode = "TN";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "TRIPURA")
            {
                StateShortCode = "TR";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "UTTARAKHAND")
            {
                StateShortCode = "UK";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "UTTAR PRADESH")
            {
                StateShortCode = "UP";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "WEST BENGAL")
            {
                StateShortCode = "WB";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "Andaman And Nicobar Islands")
            {
                StateShortCode = "AN";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "Chandigarh".ToUpper())
            {
                StateShortCode = "CH";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "DADRA AND NAGAR HAVELI")
            {
                StateShortCode = "DN";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "Dadra And Nagar Haveli and Daman And Diu".ToUpper())
            {
                StateShortCode = "DD";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "Daman And Diu Merged".ToUpper())
            {
                StateShortCode = "DD";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "Lakshadweep".ToUpper())
            {
                StateShortCode = "LD";
            }
            else if (StateName.Replace(' ', '-').ToUpper() == "Puducherry".ToUpper())
            {
                StateShortCode = "PY";
            }
            else
            {
                StateShortCode = StateName.Substring(0, 2);
            }

            string strResult = "";
            try
            {
                string stSQL = "SELECT ShortDistCode FROM lgd_dist where  DistrictNameE='" + DistrictName + "';";

                MySqlCommand cmd = new MySqlCommand(stSQL, con);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                MySqlDataAdapter MySqlDataAdapter = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                da.Fill(dt);


                if (dt.Rows.Count > 0)
                {
                    strResult = dt.Rows[0][0].ToString();
                }
            }
            catch
            { }

            string DistShortCode = "";
            if (strResult.Length >= 2)
            {
                DistShortCode = strResult;
            }
            else
            {
                if (DistrictName.Replace(" ", "").Length > 9)
                {
                    DistShortCode = DistrictName.Replace(" ", "").Substring(0, 8);
                }
                else if (DistrictName.Replace(" ", "").Length > 8)
                {
                    DistShortCode = DistrictName.Replace(" ", "").Substring(0, 7);
                }
                else if (DistrictName.Replace(" ", "").Length > 7)
                {
                    DistShortCode = DistrictName.Replace(" ", "").Substring(0, 6);
                }
                if (DistrictName.Replace(" ", "").Length > 6)
                {
                    DistShortCode = DistrictName.Replace(" ", "").Substring(0, 5);
                }
                else if (DistrictName.Replace(" ", "").Length > 5)
                {
                    DistShortCode = DistrictName.Replace(" ", "").Substring(0, 4);
                }
                else if (DistrictName.Replace(" ", "").Length > 4)
                {
                    DistShortCode = DistrictName.Replace(" ", "").Substring(0, 3);
                }
                else
                {
                    DistShortCode = DistrictName.Substring(0, 2);
                }
            }
            string strHFname = HFName.Replace("HSC ", "").Replace("PHC", "");

            string stRes = "";

            if (Type == "SubCentre")
            {
                strTypeShortCode = "SC";
            }
            else
            {
                strTypeShortCode = Type;
            }
            stRes = StateShortCode + strHFname + DistShortCode + strTypeShortCode;
            //strHFname
            return stRes;
        }

        public string ExeQueryOutput(string sql)
        {
            string strRes = "0";

            using (MySqlCommand cmd = new MySqlCommand(sql, con))
            {

                con.Open();

                strRes = cmd.ExecuteScalar().ToString();

                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();


            }
            return strRes;
        }
        public bool ValidateEmail(string email)
        {
            bool Res = true;
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(email);
            if (match.Success)
                Res = true;
            else
                Res = false;
            return Res;
        }

        public bool IsWholeNumber(string strNumber)
        {
            Regex objNotWholePattern = new Regex("[^0-9]");
            return !objNotWholePattern.IsMatch(strNumber);
        }

        public bool ValidateMobile(string strMobile)
        {
            bool res = true;
            if (strMobile.Trim().Length < 10)
            {
                res = false;
            }
            else
            {
                if (IsWholeNumber(strMobile.Trim()) == false)
                {
                    res = false;
                }
            }
            return res;
        }
        public bool CheckDuplicate(string strUserEmail, string strUserMobile)
        {
            bool res = false;

            MySqlCommand cmd = new MySqlCommand("Select Email from md_members where Email='" + strUserEmail + "' or Mobile='" + strUserMobile + "'", con);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            da.Fill(ds);
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                res = true;
            }
            return res;
        }

        public string ValidateData(string strHFName, string strHFEmail, string strMobile, string strUserEmail, string strUserFName, string strUserLName)
        {
            string strMessage = "";

            if (strHFName.Trim() == "")
            {
                strMessage = "HFName can not be blank!";
            }
            if (strUserFName.Trim() == "")
            {
                strMessage = strMessage + " User First Name can not be blank!";
            }
            if (strUserLName.Trim() == "")
            {
                strMessage = strMessage + " User Last Name can not be blank!";
            }
            if (strMobile.Trim() == "")
            {
                strMessage = strMessage + " User Mobile can not be blank!";
            }
            if (strUserEmail.Trim() == "")
            {
                strMessage = strMessage + " User Email can not be blank!";
            }

            if (ValidateEmail(strUserEmail.Trim()) == false)
            {
                strMessage = strMessage + " Invalid User Email !";
            }
            if (strHFEmail.Trim().Length > 4)
            {
                if (ValidateEmail(strHFEmail.Trim()) == false)
                {
                    strMessage = strMessage + " Invalid HF Email !";
                }
            }

            if (ValidateMobile(strMobile.Trim()) == false)
            {
                strMessage = strMessage + " Invalid User Mobile !";
            }

            if (CheckDuplicate(strUserEmail, strMobile) == true)
            {
                strMessage = strMessage + strHFName + "- Duplicate  User Found !";
            }





            return strMessage;
        }


        public DataTable GetMenuMappingID(string RoleID)
        {


            string strsql = "Select distinct MenuMappingId from  mp_member_menu where RoleId in(" + RoleID + ")";

            MySqlCommand cmd = new MySqlCommand(strsql, con);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            da.Fill(ds);
            dt = ds.Tables[0];


            return dt;

        }

        public string MemberInstitutionMapping(string memberid, string institutionid)
        {
            string strRes = "";

            string strsql = "INSERT INTO  `mp_member_institution`";
            strsql = strsql + "(";
            strsql = strsql + "`MemberId`,";
            strsql = strsql + "`InstitutionId`,";
            strsql = strsql + "`IsActive`,";
            strsql = strsql + "`SourceId`)";

            strsql = strsql + "VALUES";
            strsql = strsql + "( ";
            strsql = strsql + "" + memberid + ",";
            strsql = strsql + "" + institutionid + ",";

            strsql = strsql + "" + "1" + ",";

            strsql = strsql + "" + "99" + ");SELECT LAST_INSERT_ID();";


            MySqlCommand cmd = new MySqlCommand(strsql, con);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            da.Fill(ds);
            dt = ds.Tables[0];
            if (dt.Rows.Count > 0)
            {
                strRes = dt.Rows[0][0].ToString();
            }

            return strRes;

        }

        public string AssignMemberMenu(string RoleID, string MemberID, string InstituteID)
        {
            DataTable dtMenuList = new DataTable();
            dtMenuList = GetMenuMappingID(RoleID);
            int intRes = 0;
            for (int i = 0; i < dtMenuList.Rows.Count; i++)
            {
                intRes = intRes + 1;
                string strsql = "INSERT INTO  `mp_member_menu`";
                strsql = strsql + "(";
                strsql = strsql + "`RoleId`,";
                strsql = strsql + "`MemberId`,";
                strsql = strsql + "`MenuMappingId`,";
                strsql = strsql + "`IsActive`,";
                strsql = strsql + "`InstitutionId`,";
                strsql = strsql + "`SourceId`)";
                strsql = strsql + "VALUES";
                strsql = strsql + "( ";
                strsql = strsql + "" + RoleID + ",";
                strsql = strsql + "" + MemberID + ",";
                strsql = strsql + "" + dtMenuList.Rows[i]["MenuMappingId"].ToString() + ", ";
                strsql = strsql + "" + "1" + ",";
                strsql = strsql + "" + InstituteID + ",";
                strsql = strsql + "" + "99" + ");SELECT LAST_INSERT_ID();";


                MySqlCommand cmd = new MySqlCommand(strsql, con);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                da.Fill(ds);
                dt = ds.Tables[0];


            }
            return intRes.ToString();
        }

        public string MemberSlot(string MemberID, string Day, string SlotTo, string SlotFrom)
        {
            string strRes = "";
            string strsql = "INSERT INTO  `md_members_slot`";
            strsql = strsql + "(";
            strsql = strsql + "`MemberId`,";
            strsql = strsql + "`Day`,";
            strsql = strsql + "`SlotTo`,";
            strsql = strsql + "`SlotFrom`,";
            strsql = strsql + "`CreatedDate`,";
            strsql = strsql + "`IsActive`,";

            strsql = strsql + "`SourceId`)";
            strsql = strsql + "VALUES";
            strsql = strsql + "( ";
            strsql = strsql + "" + MemberID + ",";
            strsql = strsql + "'" + Day + "','";
            strsql = strsql + SlotTo + "','";
            strsql = strsql + SlotFrom + "','";
            strsql = strsql + DateTime.Now.ToString("yyyy-MM-dd") + "',";
            strsql = strsql + "1,";

            strsql = strsql + "99);SELECT LAST_INSERT_ID();";

            MySqlCommand cmd = new MySqlCommand(strsql, con);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            da.Fill(ds);
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                strRes = dt.Rows[0][0].ToString();
            }

            return strRes;

        }

        public string GetInstutionID(string InstutionName)
        {
            string strRes = "";
            string strsql = "Select InstitutionId from  md_institution where Name='" + InstutionName + "'";

            MySqlCommand cmd = new MySqlCommand(strsql, con);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            da.Fill(ds);
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                strRes = dt.Rows[0][0].ToString();
            }

            return strRes;

        }

        public string MappedNetwork(string ParentInstitutionId, string ChildInstitutionID)
        {
            string strRes = "";
            string strsql = "INSERT INTO  `md_network_mapping`";
            strsql = strsql + "(";
            strsql = strsql + "`ParentInstitutionId`,";
            strsql = strsql + "`ChildInstitutionId`,";
            strsql = strsql + "`IsActive`,";
            strsql = strsql + "`SourceId`,";
            strsql = strsql + "`CreatedDate`,";
            strsql = strsql + "`Distance`,";
            strsql = strsql + "`TotalTime`,";
            strsql = strsql + "`Fare`)";
            strsql = strsql + "VALUES";
            strsql = strsql + "( ";
            strsql = strsql + "" + ParentInstitutionId + ",";
            strsql = strsql + "" + ChildInstitutionID + ",";
            strsql = strsql + "1,";
            strsql = strsql + "99,";
            strsql = strsql + "'" + DateTime.Now.ToString("yyyy-MM-dd") + "',";
            strsql = strsql + "0,";
            strsql = strsql + "0,";
            strsql = strsql + "0);SELECT LAST_INSERT_ID();";

            MySqlCommand cmd = new MySqlCommand(strsql, con);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            da.Fill(ds);
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                strRes = dt.Rows[0][0].ToString();
            }

            return strRes;

        }

        public string CreateServiceProvider(string HFNanme, string Addres1, string Addres2, string RefNumber, string CountryId, string StateID, string DistId, string CityId, string Pin, string HfMobile, string HfEmail, string TypeId)
        {

            string strRes = "";
            string InstitutaionID = GetInstutionID(HFNanme);
            if (InstitutaionID == "")
            {
                string strsql = "INSERT INTO  `md_institution`";
                strsql = strsql + "(";
                strsql = strsql + "`Name`,";
                strsql = strsql + "`AddressLine1`,";
                strsql = strsql + "`AddressLine2`,";
                strsql = strsql + "`ReferenceNumber`,";
                strsql = strsql + "`CountryId`,";
                strsql = strsql + "`StateId`,";
                strsql = strsql + "`DistrictId`,";
                strsql = strsql + "`CityId`,";
                strsql = strsql + "`PinCode`,";
                strsql = strsql + "`Mobile`,";
                strsql = strsql + "`Email`,";


                strsql = strsql + "`InstitutionTypeId`,";
                strsql = strsql + "`IsActive`,";


                strsql = strsql + "`CreatedDate`,";
                strsql = strsql + "`SourceId`,";
                strsql = strsql + "`StatusId`)";
                strsql = strsql + "VALUES";
                strsql = strsql + "( ";
                strsql = strsql + "'" + HFNanme + "',";
                strsql = strsql + "'" + Addres1 + "',";
                strsql = strsql + "'" + Addres2 + "',";
                strsql = strsql + "'" + RefNumber + "',";
                strsql = strsql + "" + CountryId + ",";
                strsql = strsql + "" + StateID + ",";
                strsql = strsql + "" + DistId + ",";
                strsql = strsql + "" + CityId + ",";
                strsql = strsql + "'" + Pin + "',";
                strsql = strsql + "'" + HfMobile + "',";
                strsql = strsql + "'" + HfEmail + "',";
                strsql = strsql + "" + TypeId + ",";
                strsql = strsql + "1,";
                strsql = strsql + "'" + DateTime.Now.ToString("yyyy-MM-dd") + "',";
                strsql = strsql + "99,";
                strsql = strsql + "1);SELECT LAST_INSERT_ID();";

                MySqlCommand cmd = new MySqlCommand(strsql, con);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                da.Fill(ds);
                dt = ds.Tables[0];

                if (dt.Rows.Count > 0)
                {
                    strRes = dt.Rows[0][0].ToString();
                }
            }
            else
            {
                strRes = InstitutaionID;
            }


            return strRes;
        }

        public string CreateMember
       (string pFirstName,
          string pMiddleName,
          string pLastName,
          string pAgeType,
          string pDOB,
          string pAge,
          string pMobile,
          string pEmail,
          string pImagePath,
          string pCreatedBy,
          string pCreatedDate,
          string pIsActive,
          string pGenderId,
          string pRegistrationNumber,
          string pAddressLine1,
          string pAddressLine2,
          string pStateId,
          string pDistrictId,
          string pCityId,
          string pSpecializationId,
          string pQualificationId,
          string pPinCode,
          string pFax,
          string pLoginOTP,
          string pIsLoginOTPActive,
          string pSignaturePath,
          string pCountryId,
          string pStatusId,
          string pRatingMasterId,
          string pSourceId,
          string pIsMaster,
          string pPrefix,
          string pCreationRole
         )
        {

            string strRes = "";

            string strsql = "INSERT INTO `md_members` (`FirstName`,";
            strsql = strsql + "`MiddleName`,";
            strsql = strsql + "`LastName`,";
            strsql = strsql + "`AgeType`,";
            strsql = strsql + "`DOB`,";
            strsql = strsql + "`Age`,";
            strsql = strsql + "`Mobile`,";
            strsql = strsql + "`Email`,";
            strsql = strsql + "`ImagePath`,";
            strsql = strsql + "`CreatedBy`,";
            strsql = strsql + "`CreatedDate`,";
            strsql = strsql + "`IsActive`,";
            strsql = strsql + "`GenderId`,";
            strsql = strsql + "`RegistrationNumber`,";
            strsql = strsql + "`AddressLine1`,";
            strsql = strsql + "`AddressLine2`,";
            strsql = strsql + "`StateId`,";
            strsql = strsql + "`DistrictId`,";
            strsql = strsql + "`CityId`,";
            strsql = strsql + "`SpecializationId`,";
            strsql = strsql + "`QualificationId`,";
            strsql = strsql + "`PinCode`,";
            strsql = strsql + "`Fax`,";
            strsql = strsql + "`LoginOTP`,";
            strsql = strsql + "`IsLoginOTPActive`,";
            strsql = strsql + "`SignaturePath`,";
            strsql = strsql + "`CountryId`,";
            strsql = strsql + "`StatusId`,";
            strsql = strsql + "`RatingMasterId`,";
            strsql = strsql + "`SourceId`,";
            strsql = strsql + "`IsMaster`,";
            strsql = strsql + "`Prefix`,";
            strsql = strsql + "`CreationRole`)";
            strsql = strsql + " VALUES('";
            strsql = strsql + pFirstName +
                "','" + pMiddleName +
                "','" + pLastName +
                "'," + pAgeType +
                ",'" + pDOB +
                "'," + pAge +
                ",'" + pMobile +
                "','" + pEmail +
                "','" + pImagePath +
                "'," + pCreatedBy +
                ",'" + pCreatedDate +
                "'," + pIsActive +
                "," + pGenderId +
                ",'" + pRegistrationNumber +
                "','" + pAddressLine1 +
                "','" + pAddressLine2 +
                "'," + pStateId +
                "," + pDistrictId +
                "," + pCityId +
                "," + pSpecializationId +
                "," + pQualificationId +
                ",'" + pPinCode +
                "','" + pFax +
                "','" + pLoginOTP +
                "'," + pIsLoginOTPActive +
                ",'" + pSignaturePath +
                "'," + pCountryId +
                "," + pStatusId +
                "," + pRatingMasterId +
                "," + "99" +
                "," + pIsMaster +
                ",'" + pPrefix +
                "'," + pCreationRole + ")";
            strsql = strsql + "; SELECT LAST_INSERT_ID();";


            MySqlCommand cmd = new MySqlCommand(strsql, con);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            da.Fill(ds);
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                strRes = dt.Rows[0][0].ToString();
            }


            return strRes;

        }

        public string CreateLogin(string UserName, string RefernceId)
        {

            string strRes = "";

            string strsql = "INSERT INTO `md_login`(`UserName`,`Password`,`ReferenceId`,`IsActive`,`SourceId`)VALUES";
            strsql = strsql + "(";
            strsql = strsql + "'" + UserName.ToLower() + "',";
            strsql = strsql + "'ba3253876aed6bc22d4a6ff53d8406c6ad864195ed144ab5c87621b6c233b548baeae6956df346ec8c17f5ea10f35ee3cbc514797ed7ddd3145464e2a0bab413',";
            strsql = strsql + "'" + RefernceId + "',";
            strsql = strsql + "0,";
            strsql = strsql + "'99'); SELECT LAST_INSERT_ID();";


            MySqlCommand cmd = new MySqlCommand(strsql, con);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            da.Fill(ds);
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                strRes = dt.Rows[0][0].ToString();
            }


            return strRes;

        }


        public string AuditLog(string Message, string IconPath, string MemberID, string ModuleId, string EventId, string AccessType, string IP, string Source, string UserType)

        {
            string strRes = "";
            string strsql = "INSERT INTO  `md_audittrail`";
            strsql = strsql + "(";
            strsql = strsql + "`Message`,";
            strsql = strsql + "`CreatedDate`,";
            strsql = strsql + "`IconPath`,";
            strsql = strsql + "`MemberId`,";
            strsql = strsql + "`ModuleId`,";
            strsql = strsql + "`EventId`,";
            strsql = strsql + "`AccessType`,";
            strsql = strsql + "`LocationIPAddress`,";
            strsql = strsql + "`SourceId`,";
            strsql = strsql + "`UserTypeId`)";
            strsql = strsql + "VALUES";
            strsql = strsql + "( ";
            strsql = strsql + "'" + Message + "',";
            strsql = strsql + "'" + DateTime.Now.ToString("yyyy-MM-dd") + "',";
            strsql = strsql + "'" + IconPath + "',";
            strsql = strsql + "" + MemberID + ",";
            strsql = strsql + "" + ModuleId + ",";
            strsql = strsql + "'" + EventId + "',";
            strsql = strsql + "'" + AccessType + "',";
            strsql = strsql + "'" + IP + "',";
            strsql = strsql + "" + "99" + ",";
            strsql = strsql + UserType + ");SELECT LAST_INSERT_ID();";

            MySqlCommand cmd = new MySqlCommand(strsql, con);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            da.Fill(ds);
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                strRes = dt.Rows[0][0].ToString();
            }

            return strRes;

        }



        string IUserRepository.CheckDist(string DistName)
        {
            string strMessage = "";
            MySqlCommand cmd = new MySqlCommand("Select ID from lgd_dist where DistrictNameE='" + DistName + "'", con);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            da.Fill(ds);
            dt = ds.Tables[0];

            if (dt.Rows.Count <= 0)
            {
                strMessage = strMessage + "- Some District Name Does Not Exists in Database !";
            }
            return strMessage;
        }

        string IUserRepository.CheckState(string StateName)
        {
            string strMessage = "";
            MySqlCommand cmd = new MySqlCommand("Select Id from md_state where StateName='" + StateName + "'", con);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            da.Fill(ds);
            dt = ds.Tables[0];

            if (dt.Rows.Count <= 0)
            {
                strMessage = strMessage + "- Some State Name Does Not Exists in Database !";
            }
            return strMessage;
        }

        public async Task<IEnumerable<StateDistrictCity>> GetStateDistrictCities()
        {
            var sql = "SELECT S.StateId, S.StateName,S.StateCode " +
                        ", D.DistrictId, D.DistrictName, " +
                        ", (SELECT ShortDistCode FROM lgd_dist AS L WHERE L.DistrictNameE = D.DistrictName LIMIT 1) AS DistrictShortCode " +
                        ", C.CityId, C.CityName, C.CityCode " +
                        " FROM md_state AS S " +
                        " JOIN md_district AS D ON S.StateId = D.StateId " +
                        " JOIN md_city AS C ON C.DistrictId = D.DistrictId " +
                        " WHERE S.CountryId = 1; ";
            var result = await con.QueryAsync<StateDistrictCity>(sql);
            return result;
        }
    }

}