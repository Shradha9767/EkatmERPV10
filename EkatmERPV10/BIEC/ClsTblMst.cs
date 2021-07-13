using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
public class ClsTblMst
{
    //private ClsDB vDBMst;
    //private string vRecId;
    //private string vTblMstId;
    //private string vTblName;
    //private string vTblDesc;
    //private string vQuery;
    public string RecId { get; set; }
    public string TblMstId { get; set; }
    public string TblName { get; set; }
    public string TblDesc { get; set; }
    public ClsDB DBMst { get; set; }
    public string TblType { get; set; }
    public string Query { get; set; }
}
