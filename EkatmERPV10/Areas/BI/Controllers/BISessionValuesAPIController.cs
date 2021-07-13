using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class BISessionValuesAPIController : Controller
{
    private readonly ICommonFunctions _iCommonFunc;
    public BISessionValuesAPIController(ICommonFunctions IcommonFunctions)
    {
        _iCommonFunc = IcommonFunctions;
    }
    [HttpGet]
    public DataTable GetBISessionValue(string SessionVal)
    {
        DataTable dt = new DataTable();
        StringBuilder sb = new StringBuilder();
        DataTable dtqry = new DataTable();
        var currentUser = HttpContext.User;
        string UserName = Convert.ToString(currentUser.Claims.FirstOrDefault(c => c.Type == "UserName").Value);
        string DeptMasterId = currentUser.Claims.FirstOrDefault(c => c.Type == "DeptMasterId").Value;
        string UserMasterId = currentUser.Claims.FirstOrDefault(c => c.Type == "UserMasterId").Value;
        string AppEnvMasterId = currentUser.Claims.FirstOrDefault(c => c.Type == "AppEnvMasterId").Value;
        string PlantId = currentUser.Claims.FirstOrDefault(c => c.Type == "PlantId").Value;
        string MyConnRpt = currentUser.Claims.FirstOrDefault(c => c.Type == "RgtCnnStr").Value;
        string MyConn = currentUser.Claims.FirstOrDefault(c => c.Type == "EkatmDbCnnStr").Value;
        try
        {
           
            sb.Append("SELECT BISessionValueQuery FROM BISessionValue WHERE BISessionValueCode='" + SessionVal + "'");
            dtqry = _iCommonFunc.GetDataTable(sb.ToString(), false, MyConnRpt);
           
            if (dtqry.Rows.Count > 0 && dtqry.Rows[0][0] != "")
            {
                string Id = "declare @UserMasterId varchar(max)='" + UserMasterId + "';";
                if (SessionVal == "User Name")
                {
                    string User = "declare @UserName varchar(max)='" + UserName + "';";
                    string Qry = dtqry.Rows[0]["BISessionValueQuery"].ToString();
                    Qry = User + Qry;
                    dt = _iCommonFunc.GetDataTable(Qry.ToString(), false, MyConn);
                }
                else if (SessionVal == "Department")
                {
                    string Qry = Convert.ToString(dtqry.Rows[0]["BISessionValueQuery"]);
                    Qry = Id + Qry;
                    dt = _iCommonFunc.GetDataTable(Qry.ToString(), false, MyConn);
                }
                else if (SessionVal == "Branch")
                {
                    string Qry = dtqry.Rows[0]["BISessionValueQuery"].ToString();
                    Qry = Id + Qry;
                    dt = _iCommonFunc.GetDataTable(Qry.ToString(), false, MyConn);
                }
                else if (SessionVal == "Designation")
                {
                    string Qry = dtqry.Rows[0]["BISessionValueQuery"].ToString();
                    Qry = Id + Qry;
                    dt = _iCommonFunc.GetDataTable(Qry.ToString(), false, MyConn);
                }
                else if (SessionVal == "Plant")
                {
                    string Plant = "declare @PlantId varchar(max)='" + PlantId + "';";
                    string Qry = dtqry.Rows[0]["BISessionValueQuery"].ToString();
                    Qry = Plant + Qry;
                    dt = _iCommonFunc.GetDataTable(Qry.ToString(), false, MyConn);
                }
                else if (SessionVal == "Report To")
                {
                    string Qry = dtqry.Rows[0]["BISessionValueQuery"].ToString();
                    Qry = Id + Qry;
                    dt = _iCommonFunc.GetDataTable(Qry.ToString(), false, MyConn);
                }
                else if (SessionVal == "Reportee")
                {
                    string Qry = dtqry.Rows[0]["BISessionValueQuery"].ToString();
                    Qry = Id + Qry;
                    dt = _iCommonFunc.GetDataTable(Qry.ToString(), false, MyConn);
                }
                else if (SessionVal == "Calender Year Begin")
                {
                    string Qry = dtqry.Rows[0]["BISessionValueQuery"].ToString();
                    dt = _iCommonFunc.GetDataTable(Qry.ToString(), false, MyConn);
                }
                else if (SessionVal == "Calender Year End")
                {
                    string Qry = dtqry.Rows[0]["BISessionValueQuery"].ToString();
                    dt = _iCommonFunc.GetDataTable(Qry.ToString(), false, MyConn);
                }
                else if (SessionVal == "Current Month Begin")
                {
                    string Qry = dtqry.Rows[0]["BISessionValueQuery"].ToString();
                    dt = _iCommonFunc.GetDataTable(Qry.ToString(), false, MyConn);
                }
                else if (SessionVal == "Current Month End")
                {
                    string Qry = dtqry.Rows[0]["BISessionValueQuery"].ToString();
                    dt = _iCommonFunc.GetDataTable(Qry.ToString(), false, MyConn);
                }
                else if (SessionVal == "Current Week Begin")
                {
                    string Qry = dtqry.Rows[0]["BISessionValueQuery"].ToString();
                    dt = _iCommonFunc.GetDataTable(Qry.ToString(), false, MyConn);
                }
                else if (SessionVal == "Current Week End")
                {
                    string Qry = dtqry.Rows[0]["BISessionValueQuery"].ToString();
                    dt = _iCommonFunc.GetDataTable(Qry.ToString(), false, MyConn);
                }
                else if (SessionVal == "Financial Year Begin")
                {
                    string Qry = dtqry.Rows[0]["BISessionValueQuery"].ToString();
                    dt = _iCommonFunc.GetDataTable(Qry.ToString(), false, MyConn);
                }
                else if (SessionVal == "Financial Year End")
                {
                    string Qry = dtqry.Rows[0]["BISessionValueQuery"].ToString();
                    dt = _iCommonFunc.GetDataTable(Qry.ToString(), false, MyConn);
                }
                else if (SessionVal == "Financial Year")
                {
                    string Qry = dtqry.Rows[0]["BISessionValueQuery"].ToString();
                    dt = _iCommonFunc.GetDataTable(Qry.ToString(), false, MyConn);
                }
                else if (SessionVal == "Today Date")
                {
                    string Qry = dtqry.Rows[0]["BISessionValueQuery"].ToString();
                    dt = _iCommonFunc.GetDataTable(Qry.ToString(), false, MyConn);
                }
                else if (SessionVal == "Tommorrow")
                {
                    string Qry = dtqry.Rows[0]["BISessionValueQuery"].ToString();
                    dt = _iCommonFunc.GetDataTable(Qry.ToString(), false, MyConn);
                }
                else if (SessionVal == "Yesterday")
                {
                    string Qry = dtqry.Rows[0]["BISessionValueQuery"].ToString();
                    dt = _iCommonFunc.GetDataTable(Qry.ToString(), false, MyConn);
                }
                
            }
        }
        catch (Exception ex)
        {
            // ObjBICommon.WriteApplicationLog("ReportInfoAPIController --> ", "Error in GetAllReportInfo(): --> ", ex.Message);
            throw new Exception("BISessionValuesAPIController --> GetBISessionValue() --> " + ex.Message);
        }
        return dt;
    }
}
