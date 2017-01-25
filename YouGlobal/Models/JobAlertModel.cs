using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YG_MVC.Models
{
    public class JobAlertModel
    {
        public int JobAlertId { get; set; }

        public int CandidateId { get; set; }

        public string CandidateGUID { get; set; }

        [Required(ErrorMessage = "Name Required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "SurName Required")]
        public string SurName { get; set; }

        [Required(ErrorMessage = "Email Required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Contact Number Required")]
        public string ContactNumber { get; set; }

        [Required(ErrorMessage = "Phone Code Required")]
        public string PhoneCode { get; set; }

        public List<int> IndustrySelect { get; set; }
        public List<int> IndustrySelectID { get; set; }
        public List<int> LocationSelect { get; set; }
        public List<string> SelectedLocationIDList { get; set; }
        public List<int> WorkTypeSelect { get; set; }
        public List<int> OccupationSelect { get; set; }
        public List<int> OccupationSelectID { get; set; }        
        public int MailFrequency { get; set; }
    }
}