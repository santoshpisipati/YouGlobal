using MySql.Data.MySqlClient;
using System;
using YG_DataAccess;

namespace YG_Business
{
    public class Logininfo
    {
        public static MySqlDataReader AddMember(Member member)
        {
            return AccountAccess.AddMember(member.FirstName, member.LastName, member.Password, member.PhoneNo, DateTime.Now, member.EmailId, member.IsJobSeeker, member.IsEmployer, member.IsConsultant);
        }

        public static Member GetLoginDetails(Login login)
        {
            //Member m = new Member();
            //return m;
            return AccountAccess.GetLoginDetails(login.EmailId, login.Password);
        }

        public static Int32 GetMemberId(string emailId, string password)
        {
            return AccountAccess.GetMemberId(emailId, password);
        }

        public static Int32 ResetPassword(string password, Int32 memberId)
        {
            return AccountAccess.ResetPassword(password, memberId);
        }
    }
}