using MySql.Data.MySqlClient;
using Sample.Web.ModalLogin.Classes;
using System.Collections.Generic;
using System.Web.Mvc;
using YG_DataAccess;

namespace Sample.Web.ModalLogin.Controllers
{
    public class TreeViewController : Controller
    {
        // GET: TreeView
        public ActionResult Index()
        {
            return View(searchOccupationlist());
        }

        public ActionResult Sample()
        {
            return View();
        }

        public List<SiteMenu> searchOccupationlist()
        {
            KeyValuePair<string, string> kv = new KeyValuePair<string, string>();
            List<KeyValuePair<string, string>> lst = new List<KeyValuePair<string, string>>();
            List<string> result = new List<string>();
            MySqlDataReader drOccupation = CommonDataAccess.searchOccupation("Sales");
            List<SiteMenu> treeViewResult = new List<SiteMenu>();
            SiteMenu sitemenu = new SiteMenu();
            int menuId = 1;
            int ParentMenuID = 0;
            while (drOccupation.Read())
            {
                if (!result.Contains("> " + drOccupation["c1"].ToString() + " " + drOccupation["d1"].ToString()))
                {
                    ParentMenuID = menuId++;
                    result.Add("> " + ParentMenuID + " " + drOccupation["d1"].ToString());
                    sitemenu = new SiteMenu();
                    sitemenu.MenuID = ParentMenuID;
                    sitemenu.ParentMenuID = 0;
                    sitemenu.MenuName = drOccupation["d1"].ToString();
                    result.Add("> " + drOccupation["c1"].ToString() + " " + drOccupation["d1"].ToString());
                    kv = new KeyValuePair<string, string>(drOccupation["id1"].ToString() + " " + drOccupation["c1"].ToString(), "<span style='color:#9AC435'>> " + "Tier 1 -" + " " + drOccupation["d1"].ToString() + "</span>");
                    lst.Add(kv); treeViewResult.Add(sitemenu);
                }
                if (!result.Contains(">> " + drOccupation["c2"].ToString() + " " + drOccupation["d2"].ToString()))
                {
                    sitemenu = new SiteMenu();
                    sitemenu.MenuID = menuId++;
                    sitemenu.ParentMenuID = ParentMenuID;
                    ParentMenuID = sitemenu.MenuID;
                    sitemenu.MenuName = drOccupation["d2"].ToString();
                    result.Add(">> " + drOccupation["c2"].ToString() + " " + drOccupation["d2"].ToString());
                    kv = new KeyValuePair<string, string>(drOccupation["id2"].ToString() + " " + drOccupation["c2"].ToString(), "<span style='color:#124812'>>> " + "Tier 2 -" + " " + drOccupation["d2"].ToString() + "</span>");
                    lst.Add(kv); treeViewResult.Add(sitemenu);
                }
                if (!result.Contains(">>> " + drOccupation["c3"].ToString() + " " + drOccupation["d3"].ToString()))
                {
                    sitemenu = new SiteMenu();
                    sitemenu.MenuID = menuId++;
                    sitemenu.ParentMenuID = ParentMenuID;
                    ParentMenuID = sitemenu.MenuID;
                    sitemenu.MenuName = drOccupation["d3"].ToString();
                    result.Add(">>> " + drOccupation["c3"].ToString() + " " + drOccupation["d3"].ToString());
                    kv = new KeyValuePair<string, string>(drOccupation["id3"].ToString() + " " + drOccupation["c3"].ToString(), "<span style='color:#51C0EE '>>>> " + "Tier 3 -" + " " + drOccupation["d3"].ToString() + "</span>");
                    lst.Add(kv); treeViewResult.Add(sitemenu);
                }
                sitemenu = new SiteMenu();
                sitemenu.MenuID = menuId++;
                sitemenu.ParentMenuID = ParentMenuID;
                sitemenu.MenuName = drOccupation["d4"].ToString();
                result.Add(">>>> " + drOccupation["c4"].ToString() + " " + drOccupation["d4"].ToString());
                kv = new KeyValuePair<string, string>(drOccupation["id4"].ToString() + " " + drOccupation["c4"].ToString(), "<span style='color:#0000FF'>>>>> " + "Tier 4 -" + " " + drOccupation["d4"].ToString() + "</span>");
                lst.Add(kv);
                treeViewResult.Add(sitemenu);
            }
            drOccupation.Close();
            drOccupation.Dispose();
            sitemenu = new SiteMenu();
            sitemenu.text = "Any";
            treeViewResult.Insert(0, sitemenu);
            kv = new KeyValuePair<string, string>("0", "<span>- Any -</span>");
            lst.Insert(0, kv);
            return treeViewResult;
        }

        public JsonResult searchOccupation(string term)
        {
            KeyValuePair<string, string> kv = new KeyValuePair<string, string>();
            List<KeyValuePair<string, string>> lst = new List<KeyValuePair<string, string>>();
            List<string> result = new List<string>();
            MySqlDataReader drOccupation = CommonDataAccess.searchOccupation(term);
            List<SiteMenu> treeViewResult = new List<SiteMenu>();
            SiteMenuSub subItem = new SiteMenuSub();
            SiteMenuSubSub subSubItem = new SiteMenuSubSub();
            SiteMenuSubSubSub subSubSubItem = new SiteMenuSubSubSub();
            SiteMenu sitemenu = new SiteMenu();
            string menuName = string.Empty;
            int ParentMenuID = 0;
            while (drOccupation.Read())
            {
                if (!result.Contains("> " + drOccupation["c1"].ToString() + " " + drOccupation["d1"].ToString()))
                {
                    result.Add("> " + ParentMenuID + " " + drOccupation["d1"].ToString());
                    sitemenu = new SiteMenu();
                    sitemenu.MenuID = ParentMenuID;
                    sitemenu.ParentMenuID = 0;
                    sitemenu.nodes = new List<SiteMenuSub>();
                    sitemenu.value = drOccupation["id1"].ToString() + " " + drOccupation["c1"].ToString();
                    sitemenu.text = drOccupation["d1"].ToString();
                    result.Add("> " + drOccupation["c1"].ToString() + " " + drOccupation["d1"].ToString());
                    kv = new KeyValuePair<string, string>(drOccupation["id1"].ToString() + " " + drOccupation["c1"].ToString(), "<span style='color:#9AC435'>> " + "Tier 1 -" + " " + drOccupation["d1"].ToString() + "</span>");
                    lst.Add(kv);
                }
                if (!result.Contains(">> " + drOccupation["c2"].ToString() + " " + drOccupation["d2"].ToString()))
                {
                    if (sitemenu.nodes == null)
                    {
                        sitemenu.nodes = new List<SiteMenuSub>();
                        subItem.text = drOccupation["d2"].ToString();
                        subItem.value = drOccupation["id2"].ToString() + " " + drOccupation["c2"].ToString();
                        sitemenu.nodes.Add(subItem);
                    }
                    else
                    {
                        subItem = new SiteMenuSub();
                        subItem.value = drOccupation["id2"].ToString() + " " + drOccupation["c2"].ToString();
                        subItem.text = drOccupation["d2"].ToString();
                        sitemenu.nodes.Add(subItem);
                    }
                    result.Add(">> " + drOccupation["c2"].ToString() + " " + drOccupation["d2"].ToString());
                    kv = new KeyValuePair<string, string>(drOccupation["id2"].ToString() + " " + drOccupation["c2"].ToString(), "<span style='color:#124812'>>> " + "Tier 2 -" + " " + drOccupation["d2"].ToString() + "</span>");
                    lst.Add(kv);
                }
                if (!result.Contains(">>> " + drOccupation["c3"].ToString() + " " + drOccupation["d3"].ToString()))
                {
                    if (subItem.nodes == null)
                    {
                        subItem.nodes = new List<SiteMenuSubSub>();
                        subSubItem = new SiteMenuSubSub();
                        subSubItem.value = drOccupation["id3"].ToString() + " " + drOccupation["c3"].ToString();
                        subSubItem.text = drOccupation["d3"].ToString();
                        subItem.nodes.Add(subSubItem);
                    }
                    else
                    {
                        subSubItem = new SiteMenuSubSub();
                        subSubItem.value = drOccupation["id3"].ToString() + " " + drOccupation["c3"].ToString();
                        subSubItem.text = drOccupation["d3"].ToString();
                        if (subSubItem.nodes == null)
                        {
                            subItem.nodes = new List<SiteMenuSubSub>();
                            subItem.nodes.Add(subSubItem);
                        }
                        else
                        {
                            subItem.nodes.Add(subSubItem);
                        }
                    }
                    result.Add(">>> " + drOccupation["c3"].ToString() + " " + drOccupation["d3"].ToString());
                    kv = new KeyValuePair<string, string>(drOccupation["id3"].ToString() + " " + drOccupation["c3"].ToString(), "<span style='color:#51C0EE '>>>> " + "Tier 3 -" + " " + drOccupation["d3"].ToString() + "</span>");
                    lst.Add(kv);
                }
                if (subSubItem.nodes == null)
                {
                    subSubItem.nodes = new List<SiteMenuSubSubSub>();
                    subSubSubItem = new SiteMenuSubSubSub();
                    subSubSubItem.text = drOccupation["d4"].ToString();
                    subSubSubItem.value = drOccupation["id4"].ToString() + " " + drOccupation["c4"].ToString();
                    subSubItem.nodes.Add(subSubSubItem);
                }
                else
                {
                    subSubSubItem = new SiteMenuSubSubSub();
                    subSubSubItem.text = drOccupation["d4"].ToString();
                    subSubSubItem.value = drOccupation["id4"].ToString() + " " + drOccupation["c4"].ToString();
                    if (subSubItem.nodes == null)
                    {
                        subSubItem.nodes = new List<SiteMenuSubSubSub>();
                        subSubItem.nodes.Add(subSubSubItem);
                    }
                    else
                    {
                        subSubItem.nodes.Add(subSubSubItem);
                    }
                    result.Add(">>>> " + drOccupation["c4"].ToString() + " " + drOccupation["d4"].ToString());
                    kv = new KeyValuePair<string, string>(drOccupation["id4"].ToString() + " " + drOccupation["c4"].ToString(), "<span style='color:#0000FF'>>>>> " + "Tier 4 -" + " " + drOccupation["d4"].ToString() + "</span>");
                    lst.Add(kv);
                }
                if (sitemenu.text != menuName)
                {
                    menuName = sitemenu.text;
                    treeViewResult.Add(sitemenu);
                }
            }
            drOccupation.Close();
            drOccupation.Dispose();
            sitemenu = new SiteMenu();
            sitemenu.text = "- Any -";
            treeViewResult.Insert(0, sitemenu);
            kv = new KeyValuePair<string, string>("0", "<span>- Any -</span>");
            lst.Insert(0, kv);
            return Json(treeViewResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult searchIndustry(string term)
        {
            KeyValuePair<string, string> kv = new KeyValuePair<string, string>();
            List<KeyValuePair<string, string>> lst = new List<KeyValuePair<string, string>>();
            List<string> result = new List<string>();
            MySqlDataReader drIndustry = CommonDataAccess.searchIndustry(term);
            List<SiteMenu> treeViewResult = new List<SiteMenu>();
            SiteMenuSub subItem = new SiteMenuSub();
            SiteMenuSubSub subSubItem = new SiteMenuSubSub();
            SiteMenuSubSubSub subSubSubItem = new SiteMenuSubSubSub();
            SiteMenu sitemenu = new SiteMenu();
            string menuName = string.Empty;
            int ParentMenuID = 0;
            while (drIndustry.Read())
            {
                if (!result.Contains("> " + drIndustry["c1"].ToString() + " " + drIndustry["d1"].ToString()))
                {
                    sitemenu = new SiteMenu();
                    sitemenu.MenuID = ParentMenuID;
                    sitemenu.ParentMenuID = 0;
                    sitemenu.nodes = new List<SiteMenuSub>();
                    sitemenu.value = drIndustry["id1"].ToString() + " " + drIndustry["c1"].ToString();
                    sitemenu.text = drIndustry["d1"].ToString();
                    result.Add("> " + drIndustry["c1"].ToString() + " " + drIndustry["d1"].ToString());
                    kv = new KeyValuePair<string, string>(drIndustry["id1"].ToString() + " " + drIndustry["c1"].ToString(), "<span style='color:#9AC435'>> " + "Tier 1 -" + " " + drIndustry["d1"].ToString() + "</span>");
                    lst.Add(kv);
                }
                if (!result.Contains(">> " + drIndustry["c2"].ToString() + " " + drIndustry["d2"].ToString()))
                {
                    if (sitemenu.nodes == null)
                    {
                        sitemenu.nodes = new List<SiteMenuSub>();
                        subItem.text = drIndustry["d2"].ToString();
                        subItem.value = drIndustry["id2"].ToString() + " " + drIndustry["c2"].ToString() + " " + drIndustry["c1"].ToString();
                        sitemenu.nodes.Add(subItem);
                    }
                    else
                    {
                        subItem = new SiteMenuSub();
                        subItem.value = drIndustry["id2"].ToString() + " " + drIndustry["c2"].ToString() + " " + drIndustry["c1"].ToString();
                        subItem.text = drIndustry["d2"].ToString();
                        sitemenu.nodes.Add(subItem);
                    }
                    result.Add(">> " + drIndustry["c2"].ToString() + " " + drIndustry["d2"].ToString());
                    kv = new KeyValuePair<string, string>(drIndustry["id2"].ToString() + " " + drIndustry["c2"].ToString() + " " + drIndustry["c1"].ToString(), "<span style='color:#124812'>>> " + "Tier 2 -" + " " + drIndustry["d2"].ToString() + "</span>");
                    lst.Add(kv);
                }
                if (!result.Contains(">>> " + drIndustry["c3"].ToString() + " " + drIndustry["d3"].ToString()))
                {
                    if (subItem.nodes == null)
                    {
                        subItem.nodes = new List<SiteMenuSubSub>();
                        subSubItem = new SiteMenuSubSub();
                        subSubItem.value = drIndustry["id3"].ToString() + " " + drIndustry["c3"].ToString() + " " + drIndustry["c1"].ToString();
                        subSubItem.text = drIndustry["d3"].ToString();
                        subItem.nodes.Add(subSubItem);
                    }
                    else
                    {
                        subSubItem = new SiteMenuSubSub();
                        subSubItem.value = drIndustry["id3"].ToString() + " " + drIndustry["c3"].ToString() + " " + drIndustry["c1"].ToString();
                        subSubItem.text = drIndustry["d3"].ToString();
                        if (subSubItem.nodes == null)
                        {
                            subItem.nodes = new List<SiteMenuSubSub>();
                            subItem.nodes.Add(subSubItem);
                        }
                        else
                        {
                            subItem.nodes.Add(subSubItem);
                        }
                    }
                    result.Add(">>> " + drIndustry["c3"].ToString() + " " + drIndustry["d3"].ToString());
                    kv = new KeyValuePair<string, string>(drIndustry["id3"].ToString() + " " + drIndustry["c3"].ToString() + " " + drIndustry["c1"].ToString(), "<span style='color:#51C0EE'>>>> " + "Tier 3 -" + " " + drIndustry["d3"].ToString() + "</span>");
                    lst.Add(kv);
                }
                if (subSubItem.nodes == null)
                {
                    subSubItem.nodes = new List<SiteMenuSubSubSub>();
                    subSubSubItem = new SiteMenuSubSubSub();
                    subSubSubItem.text = drIndustry["d4"].ToString();
                    subSubSubItem.value = drIndustry["id4"].ToString() + " " + drIndustry["c4"].ToString() + " " + drIndustry["c1"].ToString();
                    subSubItem.nodes.Add(subSubSubItem);
                }
                else
                {
                    subSubSubItem = new SiteMenuSubSubSub();
                    subSubSubItem.text = drIndustry["d4"].ToString();
                    subSubSubItem.value = drIndustry["id4"].ToString() + " " + drIndustry["c4"].ToString() + " " + drIndustry["c1"].ToString();
                    if (subSubItem.nodes == null)
                    {
                        subSubItem.nodes = new List<SiteMenuSubSubSub>();
                        subSubItem.nodes.Add(subSubSubItem);
                    }
                    else
                    {
                        subSubItem.nodes.Add(subSubSubItem);
                    }
                    result.Add(">>>> " + drIndustry["c4"].ToString() + " " + drIndustry["d4"].ToString());
                    kv = new KeyValuePair<string, string>(drIndustry["id4"].ToString() + " " + drIndustry["c4"].ToString() + " " + drIndustry["c1"].ToString(), "<span style='color:#0000FF'>>>>> " + "Tier 4 -" + " " + drIndustry["d4"].ToString() + "</span>");
                    lst.Add(kv);
                }
                if (sitemenu.text != menuName)
                {
                    menuName = sitemenu.text;
                    treeViewResult.Add(sitemenu);
                }
            }
            drIndustry.Close();
            drIndustry.Dispose();
            sitemenu = new SiteMenu();
            sitemenu.text = "- Any -";
            treeViewResult.Insert(0, sitemenu);
            kv = new KeyValuePair<string, string>("0", "<span>- Any -</span>");
            lst.Insert(0, kv);
            return Json(treeViewResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchLocationWithId(string term)
        {
            KeyValuePair<string, string> kv = new KeyValuePair<string, string>();
            List<KeyValuePair<string, string>> lst = new List<KeyValuePair<string, string>>();
            List<string> result = new List<string>();
            MySqlDataReader drLocation = CommonDataAccess.searchLocation(term);
            List<SiteMenu> treeViewResult = new List<SiteMenu>();
            SiteMenuSub subItem = new SiteMenuSub();
            SiteMenuSubSub subSubItem = new SiteMenuSubSub();
            SiteMenuSubSubSub subSubSubItem = new SiteMenuSubSubSub();
            SiteMenuSubSubSubSub subSubSubSubItem = new SiteMenuSubSubSubSub();
            SiteMenu sitemenu = new SiteMenu();
            string menuName = string.Empty;
            int ParentMenuID = 0;
            while (drLocation.Read())
            {
                if (!string.IsNullOrEmpty(drLocation["name"].ToString()))
                {
                    if (!result.Contains("<span style='color:#9AC435'> > " + drLocation["name"].ToString() + "</span>"))
                    {
                        sitemenu = new SiteMenu();
                        sitemenu.MenuID = ParentMenuID;
                        sitemenu.ParentMenuID = 0;
                        sitemenu.nodes = new List<SiteMenuSub>();
                        sitemenu.value = drLocation["countryid"].ToString() + ":1";
                        sitemenu.text = drLocation["name"].ToString();
                        result.Add("<span style='color:#9AC435'> > " + drLocation["name"].ToString() + "</span>");
                        kv = new KeyValuePair<string, string>(drLocation["countryid"].ToString() + ":1", "<span style='color:#9AC435'> > " + drLocation["name"].ToString() + "</span>");
                        lst.Add(kv);
                    }
                }
                if (!string.IsNullOrEmpty(drLocation["locationname"].ToString()))
                {
                    if (!result.Contains("<span style='color:#124812'> >> " + drLocation["locationname"].ToString() + "</span>"))
                    {
                        if (sitemenu.nodes == null)
                        {
                            sitemenu.nodes = new List<SiteMenuSub>();
                            subItem.text = drLocation["locationname"].ToString();
                            subItem.value = drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + ":2";
                            sitemenu.nodes.Add(subItem);
                        }
                        else
                        {
                            subItem = new SiteMenuSub();
                            subItem.value = drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + ":2";
                            subItem.text = drLocation["locationname"].ToString();
                            sitemenu.nodes.Add(subItem);
                        }
                        result.Add("<span style='color:#124812'> >> " + drLocation["locationname"].ToString() + "</span>");
                        kv = new KeyValuePair<string, string>(drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + ":2", "<span style='color:#124812'> >> " + drLocation["locationname"].ToString() + "</span>");
                        lst.Add(kv);
                    }
                }
                if (!string.IsNullOrEmpty(drLocation["sublocation"].ToString()))
                {
                    if (!result.Contains("<span style='color:#51C0EE'> >>> " + drLocation["sublocation"].ToString() + "</span>"))
                    {
                        if (subItem.nodes == null)
                        {
                            subItem.nodes = new List<SiteMenuSubSub>();
                            subSubItem = new SiteMenuSubSub();
                            subSubItem.value = drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + ":3";
                            subSubItem.text = drLocation["sublocation"].ToString();
                            subItem.nodes.Add(subSubItem);
                        }
                        else
                        {
                            subSubItem = new SiteMenuSubSub();
                            subSubItem.value = drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + ":3";
                            subSubItem.text = drLocation["sublocation"].ToString();
                            if (subSubItem.nodes == null)
                            {
                                subItem.nodes = new List<SiteMenuSubSub>();
                                subItem.nodes.Add(subSubItem);
                            }
                            else
                            {
                                subItem.nodes.Add(subSubItem);
                            }
                        }
                        result.Add("<span style='color:#51C0EE'> >>> " + drLocation["sublocation"].ToString() + "</span>");
                        kv = new KeyValuePair<string, string>(drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + ":3", "<span style='color:#51C0EE'> >>> " + drLocation["sublocation"].ToString() + "</span>");
                        lst.Add(kv);
                    }
                }
                if (!string.IsNullOrEmpty(drLocation["subsublocation"].ToString()))
                {
                    if (!result.Contains("<span style='color:#0000FF'> >>>> " + drLocation["subsublocation"].ToString() + "</span>"))
                    {
                        if (subSubItem.nodes == null)
                        {
                            subSubItem.nodes = new List<SiteMenuSubSubSub>();
                            subSubSubItem = new SiteMenuSubSubSub();
                            subSubSubItem.text = drLocation["subsublocation"].ToString();
                            subSubSubItem.value = drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + "," + drLocation["subsublocationid"].ToString() + ":4";
                            subSubItem.nodes.Add(subSubSubItem);
                        }
                        else
                        {
                            subSubSubItem = new SiteMenuSubSubSub();
                            subSubSubItem.text = drLocation["subsublocation"].ToString();
                            subSubSubItem.value = drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + "," + drLocation["subsublocationid"].ToString() + ":4";
                            if (subSubItem.nodes == null)
                            {
                                subSubItem.nodes = new List<SiteMenuSubSubSub>();
                                subSubItem.nodes.Add(subSubSubItem);
                            }
                            else
                            {
                                subSubItem.nodes.Add(subSubSubItem);
                            }
                            result.Add("<span style='color:#0000FF'> >>>> " + drLocation["subsublocation"].ToString() + "</span>");
                            kv = new KeyValuePair<string, string>(drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + "," + drLocation["subsublocationid"].ToString() + ":4", "<span style='color:#0000FF'> >>>> " + drLocation["subsublocation"].ToString() + "</span>");
                            lst.Add(kv);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(drLocation["groupname"].ToString()))
                {
                    if (!result.Contains("<span style='color:#0000FF'> >>>> " + drLocation["groupname"].ToString() + "</span>"))
                    {
                        if (subSubSubItem.nodes == null)
                        {
                            subSubSubItem.nodes = new List<SiteMenuSubSubSubSub>();
                            subSubSubSubItem = new SiteMenuSubSubSubSub();
                            subSubSubSubItem.text = drLocation["groupname"].ToString();
                            subSubSubSubItem.value = drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + "," + drLocation["subsublocationid"].ToString() + "," + drLocation["location_groupid"].ToString() + ":5";
                            subSubSubItem.nodes.Add(subSubSubSubItem);
                        }
                        else
                        {
                            subSubSubSubItem = new SiteMenuSubSubSubSub();
                            subSubSubSubItem.text = drLocation["groupname"].ToString();
                            subSubSubSubItem.value = drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + "," + drLocation["subsublocationid"].ToString() + "," + drLocation["location_groupid"].ToString() + ":5";
                            if (subSubSubItem.nodes == null)
                            {
                                subSubSubItem.nodes = new List<SiteMenuSubSubSubSub>();
                                subSubSubItem.nodes.Add(subSubSubSubItem);
                            }
                            else
                            {
                                subSubSubItem.nodes.Add(subSubSubSubItem);
                            }
                            result.Add("<span style='color:#0000FF'> >>>> " + drLocation["groupname"].ToString() + "</span>");
                            kv = new KeyValuePair<string, string>(drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + "," + drLocation["subsublocationid"].ToString() + "," + drLocation["location_groupid"].ToString() + ":5", "<span style='color:#0000FF'> >>>> " + drLocation["subsublocation"].ToString() + "</span>");
                            lst.Add(kv);
                        }
                    }
                }
                if (sitemenu.text != menuName)
                {
                    menuName = sitemenu.text;
                    if (!string.IsNullOrEmpty(sitemenu.text))
                    {
                        treeViewResult.Add(sitemenu);
                    }
                }
            }
            sitemenu = new SiteMenu();
            sitemenu.text = "- Anywhere -";
            treeViewResult.Insert(0,sitemenu);
            drLocation.Close();
            drLocation.Dispose();
            kv = new KeyValuePair<string, string>("0", "<span>- Any -</span>");
            lst.Insert(0, kv);
            return Json(treeViewResult, JsonRequestBehavior.AllowGet);
        }
    }
}

public class Entity
{
    public int text { get; set; }
    public string Name { get; set; }
    public int parentId { get; set; }
}