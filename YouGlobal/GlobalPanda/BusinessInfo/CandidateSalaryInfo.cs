using System;

/// <summary>
/// Summary description for CandidateSalaryInfo
/// </summary>
namespace GlobalPanda.BusinessInfo
{
    public class CandidateSalaryInfo
    {
        public int CandidateId { get; set; }

        public int MinAmount { get; set; }

        public int MaxAmount { get; set; }

        public string Currency { get; set; }

        public int Frequency { get; set; }

        public int SalaryType { get; set; }

        public DateTime? SalaryDate { get; set; }
    }
}