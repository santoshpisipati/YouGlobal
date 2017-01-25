using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using Sample.Web.ModalLogin.Classes;
using Sample.Web.ModalLogin.Helpers;
using Sample.Web.ModalLogin.Models;
using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using YG_Business;

namespace Sample.Web.ModalLogin.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            Session["username"] = null;
            if (returnUrl == "/Account/Settings")
            {
                return Redirect("~/Account/Settings");
            }
            else

                return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password, bool rememberme, bool IsJobSeeker, bool IsEmployer, bool IsConsultant, string returnUrl)
        {
            LoginModel model = new LoginModel();
            model.UserName = username;
            model.LoginPassword = password;
            model.RememberMe = rememberme;
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.UserName))
                {
                    Member member = new Member();
                    Login login = new Login();
                    if (!string.IsNullOrEmpty(model.LoginPassword))
                    {
                        login.EmailId = model.UserName;
                        login.Password = CryptorEngine.Encrypt(model.LoginPassword, true);
                        member = Logininfo.GetLoginDetails(login);
                        if (member != null)
                        {
                            if (member.MemberId > 0)
                            {
                                if (ModelState.IsValid)
                                {
                                    string userName = member.FirstName;
                                    Session["memberID"] = member.MemberId;
                                    Session["username"] = string.Format("Hello {0} !", userName);
                                    if (member.IsJobSeeker) { Session["loggedinas"] = "1"; }
                                    if (member.IsEmployer) { Session["loggedinas"] = "2"; }
                                    if (member.IsConsultant) { Session["loggedinas"] = "3"; }

                                    return Json(member);
                                }
                            }
                        }
                    }
                }
            }
            return Json(false);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> LoginJson(string username, string password, bool rememberme)
        {
            var result = await SignInManager.PasswordSignInAsync(username, password, rememberme, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return Json(true);

                case SignInStatus.LockedOut:
                case SignInStatus.RequiresVerification:
                case SignInStatus.Failure:
                    return Json(false);

                default:
                    break;
            }
            return Json(false);
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);

                case SignInStatus.LockedOut:
                    return View("Lockout");

                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ValidateCaptcha()
        {
            var response = Request["g-recaptcha-response"];
            //secret that was generated in key value pair
            const string secret = "6Le55gsTAAAAAC1e5EtYFXkkmW5uJH30aJcRysFI";

            var client = new WebClient();
            var reply =
                client.DownloadString(
                    string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));

            var captchaResponse = JsonConvert.DeserializeObject<ReCaptchaClass>(reply);

            //when response is false check for the error message
            if (!captchaResponse.Success)
            {
                if (captchaResponse.ErrorCodes.Count <= 0) return View();

                var error = captchaResponse.ErrorCodes[0].ToLower();
                switch (error)
                {
                    case ("missing-input-secret"):
                        ViewBag.Message = "The secret parameter is missing.";
                        break;

                    case ("invalid-input-secret"):
                        ViewBag.Message = "The secret parameter is invalid or malformed.";
                        break;

                    case ("missing-input-response"):
                        ViewBag.Message = "The response parameter is missing.";
                        break;

                    case ("invalid-input-response"):
                        ViewBag.Message = "The response parameter is invalid or malformed.";
                        break;

                    default:
                        ViewBag.Message = "Error occured. Please try again";
                        break;
                }
            }
            else
            {
                ViewBag.Message = "Valid";
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterJs(FormCollection data)
        {
            string errorMessage = "";
            var returnValue = new { Success = "", Message = "" };
            var FirstName = Request["RegisterModel.FirstName"];
            var LastName = Request["RegisterModel.LastName"];
            var Email = Request["RegisterModel.Email"];
            var RegisterPassword = Request["RegisterModel.RegisterPassword"];
            var ConfirmPassword = Request["RegisterModel.ConfirmPassword"];
            var PhoneCode = Request["RegisterModel.PhoneCode"];
            var AreaCode = Request["RegisterModel.AreaCode"];
            var PhoneNumber = Request["RegisterModel.PhoneNumber"];
            var RegisterAs = Request["RegisterModel.RegisterAs"];
            var PinCode = Request["RegisterModel.PinCode"];
            var response = Request["g-recaptcha-response"];
            //secret that was generated in key value pair
            const string secret = "6Le55gsTAAAAAC1e5EtYFXkkmW5uJH30aJcRysFI";

            var client = new WebClient();
            var reply =
                client.DownloadString(
                    string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));

            var captchaResponse = JsonConvert.DeserializeObject<ReCaptchaClass>(reply);

            //when response is false check for the error message
            if (!captchaResponse.Success)
            {
                if (captchaResponse.ErrorCodes == null)
                {
                    returnValue = new { Success = "False", Message = "Refresh Captcha" };
                    return Json(returnValue, JsonRequestBehavior.AllowGet);
                }
                if (captchaResponse.ErrorCodes != null && captchaResponse.ErrorCodes.Count <= 0)
                {
                    returnValue = new { Success = "False", Message = "Refresh Captcha" };
                    return Json(returnValue, JsonRequestBehavior.AllowGet);
                }
                var error = captchaResponse.ErrorCodes[0].ToLower();
                switch (error)
                {
                    case ("missing-input-secret"):
                        errorMessage = "The secret parameter is missing.";
                        returnValue = new { Success = "False", Message = errorMessage };
                        return Json(returnValue, JsonRequestBehavior.AllowGet);

                    case ("invalid-input-secret"):
                        errorMessage = "The secret parameter is invalid or malformed.";
                        returnValue = new { Success = "False", Message = errorMessage };
                        return Json(returnValue, JsonRequestBehavior.AllowGet);

                    case ("missing-input-response"):
                        errorMessage = "The response parameter is missing.";
                        returnValue = new { Success = "False", Message = errorMessage };
                        return Json(returnValue, JsonRequestBehavior.AllowGet);

                    case ("invalid-input-response"):
                        errorMessage = "The response parameter is invalid or malformed.";
                        returnValue = new { Success = "False", Message = errorMessage };
                        return Json(returnValue, JsonRequestBehavior.AllowGet);

                    default:
                        errorMessage = "Error occured. Please try again";
                        returnValue = new { Success = "False", Message = errorMessage };
                        return Json(returnValue, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ViewBag.Message = "Valid";
                if (ModelState.IsValid)
                {
                    RegisterModel model = new RegisterModel();
                    model.FirstName = FirstName;
                    model.LastName = LastName;
                    model.RegisterPassword = RegisterPassword;
                    model.PhoneCode = PhoneCode;
                    model.PhoneNumber = PhoneNumber;
                    model.RegisterAs = RegisterAs != null ? RegisterAs : "1";
                    model.PinCode = PinCode;
                    model.Email = Email;
                    model.AreaCode = AreaCode;
                    if (model != null)
                    {
                        Member member = new Member();
                        member.EmailId = model.Email;
                        member.IsJobSeeker = RegisterAs == "1" ? true : false;
                        member.IsEmployer = RegisterAs == "2" ? true : false;
                        member.IsConsultant = RegisterAs == "3" ? true : false;
                        member.FirstName = model.FirstName;
                        member.LastName = model.LastName;
                        string[] result = model.PhoneCode.Split(new char[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                        member.PhoneNo = !string.IsNullOrEmpty(model.PhoneNumber) ? string.Format("{0} {1} {2}", result[0], model.AreaCode, model.PhoneNumber) : "";
                        if (!string.IsNullOrEmpty(model.RegisterPassword))
                        {
                            member.Password = CryptorEngine.Encrypt(model.RegisterPassword, true);
                            member.CreatedOn = DateTime.Now;
                            member.isActive = true;
                            if (!string.IsNullOrEmpty(model.RegisterPassword))
                            {
                                int memberID = Logininfo.GetMemberId(model.Email, "");
                                if (memberID > 0)
                                {
                                    errorMessage = string.Format("Emailid {0} already exists.please try alternate email.", model.Email);
                                    returnValue = new { Success = "False", Message = errorMessage };
                                    return Json(returnValue, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    Logininfo.AddMember(member);
                                    TempData["notice"] = "Registration Successful.";
                                    returnValue = new { Success = "True", Message = "Registration Successful." };

                                    return Json(returnValue, JsonRequestBehavior.AllowGet);
                                }
                            }
                        }
                    }
                }
            }
            returnValue = new { Success = "False", Message = "Invalid Registration" };
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Recaptcha.RecaptchaControlMvc.CaptchaValidator]
        public ActionResult Register(RegisterLoginModel model, bool captchaValid, string captchaErrorMessage)
        {
            if (ModelState.IsValid)
            {
                if (model != null)
                {
                    if (!captchaValid)
                    {
                        ViewData["Message"] = "Invalid Captcha.";
                        ModelState.AddModelError("captcha", captchaErrorMessage);
                    }
                    else
                    {
                        Member member = new Member();
                        member.EmailId = model.RegisterModel.Email;
                        member.FirstName = model.RegisterModel.FirstName;
                        member.LastName = model.RegisterModel.LastName;
                        string[] result = model.RegisterModel.PhoneCode.Split(new char[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                        member.PhoneNo = !string.IsNullOrEmpty(model.RegisterModel.PhoneNumber) ? string.Format("{0} {1} {2}", result[0], model.RegisterModel.AreaCode, model.RegisterModel.PhoneNumber) : "";
                        if (!string.IsNullOrEmpty(model.RegisterModel.RegisterPassword))
                        {
                            member.Password = CryptorEngine.Encrypt(model.RegisterModel.RegisterPassword, true);
                            member.CreatedOn = DateTime.Now;
                            member.isActive = true;
                            if (!string.IsNullOrEmpty(model.RegisterModel.RegisterPassword))
                            {
                                int memberID = Logininfo.GetMemberId(model.RegisterModel.Email, "");
                                if (memberID > 0)
                                {
                                    TempData["notice"] = string.Format("Emailid {0} already exists.please try alternate email.", model.RegisterModel.Email);
                                }
                                else
                                {
                                    Logininfo.AddMember(member);
                                    TempData["notice"] = "Successfully registered";
                                    return RedirectToAction("Home", "Home");
                                }
                            }
                        }
                    }
                }
            }
            return Json(false);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(PasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model != null)
            {
                Int32 memberID = Logininfo.GetMemberId(model.EmailId, "");
                if (memberID > 0)
                {
                    if (!string.IsNullOrEmpty(model.NewPassword))
                    {
                        string newPassword = CryptorEngine.Encrypt(model.NewPassword, true);
                        Int32 Id = Logininfo.ResetPassword(newPassword, memberID);
                        if (Id > 0)
                        {
                            RegisterLoginModel login = new RegisterLoginModel();
                            login.LoginModel.UserName = model.EmailId;
                            login.LoginModel.LoginPassword = newPassword;
                            return RedirectToAction("Home", "Home");
                        }
                    }
                }
            }
            Session["username"] = null;
            return RedirectToAction("Home", "Home");
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);

                case SignInStatus.LockedOut:
                    return View("Lockout");

                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });

                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Logout()
        {
            Session["username"] = null;
            return Redirect("~/Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public override string DefaultViewName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion Helpers
    }
}