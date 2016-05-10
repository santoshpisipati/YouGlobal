using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

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
    }
}