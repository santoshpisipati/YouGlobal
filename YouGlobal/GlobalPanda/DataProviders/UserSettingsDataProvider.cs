using MySql.Data.MySqlClient;
using System;

/// <summary>
/// Summary description for UserSettingsDataProvider
/// </summary>
namespace GlobalPanda.DataProviders
{
    public class UserSettingsDataProvider
    {
        public static void insertUsersettings(uint userid, DateTime? grpahStartDate, DateTime? graphEndDate, int type)
        {
            string sql = "insert into users_Settings (userid,graphstartdate,graphenddate,type) values (?userid,?graphstartdate,?graphenddate,?type)";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("userid", userid), new MySqlParameter("graphstartdate", grpahStartDate), new MySqlParameter("graphenddate", graphEndDate), new MySqlParameter("type", type));
        }

        public static void updateUsersettings(uint userid, DateTime? grpahStartDate, DateTime? graphEndDate, int type)
        {
            string sql = "update users_settings set graphstartdate=?graphstartdate,graphenddate=?graphenddate where userid=?userid and type=?type";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("userid", userid), new MySqlParameter("graphstartdate", grpahStartDate), new MySqlParameter("graphenddate", graphEndDate), new MySqlParameter("type", type));
        }

        public static bool existSettings(uint userid, int type)
        {
            bool exist = false;
            string sql = "select userid from users_Settings where userid=?userid and type=?type";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("userid", userid), new MySqlParameter("type", type));
            if (dr.HasRows)
                exist = true;
            dr.Close(); dr.Dispose();
            return exist;
        }

        public static MySqlDataReader getUserSetting(uint userid, int type)
        {
            string sql = "select userid,date_format(graphstartdate,'%Y-%m-%d') as graphstartdate,date_format(graphenddate,'%Y-%m-%d') as graphenddate from users_Settings where userid=?userid and type=?type";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("userid", userid), new MySqlParameter("type", type));
            return dr;
        }
    }
}