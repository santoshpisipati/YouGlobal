using MySql.Data.MySqlClient;
using System;

/// <summary>
/// Summary description for JobDesirableCriteriaDataProvider
/// </summary>
namespace GlobalPanda.DataProviders
{
    public class JobDesirableCriteriaDataProvider
    {
        public static uint insertDesirableCriteria(uint jobId, string description, uint answerLength, int sortorder)
        {
            string sql = "insert into desirablecriteria (JobId, Description,AnswerLength,sortorder) values (?jobId,?description,  ?answerLength,?sortorder); select last_insert_id()";
            uint desirableCriteriaID = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("jobId", jobId), new MySqlParameter("description", description), new MySqlParameter("answerLength", answerLength), new MySqlParameter("sortorder", sortorder)));
            return desirableCriteriaID;
        }

        public static void updateDesirableCriteria(uint desirableCriteriaID, string description, uint answerLength, int sortorder)
        {
            string sql = "update desirablecriteria set Description=?description ,AnswerLength=?answerLength where DesirableCriteriaID=?desirableCriteriaID,sortorder=?sortorder;";
            Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("desirableCriteriaID", desirableCriteriaID), new MySqlParameter("description", description), new MySqlParameter("answerLength", answerLength), new MySqlParameter("sortorder", sortorder)));
        }

        public static void deleteDesirableCriteria(int jobId)
        {
            string sql = "delete from desirablecriteria where jobId=?jobId;";
            DAO.ExecuteScalar(sql, new MySqlParameter("jobId", jobId));
        }

        public static void copyDesirableCriteria(int jobId, int newjobId)
        {
            string sql = "insert into desirablecriteria (JobId, Description,AnswerLength,sortorder)" +
            "select ?newjobId,Description,AnswerLength,sortorder from desirablecriteria where JobID=?jobId";

            DAO.ExecuteScalar(sql, new MySqlParameter("jobId", jobId), new MySqlParameter("newjobId", newjobId));
        }

        public static MySqlDataReader searchDesirableCriteria(uint jobId)
        {
            string sql = "select DesirableCriteriaID,JobID,Description,AnswerLength,sortorder,DesirableCriteriaID as CriteriaId from desirablecriteria where JobID=?jobId";
            return DAO.ExecuteReader(sql, new MySqlParameter("jobId", jobId));
        }
    }
}