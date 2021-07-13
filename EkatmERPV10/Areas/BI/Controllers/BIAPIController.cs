using EkatmERPV10.CommonObjCC;
//using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Xml;

namespace EkatmERPV10.Areas.BI.Controllers
{
    // [Area("Customer")]
    //[Area("BIAPI")]
    [Route("api/[controller]")]
    //[Route("api/BIAPI")]//https://localhost:44315/api/BIAPI
    [ApiController]

    public class BIAPIController : ControllerBase
    {

        private readonly ICommonFunctions _iCommonFunc;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHostingEnvironment _HostEnvironment;

        public BIAPIController(ICommonFunctions IcommonFunctions, IHttpContextAccessor httpContextAccessor, IHostingEnvironment HostEnvironment)
        {
            _iCommonFunc = IcommonFunctions;
            _httpContextAccessor = httpContextAccessor;
            _HostEnvironment = HostEnvironment;

        }
        string MyConnstr = "";
        string RgtConnstr = "";
        [Authorize]
        [Route("GetCriteria")]
        [HttpGet]
        public string GetCriteria(string RptCode, string linkFlag, string linkCol, string ParentRgtCode, string RowId)
        {
            try
            {
                var currentUser = HttpContext.User;
                RgtConnstr = currentUser.Claims.FirstOrDefault(c => c.Type == "RgtCnnStr").Value;
                MyConnstr = currentUser.Claims.FirstOrDefault(c => c.Type == "EkatmDbCnnStr").Value;

                clsCtrlRGT objrgt = new clsCtrlRGT(_iCommonFunc, _httpContextAccessor, _HostEnvironment);
                string RptName;
                string NewToken;
                string RptCriteria = objrgt.GetCriteria(RptCode, linkFlag, linkCol, ParentRgtCode, RowId, MyConnstr, RgtConnstr, out RptName, out NewToken);
                string RgtActions = objrgt.GetActions(RptCode, RgtConnstr);
                if (RptCriteria != "[]")
                    return RptCriteria + "$@#" + RptName + "$@#" + RgtActions + "$@#" + NewToken;
                else
                    return RptCriteria;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        //*****************************************JSAPI*************************************************
        [Authorize]
        [Route("GetData")]
        [HttpGet]
        public string GetData(string RptCode, string Criteria)
        {
            try
            {
                //dynamic objArr = Arr;
                //string RptCode = RptCode;//objArr.RptCode;
                //string Criteria = Criteria;// objArr.Criteria;
                DataTable Dtcriteria = new DataTable();
                ArrayList arrColStyle = new ArrayList();
                Dictionary<string, string> RptDict = new Dictionary<string, string>();
                string RptXml = "";
                string FldMstId = Criteria;
                string RptStyle = "";
                string RptUniqueField = "";
                DataTable dtRpt = new DataTable();
                DataTable DtLinkInfo = new DataTable();
                DataTable dtrptActions = new DataTable();
                ClsRptXml ObjRptXml = new ClsRptXml();
                clsShowMyReport objcls = new clsShowMyReport(_iCommonFunc, _httpContextAccessor, _HostEnvironment);
                var CurrentUser = HttpContext.User;
                string EkatmdbConn = Convert.ToString(CurrentUser.Claims.FirstOrDefault(c => c.Type == "EkatmDbCnnStr").Value);
                string RptConn = Convert.ToString(CurrentUser.Claims.FirstOrDefault(c => c.Type == "RgtCnnStr").Value);
                objcls.GetReportInfo(RptCode, RptConn, EkatmdbConn, ref RptXml, ref arrColStyle, ref Dtcriteria, ref DtLinkInfo, ref dtrptActions, ref RptStyle, ref RptUniqueField, ref ObjRptXml);
                ///Added for BI Session//add these in claim
                //arrColStyle = objReportSession.arrColStyle;
                //Dtcriteria = objReportSession.Dtcriteria;
                //RptXml = objReportSession.RptXml;
                //RptUniqueField = objReportSession.RptUniqueField;
                //dtrptActions = objReportSession.dtrptActions;
                //RptStyle = objReportSession.RptStyle;
                //DtLinkInfo = objReportSession.DtLinkInfo;
                // ObjRptXml = objReportSession.objEntity;
                ///
                string RptxmlStrglobal = RptXml;
                if (Criteria != "" && Criteria != null)
                {
                    for (int i = 0; i < Dtcriteria.Rows.Count; i++)
                    {
                        Dtcriteria.Rows[i]["Value"] = "";
                        Dtcriteria.Rows[i]["Value1"] = "";
                        Dtcriteria.Rows[i]["Value2"] = "";
                        Dtcriteria.Rows[i]["Value3"] = "";
                        Dtcriteria.Rows[i]["SessionValue1"] = "";
                        Dtcriteria.Rows[i]["SessionValue2"] = "";
                    }
                    Dtcriteria = SetCriteria(Criteria, Dtcriteria);
                }
                if (ObjRptXml.ChartXml == "")
                {
                    if (ObjRptXml.RptStyle != "Fixed")
                    {
                        string fldName = "";
                        dtRpt = objcls.GetDataForReport(Dtcriteria, EkatmdbConn, RptConn, RptCode, arrColStyle, "");
                        string data = JsonConvert.SerializeObject(dtRpt);
                        ClsFldMst objfldmst = new ClsFldMst();
                        if (objfldmst.IsBrk == true)
                        {
                            fldName = objfldmst.FldAlias;
                        }
                        fldName = "";
                        DataTable dtStyle = new DataTable();
                        dtStyle.Columns.Add("IsBreak");
                        dtStyle.Columns.Add("Data");
                        DataRow dr = dtStyle.NewRow();
                        dr["IsBreak"] = fldName;
                        dr["Data"] = data;
                        dtStyle.Rows.Add(dr);
                        string data1 = JsonConvert.SerializeObject(dtStyle);
                        return data1;
                    }
                    else
                    {
                        string RptXmlstr = RptxmlStrglobal;
                        DataSet DsXml = new DataSet();
                        string PageUrl = "";
                        DsXml.ReadXml(new XmlTextReader(new StringReader(RptXmlstr)));
                        DataTable dt = DsXml.Tables["RptTblSel"];
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            if (Convert.ToString(dt.Rows[0]["TableName"]) != "")
                            {
                                PageUrl = Convert.ToString(dt.Rows[0]["TableName"]);
                            }
                        }
                        DataTable dtStyle = new DataTable();
                        dtStyle.Columns.Add("RptStyle");
                        dtStyle.Columns.Add("TableName");
                        DataRow dr = dtStyle.NewRow();
                        dr["RptStyle"] = "Fixed";
                        dr["TableName"] = PageUrl;
                        dtStyle.Rows.Add(dr);
                        string data = JsonConvert.SerializeObject(dtStyle);
                        return data;
                    }
                }
                else
                {
                    DataTable dtStyle = new DataTable();
                    dtStyle.Columns.Add("IsBreak");
                    dtStyle.Columns.Add("Data");
                    DataRow dr = dtStyle.NewRow();
                    dr["IsBreak"] = "";
                    dr["Data"] = "Chart";
                    dtStyle.Rows.Add(dr);
                    string data = JsonConvert.SerializeObject(dtStyle);
                    return data;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetData()-" + ex.Message);
            }
        }
        public DataTable SetCriteria(string Value1, DataTable DtCriteria)
        {
            try
            {
                DataTable dtFldMstIds = new DataTable();
                string CombVal = "";
                string field = "";
                string fields = "";
                field = Value1;
                int occurences = CountStringOccurrences(Value1, "FldMstId");
                string[] arr1 = Value1.Split(Convert.ToChar("@"));
                dtFldMstIds.Columns.Add("FldMstId");
                dtFldMstIds.Columns.Add("Value1");
                dtFldMstIds.Columns.Add("Value2");
                dtFldMstIds.Columns.Add("Value3");
                string fldvalue1 = "";
                string[] arr = Value1.Split(Convert.ToChar("@"));
                for (int l = 0; l < arr.Length; l++)
                {
                    field = arr[l];
                    if (field != ",")
                    {
                        string[] arr2 = field.Split(Convert.ToChar(","));
                        for (int m = 0; m < arr2.Length; m++)
                        {
                            string field1 = arr2[m];
                            if (field1 != "")
                            {
                                int index = field1.IndexOf("=");
                                int range = field1.Length - index;
                                string colname = field1.Substring(0, index);
                                fields = field1.Substring(index + 1, range - 1);
                                DataView dvc = DtCriteria.DefaultView;
                                if (colname == "CmbOperatorValue")
                                {
                                    CombVal = fields;
                                }
                                if (colname == "FldMstId")
                                {
                                    if (CombVal == "")
                                    {
                                        dvc.RowFilter = "FldMstId= '" + fields + "'";
                                    }
                                    else
                                    {
                                        dvc.RowFilter = "FldMstId= '" + fields + "' AND CmbOperatorValue='" + CombVal + "'";
                                    }
                                }
                                if (colname == "Value1")
                                {
                                    if (Convert.ToString(dvc[0]["CmbOperatorValue"]) == "9" || Convert.ToString(dvc[0]["CmbOperatorValue"]) == "10" || Convert.ToString(dvc[0]["CmbOperatorValue"]) == "11")
                                    {
                                        dvc[0]["Value1"] = fields;
                                        dvc[0]["Value3"] = "'%" + fields + "%'";
                                        dvc[0]["Value"] = "like '%" + fields + "%'";
                                        fldvalue1 = fields;
                                    }
                                    else if (Convert.ToString(dvc[0]["CmbOperatorValue"]) == "8")
                                    {
                                        dvc[0]["Value1"] = fields;
                                        dvc[0]["Value"] = "<='" + fields + "'";
                                        dvc[0]["Value3"] = "'" + fields + "'";
                                        fldvalue1 = fields;
                                    }
                                    else if (Convert.ToString(dvc[0]["CmbOperatorValue"]) == "7")
                                    {
                                        dvc[0]["Value1"] = fields;
                                        dvc[0]["Value"] = ">='" + fields + "'";
                                        dvc[0]["Value3"] = "'" + fields + "'";
                                        fldvalue1 = fields;
                                    }
                                    else
                                    {
                                        dvc[0]["Value1"] = fields;
                                        dvc[0]["Value"] = "='" + fields + "'";
                                        dvc[0]["Value3"] = "'" + fields + "'";
                                        fldvalue1 = fields;
                                    }
                                }
                                if (colname == "Value2")
                                {
                                    if (Convert.ToString(dvc[0]["CmbOperatorValue"]) == "9" || Convert.ToString(dvc[0]["CmbOperatorValue"]) == "10" || Convert.ToString(dvc[0]["CmbOperatorValue"]) == "11")
                                    {
                                        dvc[0]["Value2"] = fields;
                                        dvc[0]["Value"] = "like '%" + fldvalue1 + "%' and '%" + fields + "%'";
                                        dvc[0]["Value3"] = "'%" + fldvalue1 + "%' and '%" + fields + "%'";
                                    }
                                    else if (Convert.ToString(DtCriteria.Rows[0]["CmbOperatorValue"]) == "8")
                                    {
                                        dvc[0]["Value2"] = fields;
                                        dvc[0]["Value"] = "<='" + fldvalue1 + "' and '" + fields + "'";
                                        dvc[0]["Value3"] = "'" + fldvalue1 + "' and '" + fields + "'";
                                    }
                                    else if (Convert.ToString(dvc[0]["CmbOperatorValue"]) == "7")
                                    {
                                        dvc[0]["Value1"] = fields;
                                        dvc[0]["Value"] = ">='" + fields + "'";
                                        dvc[0]["Value3"] = "'" + fields + "'";
                                        fldvalue1 = fields;
                                    }
                                    else
                                    {
                                        dvc[0]["Value2"] = fields;
                                        dvc[0]["Value"] = "='" + fldvalue1 + "' and '" + fields + "'";
                                        dvc[0]["Value3"] = "'" + fldvalue1 + "' and '" + fields + "'";
                                    }
                                }
                            }
                        }
                    }
                }
                return DtCriteria;
            }
            catch (Exception ex)
            {
                throw new Exception("SetCriteria()-" + ex.Message);
            }
        }
        public static int CountStringOccurrences(string text, string pattern)
        {
            try
            {
                int count = 0;
                int i = 0;
                while ((i = text.IndexOf(pattern, i)) != -1)
                {
                    i += pattern.Length;
                    count++;
                }
                return count;
            }
            catch (Exception ex)
            {
                throw new Exception("CountStringOccurrences()-" + ex.Message);
            }
        }

        [Authorize]
        [Route("GetCriteriaData")]
        [HttpGet]
        public string GetCriteriaData(string SearchText, string FldMstId, string RgtCode)
        {
            List<string> Data = new List<string>();
            clsCtrlRGT objrgt = new clsCtrlRGT(_iCommonFunc, _httpContextAccessor, _HostEnvironment);
            try
            {

                string res = objrgt.GetCriteriaData(FldMstId, RgtCode);
                return res;
            }
            catch (Exception ex)
            {
                throw new Exception("GetCriteriaData :" + ex.Message);
            }
        }
    }
}
