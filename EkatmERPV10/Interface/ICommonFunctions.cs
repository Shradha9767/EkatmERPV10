using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Collections;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

public interface ICommonFunctions
{
    //IHttpContextAccessor
    public string showURL();
    //Appsettings method
    public string GetConnection();
    public string GetConnection(string dbName);
    public string WebUrl();
    public string GetEnvironments(string dbName);
    public string GetDynamicURL();
    public string GetAppCode();
    public string MailCnnStr();
    public string AccType();
    //CommonFunctions methods
    public string GenerateJSONWebToken(Claim[] claims);
    public DataTable GetDataTable(string query, bool IsStoredPro, string connStr);
    public DataView GetDataView(string query, bool IsStoredPro, string connStr);
    public string GetJSON(string query, string Connstr);
    public string GetVDecryptedString(string Input);
    public string EncryptPwd(string Pwd);
    public string GetEncryptedPasswordMD5(string input);
    public string GetScaler(string query, string Connstr);
    public bool SendEmail(string FromEmailId, string ToEmailId, string CCEmailId, string BCCEmailID, string Subject, string Body, string FromName, ArrayList Attachmnts);
    public string SendEmailData(string SendermailId, string ReceivermailId, string MailCc, string MailBcc, string MailSubject, string MailBody, ArrayList attachments, string MsgType, string MsgStatus, DateTime MailSentAtTime, decimal ExpiryPeriod, string ConStr, string AcntType, string MailType, string AppointmentString);
    public bool CheckFileAttachement(ArrayList list, string cnnstr, string MSGID);
    public byte[] StringToByteArray(string str, MailDataForm.MailUtility.EncodingType encodingType);
    public int ExecuteNonQuery(string query, string Connstr);
    public List<T> ConvertDataTable<T>(DataTable dt);
    //public string DataTableToJSONWithJavaScriptSerializer(DataTable table);
}
