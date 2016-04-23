using System;
using System.Collections.Generic;

/// <summary>
/// Summary description for EventInfo
/// </summary>
///
namespace GlobalPanda.BusinessInfo
{
    public class EventInfo
    {
        public int EventId { get; set; }

        public string EventName { get; set; }

        public string Description { get; set; }

        public DateTime EventDate { get; set; }

        public int Category { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public int UserId { get; set; }

        public bool Private { get; set; }

        public bool AllDay { get; set; }

        public int Remainder { get; set; }

        public bool Recurred { get; set; }

        public string TimezoneId { get; set; }

        public int RecurredType { get; set; }

        public DateTime StartActual { get; set; }

        public DateTime RecurredEndDate { get; set; }

        public List<EventPermissionInfo> PermissionList { get; set; }
    }

    public class EventPermissionInfo
    {
        public int PermissionId { get; set; }

        public int EventId { get; set; }

        public int UserRoleId { get; set; }

        public bool View { get; set; }

        public bool Edit { get; set; }
    }
}