using MySql.Data.MySqlClient;
using System;

/// <summary>
/// Summary description for JobConsultantDataProvider
/// </summary>
namespace GlobalPanda.DataProviders
{
    public class JobConsultantDataProvider
    {
        public static uint insertJobConsultant(uint jobId, uint consultantId)
        {
            string sql = "insert into job_consultants (jobID, consultantID) values (?jobId,?consultantId); select last_insert_id()";
            uint essentialID = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("jobId", jobId), new MySqlParameter("consultantId", consultantId)));
            return essentialID;
        }

        public static void deleteJobConsultant(int jobId)
        {
            string sql = "delete from job_consultants where jobID = ?jobId ";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("jobId", jobId));
        }

        public static MySqlDataReader searchJobConsultant(uint jobId)
        {
            string sql = "select c.consultantid, c.first, c.last,c.nickname " +
              "from consultants as c " +
              "join job_consultants as j on j.consultantID = c.consultantid " +
              "where j.jobId = ?jobId " +
              "order by c.last, c.first ";
            return DAO.ExecuteReader(sql, new MySqlParameter("jobId", jobId));
        }

        public static void copyConsultant(int jobId, int newjobId)
        {
            string sql = "insert into job_consultants (jobID, consultantID)" +
            "select ?newjobId,consultantid from job_consultants where JobID=?jobId";

            DAO.ExecuteScalar(sql, new MySqlParameter("jobId", jobId), new MySqlParameter("newjobId", newjobId));
        }
    }
}