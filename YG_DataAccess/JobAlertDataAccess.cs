using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace YG_DataAccess
{
    public class JobAlertDataAccess
    {
        public static int existCandidate(string email, ref string candidateGUId, ref int emailId)
        {
            int candidateId = 0;
            string sql = "select ce.candidateid,c.candidateguid,ce.emailId from candidates c inner join candidates_emails ce on ce.candidateid=c.candidateid inner join emails e on ce.emailid=e.emailid  where e.email like concat_ws(?email,'%','%') ";
            MySqlDataReader reader = DataAccess.ExecuteReader(sql, new MySqlParameter("email", email));
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    candidateId = Convert.ToInt32(DataAccess.getInt(reader, "candidateid"));
                    candidateGUId = DataAccess.getString(reader, "candidateguid");
                    emailId = Convert.ToInt32(DataAccess.getInt(reader, "emailId"));
                }
            }
            reader.Close();
            reader.Dispose();
            if (candidateId > 0)
            {
                if (string.IsNullOrEmpty(candidateGUId))
                {
                    sql = "update candidates set candidateguid=?candidateguid where candidateid=?candidateid";
                    candidateGUId = Guid.NewGuid().ToString();
                    DataAccess.ExecuteNonQuery(sql, new MySqlParameter("candidateguid", candidateGUId), new MySqlParameter("candidateid", candidateId));
                }
            }
            return candidateId;
        }

        public static int existJobAlert(int candidateId)
        {
            int alertID = 0;
            string sql = "select * from job_alert where candidateId=?candidateId";
            MySqlDataReader reader = DataAccess.ExecuteReader(sql, new MySqlParameter("candidateId", candidateId));
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    alertID = Convert.ToInt32(DataAccess.getInt(reader, "job_alertId"));
                }
            }
            return alertID;
        }

        public static bool IsNoJobalertset(int candidateid)
        {
            string sql = "select nojobalerts from candidates where candidateid=?candidateid";
            bool? exist = false;
            MySqlDataReader dr = DataAccess.ExecuteReader(sql, new MySqlParameter("candidateid", candidateid));
            if (dr.HasRows)
            {
                dr.Read();
                exist = DataAccess.getBool(dr, "nojobalerts");
            }
            dr.Close();
            dr.Dispose();
            if (exist != null)
                return Convert.ToBoolean(exist);
            else
                return false;
        }

        public static int insertEmail(string email)
        {
            string sql = "insert into emails (email) values (?email); select last_insert_id()";
            int emailid = Convert.ToInt32(DataAccess.ExecuteScalar(sql, new MySqlParameter("email", email)));
            return emailid;
        }

        public static int addEmail(int candidateid, string email, uint emailtypeid, bool defaultemail)
        {
            int emailid = insertEmail(email);
            string sql = "insert into candidates_emails (candidateid, emailid, emailtypeid, defaultemail) values (?candidateid, ?emailid, ?emailtypeid, ?defaultemail)";
            DataAccess.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("emailid", emailid), new MySqlParameter("emailtypeid", emailtypeid), new MySqlParameter("defaultemail", defaultemail));
            return emailid;
        }

        public static uint insertPhoneNumber(string number, string countrycode)
        {
            string sql = "insert into phonenumbers (number,countrycode) values (?number,?countrycode); select last_insert_id()";
            uint phonenumberid = Convert.ToUInt32(DataAccess.ExecuteScalar(sql, new MySqlParameter("number", number), new MySqlParameter("countrycode", countrycode)));
            return phonenumberid;
        }

        public static void addPhoneNumber(int candidateid, string number, uint phonenumbertypeid, string countrycode)
        {
            uint phonenumberid = insertPhoneNumber(number, countrycode);
            string sql = "insert into candidates_phonenumbers (candidateid, phonenumberid, phonenumbertypeid) values (?candidateid, ?phonenumberid, ?phonenumbertypeid)";
            DataAccess.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("phonenumberid", phonenumberid), new MySqlParameter("phonenumbertypeid", phonenumbertypeid));
        }

        public static int insertCandidate(uint tenantid, string title, string first, string nickname, string gender, string email, string phoneNumber, string phoneCode, ref string candidateguid, ref int emailId, string middlename = "")
        {
            string sql = "insert into candidates (tenantid, title, first, last,  gender,candidateguid,middle) values (?tenantid, ?title, ?first,  ?last,  ?gender,?candidateguid,?middle); select last_insert_id()";
            candidateguid = Guid.NewGuid().ToString();
            int candidateid = Convert.ToInt32(DataAccess.ExecuteScalar(sql, new MySqlParameter("tenantid", tenantid), new MySqlParameter("title", title), new MySqlParameter("first", first), new MySqlParameter("last", nickname),
                new MySqlParameter("gender", gender), new MySqlParameter("candidateguid", candidateguid), new MySqlParameter("middle", middlename)));

            emailId = addEmail(candidateid, email, 4, false);
            addPhoneNumber(candidateid, phoneNumber, 8, phoneCode);
            return candidateid;
        }

        public static int insertJobAlert(int cadidateId, int frequencyId, string phoneCode, string phoneNumber, string email, DateTime createdDate, int emailId, bool confirmed)
        {
            int alertId = 0;
            string sql = "insert into job_alert (candidateId,frequencyId,phoneCode,phoneNumber,email,createdDate,emailId,confirmed) values (?candateId,?frequencyID,?phoneCode,?phoneNumber,?email,?createdDate,?emailId,?confirmed); select last_insert_id() ";
            alertId = Convert.ToInt32(DataAccess.ExecuteScalar(sql, new MySqlParameter("candateId", cadidateId), new MySqlParameter("frequencyID", frequencyId), new MySqlParameter("phoneCode", phoneCode),
                new MySqlParameter("phoneNumber", phoneNumber), new MySqlParameter("email", email), new MySqlParameter("createdDate", createdDate), new MySqlParameter("emailId", emailId), new MySqlParameter("confirmed", confirmed)));
            return alertId;
        }

        public static int getCandidateEmailId(int candidateId, string email)
        {
            int emailId = 0;
            string sql = "select e.emailid from emails e inner join candidates_emails ce on ce.emailid=e.emailid where candidateid=?candidateid and e.email=?email";
            MySqlDataReader dr = DataAccess.ExecuteReader(sql, new MySqlParameter("candidateid", candidateId), new MySqlParameter("email", email));
            if (dr.HasRows)
            {
                dr.Read();
                emailId = Convert.ToInt32(DataAccess.getInt(dr, "emailid"));
            }
            dr.Close();
            dr.Dispose();
            return emailId;
        }

        public static void updateJobAlert(int alertId, int frequencyId, string phoneCode, string phoneNumber, string email)
        {
            string sql = "select candidateid from job_alert where job_alertid=?alertid";
            int candidateId = Convert.ToInt32(DataAccess.ExecuteScalar(sql, new MySqlParameter("alertid", alertId)));
            int emailId = getCandidateEmailId(candidateId, email);
            if (emailId == 0)
            {
                emailId = addEmail(candidateId, email, 4, false);
            }
            sql = "update job_alert set frequencyId=?frequencyId,phoneCode=?phoneCode,phoneNumber=?phoneNumber,emailid=?emailid where job_alertId=?jobAlertId ";
            DataAccess.ExecuteNonQuery(sql, new MySqlParameter("jobAlertId", alertId), new MySqlParameter("frequencyID", frequencyId), new MySqlParameter("phoneCode", phoneCode), new MySqlParameter("phoneNumber", phoneNumber),
                new MySqlParameter("emailid", emailId));
        }

        public static void inserJobAlertIndustry(int alertId, int candidateId, int subIndustryId)
        {
            string sql = "insert into jobalert_industry (job_alertId,candidateID,isicrev4id) values (?jobAlertId,?candidateId,?industryID);";
            DataAccess.ExecuteNonQuery(sql, new MySqlParameter("jobAlertId", alertId), new MySqlParameter("candidateId", candidateId), new MySqlParameter("industryID", subIndustryId));
        }

        public static void insertJobAlertLocation(int alertId, int candidateId, int locationId, int locationtype)
        {
            string sql = "insert into jobalert_location (job_alertId,candidateID,locationID,locationtype) values (?jobAlertId,?candidateId,?locationID,?locationtype);";
            DataAccess.ExecuteNonQuery(sql, new MySqlParameter("jobAlertId", alertId), new MySqlParameter("candidateId", candidateId), new MySqlParameter("locationID", locationId), new MySqlParameter("locationtype", locationtype));
        }

        public static void insertJobAlertWorkType(int alertId, int candidateId, int workTypeId)
        {
            string sql = "insert into jobalert_workType (job_alertId,candidateID,job_typeId) values (?jobAlertId,?candidateId,?typeId);";
            DataAccess.ExecuteNonQuery(sql, new MySqlParameter("jobAlertId", alertId), new MySqlParameter("candidateId", candidateId), new MySqlParameter("typeID", workTypeId));
        }

        public static void insertJobAlertOccupation(int alertId, int candidateId, int iscoid)
        {
            string sql = "insert into jobalert_isoc08 (job_alertId,candidateID,isco08id) values (?jobAlertId,?candidateId,?iscoid);";
            DataAccess.ExecuteNonQuery(sql, new MySqlParameter("jobAlertId", alertId), new MySqlParameter("candidateId", candidateId), new MySqlParameter("iscoid", iscoid));
        }

        public static void confirmSubscribtion(int alertId)
        {
            string sql = "update job_alert set confirmed=1 where job_alertId=?alertId";
            DataAccess.ExecuteNonQuery(sql, new MySqlParameter("alertId", alertId));
        }

        public static void deleteJobAlert(int jobAlertId, int candidateid, string candidateGUId, string firstname, string lastname)
        {
            string sql;
            sql = "select candidateid from candidates where candidateid=?candidateid and candidateguid=?guid";
            MySqlDataReader dr = DataAccess.ExecuteReader(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("guid", candidateGUId));
            if (dr.HasRows)
            {
                sql = "delete from jobalert_industry where job_alertId=?jobAlertId";
                DataAccess.ExecuteNonQuery(sql, new MySqlParameter("jobAlertId", jobAlertId));

                sql = "delete from jobalert_location where job_alertId=?jobAlertId";
                DataAccess.ExecuteNonQuery(sql, new MySqlParameter("jobAlertId", jobAlertId));

                sql = "delete from jobalert_workType where job_alertId=?jobAlertId";
                DataAccess.ExecuteNonQuery(sql, new MySqlParameter("jobAlertId", jobAlertId));

                sql = "delete from jobalert_isoc08 where job_alertId=?jobAlertId";
                DataAccess.ExecuteNonQuery(sql, new MySqlParameter("jobAlertId", jobAlertId));

                sql = "delete from job_alert where Job_alertID=?jobAlertId;";
                DataAccess.ExecuteNonQuery(sql, new MySqlParameter("jobAlertId", jobAlertId));

                insertUnsubscribeHistory(candidateid, jobAlertId, firstname, lastname);
            }
            dr.Close();
            dr.Dispose();
        }

        public static void deleteJobAlertRelatedRecord(int jobAlertId)
        {
            string sql;
            sql = "delete from jobalert_industry where job_alertId=?jobAlertId";
            DataAccess.ExecuteNonQuery(sql, new MySqlParameter("jobAlertId", jobAlertId));

            sql = "delete from jobalert_location where job_alertId=?jobAlertId";
            DataAccess.ExecuteNonQuery(sql, new MySqlParameter("jobAlertId", jobAlertId));

            sql = "delete from jobalert_workType where job_alertId=?jobAlertId";
            DataAccess.ExecuteNonQuery(sql, new MySqlParameter("jobAlertId", jobAlertId));

            sql = "delete from jobalert_isoc08 where job_alertId=?jobAlertId";
            DataAccess.ExecuteNonQuery(sql, new MySqlParameter("jobAlertId", jobAlertId));
        }

        public static DataTable getJobAlert(int alertId)
        {
            string sql = "Select ja.job_alertId,ja.candidateId,ja.frequencyId,ja.phonecode,ja.phonenumber,e.email,c.first,c.last,f.frequency,c.candidateguid " +
                " from job_alert ja inner join candidates c on ja.candidateid=c.candidateid inner join alert_frequency f on ja.frequencyid=f.alertfrequencyid join emails e on ja.emailid=e.emailid " +
                " where ja.job_alertId=?alertId  ";

            MySqlDataReader reader = DataAccess.ExecuteReader(sql, new MySqlParameter("alertId", alertId));
            DataTable dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();

            return dt;
        }

        public static DataSet getJobAlert(int alertId, string candidateguid)
        {
            string sql = "Select ja.job_alertId,ja.candidateId,ja.frequencyId,ja.phonecode,ja.phonenumber,e.email,c.first,c.last,f.frequency,ja.emailid " +
                " from job_alert ja inner join candidates c on ja.candidateid=c.candidateid inner join alert_frequency f on ja.frequencyid=f.alertfrequencyid join emails e on ja.emailid=e.emailid " +
                "where ja.job_alertId=?alertId and c.candidateguid=?candidateguid; " +

                //"select jl.candidateid,jl.locationid,lo.name from jobalert_location jl " +
                //"left outer join locations lo on jl.locationid = lo.locationid " +
                //"where jl.job_alertId=?alertId; " +

                " select * from( select concat('0',':1') as locationid,concat('0',':1') as lid ,concat('- Anywhere -') as location from jobalert_location where locationid=0 and job_alertId=?alertId " +
                " union select concat(c.countryid,':1') as locationid,concat(c.countryid,':1') as lid ,concat('>',c.name) as location from jobalert_location jl join countries c on jl.locationid=c.countryid and jl.locationtype=1 where jl.job_alertId=?alertId and jl.locationid!=0 " +
                " union  select concat(c.countryid,',',l.locationid,':2') as locationid,concat(l.locationid,':2') as lid, concat('>>',l.name) as location from countries c join locations l on l.countryid=c.countryid " +
                " join jobalert_location jl on jl.locationid=l.locationid and jl.locationtype=2 where  jl.job_alertId=?alertId union " +
                " select concat(c.countryid,',',l.locationid,',',s.sublocationid,':3') as locationid,concat(s.sublocationid,':3') as lid, concat('>>>',s.sublocation) as location from countries c join locations l on l.countryid=c.countryid  " +
                " join locationsub s on s.locationid=l.locationid join jobalert_location jl on jl.locationid=s.sublocationid and jl.locationtype=3 where   jl.job_alertId=?alertId " +
                " union  select concat(c.countryid,',',l.locationid,',',s.sublocationid,',',ss.subsublocationid,':4') as locationid,concat(ss.subsublocationid,':4') as lid, concat('>>>>',ss.name) as location from countries c join locations l on l.countryid=c.countryid " +
                " join locationsub s on s.locationid=l.locationid  join locationsub_subs ss on ss.sublocationid=s.sublocationid join jobalert_location jl on jl.locationid=ss.subsublocationid and jl.locationtype=4 where jl.job_alertId=?alertId " +
                " union  select concat(0,',',0,',',0,',',0,',',location_groupid,':5') as locationid,concat(location_groupid,':5') as lid, concat('>',groupname) as location from jobalert_location jl join location_group g on g.location_groupid=jl.locationid and jl.locationtype=5 " +
                " where jl.job_alertId=?alertId ) as tbl; " +

                "select jw.job_typeid,jt.type from jobalert_worktype jw " +
                "left outer join jobtype jt on jw.job_typeid = jt.jobtypeid " +
                "where jw.job_alertId = ?alertId; " +

                //"select ja.candidateid,ja.jobindustrysubid,ji.subclassification from " +
                //"jobalert_industry ja left outer join (Select distinct a.jobindustrysubid,concat(b.classification,' - ',a.subclassification) " +
                //"as subclassification from jobindustry b left outer join jobindustrysub a on b.jobindustryid = a.jobindustryid " +
                //"order by b.classification) ji on ja.jobindustrysubid = ji.jobindustrysubid " +
                //"where ja.job_alertId=?alertId order by subclassification;" +

                " select ja.candidateid,coalesce( ja.isicrev4id,-1) as isicrev4id,isc.description,isc.code,isc.level,c1 from jobalert_industry ja left join isicrev4 isc on ja.isicrev4id=isc.isicrev4id " +
                "left join v_isicrev4 v on (v.id1=isc.isicrev4id and isc.level=1) or (v.id2=isc.isicrev4id and isc.level=2) or (v.id3=isc.isicrev4id and isc.level=3) or (v.id4=isc.isicrev4id and isc.level=4) " +
                " where ja.job_alertid=?alertid group by ja.candidateid, ja.isicrev4id,isc.description,isc.code,isc.level,c1; " +

                "select coalesce(jw.isco08id,-1) as isco08id,jt.groupcode,jt.title ,jt.type from jobalert_isoc08 jw " +
                "left outer join isco08 jt on jw.isco08id = jt.isco08id " +
                "where jw.job_alertId = ?alertId; ";

            MySqlDataReader reader = DataAccess.ExecuteReader(sql, new MySqlParameter("alertId", alertId), new MySqlParameter("candidateguid", candidateguid));
            DataSet ds = new DataSet();
            ds.Load(reader, LoadOption.OverwriteChanges, new string[5]);
            reader.Close();
            reader.Dispose();

            return ds;
        }

        public static DataTable getJobalertIndustry(int alertId)
        {
            string sql = " select ja.candidateid,coalesce( ja.isicrev4id,-1) as isicrev4id,isc.description,isc.code,isc.level,c1 from jobalert_industry ja left join isicrev4 isc on ja.isicrev4id=isc.isicrev4id " +
                "left join v_isicrev4 v on (v.id1=isc.isicrev4id and isc.level=1) or (v.id2=isc.isicrev4id and isc.level=2) or (v.id3=isc.isicrev4id and isc.level=3) or (v.id4=isc.isicrev4id and isc.level=4) " +
                " where ja.job_alertid=?alertid group by ja.candidateid, ja.isicrev4id,isc.description,isc.code,isc.level,c1; ";

            MySqlDataReader dr = DataAccess.ExecuteReader(sql, new MySqlParameter("alertid", alertId));
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            dr.Dispose();
            return dt;
        }

        public static void updateCandidateGuid(int candidateId, string candidateGuid)
        {
            string sql = "update candidates set candidateguid=?candidateguid where candidateid=?candidateid";

            DataAccess.ExecuteNonQuery(sql, new MySqlParameter("candidateguid", candidateGuid), new MySqlParameter("candidateid", candidateId));
        }

        public static void insertUnsubscribeHistory(int candidateid, int alertid, string firstname, string lastname)
        {
            int historyId = 0;
            string sql = "insert into history (userId,moduleId,typeId,recordId,modifiedDate,parentid) values (?userId,?moduleId,?typeId,?recordId,?modifiedDate,?parentid);select last_insert_id()";
            MySqlParameter[] param ={
                                        new MySqlParameter("userId",0),
                                        new MySqlParameter("moduleId",60),
                                        new MySqlParameter("typeId",3),
                                        new MySqlParameter("recordId",alertid),
                                        new MySqlParameter("modifiedDate",DateTime.UtcNow),
                                        new MySqlParameter("parentid",candidateid)
                                    };
            historyId = Convert.ToInt32(DataAccess.ExecuteScalar(sql, param));
            sql = "insert into historyDetail (historyId,columnName,oldValue,newValue) values (?historyId,?columnName,?oldValue,?newValue)";
            DataAccess.ExecuteNonQuery(sql, new MySqlParameter("historyId", historyId), new MySqlParameter("columnName", "Unsubscribed"), new MySqlParameter("oldValue", string.Empty), new MySqlParameter("newValue", "Candidate " + candidateid + " unsubscribed from Job Alerts"));
        }

        public static int insertJobalertHistory(int candidateid, int alertid, int type)
        {
            int historyId = 0;
            string sql = "insert into history (userId,moduleId,typeId,recordId,modifiedDate,parentid) values (?userId,?moduleId,?typeId,?recordId,?modifiedDate,?parentid);select last_insert_id()";
            MySqlParameter[] param ={
                                        new MySqlParameter("userId",0),
                                        new MySqlParameter("moduleId",60),
                                        new MySqlParameter("typeId",type),
                                        new MySqlParameter("recordId",alertid),
                                        new MySqlParameter("modifiedDate",DateTime.UtcNow),
                                        new MySqlParameter("parentid",candidateid)
                                    };
            historyId = Convert.ToInt32(DataAccess.ExecuteScalar(sql, param));
            return historyId;
        }

        public static void insertHistoryDetails(int historyId, string columnname, string oldvalue, string newvalue)
        {
            string sql = "insert into historyDetail (historyId,columnName,oldValue,newValue) values (?historyId,?columnName,?oldValue,?newValue)";
            DataAccess.ExecuteNonQuery(sql, new MySqlParameter("historyId", historyId), new MySqlParameter("columnName", columnname), new MySqlParameter("oldValue", oldvalue), new MySqlParameter("newValue", newvalue));
        }
    }
}