using System.Collections.Generic;

namespace Sample.Web.ModalLogin.Classes
{
    public partial class SiteMenu
    {
        public int MenuID { get; set; }
        public string MenuName { get; set; }

        public string value { get; set; }
        public string text { get; set; }
        public string NavURL { get; set; }
        public List<SiteMenuSub> nodes { get; set; }
        public int ParentMenuID { get; set; }
        public int ParentSubMenuID { get; set; }
        public int ParentSubSubMenuID { get; set; }
    }

    public class SiteMenuSub
    {
        public string text { get; set; }
        public string value { get; set; }
        public List<SiteMenuSubSub> nodes { get; set; }
    }

    public class SiteMenuSubSub
    {
        public List<SiteMenuSubSubSub> nodes { get; set; }
        public string text { get; set; }
        public string value { get; set; }
    }

    public class SiteMenuSubSubSub
    {
        public List<SiteMenuSubSubSubSub> nodes { get; set; }
        public string value { get; set; }
        public string text { get; set; }
    }
    public class SiteMenuSubSubSubSub
    {
        public string value { get; set; }
        public string text { get; set; }
    }
}