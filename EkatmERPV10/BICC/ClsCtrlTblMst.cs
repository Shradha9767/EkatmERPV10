using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
public class ClsCtrlTblMst
{
    private readonly ICommonFunctions _iCommonFunc;
    public ClsCtrlTblMst(ICommonFunctions IcommonFunctions)
    {
        _iCommonFunc = IcommonFunctions;
    }
    public ClsTblMst GetRecordById(string MyKeyId, string MyCnnStr)
    {
        string StrQuery = null;
        ClsCtrlDB ObjCtrlDBMst = new ClsCtrlDB(_iCommonFunc);
        DataTable dt = new DataTable();
        var result = new ClsTblMst();
        try
        {
            StrQuery = "Select * from TblMst Where TblMstId = '" + MyKeyId + "'";
            dt = _iCommonFunc.GetDataTable(StrQuery, false, MyCnnStr);
          
            if (dt.Rows.Count > 0)
            {
                result = (from rw in dt.AsEnumerable()
                          select new ClsTblMst
                          {
                              RecId = rw["RecId"].ToString(),
                              TblMstId = rw["TblMstId"].ToString(),
                              TblName = rw["TblName"].ToString(),
                              TblDesc = rw["TblDesc"].ToString(),
                              TblType = rw["TblType"].ToString(),
                              DBMst = ObjCtrlDBMst.GetRecordById(rw["DBMstId"].ToString(), MyCnnStr),
                          }).FirstOrDefault();
            }
            return result;
        }
        catch (Exception ex)
        {
            //ObjBICommon.WriteApplicationLog("ClsCtrlTblMst", "GetRecordById", ex.Message);           
            throw new Exception(ex.Message);
        }
    }
    public DataTable GetChildRptInfo(string PRptId, string MyConn)
    {
        try
        {
            DataTable DT1 = new DataTable();
            string StrQ = " SELECT dbo.LinkRpts.ParentRptId, dbo.RptXml.RptName, dbo.RptXml.RptCode, dbo.RptXml.RptVer, dbo.RptXml.ExcelXML, dbo.RptXml.C1XML, " + " dbo.RptXml.DbMstId, dbo.LinkRpts.FixedType, dbo.LinkRpts.ChildRptId, dbo.LinkRpts.RuntimeType, dbo.LinkRpts.ColumnValueType,  " + " dbo.LinkRpts.ParentSelCriteriaType FROM  dbo.LinkRpts INNER JOIN " + " dbo.RptXml ON dbo.LinkRpts.ChildRptId = dbo.RptXml.RptXmlId " + " Where LinkRpts.ParentRptId = '" + PRptId + "' ";
            DT1 = _iCommonFunc.GetDataTable(StrQ, false, MyConn);
            return DT1;
        }
        catch (Exception ex)
        {
            // ObjBICommon.WriteApplicationLog("ClsCtrlTblMst", "GetChildRptInfo", ex.Message);
            Exception ex1 = new Exception(ex.Message);
            throw ex1;
        }
    }
}
