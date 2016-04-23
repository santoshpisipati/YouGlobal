using System;
using System.Data;
using System.Linq;
using YG_DataAccess;

namespace YG_Business
{
    public class JobAlert
    {
        public int InsertJobAlert(JobAlertInfo info, ref string candidateguid)
        {
            int candidateId = info.CadidateId;
            int alertId = info.JobAlertId;
            int emailId = 0;
            int historyType;
            //candidateId = JobAlertDataAccess.existCandidate(info.Email);
            //if (candidateId == 0)
            //{
            //    candidateId = JobAlertDataAccess.insertCandidate(1, "Unknown", info.FirstName, info.SurName, "U", info.Email, info.PhoneNo, info.PhoneCode);
            //}
            //else
            //    alertId = JobAlertDataAccess.existJobAlert(candidateId);
            DataSet ds = new DataSet();
            if (alertId > 0)
            {
                //JobAlertDataAccess.deleteJobAlert(alertId);
                historyType = 2;
                ds = JobAlertDataAccess.getJobAlert(alertId, candidateguid);
                JobAlertDataAccess.updateJobAlert(alertId, info.FrequencyId, info.PhoneCode, info.PhoneNo, info.Email);
                JobAlertDataAccess.deleteJobAlertRelatedRecord(alertId);
            }
            else
            {
                JobApply apply = new JobApply();
                DataTable dtCandidates = apply.GetCandidatesByEmail(info.Email.Trim());
                if (dtCandidates.Rows.Count == 1)
                {
                    if (dtCandidates.Rows[0]["first"].ToString().Trim().ToLower() == info.FirstName.Trim().ToLower())
                    {
                        candidateguid = dtCandidates.Rows[0]["candidateguid"].ToString();
                        candidateId = Convert.ToInt32(dtCandidates.Rows[0]["candidateid"].ToString());
                        emailId = Convert.ToInt32(dtCandidates.Rows[0]["emailid"].ToString());
                    }
                    else
                    {
                        candidateId = JobAlertDataAccess.insertCandidate(1, "Unknown", info.FirstName, info.SurName, "U", info.Email, info.PhoneNo, info.PhoneCode, ref candidateguid, ref emailId, info.MiddleName);
                        apply.CandidateCombine(dtCandidates, candidateId);
                    }
                }
                else if (dtCandidates.Rows.Count > 1)
                {
                    candidateId = JobAlertDataAccess.insertCandidate(1, "Unknown", info.FirstName, info.SurName, "U", info.Email, info.PhoneNo, info.PhoneCode, ref candidateguid, ref emailId, info.MiddleName);
                    apply.CandidateCombine(dtCandidates, candidateId);
                }
                else
                {
                    candidateId = JobAlertDataAccess.insertCandidate(1, "Unknown", info.FirstName, info.SurName, "U", info.Email, info.PhoneNo, info.PhoneCode, ref candidateguid, ref emailId, info.MiddleName);
                }
                // candidateId = JobAlertDataAccess.existCandidate(info.Email, ref candidateguid, ref emailId);
                //if (candidateId == 0)
                //{
                //    candidateId = JobAlertDataAccess.insertCandidate(1, "Unknown", info.FirstName, info.SurName, "U", info.Email, info.PhoneNo, info.PhoneCode, ref candidateguid, ref emailId);
                //}
                alertId = JobAlertDataAccess.existJobAlert(candidateId);

                if (alertId == 0)
                {
                    historyType = 1;
                    alertId = JobAlertDataAccess.insertJobAlert(candidateId, info.FrequencyId, info.PhoneCode, info.PhoneNo, info.Email, info.CreatedDate, emailId, info.Confirmed);
                }
                else
                {
                    historyType = 2;
                    ds = JobAlertDataAccess.getJobAlert(alertId, candidateguid);
                    JobAlertDataAccess.updateJobAlert(alertId, info.FrequencyId, info.PhoneCode, info.PhoneNo, info.Email);

                    JobAlertDataAccess.deleteJobAlertRelatedRecord(alertId);
                }
            }
            int historyId = 0;
            if (historyType == 1)
            {
                if (historyId == 0)
                    historyId = JobAlertDataAccess.insertJobalertHistory(candidateId, alertId, historyType);
                JobAlertDataAccess.insertHistoryDetails(historyId, "New Subscription", string.Empty, "A Job Alerts subscription was requested for " + candidateId + ". An email requesting confirmation was sent to " + info.Email);
            }
            else
            {
                if (ds.Tables[0].Rows[0]["email"].ToString() != info.Email)
                {
                    historyId = JobAlertDataAccess.insertJobalertHistory(candidateId, alertId, historyType);
                    JobAlertDataAccess.insertHistoryDetails(historyId, "Email", ds.Tables[0].Rows[0]["email"].ToString(), info.Email);
                }
                if (ds.Tables[0].Rows[0]["frequencyId"].ToString() != info.FrequencyId.ToString())
                {
                    if (historyId == 0)
                        historyId = JobAlertDataAccess.insertJobalertHistory(candidateId, alertId, historyType);
                    JobAlertDataAccess.insertHistoryDetails(historyId, "Frequency", ds.Tables[0].Rows[0]["frequencyId"].ToString(), info.FrequencyId.ToString());
                }
                DataTable dtIndustry = ds.Tables[3];
                if (dtIndustry.Rows.Count != info.IndustryList.Count)
                {
                    string industrylist = string.Empty;
                    foreach (DataRow dr in dtIndustry.Rows)
                    {
                        industrylist = dr["code"].ToString() + " " + dr["description"].ToString() + "<br/> " + industrylist;
                    }
                    if (historyId == 0)
                        historyId = JobAlertDataAccess.insertJobalertHistory(candidateId, alertId, historyType);
                    JobAlertDataAccess.insertHistoryDetails(historyId, "Industry", industrylist, info.IndustryNameList);
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
                            if (historyId == 0)
                                historyId = JobAlertDataAccess.insertJobalertHistory(candidateId, alertId, historyType);
                            JobAlertDataAccess.insertHistoryDetails(historyId, "Industry", industrylist, info.IndustryNameList);
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
                    if (historyId == 0)
                        historyId = JobAlertDataAccess.insertJobalertHistory(candidateId, alertId, historyType);
                    JobAlertDataAccess.insertHistoryDetails(historyId, "Occupation", occupationlist, info.OccupationNameList);
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
                            if (historyId == 0)
                                historyId = JobAlertDataAccess.insertJobalertHistory(candidateId, alertId, historyType);
                            JobAlertDataAccess.insertHistoryDetails(historyId, "Occupation", occupationlist, info.OccupationNameList);
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
                        locationList = dr["location"].ToString() + "<br/> " + locationList;
                    }
                    if (historyId == 0)
                        historyId = JobAlertDataAccess.insertJobalertHistory(candidateId, alertId, historyType);
                    JobAlertDataAccess.insertHistoryDetails(historyId, "Location", locationList, info.LocationNameList);
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
                                locationList = dr["location"].ToString() + "<br/> " + locationList;
                            }
                            if (historyId == 0)
                                historyId = JobAlertDataAccess.insertJobalertHistory(candidateId, alertId, historyType);
                            JobAlertDataAccess.insertHistoryDetails(historyId, "Location", locationList, info.LocationNameList);
                            break;
                        }
                    }
                }
            }

            foreach (JobAlertIndustry industry in info.IndustryList)
            {
                JobAlertDataAccess.inserJobAlertIndustry(alertId, candidateId, industry.ISICRev4Id);
            }

            foreach (JobAlertLocation location in info.LocationList)
            {
                JobAlertDataAccess.insertJobAlertLocation(alertId, candidateId, location.LocationId, location.LocationType);
            }

            foreach (JobAlertWorkType type in info.WorkTypeList)
            {
                JobAlertDataAccess.insertJobAlertWorkType(alertId, candidateId, type.WorkTypeId);
            }

            foreach (JobAlertOccupation occupation in info.OccupationList)
            {
                JobAlertDataAccess.insertJobAlertOccupation(alertId, candidateId, occupation.ISCO08Id);
            }

            return alertId;
        }

        public int InsertJobAlert(JobAlertInfo info)
        {
            int candidateId = info.CadidateId;
            int alertId = info.JobAlertId;
            int emailId = JobAlertDataAccess.getCandidateEmailId(candidateId, info.Email);

            alertId = JobAlertDataAccess.insertJobAlert(candidateId, info.FrequencyId, info.PhoneCode, info.PhoneNo, info.Email, info.CreatedDate, emailId, info.Confirmed);
            int historyid = JobAlertDataAccess.insertJobalertHistory(candidateId, alertId, 1);
            JobAlertDataAccess.insertHistoryDetails(historyid, "New Subscription", string.Empty, "New Job Alerts subscription requested by Candidate " + candidateId + " whilst uploading a CV or applying for a job. The need to confirm the Job Alerts request has been bypassed. Job Alerts will be sent to \"" + info.Email + "\".");

            foreach (JobAlertIndustry industry in info.IndustryList)
            {
                JobAlertDataAccess.inserJobAlertIndustry(alertId, candidateId, industry.ISICRev4Id);
            }
            if (!string.IsNullOrEmpty(info.IndustryNameList))
            {
                JobAlertDataAccess.insertHistoryDetails(historyid, "Industry", string.Empty, info.IndustryNameList);
            }

            foreach (JobAlertLocation location in info.LocationList)
            {
                JobAlertDataAccess.insertJobAlertLocation(alertId, candidateId, location.LocationId, location.LocationType);
            }
            if (!string.IsNullOrEmpty(info.LocationNameList))
            {
                JobAlertDataAccess.insertHistoryDetails(historyid, "Location", string.Empty, info.LocationNameList);
            }

            foreach (JobAlertWorkType type in info.WorkTypeList)
            {
                JobAlertDataAccess.insertJobAlertWorkType(alertId, candidateId, type.WorkTypeId);
            }

            foreach (JobAlertOccupation occupation in info.OccupationList)
            {
                JobAlertDataAccess.insertJobAlertOccupation(alertId, candidateId, occupation.ISCO08Id);
            }
            if (!string.IsNullOrEmpty(info.OccupationNameList))
            {
                JobAlertDataAccess.insertHistoryDetails(historyid, "Occupation", string.Empty, info.OccupationNameList);
            }
            return alertId;
        }

        public void AlertConfirmation(int alertId)
        {
            DataTable dt = getJobAlert(alertId);
            if (dt.Rows.Count > 0)
            {
                int historyId = JobAlertDataAccess.insertJobalertHistory(Convert.ToInt32(dt.Rows[0]["candidateid"]), alertId, 2);
                JobAlertDataAccess.insertHistoryDetails(historyId, "Subscription confirmation", string.Empty, "A Job Alerts subscription request for " + dt.Rows[0]["candidateid"]
                    + " has been confirmed and is now active, with Job Alerts being sent to " + dt.Rows[0]["email"]);
            }
            JobAlertDataAccess.confirmSubscribtion(alertId);
        }

        public void AlertDelete(int alertId, int candidateid, string candidateGUId, string firstname, string lastname)
        {
            JobAlertDataAccess.deleteJobAlert(alertId, candidateid, candidateGUId, firstname, lastname);
        }

        public DataTable getJobAlert(int alertId)
        {
            DataTable dt = new DataTable();
            dt = JobAlertDataAccess.getJobAlert(alertId);
            return dt;
        }

        public DataSet getJobAlert(int alertId, string candidateguid)
        {
            DataSet ds = new DataSet();
            ds = JobAlertDataAccess.getJobAlert(alertId, candidateguid);

            return ds;
        }

        public DataTable getJobalertIndustry(int alertId)
        {
            DataTable dt = new DataTable();
            dt = JobAlertDataAccess.getJobalertIndustry(alertId);
            return dt;
        }

        public int getJobAlertId(string email, ref string candidateguid)
        {
            int alertId = 0;
            int emailId = 0;
            int candidateId = JobAlertDataAccess.existCandidate(email, ref candidateguid, ref emailId);
            if (candidateId > 0)
                alertId = JobAlertDataAccess.existJobAlert(candidateId);

            return alertId;
        }

        public string updateCandidateGUID(int candidateId)
        {
            string candidateGuid = Guid.NewGuid().ToString();
            JobAlertDataAccess.updateCandidateGuid(candidateId, candidateGuid);
            return candidateGuid;
        }

        public int existJobalert(int candidateId)
        {
            return JobAlertDataAccess.existJobAlert(candidateId);
        }

        public bool IsNoJobalertset(int candidateid)
        {
            return JobAlertDataAccess.IsNoJobalertset(candidateid);
        }
    }
}