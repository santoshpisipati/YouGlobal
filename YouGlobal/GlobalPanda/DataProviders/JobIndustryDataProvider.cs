using MySql.Data.MySqlClient;
using System;
using System.Data;

/// <summary>
/// Summary description for JobIndustryDataProvider
/// </summary>

namespace GlobalPanda.DataProviders
{
    public class JobIndustryDataProvider
    {
        public static int insertJobIndustry(string classification, DataTable dtjobindustrysub)
        {
            int jobindustryID = 0;

            string sql = "select JobIndustryID from jobindustry where Classification=?classification";
            jobindustryID = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("classification", classification)));

            if (jobindustryID == 0)
            {
                sql = "insert into jobindustry (Classification) values (?classification); select last_insert_id() ";
                jobindustryID = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("classification", classification)));

                addJobIndustrySub(dtjobindustrysub, jobindustryID);
            }

            return jobindustryID;
        }

        public static int updateJobIndustry(uint jobindustryid, string classification)
        {
            int jobindustryID = 0;

            string sql = "select JobIndustryID from jobindustry where Classification=?classification";
            jobindustryID = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("classification", classification)));

            if (jobindustryID == 0)
            {
                sql = "update jobindustry set " +
                "Classification = ?classification " +
                "where JobIndustryID = ?jobindustryid";
                DAO.ExecuteNonQuery(sql, new MySqlParameter("jobindustryid", jobindustryid), new MySqlParameter("classification", classification));
            }

            return jobindustryID;
        }

        public static void removeJobIndustry(uint jobindustryid)
        {
            deleteJobIndustrySub(jobindustryid);

            string sql = "delete from jobindustry " +
                "where JobIndustryID = ?jobindustryid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("jobindustryid", jobindustryid));
        }

        public static MySqlDataReader searchJobIndustry(string keyword, string sortexpression)
        {
            string sql = "Select distinct Cn.jobindustryid,Cn.Classification from jobindustry Cn " +
                         "inner join jobindustrysub Sc on Cn.jobindustryid = Sc.jobindustryid where " +
                         "Cn.classification like concat_ws(?keyword,'%','%') or Sc.subclassification like concat_ws(?keyword,'%','%') order by " + sortexpression;

            return DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword));
        }

        public static MySqlDataReader searchJobIndustry(string keyword)
        {
            string sql = "select distinct Js.JobIndustrySubId,concat(Ji.Classification,' - ',Js.SubClassification) as classification from jobindustry Ji " +
                         "inner join jobindustrysub Js on Ji.jobindustryid = Js.jobindustryid where Ji.Classification like concat_ws(?keyword,'%','%') " +
                         "or Js.SubClassification like concat_ws(?keyword,'%','%') order by classification";

            return DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword));
        }

        public static MySqlDataReader getJobIndustry(uint jobindustryid)
        {
            string sql = "select JobIndustryID, Classification from jobindustry where jobindustryid = ?jobindustryid";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("jobindustryid", jobindustryid));
            return dr;
        }

        public static int getJobIndustryId(string key)
        {
            int id = 0;
            string sql = "select JobIndustryID, Classification from jobindustry where Classification = ?Classification";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("Classification", key));

            if (dr.HasRows)
            {
                dr.Read();
                id = Convert.ToInt32(DAO.getInt(dr, "JobIndustryID"));
            }
            dr.Close();
            dr.Dispose();
            return id;
        }

        public static int checkreferceJobIndustry(uint jobindustryid)
        {
            int count = 0;
            string sql = "select count(jobdetailid) as count from jobdetail where jobIndustryId = ?jobindustryid";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("jobIndustryId", jobindustryid));

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    count = Convert.ToInt32(dr["count"]);
                }
            }
            dr.Close();
            dr.Dispose();
            return count;
        }

        public static int addJobIndustrySub(string subClassification, int jobIndustryID)
        {
            int jobIndustrySubID = 0;
            string sql = "select JobIndustrySubID from jobindustrysub where SubClassification=?subClassification";
            jobIndustrySubID = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("subClassification", subClassification)));

            if (jobIndustrySubID == 0)
            {
                sql = "insert into jobindustrysub (SubClassification,JobIndustryId) values (?subClassification,?JobindustryId)";
                Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("JobindustryId", jobIndustryID), new MySqlParameter("subClassification", subClassification)));
            }

            return jobIndustrySubID;
        }

        public static int addJobIndustrySub(DataTable dtsubclassification, int jobIndustryID)
        {
            int jobIndustrySubID = 0;

            foreach (DataRow drow in dtsubclassification.Rows)
            {
                string sql = "select JobIndustrySubID from jobindustrysub where JobIndustryID=?jobindustryid and SubClassification=?subClassification";
                jobIndustrySubID = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("JobindustryId", jobIndustryID), new MySqlParameter("subClassification", drow["subclassification"].ToString())));

                if (jobIndustrySubID == 0)
                {
                    sql = "insert into jobindustrysub (SubClassification,JobIndustryId) values (?subClassification,?JobindustryId); select last_insert_id() ";
                    jobIndustrySubID = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("JobindustryId", jobIndustryID), new MySqlParameter("subClassification", drow["subclassification"].ToString())));
                }
            }
            return jobIndustrySubID;
        }

        public static DataTable getJobIndustrySub(int industryId)
        {
            string sql = "select JobIndustrySubID from jobindustrysub where JobIndustryID=?jobindustryid";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("jobindustryid", industryId));
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            dr.Dispose();
            return dt;
        }

        public static void updateJobIndustrySub(uint jobindustrysubid, string subclassification)
        {
            string sql = "update jobindustrysub set " +
                "subClassification = ?subclassification " +
                "where JobIndustrysubID = ?jobindustrysubid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("jobindustrysubid", jobindustrysubid), new MySqlParameter("subclassification", subclassification));
        }

        public static void removeJobIndustrySub(uint jobindustrysubid)
        {
            string sql = "delete from jobindustrysub where JobIndustrysubID = ?jobindustrysubid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("jobindustrysubid", jobindustrysubid));
        }

        public static void deleteJobIndustrySub(uint jobindustryid)
        {
            string sql = "delete from jobindustrysub where JobIndustryID = ?jobindustryid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("jobindustryid", jobindustryid));
        }

        public static int checkreferceJobIndustrySub(uint jobindustrysubid)
        {
            int count = 0;
            string sql = "select count(jobdetailid) as count from jobdetail where jobIndustrySubId = ?jobindustrysubid";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("jobindustrysubid", jobindustrysubid));

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    count = Convert.ToInt32(dr["count"]);
                }
            }
            return count;
        }

        public static int getCategoryId(string categoryName)
        {
            int categoryID = 0;

            string sql = "select JobIndustryID from jobindustry where Classification=?category";
            categoryID = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("category", categoryName)));
            if (categoryID == 0)
            {
                sql = "insert into jobindustry (Classification) values (?category); select last_insert_id() ";
                categoryID = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("category", categoryName)));
            }

            return categoryID;
        }

        public static int getSubCategoryId(string categoryName, int categoryID)
        {
            int subCategoryID = 0;
            string sql = "select JobIndustrySubID from jobindustrysub where SubClassification=?category and JobIndustryId=?categoryID";
            subCategoryID = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("category", categoryName), new MySqlParameter("categoryID", categoryID)));
            if (subCategoryID == 0)
            {
                sql = "insert into jobindustrysub (SubClassification,JobIndustryId) values (?category,?industryId); select last_insert_id() ";
                subCategoryID = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("industryId", categoryID), new MySqlParameter("category", categoryName)));
            }

            return subCategoryID;
        }
    }
}