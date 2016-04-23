using GlobalPanda.BusinessInfo;
using MySql.Data.MySqlClient;
using System;
using System.Data;

/// <summary>
/// Summary description for EmailTemplateDataProvider
/// </summary>
namespace GlobalPanda.DataProviders
{
    public class EmailTemplateDataProvider
    {
        public static void insertTemplate(EmailTemplateInfo Info)
        {
            string sql = "Insert into email_template(subject,header,body,footer,status,createddate,version,active,userid) Values(?subject,?header,?body,?footer,?status,?createddate,?version,?active,?userid);select last_insert_id()";
            int id = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("subject", Info.Subject), new MySqlParameter("header", Info.Header), new MySqlParameter("body", Info.Body), new MySqlParameter("footer", Info.Footer),
               new MySqlParameter("status", Info.Status), new MySqlParameter("createddate", DateTime.UtcNow), new MySqlParameter("version", Info.Version), new MySqlParameter("active", true), new MySqlParameter("userid", GPSession.UserId)));

            sql = "update email_template set parentid=?id where email_templateid = ?id";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("id", id));

            //insertVersion(id, Info.Version);
        }

        public static void insertNewVersion(EmailTemplateInfo info)
        {
            string sql = "Insert into email_template(subject,header,body,footer,status,createddate,version,parentid,active,userid) Values(?subject,?header,?body,?footer,?status,?createddate,?version,?parentid,?active,?userid);select last_insert_id()";
            int id = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("subject", info.Subject), new MySqlParameter("header", info.Header), new MySqlParameter("body", info.Body), new MySqlParameter("footer", info.Footer),
               new MySqlParameter("status", info.Status), new MySqlParameter("createddate", DateTime.UtcNow), new MySqlParameter("version", info.Version), new MySqlParameter("parentid", info.ParentId),
               new MySqlParameter("active", true), new MySqlParameter("userid", GPSession.UserId)));

            sql = "update email_template set active=0 where email_templateid = ?id";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("id", info.ParentId));

            //sql = "insert into emailtemplate_jobs (email_templateid,jobdetailid)  select ?id,jobdetailid from emailtemplate_jobs where email_templateid=?parentid";
            //DAO.ExecuteNonQuery(sql, new MySqlParameter("id", id), new MySqlParameter("parentid", info.ParentId));
            //insertVersion(id, info.Version);
        }

        public static void insertVersion(int templateid, int version)
        {
            string sql = "insert into email_templateVersion (email_templateid,created,userid,version) values (?templateid,?created,?userid,?version)";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("templateid", templateid), new MySqlParameter("created", DateTime.UtcNow), new MySqlParameter("userid", GPSession.UserId), new MySqlParameter("version", version));
        }

        public static void duplicateTemplate(int templateId)
        {
            string sql = "insert into email_template(subject,header,body,footer,status,version,active,userid,createddate) select subject,header,body,footer,status,1,1,?userid,?createddate from email_template where email_templateid = ?templateId; select last_insert_id()";
            int id = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("templateId", templateId), new MySqlParameter("userid", GPSession.UserId), new MySqlParameter("createddate", DateTime.UtcNow)));
            sql = "update email_template set parentid=?id where email_templateid = ?id";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("id", id));

            //insertVersion(id, 0);
        }

        public static void updateTemplate(EmailTemplateInfo Info)
        {
            string sql = "update email_template set subject = ?subject,body=?body,status=?status,header=?header,footer=?footer" +
                " where email_templateid = ?email_templateid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("subject", Info.Subject), new MySqlParameter("body", Info.Body),
                new MySqlParameter("status", Info.Status), new MySqlParameter("createddate", Info.CreatedDate), new MySqlParameter("header", Info.Header), new MySqlParameter("footer", Info.Footer),
                new MySqlParameter("email_templateid", Info.email_templateid));
        }

        public static void deleteTemplate(int templateId)
        {
            string sql = "delete from emailtemplate_jobs where email_templateid=?templateid; delete from email_template where email_templateid=?templateid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("templateid", templateId));
        }

        public static MySqlDataReader searchTemplate(string keyword, int status)
        {
            string sql = "select email_templateid as id,subject,case status when 1 then 'Active' when 2 then 'Inactive' when 3 then 'Assigned' end as status,version from email_template where " +
                         " (email_templateid like concat_ws(?keyword,'%','%') or subject like concat_ws(?keyword,'%','%')) and " +
                         " (status = ?status OR ?status=0) order by email_templateid";

            return DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword), new MySqlParameter("status", status));
        }

        public static MySqlDataReader searchTemplate(string keyword, int? used, int? assigned)
        {
            string sql = "select  et.email_templateid as id,subject,et.version, case when el.candidateid>0 then 1 else 0 end as used,case when ej.jobdetailid>0 then 1 else 0 end as assigned,parentid " +
                " from email_template et left join email_log el on el.templateid=et.email_templateid left join emailtemplate_jobs ej on ej.email_templateid = et.parentid " +
                         " where (et.email_templateid like concat_ws(?keyword,'%','%') or subject like concat_ws(?keyword,'%','%')) and " +
                         " (case when ej.jobdetailid>0 then 1 else 0 end =?assigned or ?assigned is null) and (case when el.candidateid>0 then 1 else 0 end=?used or ?used is null) and active=1 " +
                         " group by et.email_templateid ,subject,et.version order by et.email_templateid";

            return DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword), new MySqlParameter("assigned", assigned), new MySqlParameter("used", used));
        }

        public static int getTemplateParentId(int templateId)
        {
            int id = 0;
            string sql = "select parentid from email_template where email_templateid=?templateid";
            id = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("templateid", templateId)));
            return id;
        }

        public static EmailTemplateInfo getMailTemplate(int emailtemplateid)
        {
            EmailTemplateInfo Info = new EmailTemplateInfo();
            string sql = "Select email_templateid as id,subject,body,status,header,footer,version,parentid from email_template where email_templateid = ?emailtemplateid";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("emailtemplateid", emailtemplateid));
            while (dr.Read())
            {
                Info.email_templateid = Convert.ToInt32(dr["id"]);
                Info.Subject = dr["subject"].ToString();
                Info.Body = dr["body"].ToString();
                Info.Status = Convert.ToInt32(dr["status"]);
                Info.Header = dr["header"].ToString();
                Info.Footer = dr["footer"].ToString();
                Info.Version = Convert.ToInt32(dr["version"].ToString());
                Info.ParentId = Convert.ToInt32(dr["parentid"].ToString());
            }
            dr.Close();
            dr.Dispose();
            return Info;
        }

        public static DataTable getMailTemplateByParentId(int parentId)
        {
            EmailTemplateInfo Info = new EmailTemplateInfo();
            string sql = "Select email_templateid as id,subject,body,status,header,footer,version,date_format(createddate,'%d-%b-%Y-%T') as createddate,username from email_template e inner join users u on e.userid=u.userid where parentid = ?parentid";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("parentid", parentId));
            DataSet ds = new DataSet();
            ds.EnforceConstraints = false;
            DataTable dt = new DataTable();

            ds.Load(dr, LoadOption.PreserveChanges, new string[1]);
            dt = ds.Tables[0];
            dr.Close();
            dr.Dispose();
            return dt;
        }

        public static bool existEmailTemplateJobs(int templateId, int jobId)
        {
            bool exist;
            string sql = "select email_templateid from emailtemplate_jobs where email_templateid=?templateId and jobdetailid=?jobId";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("templateId", templateId), new MySqlParameter("jobId", jobId));
            if (reader.HasRows)
                exist = true;
            else
                exist = false;
            reader.Close();
            reader.Dispose();

            return exist;
        }

        public static void insertEmailTemplateJobs(int templateId, int jobId)
        {
            string sql = "insert into emailtemplate_jobs (email_templateid,jobdetailid) values (?templateId,?jobId)";

            DAO.ExecuteNonQuery(sql, new MySqlParameter("templateId", templateId), new MySqlParameter("jobId", jobId));
        }

        public static void deleteEmailTemplateJobs(int templateId, int jobId)
        {
            string sql = "delete from emailtemplate_jobs where email_templateId=?templateId and jobdetailId=?jobId";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("templateId", templateId), new MySqlParameter("jobId", jobId));
        }

        public static MySqlDataReader filterJob(string keyword, uint consultantid, uint clientid, uint status, string sortexpression, int templateId)
        {
            string sql = "Select distinct JD.jobdetailid as jobid,JD.title as jobtitle,JD.referenceno as refno,JT.jobtypeid,JT.type as jobtype,Lo.locationid," +
                         "Lo.name as regionname,date_format(JD.createddate,'%d-%b-%Y') as createddate,JC.consultantid,JD.clientid,JS.status," +
                         "cl.clientname,JD.isApprove from jobdetail JD inner join jobtype JT on JD.typeid = JT.jobtypeid inner join locations Lo on JD.locationid = Lo.locationid left outer join Client Cl " +
                         "on JD.clientid = Cl.clientid left outer join jobstatus JS on JS.jobstatusid = JD.status left outer join job_consultants JC on JD.jobdetailid = JC.jobid " +
                         "Where (JD.referenceno like concat_ws(?keyword,'%','%') or JD.title like concat_ws(?keyword,'%','%')) and (JC.consultantid =?consultantid OR ?consultantid =0) and (JD.clientid =?clientid OR ?clientid=0) " +
                         "and (JD.status =?status OR ?status =0) and JD.jobdetailid not in(select jobDetailId from emailtemplate_jobs where email_templateid=?templateId) order by " + sortexpression;

            return DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword), new MySqlParameter("consultantid", consultantid), new MySqlParameter("clientid", clientid), new MySqlParameter("status", status), new MySqlParameter("templateId", templateId));
        }

        public static MySqlDataReader getTemplateJobs(int templateId)
        {
            string sql = "SELECT j.title,'' as regionname,j.jobdetailId, j.title as jobtitle,j.referenceno,j.bullet1,j.bullet2,j.bullet2,j.bullet3,j.jobcontent FROM emailtemplate_jobs cj inner join jobdetail j on cj.jobdetailid=j.jobdetailid  where email_templateid=?templateId";
            return DAO.ExecuteReader(sql, new MySqlParameter("templateId", templateId));
        }

        public static DataTable getEmailCandidates(int templateId, string sortExp)
        {
            string sql = "select c.candidateid,c.title,c.first,c.middle,c.last,c.deceased,c.nomailshots,cj.jobid,case when ce.candidateid is null then 0 else 1 end as assigned,case when el.candidateid is null then 0 else 1 end as mailsend,date_format(el.senddate,'%d-%b-%Y') as senddate " +
                        " from email_template e inner join emailtemplate_jobs ej on ej.email_templateid = e.parentid " +
                        " inner join candidate_jobsassigned cj on cj.jobid=ej.jobdetailid  " +
                        " inner join candidates c on cj.candidateid=c.candidateid  left outer join candidates_emailtemplates ce on ce.email_templateId=e.email_templateId and ce.candidateid=c.candidateid " +
                        " left outer join email_log el on el.templateId=e.email_templateid and el.candidateid=c.candidateid where e.email_templateid=?templateId  and " +
                        " c.candidateid not in(select candidateid from candidates_jobs where category=1 and jobid=cj.jobid and candidateid=cj.candidateid ) order by ?sortExp";

            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("templateId", templateId), new MySqlParameter("sortExp", sortExp));
            DataTable dt = new DataTable();
            dt.Load(reader);
            dt.DefaultView.Sort = sortExp;
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public static DataTable getEmailJobsByCandidateId(int templateId, int candidateId)
        {
            string sql = "select j.jobdetailId, j.title as jobtitle,j.referenceno,j.bullet1,j.bullet2,j.bullet2,j.bullet3,j.jobcontent from email_template e inner join emailtemplate_jobs ej on ej.email_templateid = e.parentid " +
                        " inner join candidates_jobs cj on cj.jobid=ej.jobdetailid " +
                        " inner join jobdetail j on cj.jobid=j.jobdetailid where e.email_templateid=?templateId and cj.candidateid=?candidateId ";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("templateId", templateId), new MySqlParameter("candidateId", candidateId));

            DataTable dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public static void insertCandidateEmailTempalte(int templateId, int candidateId)
        {
            string sql = "insert into candidates_emailtemplates (candidateId,email_templateId) values (?candidateId,?templateId);";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateId", candidateId), new MySqlParameter("templateId", templateId));
        }

        public static void deleteCandidateEmailTemplate(int templateId)
        {
            string sql = "delete from candidates_emailtemplates where email_templateId=?templateId";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("templateId", templateId));
        }

        public static void insertEmailLog(EmailSentHistoryInfo info)
        {
            string sql = "insert into email_log (templateId,candidateId,jobId,version,sendDate,emailid) values (?templateId,?candidateId,?jobId,?version,?sendDate,?emailid);";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("templateId", info.EmailTemplateId), new MySqlParameter("candidateId", info.CanddiateId), new MySqlParameter("jobId", info.JobId),
                new MySqlParameter("version", info.Version), new MySqlParameter("sendDate", info.SendDate), new MySqlParameter("emailid", info.EmailId));
        }

        public static MySqlDataReader getEmailLog(int templateid)
        {
            string sql = "select c.candidateid,c.title,c.first,c.last,emailid,date_format(senddate,'%d-%b-%Y-%T') as senddate,et.subject,et.email_templateid from email_log el inner join email_template et on el.templateid=et.email_templateid inner join candidates c on el.candidateid=c.candidateid where templateid=?templateid";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("templateid", templateid));
            return reader;
        }

        public static MySqlDataReader getEmailLogByCandidateid(int candidateid)
        {
            string sql = "select c.candidateid,c.title,c.first,c.last,emailid,date_format(senddate,'%d-%b-%Y-%T') as senddate,et.subject,et.email_templateid,parentid " +
                " from email_log el inner join email_template et on el.templateid=et.email_templateid inner join candidates c on el.candidateid=c.candidateid where c.candidateid=?candidateid";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("candidateid", candidateid));
            return reader;
        }

        public static MySqlDataReader getEmailTemplateByJobId(int jobid)
        {
            string sql = "select subject,et.email_templateid,parentid from email_template et inner join emailtemplate_jobs ej on ej.email_templateid=et.email_templateid where ej.jobdetailid=?jobid";

            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("jobid", jobid));
            return reader;
        }
    }
}