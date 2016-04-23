/// <summary>
/// Summary description for CalendarEventInfo
/// </summary>
///
namespace GlobalPanda.BusinessInfo
{
    public class CalendarEventInfo
    {
        public int EventId { get; set; }

        public string EventName { get; set; }

        public string Description { get; set; }

        public string EventDate { get; set; }

        //public int Category { get; set; }

        public string EndDate { get; set; }

        public bool Private { get; set; }

        public bool AllDay { get; set; }

        //public bool Remainder { get; set; }

        public bool Recurred { get; set; }

        public string TimezoneId { get; set; }

        public int RecurredType { get; set; }

        public string RecurredEndDate { get; set; }
    }
}