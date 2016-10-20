using System.Collections.Generic;

namespace Sample.Web.ModalLogin.Classes
{
    public partial class Tier
    {
        public int TierId { get; set; }
        public string MenuName { get; set; }
        public string key { get; set; }
        public string title { get; set; }
        public string NavURL { get; set; }
        public List<Tier1> children { get; set; }
        public int ParentMenuID { get; set; }
        public bool expanded = true;
        public int ParentSubMenuID { get; set; }
        public int ParentSubSubMenuID { get; set; }
    }

    public class Tier1
    {
        public int Tier1Id { get; set; }
        public string title { get; set; }
        public string key { get; set; }
        public bool expanded = true;
        public string parentString { get; set; }
        public List<Tier2> children { get; set; }
    }

    public class Tier2
    {
        public int Tier2Id { get; set; }
        public List<Tier3> children { get; set; }
        public bool expanded = true;
        public string title { get; set; }
        public string parentString { get; set; }
        public string key { get; set; }
    }

    public class Tier3
    {
        public int Tier3Id { get; set; }
        public List<Tier4> Nodes { get; set; }
        public string key { get; set; }
        public bool expanded = true;
        public string parentString { get; set; }
        public string title { get; set; }
    }

    public class Tier4
    {
        public int Tier4Id { get; set; }
        public string key { get; set; }
        public string title { get; set; }
        public bool expanded = true;
        public string parentString { get; set; }
    }
}