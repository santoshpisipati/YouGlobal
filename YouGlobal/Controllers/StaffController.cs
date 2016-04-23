using System.Web.Mvc;

namespace Sample.Web.ModalLogin.Controllers
{
    public class StaffController : BaseController
    {
        // GET: Staff
        public ActionResult LookingForStaff()
        {
            return this.Index();
        }

        public ActionResult PositionDescription()
        {
            return base.View();
        }

        public override string DefaultViewName
        {
            get
            {
                return "LookingForStaff";
            }
        }
    }
}