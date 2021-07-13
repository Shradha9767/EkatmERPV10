using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
public class ClsFldMst
{
   
    //private string vRecId;
    //private string vFldMstId;
    //private ClsTblMst vTblMst;
    //private string vFldName;
    //private string vFldDesc;
    //private string vFldQry;
    //private string vFldDataType;
    //private string vFldColor;
    //private string vFldFont;
    //private string vFldFontStyle;
    //private string vFldFontSize;
    //private string vFldColWidth;
  
    public string RecId { get; set; }
   
    public ClsTblMst TblMst { get; set; }
  
    public string FldMstId { get; set; }
  
    public string FldName { get; set; }
    
    public string FldDesc { get; set; }
 
    public string FldQry { get; set; }
  
    public string FldDataType { get; set; }
   
    public string FldColor { get; set; }
    
    public string FldFont { get; set; }
  
    public string FldFontStyle { get; set; }
   
    public string FldFontSize { get; set; }
   
    public string FldColWidth { get; set; }
   
   
    public string OrderBy { get; set; }
   
  
    public string OrderSeq { get; set; }
   
   
    public string FldAlias { get; set; }
   
  
    public string SummaryCondn { get; set; }

 
    public bool IsVisible { get; set; }
   
   
    public string SummaryDesc { get; set; }
    
   
    public string GrpBy { get; set; }


    public bool IsBrk { get; set; }
  
    public bool ViewInHeader { get; set; }
   
    public string Format { get; set; }
   
    public string NumericFormat { get; set; }
   
    //public ClsFldMst()
    //{
    //    init();
    //}
    //private void init()
    //{
    //    vFldMstId = "";
    //    vTblMst = new ClsTblMst();
    //    vFldName = "";
    //    vFldDesc = "";
    //    vFldQry = "";
    //    vFldDataType = "";
    //    vFldColor = "";
    //    vFldFont = "";
    //    vFldFontStyle = "";
    //    vFldFontSize = "";
    //    vFldColWidth = "";
    //}
}
