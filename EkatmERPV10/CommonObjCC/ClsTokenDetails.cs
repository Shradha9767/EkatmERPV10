using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
public class ClsTokenDetails
{
    public string UserMasterId { get; set; }
    public string UserName { get; set; }
    public string UserLoginId { get; set; }
    public string AppEnvMasterId { get; set; }
    public string Plant { get; set; }
    public string PlantId { get; set; }
    public string DeptMasterId { get; set; }
    public bool IsValidUser { get; set; }
    public string EkatmDbCnnStr { get; set; }
    public string RgtCnnStr { get; set; }
    public string EkatmMsgCnnStr { get; set; }
    //BI Session
    public string RptXml { get; set; }
}
