using System.Collections.Generic;
using System.Data;
using System.Web;

namespace YG_MVC.Models
{
    public class UploadCVModel
    {
        public string FirstName { get; set; }

        public List<string> MiddleName { get; set; }

        public string LastName { get; set; }

        public List<string> PhoneList { get; set; }

        public string NickName { get; set; }

        public DataTable Nationality { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string BirthDate { get; set; }

        public HttpPostedFileBase Attachresume { get; set; }

        public HttpPostedFileBase Coverletteroptional { get; set; }

        public string SelectIndustry { get; set; }

        public string OtherIndustry { get; set; }
    }
}