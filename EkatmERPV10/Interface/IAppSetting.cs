using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
public interface IAppSetting
{
    public string GetConnection();
    // public string GetConnection(string dbName);
    //public string GetEnvironments(string dbName);
    public string WebUrl();

    public string GetDynamicURL();
    public string GetAppCode();
    public string MailCnnStr();
    public string AccType();

    public string GetHelloword() => hello();
    
    public string hello()
    {
        return "Hellow";
    }
}
