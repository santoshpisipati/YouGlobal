using System;

namespace GlobalPanda.BusinessInfo
{
    /// <summary>
    /// Summary description for JobBoardJobInfo
    /// </summary>
    public class JobBoardJobInfo
    {
        public int ExportId { get; set; }

        public int JobBoardId { get; set; }

        public int JobId { get; set; }

        public string AdTitle { get; set; }

        public string AdText { get; set; }

        public int ClassificationId { get; set; }

        public string SaleType { get; set; }

        public int CountryId { get; set; }

        public int LocationId { get; set; }

        public int? MinSalary { get; set; }

        public int? MaxSalary { get; set; }

        public string ContactNo { get; set; }

        public string Email { get; set; }

        public string Longitude { get; set; }

        public string Latitude { get; set; }

        public DateTime ExpiryDate { get; set; }
    }
}