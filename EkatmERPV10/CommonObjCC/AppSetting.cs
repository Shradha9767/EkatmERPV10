using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
namespace EkatmERPV10.CommonObjCC
{
    public class AppSetting : IAppSetting
    {
        private readonly IOptions<ConnectionStrings> _configuration;
        private readonly IOptions<AppSettings> _appSettings;
        
        public AppSetting(IOptions<ConnectionStrings> configuration, IOptions<AppSettings> appSettings)
        {
            _configuration = configuration;
            _appSettings = appSettings;
           
        }
        // clsCommonFunctions _iCommonFunc = new clsCommonFunctions();
        public string GetConnection() => _configuration.Value.VsAppEnvDBCnnStr;
        //{
        //    return _configuration.Value.VsAppEnvDBCnnStr;
        //}
        public string WebUrl()=> _appSettings.Value.WebUrl;
        //{
        //    return _appSettings.Value.WebUrl;
        //}
        public string GetDynamicURL()=>_appSettings.Value.DynamicURL;
        //{
        //    return _appSettings.Value.DynamicURL;
        //}
        public string MailCnnStr()=> _appSettings.Value.MailCnnStr;
        //{
        //    return _appSettings.Value.MailCnnStr;
        //}
        public string AccType()=> _appSettings.Value.AccType;
        //{
        //    return _appSettings.Value.AccType;
        //}
        public string GetAppCode()
        {
            string AppCode = _appSettings.Value.AppCode;
            if (AppCode == "" || AppCode == null)
            {
                AppCode = "Ekatm";
            }
            return AppCode;
        }
        //public string GetConnection(string dbName)
        //{
        //    string cnnStr = "";
        //    string VsAppEnvDBCnn = _configuration.Value.VsAppEnvDBCnnStr;
        //    string appenvmasterid = _appSettings.Value.WebUrl;
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append(" select CnnString from appenvdb where appenvmasterid='" + appenvmasterid + "' and DBName='" + dbName + "'");
        //    cnnStr = _iCommonFunc.GetScaler(sb.ToString(), VsAppEnvDBCnn);
        //    string CnnString = _iCommonFunc.GetVDecryptedString(cnnStr);
        //    return CnnString;
        //}
        //public string GetEnvironments(string dbName)
        //{
        //    string cnnStr = "";
        //    string VsAppEnvDBCnn = _configuration.Value.VsAppEnvDBCnnStr;
        //    string appenvmasterid = _appSettings.Value.WebUrl;
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append(" select AppEnvMasterId,DBName from appenvdb where appenvmasterid='" + appenvmasterid + "' and DBName='" + dbName + "'");
        //    cnnStr = _iCommonFunc.GetJSON(sb.ToString(), VsAppEnvDBCnn);
        //    return cnnStr;
        //}
    }
}
