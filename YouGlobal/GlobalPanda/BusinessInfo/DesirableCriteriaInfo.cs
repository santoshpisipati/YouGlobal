/// <summary>
/// Summary description for DesirableCriteriaInfo
/// </summary>
namespace GlobalPanda.BusinessInfo
{
    public class DesirableCriteriaInfo
    {
        public uint DesirableCriteriaId { get; set; }

        public uint JobId { get; set; }

        public string Description { get; set; }

        public uint AnswerLength { get; set; }

        public int SortOrder { get; set; }
    }
}