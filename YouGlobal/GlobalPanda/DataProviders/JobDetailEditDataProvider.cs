using GlobalPanda.BusinessInfo;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

/// <summary>
/// Summary description for JobDetailEditDataProvider
/// </summary>
///
namespace GlobalPanda.DataProviders
{
    public class JobDetailEditDataProvider
    {
        public static uint insertJobDetailEdit(JobDetailInfo info, bool isnew)
        {
            MySqlDataReader reader = JobDetailDataProvider.getJobById(Convert.ToUInt32(info.JobId));

            string sql = "insert into jobdetail_edit (jobdetailId,locationID, typeID,salaryMin,salaryMax,ReferenceNo,title,subHeading,searchTitle,bullet1,bullet2,bullet3,summary,jobContent,websiteURL,adFooter,isResidency,status,isApprove,clientId," +
                          "jobIndustryId,jobIndustrySubId, createdDate,createdBy,modifieddate,modifiedby,hotJob,salaryCurrency,salaryFrequency,published,internalchange,active,version,versionstatus,versiondate,versionuser,isco08id,isicrev4id)" +
                          "values (?jobdetailid,?locationID, ?typeID,?salaryMin,?salaryMax,?ReferenceNo,?title,?subHeading,?searchTitle,?bullet1,?bullet2,?bullet3,?summary,?jobContent,?websiteURL,?adFooter,?isResidency,?status,?isApprove,?clientId," +
                          "?jobIndustryId,?jobIndustrySubId,?createdDate,?createdBy,?modifieddate,?modifiedby,?hotJob,?salaryCurrency,?salaryFrequency,?published,?internalchange,?active,?version,?versionstatus,?vdate,?vuser,?isco08id,?isicrev4id); select last_insert_id()";
            if (isnew)
            {
                info.Status = 1;
                info.Published = true;
            }

            MySqlParameter[] param ={
             new MySqlParameter("jobdetailid",info.JobId),
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
             new MySqlParameter("status", info.Status),
             new MySqlParameter("isApprove",info.IsApprove),
             new MySqlParameter("clientId",info.ClientId),
             new MySqlParameter("jobIndustryId",info.JobIndustryId),
             new MySqlParameter("jobIndustrySubId",info.JobIndustrySubId),
             new MySqlParameter("createdDate",info.CreatedDate),
              new MySqlParameter("createdBy",info.CreatedBy),
              info.IsApprove?
                new MySqlParameter("modifieddate", info.ModifiedDate):new MySqlParameter("modifieddate", DBNull.Value),
                isnew? new MySqlParameter("modifiedBy",DBNull.Value): new MySqlParameter("modifiedBy",info.ModifiedBy),
              new MySqlParameter("hotJob",info.HotJob),
              new MySqlParameter("salaryCurrency",info.SalaryCurrency),
              new MySqlParameter("salaryFrequency",info.SalaryFrequency),
              new MySqlParameter("published",info.Published),
              new MySqlParameter("internalchange",info.InternalChange),
              new MySqlParameter("active",1),
              new MySqlParameter("version",info.Version),
              new MySqlParameter("versionstatus",1),
              new MySqlParameter("vdate",DateTime.UtcNow),
              new MySqlParameter("vuser",GPSession.UserId),
               new MySqlParameter("isco08id",info.ISCO08Id),
               new MySqlParameter("isicrev4id",info.ISICRev4Id)
            };
            uint jobeditId = Convert.ToUInt32(DAO.ExecuteScalar(sql, param));
            uint jobId = Convert.ToUInt32(info.JobId);

            uint essid;
            foreach (EssentialCriteriaInfo essInfo in info.EssentialCriteriaList)
            {
                essid = insertEssentialCriteria(jobId, jobeditId, essInfo.Description, essInfo.AnswerLength, essInfo.SortOrder);
            }
            uint dessid;
            foreach (DesirableCriteriaInfo desiInfo in info.DesirableCriteriaList)
            {
                dessid = insertDesirableCriteria(jobId, jobeditId, desiInfo.Description, desiInfo.AnswerLength, desiInfo.SortOrder);
            }

            foreach (JobConsultantInfo consult in info.ConsultantList)
            {
                insertJobConsultant(jobId, jobeditId, consult.ConsultantId);
            }

            foreach (JobLocation location in info.LocationList)
            {
                insertJobLocation(jobId, location.LocationId, location.Locationtype, jobeditId);
            }

            if (!isnew)
                JobDetailDataProvider.updateIsmodified(info.JobId);

            #region History

            if (reader.HasRows)
            {
                reader.Read();
                HistoryDataProvider history = new HistoryDataProvider();
                HistoryInfo historyInfo = new HistoryInfo();
                historyInfo.UserId = GPSession.UserId;
                historyInfo.ModuleId = (int)HistoryInfo.Module.Job;
                historyInfo.TypeId = (int)HistoryInfo.ActionType.Edit;
                historyInfo.RecordId = Convert.ToUInt32(info.JobId);
                historyInfo.ModifiedDate = DateTime.Now;
                historyInfo.Details = new List<HistoryDetailInfo>();

                if (DAO.getInt(reader, "locationid") != info.LocationId)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "locationid", OldValue = DAO.getInt(reader, "locationid").ToString(), NewValue = info.LocationId.ToString() });
                }
                if (DAO.getInt(reader, "typeid") != info.TypeId)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "typeid", OldValue = DAO.getInt(reader, "typeid").ToString(), NewValue = info.TypeId.ToString() });
                }
                //if (DAO.getString(reader, "salaryMax").ToString() != info.SalaryMax)
                //{
                //    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "salaryMax", OldValue = DAO.getString(reader, "salaryMax"), NewValue = info.SalaryMax });
                //}
                //if (DAO.getString(reader, "salaryMin").ToString() != info.SalaryMin)
                //{
                //    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "salaryMin", OldValue = DAO.getString(reader, "salaryMin"), NewValue = info.SalaryMin });
                //}
                if (DAO.getString(reader, "ReferenceNo").ToString() != info.ReferenceNo)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "ReferenceNo", OldValue = DAO.getString(reader, "ReferenceNo"), NewValue = info.ReferenceNo });
                }
                if (DAO.getString(reader, "title").ToString() != info.Title)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "title", OldValue = DAO.getString(reader, "title"), NewValue = info.Title });
                }
                //if (DAO.getString(reader, "subHeading").ToString() != info.SubHeading)
                //{
                //    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "subHeading", OldValue = DAO.getString(reader, "subHeading"), NewValue = info.SubHeading });
                //}
                //if (DAO.getString(reader, "searchTitle").ToString() != info.SubHeading)
                //{
                //    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "searchTitle", OldValue = DAO.getString(reader, "searchTitle"), NewValue = info.SearchTitle });
                //}
                if (DAO.getString(reader, "bullet1").ToString() != info.Bullet1)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "bullet1", OldValue = DAO.getString(reader, "bullet1"), NewValue = info.Bullet1 });
                }
                if (DAO.getString(reader, "bullet2").ToString() != info.Bullet2)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "bullet2", OldValue = DAO.getString(reader, "bullet2"), NewValue = info.Bullet2 });
                }
                if (DAO.getString(reader, "bullet3").ToString() != info.Bullet3)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "bullet3", OldValue = DAO.getString(reader, "bullet3"), NewValue = info.Bullet3 });
                }
                if (DAO.getString(reader, "summary").ToString() != info.Summary)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "summary", OldValue = DAO.getString(reader, "summary"), NewValue = info.Summary });
                }
                if (DAO.getString(reader, "jobContent").ToString() != info.JobContent)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "jobContent", OldValue = DAO.getString(reader, "jobContent"), NewValue = info.JobContent });
                }
                if ((string.IsNullOrEmpty(DAO.getString(reader, "websiteURL")) ? string.Empty : DAO.getString(reader, "websiteURL").ToString()) != info.WebsiteURL)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "websiteURL", OldValue = DAO.getString(reader, "websiteURL"), NewValue = info.WebsiteURL });
                }
                if (DAO.getString(reader, "adFooter") != info.AdFooter)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "adFooter", OldValue = DAO.getString(reader, "adFooter"), NewValue = info.AdFooter });
                }
                if (DAO.getInt(reader, "status") != info.Status)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "status", OldValue = DAO.getInt(reader, "status").ToString(), NewValue = info.Status.ToString() });
                }
                if (DAO.getInt(reader, "clientId") != info.ClientId)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "clientId", OldValue = DAO.getInt(reader, "clientId").ToString(), NewValue = info.ClientId.ToString() });
                }
                if (DAO.getInt(reader, "jobIndustryId") != info.JobIndustryId)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "jobIndustryId", OldValue = DAO.getInt(reader, "jobIndustryId").ToString(), NewValue = info.JobIndustryId.ToString() });
                }
                if (DAO.getInt(reader, "jobIndustrySubId") != info.JobIndustrySubId)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "jobIndustrySubId", OldValue = DAO.getInt(reader, "jobIndustrySubId").ToString(), NewValue = info.JobIndustrySubId.ToString() });
                }
                if (DAO.getBool(reader, "hotJob") != info.HotJob)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "hotJob", OldValue = DAO.getBool(reader, "hotJob").ToString(), NewValue = info.HotJob.ToString() });
                }

                if ((string.IsNullOrEmpty(DAO.getString(reader, "salaryCurrency")) ? string.Empty : DAO.getString(reader, "salaryCurrency").ToString()) != info.SalaryCurrency)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "salaryCurrency", OldValue = DAO.getString(reader, "salaryCurrency"), NewValue = info.SalaryCurrency });
                }
                if (DAO.getInt(reader, "salaryFrequency") != info.SalaryFrequency)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "salaryFrequency", OldValue = DAO.getInt(reader, "salaryFrequency").ToString(), NewValue = info.SalaryFrequency.ToString() });
                }

                if (historyInfo.Details.Count > 0)
                {
                    history.insertHistory(historyInfo);
                }
            }
            reader.Close();
            reader.Dispose();

            #endregion History

            return jobeditId;
        }

        public static void updateJobDetail(JobDetailInfo info)
        {
            MySqlDataReader reader = getJobById(Convert.ToUInt32(info.JobId), info.JobEditId);

            string sql = "update jobdetail_edit set locationID=?locationID,typeID=?typeID,salaryMin=?salaryMin,salaryMax=?salaryMax,ReferenceNo=?ReferenceNo,title=?title,subHeading=?subHeading,searchTitle=?searchTitle,bullet1=?bullet1" +
                ",bullet2=?bullet2,bullet3=?bullet3,summary=?summary,jobContent=?jobContent,websiteURL=?websiteURL,adFooter=?adFooter,isResidency=?isResidency,status=?status,clientId=?clientId,jobIndustryId=?jobIndustryId,jobIndustrySubId=?jobIndustrySubId," +
                " modifiedDate=?modifiedDate,modifiedBy=?modifiedBy,hotJob=?hotJob,salaryCurrency=?salaryCurrency,salaryFrequency=?salaryFrequency,published=?published,internalchange=?internalchange, isco08id=?isco08id,isicrev4id=?isicrev4id where JobDetailID=?jobId and active=1";

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
             new MySqlParameter("internalchange",info.InternalChange),
              new MySqlParameter("isco08id",info.ISCO08Id),
              new MySqlParameter("isicrev4id",info.ISICRev4Id)
            };

            DAO.ExecuteScalar(sql, param);

            sql = "update jobdetail set ismodified=1 where JobDetailID=?jobId";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("jobId", info.JobId));

            deleteEssentialCriteria(info.JobId);
            deleteDesirableCriteria(info.JobId);
            deleteJobConsultant(info.JobId);
            deleteJobLocation(info.JobId);

            uint jobId = Convert.ToUInt32(info.JobId);
            uint jobeditid = Convert.ToUInt32(info.JobEditId);
            foreach (EssentialCriteriaInfo essInfo in info.EssentialCriteriaList)
            {
                insertEssentialCriteria(jobId, jobeditid, essInfo.Description, essInfo.AnswerLength, essInfo.SortOrder);
            }

            foreach (DesirableCriteriaInfo desiInfo in info.DesirableCriteriaList)
            {
                insertDesirableCriteria(jobId, jobeditid, desiInfo.Description, desiInfo.AnswerLength, desiInfo.SortOrder);
            }

            foreach (JobConsultantInfo consult in info.ConsultantList)
            {
                insertJobConsultant(jobId, jobeditid, consult.ConsultantId);
            }
            foreach (JobLocation location in info.LocationList)
            {
                insertJobLocation(jobId, location.LocationId, location.Locationtype, jobeditid);
            }
            JobDetailDataProvider.updateIsmodified(info.JobId);

            #region History

            if (reader.HasRows)
            {
                reader.Read();
                HistoryDataProvider history = new HistoryDataProvider();
                HistoryInfo historyInfo = new HistoryInfo();
                historyInfo.UserId = GPSession.UserId;
                historyInfo.ModuleId = (int)HistoryInfo.Module.Job;
                historyInfo.TypeId = (int)HistoryInfo.ActionType.Edit;
                historyInfo.RecordId = Convert.ToUInt32(info.JobId);
                historyInfo.ModifiedDate = DateTime.Now;
                historyInfo.Details = new List<HistoryDetailInfo>();

                if (DAO.getInt(reader, "locationid") != info.LocationId)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "locationid", OldValue = DAO.getInt(reader, "locationid").ToString(), NewValue = info.LocationId.ToString() });
                }
                if (DAO.getInt(reader, "typeid") != info.TypeId)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "typeid", OldValue = DAO.getInt(reader, "typeid").ToString(), NewValue = info.TypeId.ToString() });
                }
                if (DAO.getString(reader, "salaryMax").ToString() != info.SalaryMax)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "salaryMax", OldValue = DAO.getString(reader, "salaryMax"), NewValue = info.SalaryMax });
                }
                if (DAO.getString(reader, "salaryMin").ToString() != info.SalaryMin)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "salaryMin", OldValue = DAO.getString(reader, "salaryMin"), NewValue = info.SalaryMin });
                }
                if (DAO.getString(reader, "ReferenceNo").ToString() != info.ReferenceNo)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "ReferenceNo", OldValue = DAO.getString(reader, "ReferenceNo"), NewValue = info.ReferenceNo });
                }
                if (DAO.getString(reader, "title").ToString() != info.Title)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "title", OldValue = DAO.getString(reader, "title"), NewValue = info.Title });
                }
                //if ((string.IsNullOrEmpty(DAO.getString(reader, "subHeading")) ? "" : DAO.getString(reader, "subHeading").ToString()) != info.SubHeading)
                //{
                //    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "subHeading", OldValue = DAO.getString(reader, "subHeading"), NewValue = info.SubHeading });
                //}
                //if (DAO.getString(reader, "searchTitle").ToString() != info.SubHeading)
                //{
                //    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "searchTitle", OldValue = DAO.getString(reader, "searchTitle"), NewValue = info.SearchTitle });
                //}
                if (DAO.getString(reader, "bullet1").ToString() != info.Bullet1)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "bullet1", OldValue = DAO.getString(reader, "bullet1"), NewValue = info.Bullet1 });
                }
                if (DAO.getString(reader, "bullet2").ToString() != info.Bullet2)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "bullet2", OldValue = DAO.getString(reader, "bullet2"), NewValue = info.Bullet2 });
                }
                if (DAO.getString(reader, "bullet3").ToString() != info.Bullet3)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "bullet3", OldValue = DAO.getString(reader, "bullet3"), NewValue = info.Bullet3 });
                }
                if (DAO.getString(reader, "summary").ToString() != info.Summary)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "summary", OldValue = DAO.getString(reader, "summary"), NewValue = info.Summary });
                }
                if (DAO.getString(reader, "jobContent").ToString() != info.JobContent)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "jobContent", OldValue = DAO.getString(reader, "jobContent"), NewValue = info.JobContent });
                }
                if ((string.IsNullOrEmpty(DAO.getString(reader, "websiteURL")) ? string.Empty : DAO.getString(reader, "websiteURL").ToString()) != info.WebsiteURL)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "websiteURL", OldValue = DAO.getString(reader, "websiteURL"), NewValue = info.WebsiteURL });
                }
                if (DAO.getString(reader, "adFooter").ToString() != info.AdFooter)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "adFooter", OldValue = DAO.getString(reader, "adFooter"), NewValue = info.AdFooter });
                }
                if (DAO.getInt(reader, "status") != info.Status)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "status", OldValue = DAO.getInt(reader, "status").ToString(), NewValue = info.Status.ToString() });
                }
                if (DAO.getInt(reader, "clientId") != info.ClientId)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "clientId", OldValue = DAO.getInt(reader, "clientId").ToString(), NewValue = info.ClientId.ToString() });
                }
                if (DAO.getInt(reader, "jobIndustryId") != info.JobIndustryId)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "jobIndustryId", OldValue = DAO.getInt(reader, "jobIndustryId").ToString(), NewValue = info.JobIndustryId.ToString() });
                }
                if (DAO.getInt(reader, "jobIndustrySubId") != info.JobIndustrySubId)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "jobIndustrySubId", OldValue = DAO.getInt(reader, "jobIndustrySubId").ToString(), NewValue = info.JobIndustrySubId.ToString() });
                }
                if (DAO.getBool(reader, "hotJob") != info.HotJob)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "hotJob", OldValue = DAO.getBool(reader, "hotJob").ToString(), NewValue = info.HotJob.ToString() });
                }
                if (DAO.getString(reader, "salaryCurrency").ToString() != info.SalaryCurrency)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "salaryCurrency", OldValue = DAO.getString(reader, "salaryCurrency"), NewValue = info.SalaryCurrency });
                }
                if (DAO.getInt(reader, "salaryFrequency") != info.SalaryFrequency)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "salaryFrequency", OldValue = DAO.getInt(reader, "salaryFrequency").ToString(), NewValue = info.SalaryFrequency.ToString() });
                }

                if (historyInfo.Details.Count > 0)
                {
                    history.insertHistory(historyInfo);
                }
            }
            reader.Close();
            reader.Dispose();

            #endregion History
        }

        public static MySqlDataReader getJobById(uint jobId, int jobeditid)
        {
            string sql = "select JD.jobeditid,JD.version, JD.JobDetailID,JD.locationID,JD.typeID,JD.salaryMin,JD.salaryMax,JD.ReferenceNo,JD.title,JD.subHeading,JD.searchTitle,JD.bullet1,JD.bullet2,JD.bullet3,JD.summary,JD.jobContent,JD.createdby,JD.published,JD.internalchange, " +
                          "JD.websiteURL,JD.adFooter,JD.isResidency,JD.status,JD.isApprove,JD.clientId,JD.createdDate,Jd.modifieddate,JT.Type as jobtype,Lo.Name as location,JD.jobIndustryId,JD.jobIndustrySubId,JD.hotJob,JD.salaryFrequency,JD.salaryCurrency," +
                          " f.frequency,JD.isco08id,concat('UNIT ',i.groupcode,' ',i.title) as isco,JD.isicrev4id,concat('CLASS ',ir.code,' ', ir.description) as industry  " +
                          "from jobdetail_edit JD inner join jobtype JT on JD.typeID=JT.JobTypeId left join locations Lo on JD.locationID=Lo.locationid left join frequency f on f.frequencyId=JD.salaryFrequency left join isco08 i on JD.isco08id=i.isco08id left join isicrev4 ir on JD.isicrev4id=ir.isicrev4id  where jobeditid=?jobeditid and JobDetailID=?jobId";
            return DAO.ExecuteReader(sql, new MySqlParameter("jobId", jobId), new MySqlParameter("jobeditid", jobeditid));
        }

        public static MySqlDataReader getJobByeditId(int jobeditid)
        {
            string sql = "select JD.jobeditid,JD.version, JD.JobDetailID,JD.locationID,JD.typeID,JD.salaryMin,JD.salaryMax,JD.ReferenceNo,JD.title,JD.subHeading,JD.searchTitle,JD.bullet1,JD.bullet2,JD.bullet3,JD.summary,JD.jobContent,JD.createdby,JD.published,JD.internalchange, " +
                          "JD.websiteURL,JD.adFooter,JD.isResidency,JD.status,JD.isApprove,JD.clientId,JD.createdDate,Jd.modifieddate,JT.Type as jobtype,Lo.Name as location,JD.jobIndustryId,JD.jobIndustrySubId,JD.hotJob,JD.salaryFrequency," +
                          " JD.salaryCurrency,f.frequency,JD.isco08id,concat('UNIT ',i.groupcode,' ',i.title) as isco,JD.isicrev4id,concat('CLASS ',ir.code,' ', ir.description) as industry " +
                          "from jobdetail_edit JD inner join jobtype JT on JD.typeID=JT.JobTypeId left join locations Lo on JD.locationID=Lo.locationid left join frequency f on f.frequencyId=JD.salaryFrequency left join isco08 i on JD.isco08id=i.isco08id left join isicrev4 ir on JD.isicrev4id=ir.isicrev4id where jobeditid=?jobeditid ";
            return DAO.ExecuteReader(sql, new MySqlParameter("jobeditid", jobeditid));
        }

        public static bool jobReferenceExist(string referenceNo, int jobId)
        {
            bool exist = false;
            string sql = "select ReferenceNo from jobdetail_edit where ReferenceNo =?referenceCode and jobdetailId !=?jobId ";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("referenceCode", referenceNo), new MySqlParameter("jobId", jobId));
            if (reader.HasRows)
                exist = true;
            reader.Close();
            reader.Dispose();
            return exist;
        }

        public static void decline(int jobId)
        {
            //deleteJobDetail(jobId);
            string sql = "update jobdetail set ismodified=0 where JobDetailID=?jobId; update jobdetail_edit set versionstatus=3,active=0,versionstatusdate=?vdate,versionapproveuser=?vuser where jobdetailid=?jobid and active=1";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("jobId", jobId), new MySqlParameter("vdate", DateTime.UtcNow), new MySqlParameter("vuser", GPSession.UserId));
        }

        public static void copyJobDetail(int jobId, DateTime createdDate, string refCode, uint createdBy)
        {
            //refCode = refCode + jobReferenceCode(refCode);
            string sql = "insert into jobdetail_edit (jobdetailId,locationID, typeID,salaryMin,salaryMax,ReferenceNo,title,subHeading,searchTitle,bullet1,bullet2,bullet3,summary,jobContent,websiteURL,adFooter,isResidency,status,isApprove,clientId" +
                ",jobIndustryId,jobIndustrySubId, createdDate,createdBy,hotJob,salaryCurrency,salaryFrequency,published,active,version,versionstatus,versiondate,versionuser,isco08id,isicrev4id)" +
                "select jobdetailId,locationID,typeID,salaryMin,salaryMax,?referenceNo,title,subHeading,searchTitle,bullet1,bullet2,bullet3,summary,jobContent,websiteURL" +
                ",adFooter,isResidency,1,0,clientId,jobIndustryId,jobIndustrySubId,?createdDate,?createdBy,hotJob,salaryCurrency,salaryFrequency,published,1,1,1,?createddate,?createdby,isco08id,isicrev4id from jobdetail where JobDetailID=?jobId; select last_insert_id()";

            int newJobId;
            newJobId = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("jobId", jobId), new MySqlParameter("createdDate", createdDate), new MySqlParameter("referenceNo", refCode), new MySqlParameter("createdBy", createdBy)));

            copyEssentialCriteria(jobId, newJobId);
            copyDesirableCriteria(jobId, newJobId);
            copyConsultant(jobId, newJobId);
            copyLocation(jobId, newJobId);
        }

        public static void updateActive(int jobId, int jobeditid)
        {
            string sql = "update jobdetail_edit set active=0,versionstatus=2,versionstatusdate=?vdate,versionapproveuser=?vuser where jobeditid=?jobeditid and jobdetailid=?jobId";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("jobId", jobId), new MySqlParameter("jobeditid", jobeditid), new MySqlParameter("vdate", DateTime.UtcNow), new MySqlParameter("vuser", GPSession.UserId));
        }

        public static void deleteJobDetail(int jobId)
        {
            deleteEssentialCriteria(jobId);
            deleteDesirableCriteria(jobId);
            deleteJobConsultant(jobId);

            string sql = "delete from jobdetail_edit where JobDetailID=?jobId";

            DAO.ExecuteScalar(sql, new MySqlParameter("jobId", jobId));
        }

        public static bool isExist(int jobid, ref int jobeditid)
        {
            bool exist = false;
            string sql = "select jobeditid from jobdetail_edit where jobdetailid=?jobdetailid and active=1";
            jobeditid = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("jobdetailid", jobid)));

            if (jobeditid > 0)
                exist = true;

            return exist;
        }

        public static MySqlDataReader filterJob(string keyword, uint consultantid, uint clientid, int status, string sortexpression, int consultantId = 0)
        {
            string sql = "Select distinct case when count(ej.jobdetailid)>0 then 1 else 0 end as isemailtemplate,case when count(jb.jobdetailid)>0 then 1 else 0 end as isexport,JD.jobdetailid as jobid,JD.title as jobtitle," +
                         " JD.referenceno as refno,JT.jobtypeid,JT.type as jobtype,'' as locationid,JD.published,'' as regionname,date_format(JD.createddate,'%d-%b-%Y') as createddateFormate,JD.createddate,JD.clientid,JS.status," +
                         " JD.status as statusid,case when (JD.modifieddate='0001-01-01 00:00:00' or JD.modifieddate is null) then JD.createddate else JD.modifieddate end as modifieddate, " +
                         " date_format((case when (JD.modifieddate='0001-01-01 00:00:00' or JD.modifieddate is null) then JD.createddate else JD.modifieddate end),'%d-%b-%Y-%T') as modifieddateFormate," +
                         " cl.clientname,JD.isApprove,JD.modifiedby as Versionuser,date_format(JD.modifieddate,'%d-%b-%Y-%T') as modifiedFormate,coalesce((" + consultantId + "=JC.consultantId),0) as isconsultant " +
                         " from jobdetail JD inner join jobtype JT on JD.typeid = JT.jobtypeid  left outer join Client Cl " +
                         " on JD.clientid = Cl.clientid left outer join jobstatus JS on JS.jobstatusid = JD.status left outer join job_consultants JC on JD.jobdetailid = JC.jobid  " +
                         " left join jobboard_export jb on jb.jobdetailid=jd.jobdetailid left join emailtemplate_jobs ej on ej.jobdetailid=JD.jobdetailid " +
                         " Where  (JD.referenceno like concat_ws(?keyword,'%','%') or JD.title like concat_ws(?keyword,'%','%')) and (JC.consultantid =?consultantid OR ?consultantid =0) and (JD.clientid =?clientid OR ?clientid=0) " +
                         " and (JD.status =?status OR ?status =0) and (JD.ismodified=0 or JD.ismodified is null) /*and (JD.isapprove=0 or JD.isapprove is null)*/  group by JD.jobdetailid ,JD.title ,JD.referenceno ,JT.jobtypeid,JT.type ,JD.published ";
            sql += " union  Select distinct case when count(ej.jobdetailid)>0 then 1 else 0 end as isemailtemplate,case when count(jb.jobdetailid)>0 then 1 else 0 end as isexport,JD.jobdetailid as jobid,JD.title as jobtitle," +
                   " JD.referenceno as refno,JT.jobtypeid,JT.type as jobtype,'' as locationid,JD.published," +
                         " '' as regionname,date_format(JD.createddate,'%d-%b-%Y') as createddateFormate,JD.createddate,JD.clientid,case when JD.status=1 then JS.Status else 'Change awaits approval' end as status," +
                         " case when JD.status=1 then JD.Status else 5 end as statusid," +
                        " case when (JD.modifieddate='0001-01-01 00:00:00' or JD.modifieddate is null) then JD.createddate else JD.modifieddate end as modifieddate," +
                        " date_format((case when (JD.modifieddate='0001-01-01 00:00:00' or JD.modifieddate is null) then JD.createddate else JD.modifieddate end),'%d-%b-%Y-%T') as modifieddateFormate," +
                         " cl.clientname,JD.isApprove,JD.Versionuser,date_format(JD.modifieddate,'%d-%b-%Y-%T') as modifiedFormate,coalesce((" + consultantId + "=JC.consultantId),0) as isconsultant " +
                         " from jobdetail_edit JD inner join jobtype JT on JD.typeid = JT.jobtypeid  left outer join Client Cl " +
                         " on JD.clientid = Cl.clientid left outer join jobstatus JS on JS.jobstatusid = JD.status left outer join job_consultants_edit JC on JD.jobdetailid = JC.jobid " +
                         " left join jobboard_export jb on jb.jobdetailid=jd.jobdetailid left join emailtemplate_jobs ej on ej.jobdetailid=JD.jobdetailid" +
                         " Where active=1 and (JD.referenceno like concat_ws(?keyword,'%','%') or JD.title like concat_ws(?keyword,'%','%')) and (JC.consultantid =?consultantid OR ?consultantid =0) and (JD.clientid =?clientid OR ?clientid=0) " +
                         "and (JD.status =?status OR ?status =0 OR ?status=5) /*and (JD.isapprove=0 or JD.isapprove is null)*/ group by JD.jobdetailid ,JD.title ,JD.referenceno ,JT.jobtypeid,JT.type ,JD.published order by " + sortexpression;

            return DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword), new MySqlParameter("consultantid", consultantid), new MySqlParameter("clientid", clientid), new MySqlParameter("status", status));
        }

        //public static MySqlDataReader filterConsultantJob(string keyword, uint consultantid, uint clientid, int status, string sortexpression, int consultantId = 0)
        //{
        //    string sql = "Select * from ( Select distinct JD.jobdetailid as jobid,JD.title as jobtitle,JD.referenceno as refno,JT.jobtypeid,JT.type as jobtype,Lo.locationid,JD.published," +
        //                 "Lo.name as regionname,date_format(JD.createddate,'%d-%b-%Y') as createddateFormate,JD.createddate,JD.clientid,JS.status,JD.status as statusid,case when (JD.modifieddate='0001-01-01 00:00:00' or JD.modifieddate is null) then JD.createddate else JD.modifieddate end as modifieddate, " +
        //                 " date_format((case when (JD.modifieddate='0001-01-01 00:00:00' or JD.modifieddate is null) then JD.createddate else JD.modifieddate end),'%d-%b-%Y-%T') as modifieddateFormate," +
        //                 "cl.clientname,JD.isApprove,JD.modifiedby as Versionuser,date_format(JD.modifieddate,'%d-%b-%Y-%T') as modifiedFormate,coalesce((" + consultantId + "=JC.consultantId),0) as isconsultant " +
        //                 " from jobdetail JD inner join jobtype JT on JD.typeid = JT.jobtypeid inner join locations Lo on JD.locationid = Lo.locationid left outer join Client Cl " +
        //                 "on JD.clientid = Cl.clientid left outer join jobstatus JS on JS.jobstatusid = JD.status left outer join job_consultants JC on JD.jobdetailid = JC.jobid  " +
        //                 "Where  (JD.referenceno like concat_ws(?keyword,'%','%') or JD.title like concat_ws(?keyword,'%','%')) and (JC.consultantid =?consultantid OR ?consultantid =0) and (JD.clientid =?clientid OR ?clientid=0) " +
        //                 " and (JD.status =?status OR ?status =0 OR (?status=-1 and JD.status in (2,6))) and (JD.ismodified=0 or JD.ismodified is null) ";
        //    sql += " union  Select distinct JD.jobdetailid as jobid,JD.title as jobtitle,JD.referenceno as refno,JT.jobtypeid,JT.type as jobtype,Lo.locationid,JD.published," +
        //                 "Lo.name as regionname,date_format(JD.createddate,'%d-%b-%Y') as createddateFormate,JD.createddate,JD.clientid,case when JD.status=1 then JS.Status else 'Change awaits approval' end as status," +
        //                 " case when JD.status=1 then JD.Status else 5 end as statusid," +
        //                " case when (JD.modifieddate='0001-01-01 00:00:00' or JD.modifieddate is null) then JD.createddate else JD.modifieddate end as modifieddate," +
        //                " date_format((case when (JD.modifieddate='0001-01-01 00:00:00' or JD.modifieddate is null) then JD.createddate else JD.modifieddate end),'%d-%b-%Y-%T') as modifieddateFormate," +
        //                 "cl.clientname,JD.isApprove,JD.Versionuser,date_format(JD.modifieddate,'%d-%b-%Y-%T') as modifiedFormate,coalesce((" + consultantId + "=JC.consultantId),0) as isconsultant " +
        //                 "from jobdetail_edit JD inner join jobtype JT on JD.typeid = JT.jobtypeid inner join locations Lo on JD.locationid = Lo.locationid left outer join Client Cl " +
        //                 "on JD.clientid = Cl.clientid left outer join jobstatus JS on JS.jobstatusid = JD.status left outer join job_consultants_edit JC on JD.jobdetailid = JC.jobid " +
        //                 "Where active=1 and (JD.referenceno like concat_ws(?keyword,'%','%') or JD.title like concat_ws(?keyword,'%','%')) and (JC.consultantid =?consultantid OR ?consultantid =0) and (JD.clientid =?clientid OR ?clientid=0) " +
        //                 "and (JD.status =?status OR ?status =0 OR ?status=5 OR (?status=-1 and JD.status in (2,6)))  order by " + sortexpression + ") tbl where isconsultant=1 or (isconsultant=0 and statusid in (2,6))";

        //    return DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword), new MySqlParameter("consultantid", consultantid), new MySqlParameter("clientid", clientid), new MySqlParameter("status", status));
        //}

        public static MySqlDataReader filterConsultantJob(string keyword, uint consultantid, uint clientid, int status, string sortexpression, int consultantId = 0)
        {
            string sql = "Select * from ( Select distinct case when count(ej.jobdetailid)>0 then 1 else 0 end as isemailtemplate,case when count(jb.jobdetailid)>0 then 1 else 0 end as isexport, JD.jobdetailid as jobid,JD.title as jobtitle," +
                " JD.referenceno as refno,JT.jobtypeid,JT.type as jobtype,'' as locationid,JD.published,'' as regionname,date_format(JD.createddate,'%d-%b-%Y') as createddateFormate,JD.createddate,JD.clientid,JS.status,JD.status as statusid," +
                " case when (JD.modifieddate='0001-01-01 00:00:00' or JD.modifieddate is null) then JD.createddate else JD.modifieddate end as modifieddate, " +
                         " date_format((case when (JD.modifieddate='0001-01-01 00:00:00' or JD.modifieddate is null) then JD.createddate else JD.modifieddate end),'%d-%b-%Y-%T') as modifieddateFormate," +
                         " cl.clientname,JD.isApprove,JD.modifiedby as Versionuser,date_format(JD.modifieddate,'%d-%b-%Y-%T') as modifiedFormate,coalesce((" + consultantId + "=JC.consultantId),0) as isconsultant " +
                         " from jobdetail JD inner join jobtype JT on JD.typeid = JT.jobtypeid  left outer join Client Cl " +
                         " on JD.clientid = Cl.clientid left outer join jobstatus JS on JS.jobstatusid = JD.status left outer join job_consultants JC on JD.jobdetailid = JC.jobid  " +
                         " left join jobboard_export jb on jb.jobdetailid=jd.jobdetailid left join emailtemplate_jobs ej on ej.jobdetailid=JD.jobdetailid " +
                         " Where  (JD.referenceno like concat_ws(?keyword,'%','%') or JD.title like concat_ws(?keyword,'%','%')) and (JC.consultantid =?consultantid OR ?consultantid =0) and (JD.clientid =?clientid OR ?clientid=0) " +
                         " and (JD.status =?status OR ?status =0 OR (?status=-1 and JD.status in (2,6))) and (JD.ismodified=0 or JD.ismodified is null) group by JD.jobdetailid ,JD.title ,JD.referenceno ,JT.jobtypeid,JT.type ,JD.published ";
            sql += " union  Select distinct case when count(ej.jobdetailid)>0 then 1 else 0 end as isemailtemplate, case when count(jb.jobdetailid)>0 then 1 else 0 end as isexport, JD.jobdetailid as jobid,JD.title as jobtitle," +
                " JD.referenceno as refno,JT.jobtypeid,JT.type as jobtype,'' as locationid,JD.published,'' as regionname,date_format(JD.createddate,'%d-%b-%Y') as createddateFormate,JD.createddate,JD.clientid," +
                " case when JD.status=1 then JS.Status else 'Change awaits approval' end as status," +
                         " case when JD.status=1 then JD.Status else 5 end as statusid," +
                        " case when (JD.modifieddate='0001-01-01 00:00:00' or JD.modifieddate is null) then JD.createddate else JD.modifieddate end as modifieddate," +
                        " date_format((case when (JD.modifieddate='0001-01-01 00:00:00' or JD.modifieddate is null) then JD.createddate else JD.modifieddate end),'%d-%b-%Y-%T') as modifieddateFormate," +
                         " cl.clientname,JD.isApprove,JD.Versionuser,date_format(JD.modifieddate,'%d-%b-%Y-%T') as modifiedFormate,coalesce((" + consultantId + "=JC.consultantId),0) as isconsultant " +
                         " from jobdetail_edit JD inner join jobtype JT on JD.typeid = JT.jobtypeid left join locations Lo on JD.locationid = Lo.locationid left outer join Client Cl " +
                         " on JD.clientid = Cl.clientid left outer join jobstatus JS on JS.jobstatusid = JD.status left outer join job_consultants_edit JC on JD.jobdetailid = JC.jobid " +
                         " left join jobboard_export jb on jb.jobdetailid=jd.jobdetailid left join emailtemplate_jobs ej on ej.jobdetailid=JD.jobdetailid " +
                         " Where active=1 and (JD.referenceno like concat_ws(?keyword,'%','%') or JD.title like concat_ws(?keyword,'%','%')) and (JC.consultantid =?consultantid OR ?consultantid =0) and (JD.clientid =?clientid OR ?clientid=0) " +
                         " and (JD.status =?status OR ?status =0 OR ?status=5 OR (?status=-1 and JD.status in (2,6))) group by JD.jobdetailid ,JD.title ,JD.referenceno ,JT.jobtypeid,JT.type ,JD.published " +
                         " order by " + sortexpression + ") tbl where isconsultant=1 or (isconsultant=0 and statusid in (2,6))";

            return DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword), new MySqlParameter("consultantid", consultantid), new MySqlParameter("clientid", clientid), new MySqlParameter("status", status));
        }

        public static MySqlDataReader searchUnapproveJob(string keyword, uint consultantid, uint clientid, string sortexpression)
        {
            string sql = "Select distinct JD.jobdetailid as jobid,JD.title as jobtitle,JD.referenceno as refno,JT.jobtypeid,JT.type as jobtype,Lo.locationid," +
                         "Lo.name as regionname,date_format(JD.createddate,'%d-%b-%Y-%T') as createddateFormate,JD.createddate,JD.clientid,JS.status," +
                         "cl.clientname,JD.isApprove, JD.modifiedBy,1 as version,date_format(JD.modifieddate,'%d-%b-%Y-%T') as modifiedFormate from jobdetail JD inner join jobtype JT on JD.typeid = JT.jobtypeid inner join locations Lo on JD.locationid = Lo.locationid left outer join Client Cl " +
                         "on JD.clientid = Cl.clientid left outer join jobstatus JS on JS.jobstatusid = JD.status left outer join job_consultants JC on JD.jobdetailid = JC.jobid left outer join jobdetail_edit e on e.jobdetailid=jd.jobdetailid " +
                         "Where (JD.referenceno like concat_ws(?keyword,'%','%') or JD.title like concat_ws(?keyword,'%','%')) and (JC.consultantid =?consultantid OR ?consultantid =0) and (JD.clientid =?clientid OR ?clientid=0) " +
                         " and (JD.ismodified=0 or JD.ismodified is null) and (JD.isapprove=0 or JD.isapprove is null) and e.versionstatus=1 ";
            sql += " union  Select distinct JD.jobdetailid as jobid,JD.title as jobtitle,JD.referenceno as refno,JT.jobtypeid,JT.type as jobtype,Lo.locationid," +
                         "Lo.name as regionname,date_format(JD.createddate,'%d-%b-%Y-%T') as createddateFormate,JD.createddate,JD.clientid,JS.status," +
                         "cl.clientname,JD.isApprove, JD.modifiedBy,JD.version,date_format(JD.modifieddate,'%d-%b-%Y-%T') as modifiedFormate from jobdetail_edit JD inner join jobtype JT on JD.typeid = JT.jobtypeid inner join locations Lo on JD.locationid = Lo.locationid left outer join Client Cl " +
                         "on JD.clientid = Cl.clientid left outer join jobstatus JS on JS.jobstatusid = JD.status left outer join job_consultants_edit JC on JD.jobdetailid = JC.jobid " +
                         "Where active=1 and (JD.referenceno like concat_ws(?keyword,'%','%') or JD.title like concat_ws(?keyword,'%','%')) and (JC.consultantid =?consultantid OR ?consultantid =0) and (JD.clientid =?clientid OR ?clientid=0) " +
                         "  order by " + sortexpression;

            return DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword), new MySqlParameter("consultantid", consultantid), new MySqlParameter("clientid", clientid));
        }

        public static uint insertEssentialCriteria(uint jobId, uint jobeditId, string description, uint answerLength, int sortorder)
        {
            string sql = "insert into essentialcriteria_edit (JobId, Description,AnswerLength,sortorder,jobeditid) values (?jobId,?description,  ?answerLength,?sortorder,?jobeditid); select last_insert_id()";
            uint essentialID = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("jobId", jobId), new MySqlParameter("description", description), new MySqlParameter("answerLength", answerLength),
                new MySqlParameter("sortorder", sortorder), new MySqlParameter("jobeditid", jobeditId)));
            return essentialID;
        }

        public static void deleteEssentialCriteria(int jobId)
        {
            string sql = "delete from essentialcriteria_edit where jobId=?jobId;";
            DAO.ExecuteScalar(sql, new MySqlParameter("jobId", jobId));
        }

        public static void copyEssentialCriteria(int jobId, int newjobId)
        {
            string sql = "insert into essentialcriteria_edit (JobId, Description,AnswerLength,sortorder,jobeditid)" +
            "select ?jobid,Description,AnswerLength,sortorder,?newjobid from essentialcriteria where JobID=?jobId";

            DAO.ExecuteScalar(sql, new MySqlParameter("jobId", jobId), new MySqlParameter("newjobId", newjobId));
        }

        public static MySqlDataReader searchEssentialCriteria(uint jobId, int jobeditId)
        {
            string sql = "select EssentialCriteriaId,JobID,Description,AnswerLength,sortorder from essentialcriteria_edit where jobeditid=?jobeditid and JobID=?jobId";
            return DAO.ExecuteReader(sql, new MySqlParameter("jobId", jobId), new MySqlParameter("jobeditid", jobeditId));
        }

        public static MySqlDataReader searchEssentialCriteriaByeditId(int jobeditId)
        {
            string sql = "select EssentialCriteriaId,JobID,Description,AnswerLength,sortorder from essentialcriteria_edit where jobeditid=?jobeditid ";
            return DAO.ExecuteReader(sql, new MySqlParameter("jobeditid", jobeditId));
        }

        public static uint insertDesirableCriteria(uint jobId, uint jobeditid, string description, uint answerLength, int sortorder)
        {
            string sql = "insert into desirablecriteria_edit (JobId, Description,AnswerLength,sortorder,jobeditid) values (?jobId,?description,  ?answerLength,?sortorder,?jobeditid); select last_insert_id()";
            uint desirableCriteriaID = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("jobId", jobId), new MySqlParameter("description", description), new MySqlParameter("answerLength", answerLength),
                new MySqlParameter("sortorder", sortorder), new MySqlParameter("jobeditid", jobeditid)));
            return desirableCriteriaID;
        }

        public static void deleteDesirableCriteria(int jobId)
        {
            string sql = "delete from desirablecriteria_edit where jobId=?jobId;";
            DAO.ExecuteScalar(sql, new MySqlParameter("jobId", jobId));
        }

        public static void copyDesirableCriteria(int jobId, int newjobId)
        {
            string sql = "insert into desirablecriteria_edit (JobId, Description,AnswerLength,sortorder,jobeditid)" +
            "select ?jobid,Description,AnswerLength,sortorder,?newjobid from desirablecriteria where JobID=?jobId";

            DAO.ExecuteScalar(sql, new MySqlParameter("jobId", jobId), new MySqlParameter("newjobId", newjobId));
        }

        public static MySqlDataReader searchDesirableCriteria(uint jobId, int jobeditId)
        {
            string sql = "select DesirableCriteriaID,JobID,Description,AnswerLength,sortorder from desirablecriteria_edit where jobeditid=?jobeditid and JobID=?jobId";
            return DAO.ExecuteReader(sql, new MySqlParameter("jobId", jobId), new MySqlParameter("jobeditid", jobeditId));
        }

        public static MySqlDataReader searchDesirableCriteriabyEditId(int jobeditId)
        {
            string sql = "select DesirableCriteriaID,JobID,Description,AnswerLength,sortorder from desirablecriteria_edit where jobeditid=?jobeditid ";
            return DAO.ExecuteReader(sql, new MySqlParameter("jobeditid", jobeditId));
        }

        public static uint insertJobConsultant(uint jobId, uint jobeditid, uint consultantId)
        {
            string sql = "insert into job_consultants_edit (jobID, consultantID,jobeditid) values (?jobId,?consultantId,?jobeditid); select last_insert_id()";
            uint essentialID = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("jobId", jobId), new MySqlParameter("consultantId", consultantId), new MySqlParameter("jobeditid", jobeditid)));
            return essentialID;
        }

        public static void deleteJobConsultant(int jobId)
        {
            string sql = "delete from job_consultants_edit where jobID = ?jobId ";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("jobId", jobId));
        }

        public static void copyConsultant(int jobId, int newjobId)
        {
            string sql = "insert into job_consultants_edit (jobID, consultantID,jobeditid)" +
            "select ?jobid,consultantid,?newjobid from job_consultants where JobID=?jobId";

            DAO.ExecuteScalar(sql, new MySqlParameter("jobId", jobId), new MySqlParameter("newjobId", newjobId));
        }

        public static MySqlDataReader searchJobConsultant(uint jobId, int jobeditId)
        {
            string sql = "select c.consultantid, c.first, c.last,c.nickname " +
              "from consultants as c " +
              "join job_consultants_edit as j on j.consultantID = c.consultantid " +
              "where j.jobeditid=?jobeditid and j.jobId = ?jobId " +
              "order by c.last, c.first ";
            return DAO.ExecuteReader(sql, new MySqlParameter("jobId", jobId), new MySqlParameter("jobeditid", jobeditId));
        }

        public static MySqlDataReader searchJobConsultantbyEditId(int jobeditId)
        {
            string sql = "select c.consultantid, c.first, c.last,c.nickname " +
              "from consultants as c " +
              "join job_consultants_edit as j on j.consultantID = c.consultantid " +
              "where j.jobeditid=?jobeditid  " +
              "order by c.last, c.first ";
            return DAO.ExecuteReader(sql, new MySqlParameter("jobeditid", jobeditId));
        }

        public static uint insertJobLocation(uint jobId, int locationId, int locationtype, uint jobeditId)
        {
            string sql = "insert into jobs_locations_edit (jobdetailid, locationid,locationtype,jobeditId) values (?jobId,?locationid,?locationtype,?jobeditId); select last_insert_id()";
            uint id = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("jobId", jobId), new MySqlParameter("locationid", locationId), new MySqlParameter("locationtype", locationtype), new MySqlParameter("jobeditId", jobeditId)));
            return id;
        }

        public static void deleteJobLocation(int jobId)
        {
            string sql = "delete from jobs_locations_edit where jobdetailid = ?jobId ";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("jobId", jobId));
        }

        public static MySqlDataReader searchJobLocation(int jobId, int jobeditId)
        {
            string sql = "Select jl.jobdetailid,jl.locationtype,jl.locationid,(select concat('Anywhere',',','0',',','0,0,0') from jobs_locations  where jl.locationid=0 and jl.locationtype=1 " +
       " union select concat(c.name,',',CAST(c.countryid as char),',','0,0,0') from countries c where c.countryid=jl.locationid and jl.locationtype=1 and jl.locationid!=0 " +
       " union select concat(concat(c.name,'>',l.name),',',CAST(c.countryid as char),',',cast(l.locationid as char),',', '0,0')  from countries c inner join locations l on l.countryid=c.countryid where l.locationid=jl.locationid and jl.locationtype=2 " +
       " union select concat(concat(c.name,'>',l.name,'>',s.sublocation),',',CAST(c.countryid as char),',',cast(l.locationid as char),',', cast(s.sublocationid as char),',','0')  from countries c inner join locations l on l.countryid=c.countryid " +
                         " inner join locationsub s on s.locationid=l.locationid where s.sublocationid=jl.locationid and jl.locationtype=3 " +
       " union select concat(concat(c.name,'>',l.name,'>',s.sublocation,'>',ss.name),',',CAST(c.countryid as char),',',cast(l.locationid as char),',', cast(s.sublocationid as char),',',cast(ss.subsublocationid as char) ) from countries c inner join locations l on l.countryid=c.countryid " +
      " inner join locationsub s on s.locationid=l.locationid inner join locationsub_subs ss on ss.sublocationid=s.sublocationid where ss.subsublocationid=jl.locationid and jl.locationtype=4 " +
                         " union select  concat(groupname,',','0,0,0,0,',g.location_groupid) groupname from location_group g where g.location_groupid=jl.locationid and jl.locationtype=5   " +
                         " ) as location from jobs_locations_edit jl where jl.jobdetailid=?jobId and jl.jobeditId=?jobeditId ";
            return DAO.ExecuteReader(sql, new MySqlParameter("jobId", jobId), new MySqlParameter("jobeditId", jobeditId));
        }

        public static void copyLocation(int jobId, int newjobId)
        {
            string sql = "insert into jobs_locations_edit (jobdetailid, locationid,locationtype,jobeditId)" +
            "select ?jobId, locationid,locationtype,?newjobId from jobs_locations where jobdetailid=?jobId";

            DAO.ExecuteScalar(sql, new MySqlParameter("jobId", jobId), new MySqlParameter("newjobId", newjobId));
        }

        public static int getVersion(uint jobId)
        {
            int version = 0;
            string sql = "select max(version) from jobdetail_edit where jobdetailid=?jobid";
            object obj = DAO.ExecuteScalar(sql, new MySqlParameter("jobid", jobId));
            if (!string.IsNullOrEmpty(obj.ToString()))
                version = Convert.ToInt32(obj);
            return version;
        }

        public static MySqlDataReader getJobVersions(int jobId)
        {
            string sql = "select e.jobeditid,e.version,e.active,e.referenceno,e.title,e.versionstatus,u.username as createuser,u1.username as approveuser,date_format(versiondate,'%d-%b-%Y-%T') as versiondateFormate,date_format(versionstatusdate,'%d-%b-%Y-%T') as versionstatusdateFormate," +
                " e.published,e.active,j.version as liveversion from jobdetail_edit e inner join jobdetail j on e.jobdetailid=j.jobdetailid left outer join users u on e.versionuser=u.userid left outer join users u1 on e.versionapproveuser=u1.userid where e.jobdetailid=?jobid";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("jobid", jobId));

            return dr;
        }
    }
}