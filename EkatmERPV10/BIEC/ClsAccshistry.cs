using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
public class ClsAccshistry
{
    public string RGTCode { get; set; }
    public string Query { get; set; }
    public DateTime DateTime { get; set; }
    public string UserName { get; set; }
    public string Ip { get; set; }
    public string SelectionCriteria { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public double TimeSpan { get; set; }
    public string AcchistryId { get; set; }
    public long NoofRecs { get; set; }
    public long Sizeinkb { get; set; }
    public bool IsScheduled { get; set; }
    public DateTime SchTime { get; set; }
    public bool IsMailCreated { get; set; }
    public string VCID { get; set; }
    public DateTime SchRunTime { get; set; }
    //private DateTime? _SchTime = null;
    //public DateTime SchTime
    //{
    //    get
    //    {
    //        return this._SchTime.HasValue
    //           ? this._SchTime.Value
    //           : DateTime.Now;
    //    }
    //    set { _SchTime = value; }
    //}
    //private DateTime? _SchRunTime = null;
    //public DateTime SchRunTime
    //{
    //    get
    //    {
    //        return this._SchRunTime.HasValue
    //           ? this._SchRunTime.Value
    //           : DateTime.Now;
    //    }
    //    set { _SchRunTime = value; }
    //}
}
