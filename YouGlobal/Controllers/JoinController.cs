using System.Web.Mvc;

namespace Sample.Web.ModalLogin.Controllers
{
    public class JoinController : Controller
    {
        // GET: Join
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult JoinUsHtml()
        {
            return base.View("JoinUsHtml");
        }
    }
}