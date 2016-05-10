using System.Collections.Generic;

namespace Sample.Web.ModalLogin.Classes
{
    public partial class Tier
    {
        public int TierId { get; set; }
        public string MenuName { get; set; }
        public string value { get; set; }
        public string text { get; set; }
        public string NavURL { get; set; }
        public List<Tier1> nodes { get; set; }
        public int ParentMenuID { get; set; }
        public int ParentSubMenuID { get; set; }
        public int ParentSubSubMenuID { get; set; }
    }

    public class Tier1
    {
        public int Tier1Id { get; set; }
        public string text { get; set; }
        public string value { get; set; }
        public List<Tier2> nodes { get; set; }
    }

    public class Tier2
    {
        public int Tier2Id { get; set; }
        public List<Tier3> nodes { get; set; }
        public string text { get; set; }
        public string value { get; set; }
    }

    public class Tier3
    {
        public int Tier3Id { get; set; }
        public List<Tier4> Nodes { get; set; }
        public string value { get; set; }
        public string text { get; set; }
    }
    public class Tier4
    {
        public int Tier4Id { get; set; }
        public string value { get; set; }
        public string text { get; set; }
    }
}