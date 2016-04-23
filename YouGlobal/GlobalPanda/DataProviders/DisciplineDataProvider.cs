using MySql.Data.MySqlClient;
using System;

/// <summary>
/// Summary description for MenuItemDataProvider
/// </summary>
namespace GlobalPanda.DataProviders
{
    public class DisciplineDataProvider
    {
        public static uint addDiscipline(string name)
        {
            string sql = "select disciplineid from disciplines where name = ?name";
            uint? disciplineid = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("name", name)));
            if (disciplineid <= 0)
            {
                sql = "insert into disciplines (name) values (?name); select last_insert_id()";
                disciplineid = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("name", name)));
            }
            return Convert.ToUInt32(disciplineid);
        }
    }
}