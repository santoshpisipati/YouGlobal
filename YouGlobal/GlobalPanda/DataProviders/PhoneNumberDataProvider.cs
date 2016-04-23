using MySql.Data.MySqlClient;
using System;

/// <summary>
/// Summary description for EmailDataProvider
/// </summary>

namespace GlobalPanda.DataProviders
{
    public class PhoneNumberDataProvider
    {
        public enum PhoneNumberType { Home = 1, Work = 2, Mobile = 3, Fax = 4 };

        public static uint insertPhoneNumber(string number, string countrycode, string areacode = "")
        {
            string sql = "insert into phonenumbers (number,countrycode,areacode) values (?number,?countrycode,?areacode); select last_insert_id()";
            uint phonenumberid = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("number", number), new MySqlParameter("countrycode", countrycode), new MySqlParameter("areacode", areacode)));
            return phonenumberid;
        }

        public static void updatePhoneNumber(uint phonenumberid, string number, string countrycode, string areacode = "")
        {
            string sql = "update phonenumbers set number = ?number, countrycode=?countrycode,areacode=?areacode where phonenumberid = ?phonenumberid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("phonenumberid", phonenumberid), new MySqlParameter("countrycode", countrycode), new MySqlParameter("number", number), new MySqlParameter("areacode", areacode));
        }

        public static void removePhoneNumber(uint phonenumberid)
        {
            string sql = "delete from phonenumbers where phonenumberid = ?phonenumberid;";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("phonenumberid", phonenumberid));
        }

        public static MySqlDataReader listPhoneNumberTypes()
        {
            string sql = "select phonenumbertypeid, name from phonenumbertypes order by name";
            return DAO.ExecuteReader(sql);
        }

        public static bool isDuplicatePhoneNumberType(string name, uint phonenumbertypeid)
        {
            string sql = "select count(phonenumbertypeid) as `exists` from phonenumbertypes where name = ?name and phonenumbertypeid != ?phonenumbertypeid";
            uint exists = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("name", name), new MySqlParameter("phonenumbertypeid", phonenumbertypeid)));
            return (exists > 0);
        }

        public static string getPhoneNumber(uint phonenumberid)
        {
            string sql = "select number from phonenumbers where phonenumberid=?phonenumberid";
            string number = (string)DAO.ExecuteScalar(sql, new MySqlParameter("phonenumberid", phonenumberid));
            return number;
        }

        public static string getPhoneNumberType(int phonenumbertypeid)
        {
            string sql = "select  name from phonenumbertypes where phonenumbertypeid=?phonenumbertypeid";
            string type = (string)DAO.ExecuteScalar(sql, new MySqlParameter("phonenumbertypeid", phonenumbertypeid));
            return type;
        }
    }
}