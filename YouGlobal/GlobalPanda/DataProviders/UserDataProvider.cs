using GlobalPanda.BusinessInfo;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Summary description for MenuItemDataProvider
/// </summary>
namespace GlobalPanda.DataProviders
{
    public class UserDataProvider
    {
        private static string hashPassword(string username, string password)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            UTF8Encoding encoder = new UTF8Encoding();
            byte[] bytes = md5Hasher.ComputeHash(encoder.GetBytes(username + password));
            return Convert.ToBase64String(bytes);
        }

        public static uint insertUser(string username, string password, uint userroleid, uint tenantid)
        {
            string sql = "insert into users (username, password, active, userroleid, tenantid) values (?username, ?password, 1, ?userroleid, ?tenantid); select last_insert_id()";
            uint userid = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("username", username), new MySqlParameter("password", hashPassword(username, password)), new MySqlParameter("userroleid", userroleid), new MySqlParameter("tenantid", tenantid)));
            return userid;
        }

        public static uint insertUser(string username, string password, uint userroleid, uint tenantid, int utcoffset, string timezoneid)
        {
            string sql = "insert into users (username, password, active, userroleid, tenantid,utcoffset,timezoneid) values (?username, ?password, 1, ?userroleid, ?tenantid,?utcoffset,?timezoneid); select last_insert_id()";
            uint userid = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("username", username), new MySqlParameter("password", hashPassword(username, password)), new MySqlParameter("userroleid", userroleid),
                new MySqlParameter("tenantid", tenantid), new MySqlParameter("utcoffset", utcoffset), new MySqlParameter("timezoneid", timezoneid)));
            return userid;
        }

        public static MySqlDataReader searchUsers(string keyword, uint tenantid)
        {
            string sql = "select u.userid, u.username, u.active, ur.name as userrole, u.utcoffset " +
                "from users as u " +
                "join userroles as ur on ur.userroleid = u.userroleid " +
                "where u.tenantid = ?tenantid " +
                "and u.username like concat_ws(?keyword,'%','%') " +
                "order by u.username ";
            return DAO.ExecuteReader(sql, new MySqlParameter("tenantid", tenantid), new MySqlParameter("keyword", keyword));
        }

        public static MySqlDataReader listAllUsers(uint tenantid)
        {
            string sql = "select u.userid, u.username, u.active, ur.name as userrole, u.utcoffset " +
                "from users as u " +
                "join userroles as ur on ur.userroleid = u.userroleid " +
                "where u.tenantid = ?tenantid " +
                "order by u.username ";
            return DAO.ExecuteReader(sql, new MySqlParameter("tenantid", tenantid));
        }

        public static MySqlDataReader getUser(string username, string password)
        {
            string sql = "select u.userid, u.username, u.active, u.userroleid, u.timezoneid, ur.name as userrolename, u.tenantid, t.name as tenantname, u.utcoffset " +
                "from users as u " +
                "join userroles as ur on ur.userroleid = u.userroleid " +
                "join tenants as t on t.tenantid = u.tenantid " +
                "where u.username=?username and u.password=?password";
            MySqlDataReader drUser = DAO.ExecuteReader(sql, new MySqlParameter("username", username), new MySqlParameter("password", hashPassword(username, password)));
            return drUser;
        }

        public static MySqlDataReader getUser(uint? userid)
        {
            string sql = "select u.userid, u.username, u.password, u.active, u.userroleid, u.timezoneid, ur.name as userrole, u.utcoffset,u.timezoneid " +
                "from users as u " +
                "join userroles as ur on ur.userroleid = u.userroleid " +
                "where u.userid = ?userid";
            MySqlDataReader drUser = DAO.ExecuteReader(sql, new MySqlParameter("userid", userid));
            return drUser;
        }

        public static DataSet getUser(string username)
        {
            string sql = "select userid, username from users where username=?username";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("username", username));
            DataTable dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            return ds;
        }

        public static DataTable getUserIPAddress(uint? userid)
        {
            string sql = "select c.consultantid, ip.ipaddress,  ip.consultants_ipaddressid from consultants as c join consultants_ipaddress as ip on c.consultantid = ip.consultantid where c.userid = ?userid";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("userid", userid));
            DataTable dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public static void updateUser(uint userid, bool active, int utcoffset, string timezoneid)
        {
            MySqlDataReader reader = getUser(userid);

            string sql = "update users set active = ?active, utcoffset = ?utcoffset, timezoneid = ?timezoneid where userid = ?userid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("userid", userid), new MySqlParameter("active", active), new MySqlParameter("utcoffset", utcoffset), new MySqlParameter("timezoneid", timezoneid));

            if (reader.HasRows)
            {
                reader.Read();
                HistoryDataProvider history = new HistoryDataProvider();
                HistoryInfo historyInfo = new HistoryInfo();
                historyInfo.UserId = GPSession.UserId;
                historyInfo.ModuleId = (int)HistoryInfo.Module.User;
                historyInfo.TypeId = (int)HistoryInfo.ActionType.Edit;
                historyInfo.RecordId = userid;
                historyInfo.ModifiedDate = DateTime.Now;
                historyInfo.Details = new List<HistoryDetailInfo>();

                if (DAO.getBool(reader, "active") != active)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "active", OldValue = DAO.getBool(reader, "active").ToString(), NewValue = active.ToString() });
                }
                if (DAO.getInt(reader, "utcoffset") != utcoffset)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "utcoffset", OldValue = DAO.getInt(reader, "utcoffset").ToString(), NewValue = utcoffset.ToString() });
                }

                if (historyInfo.Details.Count > 0)
                {
                    history.insertHistory(historyInfo);
                }
            }
        }

        public static void updateUsernamePassword(uint userid, string username, string password)
        {
            string sql = "update users set username = ?username, password = ?password where userid = ?userid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("username", username), new MySqlParameter("password", hashPassword(username, password)), new MySqlParameter("userid", userid));
        }

        public static void updatePassword(uint userid, string password)
        {
            string sql = "select username from users where userid = ?userid";
            string username = Convert.ToString(DAO.ExecuteScalar(sql, new MySqlParameter("userid", userid)));
            updateUsernamePassword(userid, username, password);

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo historyInfo = new HistoryInfo();
            historyInfo.UserId = GPSession.UserId;
            historyInfo.ModuleId = (int)HistoryInfo.Module.User;
            historyInfo.TypeId = (int)HistoryInfo.ActionType.Edit;
            historyInfo.RecordId = userid;
            historyInfo.ModifiedDate = DateTime.Now;
            historyInfo.Details = new List<HistoryDetailInfo>();

            historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "password", NewValue = "password changed" });
            history.insertHistory(historyInfo);
        }

        public static bool isUsernameUnique(string username)
        {
            string sql = "select count(userid) as `count` from users where username = ?username";
            int count = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("username", username)));
            if (count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool isUsernameUnique(string username, uint userid)
        {
            string sql = "select count(userid) as `count` from users where username = ?username and userid != ?userid";
            int count = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("username", username), new MySqlParameter("userid", userid)));
            if (count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static int getUserutcoffset(uint userid)
        {
            string sql = "select utcoffset from users where userid = ?userid";
            int utcoffset = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("userid", userid)));
            return utcoffset;
        }
    }
}