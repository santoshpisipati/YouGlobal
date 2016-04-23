using System;

namespace YG_MVC.Models
{
    public class JobModel
    {
        public int JobId { get; set; }

        public int LocationId { get; set; }

        public int TypeId { get; set; }

        public string SalaryMin { get; set; }

        public string SalaryMax { get; set; }

        public string ReferenceNo { get; set; }

        public string Title { get; set; }

        public string SubHeading { get; set; }

        public string SearchTitle { get; set; }

        public string Bullet1 { get; set; }

        public string Bullet2 { get; set; }

        public string Bullet3 { get; set; }

        public string Summary { get; set; }

        public string JobContent { get; set; }

        public string WebsiteURL { get; set; }

        public string AdFooter { get; set; }

        public bool IsResidency { get; set; }

        public int Status { get; set; }

        public bool IsApprove { get; set; }

        public int ClientId { get; set; }

        public int JobIndustryId { get; set; }

        public int JobIndustrySubId { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        //public List<EssentialCriteriaInfo> EssentialCriteriaList { get; set; }

        //public List<DesirableCriteriaInfo> DesirableCriteriaList { get; set; }

        //public List<JobConsultantInfo> ConsultantList { get; set; }

        public bool HotJob { get; set; }

        public string SalaryCurrency { get; set; }

        public int SalaryFrequency { get; set; }

        public string Location { get; set; }
    }
}