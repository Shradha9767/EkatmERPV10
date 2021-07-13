using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
public class ClsCtrlRptXml
{
    private readonly ICommonFunctions _iCommonFunc;
    public ClsCtrlRptXml(ICommonFunctions IcommonFunctions)
    {
        _iCommonFunc = IcommonFunctions;
    }
    public ClsRptXml GetRecord(string MyCnStr, string MyReportCode, string MyKeyId)
    {
        DataView MyDV = new DataView();
        ClsRptXml vMyRptXml = new ClsRptXml();
        var result = new ClsRptXml();
        StringBuilder sb = new StringBuilder();
        try
        {
            sb.Append("  Select * from RptXml Where 1=1  ");
            if (MyKeyId != "" && MyKeyId != null)
                sb.Append(" and RptXmlId ='" + MyKeyId + "' ");
            if (MyReportCode != "" && MyReportCode != null)
                sb.Append(" and RptCode ='" + MyReportCode + "' ");
            MyDV = _iCommonFunc.GetDataView(sb.ToString(), false, MyCnStr);
            if (MyDV.Count > 0)
            {
                ClsCtrlDB ObjCtrlDB = new ClsCtrlDB(_iCommonFunc);
                ClsCtrlTblMst ObjCtrlTbl = new ClsCtrlTblMst(_iCommonFunc);
                DataTable dt = MyDV.ToTable();
                result = (from rw in dt.AsEnumerable()
                          select new ClsRptXml
                          {
                              RecId = Convert.ToString(rw["RecId"]),
                              RptXmlId = Convert.ToString(rw["RptXmlId"]),
                              RptName = Convert.ToString(rw["RptName"]),
                              RptDesc = Convert.ToString(rw["RptDesc"]),
                              ModuleInfo = Convert.ToString(rw["ModuleInfo"]),
                              ExcelXML = Convert.ToString(rw["ExcelXML"]),
                              C1XML = Convert.ToString(rw["C1XML"]),
                              RptCode = Convert.ToString(rw["RptCode"]),
                              RptVer = Convert.ToString(rw["RptVer"]),
                              ChildRptId = Convert.ToString(rw["ChildRptId"]),
                              DBMst = ObjCtrlDB.GetRecordById(Convert.ToString(rw["DbMstId"]), MyCnStr),
                              TblMst = ObjCtrlTbl.GetRecordById(MyDV[0]["TblMstId"].ToString(), MyCnStr),
                              IsActive = MyDV[0]["IsActive"].ToString(),
                              RepClass = Convert.ToInt16(rw["RepClass"]),
                              ActionCol = Convert.ToBoolean(rw["ActionCol"]),
                              WebPageSize = Convert.ToInt16(rw["WebPageSize"]),
                              RptStyle = Convert.ToString(rw["RptStyle"]),
                              RptUniqueField = rw["UniqueField"].ToString(),
                              ChartXml = rw["ChartXml"].ToString(),
                              DisplayType = rw["DisplayType"].ToString(),
                              UseEnvCnnstr = Convert.ToBoolean(rw["UseEnvCnnstr"]),
                              ShowColHeader = Convert.ToBoolean(rw["ShowColHeader"]),
                              ShowMasterPage = Convert.ToBoolean(rw["ShowMasterPage"]),
                              ShowRptHeader = Convert.ToBoolean(rw["ShowRptHeader"]),
                              ShowGroupingBand = Convert.ToBoolean(rw["ShowGroupingBand"]),
                              AboutRpt = rw["AboutRpt"].ToString(),
                          }).FirstOrDefault();
            }
            return result;
        }
        catch (Exception ex)
        {
            //ObjBICommon.WriteApplicationLog("ClsCtrlRptXml", "GetRecordByCodeUsingMyCnnSt", ex.Message);
            Exception ex1 = new Exception(ex.Message);
            throw ex1;
        }
    }
}
