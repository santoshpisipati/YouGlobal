using System;
using System.Collections.Generic;

/// <summary>
/// Summary description for CVFormatInfo
/// </summary>
namespace GlobalPanda.BusinessInfo
{
    public class CVFormatInfo
    {
        public int CVFormatId { get; set; }

        public int CandidateId { get; set; }

        public int JobId { get; set; }

        public string Interfaced { get; set; }

        public DateTime? InterfacedDate { get; set; }

        public string InterfacedNote { get; set; }

        public string Location { get; set; }

        public string Visa { get; set; }

        public DateTime? DOB { get; set; }

        public bool NODOB { get; set; }

        public string DOBFormat { get; set; }

        public string ReleventExperience { get; set; }

        //public string Qualification { get; set; }

        public string Marital { get; set; }

        public string LastSalaryCurrency { get; set; }

        public string LastSalary { get; set; }

        public string LastToSalary { get; set; }

        public int LastSalaryFrequency { get; set; }

        public string LastSalaryNote { get; set; }

        public string ExpectSalaryCurrency { get; set; }

        public string ExpectSalary { get; set; }

        public string ExpectToSalary { get; set; }

        public int ExpectSalaryFrequency { get; set; }

        public string ExpectSalaryNote { get; set; }

        public string Availability { get; set; }

        public List<int> NationalityList { get; set; }

        public List<CVFormatLanguageInfo> LanguageList { get; set; }

        public List<ChildStatusInfo> ChildStatusList { get; set; }

        public List<SummaryPointInfo> SummaryPointList { get; set; }

        public List<AdditionalInfo> AdditionalInfoList { get; set; }

        public List<CriteriaNotMetInfo> CriteriaNotMetList { get; set; }

        public List<QualificationInfo> QualificationList { get; set; }
    }

    public class CVFormatLanguageInfo
    {
        public int CVFormatLanguageId { get; set; }

        public int CVFormatId { get; set; }

        public int LanguageId { get; set; }

        public string Language { get; set; }

        public Int16 Spoken { get; set; }

        public Int16 Written { get; set; }

        public Int16 Listening { get; set; }

        public Int16 Reading { get; set; }
    }

    public class ChildStatusInfo
    {
        public int ChildStatusId { get; set; }

        public int CVFormatId { get; set; }

        public int RelationshipId { get; set; }

        public string Relationship { get; set; }

        public string Gender { get; set; }

        public string Age { get; set; }
    }

    public class SummaryPointInfo
    {
        public int SummaryPointId { get; set; }

        public int CVFormatId { get; set; }

        public string Notes { get; set; }

        public int UserId { get; set; }

        public DateTime Created { get; set; }
    }

    public class AdditionalInfo
    {
        public int AdditionalInfoId { get; set; }

        public int CVFormatId { get; set; }

        public string Notes { get; set; }

        public int UserId { get; set; }

        public DateTime Created { get; set; }
    }

    public class CriteriaNotMetInfo
    {
        public int Id { get; set; }

        public int CVFormatId { get; set; }

        public int CriteriaId { get; set; }

        public string Criteria { get; set; }

        public string CriteriaMet { get; set; }

        public string Reason { get; set; }

        public Int16 Type { get; set; }
    }

    public class QualificationInfo
    {
        public int Id { get; set; }

        public int CVFormatId { get; set; }

        public string Qualification { get; set; }
    }
}