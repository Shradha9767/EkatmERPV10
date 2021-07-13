using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EkatmERPV10.CommonObjCC
{
    public class clsCommonFunctions : ICommonFunctions
    {
        IHttpContextAccessor _httpcontextaccessor;
        private readonly IAppSetting _iAppSetting;
        private readonly IHostingEnvironment _HostEnvironment;
        IConfiguration _config;

        public clsCommonFunctions(IHttpContextAccessor httpcontextaccessor, IAppSetting iAppSetting, IHostingEnvironment HostEnvironment, IConfiguration config)
        {
            _httpcontextaccessor = httpcontextaccessor;
            _iAppSetting = iAppSetting;
            _HostEnvironment = HostEnvironment;
            _config = config;
        }
        string MailcnnString = "";//get from claim
        string AccountType = "vritti";
        string onlyfile = "";
        private bool IsFTP;
        public bool IsLocal;

        public string GenerateJSONWebToken(Claim[] claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public string showURL()
        {
            ClsHttpContext url = new ClsHttpContext();
            return url.showURL(_httpcontextaccessor);
        }
       
        public string GetConnection()
        {
            return _iAppSetting.GetConnection();
        }
        public string WebUrl()
        {
            return _iAppSetting.WebUrl();
        }
        public string GetDynamicURL()
        {
            return _iAppSetting.GetDynamicURL();
        }
        public string GetAppCode()
        {
            return _iAppSetting.GetAppCode();
        }
        public string MailCnnStr()
        {
            return _iAppSetting.MailCnnStr();
        }
        public string AccType()
        {
            return _iAppSetting.AccType();
        }
        public string GetConnection(string dbName)
        {
            string cnnStr = "";
            string VsAppEnvDBCnn = _iAppSetting.GetConnection();//_configuration.Value.VsAppEnvDBCnnStr;
            string appenvmasterid = _iAppSetting.WebUrl(); //_appSettings.Value.WebUrl;
            StringBuilder sb = new StringBuilder();
            sb.Append(" select CnnString from appenvdb where appenvmasterid='" + appenvmasterid + "' and DBName='" + dbName + "'");
            cnnStr = GetScaler(sb.ToString(), VsAppEnvDBCnn);
            string CnnString = GetVDecryptedString(cnnStr);
            return CnnString;
        }
        public string GetEnvironments(string dbName)
        {
            string cnnStr = "";
            string VsAppEnvDBCnn = _iAppSetting.GetConnection();
            string appenvmasterid = _iAppSetting.WebUrl();
            StringBuilder sb = new StringBuilder();
            sb.Append(" select AppEnvMasterId,DBName from appenvdb where appenvmasterid='" + appenvmasterid + "' and DBName='" + dbName + "'");
            cnnStr = GetJSON(sb.ToString(), VsAppEnvDBCnn);
            return cnnStr;
        }
        public DataTable GetDataTable(string query, bool IsStoredPro, string connStr)
        {
            try
            {
                DataTable dt = new DataTable();
                using (var Conn = new SqlConnection(connStr))
                {
                    var cmm = Conn.CreateCommand();
                    if (IsStoredPro == true)
                    {
                        cmm.CommandType = CommandType.StoredProcedure;
                    }
                    else
                    {
                        cmm.CommandType = CommandType.Text;
                    }
                    cmm.CommandText = query;
                    cmm.Connection = Conn;
                    Conn.Open();
                    SqlDataAdapter DA = new SqlDataAdapter(cmm);
                    DA.Fill(dt);
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public DataView GetDataView(string query, bool IsStoredPro, string connStr)
        {
            try
            {
                DataView dv = new DataView();
                DataTable dt = new DataTable();
                using (var Conn = new SqlConnection(connStr))
                {
                    var cmm = Conn.CreateCommand();
                    if (IsStoredPro == true)
                    {
                        cmm.CommandType = CommandType.StoredProcedure;
                    }
                    else
                    {
                        cmm.CommandType = CommandType.Text;
                    }
                    cmm.CommandText = query;
                    cmm.Connection = Conn;
                    Conn.Open();
                    SqlDataAdapter DA = new SqlDataAdapter(cmm);
                    DA.Fill(dt);
                    dv = dt.DefaultView;
                }
                return dv;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public string GetJSON(string query, string Connstr)
        {
            string result = "";
            try
            {
                using (var Conn = new SqlConnection(Connstr))
                {
                    var cmm = Conn.CreateCommand();
                    cmm.CommandType = System.Data.CommandType.Text;
                    cmm.CommandText = query;
                    cmm.Connection = Conn;
                    Conn.Open();
                    SqlDataAdapter DA = new SqlDataAdapter(cmm);
                    DataTable dtEnv = new DataTable();
                    DA.Fill(dtEnv);
                    result = JsonConvert.SerializeObject(dtEnv);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }
        public string GetVDecryptedString(string Input)
        {
            try
            {
                string functionReturnValue = "";
                if (string.IsNullOrEmpty(Input))
                    return functionReturnValue;
                StringBuilder sb = new StringBuilder();
                char[] EncrChars = Input.ToCharArray();
                int CurrentTime = Strings.AscW(EncrChars[0]);
                for (int i = 1; i <= EncrChars.Length - 1; i++)
                {
                    sb.Append(Strings.ChrW(Strings.AscW(EncrChars[i]) - CurrentTime));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetVDecryptedString()-" + ex.Message);
            }
        }
        public string EncryptPwd(string Pwd)
        {
            string functionReturnValue = null;
            try
            {
                int i = 0;
                StringBuilder sb = new StringBuilder();
                string strChar = null;
                for (i = 0; i <= Pwd.Length - 1; i++)
                {
                    strChar = Microsoft.VisualBasic.Strings.Mid(Pwd, i + 1, 1);
                    sb.Append(Microsoft.VisualBasic.Strings.Chr(Microsoft.VisualBasic.Strings.Asc(strChar) + 27));
                    functionReturnValue = sb.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("EncryptPwd()-" + ex.Message);
            }
            return functionReturnValue;
        }
        public string GetEncryptedPasswordMD5(string input)
        {
            using MD5 md5Hash = System.Security.Cryptography.MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
        public string GetScaler(string query, string Connstr)
        {
            string result = "";
            try
            {
                using (var Conn = new SqlConnection(Connstr))
                {
                    var cmm = Conn.CreateCommand();
                    cmm.CommandType = System.Data.CommandType.Text;
                    cmm.CommandText = query;
                    cmm.Connection = Conn;
                    Conn.Open();
                    result = cmm.ExecuteScalar().ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }
        public bool SendEmail(string FromEmailId, string ToEmailId, string CCEmailId, string BCCEmailID, string Subject, string Body, string FromName, ArrayList Attachmnts)
        {
            try
            {
                string VrittiCommtr = GetConnection("MessageDB");
                SendEmailData(FromEmailId, ToEmailId, CCEmailId, BCCEmailID, Subject, Body, Attachmnts, "E", "N", DateTime.Now, Convert.ToDecimal(10), MailcnnString, AccountType, "Mail", "");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public string SendEmailData(string SendermailId, string ReceivermailId, string MailCc, string MailBcc, string MailSubject, string MailBody, ArrayList attachments, string MsgType, string MsgStatus, DateTime MailSentAtTime, decimal ExpiryPeriod, string ConStr, string AcntType, string MailType, string AppointmentString)
        {
            string Emailinsert;
            SqlCommand CmdEmailInsert;
            string MsgIDGuid;
            bool CheckFile = false;
            bool AttChkFlag = true;
            //string MailCnnStr = objClient.ERPMSGCnnStr.ToString();
            string MailCnnStr = MailcnnString;
            try
            {
                if (MailCnnStr == "" || MailCnnStr == null || MailCnnStr == "undefined")
                {
                    MailCnnStr = MailcnnString;
                    if (MailCnnStr == "" || MailCnnStr == null || MailCnnStr == "undefined")
                    {
                        MailCnnStr = ConStr;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            try
            {
                if (AttChkFlag == true)
                {
                    byte[] mailbodyinbyte = StringToByteArray(MailBody, MailDataForm.MailUtility.EncodingType.ASCII);
                    MsgIDGuid = Convert.ToString(Guid.NewGuid());
                    if (attachments != null)
                    {
                        if (attachments.Count > 0)
                        {
                            CheckFile = CheckFileAttachement(attachments, MailCnnStr, MsgIDGuid);
                        }
                    }
                    Emailinsert = "Insert Into VSL_VC_MailDataMaster(MessageID,MessageFrom,MessageTo,MessageCC,MessageBCC,MessageSubject,MessageBody,MessageType,MessageSentAt,MessageStatus,MessageRemark,AddedBy,AddedDate,ExpiryTime,AccountType,AppointmentString,MailType)";
                    Emailinsert += " Values(@MsgId,@MsgFrom,@MsgTo,@MsgCC,@MsgBCC,@MsgSubject,@MsgBody,@MsgType,@MsgSentAt,@MsgStatus,@MsgRemark,@AddedBy,@AddedDt,@ExpiryTime,@AccType,@AppointmentString,@MailType)";
                    CmdEmailInsert = new SqlCommand(Emailinsert);
                    CmdEmailInsert.Parameters.AddWithValue("@MsgId", MsgIDGuid);
                    CmdEmailInsert.Parameters.AddWithValue("@MsgFrom", SendermailId);
                    CmdEmailInsert.Parameters.AddWithValue("@MsgTo", ReceivermailId);
                    CmdEmailInsert.Parameters.AddWithValue("@MsgCC", MailCc);
                    CmdEmailInsert.Parameters.AddWithValue("@MsgBCC", MailBcc);
                    CmdEmailInsert.Parameters.AddWithValue("@MsgSubject", MailSubject);
                    CmdEmailInsert.Parameters.AddWithValue("@MsgBody", mailbodyinbyte);
                    CmdEmailInsert.Parameters.AddWithValue("@MsgType", MsgType);
                    CmdEmailInsert.Parameters.AddWithValue("@MsgSentAt", MailSentAtTime.ToString("dd-MMM-yyyy hh:mm tt"));
                    CmdEmailInsert.Parameters.AddWithValue("@MsgStatus", MsgStatus);
                    CmdEmailInsert.Parameters.AddWithValue("@MsgRemark", "");
                    CmdEmailInsert.Parameters.AddWithValue("@AddedBy", Environment.UserName);
                    CmdEmailInsert.Parameters.AddWithValue("@AddedDt", DateTime.Now);
                    CmdEmailInsert.Parameters.AddWithValue("@ExpiryTime", MailSentAtTime.AddHours(Convert.ToInt32(ExpiryPeriod)).ToString("dd-MMM-yyyy hh:mm tt"));
                    CmdEmailInsert.Parameters.AddWithValue("@AccType", AcntType);
                    CmdEmailInsert.Parameters.AddWithValue("@AppointmentString", AppointmentString);
                    CmdEmailInsert.Parameters.AddWithValue("@MailType", MailType);
                    string Emailreply = "";
                    Emailreply = ExecuteParameterQuery(CmdEmailInsert, MailCnnStr);
                    if (Emailreply != "Success")
                    {
                        return "F";
                    }
                    else
                    {
                        return "S";
                    }
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public string ExecuteParameterQuery(SqlCommand CmdParam, string DBcnn)
        {
            try
            {
                SqlConnection con = new SqlConnection();
                SqlDataAdapter sqlDA;
                DataSet sqlDS;
                CmdParam.Connection = con;
                CmdParam.Connection.ConnectionString = DBcnn;
                CmdParam.CommandType = CommandType.Text;
                if (CmdParam.Connection.State == ConnectionState.Closed)
                {
                    CmdParam.Connection.Open();
                }
                sqlDA = new SqlDataAdapter();
                sqlDS = new DataSet();
                sqlDA.SelectCommand = CmdParam;
                CmdParam.ExecuteNonQuery();
                CmdParam.Connection.Close();
                return "Success";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool CheckFileAttachement(ArrayList list, string cnnstr, string MSGID)
        {
            try
            {
                int number = list.Count;
                if (number == 0)
                {
                    onlyfile = "";
                    return true;
                }
                else
                {
                    for (int i = 0; i < number; i++)
                    {
                        string AttachFile = list[i].ToString();//LstFile.Items[i].ToString();
                        string MyAttachment = AttachFile.Replace("'", "");
                        int index = MyAttachment.LastIndexOf(@"\");
                        onlyfile = MyAttachment.Substring(index + 1);
                        string str = "";
                        if (IsFTP == true)
                        {
                            //str = FileUpload_2_FTP(AttachFile, cnnstr, MSGID);
                        }
                        else
                        {
                            //IsLocal = IsLocal;
                            str = InsertAttachment_Old(AttachFile, cnnstr, MSGID);
                        }
                        if (str != "Success")
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ArrayList GetFileNameAndExtension(string CompleteFile)
        {
            ArrayList List = new ArrayList();
            try
            {
                int i = CompleteFile.LastIndexOf(@"\");
                int j = CompleteFile.LastIndexOf(".");
                int len = j - i;
                string FN = CompleteFile.Substring(i + 1, len - 1);
                string Extension = CompleteFile.Substring(j + 1);
                List.Add(FN);
                List.Add(Extension);
                return List;
            }
            catch (Exception ex)
            { throw new Exception(ex.Message); }
        }
        public string InsertAttachment_Old(string filetoattach, string Cnn, string MSGID)
        {
            SqlCommand CmdAttachmentInsert;
            string AttachGUID;
            byte[] ImageData = Encoding.Unicode.GetBytes("");
            try
            {
                ArrayList FDetails = new ArrayList();
                string AttachmentID = MSGID;
                FDetails = GetFileNameAndExtension(filetoattach);
                string insertattach = "Insert into VSL_VC_AttachmentMaster(AttachmentId,MsgId,FileName,FileType,FileContent,FilePath)";
                insertattach += " Values(@AttachId,@MsgId,@FileName,@FileType,@FileContent,@FilePath)";
                CmdAttachmentInsert = new SqlCommand(insertattach);
                AttachGUID = Convert.ToString(Guid.NewGuid());
                CmdAttachmentInsert.Parameters.AddWithValue("@AttachId", AttachGUID);
                CmdAttachmentInsert.Parameters.AddWithValue("@MsgId", AttachmentID);
                CmdAttachmentInsert.Parameters.AddWithValue("@FileName", FDetails[0]);
                CmdAttachmentInsert.Parameters.AddWithValue("@FileType", FDetails[1]);
                CmdAttachmentInsert.Parameters.AddWithValue("@FileContent", ImageData);
                CmdAttachmentInsert.Parameters.AddWithValue("@FilePath", filetoattach);
                string reply = ExecuteParameterQuery(CmdAttachmentInsert, Cnn);
                if (reply != "Success")
                {
                    return reply;
                }
                return reply;
            }
            catch (Exception ex)
            { throw new Exception(ex.Message); }
        }
        public byte[] StringToByteArray(string str, MailDataForm.MailUtility.EncodingType encodingType)
        {
            System.Text.Encoding encoding = null;
            switch (encodingType)
            {
                case MailDataForm.MailUtility.EncodingType.ASCII:
                    encoding = new System.Text.ASCIIEncoding();
                    break;
                case MailDataForm.MailUtility.EncodingType.Unicode:
                    encoding = new System.Text.UnicodeEncoding();
                    break;
                case MailDataForm.MailUtility.EncodingType.UTF7:
                    encoding = new System.Text.UTF7Encoding();
                    break;
                case MailDataForm.MailUtility.EncodingType.UTF8:
                    encoding = new System.Text.UTF8Encoding();
                    break;
            }
            return encoding.GetBytes(str);
        }
        public int ExecuteNonQuery(string query, string Connstr)
        {
            int result;
            try
            {
                using (var Conn = new SqlConnection(Connstr))
                {
                    var cmm = Conn.CreateCommand();
                    cmm.CommandType = System.Data.CommandType.Text;
                    cmm.CommandText = query;
                    cmm.Connection = Conn;
                    Conn.Open();
                    result = cmm.ExecuteNonQuery();
                }
              
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public void WriteFinanceLog(string MsgBody, String Error,string Module)    
        {
            string Path = null;
           
            StackTrace stackTrace = new StackTrace();
            StackFrame[] stackFrames = stackTrace.GetFrames();
            string[] methodName = new string[stackTrace.FrameCount];
            int cnt = 0;
            string ErrorOrigin = "";
            foreach (StackFrame stackFrame in stackFrames)
            {
                if (stackFrame.GetMethod().Name != "lambda_method") 
                {
                    methodName[cnt] = stackFrame.GetMethod().Name;
                    cnt++;
                }
                else
                {
                    ErrorOrigin = methodName[--cnt];
                    break;
                }
            }
            string webRootPath = _HostEnvironment.WebRootPath;
            string contentRootPath = _HostEnvironment.ContentRootPath;

            Path= System.IO.Path.Combine(webRootPath + "~/ErrorLogs/"+Module+"Log");
            //Path = HttpContext.Server.MapPath("~/ErrorLogs/FinanceLog");
            if (System.IO.Directory.Exists(Path) == false)
            {
                System.IO.Directory.CreateDirectory(Path);       
            }
            if (Error != "")
            {
                Path += "//" + WebUrl() +ErrorOrigin + DateAndTime.Now.ToString("yyyyMMdd") + "FinanceLog.txt";
                System.IO.StreamWriter SW = new System.IO.StreamWriter(Path, true);
                SW.Write(DateTime.Now);
                SW.Write("\t");
                SW.Write(MsgBody);
                SW.Write("\t");
                SW.Write(Error);
                SW.WriteLine();
                SW.Close();
            }
            else
            {
                Path += "\\" + WebUrl() + ErrorOrigin + DateAndTime.Now.ToString("yyyyMMdd") + "FinanceLog.txt";
                System.IO.StreamWriter SW = new System.IO.StreamWriter(Path, true);
                SW.Write(DateTime.Now);
                SW.Write("\t");
                SW.Write(MsgBody);
                SW.Write("\t");
                SW.Write("Successful");
                SW.WriteLine();
                SW.Close();
            }
        }
        public  List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        public  T GetItem<T>(DataRow dr)
        {
            try
            {
                Type temp = typeof(T);
                T obj = Activator.CreateInstance<T>();
                int cnt = 0;
                foreach (DataColumn column in dr.Table.Columns)
                {
                    cnt = 0;
                    foreach (PropertyInfo pro in temp.GetProperties())
                    {
                        if (cnt != 1)
                        {
                            if (pro.Name == column.ColumnName)
                            {
                                cnt = 1;
                                if (!object.Equals(dr[pro.Name], DBNull.Value))
                                {
                                    pro.SetValue(obj, dr[pro.Name], null);
                                }
                            }
                            else
                                continue;
                        }
                        else
                            break;
                    }
                }
                return obj;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public string DataTableToJSONWithJavaScriptSerializer(DataTable table)
        //{
        //    JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
        //    jsSerializer.MaxJsonLength = Int32.MaxValue;
        //    List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
        //    Dictionary<string, object> childRow;
        //    foreach (DataRow row in table.Rows)
        //    {
        //        childRow = new Dictionary<string, object>();
        //        foreach (DataColumn col in table.Columns)
        //        {
        //            childRow.Add(col.ColumnName, row[col]);
        //        }
        //        parentRow.Add(childRow);
        //    }
        //    return jsSerializer.Serialize(parentRow);
        //}


        public DataTable ToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection props =TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }
        public DataTable JsonStringToDataTable(string jsonString)
        {
            DataTable dt = new DataTable();
            string[] jsonStringArray = Regex.Split(jsonString.Replace("[", "").Replace("]", ""), "},\r\n  {");
            List<string> ColumnsName = new List<string>();
            foreach (string jSA in jsonStringArray)
            {
                string[] jsonStringData = Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
                foreach (string ColumnsNameData in jsonStringData)
                {
                    try
                    {
                        int idx = ColumnsNameData.IndexOf(":");
                        string ColumnsNameString = ColumnsNameData.Substring(0, idx - 1).Replace("\"", "").Replace("\r\n", "").Replace(" ", "");
                        if (!ColumnsName.Contains(ColumnsNameString))
                        {
                            ColumnsName.Add(ColumnsNameString);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Error Parsing Column Name : {0}", ColumnsNameData));
                    }
                }
                break;
            }
            for (int i = 0; i < ColumnsName.Count; i++)
            {
                dt.Columns.Add(ColumnsName[i]);
            }
            foreach (string jSA in jsonStringArray)
            {
                string[] RowData = Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
                int cnt = 0;
                int colcnt = ColumnsName.Count;
                DataRow nr = dt.NewRow();
                foreach (string rowData in RowData)
                {
                    if (colcnt == ColumnsName.Count)
                    {
                        DataRow dr = dt.NewRow();
                        dt.Rows.Add(dr);
                        cnt = dt.Rows.Count - 1;
                        colcnt = 0;
                    }
                    try
                    {
                        int idx = rowData.IndexOf(":");
                        string RowColumns = rowData.Substring(0, idx - 1).Replace("\"", "").Replace("\r\n", "").Replace(" ", "");
                        string RowDataString = rowData.Substring(idx + 1).Replace("\"", "").Replace("\r\n", "");
                        nr[RowColumns] = RowDataString;
                        dt.Rows[cnt][RowColumns] = RowDataString;
                        colcnt = colcnt + 1;
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
            }
            return dt;
        }
        public T BindData<T>(DataTable dt, DataRow dr1)
        {
            DataRow dr = dr1;
            List<string> columns = new List<string>();
            foreach (DataColumn dc in dt.Columns)
            {
                columns.Add(dc.ColumnName.Trim());
            }
            var ob = Activator.CreateInstance<T>();
            var properties = typeof(T).GetProperties();
            for (int i = 0; i < columns.Count; i++)
            {
                foreach (var propertyInfo in properties)
                {
                    if (columns[i].Contains(propertyInfo.Name))
                    {
                        char[] delimiterChars = { ' ' };
                        string[] temp = propertyInfo.ToString().Split();
                        string str = propertyInfo.PropertyType.FullName;
                        string pro = temp[0];
                        propertyInfo.SetValue(ob, Convert.ChangeType(dr[i], Type.GetType(str)));
                    }
                }
            }
            return ob;
        }
        public IEnumerable<FieldInfo> GetAllFields(Type t)
        {
            if (t == null)
                return Enumerable.Empty<FieldInfo>();
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
                                 BindingFlags.Static | BindingFlags.Instance |
                                 BindingFlags.DeclaredOnly;
            return t.GetFields(flags).Concat(GetAllFields(t.BaseType));
        }
    }

}
