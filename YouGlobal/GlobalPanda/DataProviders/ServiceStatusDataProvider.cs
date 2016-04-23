using GlobalPanda;
using MySql.Data.MySqlClient;
using System;

/// <summary>
/// Summary description for ServiceStatusDataProvider
/// </summary>
public class ServiceStatusDataProvider
{
    public static void addLog(int userid, string task, string status, DateTime modifiedDate)
    {
        string sql = "Insert into service_log(userid,task,status,modifiedDate) Values(?userid,?task,?status,?modifieddate)";
        DAO.ExecuteNonQuery(sql, new MySqlParameter("userid", userid), new MySqlParameter("task", task), new MySqlParameter("status", status), new MySqlParameter("modifiedDate", modifiedDate));
    }

    public static MySqlDataReader searchStatus(string keyword)
    {
        string sql = "Select service_logid,task,status,Date_Format(modifieddate,'%d-%b-%Y') as modifieddate from service_log where service_logid like concat_ws(?keyword,'%','%') or " +
                     "task like concat_ws(?keyword,'%','%') or status like concat_ws(?keyword,'%','%') or modifieddate like concat_ws(?keyword,'%','%') order by service_logid";

        return DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword));
    }
}