using MySql.Data.MySqlClient;
using System;

namespace YG_DataAccess
{
    public class JobDataAccess
    {
        //public MySqlDataReader SearchJobs(string industry, string role, string location, string workArrangement, string keywords, string isco)
        //{
        //    industry = string.IsNullOrEmpty(industry) ? string.Empty : industry;
        //    role = string.IsNullOrEmpty(role) ? string.Empty : role;
        //    workArrangement = string.IsNullOrEmpty(workArrangement) ? string.Empty : workArrangement;
        //    keywords = string.IsNullOrEmpty(keywords) ? string.Empty : keywords;
        //    location = string.IsNullOrEmpty(location) ? string.Empty : location;
        //    isco = string.IsNullOrEmpty(isco) ? string.Empty : isco;

        //    string sql = "Select JD.jobdetailid as jobid,JD.title as jobtitle,JD.referenceno ,JD.searchtitle,JD.summary,JT.jobtypeid," +
        //                 "JT.type as jobtype,Lo.locationid,Lo.name as regionname,JD.status,JD.createddate,case when (JD.modifieddate='0001-01-01 00:00:00' or JD.modifieddate is null) then JD.createddate else JD.modifieddate end as modifieddate," +
        //                 " case when (JD.modifieddate='0001-01-01 00:00:00' or JD.modifieddate is null) then 0 else 1 end as ismodified, JD.isApprove from jobdetail JD " +
        //                 "inner join jobtype JT on JD.typeid = JT.jobtypeid inner join locations Lo on JD.locationid = Lo.locationid " +
        //                 "inner join jobindustry JI on JD.jobIndustryId=JI.jobIndustryId inner join jobindustrysub JIS on JI.JobindustryId=JIS.JobIndustryId and JD.jobIndustrySubId = JIS.JobIndustrySubId " +
        //                 "where JD.isApprove and published and status in(2,3,4,6) and (JD.referenceno like concat_ws(?keyword,'%','%') or JD.title like concat_ws(?keyword,'%','%') or Lo.name like concat_ws(?keyword,'%','%')) " +
        //                 " and (JIS.SubClassification like concat_ws(?role,'%','%') ) " +
        //                 " and (JI.Classification like concat_ws(?industy,'%','%') ) " +
        //                 " and (JT.Type like concat_ws(?jobType,'%','%')) and (Lo.name like concat_ws(?location,'%','%')) ";
        //    if (!string.IsNullOrEmpty(isco))
        //        sql = " select JD.jobdetailid as jobid,JD.title as jobtitle,JD.referenceno ,JD.searchtitle,JD.summary,JT.jobtypeid," +
        //            "JT.type as jobtype,Lo.locationid,Lo.name as regionname,JD.status,JD.createddate,case when (JD.modifieddate='0001-01-01 00:00:00' or JD.modifieddate is null) then JD.createddate else JD.modifieddate end as modifieddate," +
        //            " case when (JD.modifieddate='0001-01-01 00:00:00' or JD.modifieddate is null) then 0 else 1 end as ismodified, JD.isApprove " +
        //            " from isco08 t1 left join isco08 t2 on t2.parentcode=t1.groupcode and t2.type=2 and t1.type=1 left join isco08 t3 on t3.parentcode=t2.groupcode and t3.type=3 " +
        //            " left join isco08 t4 on t4.parentcode=t3.groupcode and t4.type=4 right join jobdetail JD on t4.isco08id=JD.isco08id " +
        //            " inner join jobtype JT on JD.typeid = JT.jobtypeid inner join locations Lo on JD.locationid = Lo.locationid " +
        //            " inner join jobindustry JI on JD.jobIndustryId=JI.jobIndustryId inner join jobindustrysub JIS on JI.JobindustryId=JIS.JobIndustryId and JD.jobIndustrySubId = JIS.JobIndustrySubId " +
        //            " where  t1.title like concat('%',?isco,'%') or t2.title like concat('%',?isco,'%') or t3.title like concat('%',?isco,'%') or t4.title like concat('%',?isco,'%')" +
        //            " and JD.isApprove and published and status in(2,3,4,6) and (JD.referenceno like concat_ws(?keyword,'%','%') or JD.title like concat_ws(?keyword,'%','%') or Lo.name like concat_ws(?keyword,'%','%')) " +
        //                 " and (JIS.SubClassification like concat_ws(?role,'%','%') ) " +
        //                 " and (JI.Classification like concat_ws(?industy,'%','%') ) " +
        //                 " and (JT.Type like concat_ws(?jobType,'%','%')) and (Lo.name like concat_ws(?location,'%','%'))";

        //    sql = sql + " order by modifieddate desc";

        //    return DataAccess.ExecuteReader(sql, new MySqlParameter("keyword", keywords), new MySqlParameter("industy", industry), new MySqlParameter("role", role), new MySqlParameter("location", location),
        //        new MySqlParameter("jobType", workArrangement), new MySqlParameter("isco", isco));
        //}

        public MySqlDataReader SearchJobs(string industry, string role, string location, string workArrangement, string keywords, string isco)
        {
            industry = string.IsNullOrEmpty(industry) ? string.Empty : " and ( isr.id1 in (" + industry.Trim() + ")" + " or isr.id2 in (" + industry.Trim() + ")" + " or isr.id3 in (" + industry.Trim() + ")" + " or isr.id4 in (" + industry.Trim() + ")" + ")";
            role = string.IsNullOrEmpty(role) ? string.Empty : role.Trim();
            workArrangement = string.IsNullOrEmpty(workArrangement) ? string.Empty : workArrangement.Trim();
            keywords = string.IsNullOrEmpty(keywords) ? string.Empty : keywords.Trim();
            location = string.IsNullOrEmpty(location) ? string.Empty : location.Trim();
            isco = string.IsNullOrEmpty(isco) ? string.Empty : " and ( isc.id1 in (" + isco.Trim() + ")" + " or isc.id2 in (" + isco.Trim() + ")" + " or isc.id3 in (" + isco.Trim() + ")" + " or isc.id4 in (" + isco.Trim() + ")" + ")";

            //string sql = "Select JD.jobdetailid as jobid,JD.title as jobtitle,JD.referenceno ,JD.searchtitle,JD.summary,JT.jobtypeid, JT.type as jobtype,0 as locationid,'' as regionname,JD.status,JD.createddate, " +
            //             " case when (JD.modifieddate='0001-01-01 00:00:00' or JD.modifieddate is null) then JD.createddate else JD.modifieddate end as modifieddate, " +
            //             " case when (JD.modifieddate='0001-01-01 00:00:00' or JD.modifieddate is null) then 0 else 1 end as ismodified, JD.isApprove from isco08 t1 left join isco08 t2 on t2.parentcode=t1.groupcode and t2.type=2 and t1.type=1 " +
            //             " left join isco08 t3 on t3.parentcode=t2.groupcode and t3.type=3 left join isco08 t4 on t4.parentcode=t3.groupcode and t4.type=4 right join jobdetail JD on t4.isco08id=JD.isco08id " +
            //             " inner join jobtype JT on JD.typeid = JT.jobtypeid  left join jobs_locations jl on jl.jobdetailid=JD.jobdetailid left join countries c on (jl.locationid=c.countryid or jl.locationid=0) or jl.locationtype=1  " +
            //             " left join locations Lo on lo.countryid=c.countryid or (jl.locationid= Lo.locationid and jl.locationtype=2 )  left join locationsub s on s.locationid=lo.locationid " +
            //             " or (jl.locationid=s.sublocationid and jl.locationtype=3 ) left join locationsub_subs ss on ss.sublocationid=s.sublocationid or( jl.locationid=ss.subsublocationid and jl.locationtype=4 ) " +
            //             " where  (( ?isco ='' and jd.isco08id is null) or t1.title like concat('%',?isco,'%') or t2.title like concat('%',?isco,'%') or t3.title like concat('%',?isco,'%') or t4.title like concat('%',?isco,'%') )" +
            //             " and JD.isApprove and published and status in(2,3,4,6) and (JD.referenceno like concat_ws(?keyword,'%','%') or JD.title like concat_ws(?keyword,'%','%') ) " +
            //             " and (JT.Type like concat_ws(?jobType,'%','%')) and (( ?location='' and jl.locationid is null ) or Lo.name like concat_ws(?location,'%','%') or c.name like concat_ws(?location,'%','%') or s.sublocation like concat_ws(?location,'%','%') or ss.name like concat_ws(?location,'%','%'))";
            //sql = sql + " group by JD.jobdetailid,JD.title,JD.referenceno ,JD.searchtitle,JD.summary,JT.jobtypeid, JT.type,JD.status,JD.createddate,JD.modifieddate,JD.isApprove order by modifieddate desc";

            //    string sql = "Select JD.jobdetailid as jobid,JD.title as jobtitle,JD.referenceno ,JD.searchtitle,JD.summary,JT.jobtypeid, JT.type as jobtype,0 as locationid,'' as regionname,JD.status,JD.createddate," +
            //" case when (JD.modifieddate='0001-01-01 00:00:00' or JD.modifieddate is null) then JD.createddate else JD.modifieddate end as modifieddate,case when (JD.modifieddate='0001-01-01 00:00:00' or JD.modifieddate is null) then 0 else 1 end as ismodified," +
            //" JD.isApprove from jobdetail JD inner join jobtype JT on JD.typeid = JT.jobtypeid left join v_isco08 isc on isc.id4=JD.isco08id left join v_isicrev4 isr on isr.id4=JD.isicrev4id  left join jobs_locations jl on jl.jobdetailid=JD.jobdetailid " +
            //               " left join v_locations l on (jl.locationid = l.countryid and jl.locationtype=1) or (jl.locationid = l.locationid and jl.locationtype=2) or (jl.locationid = l.sublocationid and jl.locationtype=3) " +
            //               " or (jl.locationid = l.subsublocationid and jl.locationtype=4)  left join location_groupdetails gd on (jl.locationid = gd.locationid and gd.locationtype=1 and l.countryid=gd.locationid) "+
            //               " or (jl.locationid = gd.locationid and gd.locationtype=2 and l.locationid=gd.locationid) "+
            //" or (jl.locationid = gd.locationid and gd.locationtype=3 and l.sublocationid=gd.locationid) or (jl.locationid = gd.locationid and gd.locationtype=4 and l.subsublocationid=gd.locationid) left join location_group g on g.location_groupid=gd.location_groupid or (g.location_groupid= jl.locationid and jl.locationtype=5) " +
            //               " where JD.isApprove and published and status in(2,3,4,6) and (JD.referenceno like concat_ws(?keyword,'%','%') or JD.title like concat_ws(?keyword,'%','%') )" +
            //               " and (JT.Type like concat_ws(?jobType,'%','%')) and ((l.name like concat('%',?location,'%') ) or (l.locationname like concat('%',?location,'%') and jl.locationtype in(2,3,4)) "+
            //               " or (l.sublocation like concat('%',?location,'%') and jl.locationtype in(3,4)) or (l.subsublocation like concat('%',?location,'%') and jl.locationtype=4)  or g.groupname like concat('%',?location,'%')" +
            //               " or jl.locationid=0 or ?location='')" + industry + isco + " group by JD.jobdetailid,JD.title,JD.referenceno ,JD.searchtitle,JD.summary,JT.jobtypeid, JT.type,JD.status,JD.createddate,JD.modifieddate,JD.isApprove " +
            //               " order by modifieddate desc";

            string sql = "Select SQL_NO_CACHE JD.jobdetailid as jobid,JD.title as jobtitle,JD.referenceno ,JD.searchtitle,JD.summary,JT.jobtypeid, JT.type as jobtype,0 as locationid,'' as regionname,JD.status,JD.createddate," +
      " case when (JD.modifieddate='0001-01-01 00:00:00' or JD.modifieddate is null) then JD.createddate else JD.modifieddate end as modifieddate,case when (JD.modifieddate='0001-01-01 00:00:00' or JD.modifieddate is null) then 0 else 1 end as ismodified," +
      " JD.isApprove from jobdetail JD inner join jobtype JT on JD.typeid = JT.jobtypeid left join v_isco08 isc on isc.id4=JD.isco08id left join v_isicrev4 isr on isr.id4=JD.isicrev4id  left join jobs_locations jl on jl.jobdetailid=JD.jobdetailid " +
                     " left join v_locations l on (jl.locationid = l.countryid and jl.locationtype=1) or (jl.locationid = l.locationid and jl.locationtype=2) or (jl.locationid = l.sublocationid and jl.locationtype=3) " +
                     " or (jl.locationid = l.subsublocationid and jl.locationtype=4)  left join location_groupdetails gd on (jl.locationid = gd.locationid and gd.locationtype=1 and l.countryid=gd.locationid) " +
                     " or (jl.locationid = gd.locationid and gd.locationtype=2 and l.locationid=gd.locationid) " +
      " or (jl.locationid = gd.locationid and gd.locationtype=3 and l.sublocationid=gd.locationid) or (jl.locationid = gd.locationid and gd.locationtype=4 and l.subsublocationid=gd.locationid) left join location_group g on g.location_groupid=gd.location_groupid or (g.location_groupid= jl.locationid and jl.locationtype=5) " +
                     " where JD.isApprove and published and status in(2,3,4,6) and (JD.referenceno like concat_ws(?keyword,'%','%') or JD.title like concat_ws(?keyword,'%','%') )" +
                     " and (JT.Type like concat_ws(?jobType,'%','%')) and (((l.name =?location ) or (l.locationname=?location) and jl.locationtype in(2,3,4)) " +
                     " or (l.sublocation =?location and jl.locationtype in(3,4)) or (l.subsublocation =?location and jl.locationtype=4)  or (g.groupname =?location" +
                     " or jl.locationid=0 or ?location=''))" + industry + isco + " group by JD.jobdetailid,JD.title,JD.referenceno ,JD.searchtitle,JD.summary,JT.jobtypeid, JT.type,JD.status,JD.createddate,JD.modifieddate,JD.isApprove " +
                     " order by modifieddate desc";
            return DataAccess.ExecuteReader(sql, new MySqlParameter("keyword", keywords), new MySqlParameter("role", role), new MySqlParameter("location", location), new MySqlParameter("jobType", workArrangement));
        }

        public MySqlDataReader ListJobs(long firstPage,long lastPage)
        {
            string sql = "Select SQL_NO_CACHE JD.jobdetailid as jobid,JD.title as jobtitle,JD.referenceno,JD.searchtitle,JD.summary,JT.jobtypeid," +
                         "JT.type as jobtype,JD.status,JD.createddate,case when (JD.modifieddate='0001-01-01 00:00:00' or JD.modifieddate is null) then JD.createddate else JD.modifieddate end as modifieddate, " +
                         " case when (JD.modifieddate='0001-01-01 00:00:00' or JD.modifieddate is null) then 0 else 1 end as ismodified, JD.isApprove from jobdetail JD " +
                         "inner join jobtype JT on JD.typeid = JT.jobtypeid  where JD.isApprove and published and status in(2,3,4,6) order by modifieddate desc LIMIT ?firstPage ,?LastPage";

            MySqlDataReader reader = DataAccess.ExecuteReader(sql, new MySqlParameter("firstPage", firstPage), new MySqlParameter("LastPage", lastPage));

            return reader;
        }

        public MySqlDataReader GetJobByReferenceNo(string refNo)
        {
            //string sql = "select JD.JobDetailID,JD.locationID,JD.typeID,JD.salaryMin,JD.salaryMax,JD.ReferenceNo,JD.title,JD.subHeading,JD.searchTitle,JD.bullet1,JD.bullet2,JD.bullet3,JD.summary,JD.jobContent," +
            //            "JD.websiteURL,JD.adFooter,JD.isResidency,JD.status,JD.isApprove,JD.clientId,JD.createdDate,JT.Type as jobtype,Lo.Name as location,JD.jobIndustryId,JD.jobIndustrySubId,JD.hotJob,JD.salaryFrequency,JD.salaryCurrency,f.frequency " +
            //            "from jobdetail JD inner join jobtype JT on JD.typeID=JT.JobTypeId inner join locations Lo on JD.locationID=Lo.locationid left join frequency f on f.frequencyId=JD.salaryFrequency where ReferenceNo=?ReferenceNo and isApprove and published and status in(2,3,4,6)";
            string sql = "select SQL_NO_CACHE JD.JobDetailID,JD.locationID,JD.typeID,JD.salaryMin,JD.salaryMax,JD.ReferenceNo,JD.title,JD.subHeading,JD.searchTitle,JD.bullet1,JD.bullet2,JD.bullet3,JD.summary,JD.jobContent,JD.websiteURL,JD.adFooter," +
                         " JD.isResidency,JD.status,JD.isApprove,JD.clientId,JD.createdDate,JT.Type as jobtype,JD.jobIndustryId,JD.jobIndustrySubId,JD.hotJob,JD.salaryFrequency,JD.salaryCurrency,f.frequency " +
                         " from jobdetail JD inner join jobtype JT on JD.typeID=JT.JobTypeId left join frequency f on f.frequencyId=JD.salaryFrequency " +
                         //" left join jobs_locations jl on jl.jobdetailid=JD.jobdetailid " +
                         //" left join countries c on jl.locationid=c.countryid and jl.locationtype=1 left join locations Lo on jl.locationid= Lo.locationid and jl.locationtype=2 " +
                         //" left join locationsub s on jl.locationid=s.sublocationid and jl.locationtype=3 left join locationsub_subs ss on jl.locationid=ss.subsublocationid and jl.locationtype=4 " +
                         " where ReferenceNo=?ReferenceNo and isApprove and published and status in(2,3,4,6)";
            return DataAccess.ExecuteReader(sql, new MySqlParameter("ReferenceNo", refNo));
        }

        public MySqlDataReader HotJobs(int limit)
        {
            string sql = "Select SQL_NO_CACHE JD.jobdetailid as jobid,JD.title as jobtitle,JD.referenceno,JD.searchtitle,JD.summary,JT.jobtypeid,JD.jobContent,case when (JD.modifieddate='0001-01-01 00:00:00' or JD.modifieddate is null) then JD.createddate else JD.modifieddate end as modifieddate," +
                         "JT.type as jobtype,'' as locationid,'' as regionname,JD.status,JD.createddate,JD.isApprove from jobdetail JD " +
                         "inner join jobtype JT on JD.typeid = JT.jobtypeid  where JD.isApprove and published and status in(2,3,4,6) and hotjob order by modifieddate desc  LIMIT ?limit";

            MySqlDataReader reader = DataAccess.ExecuteReader(sql, new MySqlParameter("limit", limit));

            return reader;
        }

        public MySqlDataReader GetEssentialCriteria(int jobId)
        {
            string sql = "select EssentialCriteriaId,JobID,Description,AnswerLength,sortorder from essentialcriteria where JobID=?jobId";
            return DataAccess.ExecuteReader(sql, new MySqlParameter("jobId", jobId));
        }

        public MySqlDataReader GetDesirableCriteria(int jobId)
        {
            string sql = "select DesirableCriteriaID,JobID,Description,AnswerLength,sortorder from desirablecriteria where JobID=?jobId";
            return DataAccess.ExecuteReader(sql, new MySqlParameter("jobId", jobId));
        }

        public MySqlDataReader GetJobConsultant(int jobId)
        {
            string sql = "select SQL_NO_CACHE c.consultantid, c.first, c.last " +
              "from consultants as c " +
              "join job_consultants as j on j.consultantID = c.consultantid " +
              "where j.jobId = ?jobId " +
              "order by c.last, c.first ";
            return DataAccess.ExecuteReader(sql, new MySqlParameter("jobId", jobId));
        }

        public MySqlDataReader getJobLocation(int jobId)
        {
            string sql = "select concat('Anywhere') as location from jobs_locations jl  where jl.locationid=0 and jl.locationtype=1 and jl.jobdetailid=?jobId" +
        " union select concat(c.name) as location from jobs_locations jl join countries c on c.countryid=jl.locationid where  jl.locationtype=1 and  jl.locationid!=0 and jl.jobdetailid=?jobId" +
       " union select concat(c.name,' > ',l.name) as location from countries c inner join locations l on l.countryid=c.countryid join jobs_locations jl on l.locationid=jl.locationid " +
       " where  jl.locationtype=2 and jl.jobdetailid=?jobId" +
       " union select concat(c.name,' > ',l.name,' > ',s.sublocation) as location from countries c inner join locations l on l.countryid=c.countryid " +
       " inner join locationsub s on s.locationid=l.locationid join jobs_locations jl on jl.locationid=s.sublocationid where  jl.locationtype=3 and jl.jobdetailid=?jobId" +
       " union select concat(c.name,' > ',l.name,' >',s.sublocation,' > ',ss.name) as location from countries c inner join locations l on l.countryid=c.countryid " +
      " inner join locationsub s on s.locationid=l.locationid inner join locationsub_subs ss on ss.sublocationid=s.sublocationid join jobs_locations jl on ss.subsublocationid=jl.locationid where jl.locationtype=4 and jl.jobdetailid=?jobId" +
       "  union select  groupname as location from location_group g  join jobs_locations jl on g.location_groupid=jl.locationid where jl.locationtype=5 and jl.jobdetailid=?jobId   ";
            return DataAccess.ExecuteReader(sql, new MySqlParameter("jobId", jobId));
        }

        public MySqlDataReader GetJobsCountByIndustry()
        {
            string sql = "select isicRev4id,jobdetailid from jobdetail jd";
            return DataAccess.ExecuteReader(sql);
        }

        public MySqlDataReader GetJobsCountByOccupation()
        {
            string sql = "select isco08id,jobdetailid from jobdetail jd";
            return DataAccess.ExecuteReader(sql);
        }
    }
}