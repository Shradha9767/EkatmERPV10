using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
public class ClsDB
{
    public string DBName { get; set; }
    public string DBId { get; set; }
    public string TblName { get; set; }
    public string RecId { get; set; }
    public string DbMstId { get; set; }
    public string ServerName { get; set; }
    public string UserId { get; set; }
    public string Password { get; set; }
    public string ProviderName { get; set; }
    public string connectioString { get; set; }
    private string _RptFont { get; set; }
    public string RptFont { get; set; }
}
