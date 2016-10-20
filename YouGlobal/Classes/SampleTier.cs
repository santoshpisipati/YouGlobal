using System.Collections.Generic;

namespace Sample.Web.ModalLogin.Classes
{
    public partial class SampleTier
    {
        public int TierId { get; set; }
        public string MenuName { get; set; }
        public string id { get; set; }
        public string text { get; set; }
        public string NavURL { get; set; }
        public List<SampleTier1> children { get; set; }
        public int ParentMenuID { get; set; }
        public bool expanded = true;
        public int ParentSubMenuID { get; set; }
        public int ParentSubSubMenuID { get; set; }
    }

    public class SampleTier1
    {
        public int Tier1Id { get; set; }
        public string text { get; set; }
        public string id { get; set; }
        public bool expanded = true;
        public string parentString { get; set; }
        public List<SampleTier2> children { get; set; }
    }

    public class SampleTier2
    {
        public int Tier2Id { get; set; }
        public List<SampleTier3> children { get; set; }
        public bool expanded = true;
        public string text { get; set; }
        public string parentString { get; set; }
        public string id { get; set; }
    }

    public class SampleTier3
    {
        public int Tier3Id { get; set; }
        public List<SampleTier4> Nodes { get; set; }
        public string id { get; set; }
        public bool expanded = true;
        public string parentString { get; set; }
        public string text { get; set; }
    }

    public class SampleTier4
    {
        public int Tier4Id { get; set; }
        public string id { get; set; }
        public string text { get; set; }
        public bool expanded = true;
        public string parentString { get; set; }
    }
}