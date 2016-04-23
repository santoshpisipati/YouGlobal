using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using YG_DataAccess;

namespace YG_Business
{
    public class Job
    {
        /// <summary>
        /// Convert a string with specified date format to datetime.
        /// </summary>
        /// <param name="strDate"></param>
        /// <param name="strFormat">Ex: dd/MM/yyyy </param>
        /// <returns>Return a nullable datetime. Null if invalid format</returns>
        public DateTime? ParseDate(string strDate, string strFormat)
        {
            DateTime? dFormattedDate = null;
            System.Globalization.CultureInfo MyCultureInfo = new System.Globalization.CultureInfo("en-AU");
            System.Globalization.DateTimeFormatInfo dtfi = new System.Globalization.DateTimeFormatInfo();
            dtfi.ShortDatePattern = strFormat;
            MyCultureInfo.DateTimeFormat = dtfi;

            DateTime tmpDate;
            if (DateTime.TryParseExact(strDate, strFormat, MyCultureInfo, System.Globalization.DateTimeStyles.None, out tmpDate))
            { dFormattedDate = tmpDate; }
            return dFormattedDate;
        }

        public List<JobInfo> ListJobs()
        {
            List<JobInfo> lst = new List<JobInfo>();
            JobDataAccess job = new JobDataAccess();
            JobInfo info;
            MySqlDataReader reader = job.ListJobs();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    info = new JobInfo();
                    info.JobId = Convert.ToInt32(DataAccess.getInt(reader, "JobId"));
                    MySqlDataReader locationDr = job.getJobLocation(info.JobId);
                    int i = 1;
                    while (locationDr.Read())
                    {
                        string input = string.Empty;
                        string regex = "(\\[.*\\])|(\".*\")|('.*')|(\\(.*\\))";
                        if (i == 1)
                        {
                            input = (DataAccess.getString(locationDr, "location") == null ? "" : DataAccess.getString(locationDr, "location")) + "" + info.Location;
                            info.Location = Regex.Replace(input, regex, "");
                        }
                        else
                        {
                            input = (DataAccess.getString(locationDr, "location") == null ? "" : DataAccess.getString(locationDr, "location")) + " <br /> " + info.Location;
                            info.Location = Regex.Replace(input, regex, "");
                        }
                        i++;
                    }
                    locationDr.Close();
                    locationDr.Dispose();
                    info.SearchTitle = DataAccess.getString(reader, "searchtitle");
                    info.Title = DataAccess.getString(reader, "jobtitle");
                    info.ReferenceNo = DataAccess.getString(reader, "referenceno");
                    //info.Location = DataAccess.getString(reader, "regionname");
                    DateTime createdDate = Convert.ToDateTime(DataAccess.getDateTime(reader, "createddate") == null ? DateTime.MinValue : DataAccess.getDateTime(reader, "createddate"));
                    info.CreatedDate = createdDate;
                    info.ModifiedDate = Convert.ToDateTime(DataAccess.getDateTime(reader, "modifieddate") == null ? DateTime.MinValue : DataAccess.getDateTime(reader, "modifieddate"));
                    info.Summary = DataAccess.getString(reader, "summary");
                    info.IsModified = Convert.ToBoolean(DataAccess.getBool(reader, "ismodified"));

                    lst.Add(info);
                }
            }
            reader.Close();
            reader.Dispose();
            return lst;
        }

        public List<JobInfo> SearchJobs(string industry, string role, string location, string workArrangement, string keywords, string isco)
        {
            List<JobInfo> lst = new List<JobInfo>();
            JobDataAccess job = new JobDataAccess();
            JobInfo info;
            MySqlDataReader reader = job.SearchJobs(industry, role, location, workArrangement, keywords, isco);

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    info = new JobInfo();
                    info.JobId = Convert.ToInt32(DataAccess.getInt(reader, "JobId"));
                    MySqlDataReader locationDr = job.getJobLocation(info.JobId);
                    if (locationDr.HasRows)
                    {
                        int i = 1;
                        while (locationDr.Read())
                        {
                            string input = string.Empty;
                            string regex = "(\\[.*\\])|(\".*\")|('.*')|(\\(.*\\))";
                            if (i == 1)
                            {
                                input = (DataAccess.getString(locationDr, "location") == null ? "" : DataAccess.getString(locationDr, "location")) + "" + info.Location;
                                info.Location = Regex.Replace(input, regex, "");
                            }
                            else
                            {
                                input = (DataAccess.getString(locationDr, "location") == null ? "" : DataAccess.getString(locationDr, "location")) + " <br /> " + info.Location;
                                info.Location = Regex.Replace(input, regex, "");
                            }
                            i++;
                        }
                    }
                    locationDr.Close();
                    locationDr.Dispose();
                    info.SearchTitle = DataAccess.getString(reader, "searchtitle");
                    info.Title = DataAccess.getString(reader, "jobtitle");
                    info.ReferenceNo = DataAccess.getString(reader, "referenceno");
                    //info.Location = DataAccess.getString(reader, "regionname");
                    DateTime createdDate = Convert.ToDateTime(DataAccess.getDateTime(reader, "createddate") == null ? DateTime.MinValue : DataAccess.getDateTime(reader, "createddate"));
                    info.CreatedDate = createdDate;
                    info.ModifiedDate = Convert.ToDateTime(DataAccess.getDateTime(reader, "modifieddate") == null ? DateTime.MinValue : DataAccess.getDateTime(reader, "modifieddate"));
                    info.Summary = DataAccess.getString(reader, "summary");
                    info.IsModified = Convert.ToBoolean(DataAccess.getBool(reader, "ismodified"));
                    lst.Add(info);
                }
            }
            reader.Close();
            reader.Dispose();
            return lst;
        }

        public List<JobInfo> HotJobs(int limit)
        {
            List<JobInfo> lst = new List<JobInfo>();
            JobDataAccess job = new JobDataAccess();
            JobInfo info;
            MySqlDataReader reader = job.HotJobs(limit);
            //DataTable dt = new DataTable();
            //dt.Load(reader);

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    info = new JobInfo();
                    info.JobId = Convert.ToInt32(DataAccess.getInt(reader, "JobId"));
                    info.SearchTitle = DataAccess.getString(reader, "searchtitle");
                    info.Title = DataAccess.getString(reader, "jobtitle");
                    info.ReferenceNo = DataAccess.getString(reader, "referenceno");
                    info.Location = DataAccess.getString(reader, "regionname");
                    //info.ModifiedDate =Convert.ToDateTime( DataAccess.getDateTime(reader, "createddate"));
                    info.Summary = DataAccess.getString(reader, "summary");
                    info.JobContent = DataAccess.getString(reader, "jobContent");

                    lst.Add(info);
                }
            }
            reader.Close();
            reader.Dispose();
            return lst;
        }

        public JobInfo GetJobInfoByReferenceNo(string refeNo)
        {
            JobInfo info = new JobInfo();
            JobDataAccess job = new JobDataAccess();
            MySqlDataReader reader = job.GetJobByReferenceNo(refeNo);
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    info.JobId = Convert.ToInt32(DataAccess.getInt(reader, "JobDetailID"));
                    MySqlDataReader locationDr = job.getJobLocation(info.JobId);
                    if (locationDr.HasRows)
                    {
                        int i = 1;
                        while (locationDr.Read())
                        {
                            string input = string.Empty;
                            string regex = "(\\[.*\\])|(\".*\")|('.*')|(\\(.*\\))";
                            if (i == 1)
                            {
                                input = (DataAccess.getString(locationDr, "location") == null ? "" : DataAccess.getString(locationDr, "location")) + "" + info.Location;
                                info.Location = Regex.Replace(input, regex, "");
                            }
                            else
                            {
                                input = (DataAccess.getString(locationDr, "location") == null ? "" : DataAccess.getString(locationDr, "location")) + " <br /> " + info.Location;
                                info.Location = Regex.Replace(input, regex, "");
                            }
                            i++;
                        }
                    }
                    locationDr.Close();
                    locationDr.Dispose();

                    info.SearchTitle = DataAccess.getString(reader, "searchtitle");
                    info.Title = DataAccess.getString(reader, "title");
                    info.ReferenceNo = DataAccess.getString(reader, "referenceno");

                    //info.ModifiedDate =Convert.ToDateTime( DataAccess.getDateTime(reader, "createddate"));
                    info.Summary = DataAccess.getString(reader, "summary");
                    info.Bullet1 = DataAccess.getString(reader, "bullet1");
                    info.Bullet2 = DataAccess.getString(reader, "bullet2");
                    info.Bullet3 = DataAccess.getString(reader, "bullet3");
                    info.JobContent = DataAccess.getString(reader, "jobContent");
                    info.SalaryCurrency = DataAccess.getString(reader, "salaryCurrency");
                    info.SalaryMin = DataAccess.getString(reader, "salaryMin");
                    info.SalaryMax = DataAccess.getString(reader, "salaryMax");
                    info.AdFooter = DataAccess.getString(reader, "adFooter");
                }
            }
            reader.Close();
            reader.Dispose();
            return info;
        }

        public DataTable GetEssentialCriteria(int jobId)
        {
            DataTable dt = new DataTable();
            JobDataAccess job = new JobDataAccess();
            MySqlDataReader reader = job.GetEssentialCriteria(jobId);
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public DataTable GetDesirableCriteria(int jobId)
        {
            DataTable dt = new DataTable();
            JobDataAccess job = new JobDataAccess();
            MySqlDataReader reader = job.GetDesirableCriteria(jobId);
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public DataTable GetJobConsultant(int jobId)
        {
            DataTable dt = new DataTable();
            JobDataAccess job = new JobDataAccess();
            MySqlDataReader reader = job.GetJobConsultant(jobId);
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public MySqlDataReader GetJobsCountByIndustry()
        {
            JobDataAccess job = new JobDataAccess();
            MySqlDataReader reader = job.GetJobsCountByIndustry();
            return reader;
        }

        public MySqlDataReader GetJobsCountByOccupation()
        {
            JobDataAccess job = new JobDataAccess();
            MySqlDataReader reader = job.GetJobsCountByOccupation();
            return reader;
        }
    }
}