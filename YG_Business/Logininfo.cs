using MySql.Data.MySqlClient;
using System;
using YG_DataAccess;

namespace YG_Business
{
    public class Logininfo
    {
        public static MySqlDataReader AddMember(Member member)
        {
            return AccountAccess.AddMember(member.FirstName, member.LastName, member.Password, member.PhoneNo, DateTime.Now, member.EmailId, member.ActivationLink, member.IsJobSeeker, member.IsEmployer, member.IsConsultant);
        }

        public static Member GetLoginDetails(Login login)
        {
            //Member m = new Member();
            //return m;
            return AccountAccess.GetLoginDetails(login.EmailId, login.Password);
        }

        public static int GetMemberId(string emailId, string password)
        {
            return AccountAccess.GetMemberId(emailId, password);
        }

        public static int ResetPassword(string password, int memberId)
        {
            return AccountAccess.ResetPassword(password, memberId);
        }
        public static Member GetMemberDetails(int memberId)
        {
            return AccountAccess.GetMemberDetails(memberId);
        }

        public static int UpdateMemberEmail(string email,int memberId)
        {
            return AccountAccess.UpdateMemberEmail(email, memberId);
        }

        public int ActivateUserAccount(string actKey)
        {
            return AccountAccess.ActivateUserAccount(actKey);
        }
    }
}