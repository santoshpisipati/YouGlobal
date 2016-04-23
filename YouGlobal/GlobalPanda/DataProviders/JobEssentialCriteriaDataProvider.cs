using MySql.Data.MySqlClient;
using System;

/// <summary>
/// Summary description for JobEssentialCriteria
/// </summary>
namespace GlobalPanda.DataProviders
{
    public class JobEssentialCriteriaDataProvider
    {
        public static uint insertEssentialCriteria(uint jobId, string description, uint answerLength, int sortorder)
        {
            string sql = "insert into essentialcriteria (JobId, Description,AnswerLength,sortorder) values (?jobId,?description,  ?answerLength,?sortorder); select last_insert_id()";
            uint essentialID = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("jobId", jobId), new MySqlParameter("description", description), new MySqlParameter("answerLength", answerLength), new MySqlParameter("sortorder", sortorder)));
            return essentialID;
        }

        public static void updateEssentialCriteria(uint essentialId, string description, uint answerLength, int sortorder)
        {
            string sql = "update essentialcriteria set Description=?description ,AnswerLength=?answerLength,sortorder=?sortorder where EssentialCriteriaId=?essentialId;";
            DAO.ExecuteScalar(sql, new MySqlParameter("essentialId", essentialId), new MySqlParameter("description", description), new MySqlParameter("answerLength", answerLength), new MySqlParameter("sortorder", sortorder));
        }

        public static void deleteEssentialCriteria(int jobId)
        {
            string sql = "delete from essentialcriteria where jobId=?jobId;";
            DAO.ExecuteScalar(sql, new MySqlParameter("jobId", jobId));
        }

        public static void copyEssentialCriteria(int jobId, int newjobId)
        {
            string sql = "insert into essentialcriteria (JobId, Description,AnswerLength,sortorder)" +
            "select ?newjobId,Description,AnswerLength,sortorder from essentialcriteria where JobID=?jobId";

            DAO.ExecuteScalar(sql, new MySqlParameter("jobId", jobId), new MySqlParameter("newjobId", newjobId));
        }

        public static MySqlDataReader searchEssentialCriteria(uint jobId)
        {
            string sql = "select EssentialCriteriaId,JobID,Description,AnswerLength,sortorder,EssentialCriteriaId as CriteriaId from essentialcriteria where JobID=?jobId";
            return DAO.ExecuteReader(sql, new MySqlParameter("jobId", jobId));
        }
    }
}