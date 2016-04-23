using GlobalPanda.BusinessInfo;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

/// <summary>
/// Summary description for CVFormatDataProvider
/// </summary>
namespace GlobalPanda.DataProviders
{
    public class CVFormatDataProvider
    {
        public static int insertCVFormat(CVFormatInfo info)
        {
            string sql = "insert into cvformat (candidateid,jobid,interface,interfacedate,interfacenote,location,visa,dob,nodob,dobformat,releventExperience,marital,lastsalarycurrency,lastsalary,lastsalaryfrequency,lastsalarynote,expectsalarycurrency " +
                " ,expectsalary,expectsalaryfrequency,expectsalarynote,availability,userid,created,lasttosalary,expecttosalary) values (?candidateid,?jobid,?interface,?interfacedate,?interfacenote,?location,?visa,?dob,?nodob,?dobformat,?releventExperience,?marital,?lastsalcurrency,?lastsalary " +
                " ,?lastsalfreq,?lastsalnote,?expectsalcurrency,?expectsalary,?expectsalfreq,?expectsalnote,?availability,?userid,?created,?lasttosalary,?expecttosalary); select last_insert_id()";
            MySqlParameter[] param ={
                                        new MySqlParameter("candidateid",info.CandidateId),
                                        new MySqlParameter("jobid",info.JobId),
                                        new MySqlParameter("interface",info.Interfaced),
                                        new MySqlParameter("interfacedate",info.InterfacedDate),
                                        new MySqlParameter("interfacenote",info.InterfacedNote),
                                        new MySqlParameter("location",info.Location),
                                        new MySqlParameter("visa",info.Visa),
                                        new MySqlParameter("dob",info.DOB),
                                        new MySqlParameter("nodob",info.NODOB),
                                        new MySqlParameter("dobformat",info.DOBFormat),
                                        new MySqlParameter("releventExperience",info.ReleventExperience),
                                        //new MySqlParameter("qualification",info.Qualification),
                                        new MySqlParameter("marital",info.Marital),
                                        new MySqlParameter("lastsalcurrency",info.LastSalaryCurrency),
                                        new MySqlParameter("lastsalary",info.LastSalary),
                                        new MySqlParameter("lastsalfreq",info.LastSalaryFrequency),
                                        new MySqlParameter("lastsalnote",info.LastSalaryNote),
                                        new MySqlParameter("expectsalcurrency",info.ExpectSalaryCurrency),
                                        new MySqlParameter("expectsalary",info.ExpectSalary),
                                        new MySqlParameter("expectsalfreq",info.ExpectSalaryFrequency),
                                        new MySqlParameter("expectsalnote",info.ExpectSalaryNote),
                                        new MySqlParameter("availability",info.Availability),
                                        new MySqlParameter("userid",GPSession.UserId),
                                        new MySqlParameter("created",DateTime.UtcNow),
                                        new MySqlParameter("lasttosalary",info.LastToSalary),
                                        new MySqlParameter("expecttosalary",info.ExpectToSalary)
                                     };
            int id = Convert.ToInt32(DAO.ExecuteScalar(sql, param));

            CandidateDataProvider.updateCandidateBirthDate(info.CandidateId, Convert.ToDateTime(info.DOB), info.NODOB, info.DOBFormat);
            CandidateDataProvider.updateCandidateMaritalStatus(info.CandidateId, info.Marital);

            if (!string.IsNullOrEmpty(info.LastToSalary))
            {
                CandidateDataProvider.addCandidateSalary(Convert.ToUInt32(info.CandidateId), 1, Convert.ToInt32(string.IsNullOrEmpty(info.LastSalary) ? "0" : info.LastSalary), Convert.ToInt32(info.LastToSalary), info.LastSalaryFrequency, info.LastSalaryCurrency, DateTime.UtcNow, id, 3);
            }
            if (!string.IsNullOrEmpty(info.ExpectToSalary))
            {
                CandidateDataProvider.addCandidateSalary(Convert.ToUInt32(info.CandidateId), 2, Convert.ToInt32(string.IsNullOrEmpty(info.ExpectSalary) ? "0" : info.ExpectSalary), Convert.ToInt32(info.ExpectToSalary), info.LastSalaryFrequency, info.LastSalaryCurrency, DateTime.UtcNow, id, 3);
            }

            insertCVFormatStatus(id, 1);

            CandidateDataProvider.addNationfromCVFormat(info.CandidateId, info.NationalityList);
            foreach (int countryid in info.NationalityList)
            {
                insertNationality(countryid, id);
            }

            CandidateDataProvider.removeLanguage(info.CandidateId);
            foreach (CVFormatLanguageInfo language in info.LanguageList)
            {
                language.CVFormatId = id;
                insertCVFormatLanguages(language);
                CandidateDataProvider.addLanguagefromCVFormat(info.CandidateId, language.LanguageId, language.Spoken, language.Written, language.Listening, language.Reading);
            }
            foreach (ChildStatusInfo child in info.ChildStatusList)
            {
                child.CVFormatId = id;
                insertChildStatus(child);
            }
            foreach (SummaryPointInfo summary in info.SummaryPointList)
            {
                summary.CVFormatId = id;
                insertSummaryPoint(summary);
            }
            foreach (AdditionalInfo additional in info.AdditionalInfoList)
            {
                additional.CVFormatId = id;
                insertAdditionalInfo(additional);
            }
            foreach (QualificationInfo qualification in info.QualificationList)
            {
                qualification.CVFormatId = id;
                insertQualification(qualification);
            }
            foreach (CriteriaNotMetInfo criteria in info.CriteriaNotMetList)
            {
                criteria.CVFormatId = id;
                insertCriteriaNotMet(criteria);
            }
            return id;
        }

        public static void insertCVFormatStatus(int cvformatId, int status)
        {
            string sql = "insert into cvformat_status (cvformatid,status,userid,modified) values(?cvformatid,?status,?userid,?modified)";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("cvformatid", cvformatId), new MySqlParameter("status", status), new MySqlParameter("userid", GPSession.UserId), new MySqlParameter("modified", DateTime.UtcNow));
        }

        public static void updateCVFormatStatus(int cvformatid, int status, int oldstatus)
        {
            string sql = "update cvformat_Status set status=?status,userid=?userid,modified=?modified where cvformatid=?cvformatid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("cvformatid", cvformatid), new MySqlParameter("status", status), new MySqlParameter("userid", GPSession.UserId), new MySqlParameter("modified", DateTime.UtcNow));

            HistoryInfo historyInfo = new HistoryInfo();
            historyInfo.UserId = GPSession.UserId;
            historyInfo.ModuleId = (int)HistoryInfo.Module.CVFormat;
            historyInfo.TypeId = (int)HistoryInfo.ActionType.Edit;
            historyInfo.RecordId = Convert.ToUInt32(cvformatid);
            historyInfo.Details = new List<HistoryDetailInfo>();
            string oldStatus = string.Empty;
            string newstatus = string.Empty;
            if (status == 1)
                newstatus = "Request";
            else if (status == 2)
                newstatus = "Completed";
            else if (status == 3)
                newstatus = "Cancelled";
            else if (status == 4)
                newstatus = "Waiting";

            if (oldstatus == 1)
                oldStatus = "Request";
            else if (oldstatus == 2)
                oldStatus = "Completed";
            else if (oldstatus == 3)
                oldStatus = "Cancelled";
            else if (oldstatus == 4)
                oldStatus = "Waiting";
            historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "Status", OldValue = oldStatus, NewValue = newstatus });

            HistoryDataProvider history = new HistoryDataProvider();
            history.insertHistory(historyInfo);
        }

        public static void insertCVFormatLanguages(CVFormatLanguageInfo info)
        {
            string sql = "insert into cvformat_languages (cvformatid,languageid,spoken,written,listening,reading) values (?cvformatid,?languageid,?spoken,?written,?listening,?reading)";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("cvformatid", info.CVFormatId), new MySqlParameter("languageid", info.LanguageId), new MySqlParameter("spoken", info.Spoken), new MySqlParameter("written", info.Written),
                new MySqlParameter("listening", info.Listening), new MySqlParameter("reading", info.Reading));
        }

        public static void insertNationality(int countryid, int cvformatId)
        {
            string sql = "insert into cvformat_nationality(cvformatid,countryid) values(?cvformatid,?countryid)";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("cvformatid", cvformatId), new MySqlParameter("countryid", countryid));
        }

        public static void insertChildStatus(ChildStatusInfo info)
        {
            string sql = "insert into cvformat_childstatus(cvformatid,gender,age,relationshipsid) values (?cvformatid,?gender,?age,?relationshipsid)";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("cvformatid", info.CVFormatId), new MySqlParameter("gender", info.Gender), new MySqlParameter("age", info.Age), new MySqlParameter("relationshipsid", info.RelationshipId));
        }

        public static void insertSummaryPoint(SummaryPointInfo info)
        {
            string sql = "insert into cvformat_summarypoints(cvformatid,notes,userid,created) values (?cvformatid,?notes,?userid,?created)";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("cvformatid", info.CVFormatId), new MySqlParameter("notes", info.Notes), new MySqlParameter("userid", info.UserId), new MySqlParameter("created", info.Created));
        }

        public static void insertAdditionalInfo(AdditionalInfo info)
        {
            string sql = "insert into cvformat_additionalinfo(cvformatid,notes,userid,created) values (?cvformatid,?notes,?userid,?created)";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("cvformatid", info.CVFormatId), new MySqlParameter("notes", info.Notes), new MySqlParameter("userid", info.UserId), new MySqlParameter("created", info.Created));
        }

        public static void insertCriteriaNotMet(CriteriaNotMetInfo info)
        {
            string sql = "insert into cvforamt_criterianotmet (cvformatid,criteriaid,criteria,criteriamet,reason,type) values (?cvformatid,?criteriaid,?criteria,?criteriamet,?reason,?type)";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("cvformatid", info.CVFormatId), new MySqlParameter("criteriaid", info.CriteriaId), new MySqlParameter("criteria", info.Criteria), new MySqlParameter("criteriamet", info.CriteriaMet),
                new MySqlParameter("reason", info.Reason), new MySqlParameter("type", info.Type));
        }

        public static void insertQualification(QualificationInfo info)
        {
            string sql = "insert into cvformat_qualification(cvformatid,qualification) values (?cvformatid,?qualification)";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("cvformatid", info.CVFormatId), new MySqlParameter("qualification", info.Qualification));
        }

        public static void updateCVFormat(CVFormatInfo info)
        {
            MySqlDataReader dr = getCVFormat(info.CVFormatId);

            DataSet dsCvFormat = new DataSet();
            string[] tbl = new string[10];
            dsCvFormat.EnforceConstraints = false;
            dsCvFormat.Load(dr, LoadOption.PreserveChanges, tbl);

            string sql = "UPDATE cvformat SET interface=?interface,interfaceDate= ?interfaceDate,interfacenote=?interfacenote,location=?location,visa=?visa,dob=?dob,nodob=?nodob,dobformat=?dobformat,releventExperience=?releventExperience,marital=?marital," +
                " lastsalarycurrency=?lastsalcurrency,lastsalary=?lastsalary,lastsalaryfrequency=?lastsalfreq,lastsalarynote=?lastsalnote,expectsalarycurrency=?expectsalcurrency,expectsalary=?expectsalary," +
                " expectsalaryfrequency=?expectsalfreq,expectsalarynote=?expectsalnote,availability=?availability,modified=?modified,expecttosalary=?expecttosalary WHERE cvformatid=?cvformatid; ";

            MySqlParameter[] param ={
                                        new MySqlParameter("interface",info.Interfaced),
                                        new MySqlParameter("interfacedate",info.InterfacedDate),
                                        new MySqlParameter("interfacenote",info.InterfacedNote),
                                        new MySqlParameter("location",info.Location),
                                        new MySqlParameter("visa",info.Visa),
                                        new MySqlParameter("dob",info.DOB),
                                        new MySqlParameter("nodob",info.NODOB),
                                        new MySqlParameter("dobformat",info.DOBFormat),
                                        new MySqlParameter("releventExperience",info.ReleventExperience),
                                        //new MySqlParameter("qualification",info.Qualification),
                                        new MySqlParameter("marital",info.Marital),
                                        new MySqlParameter("lastsalcurrency",info.LastSalaryCurrency),
                                        new MySqlParameter("lastsalary",info.LastSalary),
                                        new MySqlParameter("lastsalfreq",info.LastSalaryFrequency),
                                        new MySqlParameter("lastsalnote",info.LastSalaryNote),
                                        new MySqlParameter("expectsalcurrency",info.ExpectSalaryCurrency),
                                        new MySqlParameter("expectsalary",info.ExpectSalary),
                                        new MySqlParameter("expectsalfreq",info.ExpectSalaryFrequency),
                                        new MySqlParameter("expectsalnote",info.ExpectSalaryNote),
                                        new MySqlParameter("availability",info.Availability),
                                        new MySqlParameter("modified",DateTime.UtcNow),
                                        new MySqlParameter("cvformatid",info.CVFormatId),
                                        new MySqlParameter("expecttosalary",info.ExpectToSalary)
                                     };
            DAO.ExecuteNonQuery(sql, param);
            if (dr.HasRows)
            {
                string oldvalue = string.Empty;
                string newvalue = string.Empty;
                bool lastremunarationchanged = false;
                bool expectremunarationchanged = false;

                HistoryInfo historyInfo = new HistoryInfo();
                historyInfo.UserId = GPSession.UserId;
                historyInfo.ModuleId = (int)HistoryInfo.Module.CVFormat;
                historyInfo.TypeId = (int)HistoryInfo.ActionType.Edit;
                historyInfo.RecordId = Convert.ToUInt32(info.CVFormatId);
                historyInfo.Details = new List<HistoryDetailInfo>();

                dr.Read();
                if (DAO.getString(dr, "Interface") != info.Interfaced)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "Interface", OldValue = DAO.getString(dr, "Interface"), NewValue = info.Interfaced });
                }
                if (DAO.getString(dr, "interfacedate") != info.InterfacedDate.ToString())
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "interfacedate", OldValue = DAO.getString(dr, "interfacedate"), NewValue = info.InterfacedDate.ToString() });
                }
                if (DAO.getString(dr, "interfacenote") != info.InterfacedNote)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "interfacenote", OldValue = DAO.getString(dr, "interfacenote"), NewValue = info.InterfacedNote });
                }
                if (DAO.getString(dr, "location") != info.Location)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "location", OldValue = DAO.getString(dr, "location"), NewValue = info.Location });
                }
                if (DAO.getString(dr, "visa") != info.Visa)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "visa", OldValue = DAO.getString(dr, "visa"), NewValue = info.Visa });
                }
                if (DAO.getString(dr, "dob") != info.DOB.ToString())
                {
                    oldvalue = String.IsNullOrEmpty(DAO.getString(dr, "dob")) ? "Birth date not provided by Candidate" : DAO.getString(dr, "dob");
                    newvalue = String.IsNullOrEmpty(info.DOB.ToString()) ? "Birth date not provided by Candidate" : info.DOB.ToString();

                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "dob", OldValue = oldvalue, NewValue = newvalue });
                }
                if (DAO.getString(dr, "releventExperience") != info.ReleventExperience)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "releventExperience", OldValue = DAO.getString(dr, "releventExperience"), NewValue = info.ReleventExperience });
                }
                if (DAO.getString(dr, "marital") != info.Marital)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "marital", OldValue = DAO.getString(dr, "marital"), NewValue = info.Marital });
                }

                lastremunarationchanged = false;
                oldvalue = DAO.getString(dr, "lastsalarycurrency") + " " + DAO.getString(dr, "lastsalary") + " " + DAO.getString(dr, "lastsalaryfrequency") + " " + DAO.getString(dr, "lastsalarynote");
                newvalue = info.LastSalaryCurrency + " " + info.LastSalary + " " + info.LastSalaryFrequency.ToString() + " " + info.LastSalaryNote;

                if (DAO.getString(dr, "lastsalarycurrency") != info.LastSalaryCurrency)
                {
                    if (!lastremunarationchanged)
                    {
                        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "last remunaration", OldValue = oldvalue, NewValue = newvalue });
                        lastremunarationchanged = true;
                    }
                }
                if (DAO.getString(dr, "lastsalary") != info.LastSalary)
                {
                    if (!lastremunarationchanged)
                    {
                        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "last remunaration", OldValue = oldvalue, NewValue = newvalue });
                        lastremunarationchanged = true;
                    }
                }
                if (DAO.getString(dr, "lastsalaryfrequency") != info.LastSalaryFrequency.ToString())
                {
                    if (!lastremunarationchanged)
                    {
                        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "last remunaration", OldValue = oldvalue, NewValue = newvalue });
                        lastremunarationchanged = true;
                    }
                }
                if (DAO.getString(dr, "lastsalarynote") != info.LastSalaryNote)
                {
                    if (!lastremunarationchanged)
                    {
                        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "last remunaration", OldValue = oldvalue, NewValue = newvalue });
                        lastremunarationchanged = true;
                    }
                }

                expectremunarationchanged = false;
                oldvalue = DAO.getString(dr, "expectsalarycurrency") + " " + DAO.getString(dr, "expectsalary") + " " + DAO.getString(dr, "expectsalaryfrequency") + " " + DAO.getString(dr, "expectsalarynote");
                newvalue = info.ExpectSalaryCurrency + " " + info.ExpectSalary + " " + info.ExpectSalaryFrequency.ToString() + " " + info.ExpectSalaryNote;

                if (DAO.getString(dr, "expectsalarycurrency") != info.ExpectSalaryCurrency)
                {
                    if (!expectremunarationchanged)
                    {
                        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "expect remunaration", OldValue = oldvalue, NewValue = newvalue });
                        expectremunarationchanged = true;
                    }
                }
                if (DAO.getString(dr, "expectsalary") != info.ExpectSalary)
                {
                    if (!expectremunarationchanged)
                    {
                        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "expect remunaration", OldValue = oldvalue, NewValue = newvalue });
                        expectremunarationchanged = true;
                    }
                }
                if (DAO.getString(dr, "expectsalaryfrequency") != info.ExpectSalaryFrequency.ToString())
                {
                    if (!expectremunarationchanged)
                    {
                        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "expect remunaration", OldValue = oldvalue, NewValue = newvalue });
                        expectremunarationchanged = true;
                    }
                }
                if (DAO.getString(dr, "expectsalarynote") != info.ExpectSalaryNote)
                {
                    if (!expectremunarationchanged)
                    {
                        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "expect remunaration", OldValue = oldvalue, NewValue = newvalue });
                        expectremunarationchanged = true;
                    }
                }
                if (DAO.getString(dr, "availability") != info.Availability)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "availability", OldValue = DAO.getString(dr, "availability"), NewValue = info.Availability });
                }

                if (historyInfo.Details.Count > 0)
                {
                    HistoryDataProvider history = new HistoryDataProvider();

                    history.insertHistory(historyInfo);
                }
            }

            HistoryInfo historyInfoList = new HistoryInfo();
            historyInfoList.UserId = GPSession.UserId;
            historyInfoList.ModuleId = (int)HistoryInfo.Module.CVFormat;
            historyInfoList.TypeId = (int)HistoryInfo.ActionType.Edit;
            historyInfoList.RecordId = Convert.ToUInt32(info.CVFormatId);
            historyInfoList.Details = new List<HistoryDetailInfo>();

            CandidateDataProvider.updateCandidateBirthDate(info.CandidateId, Convert.ToDateTime(info.DOB), info.NODOB, info.DOBFormat);
            CandidateDataProvider.updateCandidateMaritalStatus(info.CandidateId, info.Marital);
            if (!string.IsNullOrEmpty(info.LastToSalary))
            {
                CandidateDataProvider.addCandidateSalary(Convert.ToUInt32(info.CandidateId), 1, Convert.ToInt32(string.IsNullOrEmpty(info.LastSalary) ? "0" : info.LastSalary), Convert.ToInt32(info.LastToSalary), info.LastSalaryFrequency, info.LastSalaryCurrency, DateTime.UtcNow, info.CVFormatId, 3);
            }
            if (!string.IsNullOrEmpty(info.ExpectToSalary))
            {
                CandidateDataProvider.addCandidateSalary(Convert.ToUInt32(info.CandidateId), 2, Convert.ToInt32(string.IsNullOrEmpty(info.ExpectSalary) ? "0" : info.ExpectSalary), Convert.ToInt32(info.ExpectToSalary), info.LastSalaryFrequency, info.LastSalaryCurrency, DateTime.UtcNow, info.CVFormatId, 3);
            }

            //For Getting New Nationality inserted
            string newdata = string.Empty;
            string olddata = string.Empty;

            deleteCVFormatRelated(info.CVFormatId);

            CandidateDataProvider.addNationfromCVFormat(info.CandidateId, info.NationalityList);
            foreach (int countryid in info.NationalityList)
            {
                DataRow[] rwCountry = dsCvFormat.Tables[1].Select("countryid = " + countryid.ToString());
                if (rwCountry.Length == 0)
                {
                    newdata = LocationDataProvider.getCountry(countryid);
                    historyInfoList.Details.Add(new HistoryDetailInfo { ColumnName = "nationality", OldValue = string.Empty, NewValue = newdata });
                }
                insertNationality(countryid, info.CVFormatId);
            }

            CandidateDataProvider.removeLanguage(info.CandidateId);
            foreach (CVFormatLanguageInfo language in info.LanguageList)
            {
                DataRow[] rwLanguage = dsCvFormat.Tables[2].Select("languageid = " + language.LanguageId);
                if (rwLanguage.Length == 0)
                {
                    newdata = language.Language;
                    historyInfoList.Details.Add(new HistoryDetailInfo { ColumnName = "languages", OldValue = string.Empty, NewValue = newdata + " added" });
                }

                language.CVFormatId = info.CVFormatId;
                insertCVFormatLanguages(language);
                CandidateDataProvider.addLanguagefromCVFormat(info.CandidateId, language.LanguageId, language.Spoken, language.Written, language.Listening, language.Reading);
            }
            foreach (ChildStatusInfo child in info.ChildStatusList)
            {
                DataRow[] rwChildStatus = dsCvFormat.Tables[3].Select("relationshipsid = " + child.RelationshipId.ToString() + " and age = " + child.Age + " and gender = '" + child.Gender + "'");
                if (rwChildStatus.Length == 0)
                {
                    newdata = child.Relationship + " " + child.Gender + " " + child.Age;
                    historyInfoList.Details.Add(new HistoryDetailInfo { ColumnName = "relationships", OldValue = string.Empty, NewValue = newdata + " added" });
                }

                child.CVFormatId = info.CVFormatId;
                insertChildStatus(child);
            }
            foreach (SummaryPointInfo summary in info.SummaryPointList)
            {
                DataRow[] rwSummaryPoint = dsCvFormat.Tables[4].Select("notes = '" + summary.Notes.ToString() + "'");
                if (rwSummaryPoint.Length == 0)
                {
                    newdata = summary.Notes;
                    historyInfoList.Details.Add(new HistoryDetailInfo { ColumnName = "summary point", OldValue = string.Empty, NewValue = newdata + " added" });
                }

                summary.CVFormatId = info.CVFormatId;
                insertSummaryPoint(summary);
            }
            foreach (AdditionalInfo additional in info.AdditionalInfoList)
            {
                DataRow[] rwAddInfo = dsCvFormat.Tables[5].Select("notes = '" + additional.Notes + "'");
                if (rwAddInfo.Length == 0)
                {
                    newdata = additional.Notes;
                    historyInfoList.Details.Add(new HistoryDetailInfo { ColumnName = "additional instruction", OldValue = string.Empty, NewValue = newdata + " added" });
                }

                additional.CVFormatId = info.CVFormatId;
                insertAdditionalInfo(additional);
            }
            foreach (QualificationInfo qualification in info.QualificationList)
            {
                DataRow[] rwQualification = dsCvFormat.Tables[7].Select("qualification = '" + qualification.Qualification.ToString() + "'");
                if (rwQualification.Length == 0)
                {
                    newdata = qualification.Qualification;
                    historyInfoList.Details.Add(new HistoryDetailInfo { ColumnName = "qualification", OldValue = string.Empty, NewValue = newdata + " added" });
                }

                qualification.CVFormatId = info.CVFormatId;
                insertQualification(qualification);
            }
            foreach (CriteriaNotMetInfo criteria in info.CriteriaNotMetList)
            {
                criteria.CVFormatId = info.CVFormatId;
                insertCriteriaNotMet(criteria);
            }

            if (historyInfoList.Details.Count > 0)
            {
                HistoryDataProvider history = new HistoryDataProvider();
                history.insertHistory(historyInfoList);
            }
        }

        public static void deleteCVFormat(int cvformatId)
        {
            string sql = " delete from cvforamt_criterianotmet where cvformatid=?cvformatid;" +
                         " delete from cvformat_additionalinfo where cvformatid=?cvformatid;" +
                         "delete from cvformat_summarypoints where cvformatid=?cvformatid;" +
                         "delete from cvformat_childstatus where cvformatid=?cvformatid;" +
                         "delete from cvformat_languages where cvformatid=?cvformatid;" +
                         "delete from cvformat_nationality where cvformatid=?cvformatid;" +
                         "delete from cvformat where cvformatid=?cvformatid;";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("cvformatid", cvformatId));
        }

        public static void deleteCVFormatRelated(int cvformatId)
        {
            string sql = " delete from cvforamt_criterianotmet where cvformatid=?cvformatid;" +
                         " delete from cvformat_additionalinfo where cvformatid=?cvformatid;" +
                         "delete from cvformat_summarypoints where cvformatid=?cvformatid;" +
                         "delete from cvformat_childstatus where cvformatid=?cvformatid;" +
                         "delete from cvformat_languages where cvformatid=?cvformatid;" +
                         "delete from cvformat_nationality where cvformatid=?cvformatid;" +
                         "delete from cvformat_qualification where cvformatid=?cvformatid;";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("cvformatid", cvformatId));
        }

        public static MySqlDataReader getCVFormat(int cvformatId)
        {
            string sql = "select cv.cvformatid,candidateid,jobid,interface,Date_Format(interfaceDate,'%d-%m-%Y') as interfaceDate,interfacenote,location,visa,Date_Format(dob,'%d-%m-%Y') as dob_format,dob,nodob,dobformat,releventexperience," +
                " qualification,marital,lastsalarycurrency,lastsalary,lasttosalary,lastsalaryfrequency,lastsalarynote,expectsalarycurrency,expectsalary,expecttosalary,expectsalaryfrequency,expectsalarynote,availability,cs.status,cv.userid,u.username " +
                " from cvformat cv inner join cvformat_Status cs on cs.cvformatid=cv.cvformatid inner join users u on cv.userid=u.userid where cv.cvformatid=?cvformatid;" +
                " select cvformatid,countryid from cvformat_nationality where cvformatid=?cvformatid;" +
                " select cvformat_laguagesid,cvformatId,cl.languageid,name as language,spoken,written,listening,reading from cvformat_languages cl inner join languages l on cl.languageid=l.languageid where cvformatid=?cvformatid;" +
                " select cvformat_childstatusid,cvformatId,gender,age,relationshipsid from cvformat_childstatus where cvformatid=?cvformatid;" +
                " select cvformat_summarypointsid,cvformatId,notes,userid,created from cvformat_summarypoints where cvformatid=?cvformatid;" +
                " select cvformat_additionalinfoid,cvformatId,notes,userid,created from cvformat_additionalinfo where cvformatid=?cvformatid;" +
                " select cvforamt_criterianotmetid,criteriaid,criteria,criteriamet,reason,type from cvforamt_criterianotmet where cvformatid=?cvformatid;" +
                " select cvforamt_qualificationid,cvformatid,qualification from cvformat_qualification where cvformatid=?cvformatid;" +
                " select cvforamt_criterianotmetid,cvformatid,criteriaid,criteria as Description,criteriamet,reason from cvforamt_criterianotmet where type = 1 and cvformatid=?cvformatid;" +
                " select cvforamt_criterianotmetid,cvformatid,criteriaid,criteria as Description,criteriamet,reason from cvforamt_criterianotmet where type = 2 and cvformatid=?cvformatid";

            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("cvformatid", cvformatId));

            return reader;
        }

        public static string getCVFromatJobReferenceNo(int cvformatId)
        {
            string sql = "select referenceno from cvformat cv inner join jobdetail j on cv.jobid=j.jobdetailid where cvformatid=?cvformatid";
            return (string)DAO.ExecuteScalar(sql, new MySqlParameter("cvformatid", cvformatId));
        }

        public static MySqlDataReader searchCVFormat(string keyword, string sortOrder)
        {
            if (string.IsNullOrEmpty(sortOrder))
                sortOrder = "cvformatid asc";
            string sql = "SELECT cv.cvformatid,j.referenceno,c.candidateid,c.first,c.last,Date_Format(cv.created,'%d-%b-%Y-%T') as requesteddate ,u.username,cv.jobid,j.title as jobtitle,l.name as location," +
                        " case when cs.status=2 then Date_Format(cs.modified,'%d-%b-%Y-%T') else '' end as completeddate,cs.status," +
                        "case when cs.status=4 then 1 else cs.status end as sstatus, " +
                        " case when cs.status=2 then u1.username else '' end as completeuser,case when cs.status=2 then cs.modified else '' end as completed" +
                        " from cvformat cv inner join jobdetail j on cv.jobid=j.jobdetailid left join locations l on j.locationid=l.locationid inner join candidates c on cv.candidateid=c.candidateid" +
                        " inner join cvformat_status cs on cs.cvformatid=cv.cvformatid " +
                        " inner join users u on cv.userid=u.userid inner join users u1 on cs.userid=u1.userid " +
                        " where j.referenceno like concat_ws(?keyword,'%','%') or j.title like concat_ws(?keyword,'%','%') " +
                        " or c.first like concat_ws(?keyword,'%','%') or c.last like concat_ws(?keyword,'%','%') or c.middle like concat_ws(?keyword,'%','%') " +
                        " or c.candidateid like concat_ws(?keyword,'%','%') or u.username like concat_ws(?keyword,'%','%') order by " + sortOrder;

            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword));
            return reader;
        }

        public static MySqlDataReader searchCVFormatByConsultant(string keyword, string sortOrder, int consultantId)
        {
            if (string.IsNullOrEmpty(sortOrder))
                sortOrder = "cvformatid asc";
            string sql = "SELECT cv.cvformatid,j.referenceno,c.candidateid,c.first,c.last,Date_Format(cv.created,'%d-%b-%Y-%T') as requesteddate ,u.username,cv.jobid,j.title as jobtitle,l.name as location," +
                        " case when cs.status=2 then Date_Format(cs.modified,'%d-%b-%Y-%T') else '' end as completeddate,cs.status," +
                         "case when cs.status=4 then 1 else cs.status end as sstatus, " +
                        " case when cs.status=2 then u1.username else '' end as completeuser,case when cs.status=2 then cs.modified else '' end as completed" +
                        " from cvformat cv inner join jobdetail j on cv.jobid=j.jobdetailid inner join job_consultants jc on jc.jobid=j.jobdetailid and jc.consultantid=?consultantId " +
                        " left join locations l on j.locationid=l.locationid inner join candidates c on cv.candidateid=c.candidateid" +
                        " inner join cvformat_status cs on cs.cvformatid=cv.cvformatid " +
                        " inner join users u on cv.userid=u.userid inner join users u1 on cs.userid=u1.userid " +
                        " where j.referenceno like concat_ws(?keyword,'%','%') or j.title like concat_ws(?keyword,'%','%') " +
                        " or c.first like concat_ws(?keyword,'%','%') or c.last like concat_ws(?keyword,'%','%') or c.middle like concat_ws(?keyword,'%','%') " +
                        " or c.candidateid like concat_ws(?keyword,'%','%') or u.username like concat_ws(?keyword,'%','%') order by " + sortOrder;

            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword), new MySqlParameter("consultantId", consultantId));
            return reader;
        }

        public static MySqlDataReader getcvformatTooltip()
        {
            string sql = "select tooltip,label from cvformat_tooltips";

            MySqlDataReader reader = DAO.ExecuteReader(sql);
            return reader;
        }

        public static MySqlDataReader getCVFFormatRequestByCandidateAndJob(int candidateId, int jobId)
        {
            string sql = "SELECT cv.candidateid,cv.jobid,cv.cvformatid,cs.status,date_format(cs.modified,'%d-%b-%Y-%T') as modified,cv.userid " +
                " FROM  cvformat cv inner join cvformat_Status cs on cs.cvformatid=cv.cvformatid " +
                "  where cv.candidateid=?candidateId and cv.jobid=?jobId order by created asc";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("candidateId", candidateId), new MySqlParameter("jobId", jobId));

            return reader;
        }

        public static void updateCVFormatByCandidateId(uint candidateId, List<string[]> languages, List<uint> nationalities)
        {
            string sql = "update cvformat cv inner join candidates c on cv.candidateid=c.candidateid inner join  cvformat_status cs on cs.cvformatid=cv.cvformatid set cv.dob=c.dob,cv.marital=c.maritalstatus,cv.nodob=c.nodob,cv.dobformat=c.dobformat " +
                " where cv.candidateid=?candidateid and cs.status=1";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateId));

            sql = "select cv.cvformatid from cvformat cv inner join cvformat_Status cs on cs.cvformatid=cv.cvformatid where status=1 and cv.candidateid=?candidateid ";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("candidateid", candidateId));
            if (reader.HasRows)
            {
                int cvformatId;
                while (reader.Read())
                {
                    cvformatId = Convert.ToInt32(DAO.getString(reader, "cvformatid"));
                    sql = "delete from cvformat_languages where cvformatid=?cvformatid;" +
                        "delete from cvformat_nationality where cvformatid=?cvformatid;";
                    DAO.ExecuteNonQuery(sql, new MySqlParameter("cvformatid", cvformatId));

                    foreach (int countryid in nationalities)
                    {
                        insertNationality(countryid, cvformatId);
                    }

                    foreach (string[] language in languages)
                    {
                        uint candidatelanguageid = Convert.ToUInt32(language[0]);
                        int languageid = Convert.ToInt32(language[1]);
                        Int16 spoken = Convert.ToInt16(language[2]);
                        Int16 written = Convert.ToInt16(language[3]);
                        Int16 listening = Convert.ToInt16(language[4]);
                        Int16 reading = Convert.ToInt16(language[5]);

                        CVFormatLanguageInfo info = new CVFormatLanguageInfo();
                        info.CVFormatId = cvformatId;
                        info.LanguageId = languageid;
                        info.Spoken = spoken;
                        info.Written = written;
                        info.Listening = listening;
                        info.Reading = reading;
                        insertCVFormatLanguages(info);
                    }
                }
            }
            reader.Close();
            reader.Dispose();
        }

        public static void updateCVFormatLanguageByCandidateId(uint candidateid, uint languageid, uint spoken, uint written, uint listening, uint reading)
        {
            //string sql = "delete cl from cvformat_languages cl inner join cvformat c on cl.cvformatid=c.cvformatid inner join cvformat_status cs on cs.cvformatid=c.cvformatid where c.candidateid=?candidateid and cs.status=1;";
            //DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid));
            string sql = "select cv.cvformatid from cvformat cv inner join cvformat_Status cs on cs.cvformatid=cv.cvformatid where status=1 and cv.candidateid=?candidateid ";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("candidateid", candidateid));
            if (reader.HasRows)
            {
                int cvformatId;
                while (reader.Read())
                {
                    cvformatId = Convert.ToInt32(DAO.getString(reader, "cvformatid"));
                    CVFormatLanguageInfo info = new CVFormatLanguageInfo();
                    info.CVFormatId = cvformatId;
                    info.LanguageId = Convert.ToInt32(languageid);
                    info.Spoken = Convert.ToInt16(spoken);
                    info.Written = Convert.ToInt16(written);
                    info.Listening = Convert.ToInt16(listening);
                    info.Reading = Convert.ToInt16(reading);
                    insertCVFormatLanguages(info);
                }
            }
            reader.Close();
            reader.Dispose();
        }

        public static void removeCVFormatLanguageByCandidateId(uint candidateid, int languageid)
        {
            string sql = "delete cl from cvformat_languages cl inner join cvformat c on cl.cvformatid=c.cvformatid inner join cvformat_status cs on cs.cvformatid=c.cvformatid where c.candidateid=?candidateid and cl.languageid=?languageid and cs.status=1;";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("languageid", languageid));
        }
    }
}