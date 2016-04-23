using System;
using System.Collections.Generic;

/// <summary>
/// Summary description for JobAlertInfo
/// </summary>

namespace GlobalPanda.BusinessInfo
{
    public class JobAlertInfo
    {
        public int JobAlertId { get; set; }

        public int CandidateId { get; set; }

        public string CandidateFullName { get; set; }

        public int FrequencyId { get; set; }

        public string Email { get; set; }

        public uint EmailId { get; set; }

        public string PhoneNo { get; set; }

        public string PhoneCode { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool ImportedNewCandidate { get; set; }

        public bool ImportedExistingCandidate { get; set; }

        public bool Confirmed { get; set; }

        public List<JobAlertIndustry> IndustryList { get; set; }

        public List<JobAlertLocation> LocationList { get; set; }

        public List<JobAlertWorkType> WorkTypeList { get; set; }

        public List<JobAlertOccupation> OccupationList { get; set; }

        public string IndustryNameList { get; set; }

        public string OccupationNameList { get; set; }

        public string LocationNameList { get; set; }
    }

    public class JobAlertIndustry
    {
        public int AlertIndustryId { get; set; }

        public int JobAlertId { get; set; }

        public int CandidateId { get; set; }

        public int SubIndustryId { get; set; }

        public int ISICRev4Id { get; set; }
    }

    public class JobAlertLocation
    {
        public int AlertLocationId { get; set; }

        public int JobAlertId { get; set; }

        public int CandidateId { get; set; }

        public int LocationId { get; set; }

        public int LocationType { get; set; }
    }

    public class JobAlertWorkType
    {
        public int AlertWorkTypeId { get; set; }

        public int JobAlertID { get; set; }

        public int CandidateId { get; set; }

        public int WorkTypeId { get; set; }
    }

    public class JobAlertOccupation
    {
        public int AlertOccupationId { get; set; }

        public int JobAlertId { get; set; }

        public int CandidateId { get; set; }

        public int ISCO08Id { get; set; }
    }
}