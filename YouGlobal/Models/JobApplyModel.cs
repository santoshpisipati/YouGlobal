using System.Data;
using System.Web;

namespace YG_MVC.Models
{
    public class JobApplyModel
    {
        public int JobId { get; set; }

        public string JobTitle { get; set; }

        public string ReferenceNo { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string ContactNumber { get; set; }

        public string CoverLetter { get; set; }

        public DataTable EssentialCriteriaList { get; set; }

        public DataTable DesirableCriteriaList { get; set; }

        public HttpPostedFileBase Attachment { get; set; }

        public HttpPostedFileBase CoverLetterOptional { get; set; }
    }
}