using System.Data;


namespace BD.IRepositories
{
    public interface IDBRepository
    {
        int ExecQuery(string query);
        DataTable GetDataTable(string query, string log = "0");
        DataTable GetDataTablePara(string query, Dictionary<string, string> dict);
        int ExecQueryPara(string query, Dictionary<string, string> dict);
    }
}
