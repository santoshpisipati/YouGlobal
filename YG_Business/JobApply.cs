using System;
using System.Collections.Generic;
using System.Data;

using YG_DataAccess;

namespace YG_Business
{
    public class JobApply
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

        public DataTable GetCandidatesByEmail(string email)
        {
            DataTable dt = JobApplyDataAccess.GetCandidatesByEmail(email);

            return dt;
        }

        public int CreateCandidate(string title, string first, string nickname, string gender, string email, string phoneNumber, string phoneCode, DateTime? dob, ref string candidateguid)
        {
            int candidateId = JobApplyDataAccess.insertCandidate(1, title, first.Trim(), nickname.Trim(), gender, email.Trim(), phoneNumber.Trim(), phoneCode, dob, ref candidateguid);

            return candidateId;
        }

        public int CreateCandidate(string title, string first, string last, string middle, string nickname, string gender, string marital, string email, uint emailtype, DateTime? dob, ref string candidateguid, List<string[]> numbers, List<int> nationalities, string skype)
        {
            int candidateId = JobApplyDataAccess.insertCandidate(1, title, first.Trim(), last, middle, nickname, gender, marital, emailtype, email.Trim(), dob, ref candidateguid, numbers, nationalities, skype);
            return candidateId;
        }

        public void UpdateCandidate(int candidateId, string lastname, string phoneNumber, string phoneCode)
        {
            JobApplyDataAccess.updateCandidate(candidateId, lastname.Trim(), phoneNumber.Trim(), phoneCode);
        }

        public void UpdateCandidate(int candidateId, string title, string first, string last, string middle, string nickname, string gender, string marital, string email, int emailid, uint emailtype, DateTime? dob, ref string candidateguid, List<string[]> numbers, List<int> nationalities, string skype)
        {
            JobApplyDataAccess.updateCandidate(candidateId, title, first.Trim(), last, middle, nickname, gender, marital, email.Trim(), emailid, emailtype, dob, ref candidateguid, numbers, nationalities, skype);
        }

        public void LinktoJob(int candidateId, int jobId)
        {
            if (!JobApplyDataAccess.existCandidateJobsAssigned(candidateId, jobId))
                JobApplyDataAccess.insertCandidateJobsAssigned(candidateId, jobId);
            JobApplyDataAccess.insertCandidateJobs(candidateId, jobId, 1, null);
        }

        public void CandidateCombine(DataTable dt, int master)
        {
            foreach (DataRow dr in dt.Rows)
            {
                JobApplyDataAccess.CandidateCombine(master, Convert.ToInt32(dr["candidateId"].ToString()));
            }
        }

        //public void UploadFile(int candidateid, int filetypeid, string filename, Stream file)
        //{
        //    JobApplyDataAccess.addFile(candidateid, filetypeid, filename, file);
        //}

        public DataTable GetCandidateFiles(int candidateId)
        {
            return JobApplyDataAccess.getCandidateFiles(candidateId);
        }

        public void InsertJobalertInvitation(int candidateId, string email)
        {
            JobApplyDataAccess.insertJobalertInvite(candidateId, email);
        }

        public bool JobalertInviteExist(int candidateId)
        {
            return JobApplyDataAccess.jobalertinviteexist(candidateId);
        }

        public string ConsultantEmail(int consultantId, ref int utcoffset)
        {
            DataTable dt = JobApplyDataAccess.getConsultantEmailById(consultantId);
            string toEmail = string.Empty;

            if (dt.Rows.Count > 0)
            {
                utcoffset = Convert.ToInt32(dt.Rows[0]["utcoffset"].ToString());
                DataRow[] dr = dt.Select("defaultemail=1");
                if (dr.Length == 0)
                    toEmail = dt.Rows[0]["email"].ToString();
                else
                    toEmail = dr[0]["email"].ToString();
            }
            return toEmail;
        }

        public void insertWorkhistory(int candidateid, int industryid, int positionid)
        {
            JobApplyDataAccess.addWorkHistory(candidateid, industryid, positionid);
        }

        public void addWorkhistory(int candidateid, DateTime? from, string from_display, DateTime? to, string to_display, int? locationid, int? locationtype, int? industryid, int? positionid, string employername)
        {
            int employerid;
            employerid = CommonDataAccess.existEmployer(employername);
            if (employerid == 0)
                employerid = CommonDataAccess.addEmployer(employername);
            JobApplyDataAccess.addWorkHistory(candidateid, from, from_display, to, to_display, locationid, locationtype, industryid, positionid, employerid);
        }

        public void addCandidateSalary(int candidateId, int type, int fromSalary, int toSalary, int frequency, string currency, DateTime salarydate, int sourcetype, int sourceid)
        {
            JobApplyDataAccess.addCandidateSalary(candidateId, type, fromSalary, toSalary, frequency, currency, salarydate, sourcetype, sourceid);
        }

        public int insertSaveObject(string email, byte[] buffer, int jobid, ref string secuirtyid)
        {
            return JobApplyDataAccess.insertSaveLaterObject(email, buffer, jobid, ref secuirtyid);
        }

        public byte[] getSaveLaterObject(string securityid, int id)
        {
            return JobApplyDataAccess.getSaveLaterObject(securityid, id);
        }

        public void deleteSaveObject(string email, int jobId)
        {
            JobApplyDataAccess.deleteSaveObject(email, jobId);
        }

        public void addCurrentLocation(int candidateid, int locationid, int locationtype)
        {
            JobApplyDataAccess.addCurrentlocation(candidateid, locationid, locationtype);
        }

        public void removeCurrentLocation(int candidateId)
        {
            JobApplyDataAccess.removeCurrentlocations(candidateId);
        }

        public DataTable getCurrentLocation(int candidateId)
        {
            return JobApplyDataAccess.getCurrentlocations(candidateId);
        }

        public bool checkGroupExist(DataTable dt, int groupId)
        {
            return JobApplyDataAccess.checkGroupExist(dt, groupId);
        }

        public bool checkLocationGroupExist(DataTable dt, int groupid, int locationid, int locationtype)
        {
            return JobApplyDataAccess.checkLocationGroupExist(dt, groupid, locationid, locationtype);
        }

        public void UnrecognisedLocationHistory(int candidateid, string firstname, string middlename, string lastname, string locationtext, string nameoffield, string jobrefcode)
        {
            JobApplyDataAccess.insertUnrecognisedLocationHistory(candidateid, firstname, middlename, lastname, locationtext, nameoffield, jobrefcode);
        }
    }
}