using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
public class ClsCtrlFunctionInfo
{
    private readonly ICommonFunctions _iCommonFunc;
    public readonly IHttpContextAccessor _httpContextAccessor;
    public ClsCtrlFunctionInfo(ICommonFunctions IcommonFunctions, IHttpContextAccessor httpContextAccessor)
    {
        _iCommonFunc = IcommonFunctions;
        _httpContextAccessor = httpContextAccessor;
    }
    public System.Collections.ArrayList GetAllRecordsUsingMyCnnStr(string CnnStr, string StrCondtion = "", string OrderByClause = "")
    {
        DataTable dt = new DataTable();
        ArrayList ArrList = new ArrayList();
        var result = new ClsFunctionInfo();
        try
        {
            string StrQuery = "Select * from FunctionInfo";
            if (StrCondtion != "")
                StrQuery = StrQuery + " Where " + StrCondtion;
            dt = _iCommonFunc.GetDataTable(StrQuery, false, CnnStr);
            if(dt.Rows.Count>0)
            {
                result = (from rw in dt.AsEnumerable()
                          select new ClsFunctionInfo
                          {
                              RecId = rw["RecId"].ToString(),
                              FunctionInfoId = rw["FunctionInfoId"].ToString(),
                              AssemblyName = rw["AssemblyName"].ToString(),
                              ClassName = rw["ClassName"].ToString(),
                              TblMstId = rw["TblMstId"].ToString(),
                          }).FirstOrDefault();
            }
             
            ArrList.Add(result);
            //for (i = 0; i <= MyDV.Count - 1; i++)
            //{
            //vMyFunctionInfo = new ClsFunctionInfo();
            //{
            //    var withBlock = vMyFunctionInfo;
            //    withBlock.RecId = MyDV[i]["RecId"].ToString();
            //    withBlock.FunctionInfoId = MyDV[i]["FunctionInfoId"].ToString();
            //    withBlock.AssemblyName = MyDV[i]["AssemblyName"].ToString();
            //    withBlock.ClassName = MyDV[i]["ClassName"].ToString();
            //    withBlock.TblMstId = MyDV[i]["TblMstId"].ToString();
            //}
            //ArrList.Add(vMyFunctionInfo);
            //}
            return ArrList;
        }
        catch (Exception ex)
        {
            Exception ex1 = new Exception(ex.Message);
            throw ex1;
        }
    }
}
