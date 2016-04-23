using GlobalPanda.BusinessInfo;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

/// <summary>
/// Summary description for JobDetailcs
/// </summary>
namespace GlobalPanda.DataProviders
{
    public class JobDetailDataProvider
    {
        public static uint insertJobDetail(JobDetailInfo info)
        {
            string sql = "insert into jobdetail (locationID, typeID,salaryMin,salaryMax,ReferenceNo,title,subHeading,searchTitle,bullet1,bullet2,bullet3,summary,jobContent,websiteURL,adFooter,isResidency,status,isApprove,clientId,jobIndustryId,jobIndustrySubId, createdDate,createdBy,hotJob,salaryCurrency,salaryFrequency,published,ismodified,isco08id,isicrev4id)";
            sql += "values (?locationID, ?typeID,?salaryMin,?salaryMax,?ReferenceNo,?title,?subHeading,?searchTitle,?bullet1,?bullet2,?bullet3,?summary,?jobContent,?websiteURL,?adFooter,?isResidency,?status,?isApprove,?clientId,?jobIndustryId,?jobIndustrySubId,?createdDate,?createdBy,?hotJob,?salaryCurrency,?salaryFrequency,1,1,?isco08id,?isicrev4id); select last_insert_id()";
            MySqlParameter[] param ={
             new MySqlParameter("locationID",info.LocationId),
             new MySqlParameter("typeID", info.TypeId),
             new MySqlParameter("salaryMin", info.SalaryMin),
             new MySqlParameter("salaryMax",info.SalaryMax),
             new MySqlParameter("ReferenceNo",info.ReferenceNo),
             new MySqlParameter("title",info.Title),
             new MySqlParameter("subHeading",info.SubHeading),
             new MySqlParameter("searchTitle",info.SearchTitle),
             new MySqlParameter("bullet1",info.Bullet1),
             new MySqlParameter("bullet2",info.Bullet2),
             new MySqlParameter("bullet3",info.Bullet3),
             new MySqlParameter("summary",info.Summary),
             new MySqlParameter("jobContent",info.JobContent),
             new MySqlParameter("websiteURL",info.WebsiteURL),
             new MySqlParameter("adFooter",info.AdFooter),
             new MySqlParameter("isResidency",info.IsResidency),
             new MySqlParameter("status",info.Status),
             new MySqlParameter("isApprove",info.IsApprove),
             new MySqlParameter("clientId",info.ClientId),
             new MySqlParameter("jobIndustryId",info.JobIndustryId),
             new MySqlParameter("jobIndustrySubId",info.JobIndustrySubId),
             new MySqlParameter("createdDate",info.CreatedDate),
              new MySqlParameter("createdBy",info.CreatedBy),
              new MySqlParameter("hotJob",info.HotJob),
              new MySqlParameter("salaryCurrency",info.SalaryCurrency),
              new MySqlParameter("salaryFrequency",info.SalaryFrequency),
              new MySqlParameter("isco08id",info.ISCO08Id),
              new MySqlParameter("isicrev4id",info.ISICRev4Id)
            };
            uint jobId = Convert.ToUInt32(DAO.ExecuteScalar(sql, param));

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo historyInfo = new HistoryInfo();
            historyInfo.UserId = GPSession.UserId;
            historyInfo.ModuleId = (int)HistoryInfo.Module.Job;
            historyInfo.TypeId = (int)HistoryInfo.ActionType.Add;
            historyInfo.RecordId = jobId;
            historyInfo.ModifiedDate = DateTime.Now;
            historyInfo.Details = new List<HistoryDetailInfo>();
            historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "All", NewValue = "title:" + info.Title + " ,referenceno:" + info.ReferenceNo });
            history.insertHistory(historyInfo);

            uint essid;
            foreach (EssentialCriteriaInfo essInfo in info.EssentialCriteriaList)
            {
                essid = JobEssentialCriteriaDataProvider.insertEssentialCriteria(jobId, essInfo.Description, essInfo.AnswerLength, essInfo.SortOrder);
                historyInfo = new HistoryInfo();
                historyInfo.UserId = GPSession.UserId;
                historyInfo.ModuleId = (int)HistoryInfo.Module.Essentialcriteria;
                historyInfo.TypeId = (int)HistoryInfo.ActionType.Add;
                historyInfo.RecordId = essid;
                historyInfo.ModifiedDate = DateTime.Now;
                historyInfo.Details = new List<HistoryDetailInfo>();
                historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "All", NewValue = "Description:" + essInfo.Description + " ,AnswerLength:" + essInfo.AnswerLength });
                history.insertHistory(historyInfo);
            }
            uint dessid;
            foreach (DesirableCriteriaInfo desiInfo in info.DesirableCriteriaList)
            {
                dessid = JobDesirableCriteriaDataProvider.insertDesirableCriteria(jobId, desiInfo.Description, desiInfo.AnswerLength, desiInfo.SortOrder);
                historyInfo = new HistoryInfo();
                historyInfo.UserId = GPSession.UserId;
                historyInfo.ModuleId = (int)HistoryInfo.Module.Desirablecriteria;
                historyInfo.TypeId = (int)HistoryInfo.ActionType.Add;
                historyInfo.RecordId = dessid;
                historyInfo.ModifiedDate = DateTime.Now;
                historyInfo.Details = new List<HistoryDetailInfo>();
                historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "All", NewValue = "Description:" + desiInfo.Description + " ,AnswerLength:" + desiInfo.AnswerLength });
                history.insertHistory(historyInfo);
            }

            foreach (JobConsultantInfo consult in info.ConsultantList)
            {
                JobConsultantDataProvider.insertJobConsultant(jobId, consult.ConsultantId);
            }

            foreach (JobLocation location in info.LocationList)
            {
                JobLocationDataProvider.insertJobLocation(jobId, location.LocationId, location.Locationtype);
            }

            return jobId;
        }

        public static void updateJobDetail(JobDetailInfo info)
        {
            //MySqlDataReader reader = getJobById(Convert.ToUInt32(info.JobId));

            string sql = "update jobdetail set locationID=?locationID,typeID=?typeID,salaryMin=?salaryMin,salaryMax=?salaryMax,ReferenceNo=?ReferenceNo,title=?title,subHeading=?subHeading,searchTitle=?searchTitle,bullet1=?bullet1" +
                ",bullet2=?bullet2,bullet3=?bullet3,summary=?summary,jobContent=?jobContent,websiteURL=?websiteURL,adFooter=?adFooter,isResidency=?isResidency,status=?status,clientId=?clientId,jobIndustryId=?jobIndustryId" +
                ",jobIndustrySubId=?jobIndustrySubId,modifiedDate=?modifiedDate,modifiedBy=?modifiedBy,hotJob=?hotJob,salaryCurrency=?salaryCurrency,salaryFrequency=?salaryFrequency,published=?published, isco08id=?isco08id,isicrev4id=?isicrev4id where JobDetailID=?jobId";

            MySqlParameter[] param ={
             new MySqlParameter("locationID",info.LocationId),
             new MySqlParameter("typeID", info.TypeId),
             new MySqlParameter("salaryMin", info.SalaryMin),
             new MySqlParameter("salaryMax",info.SalaryMax),
             new MySqlParameter("ReferenceNo",info.ReferenceNo),
             new MySqlParameter("title",info.Title),
             new MySqlParameter("subHeading",info.SubHeading),
             new MySqlParameter("searchTitle",info.SearchTitle),
             new MySqlParameter("bullet1",info.Bullet1),
             new MySqlParameter("bullet2",info.Bullet2),
             new MySqlParameter("bullet3",info.Bullet3),
             new MySqlParameter("summary",info.Summary),
             new MySqlParameter("jobContent",info.JobContent),
             new MySqlParameter("websiteURL",info.WebsiteURL),
             new MySqlParameter("adFooter",info.AdFooter),
             new MySqlParameter("isResidency",info.IsResidency),
             new MySqlParameter("status",info.Status),
             new MySqlParameter("clientId",info.ClientId),
             new MySqlParameter("jobIndustryId",info.JobIndustryId),
             new MySqlParameter("jobIndustrySubId",info.JobIndustrySubId),
             new MySqlParameter("modifiedDate",info.ModifiedDate),
             new MySqlParameter("modifiedBy",info.ModifiedBy),
             new MySqlParameter("hotJob",info.HotJob),
             new MySqlParameter("salaryCurrency",info.SalaryCurrency),
             new MySqlParameter("salaryFrequency",info.SalaryFrequency),
             new MySqlParameter("published",info.Published),
             new MySqlParameter("jobId",info.JobId),
             new MySqlParameter("isco08id",info.ISCO08Id),
             new MySqlParameter("isicrev4id",info.ISICRev4Id)
            };

            DAO.ExecuteScalar(sql, param);

            #region History

            //if (reader.HasRows)
            //{
            //    reader.Read();
            //    HistoryDataProvider history = new HistoryDataProvider();
            //    HistoryInfo historyInfo = new HistoryInfo();
            //    historyInfo.UserId = GPSession.UserId;
            //    historyInfo.ModuleId = (int)HistoryInfo.Module.Job;
            //    historyInfo.TypeId = (int)HistoryInfo.ActionType.Edit;
            //    historyInfo.RecordId = Convert.ToUInt32(info.JobId);
            //    historyInfo.ModifiedDate = DateTime.Now;
            //    historyInfo.Details = new List<HistoryDetailInfo>();

            //    if (DAO.getInt(reader, "locationid") != info.LocationId)
            //    {
            //        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "locationid", OldValue = DAO.getInt(reader, "locationid").ToString(), NewValue = info.LocationId.ToString() });
            //    }
            //    if (DAO.getInt(reader, "typeid") != info.TypeId)
            //    {
            //        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "typeid", OldValue = DAO.getInt(reader, "typeid").ToString(), NewValue = info.TypeId.ToString() });
            //    }
            //    if (DAO.getString(reader, "salaryMax").ToString() != info.SalaryMax)
            //    {
            //        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "salaryMax", OldValue = DAO.getString(reader, "salaryMax"), NewValue = info.SalaryMax });
            //    }
            //    if (DAO.getString(reader, "salaryMin").ToString() != info.SalaryMin)
            //    {
            //        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "salaryMin", OldValue = DAO.getString(reader, "salaryMin"), NewValue = info.SalaryMin });
            //    }
            //    if (DAO.getString(reader, "ReferenceNo").ToString() != info.ReferenceNo)
            //    {
            //        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "ReferenceNo", OldValue = DAO.getString(reader, "ReferenceNo"), NewValue = info.ReferenceNo });
            //    }
            //    if (DAO.getString(reader, "title").ToString() != info.Title)
            //    {
            //        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "title", OldValue = DAO.getString(reader, "title"), NewValue = info.Title });
            //    }
            //    if (DAO.getString(reader, "subHeading").ToString() != info.SubHeading)
            //    {
            //        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "subHeading", OldValue = DAO.getString(reader, "subHeading"), NewValue = info.SubHeading });
            //    }
            //    if (DAO.getString(reader, "searchTitle").ToString() != info.SubHeading)
            //    {
            //        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "searchTitle", OldValue = DAO.getString(reader, "searchTitle"), NewValue = info.SearchTitle });
            //    }
            //    if (DAO.getString(reader, "bullet1").ToString() != info.Bullet1)
            //    {
            //        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "bullet1", OldValue = DAO.getString(reader, "bullet1"), NewValue = info.Bullet1 });
            //    }
            //    if (DAO.getString(reader, "bullet2").ToString() != info.Bullet2)
            //    {
            //        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "bullet2", OldValue = DAO.getString(reader, "bullet2"), NewValue = info.Bullet2 });
            //    }
            //    if (DAO.getString(reader, "bullet3").ToString() != info.Bullet3)
            //    {
            //        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "bullet3", OldValue = DAO.getString(reader, "bullet3"), NewValue = info.Bullet3 });
            //    }
            //    if (DAO.getString(reader, "summary").ToString() != info.Summary)
            //    {
            //        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "summary", OldValue = DAO.getString(reader, "summary"), NewValue = info.Summary });
            //    }
            //    if (DAO.getString(reader, "jobContent").ToString() != info.JobContent)
            //    {
            //        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "jobContent", OldValue = DAO.getString(reader, "jobContent"), NewValue = info.JobContent });
            //    }
            //    if (DAO.getString(reader, "websiteURL").ToString() != info.WebsiteURL)
            //    {
            //        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "websiteURL", OldValue = DAO.getString(reader, "websiteURL"), NewValue = info.WebsiteURL });
            //    }
            //    if (DAO.getString(reader, "adFooter").ToString() != info.AdFooter)
            //    {
            //        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "adFooter", OldValue = DAO.getString(reader, "adFooter"), NewValue = info.AdFooter });
            //    }
            //    if (DAO.getInt(reader, "status") != info.Status)
            //    {
            //        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "status", OldValue = DAO.getInt(reader, "status").ToString(), NewValue = info.Status.ToString() });
            //    }
            //    if (DAO.getInt(reader, "clientId") != info.ClientId)
            //    {
            //        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "clientId", OldValue = DAO.getInt(reader, "clientId").ToString(), NewValue = info.ClientId.ToString() });
            //    }
            //    if (DAO.getInt(reader, "jobIndustryId") != info.JobIndustryId)
            //    {
            //        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "jobIndustryId", OldValue = DAO.getInt(reader, "jobIndustryId").ToString(), NewValue = info.JobIndustryId.ToString() });
            //    }
            //    if (DAO.getInt(reader, "jobIndustrySubId") != info.JobIndustrySubId)
            //    {
            //        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "jobIndustrySubId", OldValue = DAO.getInt(reader, "jobIndustrySubId").ToString(), NewValue = info.JobIndustrySubId.ToString() });
            //    }
            //    if (DAO.getBool(reader, "hotJob") != info.HotJob)
            //    {
            //        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "hotJob", OldValue = DAO.getBool(reader, "hotJob").ToString(), NewValue = info.HotJob.ToString() });
            //    }
            //    if (DAO.getString(reader, "salaryCurrency").ToString() != info.SalaryCurrency)
            //    {
            //        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "salaryCurrency", OldValue = DAO.getString(reader, "salaryCurrency"), NewValue = info.SalaryCurrency });
            //    }
            //    if (DAO.getInt(reader, "salaryFrequency") != info.SalaryFrequency)
            //    {
            //        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "salaryFrequency", OldValue = DAO.getInt(reader, "salaryFrequency").ToString(), NewValue = info.SalaryFrequency.ToString() });
            //    }

            //    if (historyInfo.Details.Count > 0)
            //    {
            //        history.insertHistory(historyInfo);
            //    }
            //}
            //reader.Close();
            //reader.Dispose();

            #endregion History

            JobEssentialCriteriaDataProvider.deleteEssentialCriteria(info.JobId);
            JobDesirableCriteriaDataProvider.deleteDesirableCriteria(info.JobId);
            JobConsultantDataProvider.deleteJobConsultant(info.JobId);
            JobLocationDataProvider.deleteJobLocation(info.JobId);

            uint jobId = Convert.ToUInt32(info.JobId);

            foreach (EssentialCriteriaInfo essInfo in info.EssentialCriteriaList)
            {
                JobEssentialCriteriaDataProvider.insertEssentialCriteria(jobId, essInfo.Description, essInfo.AnswerLength, essInfo.SortOrder);
            }

            foreach (DesirableCriteriaInfo desiInfo in info.DesirableCriteriaList)
            {
                JobDesirableCriteriaDataProvider.insertDesirableCriteria(jobId, desiInfo.Description, desiInfo.AnswerLength, desiInfo.SortOrder);
            }

            foreach (JobConsultantInfo consult in info.ConsultantList)
            {
                JobConsultantDataProvider.insertJobConsultant(jobId, consult.ConsultantId);
            }

            foreach (JobLocation location in info.LocationList)
            {
                JobLocationDataProvider.insertJobLocation(jobId, location.LocationId, location.Locationtype);
            }
        }

        public static void updateJobApprove(int jobId)
        {
            string sql = "update jobdetail JD inner join jobdetail_edit JE on JD.Jobdetailid= JE.jobdetailid " +
                        " set JD.locationID = JE.locationID, JD.typeID = JE.typeID, JD.salaryMin = JE.salaryMin, JD.salaryMax = JE.salaryMax, JD.ReferenceNo = JE.ReferenceNo, JD.title = JE.title, JD.subHeading = JE.subHeading,JD.version=JE.version, JD.internalchange = JE.internalchange," +
                        " JD.searchTitle = JE.searchTitle, JD.bullet1 = JE.bullet1, JD.bullet2 = JE.bullet2, JD.bullet3 = JE.bullet3, JD.summary = JE.summary, JD.jobContent = JE.jobContent, JD.websiteURL = JE.websiteURL, " +
                        " JD.adFooter = JE.adFooter, JD.isResidency = JE.isResidency, JD.status = JE.status, JD.isApprove = JE.isApprove, JD.clientId = JE.clientId, JD.jobIndustryId = JE.jobIndustryId, JD.jobIndustrySubId = JE.jobIndustrySubId, " +
                        " JD.modifiedDate = JE.modifiedDate, JD.modifiedBy = JE.modifiedBy, JD.hotJob = JE.hotJob, JD.salaryCurrency = JE.salaryCurrency, JD.salaryFrequency = JE.salaryFrequency, JD.published=JE.published, JD.isco08id=JE.isco08id, JD.isicrev4id=JE.isicrev4id Where JE.active=1 and JE.jobdetailid=?jobId";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("jobId", jobId));
            int jobeditid = 0;

            if (JobDetailEditDataProvider.isExist(jobId, ref jobeditid))
            {
                JobEssentialCriteriaDataProvider.deleteEssentialCriteria(jobId);
                sql = "insert into essentialcriteria (JobId, Description,AnswerLength,sortorder)  (select JobID,Description,AnswerLength,sortorder from essentialcriteria_edit  where  jobeditid=?jobeditid) ";
                DAO.ExecuteNonQuery(sql, new MySqlParameter("jobeditid", jobeditid));

                JobDesirableCriteriaDataProvider.deleteDesirableCriteria(jobId);
                sql = "insert into desirablecriteria (JobId, Description,AnswerLength,sortorder)  (select JobID,Description,AnswerLength,sortorder from desirablecriteria_edit where  jobeditid=?jobeditid) ";
                DAO.ExecuteNonQuery(sql, new MySqlParameter("jobeditid", jobeditid));

                JobConsultantDataProvider.deleteJobConsultant(jobId);
                sql = "insert into job_consultants (jobID, consultantID)  (select jobID, consultantID from job_consultants_edit where  jobeditid=?jobeditid) ";
                DAO.ExecuteNonQuery(sql, new MySqlParameter("jobeditid", jobeditid));

                JobLocationDataProvider.deleteJobLocation(jobId);
                sql = "insert into jobs_locations (jobdetailid,locationid,locationtype) (select jobdetailid,locationid,locationtype from jobs_locations_edit where  jobeditid=?jobeditid) ";
                DAO.ExecuteNonQuery(sql, new MySqlParameter("jobeditid", jobeditid));
            }

            sql = "update jobdetail set isApprove=1,status=6,ismodified=0 where JobDetailID=?jobId and status in(1,2,5,6)";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("jobId", jobId));
            sql = "update jobdetail set isApprove=1,ismodified=0 where JobDetailID=?jobId and status in(3,4)";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("jobId", jobId));
            // JobDetailEditDataProvider.deleteJobDetail(jobId);
            JobDetailEditDataProvider.updateActive(jobId, jobeditid);

            string referenceNo = getReferenceNo(jobId);

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo historyInfo = new HistoryInfo();
            historyInfo.UserId = GPSession.UserId;
            historyInfo.ModuleId = (int)HistoryInfo.Module.Job;
            historyInfo.TypeId = (int)HistoryInfo.ActionType.Edit;
            historyInfo.RecordId = Convert.ToUInt32(jobId);
            historyInfo.Details = new List<HistoryDetailInfo>();
            historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "approve", NewValue = referenceNo });
            history.insertHistory(historyInfo);
        }

        public static void updateIsmodified(int jobId)
        {
            string sql = "update jobdetail set ismodified=1 where JobDetailID=?jobId";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("jobId", jobId));
        }

        public static void deleteJobDetail(int jobId)
        {
            JobDetailEditDataProvider.deleteJobDetail(jobId);
            JobEssentialCriteriaDataProvider.deleteEssentialCriteria(jobId);
            JobDesirableCriteriaDataProvider.deleteDesirableCriteria(jobId);
            JobConsultantDataProvider.deleteJobConsultant(jobId);
            JobLocationDataProvider.deleteJobLocation(jobId);

            string sql = "delete from jobdetail where JobDetailID=?jobId";

            DAO.ExecuteScalar(sql, new MySqlParameter("jobId", jobId));
        }

        public static void copyJobDetail(int jobId, DateTime createdDate, string refCode, uint createdBy)
        {
            //refCode = refCode + jobReferenceCode(refCode);
            string sql = "insert into jobdetail (locationID, typeID,salaryMin,salaryMax,ReferenceNo,title,subHeading,searchTitle,bullet1,bullet2,bullet3,summary,jobContent,websiteURL,adFooter,isResidency,status,isApprove,clientId" +
                ",jobIndustryId,jobIndustrySubId, createdDate,createdBy,hotJob,salaryCurrency,salaryFrequency,published,ismodified,isco08id,isicrev4id)" +
                "select locationID,typeID,salaryMin,salaryMax,?referenceNo,title,subHeading,searchTitle,bullet1,bullet2,bullet3,summary,jobContent,websiteURL" +
                ",adFooter,isResidency,1,0,clientId,jobIndustryId,jobIndustrySubId,?createdDate,?createdBy,hotJob,salaryCurrency,salaryFrequency,published,1,isco08id,isicrev4id from jobdetail where JobDetailID=?jobId; select last_insert_id()";

            int newJobId;
            newJobId = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("jobId", jobId), new MySqlParameter("createdDate", createdDate), new MySqlParameter("referenceNo", refCode), new MySqlParameter("createdBy", createdBy)));

            JobEssentialCriteriaDataProvider.copyEssentialCriteria(jobId, newJobId);
            JobDesirableCriteriaDataProvider.copyDesirableCriteria(jobId, newJobId);
            JobConsultantDataProvider.copyConsultant(jobId, newJobId);
            JobLocationDataProvider.copyLocation(jobId, newJobId);

            JobDetailEditDataProvider.copyJobDetail(newJobId, createdDate, refCode, createdBy);
        }

        public static string jobReferenceCode(string referenceCode)
        {
            string _return = "001";
            string sql = "select ReferenceNo from jobdetail where ReferenceNo like concat_ws(?referenceCode,'%','%') ";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("referenceCode", referenceCode));

            if (!reader.HasRows)
            {
                _return = "001";
            }
            else
            {
                int refOut = 0;
                while (reader.Read())
                {
                    string refcode = DAO.getString(reader, "ReferenceNo");
                    int.TryParse(refcode.Substring(refcode.Length - 3, 3), out refOut);
                    if (refOut > 0)
                    {
                        refOut = refOut + 1;
                        _return = refOut.ToString("000");
                    }
                }
            }
            reader.Close();
            reader.Dispose();

            return _return;
        }

        public static DataTable listAllOpenJobs()
        {
            DataTable dt = new DataTable();
            string sql = "select jobdetailId,referenceno from jobdetail where status=2";
            MySqlDataReader reader = DAO.ExecuteReader(sql);
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public static MySqlDataReader searchJob(string keyword, string sortexpression)
        {
            string sql = "Select JD.jobdetailid as jobid,JD.title as jobtitle,JD.referenceno as refno,JT.jobtypeid," +
                         "JT.type as jobtype,Lo.locationid,Lo.name as regionname,JD.status,JD.createddate,JD.isApprove from jobdetail JD " +
                         "inner join jobtype JT on JD.typeid = JT.jobtypeid inner join locations Lo on JD.locationid = Lo.locationid " +
                         "where JD.referenceno like concat_ws(?keyword,'%','%') or JD.title like concat_ws(?keyword,'%','%') order by " + sortexpression;
            return DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword));
        }

        public static MySqlDataReader filterJob(string keyword, uint consultantid, uint clientid, uint status, string sortexpression)
        {
            string sql = "Select distinct JD.jobdetailid as jobid,JD.title as jobtitle,JD.referenceno as refno,JT.jobtypeid,JT.type as jobtype,Lo.locationid," +
                         "Lo.name as regionname,date_format(JD.createddate,'%d-%b-%Y') as createddateFormate,JD.createddate,JD.clientid,JS.status," +
                         "date_format((case when (JD.modifieddate='0001-01-01 00:00:00' or JD.modifieddate is null) then JD.createddate else JD.modifieddate end),'%d-%b-%Y-%T') as modifieddateFormate,cl.clientname,JD.isApprove " +
                         " from jobdetail JD inner join jobtype JT on JD.typeid = JT.jobtypeid left join locations Lo on JD.locationid = Lo.locationid left outer join Client Cl " +
                         "on JD.clientid = Cl.clientid left outer join jobstatus JS on JS.jobstatusid = JD.status left outer join job_consultants JC on JD.jobdetailid = JC.jobid " +
                         "Where (JD.referenceno like concat_ws(?keyword,'%','%') or JD.title like concat_ws(?keyword,'%','%')) and (JC.consultantid =?consultantid OR ?consultantid =0) and (JD.clientid =?clientid OR ?clientid=0) " +
                         "and (JD.status =?status OR ?status =0) order by " + sortexpression;

            return DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword), new MySqlParameter("consultantid", consultantid), new MySqlParameter("clientid", clientid), new MySqlParameter("status", status));
        }

        public static MySqlDataReader getJobById(uint jobId)
        {
            string sql = "select 0 as jobeditid,0 as version, JD.JobDetailID,JD.locationID,JD.typeID,JD.salaryMin,JD.salaryMax,JD.ReferenceNo,JD.title,JD.subHeading,JD.searchTitle,JD.bullet1,JD.bullet2,JD.bullet3,JD.summary,JD.jobContent,JD.createdby,JD.published, 'false' as internalchange, " +
                          "JD.websiteURL,JD.adFooter,JD.isResidency,JD.status,JD.isApprove,JD.clientId,JD.createdDate,JD.modifieddate,JT.Type as jobtype,Lo.Name as location,JD.jobIndustryId,JD.jobIndustrySubId,JD.hotJob,JD.salaryFrequency, " +
                          " JD.salaryCurrency,f.frequency,c.clientname,JD.isco08id,concat('UNIT ',i.groupcode,' ',i.title) as isco " +
                          " ,JD.isicrev4id, concat('CLASS ',ir.code,' ', ir.description) as industry from jobdetail JD inner join jobtype JT on JD.typeID=JT.JobTypeId left join locations Lo on JD.locationID=Lo.locationid left join frequency f on f.frequencyId=JD.salaryFrequency " +
                          " left join client c on JD.clientId=c.clientID left join isco08 i on JD.isco08id=i.isco08id left join isicrev4 ir on JD.isicrev4id=ir.isicrev4id where JobDetailID=?jobId";
            return DAO.ExecuteReader(sql, new MySqlParameter("jobId", jobId));
        }

        public static MySqlDataReader getJobByReferenceNo(string referenceNo)
        {
            string sql = "select 0 as jobeditid,0 as version, JD.JobDetailID,JD.locationID,JD.typeID,JD.salaryMin,JD.salaryMax,JD.ReferenceNo,JD.title,JD.subHeading,JD.searchTitle,JD.bullet1,JD.bullet2,JD.bullet3,JD.summary,JD.jobContent,JD.createdby,JD.published, 'false' as internalchange, " +
                          "JD.websiteURL,JD.adFooter,JD.isResidency,JD.status,JD.isApprove,JD.clientId,JD.createdDate,JD.modifieddate,JT.Type as jobtype,'' as location,JD.jobIndustryId,JD.jobIndustrySubId,JD.hotJob,JD.salaryFrequency, " +
                          " JD.salaryCurrency,f.frequency,c.clientname,JD.isco08id,concat('UNIT ',i.groupcode,' ',i.title) as isco " +
                          " ,JD.isicrev4id, concat('CLASS ',ir.code,' ', ir.description) as industry from jobdetail JD inner join jobtype JT on JD.typeID=JT.JobTypeId  left join frequency f on f.frequencyId=JD.salaryFrequency " +
                          " left join client c on JD.clientId=c.clientID left join isco08 i on JD.isco08id=i.isco08id left join isicrev4 ir on JD.isicrev4id=ir.isicrev4id where ReferenceNo=?referenceNo";
            return DAO.ExecuteReader(sql, new MySqlParameter("referenceNo", referenceNo));
        }

        public static bool jobReferenceExist(string referenceNo, int jobID)
        {
            bool exist = false;
            string sql = "select ReferenceNo from jobdetail where ReferenceNo =?referenceCode and jobDetailId !=?jobId ";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("referenceCode", referenceNo), new MySqlParameter("jobId", jobID));
            if (reader.HasRows)
                exist = true;
            reader.Close();
            reader.Dispose();
            return exist;
        }

        public static int getJobId(string referenceNo)
        {
            int jobid = 0;
            string sql = "select ReferenceNo,JobDetailId from jobdetail where ReferenceNo =?referenceCode ";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("referenceCode", referenceNo));
            if (reader.HasRows)
            {
                reader.Read();
                jobid = Convert.ToInt32(DAO.getInt(reader, "JobDetailId"));
            }
            reader.Close();
            reader.Dispose();
            return jobid;
        }

        public static MySqlDataReader filterJob(uint consultantid, uint clientid, uint status)
        {
            string sql = "Select distinct JD.jobdetailid as jobid,JD.title as jobtitle,JD.referenceno as refno,JT.jobtypeid,JT.type as jobtype,Lo.locationid," +
                         "Lo.name as regionname,date_format(JD.createddate,'%d-%b-%Y') as createddate,JC.consultantid,JD.clientid,JS.status," +
                         "cl.clientname from jobdetail JD inner join jobtype JT on JD.typeid = JT.jobtypeid inner join locations Lo on JD.locationid = Lo.locationid left outer join Client Cl " +
                         "on JD.clientid = Cl.clientid left outer join jobstatus JS on JS.jobstatusid = JD.status left outer join job_consultants JC on JD.jobdetailid = JC.jobid " +
                         "Where (JC.consultantid =?consultantid OR ?consultantid =0) and (JD.clientid =?clientid OR ?clientid=0) " +
                         "and (JD.status =?status OR ?status =0) order by jobtitle";

            return DAO.ExecuteReader(sql, new MySqlParameter("consultantid", consultantid), new MySqlParameter("clientid", clientid), new MySqlParameter("status", status));
        }

        public static MySqlDataReader listJobType()
        {
            string sql = "select JobTypeId, Type from jobType";
            MySqlDataReader drS = DAO.ExecuteReader(sql);
            return drS;
        }

        public static int getJobTypeId(string typeName)
        {
            int typeId = 0;
            string sql = "select JobTypeId, Type from jobType where type=?type";
            MySqlDataReader drS = DAO.ExecuteReader(sql, new MySqlParameter("type", typeName));
            if (!drS.HasRows)
            {
                sql = "insert into jobType (type) values (?type); select last_insert_id() ";
                typeId = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("type", typeName)));
            }
            else
            {
                while (drS.Read())
                {
                    typeId = Convert.ToInt32(DAO.getInt(drS, "JobTypeId"));
                }
                drS.Close();
                drS.Dispose();
            }
            return typeId;
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

        public static int getLocationID(string location)
        {
            int typeId = 0;
            string sql = "select locationid, name " +
                "from locations " +
                "where name=?location";
            MySqlDataReader drS = DAO.ExecuteReader(sql, new MySqlParameter("location", location));
            if (!drS.HasRows)
            {
                sql = "insert into locations (name,countryid) values (?location,0); select last_insert_id() ";
                typeId = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("location", location)));
            }
            else
            {
                while (drS.Read())
                {
                    typeId = Convert.ToInt32(DAO.getInt(drS, "locationid"));
                }
            }

            drS.Close();
            drS.Dispose();
            return typeId;
        }

        public static int getLocationID(string location, string country)
        {
            int typeId = 0;
            string sql = "select locationid, name " +
                "from locations " +
                "where name=?location";
            MySqlDataReader drS = DAO.ExecuteReader(sql, new MySqlParameter("location", location));
            if (!drS.HasRows)
            {
                sql = "select countryid from countries where name=?name";
                int countryid = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("name", country)));
                if (countryid == 0)
                {
                    sql = "insert into countries (name,code,active) values (?name,'##',1); select last_insert_id()";
                    countryid = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("name", country)));
                }
                sql = "insert into locations (name,countryid) values (?location,?countryid); select last_insert_id() ";
                typeId = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("location", location), new MySqlParameter("countryid", countryid)));
            }
            else
            {
                while (drS.Read())
                {
                    typeId = Convert.ToInt32(DAO.getInt(drS, "locationid"));
                }
            }

            drS.Close();
            drS.Dispose();
            return typeId;
        }

        public static MySqlDataReader listJobIndustry()
        {
            string sql = "select JobIndustryID, Classification from jobindustry";
            MySqlDataReader drS = DAO.ExecuteReader(sql);
            return drS;
        }

        public static MySqlDataReader listJobSubIndustry(int industryID)
        {
            string sql = "select JobIndustrySubID, SubClassification from jobindustrysub where JobIndustryId=?industryID";
            MySqlDataReader drS = DAO.ExecuteReader(sql, new MySqlParameter("industryID", industryID));
            return drS;
        }

        public static MySqlDataReader listJobStatus()
        {
            string sql = "select jobstatusID, status from jobstatus order by sortorder";
            MySqlDataReader drS = DAO.ExecuteReader(sql);
            return drS;
        }

        public static MySqlDataReader listopenJobStatus()
        {
            string sql = "select jobstatusID, status from jobstatus where jobstatusid in (2,6) order by sortorder";
            MySqlDataReader drS = DAO.ExecuteReader(sql);
            return drS;
        }

        public static bool jobIdExists(int jobid)
        {
            bool exist;
            string sql = "select jobDetailId from jobdetail where jobdetailid=?jobdetailid";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("jobdetailid", jobid));
            if (reader.HasRows)
                exist = true;
            else
                exist = false;
            reader.Close();
            reader.Dispose();
            return exist;
        }

        public static bool isConsultant(int jobid, int consultantId)
        {
            bool isConsultant = false;
            int id = 0;
            string sql = "select jobdetailid from jobdetail j inner join job_consultants jc on jc.jobid=j.jobdetailid inner join consultants c on c.consultantid=jc.consultantid where jc.consultantid=?consultantid and j.jobdetailid=?jobid";
            id = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("consultantid", consultantId), new MySqlParameter("jobid", jobid)));
            if (id > 0)
                isConsultant = true;
            return isConsultant;
        }

        public static string getReferenceNo(int jobId)
        {
            string referenceNo = string.Empty;
            string sql = "select ReferenceNo from jobdetail where jobDetailID =?jobId ";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("jobId", jobId));
            if (reader.HasRows)
            {
                reader.Read();
                referenceNo = DAO.getString(reader, "ReferenceNo");
            }
            reader.Close();
            reader.Dispose();
            return referenceNo;
        }
    }
}