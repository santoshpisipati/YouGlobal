/// <summary>
/// Summary description for EssentialCriteriaInfo
/// </summary>
namespace GlobalPanda.BusinessInfo
{
    public class EssentialCriteriaInfo
    {
        public uint EssentialCriteriaId { get; set; }

        public uint JobId { get; set; }

        public string Description { get; set; }

        public uint AnswerLength { get; set; }

        public int SortOrder { get; set; }
    }
}