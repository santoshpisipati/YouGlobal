using MySql.Data.MySqlClient;
using System;
using YG_Business;

namespace YG_DataAccess
{
    public class AccountAccess
    {
        public static MySqlDataReader AddMember(string firstName, string lastName, string password, string phnNumber, DateTime date, string emailId, bool isJobSeeker, bool IsEmployer, bool IsConsultant)
        {
            string sql = "insert into `globalpanda`.`members`(`firstname`,`lastname`,`email`,`password`,`phonenumber`,`isactive`,`createdon`,`isJobSeeker`,`IsEmployer`,`IsConsultant`)" +
                         "values (?firstname,?lastname,?email,?password,?phonenumber,?isactive,?createdon,?isJobSeeker,?IsEmployer,?IsConsultant)";

            return DataAccess.ExecuteReader(sql, new MySqlParameter("firstName", firstName)
                                               , new MySqlParameter("lastName", lastName)
                                               , new MySqlParameter("email", emailId)
                                               , new MySqlParameter("password", password)
                                               , new MySqlParameter("phonenumber", phnNumber)
                                               , new MySqlParameter("isactive", true)
                                               , new MySqlParameter("createdon", DateTime.Now)
                                               , new MySqlParameter("isJobSeeker", isJobSeeker)
                                               , new MySqlParameter("IsEmployer", IsEmployer)
                                               , new MySqlParameter("IsConsultant", IsConsultant));
        }

        public static Member GetLoginDetails(string emailId, string password)
        {
            Member member = new Member();
            string sql = string.Format("select * from members where email='{0}' and password='{1}'", emailId, password);
            MySqlDataReader reader = DataAccess.ExecuteReader(sql);
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    member.MemberId = Convert.ToInt32(DataAccess.getInt(reader, "memberId"));
                    member.FirstName = DataAccess.getString(reader, "firstname");
                    member.LastName = DataAccess.getString(reader, "lastname");
                    member.EmailId = DataAccess.getString(reader, "email");
                    member.Password = DataAccess.getString(reader, "password");
                    member.PhoneNo = DataAccess.getString(reader, "phonenumber");
                    member.CreatedOn = Convert.ToDateTime(DataAccess.getDateTime(reader, "createdon"));

                    if (!string.IsNullOrEmpty(reader["isactive"].ToString()) && (Convert.ToString(reader["isactive"]) == "1"))
                    { member.isActive = true; }
                    else
                    { member.isActive = false; }
                    if (!string.IsNullOrEmpty(reader["IsConsultant"].ToString()) && (Convert.ToString(reader["IsConsultant"]) == "1"))
                    { member.IsConsultant = true; }
                    else
                    { member.IsConsultant = false; }
                    if (!string.IsNullOrEmpty(reader["IsJobSeeker"].ToString()) && (Convert.ToString(reader["IsJobSeeker"]) == "1"))
                    { member.IsJobSeeker = true; }
                    else
                    { member.IsJobSeeker = false; }
                    if (!string.IsNullOrEmpty(reader["IsEmployer"].ToString()) && (Convert.ToString(reader["IsEmployer"]) == "1"))
                    { member.IsEmployer = true; }
                    else
                    { member.IsEmployer = false; }
                }
            }
            reader.Close();
            reader.Dispose();
            return member;
        }

        public static int GetMemberId(string emailId, string password)
        {
            int id = 0;
            if (string.IsNullOrEmpty(password))
            {
                string sql = string.Format("select memberId from members where email='{0}'", emailId);
                MySqlDataReader reader = DataAccess.ExecuteReader(sql);
                id = Convert.ToInt32(DataAccess.ExecuteScalar(sql));
            }
            else
            {
                string sql = string.Format("select memberId from members where email='{0}' and password= '{1}'", emailId, password);
                MySqlDataReader reader = DataAccess.ExecuteReader(sql);
                id = Convert.ToInt32(DataAccess.ExecuteScalar(sql));
            }
            return id;
        }

        public static int ResetPassword(string password, int memberId)
        {
            string sql = "update members set password=?password where memberId=?memberId";
            int id = 0;
            id = Convert.ToInt32(DataAccess.ExecuteNonQuery(sql, new MySqlParameter("password", password), new MySqlParameter("memberId", memberId)));
            return id;
        }

        public static Member GetMemberDetails(int memberId)
        {
            Member member = new Member();
            string sql = string.Format("select * from members where memberId='{0}'", memberId);
            MySqlDataReader reader = DataAccess.ExecuteReader(sql);
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    member.MemberId = Convert.ToInt32(DataAccess.getInt(reader, "memberId"));
                    member.FirstName = DataAccess.getString(reader, "firstname");
                    member.LastName = DataAccess.getString(reader, "lastname");
                    member.EmailId = DataAccess.getString(reader, "email");
                    member.Password = DataAccess.getString(reader, "password");
                    member.PhoneNo = DataAccess.getString(reader, "phonenumber");
                    member.CreatedOn = Convert.ToDateTime(DataAccess.getDateTime(reader, "createdon"));

                    if (!string.IsNullOrEmpty(reader["isactive"].ToString()) && (Convert.ToString(reader["isactive"]) == "1"))
                    { member.isActive = true; }
                    else
                    { member.isActive = false; }
                    if (!string.IsNullOrEmpty(reader["IsConsultant"].ToString()) && (Convert.ToString(reader["IsConsultant"]) == "1"))
                    { member.IsConsultant = true; }
                    else
                    { member.IsConsultant = false; }
                    if (!string.IsNullOrEmpty(reader["IsJobSeeker"].ToString()) && (Convert.ToString(reader["IsJobSeeker"]) == "1"))
                    { member.IsJobSeeker = true; }
                    else
                    { member.IsJobSeeker = false; }
                    if (!string.IsNullOrEmpty(reader["IsEmployer"].ToString()) && (Convert.ToString(reader["IsEmployer"]) == "1"))
                    { member.IsEmployer = true; }
                    else
                    { member.IsEmployer = false; }
                }
            }
            reader.Close();
            reader.Dispose();
            return member;
        }

        public static int UpdateMemberEmail(string email, int memberId)
        {
            string sql = "update members set email=?email where memberId=?memberId";
            int id = Convert.ToInt32(DataAccess.ExecuteNonQuery(sql, new MySqlParameter("email", email), new MySqlParameter("memberId", memberId)));
            return id;
        }
    }
}