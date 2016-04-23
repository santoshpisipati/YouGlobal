using System;
using System.Collections.Generic;
using System.Data;

/// <summary>
/// Summary description for HistoryInfo
/// </summary>
///
namespace GlobalPanda.BusinessInfo
{
    public class HistoryInfo
    {
        public enum Module
        {
            Candidate = 1,
            candidates_emails,
            candidates_phonenumbers,
            candidates_addresses,
            candidates_othercontacts,
            candidates_qualifications,
            candidates_languages,
            candidates_softwares,
            candidates_workhistories,
            candidates_notes,
            candidates_files,
            candidates_disciplines,
            Client,
            client_emails,
            client_phonenumbers,
            client_addresses,
            client_representatives,
            Representaive,
            representatives_emails,
            representatives_phonenumbers,
            representatives_address,
            representatives_othercontacts,
            representatives_notes,
            Job,
            Essentialcriteria,
            Desirablecriteria,
            Consultant,
            consultants_emails,
            consultants_phonenumbers,
            User,
            Address,
            AddressType,
            AlternativeContacts,
            Countries,
            Currencies,
            Disciplines,
            Divisions,
            EmailTypes,
            FileTypes,
            Industries,
            Institutions,
            Languages,
            Locations,
            Offices,
            PhoneTypes,
            Occupational,
            Qualifications,
            Titles,
            Sectors,
            Software,
            candidate_jobs,
            client_consultants,
            jobalert_invite,
            consultant_address,
            consultant_ipaddress,
            Login,
            file_change,
            Relationships,
            CVFormat,
            Jobalert,
            Candidate_CurrentSalary,
            Candidate_ExpectedSalary
        }

        public enum ActionType { Add = 1, Edit, Delete }

        public enum FileUploadSource { CV = 1, Application = 2, Manual = 3, Import = 4 }

        public int HistoryId { get; set; }

        public uint UserId { get; set; }

        public int ModuleId { get; set; }

        public int TypeId { get; set; }

        public uint RecordId { get; set; }

        public DateTime ModifiedDate { get; set; }

        public uint ParentRecordId { get; set; }

        public List<HistoryDetailInfo> Details { get; set; }

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