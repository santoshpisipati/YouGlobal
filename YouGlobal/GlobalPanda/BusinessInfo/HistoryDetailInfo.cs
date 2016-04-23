/// <summary>
/// Summary description for HistoryDetailInfo
/// </summary>
///
namespace GlobalPanda.BusinessInfo
{
    public class HistoryDetailInfo
    {
        public int HistoryDetailId { get; set; }

        public int HistoryId { get; set; }

        public string ColumnName { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }
    }
}