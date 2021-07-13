using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
public class ClsCtrlVSKeys
{
    private readonly ICommonFunctions _iCommonFunc;
    public ClsCtrlVSKeys(ICommonFunctions IcommonFunctions)
    {
        _iCommonFunc = IcommonFunctions;
    }
    public ClsVSKeys GetRecordById(string MyKeyId, string StrCondition, string MyConnStr)
    {
        string StrQuery = null;
        DataView MyDv = new DataView();
        try
        {
           
            StrQuery = "Select * From VSKeys Where CodeId = '" + MyKeyId + "' ";
            if (!string.IsNullOrEmpty(StrCondition))
            {
                StrQuery = StrQuery + StrCondition;
            }
            MyDv = _iCommonFunc.GetDataView(StrQuery, false, MyConnStr);
            ClsVSKeys vVSKeys = new ClsVSKeys();
            if (MyDv.Count > 0)
            {
                vVSKeys = new ClsVSKeys();
                var _with1 = vVSKeys;
                _with1.RecId = MyDv[0]["RecId"].ToString();
                _with1.TypeCode = MyDv[0]["TypeCode"].ToString();
                _with1.CodeId = MyDv[0]["CodeId"].ToString();
                _with1.CodeDesc = MyDv[0]["CodeDesc"].ToString();
                _with1.LongDesc = MyDv[0]["LongDesc"].ToString();
                _with1.Fixed = MyDv[0]["Fixed"].ToString();
                _with1.IsDeleted = MyDv[0]["IsDeleted"].ToString();
            }
            return vVSKeys;
        }
        catch (Exception ex)
        {
            //ObjBICommon.WriteApplicationLog("ClsCtrlVSKeys", "GetRecordById", ex.Message);
            throw new Exception(ex.Message);
        }
    }
}
