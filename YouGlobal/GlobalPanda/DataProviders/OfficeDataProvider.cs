using MySql.Data.MySqlClient;
using System;

/// <summary>
/// Summary description for MenuItemDataProvider
/// </summary>
namespace GlobalPanda.DataProviders
{
    public class OfficeDataProvider
    {
        public static MySqlDataReader listOffices(uint tenantid)
        {
            string sql = "select o.officeid, o.name " +
                         "from offices as o " +
                         "where o.tenantid = ?tenantid " +
                         "order by o.name";
            MySqlDataReader drO = DAO.ExecuteReader(sql, new MySqlParameter("tenantid", tenantid));
            return drO;
        }

        public static bool isDuplicateOffice(string name, uint officeid)
        {
            string sql = "select count(officeid) as `exists` from offices where name = ?name and officeid != ?officeid";
            uint exists = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("name", name), new MySqlParameter("officeid", officeid)));
            return (exists > 0);
        }

        public static string getOfficeName(int id)
        {
            string sql = "select name from offices where officeid=?id;";
            string name = Convert.ToString(DAO.ExecuteScalar(sql, new MySqlParameter("id", id)));
            return name;
        }
    }
}