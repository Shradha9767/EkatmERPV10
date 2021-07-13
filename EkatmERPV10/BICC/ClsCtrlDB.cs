using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
public class ClsCtrlDB
{
    private readonly ICommonFunctions _iCommonFunc;
    public ClsCtrlDB(ICommonFunctions IcommonFunctions)
    {
        _iCommonFunc = IcommonFunctions;
    }
    public ClsDB GetRecordById(string MyKeyId, string MyCnStr)
    {
        DataView MyDV = new DataView();
        ClsDB vMyDB = new ClsDB();
        string StrQuery = null;
        var result = new ClsDB();
        try
        {
            StrQuery = "Select * from DBMst Where DBMstId= '" + MyKeyId + "'";
            MyDV = _iCommonFunc.GetDataView(StrQuery, false, MyCnStr);
            if (MyDV.Count > 0)
            {
                result = (from rw in MyDV.ToTable().AsEnumerable()
                          select new ClsDB
                          {
                              RecId = rw["RecId"].ToString(),
                              DbMstId = rw["DbMstId"].ToString(),
                              DBName = rw["DBName"].ToString(),
                              ServerName = rw["ServerName"].ToString(),
                              UserId = rw["UserId"].ToString(),
                              Password = rw["Password"].ToString(),
                              connectioString = rw["Connection String"].ToString(),
                              ProviderName = rw["ProviderName"].ToString(),
                              RptFont = rw["RptFont"].ToString(),
                          }).FirstOrDefault();
            }
            return result;
        }
        catch (Exception ex)
        {
            //ObjBICommon.WriteApplicationLog("ClsCtrlDB", "GetRecordById", ex.Message);
            Exception ex1 = new Exception(ex.Message);
            throw ex1;
        }
    }
}
