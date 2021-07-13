using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
public class ClsLinkRpts
{
    //private string vRecId;
    //private string vParentRptId;
    //private string vChildRptId;
    //private string vFixedType;
    //private string vRuntimeType;
    //private string vColumnValueType;
    //private string vParentSelCriteriaType;
    //private string vFunctionInfoId;
    //private string vWebPageURL;
    //private int vLinkMethod;
    //private string vLinkCol;
    //private string vParentRptCode;
    //private string vChildRptCode;
    //private bool vOpenRptNewPage;
    //private string vUniqueField;
    public string RecId { get; set; }
   
    public string ParentRptId { get; set; }
    public string ChildRptId { get; set; }
    public string FixedType { get; set; }
    public string RuntimeType { get; set; }
    public string ColumnValueType { get; set; }
    public string ParentSelCriteriaType { get; set; }
    public string WebPageURL { get; set; }
    public int LinkMethod { get; set; }
    public string FunctionInfoId { get; set; }
    public string LinkCol { get; set; }
    public string ParentRptCode { get; set; }
    public string ChildRptCode { get; set; }
    public bool OpenRptNewPage { get; set; }
}
