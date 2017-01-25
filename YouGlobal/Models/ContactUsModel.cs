using System.Collections.Generic;
using System.Web.Mvc;

namespace Sample.Web.ModalLogin.Models
{
    public class ContactUsModel
    {
        public int Id { get; set; }
        public string LocationId { get; set; }
        public string Name { get; set; }
        public string ConsultantName { get; set; }
        public string Designation { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string telephoneNumber { get; set; }
        public string mobileNumber { get; set; }
        public string faxNumber { get; set; }
        public string email { get; set; }
        public string skype { get; set; }
        public string contactPerson { get; set; }
        public string AddressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string bioName { get; set; }
        public string bioText { get; set; }
        public string GeographicFamiliarity { get; set; }
        public string Languages { get; set; }
        public string IndustrySpecialisation { get; set; }
        public string FormalQualifications { get; set; }
        public string bioImage { get; set; }

        public SelectList CountryList { get; set; }
        public int CountryId { get; set; }
    }
}

public class CountryList
{
    public string Text { get; set; }
    public int Id { get; set; }
}