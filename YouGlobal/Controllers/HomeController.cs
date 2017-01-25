using Sample.Web.ModalLogin.Models;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using YG_Business;

namespace Sample.Web.ModalLogin.Controllers
{
    public class HomeController : BaseController
    {
        public override string DefaultViewName
        {
            get
            {
                this.GetPhoneCodes();
                return "Home";
            }
        }

        public ActionResult AboutUs()
        {
            return View("AboutUs");
        }

        public ActionResult Error()
        {
            return View("Error");
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Home()
        {
            this.GetPhoneCodes();
            return this.Index();
        }

        public ActionResult Privacy()
        {
            string sQueryString = "https://api.goodrx.com/low-price?name=Lipitor&api_key=a600deac67";
            ASCIIEncoding encoder = new ASCIIEncoding();
            Byte[] code = encoder.GetBytes("ebSxjDDa91xae9jzmpHfLg==");
            HMAC hmSha256 = new HMACSHA256(code);
            Byte[] hashMe = encoder.GetBytes(sQueryString);
            Byte[] hmBytes = hmSha256.ComputeHash(hashMe);
            String signature = Convert.ToBase64String(hmBytes);
            signature = signature.Replace("+", "_");
            signature = signature.Replace("/", "_");
            signature = HttpUtility.UrlEncode(signature);
            string s = string.Format("{0}&sig={1}", sQueryString, signature);

            return View("Privacy");
        }

        public ActionResult TermsOfUse()
        {
            return View("TermsOfUse");
        }
        public ActionResult Settings()
        {
            ChangeEmail ce = new ChangeEmail();
            Member member = Logininfo.GetMemberDetails(Convert.ToInt32(Session["memberID"].ToString()));
            if (member.MemberId > 0)
            {
                ce.Email = member.EmailId;
            }
            return View(ce);
        }

        public ActionResult ChangeEmail(ChangeEmail changeEmail)
        {
            if (!ModelState.IsValid)
            {
                return View(changeEmail);
            }
            if (changeEmail != null)
            {
                Int32 memberID = Logininfo.GetMemberId(changeEmail.Email, "");
                if (memberID > 0)
                {
                    if (!string.IsNullOrEmpty(changeEmail.SecondaryEmail))
                    {
                        Int32 Id = Logininfo.UpdateMemberEmail(changeEmail.SecondaryEmail, memberID);
                        if (Id > 0)
                        {
                            SendMail(changeEmail.SecondaryEmail, "", false);
                            Session["username"] = null;
                            return RedirectToAction("Home", "Home");
                        }
                    }
                }
            }
            Session["username"] = null;
            return RedirectToAction("Home", "Home");
        }
    }
}