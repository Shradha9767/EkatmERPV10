//using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.XPath;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using System.Security.Claims;
using System.Globalization;
using Newtonsoft.Json;
public class clsShowMyReport
{
    private readonly ICommonFunctions _iCommonFunc;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHostingEnvironment _HostEnvironment;
    public clsShowMyReport(ICommonFunctions IcommonFunctions, IHttpContextAccessor httpContextAccessor, IHostingEnvironment HostEnvironment)
    {
        _iCommonFunc = IcommonFunctions;
        _httpContextAccessor = httpContextAccessor;
        _HostEnvironment = HostEnvironment;
    }
    enum DtaType
    {
        String_,
        Date_
    }
    private ClsRptXml objEntity = new ClsRptXml();
    Hashtable MyReportHT = new Hashtable();
    ArrayList ArrHeaderFlds = new ArrayList();
    DataTable DtProperty = new DataTable();
    string ReportCode = "";
    string ReportId = "";
    public bool GetReportInfo(string rgtCode, string RptCnnStr, string EnvCnnStr, ref string RptXml, ref ArrayList FldArray, ref DataTable DTSelCriteria, ref DataTable DTChildInfo, ref DataTable DTRptActions, ref string RptStyle, ref string RptUniqueField, ref ClsRptXml RptObject)
    {
        try
        {
            var currentUser = _httpContextAccessor.HttpContext.User;
            string RgtCnnString = Convert.ToString(currentUser.Claims.FirstOrDefault(c => c.Type == "RgtCnnStr").Value);
            string MyConnectionString = Convert.ToString(currentUser.Claims.FirstOrDefault(c => c.Type == "RgtCnnStr").Value);
            DataView DvChildRpt = new DataView();
            DataRow DRtemp;
            objEntity = new ClsRptXml();
            ClsCtrlRptXml objCls = new ClsCtrlRptXml(_iCommonFunc);
            ClsCtrlTblMst ObjCtrlTbl = new ClsCtrlTblMst(_iCommonFunc);
            DataTable DTChildReportAllCondns;
            objEntity = objCls.GetRecord(RgtCnnString, rgtCode, "");
            RptObject = objEntity;
            RptStyle = objEntity.RptStyle;
            RptUniqueField = objEntity.RptUniqueField;
            RptXml = objEntity.ExcelXML;
            DTChildInfo = ObjCtrlTbl.GetChildRptInfo(objEntity.RptXmlId, RgtCnnString);
            DataTable DTTemp = new DataTable();
            DTTemp = CreateMyTableAsSchema();
            DRtemp = DTTemp.NewRow();
            DRtemp["ExcelXML"] = objEntity.ExcelXML;
            DRtemp["C1XML"] = objEntity.C1XML;
            FldArray = ReadXMLToCreateQry(DRtemp, MyConnectionString, EnvCnnStr);
            DTChildReportAllCondns = MakeTableToStoreRuntimeCondnInfo();
            DTChildReportAllCondns.Merge(GetAllConditionsInformation(DTChildReportAllCondns.Clone(), Convert.ToString(DRtemp["C1XML"]), MyConnectionString));
            MyReportHT.Add("FilterCriteriaTable", DTChildReportAllCondns);
            MyReportHT.Add("RptDBCnnStr", MyConnectionString);
            MyReportHT.Add("ReportName", objEntity.RptName);
            if (!(DTChildReportAllCondns == null))
                MyReportHT.Add("FinalFilterDT", DTChildReportAllCondns);
            else
                return false;
            DTSelCriteria = DTChildReportAllCondns;
            string StrQry = "Select * from VW_RGT_ActionsIconsInfo where RgtCode='" + rgtCode + "'";
            DTRptActions = _iCommonFunc.GetDataTable(StrQry, false, RgtCnnString);  //.GetVSDataTableOpenConn(StrQry, false, RgtCnnString);
                                                                                    ///////////Setting session Fot BI//////////////
                                                                                    //objReportSession.MyReportHT = MyReportHT;
                                                                                    //objReportSession.ArrHeaderFlds = ArrHeaderFlds;
                                                                                    //objReportSession.ReportCode = ReportCode;
                                                                                    //objReportSession.arrColStyle = FldArray;
                                                                                    //objReportSession.Dtcriteria = DTSelCriteria;
                                                                                    //objReportSession.RptXml = RptXml;
                                                                                    //objReportSession.RptUniqueField = RptUniqueField;
                                                                                    //objReportSession.dtrptActions = DTRptActions;
                                                                                    //objReportSession.RptStyle = RptStyle;
                                                                                    //objReportSession.DtLinkInfo = DTChildInfo;
                                                                                    //objReportSession.objEntity = objEntity;

            //arrColStyle = objReportSession.arrColStyle;
            //Dtcriteria = objReportSession.Dtcriteria;
            //RptXml = objReportSession.RptXml;
            //RptUniqueField = objReportSession.RptUniqueField;
            //dtrptActions = objReportSession.dtrptActions;
            //RptStyle = objReportSession.RptStyle;
            //DtLinkInfo = objReportSession.DtLinkInfo;
            // ObjRptXml = objReportSession.objEntity;

            //string FldsArray = JsonConvert.SerializeObject(FldArray);
            //string Dtcriteria = JsonConvert.SerializeObject(DTSelCriteria);
            //string RptXmls = RptXml;
            //string RptUniqueFields = RptUniqueField;
            //string dtrptsActions = JsonConvert.SerializeObject(DTRptActions);
            //string RptStyles = RptStyle;
            //string strDTChildInfo = JsonConvert.SerializeObject(DTChildInfo);
            //string strobjEntity = JsonConvert.SerializeObject(objEntity);
            //string strMyReportHT = JsonConvert.SerializeObject(MyReportHT);
            //string ArrHeaderFld = JsonConvert.SerializeObject(ArrHeaderFlds);
            //string DTSelCriterias = JsonConvert.SerializeObject(DTSelCriteria);

            //var list = _httpContextAccessor.HttpContext.User.Claims.ToList(); //HttpContext.User.Claims.ToList();          
            //var AppEnvMasterId = currentUser.Claims.FirstOrDefault(c => c.Type == "AppEnvMasterId").Value;
            //list.Add(new Claim("Dtcriteria", Dtcriteria));
            //list.Add(new Claim("RptXmls", RptXmls));
            //list.Add(new Claim("RptUniqueFields", RptUniqueFields));
            //list.Add(new Claim("dtrptsActions", dtrptsActions));
            //list.Add(new Claim("RptStyles", RptStyles));
            //list.Add(new Claim("strDTChildInfo", strDTChildInfo));
            //list.Add(new Claim("strobjEntity", strobjEntity));
            //list.Add(new Claim("strMyReportHT", strMyReportHT));
            //list.Add(new Claim("ArrHeaderFld", ArrHeaderFld));
            //list.Add(new Claim("DTSelCriterias", DTSelCriterias));
            //string NewToken = _iCommonFunc.GenerateJSONWebToken(list.ToArray());

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    //private void AddMyCriterions(DataTable DTAllCriteria)
    //{
    //    try
    //    {
    //        for (int j = 0; j <= DTAllCriteria.Rows.Count - 1; j++)
    //        {
    //            DTAllCriteria.Rows[j]["Value"] = DTAllCriteria.Rows[j]["FldName"].ToString();
    //            DTAllCriteria.Rows[j]["Value1"] = DTAllCriteria.Rows[j]["FldName"].ToString();
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new Exception(ex.Message);
    //    }
    //}
    private DataTable GetAllConditionsInformation(DataTable DTInput, string CondnXML, string MyConStr)
    {
        try
        {
            DataSet ds = new DataSet();
            DataView DV1 = new DataView();
            ArrayList MyArrayList = new ArrayList();
            ClsFldMst ObjFld = new ClsFldMst();
            ClsCtrlFldMst ObjCtrlFld = new ClsCtrlFldMst(_iCommonFunc);
            ClsVSKeys ObjKeys = new ClsVSKeys();
            int i;
            int GrpNo = 0;
            if (CondnXML != "")
            {
            }
            else
            {
                return DTInput;
            }
            StringReader sr = new StringReader(CondnXML);
            ds.ReadXml(sr);
            if (ds.Tables.Count > 0)
                DV1 = ds.Tables[0].DefaultView;
            if (DV1.Count > 0)
            {
                DataTable dt1 = DV1.ToTable();
                if (dt1.Columns.Contains("IsVisible") == false)
                {
                    //dt1.Columns.Add("IsVisible");
                    DataColumn newColumn = new DataColumn("IsVisible", typeof(System.String));
                    newColumn.DefaultValue = "True";
                    dt1.Columns.Add(newColumn);
                    //for (int j = 0; j < dt1.Rows.Count; j++)
                    //{
                    //    dt1.Rows[j]["IsVisible"] = "True";
                    //}
                }
                if (dt1.Columns.Contains("IsReadOnly") == false)
                    dt1.Columns.Add("IsReadOnly");
                if (dt1.Columns.Contains("SessionValue1") == false)
                    dt1.Columns.Add("SessionValue1");
                if (dt1.Columns.Contains("SessionValue2") == false)
                    dt1.Columns.Add("SessionValue2");
                if (dt1.Columns.Contains("IsDefaultChecked") == false)
                    dt1.Columns.Add("IsDefaultChecked");
                if (dt1.Columns.Contains("FldQry1") == false)
                    dt1.Columns.Add("FldQry1");
                DV1 = new DataView(dt1);
                for (i = 0; i <= DV1.Count - 1; i++)
                {
                    ObjFld = new ClsFldMst();
                    ObjFld = ObjCtrlFld.GetRecordByIdUsingMyCnnStr(MyConStr, Convert.ToString(DV1[i]["FldId"]));
                    if (!(ObjFld == null))
                    {
                        DTInput.Rows.Add();
                        if (i == 0)
                            GrpNo = 1;
                        else if (Convert.ToString(DV1[i]["UCName"]) != Convert.ToString(DV1[i - 1]["UCName"]))
                            GrpNo += 1;
                        DTInput.Rows[i]["GroupNo"] = GrpNo;
                        DTInput.Rows[i]["FldMstId"] = Convert.ToString(DV1[i]["FldId"]);
                        DTInput.Rows[i]["FldName"] = ObjFld.FldName;
                        DTInput.Rows[i]["FldDesc"] = ObjFld.FldDesc;
                        DTInput.Rows[i]["IsFixed"] = Convert.ToString(DV1[i]["IsFixed"]);
                        if (DV1[i]["EnableDisableNodeChkBox"].ToString().ToLower() == "optional")
                            DTInput.Rows[i]["IsSkipped"] = "Y";
                        else
                            DTInput.Rows[i]["IsSkipped"] = "N";
                        DTInput.Rows[i]["FldDataType"] = ObjFld.FldDataType;
                        ObjKeys = new ClsVSKeys();
                        ClsCtrlVSKeys ObjCtrlVsKeys = new ClsCtrlVSKeys(_iCommonFunc);
                        ObjKeys = ObjCtrlVsKeys.GetRecordById(Convert.ToString(DV1[i]["OperatorId"]), "  AND TypeCode = 'Operator'", MyConStr);
                        if (DV1[i]["SpecifiedVal"].ToString() == "")
                            DTInput.Rows[i]["Value"] = "";
                        else
                        {
                            DTInput.Rows[i]["Value"] = ObjKeys.CodeDesc + " '" + Convert.ToString(DV1[i]["SpecifiedVal"]) + "'";
                            DTInput.Rows[i]["Value1"] = Convert.ToString(DV1[i]["SpecifiedVal"]);
                        }
                        if (DV1[i]["IsFixed"].ToString() == "Y")
                        {
                            if (DV1[i]["FldName"].ToString().ToLower() == "depot")
                            {
                                var currentUser = _httpContextAccessor.HttpContext.User;
                                DTInput.Rows[i]["Value"] = ObjKeys.CodeDesc + " '" + Convert.ToString(currentUser.Claims.FirstOrDefault(c => c.Type == "LoginBranchId").Value);//Convert.ToString(System.Web.HttpContext.Current.Session["LoginBranchId"]) + "'";
                                DTInput.Rows[i]["Value1"] = Convert.ToString(currentUser.Claims.FirstOrDefault(c => c.Type == "LoginBranchId").Value);//Convert.ToString(System.Web.HttpContext.Current.Session["LoginBranchId"]);
                            }
                        }
                        DTInput.Rows[i]["LogicalOperator"] = Convert.ToString(DV1[i]["LogicalOp"]);
                        DTInput.Rows[i]["UCOperator"] = Convert.ToString(DV1[i]["UCOperator"]);
                        DTInput.Rows[i]["CmbOperatorValue"] = Convert.ToString(DV1[i]["OperatorId"]);
                        DTInput.Rows[i]["CmbOperatorFixedOrNot"] = (DV1[i]["EnableDisableOperator"].ToString().ToLower() == "all" ? "N" : "Y");   // If operator is fixed then "Y" else "N"
                        if (DV1.Table.Columns.Contains("EnableDisableNodeChkBox"))
                            DTInput.Rows[i]["NodeChkBoxFixedOrNot"] = (DV1[i]["EnableDisableNodeChkBox"].ToString().ToLower() == "optional" ? "N" : "Y");
                        if (DV1.Table.Columns.Contains("SetCheckUncheckRuntimeFldCondn"))
                            DTInput.Rows[i]["SetNodeCheckUncheck"] = (DV1[i]["SetCheckUncheckRuntimeFldCondn"].ToString().ToLower() == "uncheck" ? "N" : "Y");
                        if (DV1.Table.Columns.Contains("SetCheckUncheckRuntimeFldCondn"))
                        {
                            if (DV1[i]["SetCheckUncheckRuntimeFldCondn"].ToString().ToLower() == "s")
                                DTInput.Rows[i]["SetNodeCheckUncheck"] = "S";
                        }
                        DTInput.Rows[i]["IsVisible"] = Convert.ToString(DV1[i]["IsVisible"]);
                        DTInput.Rows[i]["IsReadOnly"] = Convert.ToString(DV1[i]["IsReadOnly"]);
                        DTInput.Rows[i]["SessionValue1"] = Convert.ToString(DV1[i]["SessionValue1"]);
                        DTInput.Rows[i]["SessionValue2"] = Convert.ToString(DV1[i]["SessionValue2"]);
                        DTInput.Rows[i]["IsDefaultChecked"] = Convert.ToString(DV1[i]["IsDefaultChecked"]);
                    }
                }
            }
            return DTInput;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    private ArrayList ReadXMLToCreateQry(DataRow DR1, string MyConStr, string EnvCnnStr)
    {
        try
        {

            string strXML;
            System.Xml.XPath.XPathDocument doc;
            XPathNavigator nev;
            XPathNavigator Itrnev;
            XPathNodeIterator iter;
            XPathNodeIterator NewItr;
            ArrayList FieldsPropArray = new ArrayList();
            int i;
            string RuntimeConditions;
            string RuntimeFldOperator;
            string RuntimeFldOpertorCondn;
            string RuntimeFldNodeChkBoxCondn;
            string RuntimeFldSetNodeChkUnchk;
            string MyStrQuery;
            ArrayList ArrStrFlds;
            Hashtable HTSummaryDesc = new Hashtable();
            ClsRptXml ObjRpt = new ClsRptXml();
            ClsCtrlRptXml ObjCtrlRpt = new ClsCtrlRptXml(_iCommonFunc);
            ObjRpt = objEntity;
            if (Convert.ToString(ObjRpt.UseEnvCnnstr) == "True")
            {
                MyReportHT.Add("RptCnnStr", Convert.ToString(EnvCnnStr));
            }
            else
            {
                DataTable DtEnv = _iCommonFunc.GetDataTable("Select * from DbMSt where DBMstId='" + ObjRpt.DBMst.DbMstId + "'", false, EnvCnnStr);
                if (DtEnv.Rows.Count > 0)
                    MyReportHT.Add("RptCnnStr", DtEnv.Rows[0]["Connection String"].ToString());
                else
                    throw new Exception("Connection string for datasource is not found!");
            }
            if (ObjRpt.TblMst.TblType != null)
            {
                if (Convert.ToString(ObjRpt.TblMst.TblType).ToLower() != "function")
                    MyReportHT.Add("IsFunction", false);
                else
                    MyReportHT.Add("IsFunction", true);
            }
            else
                MyReportHT.Add("IsFunction", false);
            strXML = Convert.ToString(DR1["ExcelXML"]);
            StringReader sr = new StringReader(strXML);
            doc = new XPathDocument(sr);
            nev = doc.CreateNavigator();
            iter = nev.Select("TblFlds");
            Itrnev = iter.Current;
            NewItr = Itrnev.SelectDescendants(XPathNodeType.All, false);
            while (NewItr.MoveNext())
            {
                switch (NewItr.Current.Name)
                {
                    case "TblFlds":
                        {
                            ArrayList ArrFldInfoFrmFldMst = new ArrayList();
                            ClsFldMst ObjFldMstForDatatype = new ClsFldMst();
                            ClsCtrlFldMst ObjCtrlFld;
                            i = 0;
                            while (NewItr.MoveNext() & Convert.ToString(NewItr.Current.Name) != "Condn")
                            {
                                ClsFldMst ObjFld = new ClsFldMst();
                                {
                                    var withBlock = ObjFld;
                                    withBlock.FldName = NewItr.Current.Name;
                                    withBlock.FldAlias = NewItr.Current.GetAttribute("FldAlias", Itrnev.NamespaceURI);
                                    withBlock.FldDesc = NewItr.Current.GetAttribute("FldDesc", Itrnev.NamespaceURI); // FldDesc
                                    withBlock.OrderBy = NewItr.Current.GetAttribute("OrderCondn", Itrnev.NamespaceURI);
                                    withBlock.OrderSeq = NewItr.Current.GetAttribute("OrderSeq", Itrnev.NamespaceURI);
                                    withBlock.SummaryCondn = NewItr.Current.GetAttribute("Summary", Itrnev.NamespaceURI);
                                    withBlock.SummaryDesc = NewItr.Current.GetAttribute("SummaryDesc", Itrnev.NamespaceURI);
                                    withBlock.FldColWidth = NewItr.Current.GetAttribute("ColWidth", Itrnev.NamespaceURI);
                                    withBlock.FldColor = NewItr.Current.GetAttribute("SelColor", Itrnev.NamespaceURI);
                                    withBlock.ViewInHeader = (NewItr.Current.GetAttribute("ViewInHeader", Itrnev.NamespaceURI) == "True" ? true : false);
                                    ObjCtrlFld = new ClsCtrlFldMst(_iCommonFunc);
                                    ArrFldInfoFrmFldMst = ObjCtrlFld.GetAllRecordsUsingMyCnnStr(MyConStr, "TblMstId = '" + ObjRpt.TblMst.TblMstId + "' AND  FldName = '" + withBlock.FldName + "'");
                                    if (ArrFldInfoFrmFldMst.Count > 0)
                                    {
                                        ObjFldMstForDatatype = (ClsFldMst)ArrFldInfoFrmFldMst[0];
                                        withBlock.FldDataType = ObjFldMstForDatatype.FldDataType;
                                        withBlock.FldQry = ObjFldMstForDatatype.FldQry;
                                        withBlock.Format = ObjFldMstForDatatype.Format;
                                    }
                                    else
                                    {
                                        withBlock.FldDataType = "";
                                        withBlock.FldQry = "";
                                        withBlock.Format = "";
                                    }
                                    withBlock.FldFont = NewItr.Current.GetAttribute("ColFont", Itrnev.NamespaceURI);
                                    withBlock.FldFontStyle = NewItr.Current.GetAttribute("ColFontStyle", Itrnev.NamespaceURI);
                                    withBlock.FldFontSize = NewItr.Current.GetAttribute("ColFontSize", Itrnev.NamespaceURI);
                                    withBlock.GrpBy = (NewItr.Current.GetAttribute("Break", Itrnev.NamespaceURI) == "True" ? NewItr.Current.Name : "");
                                    withBlock.IsVisible = (NewItr.Current.GetAttribute("IsVisible", Itrnev.NamespaceURI) == "True" ? true : false);
                                    withBlock.IsBrk = (NewItr.Current.GetAttribute("Break", Itrnev.NamespaceURI) == "True" ? true : false);
                                }
                                FieldsPropArray.Add(ObjFld);
                            }
                            MyReportHT.Add("FieldsArray", FieldsPropArray);
                            break;
                        }
                    case "RuntimeFldCondn":
                        {
                            RuntimeConditions = NewItr.Current.Value;
                            MyReportHT.Add("RuntimeConditions", RuntimeConditions);
                            break;
                        }
                    case "CmbOpeatorForRuntimeFldCondn":
                        {
                            RuntimeFldOperator = NewItr.Current.Value;
                            RuntimeConditions = NewItr.Current.Value;
                            MyReportHT.Add("RuntimeFldOperator", RuntimeConditions);
                            break;
                        }
                    case "EnableDisableOperatorForRuntimeFldCondn":
                        {
                            RuntimeFldOpertorCondn = NewItr.Current.Value;
                            RuntimeConditions = NewItr.Current.Value;
                            MyReportHT.Add("RuntimeFldOpertorCondn", RuntimeConditions);
                            break;
                        }
                    case "CompulsoryRuntimeFldCondn":
                        {
                            RuntimeFldNodeChkBoxCondn = NewItr.Current.Value;
                            RuntimeConditions = NewItr.Current.Value;
                            MyReportHT.Add("RuntimeFldNodeChkBoxCondn", RuntimeConditions);
                            break;
                        }
                    case "SetCheckUncheckRuntimeFldCondn":
                        {
                            RuntimeFldSetNodeChkUnchk = NewItr.Current.Value;
                            RuntimeConditions = NewItr.Current.Value;
                            MyReportHT.Add("RuntimeFldSetNodeChkUnchk", RuntimeConditions);
                            break;
                        }
                    case "RptHeader":
                        {
                            MyReportHT.Add("RptHeader", NewItr.Current.Value);
                            break;
                        }
                }
            }
            MyStrQuery = "SELECT ";
            string Str3;
            ArrStrFlds = new ArrayList();
            ArrayList ArrStrSum = new ArrayList();
            ArrayList ArrStrAvg = new ArrayList();
            ArrayList ArrStrMin = new ArrayList();
            ArrayList ArrStrMax = new ArrayList();
            ArrayList ArrStrCnt = new ArrayList();
            ArrayList ArrHideCol = new ArrayList();

            for (i = 0; i <= FieldsPropArray.Count - 1; i++)
            {
                ClsFldMst ObjFld = (ClsFldMst)FieldsPropArray[i];
                Str3 = ObjFld.FldName;
                ArrStrFlds.Add(Str3);
                if (ObjFld.ViewInHeader == true)
                    ArrHeaderFlds.Add(ObjFld);
                if (ObjFld.IsVisible == false)
                    ArrHideCol.Add(ObjFld);
                if (ObjFld.SummaryCondn != "")
                {
                    switch ((ObjFld.SummaryCondn).ToUpper())
                    {
                        case "SUM":
                            {
                                ArrStrSum.Add(ObjFld.FldAlias);
                                break;
                            }
                        case "AVG":
                            {
                                ArrStrAvg.Add(ObjFld.FldAlias);
                                break;
                            }
                        case "MIN":
                            {
                                ArrStrMin.Add(ObjFld.FldAlias);
                                break;
                            }
                        case "MAX":
                            {
                                ArrStrMax.Add(ObjFld.FldAlias);
                                break;
                            }
                        case "COUNT":
                            {
                                ArrStrCnt.Add(ObjFld.FldAlias);
                                break;
                            }
                    }
                }
                if (ObjFld.SummaryDesc != "" & ObjFld.SummaryCondn != "")
                {
                    if (ObjFld.FldAlias != "")
                        HTSummaryDesc.Add(ObjFld.FldAlias, ObjFld.SummaryDesc);
                    else
                        HTSummaryDesc.Add(ObjFld.FldDesc, ObjFld.SummaryDesc);
                }
                else if (ObjFld.SummaryDesc == "" & ObjFld.SummaryCondn != "")
                {
                    if (ObjFld.FldAlias != "")
                        HTSummaryDesc.Add(ObjFld.FldAlias, "");
                    else
                        HTSummaryDesc.Add(ObjFld.FldDesc, "");
                }
            }
            MyReportHT.Add("SUM", ArrStrSum);
            MyReportHT.Add("AVG", ArrStrAvg);
            MyReportHT.Add("MIN", ArrStrMin);
            MyReportHT.Add("MAX", ArrStrMax);
            MyReportHT.Add("COUNT", ArrStrCnt);
            MyReportHT.Add("HTSummaryDesc", HTSummaryDesc);
            if (ObjRpt.TblMst.TblType != null)
            {
                if (ObjRpt.TblMst.TblType.ToLower() != "function")
                {
                    MyStrQuery = "SELECT ";
                    for (i = 0; i <= ArrStrFlds.Count - 1; i++)
                    {
                        if (i == ArrStrFlds.Count - 1)
                            MyStrQuery = MyStrQuery + " " + System.Convert.ToString(ArrStrFlds[i]);
                        else
                            MyStrQuery = MyStrQuery + " " + System.Convert.ToString(ArrStrFlds[i]) + ", ";
                    }
                    MyStrQuery = MyStrQuery + " FROM " + ObjRpt.TblMst.TblName;
                }
                else
                    MyStrQuery = "";
            }
            else
                MyStrQuery = "";
            string CntQuery = "";
            CntQuery = "SELECT COUNT(*) FROM " + ObjRpt.TblMst.TblName;
            MyReportHT.Add("CountQuery", CntQuery);
            MyReportHT.Add("Query", MyStrQuery);
            DataTable DTOrder = new DataTable();
            DTOrder.Columns.Add("FldName", typeof(String));
            DTOrder.Columns.Add("OrderBy", typeof(String));
            DTOrder.Columns.Add("OrderSeq", typeof(Int64));
            int j;
            ArrayList ArrGrp = new ArrayList();
            j = 0;
            for (i = 0; i <= FieldsPropArray.Count - 1; i++)
            {
                ClsFldMst ObjFld = new ClsFldMst();
                ObjFld = (ClsFldMst)FieldsPropArray[i];
                if (ObjFld.GrpBy != "")
                    ArrGrp.Add(ObjFld.FldAlias);
            }
            string StrGrpInfo = "";
            for (i = 0; i <= ArrGrp.Count - 1; i++)
            {
                if (i == ArrGrp.Count - 1)
                    StrGrpInfo = StrGrpInfo + " " + System.Convert.ToString(ArrGrp[i]);
                else
                    StrGrpInfo = StrGrpInfo + " " + System.Convert.ToString(ArrGrp[i]) + ", ";
            }
            j = 0;
            for (i = 0; i <= FieldsPropArray.Count - 1; i++)
            {
                ClsFldMst ObjFld = new ClsFldMst();
                ObjFld = (ClsFldMst)FieldsPropArray[i];
                if (ObjFld.OrderBy != "Unsorted" & ObjFld.OrderBy != "")
                {
                    DTOrder.Rows.Add();
                    DTOrder.Rows[j]["FldName"] = ObjFld.FldName;
                    DTOrder.Rows[j]["OrderBy"] = ObjFld.OrderBy;
                    if ((Convert.ToString(ObjFld.OrderSeq) != ""))
                        DTOrder.Rows[j]["OrderSeq"] = ObjFld.OrderSeq;
                    else
                        DTOrder.Rows[j]["OrderSeq"] = 1;
                    j += 1;
                }
            }
            DataView DvOrder = new DataView();
            DTOrder.DefaultView.Sort = "OrderSeq ASC";
            DvOrder = DTOrder.DefaultView;
            string StrOrder = "";
            if (DvOrder.Count > 0)
            {
                StrOrder = " ORDER BY ";
                for (i = 0; i <= DvOrder.Count - 1; i++)
                {
                    if (i == DTOrder.Rows.Count - 1)
                        StrOrder = StrOrder + DvOrder[i]["FldName"].ToString() + " " + DvOrder[i]["OrderBy"].ToString();
                    else
                        StrOrder = StrOrder + DvOrder[i]["FldName"].ToString() + " " + DvOrder[i]["OrderBy"].ToString() + ", ";
                }
            }
            MyReportHT.Add("OrderByClause", StrOrder);
            MyReportHT.Add("GroupByClause", StrGrpInfo);
            return FieldsPropArray;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    private DataTable MakeTableToStoreRuntimeCondnInfo()
    {
        try
        {
            DataTable DT = new DataTable();
            DataColumn DC = new DataColumn();
            DT.Columns.Add("GroupNo", typeof(string));
            DT.Columns.Add("FldMstId", typeof(string));
            DT.Columns.Add("FldName", typeof(string));
            DT.Columns.Add("FldDesc", typeof(string));
            DT.Columns.Add("IsFixed", typeof(string));
            DT.Columns.Add("IsSkipped", typeof(string));
            DT.Columns.Add("Value", typeof(string));
            DT.Columns.Add("LogicalOperator", typeof(string));
            DT.Columns.Add("CmbOperatorValue", typeof(string));
            DT.Columns.Add("CmbOperatorFixedOrNot", typeof(string));
            DT.Columns.Add("NodeChkBoxFixedOrNot", typeof(string));
            DT.Columns.Add("SetNodeCheckUncheck", typeof(string));
            DT.Columns.Add("UCOperator", typeof(string));
            DT.Columns.Add("FldDataType", typeof(string));
            DT.Columns.Add("Value1", typeof(string));
            DT.Columns.Add("Value2", typeof(string));
            DT.Columns.Add("Value3", typeof(string));
            DT.Columns.Add("IsVisible", typeof(string));
            DT.Columns.Add("IsReadOnly", typeof(string));
            DT.Columns.Add("SessionValue1", typeof(string));
            DT.Columns.Add("SessionValue2", typeof(string));
            DT.Columns.Add("IsDefaultChecked", typeof(string));
            DT.Columns.Add("FldQry1", typeof(string));
            return DT;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    private DataTable CreateMyTableAsSchema()
    {
        try
        {
            DataTable DTNew = new DataTable();
            DTNew.Columns.Add("ReportId", typeof(string));
            DTNew.Columns.Add("RptName", typeof(string));
            DTNew.Columns.Add("RptCode", typeof(string));
            DTNew.Columns.Add("RptVer", typeof(string));
            DTNew.Columns.Add("ExcelXML", typeof(string));
            DTNew.Columns.Add("C1XML", typeof(string));
            DTNew.Columns.Add("DbMstId", typeof(string));
            return DTNew;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public DataTable GetDataForReport(DataTable DTConditions, string EnvCnnStr, string Rptcnnstr, string rgtcode, ArrayList FldArray, string IsSchemaOnly) // , ByRef ArrProperties As Dictionary(Of String, Object)) As DataTable
    {
        try
        {
            DataTable ReturnDT = new DataTable();
            DataTable fldDT = new DataTable();
            string FinalQry = "";
            string TblQry = "";
            string CondnQry = "";
            string CondnHeadr = "";
            Dictionary<string, object> ArrProperties = new Dictionary<string, object>();
            for (int i = 0; i <= DTConditions.Rows.Count - 1; i++)
            {
                if ((DTConditions.Rows[i]["Value1"]) == null)
                    DTConditions.Rows[i]["Value1"] = "";
                if ((DTConditions.Rows[i]["Value"]) == null)
                    DTConditions.Rows[i]["Value"] = "";
                if ((DTConditions.Rows[i]["Value2"]) == null)
                    DTConditions.Rows[i]["Value2"] = "";
            }
            //L_MyConnectionString = MyConStr;
            //Cnnstr = MyConStr;
            StringBuilder sb = new StringBuilder();
            //SqlConnection myCnnstr = new SqlConnection(Cnnstr);
            sb.Append(" select useenvcnnstr,TblType, Query,[Connection String] from RptXML inner join DBmst on DBmst.dbmstId= RptXML.dbmstId ");
            sb.Append(" inner join TBLmst on TBLmst.TBLmstId= RptXML.TBLmstId where Rptcode  = '" + rgtcode + "'  and  IsActive='Y' ");
            fldDT = _iCommonFunc.GetDataTable(sb.ToString(), false, Rptcnnstr);
            string conn = MyReportHT["RptCnnStr"].ToString();
            //SqlConnection conn = new SqlConnection(MyReportHT["RptCnnStr"].ToString());
            // UDF
            //if (IsReportCreatedFrmUDFunction(rgtcode, MyConStr, EnvCnnStr) == true)
            string TblType = Convert.ToString(fldDT.Rows[0]["TblType"]).ToUpper();
            switch (TblType)
            {
                case "FUNCTION":
                    Hashtable HTCondn = new Hashtable();
                    Hashtable FunctionHT = new Hashtable();
                    DataTable FunctionDT = new DataTable();
                    HTCondn = CreateHTForRuntimeConditions(DTConditions);
                    try
                    {
                        FunctionHT = TakeDataTableFromFunction(ReportId, HTCondn, Rptcnnstr);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    FunctionDT = MakeDTFromUsrDefinedFunction(FunctionHT, ReportId, Rptcnnstr);
                    ReturnDT = FunctionDT;
                    break;
                case "Q":
                    TblQry = fldDT.Rows[0]["Query"].ToString();
                    StringBuilder sbQuery = new System.Text.StringBuilder();
                    if (DTConditions.Rows.Count > 0)
                    {
                        for (int i = 0; i < DTConditions.Rows.Count; i++)
                        {
                            if ((Convert.ToString(DTConditions.Rows[i]["Value1"]) != "" & Convert.ToString(DTConditions.Rows[i]["Value2"]) == ""))
                            {
                                sbQuery.Append(" Declare @" + Convert.ToString(DTConditions.Rows[i]["FldName"]) + "_L as " + Convert.ToString(DTConditions.Rows[i]["FldDataType"]) + " = " + DTConditions.Rows[i]["value3"].ToString() + "; ");
                            }
                            else if ((Convert.ToString(DTConditions.Rows[i]["Value1"]) != ""))
                            {
                                if (Convert.ToInt16(DTConditions.Rows[i]["CmbOperatorvalue"]) == 1 || Convert.ToInt16(DTConditions.Rows[i]["CmbOperatorvalue"]) == 2)
                                {
                                    string str1 = "";
                                    string str2 = "";
                                    if (DTConditions.Rows[i]["FldDataType"].ToString().ToLower() == "datetime")
                                    {
                                        str1 = DTConditions.Rows[i]["Value1"].ToString().Replace("=", "").Trim();
                                        str1 = str1.Replace("'", "");
                                        str1 = "'" + str1 + "  00:00:00.000'";
                                        str2 = DTConditions.Rows[i]["Value2"].ToString().Replace("=", "").Trim();
                                        str2 = str2.Replace("'", "");
                                        str2 = "'" + str2 + " 23:59:59.997'";
                                    }
                                    else
                                    {
                                        str1 = DTConditions.Rows[i]["Value1"].ToString().Replace("=", "").Trim();
                                        str1 = str1.Replace("'", "");
                                        str2 = DTConditions.Rows[i]["Value2"].ToString().Replace("=", "").Trim();
                                        str2 = str2.Replace("'", "");
                                        str1 = "'" + str1 + "'";
                                        str2 = "'" + str2 + "'";
                                    }
                                    sbQuery.Append(" Declare @" + Convert.ToString(DTConditions.Rows[i]["FldName"]) + "_L as " + Convert.ToString(DTConditions.Rows[i]["FldDataType"]) + " = " + str1 + "; ");
                                    sbQuery.Append(" Declare @" + Convert.ToString(DTConditions.Rows[i]["FldName"]) + "_H as " + Convert.ToString(DTConditions.Rows[i]["FldDataType"]) + " = " + str2 + "; ");
                                }
                            }
                        }
                        sbQuery.Append(" " + TblQry + " ");
                        FinalQry = sbQuery.ToString(); ;
                        ReturnDT = _iCommonFunc.GetDataTable(FinalQry, false, conn);
                    }
                    else
                    {
                        sbQuery.Append(" " + TblQry + " ");
                        FinalQry = sbQuery.ToString(); ;
                        ReturnDT = _iCommonFunc.GetDataTable(FinalQry, false, conn);
                    }
                    break;
                default:
                    //  MyReportHT.Add("Query", MyStrQuery);
                    DataView Dvtemp1 = new DataView();
                    if (DTConditions.Rows.Count > 0)
                    {
                        Dvtemp1 = DTConditions.DefaultView;
                        Dvtemp1.RowFilter = "Value1 <> ''";
                        CondnQry = CreateWhereClause(Dvtemp1.ToTable());
                    }
                    FinalQry = Convert.ToString(MyReportHT["Query"]);
                    if (IsSchemaOnly == "1" & FinalQry != "")
                    {
                        FinalQry = FinalQry.Substring(6);
                        FinalQry = "SELECT TOP 1 " + FinalQry;
                    }
                    if (CondnQry != "")
                    {
                        FinalQry += " WHERE " + CondnQry;
                    }
                    if (!(MyReportHT["OrdrBy"] == null))
                    {
                        FinalQry += " OrderBy " + MyReportHT["OrdrBy"];
                    }
                    string StrOrder = Convert.ToString(MyReportHT["OrderByClause"]);
                    FinalQry = FinalQry + StrOrder;
                    ReturnDT = _iCommonFunc.GetDataTable(FinalQry, false, conn);
                    break;
            }
            // InsertRGTAccessHistory(CondnQry, FinalQry);
            CreateHeaderForChildRpt(DTConditions, ReturnDT);
            CondnHeadr = ReadConditions(DTConditions);
            if (CondnHeadr != "")
            {
                if ((ArrProperties.ContainsKey("Text")))
                    ArrProperties["Text"] = MyReportHT["ReportName"] + " (" + CondnHeadr + ") ";
                else
                    ArrProperties.Add("Text", MyReportHT["ReportName"] + " (" + CondnHeadr + ") ");
            }
            else if ((ArrProperties.ContainsKey("Text")))
                ArrProperties["Text"] = MyReportHT["ReportName"];
            else
                ArrProperties.Add("Text", MyReportHT["ReportName"]);
            ArrProperties["Text"] = MyReportHT["ReportName"];
            if ((ArrProperties.ContainsKey("MyReportDatabaseConnectionString")))
                ArrProperties["MyReportDatabaseConnectionString"] = GetSessionValues("RgtCnnStr");
            else
                ArrProperties.Add("MyReportDatabaseConnectionString", GetSessionValues("RgtCnnStr"));
            ArrProperties.Add("Query", FinalQry);
            ArrProperties.Add("TotalColumns", ((Convert.ToString(MyReportHT["SUM"])) == null ? "" : Convert.ToString((MyReportHT["SUM"]))));
            ArrProperties.Add("AverageOn", ((Convert.ToString(MyReportHT["AVG"])) == null ? "" : Convert.ToString((MyReportHT["AVG"])))); // IIf(Not (MyChildReportHT("AVG") Is Nothing), MyChildReportHT("AVG"), "")
            ArrProperties.Add("MinimumOn", ((Convert.ToString(MyReportHT["MIN"])) == null ? "" : Convert.ToString((MyReportHT["MIN"])))); // IIf(Not (MyChildReportHT("MIN") Is Nothing), MyChildReportHT("MIN"), "")
            ArrProperties.Add("MaximumOn", ((Convert.ToString(MyReportHT["MAX"])) == null ? "" : Convert.ToString((MyReportHT["MAX"])))); // IIf(Not (MyChildReportHT("MAX") Is Nothing), MyChildReportHT("MAX"), "")
            ArrProperties.Add("CountOn", ((Convert.ToString(MyReportHT["COUNT"])) == null ? "" : Convert.ToString((MyReportHT["COUNT"])))); // IIf(Not (MyChildReportHT("COUNT") Is Nothing), MyChildReportHT("COUNT"), "")
            ArrProperties.Add("Xml", "");
            ArrProperties.Add("ConnectionString", MyReportHT["RptCnnStr"]);
            ClsCtrlTblMst ObjCtrlTbl = new ClsCtrlTblMst(_iCommonFunc);
            ArrProperties.Add("RptDtable", ObjCtrlTbl.GetChildRptInfo(ReportId, GetSessionValues("RgtCnnStr").ToString()));
            ArrProperties.Add("HashTable", null);
            ArrProperties.Add("GroupOn", MyReportHT["GroupByClause"]);
            ArrProperties.Add("HashTableSumarryDiscription", MyReportHT["HTSummaryDesc"]);
            ArrProperties.Add("ColumnStyle", MyReportHT["FieldsArray"]);
            ArrProperties.Add("Header", MyReportHT["RptHeader"]);
            ArrProperties["Text"] = MyReportHT["RptHeader"];
            ArrProperties.Add("SelectedNode", MyReportHT["RptHeader"]);
            ArrProperties.Add("DTParentFilterCriteria", MyReportHT["FinalFilterDT"]);
            ArrProperties.Add("Header2", MyReportHT["ReportSubHeader"]);
            ArrProperties.Add("ArrViewInHeader", ArrHeaderFlds);
            if (Convert.ToString(MyReportHT["Header3"]) != "")
                ArrProperties["Header2"] = Convert.ToString(MyReportHT["ReportSubHeader"]) + Convert.ToString(MyReportHT["Header3"]);
            return ReturnDT;
        }
        catch (Exception ex)
        {
            //  ObjBICommon.WriteApplicationLog("ClsShowMyReport", "Error in GetDataForReport:", ex.Message);
            throw new Exception(ex.Message);
        }
    }
    private string ReadConditions(DataTable DTInput)
    {
        try
        {
            int i;
            string MyCondnStr = "";
            ClsFldMst ObjMyCondnFields;
            ClsCtrlFldMst ObjCtrlFld = new ClsCtrlFldMst(_iCommonFunc);
            string MyTempStr = "";
            string MyHeaderCondnStr = "";
            for (i = 0; i <= DTInput.Rows.Count - 1; i++)
            {
                if (Convert.ToString(DTInput.Rows[i]["Value"]) != "")
                {
                    if (Convert.ToString(DTInput.Rows[i]["Value"]) != "")
                    {
                        if (MyCondnStr != "")
                            MyCondnStr += ",";
                        ObjMyCondnFields = new ClsFldMst();
                        //SqlConnection mycn = new SqlConnection(MyConnectionString);
                        ObjMyCondnFields = ObjCtrlFld.GetRecordByIdUsingMyCnnStr(GetSessionValues("RgtCnnStr").ToString(), Convert.ToString(DTInput.Rows[i]["FldMstId"]));
                        //ObjMyCondnFields = ObjCtrlFld.GetRecordByIdUsingMyCnnStr(mycn, Convert.ToString(DTInput.Rows[i]["FldMstId"]));
                        if (ObjMyCondnFields.FldDataType.ToLower() == "datetime")
                        {
                            // if datatype is datetime then give proper format of date value specified by user
                            MyTempStr = FormattedDate(Convert.ToString(DTInput.Rows[i]["Value"]), Convert.ToString(DTInput.Rows[i]["CmbOperatorValue"]));
                            MyCondnStr += " " + DTInput.Rows[i]["FldDesc"].ToString() + " " + MyTempStr;
                            MyHeaderCondnStr += " " + DTInput.Rows[i]["FldDesc"].ToString() + " " + MyTempStr + "\n"; ;
                        }
                        else
                        {
                            MyCondnStr += " " + DTInput.Rows[i]["FldDesc"].ToString() + " " + DTInput.Rows[i]["Value"].ToString();
                            MyHeaderCondnStr += " " + DTInput.Rows[i]["FldDesc"].ToString() + " " + DTInput.Rows[i]["Value"].ToString() + "\n"; ;
                        }
                    }
                }
            }
            MyReportHT.Remove("ReportSubHeader");
            MyReportHT.Add("ReportSubHeader", MyHeaderCondnStr);
            return MyCondnStr;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    private string FormattedDate(string RuntimeValue, string OperatorId = "")
    {
        string FormattedDate = "";
        try
        {
            int Temp1, Temp2, strlen;
            string Date1, Date2;
            string oldDate1, OldDate2;
            List<string> ArrayStr;
            Temp1 = RuntimeValue.IndexOf("'");
            Temp2 = RuntimeValue.IndexOf("'", Temp1 + 1);
            strlen = Temp2 - (Temp1 + 1);
            oldDate1 = RuntimeValue.Substring(RuntimeValue.IndexOf("'") + 1, strlen);
            Date1 = DateTime.ParseExact(oldDate1, "dd-MMM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
            RuntimeValue = RuntimeValue.Replace(oldDate1.ToString(), Date1.ToString());
            if (RuntimeValue.Length <= Temp2 + 1)
            {
                FormattedDate = RuntimeValue;
                return FormattedDate;
            }
            Temp1 = RuntimeValue.IndexOf("'", Temp2 + 1);
            if (Temp1 > 0)
            {
                Temp2 = RuntimeValue.LastIndexOf("'");
                strlen = Temp2 - (Temp1 + 1);
                OldDate2 = RuntimeValue.Substring(Temp1 + 1, strlen);
                Date2 = DateTime.ParseExact(OldDate2, "dd-MMM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                RuntimeValue = RuntimeValue.Replace(OldDate2.ToString(), Date2.ToString());
                if (Date1 == Date2 & OperatorId != "" & OperatorId != "1")
                {
                    ClsVSKeys Obj1;
                    ClsCtrlVSKeys ObjCtrlVsKeys = new ClsCtrlVSKeys(_iCommonFunc);
                    SqlConnection MyConnectionStr = new SqlConnection();
                    //MyConnectionStr = new SqlConnection(MyConnectionString);
                    Obj1 = ObjCtrlVsKeys.GetRecordById(OperatorId, "AND TypeCode = 'Operator'", GetSessionValues("RgtCnnStr").ToString());
                    RuntimeValue = Obj1.CodeDesc + " '" + Date1 + "'";
                    return RuntimeValue;
                }
            }
            RuntimeValue = RuntimeValue.Replace(oldDate1.ToString(), Date1.ToString());
            FormattedDate = RuntimeValue;
        }
        catch (Exception ex)
        {
        }
        return FormattedDate;
    }
    public void InsertRGTAccessHistory(string CondnQry, string FinalQry)
    {
        TimeSpan Duration;
        DateTime StartTime = DateTime.Now;
        try
        {
            DateTime EndTime = DateTime.Now;
            Duration = DateTime.Now.Subtract(StartTime);
            string hostName = System.Net.Dns.GetHostName();
            string ipAddress = System.Net.Dns.GetHostByName(hostName).AddressList[0].ToString();
            ClsAccshistry EntyAccshistry = new ClsAccshistry();
            EntyAccshistry.RGTCode = ReportCode;
            EntyAccshistry.Query = FinalQry;
            EntyAccshistry.DateTime = DateTime.Now;
            //if ((VSSession.UserId != ""))
            EntyAccshistry.UserName = GetSessionValues("UserName").ToString(); //VSSession.UserId;
                                                                               //if ((VSSession.UserName != ""))
                                                                               //EntyAccshistry.UserName = GetSessionValues("UserName").ToString(); //VSSession.UserName;
                                                                               //if ((GlobalLoggedInUser != ""))
                                                                               //EntyAccshistry.UserName = GetSessionValues("UserName").ToString(); //GlobalLoggedInUser;
            if ((EntyAccshistry.UserName == ""))
                EntyAccshistry.UserName = "Admin";
            EntyAccshistry.Ip = ipAddress;
            EntyAccshistry.SelectionCriteria = CondnQry; // strDisplayRuntimeConds
            EntyAccshistry.StartTime = StartTime;
            EntyAccshistry.EndTime = EndTime;
            EntyAccshistry.TimeSpan = Duration.TotalMilliseconds;
            EntyAccshistry.AcchistryId = Guid.NewGuid().ToString();
            ClsCtrlAccshistry ctrlAcchistry = new ClsCtrlAccshistry(_iCommonFunc);
            ctrlAcchistry.clsAccshistry = EntyAccshistry;
            ctrlAcchistry.AddRec(GetSessionValues("RgtCnnStr").ToString());
        }
        catch (Exception ex)
        {
            //       ObjBICommon.WriteApplicationLog("ClsShowMyReport", "Error in method InsertRGTAccessHistory:", ex.Message);
            throw new Exception(ex.Message);
        }
    }
    public object GetSessionValues(string key)
    {
        //ClsTokenDetails clsTokenDetails = new ClsTokenDetails();
        //clsTokenDetails = _httpContextAccessor.HttpContext.User.Claims.ToList<ClsTokenDetails>();
        //var df = _httpContextAccessor.HttpContext.User.Claims.ToList();
        //return df;
        var currentUser = _httpContextAccessor.HttpContext.User;
        string result = currentUser.Claims.FirstOrDefault(c => c.Type == key).Value;
        return result;
    }
    private string CreateWhereClause(DataTable DTCondn)
    {
        try
        {
            int grpNo = 1;
            int i, j;
            DataView Dv1 = new DataView();
            string StrCondn = "";
            DataTable DtGrps = new DataTable();
            bool GrpExists = true;
            string UCOperator;
            Dv1 = DTCondn.DefaultView;
            DtGrps = DTCondn.Clone();
            while (GrpExists)
            {
                Dv1.RowFilter = "GroupNo =" + grpNo;
                if (Dv1.Count > 0)
                {
                    DtGrps.ImportRow(Dv1[0].Row);
                    grpNo += 1;
                }
                else
                    GrpExists = false;
            }
            UCOperator = "";
            StrCondn = "";
            for (i = 0; i <= DtGrps.Rows.Count - 1; i++)
            {
                Dv1.RowFilter = "GroupNo =" + DtGrps.Rows[i]["GroupNo"].ToString();
                bool ConsiderCriteria = true;
                if (Dv1.Count > 0)
                {
                    if (Convert.ToString(Dv1[i]["IsSkipped"]) == "N")
                        ConsiderCriteria = true;
                    else if (Convert.ToString(Dv1[i]["IsSkipped"]) == "Y")
                    {
                        if (Convert.ToString(Dv1[i]["Value"]) != "")
                            ConsiderCriteria = true;
                        else
                            ConsiderCriteria = false;
                    }
                    if (ConsiderCriteria == true)
                    {
                        if (i != 0)
                            StrCondn += UCOperator;
                        for (j = 0; j <= Dv1.Count - 1; j++)
                        {
                            if ((Convert.ToString(Dv1[j]["Value"])) != "")
                            {
                                if ((j == 0))
                                    StrCondn += " (";
                                if ((Convert.ToString(Dv1[j]["Value1"]) != "" & Convert.ToString(Dv1[j]["Value2"]) == ""))
                                {
                                    if (Convert.ToString(Dv1[j]["SetNodeCheckUncheck"]) == "S" && Dv1[j]["FldDataType"].ToString().ToLower() == "datetime" && Convert.ToString(Dv1[j]["Value1"]) != "")
                                    {
                                        StrCondn += "Convert(date," + Convert.ToString(Dv1[j]["FldName"]) + ")" + " " + Convert.ToString(Dv1[j]["Value"]);
                                    }
                                    else
                                    {
                                        StrCondn += Convert.ToString(Dv1[j]["FldName"]) + " " + Convert.ToString(Dv1[j]["Value"]);
                                    }
                                }
                                else if ((Convert.ToString(Dv1[j]["Value1"]) != ""))
                                {
                                    if (Convert.ToInt16(Dv1[j]["CmbOperatorvalue"]) == 1)
                                    {
                                        string str1 = "";
                                        string str2 = "";
                                        if (Dv1[j]["FldDataType"].ToString().ToLower() == "datetime")
                                        {
                                            str1 = Dv1[j]["Value1"].ToString().Replace("=", "").Trim();
                                            str1 = str1.Replace("'", "");
                                            str1 = "'" + str1 + "  00:00:00.000'";
                                            str2 = Dv1[j]["Value2"].ToString().Replace("=", "").Trim();
                                            str2 = str2.Replace("'", "");
                                            str2 = "'" + str2 + " 23:59:59.997'";
                                            if (!str1.Contains("'"))
                                            {
                                                str1 = "'" + str1 + "'";
                                                str2 = "'" + str2 + "'";
                                            }
                                        }
                                        else
                                        {
                                            str1 = Dv1[j]["Value1"].ToString().Replace("=", "").Trim();
                                            str1 = str1.Replace("'", "");
                                            str2 = Dv1[j]["Value2"].ToString().Replace("=", "").Trim();
                                            str2 = str2.Replace("'", "");
                                            if (!str1.Contains("'"))
                                            {
                                                str1 = "'" + str1 + "'";
                                                str2 = "'" + str2 + "'";
                                            }
                                        }
                                        StrCondn += Convert.ToString(Dv1[j]["FldName"]) + " between " + str1 + " and " + str2;
                                    }
                                    else
                                        StrCondn += Convert.ToString(Dv1[j]["FldName"]) + " " + Convert.ToString(Dv1[j]["Value"]);
                                }
                                if (j != Dv1.Count - 1)
                                {
                                    if ((Convert.ToString(Dv1[j + 1]["Value"]) != ""))
                                        StrCondn += " " + Convert.ToString(Dv1[j]["LogicalOperator"]) + " ";
                                }
                                if ((j == Dv1.Count - 1))
                                    StrCondn += ") ";
                            }
                        }
                        UCOperator = Convert.ToString(Dv1[j - 1]["UCOperator"]);
                    }
                }
            }
            MyReportHT["WhereClause"] = StrCondn;
            return StrCondn;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    private DataTable MakeDTFromUsrDefinedFunction(Hashtable hashTableReturn, string MyChildReportId, string CnStr)
    {
        try
        {
            List<ClsFldMst> LstFldsToRemove = new List<ClsFldMst>();
            DataTable MyFunctionDT = new DataTable();
            int Cntr1, Cntr2, colcnt;
            bool DisplayFlag;
            List<string> HideFlds = new List<string>();
            ArrayList ArrFields = new ArrayList();
            DataTable DTColumnSequence = new DataTable();
            DataTable DTModifiedSeq_Final = new DataTable();
            int i;
            ArrFields = MakeFieldsArray(MyChildReportId, CnStr);
            MyFunctionDT = (DataTable)hashTableReturn["Table"];
            colcnt = 0;
            for (Cntr1 = 0; Cntr1 <= MyFunctionDT.Columns.Count - 1; Cntr1++)
            {
                DisplayFlag = false;
                for (Cntr2 = 0; Cntr2 <= ArrFields.Count - 1; Cntr2++)
                {
                    ClsFldMst ObjFld = new ClsFldMst();
                    ObjFld = (ClsFldMst)ArrFields[Cntr2];
                    if ((ObjFld.FldName).ToLower() == (MyFunctionDT.Columns[Cntr1].ColumnName).ToLower() & ObjFld.IsVisible == true)
                    {
                        MyFunctionDT.Columns[Cntr1].ColumnName = (ObjFld.FldAlias == "" ? ObjFld.FldDesc : ObjFld.FldAlias);
                        DisplayFlag = true;
                        break;
                    }
                }
                if (Convert.ToString(MyFunctionDT.Columns[Cntr1].ExtendedProperties["RuntimeColumn"]) == "true")
                {
                    DisplayFlag = true;
                    break;
                }
                if (DisplayFlag == false)
                    HideFlds.Add(MyFunctionDT.Columns[Cntr1].ColumnName);
            }
            if (Convert.ToString(MyFunctionDT.ExtendedProperties["Fixed"]) == "true")
                DTModifiedSeq_Final = MyFunctionDT;
            else
            {
                string StrCol;
                for (Cntr1 = 0; Cntr1 <= ArrFields.Count - 1; Cntr1++)
                {
                    ClsFldMst ObjFld = new ClsFldMst();
                    ObjFld = (ClsFldMst)ArrFields[Cntr1];
                    if (!(ObjFld == null))
                    {
                        StrCol = (ObjFld.FldAlias == "" ? ObjFld.FldDesc : ObjFld.FldAlias);
                        if (MyFunctionDT.Columns[StrCol] != null)
                        {
                            if (!DTModifiedSeq_Final.Columns.Contains(StrCol))
                            {
                                DTModifiedSeq_Final.Columns.Add(StrCol);
                                DTModifiedSeq_Final.Columns[StrCol].DataType = MyFunctionDT.Columns[StrCol].DataType;
                            }
                        }
                        else
                            LstFldsToRemove.Add(ObjFld);
                    }
                }
                for (i = 0; i <= LstFldsToRemove.Count - 1; i++)
                    ArrFields.Remove(LstFldsToRemove[i]);
                DTModifiedSeq_Final.Merge(MyFunctionDT);
                MyReportHT["FieldsArray"] = ArrFields;
            }
            return DTModifiedSeq_Final;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    private ArrayList MakeFieldsArray(string MyChildRptId, string MyConnecnStr)
    {
        try
        {
            XPathDocument doc;
            XPathNavigator nev;
            XPathNavigator Itrnev;
            XPathNodeIterator iter;
            XPathNodeIterator NewItr;
            ClsRptXml ObjRptXML = new ClsRptXml();
            ClsCtrlRptXml ObjCtrlRptXml = new ClsCtrlRptXml(_iCommonFunc);
            string NewXMLFilePath;
            ArrayList MyFieldsArray = new ArrayList();
            string MyXML = "";
            ClsCtrlFldMst ObjCtrlFld;
            int i;
            ObjRptXML = ObjCtrlRptXml.GetRecord(MyConnecnStr, "", MyChildRptId);
            string webRootPath = _HostEnvironment.WebRootPath;
            NewXMLFilePath = Path.Combine(webRootPath + ObjRptXML.RptName + ".xml");
            MyXML = ObjRptXML.ExcelXML;
            StringReader sr = new StringReader(MyXML);
            doc = new XPathDocument(sr);
            nev = doc.CreateNavigator();
            iter = nev.Select("TblFlds");
            Itrnev = iter.Current;
            NewItr = Itrnev.SelectDescendants(XPathNodeType.All, false);
            MyFieldsArray = new ArrayList();
            while (NewItr.MoveNext())
            {
                if (NewItr.Current.Name.ToLower() == "tblflds")
                {
                    ArrayList ArrFldInfoFrmFldMst = new ArrayList();
                    ClsFldMst ObjFldMstForDatatype = new ClsFldMst();
                    i = 0;
                    while (NewItr.MoveNext() & NewItr.Current.Name != "Condn")
                    {
                        ClsFldMst ObjFld = new ClsFldMst();
                        {
                            var withBlock = ObjFld;
                            withBlock.FldName = NewItr.Current.Name;
                            withBlock.FldAlias = NewItr.Current.GetAttribute("FldAlias", Itrnev.NamespaceURI);
                            withBlock.FldDesc = NewItr.Current.GetAttribute("FldDesc", Itrnev.NamespaceURI); // FldDesc
                            ObjCtrlFld = new ClsCtrlFldMst(_iCommonFunc);
                            //SqlConnection mycon = new SqlConnection(MyConnectionString);
                            ArrFldInfoFrmFldMst = ObjCtrlFld.GetAllRecordsUsingMyCnnStr(GetSessionValues("RgtCnnStr").ToString(), "TblMstId = '" + ObjRptXML.TblMst.TblMstId + "' AND  FldName = '" + withBlock.FldName + "'");
                            if (ArrFldInfoFrmFldMst.Count > 0)
                            {
                                ObjFldMstForDatatype = (ClsFldMst)ArrFldInfoFrmFldMst[0];
                                withBlock.FldDataType = ObjFldMstForDatatype.FldDataType;
                            }
                            else
                                withBlock.FldDataType = "";
                            withBlock.OrderBy = NewItr.Current.GetAttribute("OrderCondn", Itrnev.NamespaceURI);
                            withBlock.OrderSeq = NewItr.Current.GetAttribute("OrderSeq", Itrnev.NamespaceURI);
                            withBlock.SummaryCondn = NewItr.Current.GetAttribute("Summary", Itrnev.NamespaceURI);
                            withBlock.SummaryDesc = NewItr.Current.GetAttribute("SummaryDesc", Itrnev.NamespaceURI);
                            withBlock.FldColWidth = NewItr.Current.GetAttribute("ColWidth", Itrnev.NamespaceURI);
                            withBlock.FldColor = NewItr.Current.GetAttribute("SelColor", Itrnev.NamespaceURI);
                            withBlock.FldFont = NewItr.Current.GetAttribute("ColFont", Itrnev.NamespaceURI);
                            withBlock.FldFontStyle = NewItr.Current.GetAttribute("ColFontStyle", Itrnev.NamespaceURI);
                            withBlock.FldFontSize = NewItr.Current.GetAttribute("ColFontSize", Itrnev.NamespaceURI);
                            withBlock.GrpBy = (NewItr.Current.GetAttribute("Break", Itrnev.NamespaceURI) == "True" ? NewItr.Current.Name : "");
                            withBlock.IsVisible = (NewItr.Current.GetAttribute("IsVisible", Itrnev.NamespaceURI) == "True" ? true : false);
                            withBlock.IsBrk = (NewItr.Current.GetAttribute("Break", Itrnev.NamespaceURI) == "True" ? true : false);
                        }
                        MyFieldsArray.Add(ObjFld);
                    }
                }
            }
            return MyFieldsArray;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    private Hashtable TakeDataTableFromFunction(string MyRptId, Hashtable MyHTCondition, string CnnStr)
    {
        try
        {
            ClsFunctionInfo ObjFunction;
            ClsCtrlFunctionInfo ObjCtrlFunction;
            ArrayList ArrFunction;
            System.Reflection.Assembly ObjAssembly;
            string StrPath;
            System.Type[] AssemblyType;
            System.Reflection.PropertyInfo propertyInfo;
            int i = 0;
            Hashtable HTInfo = new Hashtable();
            object[] TempObj = new object[2];
            ClsRptXml ObjRptXML;
            ObjFunction = new ClsFunctionInfo();
            ObjCtrlFunction = new ClsCtrlFunctionInfo(_iCommonFunc, _httpContextAccessor);
            ObjRptXML = new ClsRptXml();
            ClsCtrlRptXml ObjCtrlRptXml = new ClsCtrlRptXml(_iCommonFunc);
            if (MyRptId != "")
            {
                ObjRptXML = ObjCtrlRptXml.GetRecord(CnnStr, "", MyRptId);
                TempObj[1] = false;
                if (Convert.ToString(ObjRptXML.UseEnvCnnstr) == "True")
                {
                    TempObj[0] = Convert.ToString(GetSessionValues("EkatmDbCnnStr"));
                }
                else
                {
                    DataTable DtEnv = _iCommonFunc.GetDataTable("Select * from DbMSt where DBMstId='" + ObjRptXML.DBMst.DbMstId + "'", false, GetSessionValues("EkatmDbCnnStr").ToString());
                    if (DtEnv.Rows.Count > 0)
                        TempObj[0] = DtEnv.Rows[0]["Connection String"].ToString();
                    else
                    {
                        throw new Exception("Connection string for datasource is not found!");
                    }
                }
            }
            //ObjCtrlFunction.strFarziConnString = CnnStr;
            ArrFunction = ObjCtrlFunction.GetAllRecordsUsingMyCnnStr(CnnStr, " TblMstId = '" + ObjRptXML.TblMst.TblMstId + "'");
            Hashtable HT2;
            // paas condition hash table to dll
            HT2 = MyHTCondition;
            if (ArrFunction.Count > 0)
            {
                ObjFunction = (ClsFunctionInfo)ArrFunction[0];
                string webRootPath = _HostEnvironment.WebRootPath;
                StrPath = Path.Combine(webRootPath + ObjFunction.AssemblyName + ".dll");
                //StrPath = System.Web.HttpContext.Current.Server.MapPath(ObjFunction.AssemblyName + ".dll");
                try
                {
                    ObjAssembly = System.Reflection.Assembly.LoadFrom(StrPath);
                }
                catch (Exception ex)
                {
                    throw new Exception("DLL not found for UDF");
                }
                AssemblyType = ObjAssembly.GetExportedTypes();
                object instance = Activator.CreateInstance(AssemblyType[0]);
                // string key;
                if (!(HT2 == null))
                {
                    bool SelCriteriaFlag = false;
                    propertyInfo = AssemblyType[0].GetProperty("SelectionCriteria");
                    if (propertyInfo != null)
                    {
                        propertyInfo.SetValue(instance, DtProperty, null/* TODO Change to default(_) if this is not a reference type */);
                        SelCriteriaFlag = true;
                    }
                    foreach (var key in HT2.Keys)
                    {
                        string[] str = HT2[key].ToString().Split('|');
                        if (str.Length == 1)
                        {
                            propertyInfo = AssemblyType[0].GetProperty(Convert.ToString(key));
                            if (propertyInfo == null)
                                break;
                            if (propertyInfo.PropertyType.Name.ToString().ToLower() == "datetime")
                            {
                                if (SelCriteriaFlag == false)
                                    propertyInfo.SetValue(instance, HT2[key], null/* TODO Change to default(_) if this is not a reference type */);
                            }
                            else
                                propertyInfo.SetValue(instance, HT2[key], null/* TODO Change to default(_) if this is not a reference type */);
                        }
                        else
                        {
                            propertyInfo = AssemblyType[0].GetProperty(Convert.ToString(key));
                            if (propertyInfo == null)
                                break;
                            string StrVal = "";
                            for (int val = 0; val <= str.Length - 1; val++)
                            {
                                // Get oprator
                                string Opr = GetOperatorfromString(Convert.ToString(str[val]));
                                if (val == 0)
                                    str[0] = str[0].Replace(Opr, "IN(");
                                if (val == str.Length - 1)
                                    StrVal += str[val] + ")";
                                else
                                    StrVal += str[val] + ",";
                                val += 1;
                                StrVal = StrVal.Replace(Opr, "");
                            }
                            StrVal = StrVal.Trim();
                            propertyInfo.SetValue(instance, StrVal, null);
                        }
                    }
                }
                System.Reflection.MethodInfo[] AssemblyMembers = AssemblyType[i].GetMethods();
                int j;
                for (j = 0; j <= AssemblyMembers.Length - 1; j++)
                {
                    if (AssemblyMembers[j].ReturnType.Name.ToString() == "Hashtable")
                    {
                        HTInfo = (Hashtable)AssemblyMembers[j].Invoke(instance, TempObj);
                        break;
                    }
                }
                return HTInfo;
            }
            else
                return null/* TODO Change to default(_) if this is not a reference type */;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
            // return null/* TODO Change to default(_) if this is not a reference type */;
        }
    }
    private string GetOperatorfromString(string val)
    {
        string result = "";
        try
        {
            if (val.Contains(">="))
                result = ">=";
            else if (val.Contains("<="))
                result = "<=";
            else if (val.Contains("<>"))
                result = "<>";
            else if (val.Contains("="))
                result = "=";
            else if (val.Contains("<"))
                result = "<";
            else if (val.Contains(">"))
                result = ">";
            return result;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    private Hashtable CreateHTForRuntimeConditions(DataTable DTInput)
    {
        try
        {
            int i;
            Hashtable HTOutput = new Hashtable();
            DtProperty = new DataTable();
            DtProperty.Columns.Add("Property");
            DtProperty.Columns.Add("Value1");
            DtProperty.Columns.Add("Value2");
            DtProperty.Columns.Add("Operator");
            DtProperty.Columns.Add("OperatorValue");
            DataRow dr;
            Dictionary<string, string> DictCond = new Dictionary<string, string>();
            var CurrentUser = _httpContextAccessor.HttpContext.User;
            string Ekatmcnn = Convert.ToString(CurrentUser.Claims.FirstOrDefault(c => c.Type == "EkatmDbCnnStr").Value);
            string RptConn = Convert.ToString(CurrentUser.Claims.FirstOrDefault(c => c.Type == "RgtCnnStr").Value);
            DictCond = FillCondArray();
            if (!(DTInput == null))
            {
                ClsCtrlRptXml objCls = new ClsCtrlRptXml(_iCommonFunc);
                objEntity = objCls.GetRecord(RptConn, ReportCode, "");
                ReportId = objEntity.RptXmlId;
                for (i = 0; i <= DTInput.Rows.Count - 1; i++)
                {
                    if (Convert.ToString(DTInput.Rows[i]["IsSkipped"]) == "N")
                    {
                        if (Convert.ToString(DTInput.Rows[i]["Value"]) != "")
                            HTOutput.Add(Convert.ToString(DTInput.Rows[i]["FldName"]), Convert.ToString(DTInput.Rows[i]["FldName"]) + " " + Convert.ToString(DTInput.Rows[i]["Value"]));
                        ClsFldMst ObjFld = new ClsFldMst();
                        ClsCtrlFldMst ObjCtrlFld = new ClsCtrlFldMst(_iCommonFunc);
                        ArrayList Arr1 = new ArrayList();
                        //SqlConnection mycon = new SqlConnection(Cnnstr);
                        Arr1 = ObjCtrlFld.GetAllRecordsUsingMyCnnStr(GetSessionValues("RgtCnnStr").ToString(), " FldMstId = '" + Convert.ToString(DTInput.Rows[i]["FldMstId"]) + "' AND TblMstId ='" + objEntity.TblMst.TblMstId + "'"); ;
                        if (Arr1.Count > 0)
                        {
                            if (Arr1[0] != null)
                            {
                                ObjFld = (ClsFldMst)Arr1[0];
                                if (ObjFld.FldDataType.ToLower() == "datetime")
                                {
                                    dr = DtProperty.NewRow();
                                    dr["Property"] = ObjFld.FldName;
                                    DateTime Val1;
                                    DateTime val2 = new DateTime();
                                    if (DTInput.Rows[i]["Value1"].ToString() == "")
                                    {
                                        DTInput.Rows[i]["Value1"] = DateTime.Now.ToString("dd-MMM-yyyy");
                                        DTInput.Rows[i]["Value2"] = DateTime.Now.ToString("dd-MMM-yyyy");
                                    }
                                    Val1 = Convert.ToDateTime(DTInput.Rows[i]["Value1"].ToString().Replace("'", ""));
                                    if ((DTInput.Rows[i]["Value2"].ToString() != ""))
                                        val2 = Convert.ToDateTime(DTInput.Rows[i]["Value2"].ToString().Replace("'", ""));
                                    dr["Value1"] = Val1.Date.ToShortDateString() + " 00:00";
                                    dr["Value2"] = val2.Date.ToShortDateString() + " 23:59";
                                    dr["Operator"] = DTInput.Rows[i]["CmbOperatorValue"];
                                    dr["OperatorValue"] = DictCond[Convert.ToString(DTInput.Rows[i]["CmbOperatorValue"])];
                                    DtProperty.Rows.Add(dr);
                                }
                                else
                                {
                                    dr = DtProperty.NewRow();
                                    dr["Property"] = ObjFld.FldName;
                                    dr["Value1"] = DTInput.Rows[i]["Value1"].ToString().Replace("'", "");
                                    dr["Value2"] = DTInput.Rows[i]["Value2"].ToString().Replace("'", "");
                                    dr["Operator"] = DTInput.Rows[i]["CmbOperatorValue"];
                                    dr["OperatorValue"] = DictCond[Convert.ToString(DTInput.Rows[i]["CmbOperatorValue"])];
                                    DtProperty.Rows.Add(dr);
                                }
                            }
                            else
                            {
                                throw new Exception("Field not fount!");
                            }
                        }
                    }
                }
            }
            return HTOutput;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    private Dictionary<string, string> FillCondArray()
    {
        try
        {
            Dictionary<string, string> ArrCondn = new Dictionary<string, string>();
            ArrCondn.Add("1", "between");
            ArrCondn.Add("2", "not between");
            ArrCondn.Add("3", "=");
            ArrCondn.Add("4", "<>");
            ArrCondn.Add("5", ">");
            ArrCondn.Add("6", "<");
            ArrCondn.Add("7", ">=");
            ArrCondn.Add("8", "<=");
            return ArrCondn;
        }
        catch (Exception ex)
        {
            string err = "FillCondDrodown()-" + ex.Message;
            throw new Exception(err);
        }
    }
    private void CreateHeaderForChildRpt(DataTable Dt, DataTable DTOutput)
    {
        try
        {
            int cntView;
            string TempHeader3Str = "";
            string Header3Str = "";
            //ArrHeaderFlds = objReportSession.ArrHeaderFlds; //get from claim
            if (DTOutput.Rows.Count > 0)
            {
                for (cntView = 0; cntView <= ArrHeaderFlds.Count - 1; cntView++)
                {
                    ClsFldMst ObjFld = new ClsFldMst();
                    ObjFld = (ClsFldMst)ArrHeaderFlds[cntView];
                    int r;
                    bool @bool = false;
                    for (r = 0; r <= Dt.Rows.Count - 1; r++)
                    {
                        if (Convert.ToString(Dt.Rows[r]["FldName"]) == Convert.ToString(ObjFld.FldName) && Convert.ToString(Dt.Rows[r]["IsSkipped"]) == "N")
                        {
                            @bool = true;
                            break;
                        }
                    }
                    if (@bool == false)
                    {
                        if ((DTOutput.Columns[ObjFld.FldAlias] == null) == false)
                        {
                            if (TempHeader3Str == "")
                            {
                                if (DTOutput.Rows.Count > 1)
                                    TempHeader3Str = ObjFld.FldAlias + " = '" + Convert.ToString(DTOutput.Rows[1]["" + ObjFld.FldAlias + ""]) + "'";
                                else
                                    TempHeader3Str = ObjFld.FldAlias + " = '" + Convert.ToString(DTOutput.Rows[0]["" + ObjFld.FldAlias + ""]) + "'";
                            }
                            else if (DTOutput.Rows.Count > 1)
                                TempHeader3Str = TempHeader3Str + ", " + ObjFld.FldAlias + " = '" + Convert.ToString(DTOutput.Rows[1]["" + ObjFld.FldAlias + ""]) + "'";
                            else
                                TempHeader3Str = TempHeader3Str + ", " + ObjFld.FldAlias + " = '" + Convert.ToString(DTOutput.Rows[0]["" + ObjFld.FldAlias + ""]) + "'";
                        }
                    }
                }
                Header3Str = TempHeader3Str;
            }
            MyReportHT["Header3"] = Header3Str;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
