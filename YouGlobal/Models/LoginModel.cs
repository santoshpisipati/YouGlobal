using System.ComponentModel.DataAnnotations;

namespace Sample.Web.ModalLogin.Models
{
    public partial class RegisterModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Provide First Name")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "First Name Should be min 5 and max 20 length")]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Provide Last Name")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "First Name Should be min 4 and max 20 length")]
        public string LastName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Provide Eamil")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Please Provide Valid Email")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Provide password")]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string RegisterPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("RegisterPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string PinCode { get; set; }

        [Required(ErrorMessage = "Your must provide a PhoneNumber")]
        [Display(Name = "Home Phone")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Your must provide a PhoneNumber")]
        public string RegisterAs { get; set; }

        public string PhoneCode { get; set; }
        public string AreaCode { get; set; }
        public bool IsJobSeeker { get; set; }
        public bool IsEmployer { get; set; }
        public bool IsConsultant { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "LoginPassword")]
        public string LoginPassword { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public bool IsJobSeeker { get; set; }
        public bool IsEmployer { get; set; }
        public bool IsConsultant { get; set; }
    }

    public class RegisterLoginModel
    {
        public RegisterModel RegisterModel { get; set; }
        public LoginModel LoginModel { get; set; }
    }
}