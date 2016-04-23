using System;
using System.Collections.Generic;

namespace GlobalPanda.BusinessInfo
{
    /// <summary>
    /// Summary description for JobBoardInfo
    /// </summary>
    public class JobBoardInfo
    {
        public int JobBoardId { get; set; }

        public string BoardName { get; set; }

        public string BoardURL { get; set; }

        public string Notes { get; set; }

        public int UserId { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        public List<JobBoardIndustry> Industry { get; set; }

        public List<JobBoardLocation> Locations { get; set; }
    }

    public class JobBoardIndustry
    {
        public int Id { get; set; }

        public int JobBoardId { get; set; }

        public int ISICRev4Id { get; set; }
    }

    public class JobBoardLocation
    {
        public int Id { get; set; }

        public int JobBobardId { get; set; }

        public int LocationId { get; set; }

        public int LocationType { get; set; }
    }
}