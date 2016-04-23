using System;

/// <summary>
/// Summary description for EmailSentHistoryInfo
/// </summary>
///
namespace GlobalPanda.BusinessInfo
{
    public class EmailSentHistoryInfo
    {
        public int CanddiateId { get; set; }

        public int JobId { get; set; }

        public int EmailTemplateId { get; set; }

        public DateTime SendDate { get; set; }

        public int Version { get; set; }

        public string EmailId { get; set; }
    }
}