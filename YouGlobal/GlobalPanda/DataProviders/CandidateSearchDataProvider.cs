using GlobalPanda.BusinessInfo;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace GlobalPanda.DataProviders
{
    public class CandidateSearchDataProvider
    {
        public static int insertCandidateSearch(string label, string keyword, string description, int userid, DateTime createddate, bool approved, bool assigned)
        {
            int id = 0;
            string sql = "Insert into candidate_search(label,keyword,description,userid,createdDate,approved,assigned) Values(?label,?keyword,?description,?userid,?createdDate,?approved,?assigned);select last_insert_id()";
            id = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("label", label), new MySqlParameter("keyword", keyword), new MySqlParameter("description", description), new MySqlParameter("userid", userid),
                  new MySqlParameter("createdDate", createddate), new MySqlParameter("approved", approved), new MySqlParameter("assigned", assigned)));

            return id;
        }

        public static int insertCandidateSearch(string label, string keyword, string description, int userid, DateTime createddate, bool approved, bool assigned, List<CandidateSearchFilterInfo> lstSearchFilter)
        {
            int id = 0;
            string sql = "Insert into candidate_search(label,keyword,description,userid,createdDate,approved,assigned) Values(?label,?keyword,?description,?userid,?createdDate,?approved,?assigned);select last_insert_id()";
            id = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("label", label), new MySqlParameter("keyword", keyword), new MySqlParameter("description", description), new MySqlParameter("userid", userid),
                  new MySqlParameter("createdDate", createddate), new MySqlParameter("approved", approved), new MySqlParameter("assigned", assigned)));

            foreach (CandidateSearchFilterInfo filter in lstSearchFilter)
            {
                insertCandidateSearchFilter(id, filter.FilterType, filter.FilterText, filter.FilterValue, filter.IsInclude);
            }
            return id;
        }

        public static void insertCandidateSearchFilter(int searchId, int filterType, string filterText, string filterValue, bool isInclude)
        {
            string sql = "insert into candidate_searchfilter (candidate_searchid,filter_type,filter_text,filter_value,isinclude) values (?searchId,?filtertype,?filtertext,?filtervalue,?isinclude)";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("searchid", searchId), new MySqlParameter("filtertype", filterType), new MySqlParameter("filtertext", filterText), new MySqlParameter("filtervalue", filterValue), new MySqlParameter("isinclude", isInclude));
        }

        public static void deleteCandidateSearchFilter(int searchId)
        {
            string sql = "delete from candidate_searchfilter where candidate_searchid=?searchid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("searchid", searchId));
        }

        public static void updateCandidateSearch(int candidatesearchid, string label, string keyword, string description)
        {
            string sql = "update candidate_search set label = ?label,keyword=?keyword,description=?description" +
                " where candidate_searchid = ?candidate_searchid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("label", label), new MySqlParameter("keyword", keyword),
                new MySqlParameter("description", description), new MySqlParameter("candidate_searchid", candidatesearchid));
        }

        public static void updateCandidateSearch(int candidatesearchid, string label, string keyword, string description, List<CandidateSearchFilterInfo> lstFilter)
        {
            string sql = "update candidate_search set label = ?label,keyword=?keyword,description=?description" +
                " where candidate_searchid = ?candidate_searchid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("label", label), new MySqlParameter("keyword", keyword), new MySqlParameter("description", description), new MySqlParameter("candidate_searchid", candidatesearchid));

            deleteCandidateSearchFilter(candidatesearchid);
            foreach (CandidateSearchFilterInfo filter in lstFilter)
            {
                insertCandidateSearchFilter(candidatesearchid, filter.FilterType, filter.FilterText, filter.FilterValue, filter.IsInclude);
            }
        }

        public static void insertCandidateSearchEdit(int candidatesearchid, string keyword, int frequency)
        {
            string sql = "Insert into candidate_searchedit(candidate_searchid,keyword,frequency) Values(?candidate_searchid,?keyword,?frequency);";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidate_searchid", candidatesearchid), new MySqlParameter("keyword", keyword), new MySqlParameter("frequency", frequency));
        }

        public static void insertCandidateSearchEditFilter(int searchid)
        {
            string sql = "insert into candidate_searchfilteredit (candidate_searchid,filter_type,filter_text,filter_value,isinclude) select candidate_searchid,filter_type,filter_text,filter_value,isinclude from candidate_searchfilter where candidate_searchid=?searchid ";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("searchid", searchid));
        }

        public static void insertCandidateSearchEdit(int candidatesearchid, string keyword, int frequency, List<CandidateSearchFilterInfo> lstSearchFilter)
        {
            string sql = "Insert into candidate_searchedit(candidate_searchid,keyword,frequency) Values(?candidate_searchid,?keyword,?frequency);";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidate_searchid", candidatesearchid), new MySqlParameter("keyword", keyword), new MySqlParameter("frequency", frequency));

            foreach (CandidateSearchFilterInfo filter in lstSearchFilter)
            {
                insertCandidateSearchFilteredit(candidatesearchid, filter.FilterType, filter.FilterText, filter.FilterValue, filter.IsInclude);
            }
        }

        public static void insertCandidateSearchFilteredit(int searchId, int filterType, string filterText, string filterValue, bool isInclude)
        {
            string sql = "insert into candidate_searchfilteredit (candidate_searchid,filter_type,filter_text,filter_value,isinclude) values (?searchId,?filtertype,?filtertext,?filtervalue,?isinclude)";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("searchid", searchId), new MySqlParameter("filtertype", filterType), new MySqlParameter("filtertext", filterText), new MySqlParameter("filtervalue", filterValue), new MySqlParameter("isinclude", isInclude));
        }

        public static void deleteCandidateSearchFilteredit(int searchId)
        {
            string sql = "delete from candidate_searchfilteredit where candidate_searchid=?searchid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("searchid", searchId));
        }

        public static void updateCandidateSearchEdit(int candidatesearchid, string keyword)
        {
            string sql = "update candidate_searchedit set keyword=?keyword" +
                " where candidate_searchid = ?candidate_searchid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("keyword", keyword), new MySqlParameter("candidate_searchid", candidatesearchid));
        }

        public static void updateCandidateSearchEdit(int candidatesearchid, string keyword, List<CandidateSearchFilterInfo> lstSearchFilter)
        {
            string sql = "update candidate_searchedit set keyword=?keyword" +
                " where candidate_searchid = ?candidate_searchid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("keyword", keyword), new MySqlParameter("candidate_searchid", candidatesearchid));

            deleteCandidateSearchEdit(candidatesearchid);
            foreach (CandidateSearchFilterInfo filter in lstSearchFilter)
            {
                insertCandidateSearchFilteredit(candidatesearchid, filter.FilterType, filter.FilterText, filter.FilterValue, filter.IsInclude);
            }
        }

        public static void updateCandidateSearchEdit(int candidatesearchid, int frequency)
        {
            string sql = "update candidate_searchedit set frequency=?frequency" +
                " where candidate_searchid = ?candidate_searchid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("frequency", frequency), new MySqlParameter("candidate_searchid", candidatesearchid));
        }

        public static void deleteCandidateSearchEdit(int candidatesearchid)
        {
            string sql = "delete from candidate_searchedit where candidate_searchid=?candidatesearchid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidatesearchid", candidatesearchid));
            deleteCandidateSearchFilteredit(candidatesearchid);
        }

        public static void updateCandidateSearchActive(int searchId, bool active)
        {
            string sql = "update candidate_search set active=?active where candidate_searchid = ?candidate_searchid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidate_searchid", searchId), new MySqlParameter("active", active));
        }

        public static void deleteCandidateSearchDeActive(int candidateSearchId)
        {
            string sql = "delete from candidates_jobs where searchid=?candidateSearchId";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateSearchId", candidateSearchId));
        }

        public static void deleteCandidateSearch(int candidateSearchId)
        {
            string sql = "delete from candidate_search where candidate_searchid = ?candidate_searchid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidate_searchid", candidateSearchId));
            deleteCandidateSearchFilter(candidateSearchId);
        }

        public static bool existsCandidateSearchEdit(int candidateSearchId)
        {
            string sql = "select candidate_Searchid from candidate_searchedit where candidate_searchid=?candidatesearchid";
            bool exists = false;
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("candidatesearchid", candidateSearchId));
            if (dr.HasRows)
                exists = true;
            dr.Close();
            dr.Dispose();
            return exists;
        }

        public static void insertCandidateSearchJobs(int searchid, List<int> jobList)
        {
            string sql = "insert into candidatesearch_jobs (candidate_searchid,jobDetailId) values (?searchid,?jobId)";
            foreach (int jobId in jobList)
            {
                if (!existCandidateSearch(jobId, searchid))
                    DAO.ExecuteNonQuery(sql, new MySqlParameter("searchid", searchid), new MySqlParameter("jobid", jobId));
            }
        }

        public static int getCountcandidateSearchAssignedtoJob(int jobId)
        {
            int count = 0;
            string sql = "select count(jobdetailid) from candidatesearch_jobs where jobdetailId=?jobid ";
            object cnt = DAO.ExecuteScalar(sql, new MySqlParameter("jobid", jobId));
            count = Convert.ToInt32(cnt == null ? 0 : cnt);
            return count;
        }

        public static bool existCandidateSearch(int jobid, int searchid)
        {
            bool exist;
            string sql = "select jobDetailid from candidatesearch_jobs where candidate_searchid=?searchid and jobDetailid=?jobid";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("searchid", searchid), new MySqlParameter("jobid", jobid));
            if (reader.HasRows)
                exist = true;
            else
                exist = false;
            reader.Close();
            reader.Dispose();

            return exist;
        }

        public static void deleteCandidateSearchJobs(int searchid, int jobId)
        {
            string sql = "delete from candidatesearch_jobs where  candidate_searchid=?searchid and jobDetailId=?jobId";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("searchid", searchid), new MySqlParameter("jobid", jobId));
        }

        public static void deleteCandidateSearchJobs(int searchid)
        {
            string sql = "delete from candidatesearch_jobs where  candidate_searchid=?searchid ";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("searchid", searchid));
        }

        public static MySqlDataReader getCandidateSearchJobs(int searchid)
        {
            string sql = "SELECT j.jobdetailid,j.title,j.referenceNo FROM candidatesearch_jobs cj inner join jobdetail j on cj.jobdetailid=j.jobdetailid where candidate_searchid=?searchid";
            return DAO.ExecuteReader(sql, new MySqlParameter("searchid", searchid));
        }

        public static MySqlDataReader getCandidateSearchByJobId(int jobid)
        {
            string sql = "SELECT cs.candidate_searchid as searchid,cs.label,cs.keyword,cs.approved,date_format(cs.createdDate,'%d-%b-%Y') as createddate_format,cs.createdDate,  u.username,cs.active " +
            " FROM candidatesearch_jobs cj inner join candidate_search cs on cj.candidate_searchid=cs.candidate_searchid left outer join users u on cs.userid = u.userid where cj.jobdetailid=?jobid";
            return DAO.ExecuteReader(sql, new MySqlParameter("jobid", jobid));
        }

        public static MySqlDataReader getOnlyCandidateSearchByJobId(int jobid)
        {
            string sql = "select searchid from candidates_jobs where jobid=?jobid and category=2 group by searchid;";
            return DAO.ExecuteReader(sql, new MySqlParameter("jobid", jobid));
        }

        public static MySqlDataReader SearchCandidate(string keyword, bool? approved, int userid, string sortexpression)
        {
            string sql = "Select cs.candidate_searchid as searchid,cs.label,case when ce.keyword is null then cs.keyword else ce.keyword end as keyword,cs.approved,date_format(cs.createdDate,'%d-%b-%Y') as createddate_format," +
                          " cs.createdDate,  u.username,cs.active,COALESCE(cf.frequency,0) as frequency,count(cj.jobdetailid) as assigend,count(ce.candidate_Searchid) as edited " +
                         " from candidate_search cs left outer join users u on cs.userid = u.userid left join candidatesearch_frequency cf on cf.candidate_searchid=cs.candidate_searchid " +
                         " left join candidatesearch_jobs cj on cj.candidate_Searchid=cs.candidate_searchid left join candidate_searchedit ce on ce.candidate_searchid=cs.candidate_searchid " +
                         "  where ( cs.label like concat_ws(?keyword,'%','%') or cs.keyword like concat_ws(?keyword,'%','%')) and (cs.approved = ?approved  OR ?approved is null) and (cs.userid=?userId or ?userid=0) " +
                         " Group by cs.candidate_searchid ,cs.label,cs.keyword,cs.approved,cs.createdDate, u.username,cs.active,cf.frequency  order by ?sort";
            return DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword), new MySqlParameter("approved", approved), new MySqlParameter("userid", userid), new MySqlParameter("sort", sortexpression));
        }

        public static MySqlDataReader getSearchCandidateByJobid(int jobid)
        {
            string sql = "Select cs.candidate_searchid as searchid,cs.label,cs.keyword,cs.approved,cs.description,date_format(cs.createdDate,'%d-%b-%Y') as createddate_format,cs.createdDate,  u.username,cs.active,COALESCE(cf.frequency,0) as frequency,f.frequency as frequencyname " +
                        " from candidate_search cs left outer join users u on cs.userid = u.userid inner join candidatesearch_jobs cj on cj.candidate_searchid=cs.candidate_searchid left join candidatesearch_frequency cf on cf.candidate_searchid=cs.candidate_searchid " +
                        " left join frequency f on cf.frequency=f.frequencyid where cs.approved = 1 and cs.active=1 and cj.jobdetailid=?jobid ";
            return DAO.ExecuteReader(sql, new MySqlParameter("jobid", jobid));
        }

        public static MySqlDataReader getActiveCandidatesearch()
        {
            string sql = "Select cs.candidate_searchid as searchid,cs.label,cs.keyword,cs.approved,date_format(cs.createdDate,'%d-%b-%Y') as createddate,u.username,cs.active " +
                         "from candidate_search cs left outer join users u on cs.userid = u.userid where " +
                         "cs.approved = 1 and cs.active=1 order by cs.candidate_searchid";
            return DAO.ExecuteReader(sql);
        }

        public static MySqlDataReader listCandidatesearch()
        {
            string sql = "Select cs.candidate_searchid as searchid,cs.label,cs.keyword,cs.approved,date_format(cs.createdDate,'%d-%b-%Y') as createddate,u.username,cs.active " +
                         "from candidate_search cs left outer join users u on cs.userid = u.userid order by cs.candidate_searchid";
            return DAO.ExecuteReader(sql);
        }

        //public static MySqlDataReader SearchCandidate(string keyword, bool? approved, int userid)
        //{
        //    string sql = "Select cs.candidate_searchid as searchid,cs.label,cs.keyword,cs.approved,date_format(cs.createdDate,'%d-%b-%Y') as createddate,u.username,cs.active "
        //    + " from candidate_search cs left outer join users u on cs.userid = u.userid where " +
        //        " ( label like concat_ws(?keyword,'%','%') or keyword like concat_ws(?keyword,'%','%')) and (approved = ?approved OR ?approved is null) and userid = ?userid order by candidate_searchid";
        //    return DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword), new MySqlParameter("approved", approved), new MySqlParameter("userid", userid));
        //}

        public static MySqlDataReader getSearchCandidate(int candidatesearchid)
        {
            string sql = "Select candidate_searchid as searchid,label,keyword,description,userid from candidate_search where " +
                "candidate_searchid = ?candidatesearchid";
            return DAO.ExecuteReader(sql, new MySqlParameter("candidatesearchid", candidatesearchid));
        }

        public static MySqlDataReader getSearchCandidateFilter(int searchId)
        {
            string sql = "select * from candidate_searchfilter where candidate_searchid=?searchid";
            return DAO.ExecuteReader(sql, new MySqlParameter("searchid", searchId));
        }

        public static MySqlDataReader getSearchCandidateEdit(int candidatesearchid)
        {
            string sql = "Select cs.candidate_searchid as searchid,label,case when ce.keyword is null then cs.keyword else ce.keyword end as keyword,cs.keyword as oldkeyword,description,userid " +
                " from candidate_search cs left join candidate_searchedit ce on ce.candidate_Searchid=cs.candidate_searchid where cs.candidate_searchid = ?candidatesearchid";
            return DAO.ExecuteReader(sql, new MySqlParameter("candidatesearchid", candidatesearchid));
        }

        public static MySqlDataReader getSearchCandidateFilterEdit(int searchId)
        {
            string sql = "select * from candidate_searchfilteredit where candidate_searchid=?searchid";
            return DAO.ExecuteReader(sql, new MySqlParameter("searchid", searchId));
        }

        public static void updateCandidateSearchApprove(int candidatesearchId, bool approve, bool active)
        {
            string sql = "update candidate_search set approved = ?approve,active=?active where candidate_searchid = ?candidatesearchid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidatesearchid", candidatesearchId), new MySqlParameter("approve", approve), new MySqlParameter("active", active));
            if (existsCandidateSearchEdit(candidatesearchId))
            {
                sql = "update candidate_Search cs inner join candidate_searchedit ce on ce.candidate_Searchid = cs.candidate_Searchid set cs.keyword=ce.keyword where cs.candidate_Searchid=?candidatesearchId ";
                DAO.ExecuteNonQuery(sql, new MySqlParameter("candidatesearchId", candidatesearchId));
                sql = "select frequency from candidate_Searchedit where candidate_searchid=?candidatesearchid";
                int frequency = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("candidatesearchid", candidatesearchId)));
                if (existCandidateSearchFrequency(candidatesearchId))
                {
                    updateCandidateSearchFrequency(candidatesearchId, frequency);
                }
                else
                {
                    insertCandidateSearchFrequency(candidatesearchId, frequency);
                }

                deleteCandidateSearchFilter(candidatesearchId);
                sql = "insert into candidate_searchfilter (Candidate_searchid,filter_type,filter_text,filter_value,isinclude) select Candidate_searchid,filter_type,filter_text,filter_value,isinclude from candidate_searchfilteredit where candidate_searchid=?searchid";
                DAO.ExecuteNonQuery(sql, new MySqlParameter("searchid", candidatesearchId));

                deleteCandidateSearchEdit(candidatesearchId);
            }
        }

        public static MySqlDataReader getKeyword(int candidatesearchid)
        {
            string sql = "select keyword from candidate_search where candidate_searchid = ?candidatesearchid";
            return DAO.ExecuteReader(sql, new MySqlParameter("candidatesearchid", candidatesearchid));
        }

        public static MySqlDataReader filterJob(string keyword, uint consultantid, uint clientid, uint status, string sortexpression, int searchid)
        {
            string sql = "Select distinct JD.jobdetailid as jobid,JD.title as jobtitle,JD.referenceno as refno,JT.jobtypeid,JT.type as jobtype,Lo.locationid," +
                         "Lo.name as regionname,date_format(JD.createddate,'%d-%b-%Y') as createddate,JD.clientid,JS.status," +
                         "cl.clientname,JD.isApprove from jobdetail JD inner join jobtype JT on JD.typeid = JT.jobtypeid inner join locations Lo on JD.locationid = Lo.locationid left outer join Client Cl " +
                         "on JD.clientid = Cl.clientid left outer join jobstatus JS on JS.jobstatusid = JD.status left outer join job_consultants JC on JD.jobdetailid = JC.jobid " +
                         "Where (JD.referenceno like concat_ws(?keyword,'%','%') or JD.title like concat_ws(?keyword,'%','%')) and (JC.consultantid =?consultantid OR ?consultantid =0) and (JD.clientid =?clientid OR ?clientid=0) " +
                         "and (JD.status =?status OR ?status =0) and JD.jobdetailid not in(select jobDetailId from candidatesearch_jobs where candidate_searchid=?searchid) order by " + sortexpression;

            return DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword), new MySqlParameter("consultantid", consultantid), new MySqlParameter("clientid", clientid), new MySqlParameter("status", status), new MySqlParameter("searchid", searchid));
        }

        public static void insertCandidateJobsAssigned(int candidateid, int jobid)
        {
            string sql = "insert into candidate_jobsassigned (candidateid,jobid) values (?candidateid,?jobid)";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("jobid", jobid));
        }

        public static void deleteCandidateJobsAssigned(int candidateid, int jobid)
        {
            string sql = "delete from candidate_jobsassigned  where candidateid=?candidateid and jobid=?jobid";

            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("jobid", jobid));
        }

        public static void deleteCandidateJobsAssignedActual(int candidateid, int jobid)
        {
            string sql = "delete from candidate_jobsassigned  where candidateid=?candidateid and jobid=?jobid";
            // string sql = "update candidate_jobsassigned set deleted=1 where candidateid=?candidateid and jobid=?jobid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("jobid", jobid));
        }

        public static bool existCandidateJobsAssigned(int candidateid, int jobid)
        {
            bool exist;
            string sql = "select candidateid from candidate_jobsassigned where candidateid=?candidateid and jobid=?jobid";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("jobid", jobid));
            if (reader.HasRows)
                exist = true;
            else
                exist = false;
            reader.Close();
            reader.Dispose();

            return exist;
        }

        public static bool existCandidateJobs(int candidateid, int jobid, int category)
        {
            bool exist;
            string sql = "select candidates_jobsid from Candidates_jobs where candidateid=?candidateid and jobid=?jobid and category=?category and deleted=0";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("jobid", jobid), new MySqlParameter("category", category));
            if (reader.HasRows)
                exist = true;
            else
                exist = false;
            reader.Close();
            reader.Dispose();

            return exist;
        }

        public static bool isDeletedCandidateJobsAssigned(int candidateid, int jobid)
        {
            bool exist;
            string sql = "select candidateid from candidate_jobsassigned where deleted=1 and candidateid=?candidateid and jobid=?jobid";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("jobid", jobid));
            if (reader.HasRows)
                exist = true;
            else
                exist = false;
            reader.Close();
            reader.Dispose();

            return exist;
        }

        public static void revertDeletedCandidateJobs(int candidateid, int jobid)
        {
            string sql = "update candidate_jobsassigned set deleted=0 where candidateid=?candidateid and jobid=?jobid;" +
                " update candidates_jobs set deleted=0 where candidateid=?candidateid and jobid=?jobid";

            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("jobid", jobid));
        }

        public static int getCandidateJobsUserRating(int candidateid, int jobid)
        {
            int rating = 0;
            string sql = "select COALESCE(rating,0) from candidates_jobsrating where candidateid=?candidateid and jobid=?jobid and userid=?userid";
            rating = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("jobid", jobid), new MySqlParameter("userid", GPSession.UserId)));
            return rating;
        }

        public static int? getCandidateJobsMaxRating(int candidateid, int jobid)
        {
            object rating = 0;
            string sql = "select MAX(rating) from candidates_jobsrating where candidateid=?candidateid and jobid=?jobid ";

            rating = DAO.ExecuteScalar(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("jobid", jobid));

            int? _return = null;
            if (!string.IsNullOrEmpty(rating.ToString()))
                _return = Convert.ToInt32(rating);
            return _return;
        }

        public static void insertCandidateJobs(int candidateid, int jobid, int category, int? searchid)
        {
            uint id;
            string sql = "insert into candidates_jobs (candidateid,jobid,category,createddate,searchid,userid) values (?candidateid,?jobid,?category,?createddate,?searchid,?userid);select last_insert_id()";
            id = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("jobid", jobid), new MySqlParameter("category", category), new MySqlParameter("createddate", DateTime.UtcNow),
                new MySqlParameter("searchid", searchid), new MySqlParameter("userid", GPSession.UserId)));
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.candidate_jobs;
            info.TypeId = (int)HistoryInfo.ActionType.Add;
            info.RecordId = id;
            info.ModifiedDate = DateTime.UtcNow;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "All", NewValue = "candidateId:" + candidateid.ToString() + ", jobId:" + jobid.ToString() });

            HistoryDataProvider history = new HistoryDataProvider();
            history.insertHistory(info);
        }

        public static void updateCandidateJobs(int candidateid, int jobid, int statusid, string notes, uint cjId)
        {
            DataTable dt = getCandidateJobsById(cjId);
            string sql = "update candidates_jobs set statusiconid=?statusid,notes=?notes where candidateid=?candidateid and jobid=?jobid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("jobid", jobid), new MySqlParameter("statusid", statusid), new MySqlParameter("notes", notes));

            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.candidate_jobs;
            info.TypeId = (int)HistoryInfo.ActionType.Edit;
            info.RecordId = cjId;
            info.ModifiedDate = DateTime.UtcNow;
            info.Details = new List<HistoryDetailInfo>();

            if (dt.Rows[0]["notes"].ToString() != notes)
            {
                info.Details.Add(new HistoryDetailInfo { ColumnName = "notes", NewValue = notes, OldValue = dt.Rows[0]["notes"].ToString() });
            }

            if (dt.Rows[0]["statusiconid"].ToString() != statusid.ToString())
            {
                info.Details.Add(new HistoryDetailInfo { ColumnName = "statusiconid", NewValue = statusid.ToString(), OldValue = dt.Rows[0]["statusiconid"].ToString() });
            }

            if (info.Details.Count > 0)
            {
                HistoryDataProvider history = new HistoryDataProvider();
                history.insertHistory(info);
            }
        }

        public static void deleteCandidateJobsActual(int candidateid, int jobid, int searchid)
        {
            string sql = "delete from candidates_jobs  where candidateid=?candidateid and jobid=?jobid and searchid=?searchid";

            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("jobid", jobid), new MySqlParameter("searchid", searchid));
        }

        public static void deleteCandidateJobs(int candidateid, int jobid)
        {
            //string sql = "delete from candidates_jobs  where candidateid=?candidateid and jobid=?jobid";
            string sql = "update candidates_jobs set deleted=1 where candidateid=?candidateid and jobid=?jobid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("jobid", jobid));
        }

        public static void deleteCandidateJobs(int candidateid, int jobid, uint userid)
        {
            string sql = "delete from candidates_jobs  where candidateid=?candidateid and jobid=?jobid and (userid=?userid or ?userid=0) and category=3";
            // string sql = "update candidates_jobs set deleted=1 where candidateid=?candidateid and jobid=?jobid ";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("jobid", jobid), new MySqlParameter("userid", userid));
        }

        public static void hideCandidateJobs(int candidateid, int jobid)
        {
            //string sql = "delete from candidates_jobs  where candidateid=?candidateid and jobid=?jobid";
            string sql = "update candidates_jobs set hide=1 where candidateid=?candidateid and jobid=?jobid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("jobid", jobid));
        }

        public static void updateCandidateJobsIsDelete(int candidateid, int jobid)
        {
            string sql = "update candidates_jobs set deleted=0 where candidateid=?candidateid and jobid=?jobid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("jobid", jobid));
        }

        public static bool existCandidateJobs(int candidateid, int jobid)
        {
            bool exist;
            string sql = "select candidateid from candidates_jobs where candidateid=?candidateid and jobid=?jobid";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("jobid", jobid));
            if (reader.HasRows)
                exist = true;
            else
                exist = false;
            reader.Close();
            reader.Dispose();

            return exist;
        }

        public static bool existCandidateJobsBySearch(int candidateid, int jobid, int searchid)
        {
            bool exist;
            string sql = "select candidateid from candidates_jobs where candidateid=?candidateid and jobid=?jobid and searchid=?searchid and category=2";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("jobid", jobid), new MySqlParameter("searchid", searchid));
            if (reader.HasRows)
                exist = true;
            else
                exist = false;
            reader.Close();
            reader.Dispose();

            return exist;
        }

        public static DataTable getCandidateJobsAssigned(int jobid, string sortexpression)
        {
            string sql = "SELECT c.candidateid as candidateid,c.title ,c.first ,c.last,c.middle,coalesce(rating,0) rating,c.restrictedaccess" +
                " FROM candidate_jobsassigned cj inner join candidates c on cj.candidateid=c.candidateid " +
                " left join candidates_jobsrating r on r.candidateid=cj.candidateid and r.jobid=cj.jobid and r.userid=?userid" +
                " where cj.deleted=0 and cj.jobid=?jobid  order by " + sortexpression + ";";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("jobid", jobid), new MySqlParameter("userid", GPSession.UserId));
            DataTable dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public static DataTable getCandidateJobsBySearchID(int jobid, int searchId)
        {
            string sql = "SELECT cj.candidateid as candidateid" +
                " FROM candidates_jobs cj inner join candidates c on cj.candidateid=c.candidateid " +
                " where cj.deleted=0 and cj.jobid=?jobid  and cj.searchid=?searchid;";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("jobid", jobid), new MySqlParameter("searchid", searchId));
            DataTable dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public static DataTable getCandidateJobsBySearchID(int searchId)
        {
            string sql = "SELECT cj.candidateid as candidateid, jobid" +
                " FROM candidates_jobs cj inner join candidates c on cj.candidateid=c.candidateid " +
                " where cj.deleted=0 and cj.searchid=?searchid;";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("searchid", searchId));
            DataTable dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public static DataTable getCandidateJobsAssignedByCategory(int jobid, string categoryId, string searchids, string sortexpression)
        {
            string sql = "SELECT  c.candidateid as candidateid,c.title ,c.first ,c.last,c.middle,coalesce(r.rating,0) rating,r.rating as ratingActual,nt.note,c.restrictedaccess,min(cat1.createddate) as createddate,date_format(min(cat1.createddate) ,'%d-%b-%Y-%T') as createddate_format " +
                //" (select min(createddate) from  candidates_jobs where candidateid=cj.candidateid and jobid=cj.jobid and cj.deleted=0  and cat.category in(1,3,4)) as createddate, " +
                //" (select date_format(min(createddate) ,'%d-%b-%Y-%T') from  candidates_jobs where candidateid=cj.candidateid and jobid=cj.jobid and cj.deleted=0  and cat.category in(1,3,4)) as createddate_format "+
                " FROM candidate_jobsassigned cj inner join candidates c on cj.candidateid=c.candidateid " +
                " inner join candidates_jobs cat on cat.candidateid=cj.candidateid and cat.jobid=cj.jobid and cj.deleted=0  " +
                " join candidates_jobs cat1 on cat1.candidateid=cj.candidateid and cat1.jobid=cj.jobid and cj.deleted=0 " +
                " left join candidates_jobsrating r on r.candidateid=cj.candidateid and r.jobid=cj.jobid and r.userid=?userid " +
                " left join candidates_notes nt on nt.candidateid=c.candidateid and nt.auto=0 " +
                " where cj.jobid=?jobid and cj.deleted=0  and cat.hide=0 " + categoryId + " group by c.candidateid,c.title ,c.first ,c.last,c.middle,coalesce(r.rating,0) union " +

                "SELECT c.candidateid as candidateid,c.title ,c.first ,c.last,c.middle,coalesce(r.rating,0) rating,r.rating as ratingActual,nt.note,c.restrictedaccess,min(cat1.createddate) as createddate,date_format(min(cat1.createddate) ,'%d-%b-%Y-%T') as createddate_format " +
                //" (select min(createddate) from  candidates_jobs where candidateid=cj.candidateid and jobid=cj.jobid and cat.category=2 and cj.deleted=0  ) as createddate, " +
                //" (select date_format(min(createddate) ,'%d-%b-%Y-%T') from  candidates_jobs where candidateid=cj.candidateid and jobid=cj.jobid and cat.category=2 and cj.deleted=0  ) as createddate_format "+
                " FROM candidate_jobsassigned cj inner join candidates c on cj.candidateid=c.candidateid " +
                " inner join candidates_jobs cat on cat.candidateid=cj.candidateid and cat.jobid=cj.jobid and cj.deleted=0 " +
                " join candidates_jobs cat1 on cat1.candidateid=cj.candidateid and cat1.jobid=cj.jobid and cj.deleted=0 " +
                " left join candidates_jobsrating r on r.candidateid=cj.candidateid and r.jobid=cj.jobid and r.userid=?userid " +
                " left join candidates_notes nt on nt.candidateid=c.candidateid and nt.auto=0 " +
                " where cj.jobid=?jobid and cat.category=2 and cj.deleted=0 and cat.hide=0 " + searchids + " group by c.candidateid,c.title ,c.first ,c.last,c.middle,coalesce(r.rating,0) ";

            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("jobid", jobid), new MySqlParameter("userid", GPSession.UserId));

            DataTable dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public static DataTable getCandidateJobsAssignedByCategory(int jobid, string categoryId, string sortexpression)
        {
            string sql = "SELECT  c.candidateid as candidateid,c.title ,c.first ,c.last,c.middle,coalesce(r.rating,0) rating,r.rating as ratingActual,nt.note,c.restrictedaccess,min(cat1.createddate) as createddate,date_format(min(cat1.createddate) ,'%d-%b-%Y-%T') as createddate_format " +
                " FROM candidate_jobsassigned cj inner join candidates c on cj.candidateid=c.candidateid " +
                " inner join candidates_jobs cat on cat.candidateid=cj.candidateid and cat.jobid=cj.jobid and cj.deleted=0  " +
                " join candidates_jobs cat1 on cat1.candidateid=cj.candidateid and cat1.jobid=cj.jobid and cj.deleted=0 " +
                " left join candidates_jobsrating r on r.candidateid=cj.candidateid and r.jobid=cj.jobid and r.userid=?userid " +
                " left join candidates_notes nt on nt.candidateid=c.candidateid and nt.auto=0 " +
                " where cj.jobid=?jobid and cj.deleted=0 " + categoryId + " and c.candidateid in (select candidateid from candidates_jobs where jobid=?jobid and category in(1,3,4)) " +
                " group by c.candidateid,c.title ,c.first ,c.last,c.middle,coalesce(r.rating,0)  ";

            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("jobid", jobid), new MySqlParameter("userid", GPSession.UserId));

            DataTable dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public static MySqlDataReader getCandidateJobs(int jobid, int candidateid)
        {
            string sql = "SELECT c.candidateid as candidateid,c.title ,c.first ,c.last,c.middle,date_format(cj.createdDate,'%d-%b-%Y') as createdDateFormat,date_format(cj.createdDate,'%d-%b-%Y-%T') as createdDateTimeFormat,cj.createdDate,cj.category,cj.searchid,u1.username,j.referenceno  " +
                " ,cs.label,cs.keyword,cs.description ,u.username as searchusername " +
                " FROM jobdetail j inner join  candidates_jobs cj on cj.jobid=j.jobdetailid inner join candidates c on cj.candidateid=c.candidateid left join users u1 on cj.userid=u1.userid" +
                " left join candidate_Search cs on cj.searchid=cs.candidate_Searchid  left join users u on cs.userid=u.userid" +
                "  where cj.jobid=?jobid and cj.candidateid=?candidateid and ifnull(cj.deleted,0) = 0;";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("jobid", jobid), new MySqlParameter("candidateid", candidateid));

            return reader;
        }

        public static MySqlDataReader getCandidateJobs(int jobid, int candidateid, string searchid, string category)
        {
            string sql = "SELECT c.candidateid as candidateid,c.title ,c.first ,c.last,c.middle,date_format(cj.createdDate,'%d-%b-%Y') as createdDateFormat,date_format(cj.createdDate,'%d-%b-%Y-%T') as createdDateTimeFormat,cj.createdDate,cj.category,cj.searchid,u1.username,j.referenceno  " +
                " ,cs.label,cs.keyword,cs.description ,u.username as searchusername " +
                " FROM jobdetail j inner join  candidates_jobs cj on cj.jobid=j.jobdetailid inner join candidates c on cj.candidateid=c.candidateid left join users u1 on cj.userid=u1.userid" +
                " left join candidate_Search cs on cj.searchid=cs.candidate_Searchid  left join users u on cs.userid=u.userid" +
                "  where cj.jobid=?jobid and deleted = 0 and cj.candidateid=?candidateid " + searchid + category + ";";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("jobid", jobid), new MySqlParameter("candidateid", candidateid));

            return reader;
        }

        public static DataTable getCandidateJobsById(uint id)
        {
            string sql = "SELECT candidates_jobsid,notes,statusiconid FROM globalpanda.candidates_jobs  where candidates_jobsid=?id";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("id", id));
            DataTable dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public static DataTable getCandidateJobsByJobId(int jobid, string sortexpression)
        {
            string sql = "SELECT c.candidateid as candidateid,c.title as title,c.first as first,c.last last,c.middle,cj.candidates_jobsid,ic.imagepath,cj.notes,date_format(cj.createdDate,'%d-%b-%Y') as createdDate FROM candidates_jobs cj inner join candidates c on cj.candidateid=c.candidateid " +
                " left outer join status_icon ic on cj.statusiconid=ic.status_iconid where cj.jobid=?jobid order by " + sortexpression + ";";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("jobid", jobid));
            DataTable dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public static DataTable getCandidateJobsByCategory(int jobid, int categoryId, string sortexpression)
        {
            string sql = "SELECT c.candidateid as candidateid,c.title ,c.first ,c.last,c.middle,cj.candidates_jobsid,ic.imagepath,cj.notes,date_format(cj.createdDate,'%d-%b-%Y') as createdDate,coalesce( rating,0) as rating " +
                " FROM globalpanda.candidates_jobs cj inner join candidates c on cj.candidateid=c.candidateid " +
                " left outer join status_icon ic on cj.statusiconid=ic.status_iconid where cj.jobid=?jobid and (cj.category=?category or ?category=0) order by " + sortexpression + ";";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("jobid", jobid), new MySqlParameter("category", categoryId));
            DataTable dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public static DataTable getCandidateJobsByCanddiateID(int canddiateId, string sortexpression)
        {
            string sql = "SELECT cj.candidateid,j.jobdetailid as jobdetailid,j.title,j.referenceNo as referenceNo,date_format(cj.createdDate,'%d-%b-%Y') as createdDate_Formate, cj.createdDate,cj.candidates_jobsid,ic.imagepath,cj.notes FROM globalpanda.candidates_jobs cj inner join jobDetail j on cj.jobid=j.jobDetailId " +
                " left outer join status_icon ic on cj.statusiconid=ic.status_iconid where cj.candidateid=?canddiateId order by " + sortexpression + ";";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("canddiateId", canddiateId));
            DataTable dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public static DataTable getCandidateAssignedJobsByCanddiateID(int canddiateId, string sortexpression)
        {
            string sql = "SELECT cj.candidateid,j.jobdetailid as jobdetailid,j.title,j.referenceNo as referenceNo,cj.jobid,ifnull(cs.userid,0) as userid " +
                " ,(select count(candidates_jobsid) from candidates_jobs where jobid=cj.jobid and candidateid=cj.candidateid and category=3 and deleted=0) as manualExist " +
                " ,(select count(candidates_jobsid) from candidates_jobs where jobid=cj.jobid and candidateid=cj.candidateid and (userid=?userId or ?userid=0) and category=3) as manualUserExist " +
                " FROM candidate_jobsassigned cj inner join jobDetail j on cj.jobid=j.jobDetailId  inner join candidates_jobs cs on cj.candidateid = cs.candidateid and ifnull(cs.deleted,0) = 0" +
                "  where cj.candidateid=?canddiateId and cj.deleted=0 and cs.deleted = 0 group by cj.candidateid,j.jobdetailid ,j.title,j.referenceNo ,cj.jobid order by " + sortexpression + ";";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("canddiateId", canddiateId), new MySqlParameter("userid", GPSession.UserRoleId == 1 ? 0 : GPSession.UserId));
            DataTable dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public static DataTable getCandidateJobsByJobId(int jobid)
        {
            string sql = "SELECT c.candidateid,c.title,c.first,c.last,c.middle,cj.candidates_jobsid,ic.imagepath,cj.notes FROM globalpanda.candidates_jobs cj inner join candidates c on cj.candidateid=c.candidateid " +
                " left outer join status_icon ic on cj.statusiconid=ic.status_iconid where cj.jobid=?jobid;";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("jobid", jobid));
            DataTable dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public static DataTable getCandidateJobsByCanddiateID(int canddiateId)
        {
            string sql = "SELECT cj.candidateid,j.jobdetailid,j.title,j.referenceNo,date_format(cj.createdDate,'%d-%b-%Y') as createdDate,cj.candidates_jobsid,ic.imagepath,cj.notes FROM globalpanda.candidates_jobs cj inner join jobDetail j on cj.jobid=j.jobDetailId " +
                " left outer join status_icon ic on cj.statusiconid=ic.status_iconid where cj.candidateid=?canddiateId;";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("canddiateId", canddiateId));
            DataTable dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public static DataTable getCandidateSearchFrequency(int canddiateSearchId)
        {
            string sql = "Select candidatesearch_frequencyid,frequency from candidatesearch_frequency where candidate_searchid=?candidateSearchId";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("candidateSearchId", canddiateSearchId));
            DataTable dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public static void insertCandidateSearchFrequency(int candidateSearchFrequencyId, int candidateSearchId, int frequency)
        {
            string sql = string.Empty;

            if (candidateSearchFrequencyId == 0)
            {
                sql = "insert into candidatesearch_frequency(candidate_searchid,frequency,createdDate) values(?candidatesearchid,?frequency,?createddate)";
                DAO.ExecuteNonQuery(sql, new MySqlParameter("candidatesearchid", candidateSearchId), new MySqlParameter("frequency", frequency), new MySqlParameter("createddate", DateTime.Now.Date));
            }
            else
            {
                sql = "update candidatesearch_frequency set frequency=?frequency where candidatesearch_frequencyid=?candidatesearchfrequencyid";
                DAO.ExecuteNonQuery(sql, new MySqlParameter("frequency", frequency), new MySqlParameter("candidatesearchfrequencyid", candidateSearchFrequencyId));
            }
        }

        public static void insertCandidateSearchFrequency(int candidateSearchId, int frequency)
        {
            string sql = string.Empty;
            sql = "insert into candidatesearch_frequency(candidate_searchid,frequency,createdDate) values(?candidatesearchid,?frequency,?createddate)";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidatesearchid", candidateSearchId), new MySqlParameter("frequency", frequency), new MySqlParameter("createddate", DateTime.Now.Date));
        }

        public static void updateCandidateSearchFrequency(int candidateSearchId, int frequency)
        {
            string sql = string.Empty;
            sql = "update candidatesearch_frequency set frequency=?frequency where candidate_searchid=?candidatesearchid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidatesearchid", candidateSearchId), new MySqlParameter("frequency", frequency));
        }

        public static bool existCandidateSearchFrequency(int canddiateSearchId)
        {
            bool exist = false;
            string sql = "Select candidatesearch_frequencyid,frequency from candidatesearch_frequency where candidate_searchid=?candidateSearchId";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("candidateSearchId", canddiateSearchId));
            if (reader.HasRows)
                exist = true;
            reader.Close();
            reader.Dispose();
            return exist;
        }

        public static int getJobDetailId(string jobRefNo)
        {
            int jobid = 0;
            string sql = "select jobDetailId from jobdetail where referenceNo = ?referenceno";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("referenceno", jobRefNo));
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    jobid = Convert.ToInt32(reader["jobDetailId"]);
                }
            }
            else
            {
                jobid = 0;
            }

            reader.Close();
            reader.Dispose();

            return jobid;
        }

        public static string getJobTitle(int jobid)
        {
            string title = string.Empty;
            string sql = "select title from jobdetail where jobdetailid = ?jobid";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("jobid", jobid));
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    title = DAO.getString(reader, "title");
                }
            }

            reader.Close();
            reader.Dispose();

            return title;
        }

        public static void deleteRating(int candidateid, int jobid)
        {
            string sql = "delete from candidates_jobsrating where candidateid=?candidateid and jobid=?jobid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("jobid", jobid));
        }

        public static void updateRating(int candidateid, int jobid, int? rating)
        {
            string sql;
            sql = "select rating from candidates_jobsrating where candidateid=?candidateid and jobid=?jobid and userid=?userid";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("jobid", jobid), new MySqlParameter("userid", GPSession.UserId));
            if (reader.HasRows)
                sql = "update candidates_jobsrating set rating=?rating where candidateid=?candidateid and jobid=?jobid and userid=?userid";
            else
                sql = "insert into candidates_jobsrating (candidateid,jobid,userid,rating) values (?candidateid,?jobid,?userid,?rating)";

            reader.Close();
            reader.Dispose();

            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("jobid", jobid), new MySqlParameter("rating", rating), new MySqlParameter("userid", GPSession.UserId));
        }

        public static void insertCandidateSearchExecuteHistory(int searchId)
        {
            string sql = "insert into candidatesearch_execute (candidate_searchid,userid,executeDate) values (?searchid,?userid,?executeDate) ";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("searchid", searchId), new MySqlParameter("userid", GPSession.UserId), new MySqlParameter("executeDate", DateTime.UtcNow));
        }

        public static MySqlDataReader getCandidateSearchExecute(int searchid)
        {
            string sql = "select candidate_searchid,userid,executedate from candidatesearch_execute where candidate_searchid=?searchid order by executedate desc limit 1 ";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("searchid", searchid));

            return reader;
        }

        public static string getCandidateSearchExecuteTime(uint userId, int jobId)
        {
            string executeDate = string.Empty;
            string sql = "select date_format(max(executedate),'%d-%b-%Y-%T') as executeDate from candidatesearch_execute ce join candidatesearch_jobs cj on ce.candidate_Searchid=cj.candidate_Searchid " +
                " where userid=?userid and jobdetailid=?jobid order by executedate desc limit 1 ";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("userid", userId), new MySqlParameter("jobid", jobId));
            if (reader.HasRows)
            {
                reader.Read();
                executeDate = DAO.getString(reader, "executeDate");
            }
            reader.Close();
            reader.Dispose();
            return executeDate;
        }
    }
}