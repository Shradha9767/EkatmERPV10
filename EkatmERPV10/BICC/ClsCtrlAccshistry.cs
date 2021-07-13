using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
public class ClsCtrlAccshistry
{
    private readonly ICommonFunctions _iCommonFunc;
    public ClsCtrlAccshistry(ICommonFunctions IcommonFunctions)
    {
        _iCommonFunc = IcommonFunctions;
    }
    public ClsAccshistry clsAccshistry;
    public int AddRec(string con)
    {
        try
        {
            
            var param = new SqlParameter[]
            {
                new SqlParameter()
                {
                    ParameterName="@RGTCode",
                    SqlDbType=System.Data.SqlDbType.NVarChar,
                    Direction=System.Data.ParameterDirection.Input,
                    Value=clsAccshistry.RGTCode
                },
                new SqlParameter()
                {
                    ParameterName="@Query",
                    SqlDbType=System.Data.SqlDbType.NVarChar,
                    Direction=System.Data.ParameterDirection.Input,
                    Value=clsAccshistry.Query
                },
                new SqlParameter()
                {
                    ParameterName="@DateTime",
                    SqlDbType=System.Data.SqlDbType.DateTime,
                    Direction=System.Data.ParameterDirection.Input,
                    Value=clsAccshistry.DateTime
                },
                new SqlParameter()
                {
                    ParameterName="@UserName",
                    SqlDbType=System.Data.SqlDbType.NVarChar,
                    Direction=System.Data.ParameterDirection.Input,
                    Value=clsAccshistry.UserName
                },
                new SqlParameter()
                {
                    ParameterName="@Ip",
                    SqlDbType=System.Data.SqlDbType.NVarChar,
                    Direction=System.Data.ParameterDirection.Input,
                    Value=clsAccshistry.Ip
                },
                 new SqlParameter()
                {
                    ParameterName="@SelectionCriteria",
                    SqlDbType=System.Data.SqlDbType.NVarChar,
                    Direction=System.Data.ParameterDirection.Input,
                    Value=clsAccshistry.SelectionCriteria
                },
                  new SqlParameter()
                {
                    ParameterName="@StartTime",
                    SqlDbType=System.Data.SqlDbType.DateTime,
                    Direction=System.Data.ParameterDirection.Input,
                    Value=clsAccshistry.StartTime
                },
                  new SqlParameter()
                {
                    ParameterName="@EndTime",
                    SqlDbType=System.Data.SqlDbType.DateTime,
                    Direction=System.Data.ParameterDirection.Input,
                    Value=clsAccshistry.EndTime
                },
                   new SqlParameter()
                {
                    ParameterName="@TimeSpan",
                    SqlDbType=System.Data.SqlDbType.NVarChar,
                    Direction=System.Data.ParameterDirection.Input,
                    Value=clsAccshistry.TimeSpan
                },
                   new SqlParameter()
                {
                    ParameterName="@NoofRecs",
                    SqlDbType=System.Data.SqlDbType.Decimal,
                    Direction=System.Data.ParameterDirection.Input,
                    Value=clsAccshistry.NoofRecs
                },
                   new SqlParameter()
                {
                    ParameterName="@Sizeinkb",
                    SqlDbType=System.Data.SqlDbType.Decimal,
                    Direction=System.Data.ParameterDirection.Input,
                    Value=clsAccshistry.Sizeinkb
                },
                    new SqlParameter()
                {
                    ParameterName="@IsScheduled",
                    SqlDbType=System.Data.SqlDbType.Bit,
                    Direction=System.Data.ParameterDirection.Input,
                    Value=clsAccshistry.IsScheduled
                },
                    new SqlParameter()
                {
                    ParameterName="@SchTime",
                    SqlDbType=System.Data.SqlDbType.DateTime,
                    Direction=System.Data.ParameterDirection.Input,
                    Value=clsAccshistry.SchTime
                },
            };
            int result = 0;
            using (var cnn = new SqlConnection(con))
            {
                var cmm = cnn.CreateCommand();
                cmm.CommandType = System.Data.CommandType.StoredProcedure;
                cmm.CommandText = "DS_SP_Insert_RGT_AccessHistory";
                cmm.Parameters.AddRange(param);
                cmm.Connection = cnn;
                cnn.Open();
               
                result = Convert.ToInt32(cmm.ExecuteScalar());

            }
            return result;

            // string strDate = "";
            //SqlCommand cmd = new SqlCommand();
            //cmd.Connection = con;
            //cmd.CommandText = "DS_SP_Insert_RGT_AccessHistory";
            //cmd.CommandType = System.Data.CommandType.StoredProcedure;
            //SqlParameter para = new SqlParameter();
            //para.DbType = System.Data.DbType.String;
            //para.ParameterName = "@RGTCode";
            //para.Value = clsAccshistry.RGTCode;
            //cmd.Parameters.Add(para);
            //para = new SqlParameter();
            //para.DbType = System.Data.DbType.String;
            //para.ParameterName = "@Query";
            //para.Value = clsAccshistry.Query;
            //cmd.Parameters.Add(para);
            //para = new SqlParameter();
            //para.DbType = System.Data.DbType.DateTime;
            //para.ParameterName = "@DateTime";
            //para.Value = clsAccshistry.DateTime;
            //cmd.Parameters.Add(para);
            //para = new SqlParameter();
            //para.DbType = System.Data.DbType.String;
            //para.ParameterName = "@UserName";
            //para.Value = objuserinfo.UserMasterId;
            //cmd.Parameters.Add(para);
            //para = new SqlParameter();
            //para.DbType = System.Data.DbType.String;
            //para.ParameterName = "@Ip";
            //para.Value = clsAccshistry.Ip;
            //cmd.Parameters.Add(para);
            //para = new SqlParameter();
            //para.DbType = System.Data.DbType.String;
            //para.ParameterName = "@SelectionCriteria";
            //para.Value = clsAccshistry.SelectionCriteria;
            //cmd.Parameters.Add(para);
            //para = new SqlParameter();
            //para.DbType = System.Data.DbType.DateTime;
            //para.ParameterName = "@StartTime";
            //para.Value = clsAccshistry.StartTime;
            //cmd.Parameters.Add(para);
            //para = new SqlParameter();
            //para.DbType = System.Data.DbType.DateTime;
            //para.ParameterName = "@EndTime";
            //para.Value = clsAccshistry.EndTime;
            //cmd.Parameters.Add(para);
            //para = new SqlParameter();
            //para.DbType = System.Data.DbType.String;
            //para.ParameterName = "@TimeSpan";
            //para.Value = clsAccshistry.TimeSpan;
            //cmd.Parameters.Add(para);
            //para = new SqlParameter();
            //para.DbType = System.Data.DbType.Decimal;
            //para.ParameterName = "@NoofRecs";
            //para.Value = clsAccshistry.NoofRecs;
            //cmd.Parameters.Add(para);
            //para = new SqlParameter();
            //para.DbType = System.Data.DbType.Decimal;
            //para.ParameterName = "@Sizeinkb";
            //para.Value = clsAccshistry.Sizeinkb;
            //cmd.Parameters.Add(para);
            //para = new SqlParameter();
            //para.DbType = System.Data.DbType.Boolean;
            //para.ParameterName = "@IsScheduled";
            //para.Value = clsAccshistry.IsScheduled;
            //cmd.Parameters.Add(para);
            //para = new SqlParameter();
            //para.DbType = System.Data.DbType.DateTime;
            //para.ParameterName = "@SchTime";
            //para.Value = clsAccshistry.SchTime;
            //cmd.Parameters.Add(para);
            //para = new SqlParameter();
            //para.DbType = System.Data.DbType.DateTime;
            //para.ParameterName = "@SchRunTime";
            //para.Value = clsAccshistry.SchRunTime;
            //cmd.Parameters.Add(para);
            //para = new SqlParameter();
            //para.DbType = System.Data.DbType.Boolean;
            //para.ParameterName = "@IsMailCreated";
            //para.Value = clsAccshistry.IsMailCreated;
            //cmd.Parameters.Add(para);
            //para = new SqlParameter();
            //para.DbType = System.Data.DbType.String;
            //para.ParameterName = "@VCID";
            //para.Value = clsAccshistry.VCID;
            //cmd.Parameters.Add(para);
            //para = new SqlParameter();
            //para.DbType = System.Data.DbType.String;
            //para.ParameterName = "@AcchistryId";
            //para.Value = clsAccshistry.AcchistryId;
            //cmd.Parameters.Add(para);
            //cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
            // OBJLog.WriteApplicationLog("ClsCtrlAccshistry", "AddRec", ex.Message);
            //WriteErrorLog(ex.Message + " " + "Schtime:" + clsAccshistry.SchTime + "SchRunTime:" + clsAccshistry.SchRunTime + "IsScheduled:" + clsAccshistry.IsScheduled.ToString());
        }
    }
}
