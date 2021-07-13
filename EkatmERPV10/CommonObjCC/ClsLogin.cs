using Microsoft.AspNetCore.Http;
using System;
using System.Data;
using System.Linq;
using System.Text;
namespace EkatmERPV10.CommonObjCC
{
    public class ClsLogin
    {
        //IConfiguration _config;
        private readonly ICommonFunctions _iCommonFunc;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ClsLogin(ICommonFunctions IcommonFunctions, IHttpContextAccessor httpContextAccessor)
        {
            _iCommonFunc = IcommonFunctions;
            _httpContextAccessor = httpContextAccessor;
        }
        public bool IsUserLoggedIn()
        {
            var context = _httpContextAccessor.HttpContext;
            var currentUser = _httpContextAccessor.HttpContext.User;
            string RgtCnnString = Convert.ToString(currentUser.Claims.FirstOrDefault(c => c.Type == "RgtCnnStr").Value);
            string MyConnectionString = currentUser.Claims.FirstOrDefault(c => c.Type == "EkatmDbCnnStr").Value;
            string v = currentUser.Claims.FirstOrDefault(c => c.Type == "AppEnvMasterId").Value;
            return context.User.Identities.Any(x => x.IsAuthenticated);
        }
        public string GetWebUrl()
        {
            string DynamicSubdomain = _iCommonFunc.WebUrl();
            string webUrlFin = "";
            try
            {
                if (DynamicSubdomain == "Y")
                {
                    ClsHttpContext url = new ClsHttpContext();
                    //var currentUser = HttpContext.User;
                    string webURL = _iCommonFunc.showURL();
                    if (webURL.Contains("ekatm"))
                    {
                        int indx = webURL.IndexOf(".ekatm");
                        int indx1 = webURL.IndexOf("://");
                        int length = webURL.Length;
                        webUrlFin = webURL.Substring(indx1 + 3, indx - indx1 - 3);
                        webUrlFin = _iCommonFunc.EncryptPwd(webUrlFin);
                    }
                    else
                    {
                        int indx = webURL.IndexOf(".");
                        int indx1 = webURL.IndexOf("://");
                        int length = webURL.Length;
                        webUrlFin = webURL.Substring(indx1 + 3, indx - indx1 - 3);
                        webUrlFin = _iCommonFunc.EncryptPwd(webUrlFin);
                    }
                }
                else if (DynamicSubdomain == "C")
                {
                    webUrlFin = "PM";
                }
                else
                {
                    webUrlFin = _iCommonFunc.EncryptPwd(Convert.ToString(_iCommonFunc.WebUrl()));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return webUrlFin;
        }
        public string GetEnvironments()
        {
            try
            {
                string enviList = "[]";
                string webUrlFin = GetWebUrl();
                if (webUrlFin == "PM")
                {
                    return webUrlFin;
                }
                else if (webUrlFin != "")
                {
                    StringBuilder sb = new StringBuilder();
                    string WebUrl = _iCommonFunc.WebUrl();
                    string VsAppEnvDBCnnStr = _iCommonFunc.GetConnection();
                    sb.Append(" select AppEnvMasterId,AppCode,AppEnvDesc from appenvmaster where appenvmasterid='" + WebUrl + "' ");
                    enviList = _iCommonFunc.GetJSON(sb.ToString(), VsAppEnvDBCnnStr);
                }
                return enviList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public string GetPlants()
        {
            try
            {
                string plants = "[]";
                StringBuilder sb = new StringBuilder();
                string EkatmCnnString = _iCommonFunc.GetConnection("EkatmDB");
                sb.Append(" SELECT  PlantMasterId, PlantName FROM PlantMaster  WHERE IsActive = '1' AND IsDeleted = 'N'");
                plants = _iCommonFunc.GetJSON(sb.ToString(), EkatmCnnString);
                return plants;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public ClsTokenDetails IsValidLogin(string AppEnvMasterId, string UserLoginId, string UserPwd)
        {
            try
            {
                ClsTokenDetails tokenDtls = new ClsTokenDetails();
                DataTable dt = new DataTable();
                DataTable dtEmail = new DataTable();
                StringBuilder sb = new StringBuilder();
                string EkatmCnnString = _iCommonFunc.GetConnection("EkatmDB");
                sb.Append(" select UserMasterId,UserName,UserLoginId,isnull(PlantName,'') as PlantName,UM.PlantMasterId,DeptMasterId from UserMaster UM ");
                sb.Append(" left join PlantMaster PM on UM.PlantMasterId=PM.PlantMasterId ");
                sb.Append(" where UserLoginId = '" + UserLoginId + "'  ");
                sb.Append(" AND UserPassword  = '" + _iCommonFunc.GetEncryptedPasswordMD5(UserPwd) + "' and UM.IsActive='Y' and UM.IsDeleted='N'  ");
                dt = _iCommonFunc.GetDataTable(sb.ToString(), false, EkatmCnnString);
                sb = new StringBuilder();
                sb.Append("select UserName from UserMaster where Email = '" + UserLoginId + "' ");
                sb.Append("AND UserPassword  = '" + _iCommonFunc.GetEncryptedPasswordMD5(UserPwd) + "' and IsActive='Y' and IsDeleted='N' ");
                dtEmail = _iCommonFunc.GetDataTable(sb.ToString(), false, EkatmCnnString);
                if (dt.Rows.Count > 0 || dtEmail.Rows.Count > 0)
                {
                    tokenDtls.AppEnvMasterId = _iCommonFunc.WebUrl();
                    tokenDtls.Plant = dt.Rows[0]["PlantName"].ToString();
                    tokenDtls.UserName = dt.Rows[0]["UserName"].ToString();
                    tokenDtls.UserLoginId = dt.Rows[0]["UserLoginId"].ToString();
                    tokenDtls.EkatmDbCnnStr = EkatmCnnString;
                    tokenDtls.EkatmMsgCnnStr = _iCommonFunc.GetConnection("MessageDB");
                    tokenDtls.RgtCnnStr = _iCommonFunc.GetConnection("EkatmReportDB");
                    tokenDtls.UserMasterId = dt.Rows[0]["UserMasterId"].ToString();
                    tokenDtls.PlantId = dt.Rows[0]["PlantMasterId"].ToString();
                    tokenDtls.DeptMasterId = dt.Rows[0]["DeptMasterId"].ToString();
                    tokenDtls.IsValidUser = true;
                    return tokenDtls;
                }
                else
                {
                    tokenDtls.IsValidUser = false;
                    return tokenDtls;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
