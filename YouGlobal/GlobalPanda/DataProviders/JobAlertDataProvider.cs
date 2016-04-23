using GlobalPanda.BusinessInfo;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

/// <summary>
/// Summary description for JobAlertDataProvider
/// </summary>

namespace GlobalPanda.DataProviders
{
    public class JobAlertDataProvider
    {
        public static int insertJobAlert(JobAlertInfo info)
        {
            int alertId = 0;
            string sql = "insert into job_alert (candidateId,frequencyId,phoneCode,phoneNumber,emailid,createdDate,importednewcandidate,importedexistingcandidate,confirmed) values " +
                " (?candateId,?frequencyID,?phoneCode,?phoneNumber,?emailid,?createdDate,?importnew,?importexist,?confirmed); select last_insert_id() ";
            alertId = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("candateId", info.CandidateId), new MySqlParameter("frequencyID", info.FrequencyId), new MySqlParameter("phoneCode", info.PhoneCode),
                new MySqlParameter("phoneNumber", info.PhoneNo), new MySqlParameter("emailid", info.EmailId), new MySqlParameter("createdDate", info.CreatedDate), new MySqlParameter("importnew", info.ImportedNewCandidate),
                new MySqlParameter("importexist", info.ImportedExistingCandidate), new MySqlParameter("confirmed", info.Confirmed)));

            foreach (JobAlertIndustry industry in info.IndustryList)
            {
                sql = "insert into jobalert_industry (job_alertId,candidateID,isicrev4id) values (?jobAlertId,?candidateId,?isicrev4id);";
                DAO.ExecuteNonQuery(sql, new MySqlParameter("jobAlertId", alertId), new MySqlParameter("candidateId", info.CandidateId), new MySqlParameter("isicrev4id", industry.ISICRev4Id));
            }

            foreach (JobAlertLocation location in info.LocationList)
            {
                sql = "insert into jobalert_location (job_alertId,candidateID,locationID,locationtype) values (?jobAlertId,?candidateId,?locationID,?locationtype);";
                DAO.ExecuteNonQuery(sql, new MySqlParameter("jobAlertId", alertId), new MySqlParameter("candidateId", info.CandidateId), new MySqlParameter("locationID", location.LocationId), new MySqlParameter("locationtype", location.LocationType));
            }

            foreach (JobAlertWorkType type in info.WorkTypeList)
            {
                sql = "insert into jobalert_workType (job_alertId,candidateID,job_typeId) values (?jobAlertId,?candidateId,?typeId);";
                DAO.ExecuteNonQuery(sql, new MySqlParameter("jobAlertId", alertId), new MySqlParameter("candidateId", info.CandidateId), new MySqlParameter("typeID", type.WorkTypeId));
            }

            foreach (JobAlertOccupation occupation in info.OccupationList)
            {
                sql = "insert into jobalert_isoc08 (job_alertId,candidateID,isco08id) values (?jobAlertId,?candidateId,?iscoid);";
                DAO.ExecuteNonQuery(sql, new MySqlParameter("jobAlertId", alertId), new MySqlParameter("candidateId", info.CandidateId), new MySqlParameter("iscoid", occupation.ISCO08Id));
            }

            CandidateDataProvider.updateAlertEmail(Convert.ToUInt32(info.CandidateId), info.EmailId, info.CandidateFullName);

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo historyInfo = new HistoryInfo();
            historyInfo.UserId = GPSession.UserId;
            historyInfo.ModuleId = (int)HistoryInfo.Module.Jobalert;
            historyInfo.TypeId = (int)HistoryInfo.ActionType.Delete;
            historyInfo.RecordId = Convert.ToUInt32(alertId);
            historyInfo.ParentRecordId = Convert.ToUInt32(info.CandidateId);
            historyInfo.ModifiedDate = DateTime.Now;
            historyInfo.Details = new List<HistoryDetailInfo>();
            historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "New Subscription", NewValue = "New subscription created for Candidate (" + info.CandidateId + ") to email " + info.Email });
            history.insertHistory(historyInfo);

            return alertId;
        }

        public static void updateJobAlert(JobAlertInfo info)
        {
            MySqlDataReader drAlert = CandidateDataProvider.getJobAlert(Convert.ToUInt32(info.CandidateId));
            DataSet ds = new DataSet();
            ds.Load(drAlert, LoadOption.PreserveChanges, new string[5]);
            drAlert.Close();
            drAlert.Dispose();

            string sql = "update job_alert set frequencyId=?frequencyId,phoneCode=?phoneCode,phoneNumber=?phoneNumber,emailid=?emailid where job_alertId=?jobAlertId";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("jobAlertId", info.JobAlertId), new MySqlParameter("frequencyId", info.FrequencyId), new MySqlParameter("phoneCode", info.PhoneCode), new MySqlParameter("phoneNumber", info.PhoneNo),
                new MySqlParameter("emailid", info.EmailId));

            sql = "delete from jobalert_industry where job_alertId=?jobAlertId";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("jobAlertId", info.JobAlertId));

            sql = "delete from jobalert_location where job_alertId=?jobAlertId";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("jobAlertId", info.JobAlertId));

            sql = "delete from jobalert_workType where job_alertId=?jobAlertId";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("jobAlertId", info.JobAlertId));

            sql = "delete from jobalert_isoc08 where job_alertId=?jobAlertId";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("jobAlertId", info.JobAlertId));

            foreach (JobAlertIndustry industry in info.IndustryList)
            {
                sql = "insert into jobalert_industry (job_alertId,candidateID,ISICRev4Id) values (?jobAlertId,?candidateId,?ISICRev4Id);";
                DAO.ExecuteNonQuery(sql, new MySqlParameter("jobAlertId", info.JobAlertId), new MySqlParameter("candidateId", industry.CandidateId), new MySqlParameter("ISICRev4Id", industry.ISICRev4Id));
            }

            foreach (JobAlertLocation location in info.LocationList)
            {
                sql = "insert into jobalert_location (job_alertId,candidateID,locationID,locationtype) values (?jobAlertId,?candidateId,?locationID,?locationtype);";
                DAO.ExecuteNonQuery(sql, new MySqlParameter("jobAlertId", info.JobAlertId), new MySqlParameter("candidateId", location.CandidateId), new MySqlParameter("locationID", location.LocationId), new MySqlParameter("locationtype", location.LocationType));
            }

            foreach (JobAlertWorkType type in info.WorkTypeList)
            {
                sql = "insert into jobalert_workType (job_alertId,candidateID,job_typeId) values (?jobAlertId,?candidateId,?typeId);";
                DAO.ExecuteNonQuery(sql, new MySqlParameter("jobAlertId", info.JobAlertId), new MySqlParameter("candidateId", type.CandidateId), new MySqlParameter("typeID", type.WorkTypeId));
            }

            foreach (JobAlertOccupation occupation in info.OccupationList)
            {
                sql = "insert into jobalert_isoc08 (job_alertId,candidateID,isco08id) values (?jobAlertId,?candidateId,?iscoid);";
                DAO.ExecuteNonQuery(sql, new MySqlParameter("jobAlertId", info.JobAlertId), new MySqlParameter("candidateId", occupation.CandidateId), new MySqlParameter("iscoid", occupation.ISCO08Id));
            }

            CandidateDataProvider.updateAlertEmail(Convert.ToUInt32(info.CandidateId), info.EmailId, info.CandidateFullName);

            #region History

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo historyInfo = new HistoryInfo();
            historyInfo.UserId = GPSession.UserId;
            historyInfo.ModuleId = (int)HistoryInfo.Module.Jobalert;
            historyInfo.TypeId = (int)HistoryInfo.ActionType.Delete;
            historyInfo.RecordId = Convert.ToUInt32(info.JobAlertId);
            historyInfo.ParentRecordId = Convert.ToUInt32(info.CandidateId);
            historyInfo.ModifiedDate = DateTime.Now;
            historyInfo.Details = new List<HistoryDetailInfo>();

            if (ds.Tables[0].Rows[0]["email"].ToString() != info.Email)
            {
                historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "Email", OldValue = ds.Tables[0].Rows[0]["email"].ToString(), NewValue = info.Email });
            }
            if (ds.Tables[0].Rows[0]["frequencyId"].ToString() != info.FrequencyId.ToString())
            {
                historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "Frequency", OldValue = ds.Tables[0].Rows[0]["frequencyId"].ToString(), NewValue = info.FrequencyId.ToString() });
            }
            DataTable dtIndustry = ds.Tables[3];
            if (dtIndustry.Rows.Count != info.IndustryList.Count)
            {
                string industrylist = string.Empty;
                foreach (DataRow dr in dtIndustry.Rows)
                {
                    industrylist = dr["code"].ToString() + " " + dr["description"].ToString() + ", " + industrylist;
                }

                historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "Industry", OldValue = industrylist, NewValue = info.IndustryNameList });
            }
            else
            {
                foreach (JobAlertIndustry industry in info.IndustryList)
                {
                    DataRow[] rowSel = dtIndustry.Select("isicrev4id=" + industry.ISICRev4Id);
                    if (rowSel.Count() == 0)
                    {
                        string industrylist = string.Empty;
                        foreach (DataRow dr in dtIndustry.Rows)
                        {
                            industrylist = dr["code"].ToString() + " " + dr["description"].ToString() + "<br/> " + industrylist;
                        }
                        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "Industry", OldValue = industrylist, NewValue = info.IndustryNameList });
                        break;
                    }
                }
            }
            DataTable dtOccpation = ds.Tables[4];
            if (dtOccpation.Rows.Count != info.OccupationList.Count)
            {
                string occupationlist = string.Empty;
                foreach (DataRow dr in dtOccpation.Rows)
                {
                    occupationlist = dr["groupcode"].ToString() + " " + dr["title"].ToString() + "<br/> " + occupationlist;
                }
                historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "Occupation", OldValue = occupationlist, NewValue = info.OccupationNameList });
            }
            else
            {
                foreach (JobAlertOccupation occupation in info.OccupationList)
                {
                    DataRow[] rowSel = dtOccpation.Select("isco08id=" + occupation.ISCO08Id);
                    if (rowSel.Count() == 0)
                    {
                        string occupationlist = string.Empty;
                        foreach (DataRow dr in dtOccpation.Rows)
                        {
                            occupationlist = dr["groupcode"].ToString() + " " + dr["title"].ToString() + "<br/> " + occupationlist;
                        }
                        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "Occupation", OldValue = occupationlist, NewValue = info.OccupationNameList });
                        break;
                    }
                }
            }
            DataTable dtLocation = ds.Tables[1];
            if (dtLocation.Rows.Count != info.LocationList.Count)
            {
                string locationList = string.Empty;
                foreach (DataRow dr in dtLocation.Rows)
                {
                    locationList = dr["location"].ToString().Split(',')[0] + "<br/> " + locationList;
                }
                historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "Location", OldValue = locationList, NewValue = info.LocationNameList });
            }
            else
            {
                foreach (JobAlertLocation location in info.LocationList)
                {
                    DataRow[] rowSel = dtLocation.Select("lid='" + location.LocationId + ":" + location.LocationType + "'");
                    if (rowSel.Count() == 0)
                    {
                        string locationList = string.Empty;
                        foreach (DataRow dr in dtLocation.Rows)
                        {
                            locationList = dr["location"].ToString().Split(',')[0] + "<br/> " + locationList;
                        }
                        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "Location", OldValue = locationList, NewValue = info.LocationNameList });
                        break;
                    }
                }
            }
            if (historyInfo.Details.Count > 0)
                history.insertHistory(historyInfo);

            #endregion History
        }

        public static void deleteJobAlert(int jobAlertId, uint candidateid)
        {
            string sql;
            sql = "delete from jobalert_industry where job_alertId=?jobAlertId";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("jobAlertId", jobAlertId));

            sql = "delete from jobalert_location where job_alertId=?jobAlertId";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("jobAlertId", jobAlertId));

            sql = "delete from jobalert_workType where job_alertId=?jobAlertId";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("jobAlertId", jobAlertId));

            sql = "delete from job_alert where Job_alertID=?jobAlertId;";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("jobAlertId", jobAlertId));

            sql = "delete from jobalert_isoc08 where job_alertId=?jobAlertId";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("jobAlertId", jobAlertId));

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo historyInfo = new HistoryInfo();
            historyInfo.UserId = GPSession.UserId;
            historyInfo.ModuleId = (int)HistoryInfo.Module.Jobalert;
            historyInfo.TypeId = (int)HistoryInfo.ActionType.Delete;
            historyInfo.RecordId = Convert.ToUInt32(jobAlertId);
            historyInfo.ParentRecordId = candidateid;
            historyInfo.ModifiedDate = DateTime.Now;
            historyInfo.Details = new List<HistoryDetailInfo>();
            historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "Unsubscribed", NewValue = "Candidate " + candidateid + " was unsubscribed from Job Alerts by " + GPSession.UserName + "." });
            history.insertHistory(historyInfo);
        }

        public static MySqlDataReader listAlertFrequency()
        {
            string sql = "select alertFrequencyId,Frequency from alert_frequency";
            MySqlDataReader dr = DAO.ExecuteReader(sql);
            return dr;
        }

        public static int getAlertFrequencyId(string frequency)
        {
            int id = 0;
            string sql = "select alertFrequencyId from alert_frequency where frequency like concat_ws(?frequency,'%','%')";
            id = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("frequency", frequency)));
            if (id == 0)
            {
                sql = "insert into alert_frequency (frequency) values (?frequency); select last_insert_id() ";
                id = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("frequency", frequency)));
            }

            return id;
        }

        public static string getAlertFrequencyName(int id)
        {
            string sql = "select frequency from alert_frequency where alertFrequencyId=?id";
            string name = DAO.ExecuteScalar(sql, new MySqlParameter("id", id)).ToString();
            return name;
        }

        public static int existJobAlert(int candidateId)
        {
            int alertID = 0;
            string sql = "select * from job_alert where candidateId=?candidateId";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("candidateId", candidateId));
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    alertID = Convert.ToInt32(DAO.getInt(reader, "job_alertId"));
                }
            }
            reader.Close();
            reader.Dispose();
            return alertID;
        }

        public static int getJobAlertId(int candidateId)
        {
            int alertID = 0;
            string sql = "select * from job_alert where candidateId=?candidateId";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("candidateId", candidateId));
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    alertID = Convert.ToInt32(DAO.getInt(reader, "job_alertId"));
                }
            }
            reader.Close();
            reader.Dispose();
            return alertID;
        }

        public static int existCandidate(string email, ref int alertId, ref uint emailId)
        {
            int candidateId = 0;
            string sql = "select ce.candidateid,coalesce(ja.job_alertid) as alertid,ce.emailid from candidates_emails ce inner join emails e on ce.emailid=e.emailid left join job_alert ja on ja.candidateid=ce.candidateid " +
                " where e.email like concat_ws(?email,'%','%') order by ce.candidateid desc";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("email", email));
            if (reader.HasRows)
            {
                //candidateId = Convert.ToInt32(DAO.getInt(reader, "candidateid"));

                while (reader.Read())
                {
                    candidateId = Convert.ToInt32(DAO.getInt(reader, "candidateid"));
                    if (DAO.getString(reader, "alertid") != "0")
                    {
                        alertId = Convert.ToInt32(DAO.getString(reader, "alertid"));
                    }

                    emailId = Convert.ToUInt32(DAO.getString(reader, "emailid"));
                }
            }
            reader.Close();
            reader.Dispose();
            return candidateId;
        }

        public static void insertJobalertInvite(int candidateid, string email)
        {
            uint id = 0;
            string sql = "insert into jobalert_invite ( candidateid,email,senddate) values (?candidateid,?email,?sendDate);select last_insert_id();";
            id = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("email", email), new MySqlParameter("sendDate", DateTime.UtcNow)));

            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.TypeId = (int)HistoryInfo.ActionType.Add;
            info.ModuleId = (int)HistoryInfo.Module.jobalert_invite;
            info.RecordId = id;
            info.ModifiedDate = DateTime.UtcNow;
            info.ParentRecordId = Convert.ToUInt32(candidateid);

            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "Job alert invite", NewValue = "Job Alerts invitation initiated by " + GPSession.UserName + ", sent to " + email + "." });

            HistoryDataProvider history = new HistoryDataProvider();
            history.insertHistory(info);
        }

        public static bool jobalertinviteexist(int candidateid)
        {
            bool _exist = false;
            string sql = "select candidateid from jobalert_invite where candidateid=?candidateid and datediff(?today, senddate)<180";
            int id = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("today", DateTime.UtcNow)));
            if (id > 0)
                _exist = true;

            return _exist;
        }

        public static int CandidateAlertEmailId(uint candidateId)
        {
            int emailId = 0;
            string sql = "select emailid from job_alert where candidateid=?candidateid";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("candidateid", candidateId));
            if (dr.HasRows)
            {
                dr.Read();
                emailId = Convert.ToInt32(DAO.getInt(dr, "emailid"));
            }
            dr.Close();
            dr.Dispose();
            return emailId;
        }

        public static bool existJobAlert(uint candidateId)
        {
            bool exist = false;
            string sql = "select job_alertId from job_alert where candidateid=?candidateid";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("candidateid", candidateId));
            if (dr.HasRows)
                exist = true;
            dr.Close();
            dr.Dispose();
            return exist;
        }

        public static MySqlDataReader searchJobAlert(string keyword, int filter, string order, int status)
        {
            /*string sql = " Select ja.job_alertId,ja.candidateId,ja.frequencyId,ja.phonecode,ja.phonenumber,e.email,c.first,c.last,f.frequency,c.middle,Date_Format(createddate,'%d-%b-%Y-%T') as createddate,importednewcandidate,importedexistingcandidate,ja.confirmed " +
                         " from job_alert ja inner join candidates c on ja.candidateid=c.candidateid inner join alert_frequency f on ja.frequencyid=f.alertfrequencyid join emails e on ja.emailid=e.emailid " +

                         " where (confirmed=?status or ?status=2 or (?status=0 and confirmed is null)) and (((c.candidateid like concat_ws(?keyword,'%','%') or c.first like concat_ws(?keyword,'%','%') or c.last like concat_ws(?keyword,'%','%') or c.middle like concat_ws(?keyword,'%','%') " +
                         " or e.email like concat_ws(?keyword,'%','%')) and (?filter=0 or ?filter=1)) " +
                         "  or ( f.frequency like concat('%',?keyword,'%') and (?filter=0 or ?filter=5) )) " +
                         " group by ja.job_alertId,ja.candidateId,ja.frequencyId,ja.phonecode,ja.phonenumber,e.email,c.first,c.last,f.frequency " +

                         " union " +
                         " Select ja.job_alertId,ja.candidateId,ja.frequencyId,ja.phonecode,ja.phonenumber,e.email,c.first,c.last,f.frequency,c.middle,Date_Format(createddate,'%d-%b-%Y-%T') as createddate,importednewcandidate,importedexistingcandidate,ja.confirmed " +
                         " from job_alert ja inner join candidates c on ja.candidateid=c.candidateid inner join alert_frequency f on ja.frequencyid=f.alertfrequencyid join emails e on ja.emailid=e.emailid " +
                         "  join jobalert_worktype wt on wt.job_alertid=ja.job_alertid left join jobtype t on wt.job_typeid=t.jobtypeid " +

                         " where (confirmed=?status or ?status=2 or (?status=0 and confirmed is null)) and (((c.candidateid like concat_ws(?keyword,'%','%') or c.first like concat_ws(?keyword,'%','%') or c.last like concat_ws(?keyword,'%','%') or c.middle like concat_ws(?keyword,'%','%') " +
                         " or e.email like concat_ws(?keyword,'%','%')) and (?filter=0 or ?filter=1)) " +
                         "  or ( f.frequency like concat('%',?keyword,'%') and (?filter=0 or ?filter=5) ) or ( t.type like concat('%',?keyword,'%') and (?filter=0 or ?filter=6))) " +
                         " group by ja.job_alertId,ja.candidateId,ja.frequencyId,ja.phonecode,ja.phonenumber,e.email,c.first,c.last,f.frequency " +

                         " union " +
                         " Select ja.job_alertId,ja.candidateId,ja.frequencyId,ja.phonecode,ja.phonenumber,e.email,c.first,c.last,f.frequency,c.middle,Date_Format(createddate,'%d-%b-%Y-%T') as createddate,importednewcandidate,importedexistingcandidate,ja.confirmed " +
                         " from job_alert ja inner join candidates c on ja.candidateid=c.candidateid inner join alert_frequency f on ja.frequencyid=f.alertfrequencyid join emails e on ja.emailid=e.emailid " +
                         "  join jobalert_isoc08 jw on ja.job_alertId=jw.job_alertId left join isco08 jt on jw.isco08id = jt.isco08id  " +
                         " where (confirmed=?status or ?status=2 or (?status=0 and confirmed is null)) and (((c.candidateid like concat_ws(?keyword,'%','%') or c.first like concat_ws(?keyword,'%','%') or c.last like concat_ws(?keyword,'%','%') or c.middle like concat_ws(?keyword,'%','%') or e.email like concat_ws(?keyword,'%','%')" +
                         " or c.candidateid like concat_ws(?keyword,'%','%')) and (?filter=0 or ?filter=1)) or ((jt.title like concat('%',?keyword,'%') or jt.groupcode like concat('%',?keyword,'%')) and (?filter=0 or ?filter=2) )" +
                         "  or ( f.frequency like concat('%',?keyword,'%') and (?filter=0 or ?filter=5) ) ) " +
                         " group by ja.job_alertId,ja.candidateId,ja.frequencyId,ja.phonecode,ja.phonenumber,e.email,c.first,c.last,f.frequency " +

                         " union " +
                         " Select ja.job_alertId,ja.candidateId,ja.frequencyId,ja.phonecode,ja.phonenumber,e.email,c.first,c.last,f.frequency,c.middle,Date_Format(createddate,'%d-%b-%Y-%T') as createddate,importednewcandidate,importedexistingcandidate,ja.confirmed " +
                         " from job_alert ja inner join candidates c on ja.candidateid=c.candidateid inner join alert_frequency f on ja.frequencyid=f.alertfrequencyid join emails e on ja.emailid=e.emailid " +
                          " join jobalert_industry ji on ji.job_alertId=ja.job_alertId left join isicrev4 isc on ji.isicrev4id=isc.isicrev4id  " +
                         " where (confirmed=?status or ?status=2 or (?status=0 and confirmed is null)) and (((c.candidateid like concat_ws(?keyword,'%','%') or c.first like concat_ws(?keyword,'%','%') or c.last like concat_ws(?keyword,'%','%') or c.middle like concat_ws(?keyword,'%','%') or e.email like concat_ws(?keyword,'%','%')" +
                         " or c.candidateid like concat_ws(?keyword,'%','%')) and (?filter=0 or ?filter=1))  or ((isc.description like concat('%',?keyword,'%') or isc.code like concat('%',?keyword,'%') ) and (?filter=0 or ?filter=3)) " +
                         "  or ( f.frequency like concat('%',?keyword,'%') and (?filter=0 or ?filter=5) ) ) " +
                         " group by ja.job_alertId,ja.candidateId,ja.frequencyId,ja.phonecode,ja.phonenumber,e.email,c.first,c.last,f.frequency " +

                         " union select ja.job_alertId,ja.candidateId,ja.frequencyId,ja.phonecode,ja.phonenumber,e.email,c.first,c.last,f.frequency,c.middle,Date_Format(createddate,'%d-%b-%Y-%T') as createddate,importednewcandidate,importedexistingcandidate,ja.confirmed " +
                         " from job_alert ja inner join candidates c on ja.candidateid=c.candidateid inner join alert_frequency f on ja.frequencyid=f.alertfrequencyid join emails e on ja.emailid=e.emailid " +
                         " join v_jobalertLocation vl on vl.job_alertid=ja.job_alertid " +
                         " where (confirmed=?status or ?status=2 or (?status=0 and confirmed is null)) and (((c.candidateid like concat_ws(?keyword,'%','%') or c.first like concat_ws(?keyword,'%','%') or c.last like concat_ws(?keyword,'%','%') or c.middle like concat_ws(?keyword,'%','%') or e.email like concat_ws(?keyword,'%','%')" +
                         " or c.candidateid like concat_ws(?keyword,'%','%')) and (?filter=0 or ?filter=1)) or ((vl.country like concat('%',?keyword,'%') or vl.location like concat('%',?keyword,'%') or vl.sublocation like concat('%',?keyword,'%') or vl.subsublocation like concat('%',?keyword,'%') " +
                         " or vl.country1 like concat('%',?keyword,'%') or vl.location1 like concat('%',?keyword,'%') or vl.sublocation1 like concat('%',?keyword,'%') or vl.groupname like concat('%',?keyword,'%')) and (?filter=0 or ?filter=4) )" +

                         "  or ( f.frequency like concat('%',?keyword,'%') and (?filter=0 or ?filter=5) ))  " +
                         " group by ja.job_alertId,ja.candidateId,ja.frequencyId,ja.phonecode,ja.phonenumber,e.email,c.first,c.last,f.frequency order by " + order;
            */

            string sql = @"
                            drop temporary table if exists isco08matches;
                            create temporary table isco08matches (job_alertid int primary key);
	                        insert into isco08matches (job_alertid)
	                        select	distinct jac.job_alertid
	                        from	isco08 as i8
	                            join	jobalert_isoc08 as jac on jac.isco08id = i8.isco08id
	                        where	(i8.title like @keyword or i8.groupcode like @keyword)
                              and   @filter in (0,2);

                            drop temporary table if exists isicrev4matches;
                            create temporary table isicrev4matches (job_alertid int primary key);
	                        insert into isicrev4matches (job_alertid)
	                        select	distinct jai.job_alertid
	                        from	isicrev4 as i4
	                            join	jobalert_industry as jai on jai.isicrev4id = i4.isicrev4id
	                        where	(i4.description like @keyword or i4.code like @keyword)
                              and   @filter in (0,3);

                            drop temporary table if exists locationmatches;
                            create temporary table locationmatches (job_alertid int primary key);
	                        insert into locationmatches (job_alertid)
	                        select	distinct jal.job_alertid
	                        from	v_mergedlocations as l
	                            join	jobalert_location as jal on jal.locationtype = l.locationtype and jal.locationid = l.locationid
	                        where	l.name like @keyword
                              and   @filter in (0,4);

                            drop temporary table if exists typematches;
                            create temporary table typematches (job_alertid int primary key);
	                        insert into typematches (job_alertid)
	                        select	distinct jwt.job_alertid
	                        from	jobtype as jt
	                            join	jobalert_worktype as jwt on jwt.job_typeid = jt.jobtypeid
	                        where	jt.type like @keyword
                              and   @filter in (0,6);

                            Select	ja.job_alertId,
		                            ja.candidateId,
		                            ja.frequencyId,
		                            ja.phonecode,
		                            ja.phonenumber,
		                            e.email,
		                            c.first,
		                            c.last,
		                            f.frequency,
		                            c.middle,
		                            ja.createddate,
		                            ja.importednewcandidate,
		                            ja.importedexistingcandidate,
		                            ja.confirmed
                            from	job_alert as ja
                                join	candidates as c on c.candidateid = ja.candidateid
                                join	emails as e on e.emailid = ja.emailid
                                left join	alert_frequency f on f.alertfrequencyid = ja.frequencyid
                            where	(@status = 2 or ja.confirmed = @status or (@status = 0 and ja.confirmed is null))
                                and	(
			                            exists	(	select	c.candidateid
						                            from	candidates as c
						                            where	@filter in (0,1)
						                                and	c.candidateid = ja.candidateid
						                                and	(	c.candidateid like @keyword
						                                or		c.first like @keyword
						                                or		c.last like @keyword
						                                or		c.middle like @keyword
								                            )
					                            )

                                or		exists	(	select	ja.emailid
						                            from	emails as e
						                            where	@filter in (0,1)
						                                and	e.emailid = ja.emailid
						                                and	e.email like @keyword
					                            )

                                or		exists	(	select	i8.job_alertid
						                            from	isco08matches as i8
						                            where	i8.job_alertid = ja.job_alertid
					                            )

                                or		exists	(	select	i4.job_alertid
						                            from	isicrev4matches as i4
						                            where	i4.job_alertId = ja.job_alertId
					                            )

                                or		exists	(	select	jl.job_alertid
						                            from	locationmatches as jl
						                            where	jl.job_alertid = ja.job_alertid
					                            )

                                or		exists	(	select	af.alertfrequencyid
						                            from	alert_frequency as af
						                            where	@filter in (0,5)
						                                and	af.alertfrequencyid = ja.frequencyid
						                                and	af.frequency like @keyword
					                            )

                                or		exists	(	select	jt.job_alertid
						                            from	typematches as jt
						                            where	jt.job_alertid = ja.job_alertid
					                            )
		                            )
                            order by " + order;
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("keyword", string.Format("%{0}%", keyword)), new MySqlParameter("filter", filter), new MySqlParameter("status", status));
            return reader;
        }

        //public static MySqlDataReader searchJobAlert(string keyword, int filter, string order,int status)
        //{
        //    string sql = " Select ja.job_alertId,ja.candidateId,ja.frequencyId,ja.phonecode,ja.phonenumber,e.email,c.first,c.last,f.frequency,c.middle,Date_Format(createddate,'%d-%b-%Y-%T') as createddate,importednewcandidate,importedexistingcandidate " +
        //                 " from job_alert ja inner join candidates c on ja.candidateid=c.candidateid inner join alert_frequency f on ja.frequencyid=f.alertfrequencyid join emails e on ja.emailid=e.emailid " +
        //                 " left join jobalert_worktype wt on wt.job_alertid=ja.job_alertid left join jobtype t on wt.job_typeid=t.jobtypeid " +
        //                 " left join jobalert_isoc08 jw on ja.job_alertId=jw.job_alertId left join isco08 jt on jw.isco08id = jt.isco08id left join jobalert_industry ji on ji.job_alertId=ja.job_alertId " +
        //                 " left join isicrev4 isc on ji.isicrev4id=isc.isicrev4id  " +
        //        //" left join jobalert_location jl on jl.job_alertid=ja.job_alertid left join  countries cu on (cu.countryid=jl.locationid and jl.locationtype=1) " +
        //        //" left join locations l on (l.locationid=jl.locationid and jl.locationtype=2) left join locationsub ls on (ls.sublocationid=jl.locationid and jl.locationtype=3)  " +
        //        //" left join locationsub_subs lss on (lss.subsublocationid=jl.locationid and jl.locationtype=4) left join countries cu1 on l.countryid=cu1.countryid " +
        //        //" left join locations l1 on ls.locationid=l1.locationid left join locationsub ls1 on ls1.sublocationid=lss.sublocationid " +
        //                 " where (confirmed=?status or ?status=2 or (?status=0 and confirmed is null)) and (((c.candidateid like concat_ws(?keyword,'%','%') or c.first like concat_ws(?keyword,'%','%') or c.last like concat_ws(?keyword,'%','%') or c.middle like concat_ws(?keyword,'%','%') or e.email like concat_ws(?keyword,'%','%')" +
        //                 " or c.candidateid like concat_ws(?keyword,'%','%')) and (?filter=0 or ?filter=1)) " +
        //        //" or ((cu.name like concat('%',?keyword,'%') or l.name like concat('%',?keyword,'%') or ls.sublocation like concat('%',?keyword,'%') or lss.name like concat('%',?keyword,'%') " +
        //        //" or cu1.name like concat('%',?keyword,'%') or l1.name like concat('%',?keyword,'%') or ls1.sublocation like concat('%',?keyword,'%')) and (?filter=0 or ?filter=4) )" +
        //                 " or ((isc.description like concat('%',?keyword,'%') or isc.code like concat('%',?keyword,'%') ) and (?filter=0 or ?filter=3)) or ((jt.title like concat('%',?keyword,'%') or jt.groupcode like concat('%',?keyword,'%')) and (?filter=0 or ?filter=2) )" +
        //                 "  or ( f.frequency like concat('%',?keyword,'%') and (?filter=0 or ?filter=5) ) or ( t.type like concat('%',?keyword,'%') and (?filter=0 or ?filter=6))) " +
        //                 " group by ja.job_alertId,ja.candidateId,ja.frequencyId,ja.phonecode,ja.phonenumber,e.email,c.first,c.last,f.frequency " +

        //                 " union select ja.job_alertId,ja.candidateId,ja.frequencyId,ja.phonecode,ja.phonenumber,e.email,c.first,c.last,f.frequency,c.middle,Date_Format(createddate,'%d-%b-%Y-%T') as createddate,importednewcandidate,importedexistingcandidate " +
        //                 " from job_alert ja inner join candidates c on ja.candidateid=c.candidateid inner join alert_frequency f on ja.frequencyid=f.alertfrequencyid join emails e on ja.emailid=e.emailid " +
        //                 " left join v_jobalertLocation vl on vl.job_alertid=ja.job_alertid " +
        //                 " where (confirmed=?status or ?status=2 or (?status=0 and confirmed is null)) and (((c.candidateid like concat_ws(?keyword,'%','%') or c.first like concat_ws(?keyword,'%','%') or c.last like concat_ws(?keyword,'%','%') or c.middle like concat_ws(?keyword,'%','%') or e.email like concat_ws(?keyword,'%','%')" +
        //                 " or c.candidateid like concat_ws(?keyword,'%','%')) and (?filter=0 or ?filter=1)) or ((vl.country like concat('%',?keyword,'%') or vl.location like concat('%',?keyword,'%') or vl.sublocation like concat('%',?keyword,'%') or vl.subsublocation like concat('%',?keyword,'%') " +
        //                 " or vl.country1 like concat('%',?keyword,'%') or vl.location1 like concat('%',?keyword,'%') or vl.sublocation1 like concat('%',?keyword,'%')) and (?filter=0 or ?filter=4) )" +
        //        //" or ((isc.description like concat('%',?keyword,'%') or isc.code like concat('%',?keyword,'%') ) and (?filter=0 or ?filter=3)) or ((jt.title like concat('%',?keyword,'%') or jt.groupcode like concat('%',?keyword,'%')) and (?filter=0 or ?filter=2) )" +
        //                 "  or ( f.frequency like concat('%',?keyword,'%') and (?filter=0 or ?filter=5) ))  " +
        //                 " group by ja.job_alertId,ja.candidateId,ja.frequencyId,ja.phonecode,ja.phonenumber,e.email,c.first,c.last,f.frequency order by " + order;

        //    MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword), new MySqlParameter("filter", filter), new MySqlParameter("status", status));
        //    return reader;
        //}

        //public static MySqlDataReader searchJobAlert(string keyword, int filter, string order,int status)
        //{
        //    string sql = " select job_alertId,c.candidateId,frequencyId,phonecode,phonenumber,e.email,c.first,c.last,frequency,middle,Date_Format(createddate,'%d-%b-%Y-%T') as createddate, importednewcandidate,importedexistingcandidate "+
        //                 " from( Select ja.job_alertId,ja.candidateId,ja.emailid,ja.frequencyId,ja.phonecode,ja.phonenumber,f.frequency,Date_Format(createddate,'%d-%b-%Y-%T') as createddate, importednewcandidate,importedexistingcandidate "+
        //                 " from job_alert ja  inner join alert_frequency f on ja.frequencyid=f.alertfrequencyid left join jobalert_worktype wt on wt.job_alertid=ja.job_alertid left join jobtype t on wt.job_typeid=t.jobtypeid  "+
        //                 " left join jobalert_isoc08 jw on ja.job_alertId=jw.job_alertId left join isco08 jt on jw.isco08id = jt.isco08id  left join jobalert_industry ji on ji.job_alertId=ja.job_alertId  "+
        //                 " left join isicrev4 isc on ji.isicrev4id=isc.isicrev4id  "+
        //                  " where (confirmed=?status or ?status=2 or (?status=0 and confirmed is null)) and (((isc.description like concat('%',?keyword,'%') or isc.code like concat('%',?keyword,'%') ) and (?filter=0 or ?filter=3)) " +
        //                  " or ((jt.title like concat('%',?keyword,'%') or jt.groupcode like concat('%',?keyword,'%')) and (?filter=0 or ?filter=2) )" +
        //                 "  or ( f.frequency like concat('%',?keyword,'%') and (?filter=0 or ?filter=5) ) or ( t.type like concat('%',?keyword,'%') and (?filter=0 or ?filter=6))) " +
        //                 " group by ja.job_alertId,ja.candidateId,ja.frequencyId,ja.phonecode,ja.phonenumber,f.frequency " +

        //                 " union select ja.job_alertId,ja.candidateId,ja.emailid,ja.frequencyId,ja.phonecode,ja.phonenumber,f.frequency,Date_Format(createddate,'%d-%b-%Y-%T') as createddate, importednewcandidate,importedexistingcandidate "+
        //                 " from job_alert ja  inner join alert_frequency f on ja.frequencyid=f.alertfrequencyid left join v_jobalertLocation vl on vl.job_alertid=ja.job_alertid "+
        //                 " where (confirmed=?status or ?status=2  or (?status=0 and confirmed is null)) and (((vl.country like concat('%',?keyword,'%') or vl.location like concat('%',?keyword,'%') or vl.sublocation like concat('%',?keyword,'%') " +
        //                 " or vl.subsublocation like concat('%',?keyword,'%')  or vl.country1 like concat('%',?keyword,'%') or vl.location1 like concat('%',?keyword,'%') or vl.sublocation1 like concat('%',?keyword,'%')) and (?filter=0 or ?filter=4) )" +
        //                 "  or ( f.frequency like concat('%',?keyword,'%') and (?filter=0 or ?filter=5)))  " +
        //                 " group by ja.job_alertId,ja.candidateId,ja.frequencyId,ja.phonecode,ja.phonenumber,f.frequency  ) as tbl join candidates c on tbl.candidateid=c.candidateid join emails e on tbl.emailid=e.emailid "+
        //                 " where ((c.candidateid like concat_ws(?keyword,'%','%') or c.first like concat_ws(?keyword,'%','%') or c.last like concat_ws(?keyword,'%','%') or c.middle like concat_ws(?keyword,'%','%') or e.email like concat_ws(?keyword,'%','%')" +
        //                 " or c.candidateid like concat_ws(?keyword,'%','%')) and (?filter=0 or ?filter=1)) order by " + order;

        //    MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword), new MySqlParameter("filter", filter),new MySqlParameter("status",status));
        //    return reader;
        //}

        public static MySqlDataReader getJobalertISCO(int alertId)
        {
            string sql = "select coalesce(jw.isco08id,-1) as isco08id,jt.groupcode,jt.title ,jt.type,concat(case when jt.type = 1 then 'Major Group ' Else case when " +
                         " jt.type=2 then 'Sub-Major Group ' Else  case when jt.type=3 then 'Minor Group ' else 'Unit Group ' End End End ,jt.groupcode,' ',jt.title) as fulltitle from jobalert_isoc08 jw join isco08 jt on jw.isco08id = jt.isco08id " +
                "where jw.job_alertId = ?alertId union select '' as isco08id,'' asgroupcode,'' as title ,'' as type,'- Any -' as fulltitle from jobalert_isoc08 jw where jw.job_alertId = ?alertId and jw.isco08id =0 ; ";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("alertId", alertId));
            return reader;
        }

        public static MySqlDataReader getJobalertISICRev(int alertId)
        {
            string sql = " select ja.candidateid,coalesce( ja.isicrev4id,-1) as isicrev4id,isc.description,isc.code,isc.level,concat(case when isc.level = 1 then 'Sections ' Else case when isc.level=2 then 'Division ' Else " +
            " case when isc.level=3 then 'Group ' else 'Class ' End End End ,isc.code,' ',isc.description) as fulltitle from jobalert_industry ja join isicrev4 isc on ja.isicrev4id=isc.isicrev4id " +
                //" join v_isicrev4 v on (v.id1=isc.isicrev4id and isc.level=1) or (v.id2=isc.isicrev4id and isc.level=2) or (v.id3=isc.isicrev4id and isc.level=3) or (v.id4=isc.isicrev4id and isc.level=4) " +
                " where ja.job_alertid=?alertid union select ja.candidateid,coalesce( ja.isicrev4id,-1) as isicrev4id,'' as description,'' as code,'' as level,'- Any -' as fulltitle from jobalert_industry ja where ja.job_alertid=?alertid and ja.isicrev4id=0; ";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("alertId", alertId));
            return reader;
        }

        public static MySqlDataReader getJobalertLocations(int alertId)
        {
            string sql = "select concat('Anywhere') as location from jobalert_location jl  where jl.locationid=0 and jl.locationtype=1 and jl.job_alertid=?alertId  " +
                        " union select c.name as location from countries c join jobalert_location jl on c.countryid=jl.locationid and jl.locationtype=1 and jl.locationid!=0 where jl.job_alertid=?alertId  " +
                        " union select concat(c.name,' > ',l.name) as location  from countries c inner join locations l on l.countryid=c.countryid join jobalert_location jl on l.locationid=jl.locationid and jl.locationtype=2 " +
                        " where  jl.job_alertid=?alertId  union select concat(c.name,' > ',l.name,' > ',s.sublocation) as location  from countries c inner join locations l on l.countryid=c.countryid " +
                        " inner join locationsub s on s.locationid=l.locationid join jobalert_location jl on s.sublocationid=jl.locationid and jl.locationtype=3 where  jl.job_alertid=?alertId  " +
                        " union select concat(c.name,' > ',l.name,' > ',s.sublocation,' > ',ss.name) as location from countries c inner join locations l on l.countryid=c.countryid  inner join locationsub s on s.locationid=l.locationid " +
                        " inner join locationsub_subs ss on ss.sublocationid=s.sublocationid join jobalert_location jl on ss.subsublocationid=jl.locationid and jl.locationtype=4 where jl.job_alertid=?alertId " +
                        " union select groupname as location from location_group g join jobalert_location jl on g.location_groupid = jl.locationid and jl.locationtype=5 where jl.job_alertid=?alertId";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("alertId", alertId));
            return reader;
        }

        public static MySqlDataReader getJobalertWorktype(int alertId)
        {
            string sql = " select jw.job_typeid,jt.type from jobalert_worktype jw " +
                "left outer join jobtype jt on jw.job_typeid = jt.jobtypeid " +
                "where jw.job_alertId = ?alertId; ";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("alertId", alertId));
            return reader;
        }

        public static int getJobAlertEmailId(uint candidateId)
        {
            int emailID = 0;
            string sql = "select * from job_alert where candidateId=?candidateId";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("candidateId", candidateId));
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    emailID = Convert.ToInt32(DAO.getInt(reader, "emailid"));
                }
            }
            reader.Close();
            reader.Dispose();
            return emailID;
        }

        public static void updateAlertConfirm(int alertId, string candidateid, string emailaddress)
        {
            string sql = "update job_alert set confirmed=1 where job_alertId=?alertId";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("alertId", alertId));

            string message = "An unconfirmed Job Alerts request by Candidate " + candidateid + " was approved internally by " + GPSession.UserName + ", with Job Alerts being sent to " + emailaddress + ".";
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.TypeId = (int)HistoryInfo.ActionType.Add;
            info.ModuleId = (int)HistoryInfo.Module.Jobalert;
            info.RecordId = Convert.ToUInt32(alertId);
            info.ModifiedDate = DateTime.UtcNow;
            info.ParentRecordId = Convert.ToUInt32(alertId);

            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "Job alert Confirm", NewValue = message });

            HistoryDataProvider history = new HistoryDataProvider();
            history.insertHistory(info);
        }
    }
}