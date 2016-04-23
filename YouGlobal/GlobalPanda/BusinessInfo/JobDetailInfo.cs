using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Summary description for JobDetailInfo
/// </summary>
namespace GlobalPanda.BusinessInfo
{
    /// <summary>
    ///
    /// </summary>
    public class JobDetailInfo
    {
        /// <summary>
        /// Gets or sets the job edit identifier.
        /// </summary>
        /// <value>
        /// The job edit identifier.
        /// </value>
        public int JobEditId { get; set; }

        /// <summary>
        /// Gets or sets the job identifier.
        /// </summary>
        /// <value>
        /// The job identifier.
        /// </value>
        public int JobId { get; set; }

        /// <summary>
        /// Gets or sets the location identifier.
        /// </summary>
        /// <value>
        /// The location identifier.
        /// </value>
        public int LocationId { get; set; }

        /// <summary>
        /// Gets or sets the type identifier.
        /// </summary>
        /// <value>
        /// The type identifier.
        /// </value>
        [Required]
        [Display(Name = "Job Type Id")]
        public int TypeId { get; set; }

        /// <summary>
        /// Gets or sets the salary minimum.
        /// </summary>
        /// <value>
        /// The salary minimum.
        /// </value>
        public string SalaryMin { get; set; }

        /// <summary>
        /// Gets or sets the salary maximum.
        /// </summary>
        /// <value>
        /// The salary maximum.
        /// </value>
        public string SalaryMax { get; set; }

        /// <summary>
        /// Gets or sets the reference no.
        /// </summary>
        /// <value>
        /// The reference no.
        /// </value>
        [Required]
        [Display(Name = "Reference No")]
        public string ReferenceNo { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the sub heading.
        /// </summary>
        /// <value>
        /// The sub heading.
        /// </value>
        public string SubHeading { get; set; }

        /// <summary>
        /// Gets or sets the search title.
        /// </summary>
        /// <value>
        /// The search title.
        /// </value>
        public string SearchTitle { get; set; }

        /// <summary>
        /// Gets or sets the bullet1.
        /// </summary>
        /// <value>
        /// The bullet1.
        /// </value>
        public string Bullet1 { get; set; }

        /// <summary>
        /// Gets or sets the bullet2.
        /// </summary>
        /// <value>
        /// The bullet2.
        /// </value>
        public string Bullet2 { get; set; }

        /// <summary>
        /// Gets or sets the bullet3.
        /// </summary>
        /// <value>
        /// The bullet3.
        /// </value>
        public string Bullet3 { get; set; }

        /// <summary>
        /// Gets or sets the summary.
        /// </summary>
        /// <value>
        /// The summary.
        /// </value>
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets the content of the job.
        /// </summary>
        /// <value>
        /// The content of the job.
        /// </value>
        [Required]
        [Display(Name = "Job Content")]
        public string JobContent { get; set; }

        /// <summary>
        /// Gets or sets the website URL.
        /// </summary>
        /// <value>
        /// The website URL.
        /// </value>
        public string WebsiteURL { get; set; }

        /// <summary>
        /// Gets or sets the ad footer.
        /// </summary>
        /// <value>
        /// The ad footer.
        /// </value>
        public string AdFooter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is residency.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is residency; otherwise, <c>false</c>.
        /// </value>
        public bool IsResidency { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is approve.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is approve; otherwise, <c>false</c>.
        /// </value>
        public bool IsApprove { get; set; }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        public int ClientId { get; set; }

        /// <summary>
        /// Gets or sets the job industry identifier.
        /// </summary>
        /// <value>
        /// The job industry identifier.
        /// </value>
        public int JobIndustryId { get; set; }

        /// <summary>
        /// Gets or sets the job industry sub identifier.
        /// </summary>
        /// <value>
        /// The job industry sub identifier.
        /// </value>
        public int JobIndustrySubId { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        /// <value>
        /// The created by.
        /// </value>
        public int CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        /// <value>
        /// The created date.
        /// </value>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        /// <value>
        /// The modified by.
        /// </value>
        public int ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the modified date.
        /// </summary>
        /// <value>
        /// The modified date.
        /// </value>
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the essential criteria list.
        /// </summary>
        /// <value>
        /// The essential criteria list.
        /// </value>
        public List<EssentialCriteriaInfo> EssentialCriteriaList { get; set; }

        [Required]
        [Display(Name = "Must Have List")]
        /// <summary>
        /// Gets or sets the must have list.
        /// </summary>
        /// <value>
        /// The must have list.
        /// </value>
        public List<string> MustHaveList { get; set; }

        [Required]
        [Display(Name = "Nice To Have List")]
        /// <summary>
        /// Gets or sets the nice to have list.
        /// </summary>
        /// <value>
        /// The nice to have list.
        /// </value>
        public List<string> NiceToHaveList { get; set; }

        /// <summary>
        /// Gets or sets the desirable criteria list.
        /// </summary>
        /// <value>
        /// The desirable criteria list.
        /// </value>
        public List<DesirableCriteriaInfo> DesirableCriteriaList { get; set; }

        /// <summary>
        /// Gets or sets the consultant list.
        /// </summary>
        /// <value>
        /// The consultant list.
        /// </value>
        public List<JobConsultantInfo> ConsultantList { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [hot job].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [hot job]; otherwise, <c>false</c>.
        /// </value>
        public bool HotJob { get; set; }

        /// <summary>
        /// Gets or sets the salary currency.
        /// </summary>
        /// <value>
        /// The salary currency.
        /// </value>
        public string SalaryCurrency { get; set; }

        /// <summary>
        /// Gets or sets the salary frequency.
        /// </summary>
        /// <value>
        /// The salary frequency.
        /// </value>
        public int? SalaryFrequency { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="JobDetailInfo"/> is published.
        /// </summary>
        /// <value>
        ///   <c>true</c> if published; otherwise, <c>false</c>.
        /// </value>
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [internal change].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [internal change]; otherwise, <c>false</c>.
        /// </value>
        public bool InternalChange { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the isc o08 identifier.
        /// </summary>
        /// <value>
        /// The isc o08 identifier.
        /// </value>
        public int ISCO08Id { get; set; }

        /// <summary>
        /// Gets or sets the isic rev4 identifier.
        /// </summary>
        /// <value>
        /// The isic rev4 identifier.
        /// </value>
        public int ISICRev4Id { get; set; }

        /// <summary>
        /// Gets or sets the location list.
        /// </summary>
        /// <value>
        /// The location list.
        /// </value>
        public List<JobLocation> LocationList { get; set; }

        [Required]
        [Display(Name = "Location List")]
        /// <summary>
        /// Gets or sets the selected location list.
        /// </summary>
        /// <value>
        /// The selected location list.
        /// </value>
        public List<string> SelectedLocationList { get; set; }
      
        /// <summary>
        /// Gets or sets the selected location identifier list.
        /// </summary>
        /// <value>
        /// The selected location identifier list.
        /// </value>
        public List<string> SelectedLocationIDList { get; set; }

        /// <summary>
        /// Gets or sets the selected location type list.
        /// </summary>
        /// <value>
        /// The selected location type list.
        /// </value>
        public List<string> SelectedLocationTypeList { get; set; }

        [Required]
        [Display(Name = "NiceToHave")]
        /// <summary>
        /// Gets or sets the length of the nice to have list.
        /// </summary>
        /// <value>
        /// The length of the nice to have list.
        /// </value>
        public List<string> NiceToHaveListLength { get; set; }

        [Required]
        [Display(Name = "Duties List")]
        /// <summary>
        /// Gets or sets the duties list.
        /// </summary>
        /// <value>
        /// The duties list.
        /// </value>
        public List<string> DutiesList { get; set; }

        [Required]
        [Display(Name = "Remuneration")]
        /// <summary>
        /// Gets or sets the remuneration.
        /// </summary>
        /// <value>
        /// The remuneration.
        /// </value>
        public List<string> Remuneration { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class JobLocation
    {
        /// <summary>
        /// Gets or sets the jobdetail identifier.
        /// </summary>
        /// <value>
        /// The jobdetail identifier.
        /// </value>
        public int JobdetailId { get; set; }

        /// <summary>
        /// Gets or sets the location identifier.
        /// </summary>
        /// <value>
        /// The location identifier.
        /// </value>
        public int LocationId { get; set; }

        /// <summary>
        /// Gets or sets the locationtype.
        /// </summary>
        /// <value>
        /// The locationtype.
        /// </value>
        public int Locationtype { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        public string Location { get; set; }
    }
}