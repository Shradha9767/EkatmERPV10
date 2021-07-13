using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using System.Text;
namespace EkatmERPV10.CommonObjCC
{
    public class ForgetPassword
    {
        private readonly ICommonFunctions _iCommonFunc;
        public ForgetPassword(ICommonFunctions IcommonFunctions)
        {
            _iCommonFunc = IcommonFunctions;
        }
        public string GETPassword(string Email, string userId)
        {
            string returnMsg = "OK";
            try
            {
                string EkatmDBCnn = _iCommonFunc.GetConnection("EkatmDB");               
                string AppCode = _iCommonFunc.GetAppCode();
                string hostName = Dns.GetHostName();
                string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
                string Key = Guid.NewGuid().ToString();
                byte[] encData_byte = new byte[Key.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(Key);
                string fetchedUrl = _iCommonFunc.showURL();
                string encodedData = Convert.ToBase64String(encData_byte);
                string hdnfldULOGIN = "";
                string hdnfldEmail = "";
                string PortalName = "";
                string _from = "";
                string _subj = "";
                string _body = "";
                string domain = "";
                string subDomain = "";
                int indxOfLocal ;
                DataTable dtEmail = new DataTable();
                DataTable dtUserId = new DataTable();
                DataTable dtUser = new DataTable();

                if (AppCode == "PM")
                {
                    if (EkatmDBCnn == null && EkatmDBCnn == "")
                    {
                        returnMsg = "Company does not exist";
                        return returnMsg;
                    }                    
                   
                    if (userId != "" || Email != "")
                    {
                        if (Email != "")
                        {                           
                            dtEmail = GetUserData(Email, "", EkatmDBCnn); //dvum = FetchUserData(Email, EkatmDBCnn);
                        }
                        if (dtEmail.Rows.Count > 0)
                        {
                            string[] strekatmdb = new string[] { };
                            InsertFunction(userId, hdnfldEmail, encodedData, myIP, EkatmDBCnn);
                            _subj = "Forgot Password Information";
                            if (fetchedUrl.Contains(".simplifypractice"))
                            {
                               
                                if (fetchedUrl.Contains("https"))
                                {
                                    subDomain = fetchedUrl.Substring(8, fetchedUrl.Length - 9);
                                }
                                else
                                {
                                    subDomain = fetchedUrl.Substring(7, fetchedUrl.Length - 8);
                                }
                                 indxOfLocal = subDomain.IndexOf(".");
                                domain = subDomain.Substring(0, indxOfLocal);
                                domain = domain + ".simplifypractice.com";
                                PortalName = "Via SPMPortal";
                                _from = "automail@simplifypractice.com";
                            }
                            else
                            {
                                subDomain = fetchedUrl.Substring(7, fetchedUrl.Length - 8);
                                indxOfLocal = subDomain.IndexOf(".");
                                domain = subDomain.Substring(0, indxOfLocal);
                                domain = domain + ".appnet.co.in";
                                PortalName = "Via VrittiPortal";
                                _from = "vwb@vritti.co.in";
                            }
                            _body = "<body style=background-color:#CFECEC;font-family:Verdana>" + "Dear" + "  " + dtUser.Rows[0]["UserName"].ToString() + "," + "<br><br>" + "It appears that you have forgotten your password!" + "<br><br>" + "To reset your password" + " <a href='http:\\" + domain + "\\ForgotPass\\newPasswrd?id=" + encodedData + "'> simply click here.</a><br><br>If it is not working copy http:\\" + domain + "\\ForgotPass\\newPasswrd?id=" + encodedData + " this link and paste in your browser<br><br>Thanks & Regards,<br><br>" + PortalName + "</body>";
                            _iCommonFunc.SendEmail(_from, hdnfldEmail, "", "", _subj, _body, "", null);
                        }
                        else
                        {
                            returnMsg = "Email address is not registered";
                            return returnMsg;
                        }
                    }
                }
                else
                {
                    if (Email != "")
                    {
                       
                        dtEmail = GetUserData(Email,"",EkatmDBCnn);//FetchUserMasterData(txtEmail, EkatmDBCnn);
                        if (dtEmail.Rows.Count > 0)
                        {
                            hdnfldULOGIN = dtEmail.Rows[0]["UserLoginId"].ToString();
                            hdnfldEmail = Email;
                        }
                    }
                    if (userId != "")
                    {
                         dtUserId = GetUserData("", userId, EkatmDBCnn);//Fetchdata(userId, EkatmDBCnn);
                        
                        if (dtUserId.Rows.Count > 0)
                        {
                            hdnfldEmail = dtUserId.Rows[0]["Email"].ToString();
                            hdnfldULOGIN = userId;
                        }
                    }
                    if (userId != "" || Email != "")
                    {
                        if (Email != "")
                        {
                            dtUser = GetUserData(Email, "", EkatmDBCnn); //FetchUserData(Email, EkatmDBCnn);
                        }
                        if (userId != "")
                        {
                            dtUser = GetUserData("",Email,EkatmDBCnn); //FetchUserMasterRecords(hdnfldULOGIN, EkatmDBCnn);
                        }
                        if (dtUser.Rows.Count > 0)
                        {
                            InsertFunction(hdnfldULOGIN, hdnfldEmail, encodedData, myIP, EkatmDBCnn);
                            _subj = "Forgot Password Information";
                        }
                        if (AppCode == "Sahara")
                        {
                            if (fetchedUrl.Contains("ekatm"))
                            {
                                subDomain = fetchedUrl.Substring(7, fetchedUrl.Length - 8);
                                 indxOfLocal = subDomain.IndexOf(".");
                                domain = subDomain.Substring(0, indxOfLocal);
                                domain = domain + ".ekatm.com";
                                PortalName = "Sahara Team";
                                _from = "sahara@divinecampus.in";
                            }
                            else
                            {
                                domain = "edu.puneprivateschools.in";
                                PortalName = "Sahara Team";
                                _from = "sahara@divinecampus.in";
                            }
                        }
                        else if (fetchedUrl.Contains("localhost"))
                        {
                            subDomain = fetchedUrl.Substring(7, fetchedUrl.Length - 8);
                             indxOfLocal = subDomain.IndexOf("/");
                            domain = subDomain.Substring(0, indxOfLocal);
                            PortalName = "Via VrittiPortal";
                            _from = "vwb@ekatm.com";
                        }
                        else if (fetchedUrl.Contains("ekatm"))
                        {
                            subDomain = fetchedUrl.Substring(7, fetchedUrl.Length - 8);
                             indxOfLocal = subDomain.IndexOf(".");
                            domain = subDomain.Substring(0, indxOfLocal);
                            domain = domain + ".ekatm.com";
                            PortalName = "Via VrittiPortal";
                            _from = "vwb@ekatm.com";
                        }
                        else
                        {
                            subDomain = fetchedUrl.Substring(7, fetchedUrl.Length - 8);
                            indxOfLocal = subDomain.IndexOf(".");
                            domain = subDomain.Substring(0, indxOfLocal);
                            domain = domain + ".appnet.co.in";
                            PortalName = "Via VrittiPortal";
                            _from = "vwb@vritti.co.in";
                        }
                        _body = "<body style=background-color:#CFECEC;font-family:Verdana>" + "Dear" + "  " + dtUser.Rows[0][0].ToString() + "," + "<br><br>" + "It appears that you have forgotten your password!" + "<br><br>" + "To reset your password" + " <a href='http:\\" + domain + "\\ForgotPass\\newPasswrd?id=" + encodedData + "'> simply click here.</a><br><br>If it is not working copy http:\\" + domain + "\\ForgotPass\\newPasswrd?id=" + encodedData + " this link and paste in your browser<br><br>" +
                               "Thanks & Regards<br><br>" + PortalName + "</body>";
                        _iCommonFunc.SendEmail(_from, hdnfldEmail, "", "", _subj, _body, "", null);
                    }
                    else
                    {
                        if (userId != "")
                        {
                            returnMsg = "Entered UserId does not exist";
                            return returnMsg;
                        }
                        else
                        {
                            returnMsg = "Email address is not registered";
                            return returnMsg;
                        }
                    }
                }
                return returnMsg;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool InsertFunction(string ULOGIN, string Email, string encodedata, string IPAddress, string Mycnn)
        {
            bool b = false;
            try
            {
                string Query = "insert into ForgotPassword(UserLogInId,Email,ForgotKey, IPAddress)values('" + ULOGIN + "','" + Email + "','" + encodedata + "','" + IPAddress + "' )";
                _iCommonFunc.ExecuteNonQuery(Query, Mycnn);
                b = true;
            }
            catch (Exception ex)
            {
                throw new Exception("InsertFunction()-" + ex.Message);
            }
            return b;
        }

        public DataTable GetUserData(string UserLoginId, string Email,string MyCnn)
        {
            DataTable dt = new DataTable();
            StringBuilder sb = new StringBuilder();
            sb.Append(" Select UserName,UserPassword,UserLoginId, Email from UserMaster where  ");
            if(UserLoginId!=null|| UserLoginId!="")
            {
                sb.Append(" UserLoginId='" + UserLoginId + "' ");
            }
            if (Email != null || Email != "")
            {
                sb.Append(" Email='" + Email + "' ");
            }

            sb.Append(" and IsDeleted='N' ");
            dt = _iCommonFunc.GetDataTable(sb.ToString(),false, MyCnn);
            return dt;

        }
        //public DataTable Fetchdata(string UserName, string MyCnn)
        //{
        //    try
        //    {
        //        string Query = "Select Email from UserMaster where  UserLoginId='" + UserName + "' and IsDeleted='N'";
        //        DataTable dtUser = _iCommonFunc.GetDataTable(Query, false, MyCnn);
        //        return dtUser;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Fetchdata()-" + ex.Message);
        //    }
        //}
        //public DataTable FetchUserMasterDataFOrCA(string Email, string MyConn)
        //{
        //    try
        //    {
        //        string Query = "Select Email from UserMaster where  Email='" + Email + "' and IsDeleted='N'";
        //        DataTable Dt = _iCommonFunc.GetDataTable(Query, false, MyConn);
        //        return Dt;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("FetchUserMasterData()-" + ex.Message);
        //    }
        //}
        //public DataTable FetchUserMasterData(string Email, string MyConn)
        //{
        //    try
        //    {
        //        string Query = "Select UserLoginId from UserMaster where  Email='" + Email + "' and IsDeleted='N'";
        //        DataTable dtUser = _iCommonFunc.GetDataTable(Query, false, MyConn);
        //        return dtUser;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("FetchUserMasterData()-" + ex.Message);
        //    }
        //}
        //public DataTable FetchUserData(string Email, string MyConn)
        //{
        //    try
        //    {
        //        string Query = "Select UserName,UserPassword from UserMaster where Email='" + Email + "' and IsDeleted='N'";
        //        DataTable Dt = _iCommonFunc.GetDataTable(Query, false, MyConn);
        //        return Dt;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("FetchUserData()-" + ex.Message);
        //    }
        //}
        //public DataTable FetchUserMasterRecords(string ULOGIN, string MyConn)
        //{
        //    try
        //    {
        //        string Query = "Select UserName,UserPassword from UserMaster where UserLoginId = '" + ULOGIN + "' and IsDeleted='N'";
        //        DataTable Dt = _iCommonFunc.GetDataTable(Query, false, MyConn);
        //        return Dt;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("FetchUserMasterRecords()-" + ex.Message);
        //    }
        //}
    }
}
