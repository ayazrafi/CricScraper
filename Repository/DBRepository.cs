using BD.IRepositories;
using MySql.Data.MySqlClient;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BD.Repository
{
    public class DBRepository : IDBRepository
    {
       // private readonly IConfiguration _Configuration;
        MySqlConnection conn;
        public DBRepository()
        {
            //_Configuration = config;
            string connectionstring = "Server=148.72.245.249;Port=3306;DataBase=bigdream;Uid=temp;Pwd=786Test@786;" +
                "Convert Zero Datetime=true;SslMode=None;AllowPublicKeyRetrieval=True;"; //_Configuration.GetSection("AppSettings").GetSection("MySQLConnectioString").Value;
            conn = new MySqlConnection(connectionstring);
        }
        public int ExecQuery(string query)
        {
            var result = 0;
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, conn);
                if (conn.State == ConnectionState.Closed)
                    cmd.Connection.Open();
                if (cmd.ExecuteNonQuery() != 0)
                {
                    conn.Close();
                    return 1;
                }
                else
                {
                    conn.Close();
                    return 0;
                }

            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(@"wwwroot\loginfo.txt", ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + query + Environment.NewLine + Environment.NewLine);
                conn.Close();
                return 0;
            }

        }

        public int ExecQueryPara(string query, Dictionary<string, string> dict)
        {
            var result = 0;
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, conn);
                foreach (string key in dict.Keys)
                {
                    cmd.Parameters.AddWithValue(key, dict[key]);
                }
                if (conn.State == ConnectionState.Closed)
                    cmd.Connection.Open();
                if (cmd.ExecuteNonQuery() != 0)
                {
                    conn.Close();
                    return 1;
                }
                else
                {
                    conn.Close();
                    return 0;
                }

            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(@"wwwroot\loginfo.txt", ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + query + Environment.NewLine + Environment.NewLine);
                conn.Close();
                return 0;
            }

        }

        public DataTable GetDataTablePara(string query, Dictionary<string, string> dict)
        {
            var DTRet = new DataTable();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, conn);
                try
                {
                    foreach (string key in dict.Keys)
                    {
                        cmd.Parameters.AddWithValue(key, dict[key]);
                    }
                }
                catch (Exception ex) 
                { 
                
                }
                cmd.CommandTimeout = 6000;
                if (conn.State == ConnectionState.Closed)
                    cmd.Connection.Open();
                MySqlDataAdapter Adpter = new MySqlDataAdapter(cmd);
                DataSet Ds = new DataSet();
                Adpter.Fill(Ds);
                if (Ds != null && Ds.Tables.Count > 0 && Ds.Tables[0].Rows.Count > 0)
                {
                    DTRet = Ds.Tables[0];
                }

                conn.Close();
                return DTRet;
            }
            catch (Exception e)
            {
                System.IO.File.AppendAllText(@"wwwroot\loginfo.txt", e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + query + Environment.NewLine + Environment.NewLine);
                conn.Close();
                return DTRet;
            }
            finally
            {
                conn.Close();
            }
        }

        public DataTable GetDataTable(string query, string logexception = "0")
        {
            var DTRet = new DataTable();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.CommandTimeout = 6000;
                if (conn.State == ConnectionState.Closed)
                    cmd.Connection.Open();
                MySqlDataAdapter Adpter = new MySqlDataAdapter(cmd);
                DataSet Ds = new DataSet();
                Adpter.Fill(Ds);
                if (Ds != null && Ds.Tables.Count > 0 && Ds.Tables[0].Rows.Count > 0)
                {
                    DTRet = Ds.Tables[0];
                }

                conn.Close();
                return DTRet;
            }
            catch (Exception e)
            {
                System.IO.File.AppendAllText(@"wwwroot\loginfo.txt", e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + query + Environment.NewLine + Environment.NewLine);
                conn.Close();
                throw e;
                return DTRet;
            }
            finally
            {
                conn.Close();
            }
        }


    }
}
