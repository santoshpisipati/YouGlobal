using System;
using System.Data;

namespace GlobalPanda.BusinessInfo
{
    public class EmailTemplateInfo
    {
        public enum EmailTemplateStatus { Used = 1, Unused = 2, Assigned = 3, UnAssigned = 4 }

        public int email_templateid { get; set; }

        public string Subject { get; set; }

        public string Header { get; set; }

        public string Body { get; set; }

        public string Footer { get; set; }

        public int Status { get; set; }

        public DateTime CreatedDate { get; set; }

        public Int32 Version { get; set; }

        public int ParentId { get; set; }

        public static DataTable ToDataSource(Type type)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("value", typeof(Int32));
            dt.Columns.Add("text", typeof(String));

            foreach (Int32 value in Enum.GetValues(type))
            {
                dt.Rows.Add(new object[] { value, Enum.GetName(type, value) });
            }
            return dt;
        }
    }
}