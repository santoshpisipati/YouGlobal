using System.Web.Mvc;

namespace Sample.Web.ModalLogin.Controllers
{
    public class ContactController : BaseController
    {
        public override string DefaultViewName
        {
            get
            {
                return "ContactUsHtml";
            }
        }

        public ActionResult ContactUsHtml()
        {
            return base.View("ContactUsHtml");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SendQuery(FormCollection formCollection)
        {
            if (!base.ProcessUpload())
            {
                return RedirectToAction("MessageFailure", "Work");
            }
            return View("Thankyou");
        }

        public ActionResult Thankyou()
        {
            return base.View("Thankyou");
        }
    }
}