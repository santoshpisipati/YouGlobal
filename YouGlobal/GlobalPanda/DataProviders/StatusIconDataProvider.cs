using MySql.Data.MySqlClient;
using System;

/// <summary>
/// Summary description for StatusIconDataProvider
/// </summary>
///
namespace GlobalPanda.DataProviders
{
    public class StatusIconDataProvider
    {
        public static int insertIcon(string imagepath, string description)
        {
            int iconId = 0;
            string sql = "insert into status_icon (imagepath,description) values (?imagepath,?description); select last_insert_id()";
            iconId = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("imagepath", imagepath), new MySqlParameter("description", description)));
            return iconId;
        }

        public static void updateIcon(int id, string imagepath, string description)
        {
            string sql = "update status_icon set imagepath=?imagepath,description=?description where status_iconid=?id";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("id", id), new MySqlParameter("imagepath", imagepath), new MySqlParameter("description", description));
        }

        public static void updateIcon(int id, string description)
        {
            string sql = "update status_icon set description=?description where status_iconid=?id";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("id", id), new MySqlParameter("description", description));
        }

        public static MySqlDataReader listIcon()
        {
            string sql = "select status_iconid,imagepath,description from status_icon";
            return DAO.ExecuteReader(sql);
        }

        public static MySqlDataReader getIcon(int id)
        {
            string sql = "select status_iconid,imagepath,description from status_icon where status_iconid=?id";
            return DAO.ExecuteReader(sql, new MySqlParameter("id", id));
        }

        public static MySqlDataReader deleteIcon(int id)
        {
            string sql = "delete from status_icon where status_iconid=?id";
            return DAO.ExecuteReader(sql, new MySqlParameter("id", id));
        }

        public static bool isUsed(int id)
        {
            bool _used = false;
            int statusiconid = 0;
            string sql = "select statusiconid from candidates_jobs  where statusiconid=?statusiconid";
            statusiconid = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("statusiconid", id)));
            if (statusiconid > 0)
                _used = true;
            return _used;
        }

        public static string getIconPath(int id)
        {
            string path = string.Empty;
            string sql = "select imagepath from status_icon where status_iconid=?id";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("id", id));
            if (reader.HasRows)
            {
                reader.Read();
                path = DAO.getString(reader, "imagepath");
            }
            reader.Close();
            reader.Dispose();

            return path;
        }
    }
}