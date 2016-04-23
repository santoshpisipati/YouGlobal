using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace YG_DataAccess
{
    public class JobApplyDataAccess
    {
        public static DataTable GetCandidatesByEmail(string email)
        {
            string sql = "select ce.candidateid,c.candidateguid,c.first,date_format(c.dob,'%d %b %Y') as dob,ce.emailid from candidates c inner join candidates_emails ce on ce.candidateid=c.candidateid inner join emails e on ce.emailid=e.emailid where e.email =?email ";
            MySqlDataReader reader = DataAccess.ExecuteReader(sql, new MySqlParameter("email", email));
            DataTable dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();

            return dt;
        }

        public static int insertCandidate(uint tenantid, string title, string first, string nickname, string gender, string email, string phoneNumber, string phoneCode, DateTime? dob, ref string candidateguid)
        {
            string sql = "insert into candidates (tenantid, title, first, last,  gender, dob,nodob,dobformat, candidateguid) values (?tenantid, ?title, ?first,  ?last,  ?gender,?dob,?nodob,?dobformat,?candidateguid); select last_insert_id()";
            candidateguid = Guid.NewGuid().ToString();
            int candidateid = Convert.ToInt32(DataAccess.ExecuteScalar(sql, new MySqlParameter("tenantid", tenantid), new MySqlParameter("title", title), new MySqlParameter("first", first), new MySqlParameter("last", nickname),
                new MySqlParameter("gender", gender), new MySqlParameter("dob", dob), new MySqlParameter("nodob", false), new MySqlParameter("dobformat", "DMY"), new MySqlParameter("candidateguid", candidateguid)));

            addEmail(candidateid, email, 4, true);
            addPhoneNumber(candidateid, phoneNumber, 8, phoneCode, "");
            return candidateid;
        }

        public static int insertCandidate(uint tenantid, string title, string first, string lastname, string middlename, string nickname, string gender, string martial, uint emailtype, string email, DateTime? dob, ref string candidateguid, List<string[]> numbers, List<int> nationalities, string skype)
        {
            string sql = "insert into candidates (tenantid, title, first, last, middle, nickname, maritalstatus, gender, dob,nodob,dobformat, candidateguid) values (?tenantid, ?title,?first, ?last,?middle,?nickname,?marital,?gender,?dob,?nodob,?dobformat,?candidateguid); select last_insert_id()";
            candidateguid = Guid.NewGuid().ToString();
            int candidateid = Convert.ToInt32(DataAccess.ExecuteScalar(sql, new MySqlParameter("tenantid", tenantid), new MySqlParameter("title", title), new MySqlParameter("first", first), new MySqlParameter("last", lastname),
                new MySqlParameter("middle", middlename), new MySqlParameter("nickname", nickname), new MySqlParameter("marital", martial),
                new MySqlParameter("gender", gender), new MySqlParameter("dob", dob), new MySqlParameter("nodob", false), new MySqlParameter("dobformat", "DMY"), new MySqlParameter("candidateguid", candidateguid)));

            addEmail(candidateid, email, emailtype, true);
            //candidates_nationalities
            DataTable dtNation = new DataTable();
            DataRow[] selNation;
            if (nationalities != null && nationalities.Count > 0)
            {
                sql = "select countryid from candidates_nationalities where candidateid=?candidateid";
                MySqlDataReader drNation = DataAccess.ExecuteReader(sql, new MySqlParameter("candidateid", candidateid));
                dtNation.Load(drNation);
                drNation.Close();
                drNation.Dispose();

                foreach (uint countryid in nationalities)
                {
                    selNation = dtNation.Select("countryid=" + countryid);
                    if (selNation.Count() == 0)
                    {
                        sql = "insert into candidates_nationalities (candidateid, countryid) values (?candidateid,?countryid)";
                        DataAccess.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("countryid", countryid));
                    }
                }
            }
            //numbers
            if (numbers != null && numbers.Count > 0)
            {
                DataTable dtPhone = new DataTable();
                DataRow[] selPhone;
                MySqlDataReader drPhone = getCandidatePhonenumber(candidateid);
                dtPhone.Load(drPhone);
                drPhone.Close();
                drPhone.Dispose();
                foreach (string[] number in numbers)
                {
                    selPhone = dtPhone.Select("number='" + number[3] + "'");
                    if (selPhone.Count() == 0)
                    {
                        string countrycode = number[0];
                        string phonenumber = number[3];
                        uint phonenumbertypeid = Convert.ToUInt32(number[2]);
                        string areacode = number[1];
                        addPhoneNumber(candidateid, phonenumber, phonenumbertypeid, countrycode, areacode);
                    }
                }
            }
            if (!string.IsNullOrEmpty(skype))
            {
                addOtherContact(candidateid, skype, 2);
            }
            return candidateid;
        }

        public static void updateCandidate(int candidateId, string lastname, string phoneNumber, string phoneCode)
        {
            string sql = "update candidates set last=?last where candidateid=?candidateid";
            DataAccess.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateId), new MySqlParameter("last", lastname));
            if (!existPhoneNumber(candidateId, phoneNumber))
                addPhoneNumber(candidateId, phoneNumber, 8, phoneCode, "");
        }

        public static void updateCandidate(int candidateId, string title, string first, string lastname, string middlename, string nickname, string gender, string martial, string email, int emailid, uint emailtype, DateTime? dob, ref string candidateguid, List<string[]> numbers, List<int> nationalities, string skype)
        {
            string sql = "update candidates set title=?title,last=?last,middle=?middle,nickname=?nickname,gender=?gender,maritalstatus=?marital where candidateid=?candidateid";
            DataAccess.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateId), new MySqlParameter("title", title), new MySqlParameter("last", lastname), new MySqlParameter("middle", middlename), new MySqlParameter("nickname", nickname),
                new MySqlParameter("gender", gender), new MySqlParameter("marital", martial));

            updateEmailtype(candidateId, emailid, emailtype);

            DataTable dtNation = new DataTable();
            DataRow[] selNation;
            if (nationalities.Count > 0)
            {
                sql = "select candidateid,countryid from candidates_nationalities where candidateid=?candidateid";
                MySqlDataReader drNation = DataAccess.ExecuteReader(sql, new MySqlParameter("candidateid", candidateId));
                dtNation.Load(drNation);
                drNation.Close();
                drNation.Dispose();
            }
            foreach (uint countryid in nationalities)
            {
                selNation = dtNation.Select("countryid=" + countryid);
                if (selNation.Count() == 0)
                {
                    sql = "insert into candidates_nationalities (candidateid, countryid) values (?candidateid,?countryid)";
                    DataAccess.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateId), new MySqlParameter("countryid", countryid));
                }
            }

            //numbers
            DataTable dtPhone = new DataTable();
            DataRow[] selPhone;
            MySqlDataReader drPhone = getCandidatePhonenumber(candidateId);
            dtPhone.Load(drPhone);
            drPhone.Close();
            drPhone.Dispose();
            foreach (string[] number in numbers)
            {
                selPhone = dtPhone.Select("number='" + number[3] + "'");
                if (selPhone.Count() == 0)
                {
                    string countrycode = number[0];
                    string phonenumber = number[3];
                    uint phonenumbertypeid = Convert.ToUInt32(number[2]);
                    string areacode = number[1];
                    addPhoneNumber(candidateId, phonenumber, phonenumbertypeid, countrycode, areacode);
                }
            }
            if (!string.IsNullOrEmpty(skype))
            {
                addOtherContact(candidateId, skype, 2);
            }
        }

        public static uint insertEmail(string email)
        {
            string sql = "insert into emails (email) values (?email); select last_insert_id()";
            uint emailid = Convert.ToUInt32(DataAccess.ExecuteScalar(sql, new MySqlParameter("email", email)));
            return emailid;
        }

        public static void addEmail(int candidateid, string email, uint emailtypeid, bool defaultemail)
        {
            uint emailid = insertEmail(email);
            string sql = "insert into candidates_emails (candidateid, emailid, emailtypeid, defaultemail) values (?candidateid, ?emailid, ?emailtypeid, ?defaultemail)";
            DataAccess.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("emailid", emailid), new MySqlParameter("emailtypeid", emailtypeid), new MySqlParameter("defaultemail", defaultemail));
        }

        public static void updateEmailtype(int candidateid, int emailid, uint emailtype)
        {
            string sql = "update candidates_emails set emailtypeid=?emailtype where candidateid=?candidateid and emailid=?emailid";
            DataAccess.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("emailtype", emailtype), new MySqlParameter("emailid", emailid));
        }

        public static uint insertPhoneNumber(string number, string countrycode, string areacode)
        {
            string sql = "insert into phonenumbers (number,countrycode,areacode) values (?number,?countrycode,?areacode); select last_insert_id()";
            uint phonenumberid = Convert.ToUInt32(DataAccess.ExecuteScalar(sql, new MySqlParameter("number", number), new MySqlParameter("countrycode", countrycode), new MySqlParameter("areacode", areacode)));
            return phonenumberid;
        }

        public static MySqlDataReader getCandidatePhonenumber(int candidateId)
        {
            string sql = "select pn.phonenumberid, cpn.phonenumbertypeid, pn.number from candidates_phonenumbers as cpn " +
                  "join phonenumbers as pn on pn.phonenumberid = cpn.phonenumberid where cpn.candidateid = ?candidateid order by pn.phonenumberid; ";
            MySqlDataReader dr = DataAccess.ExecuteReader(sql, new MySqlParameter("candidateid", candidateId));
            return dr;
        }

        public static void addPhoneNumber(int candidateid, string number, uint phonenumbertypeid, string countrycode, string areacode)
        {
            uint phonenumberid = insertPhoneNumber(number, countrycode, areacode);
            string sql = "insert into candidates_phonenumbers (candidateid, phonenumberid, phonenumbertypeid) values (?candidateid, ?phonenumberid, ?phonenumbertypeid)";
            DataAccess.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("phonenumberid", phonenumberid), new MySqlParameter("phonenumbertypeid", phonenumbertypeid));
        }

        public static bool existPhoneNumber(int candidateid, string number)
        {
            bool exist = false;
            string sql = "select cp.phonenumberid from candidates_phonenumbers cp inner join phonenumbers p on cp.phonenumberid=p.phonenumberid  where number=?number and candidateid=?candidateid";
            MySqlDataReader dr = DataAccess.ExecuteReader(sql, new MySqlParameter("number", number), new MySqlParameter("candidateid", candidateid));
            if (dr.HasRows)
                exist = true;
            dr.Close();
            dr.Dispose();
            return exist;
        }

        public static bool existCandidateJobsAssigned(int candidateid, int jobid)
        {
            bool exist;
            string sql = "select candidateid from candidate_jobsassigned where candidateid=?candidateid and jobid=?jobid";
            MySqlDataReader reader = DataAccess.ExecuteReader(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("jobid", jobid));
            if (reader.HasRows)
                exist = true;
            else
                exist = false;
            reader.Close();
            reader.Dispose();

            return exist;
        }

        public static void insertCandidateJobsAssigned(int candidateid, int jobid)
        {
            string sql = "insert into candidate_jobsassigned (candidateid,jobid) values (?candidateid,?jobid)";
            DataAccess.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("jobid", jobid));
        }

        public static void insertCandidateJobs(int candidateid, int jobid, int category, int? searchid)
        {
            uint id;
            string sql = "insert into candidates_jobs (candidateid,jobid,category,createddate,searchid,userid) values (?candidateid,?jobid,?category,?createddate,?searchid,?userid);select last_insert_id()";
            id = Convert.ToUInt32(DataAccess.ExecuteScalar(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("jobid", jobid), new MySqlParameter("category", category), new MySqlParameter("createddate", DateTime.UtcNow),
                new MySqlParameter("searchid", searchid), new MySqlParameter("userid", 1)));
        }

        public static void CandidateCombine(int masterId, int duplicateId)
        {
            if (!candidatecombineexist(masterId, duplicateId))
                DataAccess.ExecuteNonQuery("cc_CreateCombineCandidate", System.Data.CommandType.StoredProcedure, new MySqlParameter("?in_MasterCandidateId", masterId), new MySqlParameter("?in_DuplicateCandidateId", duplicateId), new MySqlParameter("?in_UserId", 1));
        }

        public static bool candidatecombineexist(int masterid, int duplicateid)
        {
            bool _exist = false;
            string sql = "select CombineCandidateId from combinecandidates where MasterCandidateId = ?masterid and DuplicateCandidateId = ?duplicateid";
            int id = Convert.ToInt32(DataAccess.ExecuteScalar(sql, new MySqlParameter("masterid", masterid), new MySqlParameter("duplicateid", duplicateid)));
            if (id > 0)
                _exist = true;

            return _exist;
        }

        public static void insertJobalertInvite(int candidateid, string email)
        {
            uint id = 0;
            string sql = "insert into jobalert_invite ( candidateid,email,senddate) values (?candidateid,?email,?sendDate);select last_insert_id();";
            id = Convert.ToUInt32(DataAccess.ExecuteScalar(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("email", email), new MySqlParameter("sendDate", DateTime.UtcNow)));
        }

        public static bool jobalertinviteexist(int candidateid)
        {
            bool _exist = false;
            string sql = "select candidateid from jobalert_invite where candidateid=?candidateid and datediff(?today, senddate)<180";
            int id = Convert.ToInt32(DataAccess.ExecuteScalar(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("today", DateTime.UtcNow)));
            if (id > 0)
                _exist = true;

            return _exist;
        }

        public static DataTable getCandidateFiles(int candidateId)
        {
            DataTable dt = new DataTable();
            string sql = "select cf.fileid, f.name, cf.uploaded,date_format(cf.uploaded,'%d-%b-%Y') as uploaded_format, ft.name as filetype, f.size,ft.filetypeid,filename,f.deleted " +
                "from candidates_files as cf " +
                "join files as f on f.fileid = cf.fileid " +
                "join filetypes as ft on ft.filetypeid = cf.filetypeid " +
                "where cf.candidateid = ?candidateid " +
                "order by cf.uploaded desc, cf.fileid";
            MySqlDataReader reader = DataAccess.ExecuteReader(sql, new MySqlParameter("candidateid", candidateId));
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public static DataTable getConsultantEmailById(int Id)
        {
            DataTable dt = new DataTable();
            string sql = "select e.emailid, ce.emailtypeid, e.email, ce.defaultemail,u.utcoffset from consultants c inner join consultants_emails as ce on ce.consultantid=c.consultantid join emails as e on e.emailid = ce.emailid inner join users u on c.userid=u.userid " +
                "where ce.consultantid = ?consultantid order by ce.emailid; ";

            MySqlDataReader reader = DataAccess.ExecuteReader(sql, new MySqlParameter("consultantid", Id));
            dt.Load(reader);
            reader.Close();
            reader.Dispose();

            return dt;
        }

        public static void addWorkHistory(int candidateid, int industryid, int positionid)
        {
            if (!ExistWorkHistory(candidateid, industryid, positionid))
            {
                string sql = "insert into candidates_workhistories (candidateid, industryid, positionid) " +
                             " values (?candidateid, ?industryid, ?positionid);select last_insert_id()";
                DataAccess.ExecuteScalar(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("industryid", industryid), new MySqlParameter("positionid", positionid));
            }
        }

        public static void addWorkHistory(int candidateid, DateTime? from, string from_display, DateTime? to, string to_display, int? locationid, int? locationtype, int? industryid, int? positionid, int employerid)
        {
            if (!ExistWorkHistory(candidateid, from, from_display, to, to_display, locationid, locationtype, industryid, positionid, employerid))
            {
                string sql = "insert into candidates_workhistories (candidateid, `from`, from_display, `to`, to_display, locationid, industryid, positionid,locationtype,employerid) " +
                            " values (?candidateid, ?from, ?from_display, ?to, ?to_display,  ?locationid, ?industryid, ?positionid, ?locationtype,?employerid);select last_insert_id()";

                DataAccess.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("from", from), new MySqlParameter("from_display", from_display), new MySqlParameter("to", to),
                    new MySqlParameter("to_display", to_display), new MySqlParameter("locationid", locationid), new MySqlParameter("industryid", industryid), new MySqlParameter("positionid", positionid),
                     new MySqlParameter("locationtype", locationtype), new MySqlParameter("employerid", employerid));
            }
        }

        public static bool ExistWorkHistory(int candidateid, int industryid, int positionid)
        {
            bool exist = false;
            string sql = "Select candidateid from candidates_workhistories where candidateid=?candidateid and industryid=?industryid and positionid=?positionid";
            MySqlDataReader dr = DataAccess.ExecuteReader(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("industryid", industryid), new MySqlParameter("positionid", positionid));
            if (dr.HasRows)
                exist = true;
            dr.Close();
            dr.Dispose();
            return exist;
        }

        public static bool ExistWorkHistory(int candidateid, DateTime? from, string from_display, DateTime? to, string to_display, int? locationid, int? locationtype, int? industryid, int? positionid, int? employerid)
        {
            bool exist = false;
            string sql = "Select candidateid from candidates_workhistories where  candidateid=?candidateid and (`from` = ?frm or ?frm is null)  and " +
                " from_display = ?from_display and " +
                " (`to` = ?to or ?to is null) and " +
                " to_display = ?to_display and " +
                " (locationid = ?locationid or ?locationid is null) and " +
                " (industryid = ?industryid or ?industryid is null) and " +
                " (positionid = ?positionid or ?positionid is null) and " +
                " (locationtype=?locationtype or ?locationtype is null) and " +
                " employerid=?employerid ";
            MySqlDataReader dr = DataAccess.ExecuteReader(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("frm", from), new MySqlParameter("from_display", from_display),
                new MySqlParameter("to", to), new MySqlParameter("to_display", to_display), new MySqlParameter("locationid", locationid), new MySqlParameter("industryid", industryid), new MySqlParameter("positionid", positionid),
                 new MySqlParameter("locationtype", locationtype), new MySqlParameter("employerid", employerid));
            if (dr.HasRows)
                exist = true;
            dr.Close();
            dr.Dispose();
            return exist;
        }

        public static void addCandidateSalary(int candidateId, int type, int fromSalary, int toSalary, int frequency, string currency, DateTime salarydate, int sourcetype, int sourceid)
        {
            string sql = "select candidateid from candidates_salary where candidateid=?candidateid and salarytype=?salarytype";
            MySqlDataReader dr = DataAccess.ExecuteReader(sql, new MySqlParameter("candidateid", candidateId), new MySqlParameter("salarytype", type));
            if (dr.HasRows)
                sql = "update candidates_salary set fromamount=?fromamount,toamount=?toamount,frequency=?frequency,currency=?currency,salarydate=?salarydate,sourcetype=?sourcetype,sourceid=?sourceid where candidateid=?candidateid and salarytype=?salarytype";
            else
                sql = "insert into candidates_salary (candidateid,fromamount,toamount,frequency,currency,salarytype,salarydate,sourcetype,sourceid) values (?candidateid,?fromamount,?toamount,?frequency,?currency,?salarytype,?salarydate,?sourcetype,?sourceid)";
            DataAccess.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateId), new MySqlParameter("fromamount", fromSalary), new MySqlParameter("toamount", toSalary), new MySqlParameter("frequency", frequency),
                new MySqlParameter("currency", currency), new MySqlParameter("salarytype", type), new MySqlParameter("salarydate", salarydate), new MySqlParameter("sourcetype", sourcetype), new MySqlParameter("sourceid", sourceid));
            dr.Close();
            dr.Dispose();
        }

        public static void addOtherContact(int candidateid, string details, int alternativecontactid)
        {
            string sql = "select candidateid from candidates_othercontacts where candidateid=?candidateid and alternativecontactid=?alternativecontactid and details=?details";
            MySqlDataReader dr = DataAccess.ExecuteReader(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("alternativecontactid", alternativecontactid), new MySqlParameter("details", details));
            if (!dr.HasRows)
            {
                sql = "insert into candidates_othercontacts (candidateid, alternativecontactid, details) values (?candidateid, ?alternativecontactid, ?details)";
                DataAccess.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("alternativecontactid", alternativecontactid), new MySqlParameter("details", details));
            }
            dr.Close();
            dr.Dispose();
        }

        public static int insertSaveLaterObject(string email, byte[] buffer, int jobid, ref string secuirtyid)
        {
            int id = 0;
            secuirtyid = Guid.NewGuid().ToString();
            string sql = "select temp_jobapplyid,email,securityid from temp_jobapply where email=?email and jobid=?jobid";
            MySqlDataReader dr = DataAccess.ExecuteReader(sql, new MySqlParameter("email", email), new MySqlParameter("jobid", jobid));
            if (dr.HasRows)
            {
                dr.Read();
                secuirtyid = DataAccess.getString(dr, "securityid");
                id = Convert.ToInt32(DataAccess.getString(dr, "temp_jobapplyid"));
                sql = "update temp_jobapply set createddate=?createddate,applyobject=?object where email=?email and jobid=?jobid";
                DataAccess.ExecuteNonQuery(sql, new MySqlParameter("email", email), new MySqlParameter("createddate", DateTime.UtcNow), new MySqlParameter("object", buffer), new MySqlParameter("jobid", jobid));
            }
            else
            {
                sql = "insert into temp_jobapply ( email,createddate,applyobject,jobid,securityid) values (?email,?createddate,?object,?jobid,?securityid); select last_insert_id()";
                id = Convert.ToInt32(DataAccess.ExecuteScalar(sql, new MySqlParameter("email", email), new MySqlParameter("createddate", DateTime.UtcNow), new MySqlParameter("object", buffer),
                    new MySqlParameter("jobid", jobid), new MySqlParameter("securityid", secuirtyid)));
            }
            dr.Close(); dr.Dispose();
            return id;
        }

        public static byte[] getSaveLaterObject(string securityid, int id)
        {
            int bufferSize = 50000;// 22006; // Size of the BLOB buffer.
            byte[] outbyte = new byte[bufferSize]; // The BLOB byte[] buffer to be filled by GetBytes.
            long retval; // The bytes returned from GetBytes.
            long startIndex = 0; // The starting position in the BLOB output.
            string sql = "select email,applyobject,jobid from temp_jobapply where securityid=?securityid and temp_jobapplyid=?id";
            MySqlDataReader dr = DataAccess.ExecuteReader(sql, new MySqlParameter("securityid", securityid), new MySqlParameter("id", id));
            if (dr.HasRows)
            {
                dr.Read();
                retval = (long)dr.GetBytes(1, startIndex, outbyte, 0, bufferSize);
                while (retval == bufferSize)
                {
                    // Reposition the start index to the end of the last buffer and fill the buffer.
                    startIndex += bufferSize;
                    retval = dr.GetBytes(1, startIndex, outbyte, 0, bufferSize);
                }
            }
            dr.Close(); dr.Dispose();
            return outbyte;
        }

        public static void deleteSaveObject(string email, int jobId)
        {
            string sql = "delete from temp_jobapply where email=?email and jobid=?jobid";
            DataAccess.ExecuteNonQuery(sql, new MySqlParameter("email", email), new MySqlParameter("jobid", jobId));
        }

        public static void addCurrentlocation(int candidateid, int locationid, int locationtype)
        {
            string sql = "insert into candidates_currentlocations (candidateid,locationid,locationtype) values (?candidateid,?locationid,?locationtype)";
            DataAccess.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("locationid", locationid), new MySqlParameter("locationtype", locationtype));
            sql = "update candidates set currentlocationdate=?dt where candidateid=?candidateid";
            DataAccess.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("dt", DateTime.UtcNow));
        }

        public static void removeCurrentlocations(int candidateId)
        {
            string sql = "delete from candidates_currentlocations where candidateid=?candidateid";
            DataAccess.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateId));
        }

        public static DataTable getCurrentlocations(int candidateId)
        {
            string sql = "select cl.candidateid,cl.locationtype,cl.locationid, concat('Anywhere') as location,concat('0',',','0,0,0,0') as locationids  from candidates_currentlocations cl  where cl.locationid=0 and cl.locationtype=1 and cl.candidateid=?candidateid  " +
      " union select cl.candidateid,cl.locationtype,cl.locationid,concat(c.name) as location,concat(CAST(c.countryid as char),',','0,0,0,0') as locationids from countries c join candidates_currentlocations cl on c.countryid=cl.locationid and cl.locationtype=1 and cl.locationid!=0 where cl.candidateid=?candidateid  " +
       " union select cl.candidateid,cl.locationtype,cl.locationid,concat(c.name,' > ',l.name) as location,concat(CAST(c.countryid as char),',',cast(l.locationid as char),',', '0,0,0') as locationids  from countries c inner join locations l on l.countryid=c.countryid " +
        " join candidates_currentlocations cl on l.locationid=cl.locationid and cl.locationtype=2 where  cl.candidateid=?candidateid " +
         " union select cl.candidateid,cl.locationtype,cl.locationid,concat(c.name,' > ',l.name,' > ',s.sublocation) as location,concat(CAST(c.countryid as char),',',cast(l.locationid as char),',', cast(s.sublocationid as char),',','0,0') as locationids  from countries c inner join locations l on l.countryid=c.countryid " +
         " inner join locationsub s on s.locationid=l.locationid join candidates_currentlocations cl on s.sublocationid=cl.locationid and cl.locationtype=3 where cl.candidateid=?candidateid  " +
         " union select cl.candidateid,cl.locationtype,cl.locationid,concat(c.name,' > ',l.name,' > ',s.sublocation,' > ',ss.name) as location,concat(CAST(c.countryid as char),',',cast(l.locationid as char),',', cast(s.sublocationid as char),',',cast(ss.subsublocationid as char),',0' ) as locationids from countries c inner " +
          " join locations l on l.countryid=c.countryid  inner join locationsub s on s.locationid=l.locationid " +
          " inner join locationsub_subs ss on ss.sublocationid=s.sublocationid join candidates_currentlocations cl on ss.subsublocationid=cl.locationid and cl.locationtype=4 where cl.candidateid=?candidateid " +
            " union  select cl.candidateid,cl.locationtype,cl.locationid,concat(groupname) as location,concat('0',',','0,0,0',',',location_groupid) as locationids from candidates_currentlocations cl join location_group g on g.location_groupid=cl.locationid and cl.locationtype=5 " +
             " where  cl.candidateid=?candidateid; ";

            MySqlDataReader dr = DataAccess.ExecuteReader(sql, new MySqlParameter("candidateid", candidateId));
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            dr.Dispose();
            return dt;
        }

        public static bool checkGroupExist(DataTable dt, int grouupId)
        {
            bool exist = false;
            if (dt.Rows.Count == 0)
                return exist;
            string sWhere = "";
            DataRow[] rowSel;
            string sql = " select  vl.countryid,vl.name,vl.locationid, vl.locationname as locationname,vl.sublocationid,vl.sublocation,vl.subsublocationid,vl.name as subsublocation,gd.locationtype " +
                          " from location_group g inner join location_groupdetails gd on g.location_groupid=gd.location_groupid  join v_locations vl on (vl.countryid=gd.locationid and gd.locationtype=1) " +
                          " or (vl.locationid=gd.locationid and gd.locationtype=2) or (vl.sublocationid=gd.locationid and gd.locationtype=3) or (vl.subsublocationid=gd.locationid and gd.locationtype=4) " +
                          " where g.location_groupid=?groupId group by gd.locationid,gd.locationtype ";

            MySqlDataReader dr = DataAccess.ExecuteReader(sql, new MySqlParameter("groupId", grouupId));
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    string cId = DataAccess.getString(dr, "countryid");
                    string dId = DataAccess.getString(dr, "locationid");
                    string sId = DataAccess.getString(dr, "sublocationid");
                    string ssId = DataAccess.getString(dr, "subsublocationid");

                    switch (DataAccess.getString(dr, "locationtype"))
                    {
                        case "1":
                            sWhere = "countryId=" + cId;
                            break;

                        case "2":
                            sWhere = "adminidivisionId=" + dId + " and adminidivisionId <> 0  or ( countryId=" + cId + " and (adminidivisionId=" + dId + " or adminidivisionId = 0) and locationtype in(1,3,4))";
                            break;

                        case "3":
                            sWhere = "sublocationId=" + sId + " and sublocationId <> 0 or ( (adminidivisionId=" + dId + " or adminidivisionId = 0)  and countryId=" + cId + " and locationtype in(1,2,4))";
                            break;

                        case "4":
                            sWhere = "subsublocationId=" + ssId + " and subsublocationId <> 0 or ((sublocationId=" + sId + " or sublocationId = 0) and (adminidivisionId=" + dId + " or adminidivisionId = 0)  and countryId=" + cId + " and locationtype in(1,2,3))";
                            break;
                    }

                    rowSel = dt.Select(sWhere);

                    if (rowSel.Count() > 0)
                    {
                        //lblLocationMsg.Text = "The location you have tried to add has a subsidiary location as an existing Group member. Please remove the subsidiary location first.";
                        return true;
                    }
                }
            }
            return exist;
        }

        public static bool checkLocationGroupExist(DataTable dt, int groupid, int locationid, int locationtype)
        {
            bool exist = false;
            string sWhere = "";
            DataRow[] rowSel;
            string sql = " select  vl.countryid,vl.name,vl.locationid, vl.locationname as locationname,vl.sublocationid,vl.sublocation,vl.subsublocationid,vl.name as subsublocation,gd.locationtype " +
                          " from location_group g inner join location_groupdetails gd on g.location_groupid=gd.location_groupid  left join v_locations vl on (vl.countryid=gd.locationid and gd.locationtype=1) " +
                          " or (vl.locationid=gd.locationid and gd.locationtype=2) or (vl.sublocationid=gd.locationid and gd.locationtype=3) or (vl.subsublocationid=gd.locationid and gd.locationtype=4) " +
                          " where g.location_groupid=?groupid and gd.locationid=?locationid and gd.locationtype=?locationtype group by gd.locationid,gd.locationtype ";
            MySqlDataReader dr = DataAccess.ExecuteReader(sql, new MySqlParameter("groupid", groupid), new MySqlParameter("locationid", locationid), new MySqlParameter("locationtype", locationtype));
            DataTable dtGroup = new DataTable();
            if (dr.HasRows)
            {
                dtGroup.Load(dr);
                foreach (DataRow drGroup in dtGroup.Rows)
                {
                    string cId = drGroup["countryid"].ToString();
                    string dId = drGroup["locationid"].ToString();
                    string sId = drGroup["sublocationid"].ToString();
                    string ssId = drGroup["subsublocationid"].ToString();

                    switch (drGroup["locationtype"].ToString())
                    {
                        case "1":
                            sWhere = "countryId=" + cId;
                            break;

                        case "2":
                            sWhere = "locationid=" + dId + " and locationid <> 0  or ( countryId=" + cId + " and (locationid=" + dId + " or locationid = 0) and locationtype in(1,3,4))";
                            break;

                        case "3":
                            sWhere = "sublocationId=" + sId + " and sublocationId <> 0 or ( (locationid=" + dId + " or locationid = 0)  and countryId=" + cId + " and locationtype in(1,2,4))";
                            break;

                        case "4":
                            sWhere = "subsublocationId=" + ssId + " and subsublocationId <> 0 or ((sublocationId=" + sId + " or sublocationId = 0) and (locationid=" + dId + " or locationid = 0)  and countryId=" + cId + " and locationtype in(1,2,3))";
                            break;
                    }

                    rowSel = dtGroup.Select(sWhere);

                    if (rowSel.Count() > 0)
                    {
                        //lblLocationMsg.Text = "The location you have tried to add has a subsidiary location as an existing Group member. Please remove the subsidiary location first.";
                        return true;
                    }
                }
            }
            dr.Close();
            dr.Dispose();
            return exist;
        }

        public static void insertUnrecognisedLocationHistory(int candidateid, string firstname, string middlename, string lastname, string locationtext, string nameoffield, string jobrefcode)
        {
            int historyId = 0;
            string newvalue = string.Empty;
            if (String.IsNullOrEmpty(jobrefcode))
                newvalue = "Candidate " + candidateid.ToString() + " (" + firstname + " " + middlename + " " + lastname + ") entered the unrecognised Location \"" + locationtext + "\" in " + nameoffield + " when uploading a CV";
            else
                newvalue = "Candidate " + candidateid.ToString() + " (" + firstname + " " + middlename + " " + lastname + ") entered the unrecognised Location \"" + locationtext + "\" in " + nameoffield + " when applying for " + jobrefcode;

            string sql = "insert into history (userId,moduleId,typeId,recordId,modifiedDate,parentid) values (?userId,?moduleId,?typeId,?recordId,?modifiedDate,?parentid);select last_insert_id()";
            MySqlParameter[] param ={
                                        new MySqlParameter("userId",0),
                                        new MySqlParameter("moduleId",51),
                                        new MySqlParameter("typeId",1),
                                        new MySqlParameter("recordId",candidateid),
                                        new MySqlParameter("modifiedDate",DateTime.UtcNow),
                                        new MySqlParameter("parentid",candidateid)
                                    };
            historyId = Convert.ToInt32(DataAccess.ExecuteScalar(sql, param));
            sql = "insert into historyDetail (historyId,columnName,oldValue,newValue) values (?historyId,?columnName,?oldValue,?newValue)";
            DataAccess.ExecuteNonQuery(sql, new MySqlParameter("historyId", historyId), new MySqlParameter("columnName", "All"), new MySqlParameter("oldValue", string.Empty), new MySqlParameter("newValue", newvalue));
        }
    }
}