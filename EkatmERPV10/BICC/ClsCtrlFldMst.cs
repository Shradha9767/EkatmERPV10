using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
public class ClsCtrlFldMst
{
    private readonly ICommonFunctions _iCommonFunc;
    public ClsCtrlFldMst(ICommonFunctions IcommonFunctions)
    {
        _iCommonFunc = IcommonFunctions;
    }
    public ClsFldMst GetRecordByIdUsingMyCnnStr(string MyCnStr, string MyKeyId)
    {
        DataView MyDV = new DataView();
        string StrQuery = null;
        var result = new ClsFldMst();
        try
        {
            StrQuery = "Select * from FldMst Where FldMstId = '" + MyKeyId + "'";
            MyDV = _iCommonFunc.GetDataView(StrQuery, false, MyCnStr);
            if (MyDV.Count > 0)
            {
                ClsCtrlTblMst ObjCtrlTbl = new ClsCtrlTblMst(_iCommonFunc);
                result = (from rw in MyDV.ToTable().AsEnumerable()
                          select new ClsFldMst
                          {
                              RecId = rw["RecId"].ToString(),
                              FldMstId = rw["FldMstId"].ToString(),
                              TblMst = ObjCtrlTbl.GetRecordById(rw["TblMstId"].ToString(), MyCnStr),
                              FldName = rw["FldName"].ToString(),
                              FldDesc = rw["FldDesc"].ToString(),
                              FldQry = rw["FldQry"].ToString(),
                              FldDataType = rw["FldDataType"].ToString(),
                              FldColor = rw["FldColor"].ToString(),
                              FldFont = rw["FldFont"].ToString(),
                              FldFontStyle = rw["FldFontStyle"].ToString(),
                              FldFontSize = rw["FldFontSize"].ToString(),
                              FldColWidth = rw["FldColWidth"].ToString(),
                          }).FirstOrDefault();
            }
            return result;
        }
        catch (SqlException ex)
        {
            // ObjBICommon.WriteApplicationLog("ClsCtrlFldMst", "GetRecordByIdUsingMyCnnStr", ex.Message);
            throw new Exception(ex.Message);
        }
    }
    public ArrayList GetAllRecordsUsingMyCnnStr(string MyCnStr, string StrCondtion)
    {
        string StrQuery = null;
        DataView MyDV = new DataView();
        int i = 0;
        ArrayList ArrList = new ArrayList();
        try
        {
            
            StrQuery = "Select * from FldMst";
            if (!string.IsNullOrEmpty(StrCondtion))
            {
                StrQuery = StrQuery + " Where " + StrCondtion;
            }
            MyDV = _iCommonFunc.GetDataView(StrQuery, false, MyCnStr);
            ClsCtrlTblMst ObjCtrlTbl = new ClsCtrlTblMst(_iCommonFunc);
            //for (i = 0; i <= MyDV.Count - 1; i++)
            //{
            //    ClsFldMst vMyFld = new ClsFldMst();
               
            //    var _with7 = vMyFld;
            //    _with7.FldMstId = MyDV[0]["FldMstId"].ToString();
            //    _with7.TblMst = ObjCtrlTbl.GetRecordById(MyDV[0]["TblMstId"].ToString(), MyCnStr);
            //    _with7.FldName = MyDV[0]["FldName"].ToString();
            //    _with7.FldDesc = MyDV[0]["FldDesc"].ToString();
            //    _with7.FldQry = MyDV[0]["FldQry"].ToString();
            //    _with7.FldDataType = MyDV[0]["FldDataType"].ToString();
            //    _with7.FldColor = MyDV[0]["FldColor"].ToString();
            //    _with7.FldFont = MyDV[0]["FldFont"].ToString();
            //    _with7.FldFontStyle = MyDV[0]["FldFontStyle"].ToString();
            //    _with7.FldFontSize = MyDV[0]["FldFontSize"].ToString();
            //    _with7.FldColWidth = MyDV[0]["FldColWidth"].ToString();
            //    ArrList.Add(vMyFld);
            //}
            var result = (from rw in MyDV.ToTable().AsEnumerable()                        
                          select new ClsFldMst
                          {
                              RecId = rw["RecId"].ToString(),
                              FldMstId = rw["FldMstId"].ToString(),
                              TblMst = ObjCtrlTbl.GetRecordById(rw["TblMstId"].ToString(), MyCnStr),
                              FldName = rw["FldName"].ToString(),
                              FldDesc = rw["FldDesc"].ToString(),
                              FldQry = rw["FldQry"].ToString(),
                              FldDataType = rw["FldDataType"].ToString(),
                              FldColor = rw["FldColor"].ToString(),
                              FldFont = rw["FldFont"].ToString(),
                              FldFontStyle = rw["FldFontStyle"].ToString(),
                              FldFontSize = rw["FldFontSize"].ToString(),
                              FldColWidth = rw["FldColWidth"].ToString(),
                          }).FirstOrDefault();
            ArrList.Add(result);

            return ArrList;
        }
        catch (Exception ex)
        {
            // ObjBICommon.WriteApplicationLog("ClsCtrlFldMst", "GetAllRecordsUsingMyCnnStr", ex.Message);
            Exception ex1 = new Exception(ex.Message);
            throw ex1;
        }
    }
}
