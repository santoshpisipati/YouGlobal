using MySql.Data.MySqlClient;
using System;

/// <summary>
/// Summary description for EmailDataProvider
/// </summary>

namespace GlobalPanda.DataProviders
{
    public class EmailDataProvider
    {
        public static uint insertEmail(string email)
        {
            string sql = "insert into emails (email) values (?email); select last_insert_id()";
            uint emailid = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("email", email)));
            return emailid;
        }

        public static void updateEmail(uint emailid, string email)
        {
            string sql = "update emails set email = ?email where emailid = ?emailid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("emailid", emailid), new MySqlParameter("email", email));
        }

        public static void removeEmail(uint emailid)
        {
            string sql = "delete from emails where emailid = ?emailid;";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("emailid", emailid));
        }

        public static MySqlDataReader listEmailTypes()
        {
            string sql = "select emailtypeid, name from emailtypes order by name";
            return DAO.ExecuteReader(sql);
        }

        public static bool isDuplicateEmailType(string name, uint emailtypeid)
        {
            string sql = "select count(emailtypeid) as `exists` from emailtypes where name = ?name and emailtypeid != ?emailtypeid";
            uint exists = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("name", name), new MySqlParameter("emailtypeid", emailtypeid)));
            return (exists > 0);
        }

        public static uint insertEmailType(string name)
        {
            string sql = "insert into emailtypes (name) values (?name); select last_insert_id()";
            uint emailtypeid = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("name", name)));
            return emailtypeid;
        }

        public static string getEmail(uint emailId)
        {
            string sql = "select email from emails where emailId=?emailId";

            string email = (string)DAO.ExecuteScalar(sql, new MySqlParameter("emailId", emailId));

            return email;
        }

        public static string getEmailType(int typeId)
        {
            string sql = "select  name  from emailtypes where emailtypeid=?typeId";

            string type = (string)DAO.ExecuteScalar(sql, new MySqlParameter("typeId", typeId));

            return type;
        }

        public static MySqlDataReader getCombineCandidateEmails(int combineId)
        {
            string sql = "select e.email from combinecandidates cc join candidates_emails ce on cc.mastercandidateid=ce.candidateid join emails e on ce.emailid=e.emailid where combinecandidateid=?combineId union " +
                       " select e.email from combinecandidates cc join candidates_emails ce on cc.duplicatecandidateid=ce.candidateid join emails e on ce.emailid=e.emailid where combinecandidateid=?combineId";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("combineId", combineId));
            return dr;
        }
    }
}