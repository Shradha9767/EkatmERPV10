using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;

public class clsCtrlRGT
{
    private readonly ICommonFunctions _iCommonFunc;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHostingEnvironment _HostEnvironment;
    public clsCtrlRGT(ICommonFunctions IcommonFunctions, IHttpContextAccessor httpContextAccessor, IHostingEnvironment HostEnvironment)
    {
        _iCommonFunc = IcommonFunctions;
        _httpContextAccessor = httpContextAccessor;
        _HostEnvironment = HostEnvironment;
    }
    DataTable Dtcriteria = new DataTable();
    ArrayList arrColStyle = new ArrayList();
    ClsRptXml ObjRptXml = new ClsRptXml();
    string RptXml = "";
    string global = "";
    public string GetCriteria(string RptCode, string linkFlag, string linkCol, string ParentRgtCode, string RowId, string MyConnstr, string RgtConnstr, out string RptName, out string NewToken)
    {
        try
        {
            var currentUser = _httpContextAccessor.HttpContext.User;
            string MyconnRgt = Convert.ToString(currentUser.Claims.FirstOrDefault(c => c.Type == "RgtCnnStr").Value);// Get from claim //currentUser.Claims.FirstOrDefault(c => c.Type == "name").Value;
            RptName = null;
            NewToken = null;
            string RptStyle = "";
            string RptUniqueField = "";
            DataTable DtLinkInfo = new DataTable();
            DataTable dtrptActions = new DataTable();
            clsShowMyReport objcls = new clsShowMyReport(_iCommonFunc, _httpContextAccessor,_HostEnvironment);
            objcls.GetReportInfo(RptCode, RgtConnstr, MyConnstr, ref RptXml, ref arrColStyle, ref Dtcriteria, ref DtLinkInfo, ref dtrptActions, ref RptStyle, ref RptUniqueField, ref ObjRptXml);
            DataSet DsXml = new DataSet();
            DsXml.ReadXml(new XmlTextReader(new StringReader(RptXml)));
            DataTable dt = DsXml.Tables["RptNew"];
            StringBuilder sb = new StringBuilder();
            sb.Append(" select rptxml.rptName, isnull( BCN.RptName ,'') as CustomName  from rptxml left outer join BICustomname BCN on rptxml.rptxmlid= BCN.rptxmlid where rptcode='" + Convert.ToString(dt.Rows[0]["RptCode"]) + "' ");
            DataTable DtCustom = _iCommonFunc.GetDataTable(sb.ToString(), false, MyconnRgt);
            if (Convert.ToString(DtCustom.Rows[0]["CustomName"]) != "")
            {
                RptName = Convert.ToString(DtCustom.Rows[0]["CustomName"]);
            }
            RptName = Convert.ToString(dt.Rows[0]["RptDesc"]);
            if (linkFlag == "true")
            {
                string criteria = JsonConvert.SerializeObject(Dtcriteria);
                return criteria;
            }
            else
            {
                if (Dtcriteria.Rows.Count > 0)
                {
                    DataTable dtFldmst = Dtcriteria;
                    dtFldmst.Columns.Add("FldQry");
                    StringBuilder sbddlHtml = new StringBuilder();
                    for (int i = 0; i < dtFldmst.Rows.Count; i++)
                    {
                        #region for Add Alis in selection criteria
                        for (int ii = 0; ii < arrColStyle.Count; ii++)
                        {
                            ClsFldMst obj1 = new ClsFldMst();
                            obj1 = (ClsFldMst)arrColStyle[ii];
                            if (dtFldmst.Rows[i]["FldName"].ToString() == obj1.FldName)
                                dtFldmst.Rows[i]["FldDesc"] = obj1.FldAlias;
                        }
                        #endregion
                        string SetNodeCheckUncheck = dtFldmst.Rows[i]["SetNodeCheckUncheck"].ToString();
                        if (SetNodeCheckUncheck != "S")
                        {
                            string StrQry = "SELECT FldQry FROM FldMst WHERE FldMstId='" + Convert.ToString(dtFldmst.Rows[i]["FldMstId"]) + "'";
                            DataTable fldQry = _iCommonFunc.GetDataTable(StrQry, false, MyconnRgt);
                            if (fldQry.Rows.Count > 0 && !string.IsNullOrEmpty(fldQry.Rows[0]["FldQry"].ToString()))
                            {
                                sbddlHtml = new StringBuilder();
                                sbddlHtml.Append("<select id='FldQry" + Convert.ToString(dtFldmst.Rows[i]["FldMstId"]) + "'  class='form-control searchInput'>");
                                if (dtFldmst.Rows[i]["SetNodeCheckUncheck"].ToString() == "Y")
                                {
                                    sbddlHtml.Append(GetCriteriaData(Convert.ToString(dtFldmst.Rows[i]["FldMstId"]), RptCode));
                                }
                                sbddlHtml.Append("</select>");
                            }
                            else
                            {
                                sbddlHtml.Clear();
                            }
                        }
                        else
                        {
                            var SessionValue1 = dtFldmst.Rows[i]["SessionValue1"].ToString();
                            var SessionValue2 = dtFldmst.Rows[i]["SessionValue2"].ToString();
                            BISessionValuesAPIController BISessVal = new BISessionValuesAPIController(_iCommonFunc);
                            DataTable dtSessVal1 = new DataTable();
                            DataTable dtSessVal2 = new DataTable();
                            if (SessionValue1 != "")
                            {
                                dtSessVal1 = BISessVal.GetBISessionValue(SessionValue1);
                                dtSessVal2 = BISessVal.GetBISessionValue(SessionValue2);
                                if (dtSessVal1.Rows.Count > 1)
                                {
                                    sbddlHtml = new StringBuilder();
                                    sbddlHtml.Append("<select id='FldQry" + Convert.ToString(dtFldmst.Rows[i]["FldMstId"]) + "'  class='form-control searchInput'> ");
                                    for (int j = 0; j < dtSessVal1.Rows.Count; j++)
                                    {
                                        sbddlHtml.Append("<option value = '" + Convert.ToString(dtSessVal1.Rows[j][0]) + " '>" + Convert.ToString(dtSessVal1.Rows[j][0]) + " </option>");
                                    }
                                    sbddlHtml.Append("</select>");
                                    if (dtSessVal2.Rows.Count > 1)
                                    {
                                        sbddlHtml = new StringBuilder();
                                        sbddlHtml.Append("<select id='FldQry" + Convert.ToString(dtFldmst.Rows[i]["FldMstId"]) + "'  class='form-control searchInput'>");
                                        for (int j = 0; j < dtSessVal2.Rows.Count; j++)
                                        {
                                            sbddlHtml.Append("<option value = '" + Convert.ToString(dtSessVal2.Rows[j][0]) + " '>" + Convert.ToString(dtSessVal2.Rows[j][0]) + " </option>");
                                        }
                                        sbddlHtml.Append("</select>");
                                    }
                                    else if (dtSessVal2.Rows.Count == 1)
                                    {
                                        dtFldmst.Rows[i]["Value2"] = Convert.ToString(dtSessVal2.Rows[0][0]);
                                    }
                                }
                                else if (dtSessVal1.Rows.Count == 1)
                                {
                                    dtFldmst.Rows[i]["Value1"] = Convert.ToString(dtSessVal1.Rows[0][0]);
                                    if (dtSessVal2.Rows.Count > 1)
                                    {
                                        sbddlHtml = new StringBuilder();
                                        sbddlHtml.Append("<select id='FldQry" + Convert.ToString(dtFldmst.Rows[i]["FldMstId"]) + "'  class='form-control searchInput'>");
                                        for (int j = 0; j < dtSessVal2.Rows.Count; j++)
                                        {
                                            sb.Append("<option value = '" + Convert.ToString(dtSessVal2.Rows[j][0]) + " '>" + Convert.ToString(dtSessVal2.Rows[j][0]) + " </option>");
                                        }
                                        sb.Append("</select>");
                                    }
                                    else if (dtSessVal2.Rows.Count == 1)
                                    {
                                        dtFldmst.Rows[i]["Value2"] = Convert.ToString(dtSessVal2.Rows[0][0]);
                                    }
                                }
                            }
                        }
                        dtFldmst.Rows[i]["FldQry"] = sbddlHtml.ToString();
                    }
                    string criteria = JsonConvert.SerializeObject(dtFldmst);

                    return criteria;
                }
                else
                {
                    return "";
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public string GetActions(string rptcode, string MyconnRgt)
    {
        try
        {
            DataTable dtRptactions = new DataTable();
            StringBuilder sb = new StringBuilder();
            sb.Append(" Select Distinct RgtCode,ActionCode,ActionDesc,IconCode,Page,Visible from RgtActions ");
            sb.Append(" WHERE Rgtcode='" + rptcode + "' ");
            string actionJson = "";
            dtRptactions = _iCommonFunc.GetDataTable(sb.ToString(), false, MyconnRgt); //objcomm.GetVSDataViewOpenConn(actions, false, MyconnRgt);
            if (dtRptactions.Rows.Count > 0)
            {
                actionJson = JsonConvert.SerializeObject(dtRptactions);
            }
            return actionJson;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public string GetCriteriaData(string FldMstId, string RptCode)
    {
        List<string> Data = new List<string>();
        try
        {
            var currentUser = _httpContextAccessor.HttpContext.User;
            string MyconnRgt = Convert.ToString(currentUser.Claims.FirstOrDefault(c => c.Type == "RgtCnnStr").Value);
            string ERPCnnStr = Convert.ToString(currentUser.Claims.FirstOrDefault(c => c.Type == "EkatmDbCnnStr").Value);
            string MyRptConn = "";
            StringBuilder sb = new StringBuilder();
            sb.Append(" select UseEnvCnnstr,TblType, Query,[Connection String],DBmst.DbMstId from RptXML  ");
            sb.Append(" inner join DBmst on DBmst.dbmstId= RptXML.dbmstId ");
            sb.Append(" inner join TBLmst on TBLmst.TBLmstId= RptXML.TBLmstId ");
            sb.Append(" where Rptcode  = '" + RptCode + "'  and  IsActive='Y' ");
            DataTable fldDT = _iCommonFunc.GetDataTable(sb.ToString(), false, MyconnRgt);
            if (Convert.ToString(fldDT.Rows[0]["UseEnvCnnstr"]) == "True")
                MyRptConn = Convert.ToString(ERPCnnStr);
            else
            {
                DataTable DtEnv = _iCommonFunc.GetDataTable("Select * from DbMSt where DBMstId='" + Convert.ToString(fldDT.Rows[0]["DbMstId"]) + "'", false, ERPCnnStr);
                if (DtEnv.Rows.Count > 0)
                    MyRptConn = DtEnv.Rows[0]["Connection String"].ToString();
                else
                    throw new Exception("Connection string for datasource is not found!");
            }
         
            string FldQry = "";
            string strFld = "";
            StringBuilder SbHtml = new StringBuilder();
            ClsFldMst objfldmst = new ClsFldMst();
            DataView dvFldMst = new DataView();
            DataView dvFldQry = new DataView();
            DataTable dtFldQry = new DataTable();
            string StrQry = "";
            StrQry = "SELECT FldQry FROM FldMst WHERE FldMstId='" + FldMstId + "'";
            dvFldMst = _iCommonFunc.GetDataView(StrQry, false, MyconnRgt);
            if (dvFldMst.Count > 0)
            {
                if (Convert.ToString(dvFldMst[0]["FldQry"]) != "")
                {
                    strFld = Convert.ToString(dvFldMst[0]["FldQry"]);
                    FldQry = strFld;
                    if (!strFld.ToUpperInvariant().Contains("ORDER"))
                    {
                        strFld += " Order By ";
                        string FldQuery = FldQry.ToUpper();
                        string[] arrYSeries = FldQuery.Split(Convert.ToChar(" "));
                        int IndxSelect = Array.IndexOf(arrYSeries, "SELECT");
                        int IndxFROM = Array.IndexOf(arrYSeries, "FROM");
                        for (int i = IndxSelect + 1; i < IndxFROM; i++)
                        {
                            if (arrYSeries[i].ToString() == "AS")
                            {
                                i += 2;
                                strFld += ",";
                            }
                            if (arrYSeries[i].ToString() != "DISTINCT")
                            {
                                strFld += " " + arrYSeries[i] + " ";
                            }
                        }
                        strFld = strFld.Remove(strFld.Length - 1);
                    }
                    dtFldQry = _iCommonFunc.GetDataTable(strFld, false, MyRptConn);
                    if (dtFldQry.Rows.Count > 0)
                    {
                        for (int j = 0; j < dtFldQry.Rows.Count; j++)
                        {
                            if (dtFldQry.Columns.Count == 1)
                            {
                                SbHtml.Append("<option value = '" + Convert.ToString(dtFldQry.Rows[j][0]) + "'>" + Convert.ToString(dtFldQry.Rows[j][0]) + " </option>");
                            }
                            else
                            {
                                SbHtml.Append("<option value = '" + Convert.ToString(dtFldQry.Rows[j][0]) + "'>" + Convert.ToString(dtFldQry.Rows[j][1]) + " </option>");
                            }
                        }
                    }
                }
            }
            return SbHtml.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
