using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using YG_DataAccess;

namespace YG_Business
{
    public class Common
    {
        private string[] months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        public DataTable OccupationList()
        {
            DataTable dt = new DataTable();
            MySqlDataReader reader = CommonDataAccess.listOccupation();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public DataTable JobClassificationList()
        {
            DataTable dt = new DataTable();
            MySqlDataReader reader = CommonDataAccess.listJobClassification();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public DataTable JobIndustryList()
        {
            DataTable dt = new DataTable();
            MySqlDataReader reader = CommonDataAccess.listJobIndustry();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public DataTable JobIndustrySubList()
        {
            DataTable dt = new DataTable();
            MySqlDataReader reader = CommonDataAccess.listJobIndustrySub();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public DataTable LocationList()
        {
            DataTable dt = new DataTable();
            MySqlDataReader reader = CommonDataAccess.listLocations();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public DataTable WorkTypeList()
        {
            DataTable dt = new DataTable();
            MySqlDataReader reader = CommonDataAccess.listJobType();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public DataTable AlertFrequnecyList()
        {
            DataTable dt = new DataTable();
            MySqlDataReader reader = CommonDataAccess.listAlertFrequency();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        //public DataTable SearchCountryCode(string code)
        //{
        //    DataTable dt = new DataTable();
        //    MySqlDataReader reader = CommonDataAccess.searchCountryCode(code);
        //    dt.Load(reader);
        //    reader.Close();
        //    reader.Dispose();
        //    return dt;
        //}

        public List<string> SearchCountryCode(string code)
        {
            List<string> result = new List<string>();
            MySqlDataReader drCountryCode = CommonDataAccess.searchCountryCode(code);
            while (drCountryCode.Read())
            {
                result.Add(drCountryCode["Code"].ToString());
            }
            drCountryCode.Close();
            drCountryCode.Dispose();
            return result;
        }

        public List<KeyValuePair<string, string>> SearchLocation(string code)
        {
            KeyValuePair<string, string> kv = new KeyValuePair<string, string>();
            List<KeyValuePair<string, string>> lst = new List<KeyValuePair<string, string>>();
            List<string> result = new List<string>();
            MySqlDataReader drLocation = CommonDataAccess.searchLocation(code);
            while (drLocation.Read())
            {
                if (!string.IsNullOrEmpty(drLocation["name"].ToString()))
                {
                    if (!result.Contains("<span style='color:#9AC435'> > " + drLocation["name"].ToString() + "</span>"))
                    {
                        result.Add("<span style='color:#9AC435'> > " + drLocation["name"].ToString() + "</span>");
                        kv = new KeyValuePair<string, string>(drLocation["name"].ToString(), "<span style='color:#9AC435'> > " + drLocation["name"].ToString() + "</span>");
                        lst.Add(kv);
                    }
                }
                if (!string.IsNullOrEmpty(drLocation["locationname"].ToString()))
                {
                    if (!result.Contains("<span style='color:#124812'> >> " + drLocation["locationname"].ToString() + "</span>"))
                    {
                        result.Add("<span style='color:#124812'> >> " + drLocation["locationname"].ToString() + "</span>");
                        kv = new KeyValuePair<string, string>(drLocation["locationname"].ToString(), "<span style='color:#124812'> >> " + drLocation["locationname"].ToString() + "</span>");
                        lst.Add(kv);
                    }
                }
                if (!string.IsNullOrEmpty(drLocation["sublocation"].ToString()))
                {
                    if (!result.Contains("<span style='color:#51C0EE'> >>> " + drLocation["sublocation"].ToString() + "</span>"))
                    {
                        result.Add("<span style='color:#51C0EE'> >>> " + drLocation["sublocation"].ToString() + "</span>");
                        kv = new KeyValuePair<string, string>(drLocation["sublocation"].ToString(), "<span style='color:#51C0EE'> >>> " + drLocation["sublocation"].ToString() + "</span>");
                        lst.Add(kv);
                    }
                }
                if (!string.IsNullOrEmpty(drLocation["subsublocation"].ToString()))
                {
                    if (!result.Contains("<span style='color:#0000FF'> >>>> " + drLocation["subsublocation"].ToString() + "</span>"))
                    {
                        result.Add("<span style='color:#0000FF'> >>>> " + drLocation["subsublocation"].ToString() + "</span>");
                        kv = new KeyValuePair<string, string>(drLocation["subsublocation"].ToString(), "<span style='color:#0000FF'> >>>> " + drLocation["subsublocation"].ToString() + "</span>");
                        lst.Add(kv);
                    }
                }

                if (!string.IsNullOrEmpty(drLocation["groupname"].ToString()))
                {
                    if (!result.Contains("<span style='color:#9AC435'> > " + drLocation["groupname"].ToString() + "</span>"))
                    {
                        result.Add("<span style='color:#9AC435'> > " + drLocation["groupname"].ToString() + "</span>");
                        kv = new KeyValuePair<string, string>(drLocation["groupname"].ToString(), "<span style='color:#9AC435'> > " + drLocation["groupname"].ToString() + "</span>");
                        lst.Add(kv);
                    }
                }
            }
            drLocation.Close();
            drLocation.Dispose();
            //result.Insert(0, "- Anywhere -");
            kv = new KeyValuePair<string, string>("0", "<span>- Anywhere -</span>");
            lst.Insert(0, kv);
            return lst;
        }

        public List<KeyValuePair<string, string>> SearchLocationWithId(string code)
        {
            KeyValuePair<string, string> kv = new KeyValuePair<string, string>();
            List<KeyValuePair<string, string>> lst = new List<KeyValuePair<string, string>>();
            List<string> result = new List<string>();
            MySqlDataReader drLocation = CommonDataAccess.searchLocation(code);
            while (drLocation.Read())
            {
                if (!string.IsNullOrEmpty(drLocation["name"].ToString()))
                {
                    if (!result.Contains("<span style='color:#9AC435'> > " + drLocation["name"].ToString() + "</span>"))
                    {
                        result.Add("<span style='color:#9AC435'> > " + drLocation["name"].ToString() + "</span>");
                        kv = new KeyValuePair<string, string>(drLocation["countryid"].ToString() + ":1", "<span style='color:#9AC435'> > " + drLocation["name"].ToString() + "</span>");
                        lst.Add(kv);
                    }
                }
                if (!string.IsNullOrEmpty(drLocation["locationname"].ToString()))
                {
                    if (!result.Contains("<span style='color:#124812'> >> " + drLocation["locationname"].ToString() + "</span>"))
                    {
                        result.Add("<span style='color:#124812'> >> " + drLocation["locationname"].ToString() + "</span>");
                        kv = new KeyValuePair<string, string>(drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + ":2", "<span style='color:#124812'> >> " + drLocation["locationname"].ToString() + "</span>");
                        lst.Add(kv);
                    }
                }
                if (!string.IsNullOrEmpty(drLocation["sublocation"].ToString()))
                {
                    if (!result.Contains("<span style='color:#51C0EE'> >>> " + drLocation["sublocation"].ToString() + "</span>"))
                    {
                        result.Add("<span style='color:#51C0EE'> >>> " + drLocation["sublocation"].ToString() + "</span>");
                        kv = new KeyValuePair<string, string>(drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + ":3", "<span style='color:#51C0EE'> >>> " + drLocation["sublocation"].ToString() + "</span>");
                        lst.Add(kv);
                    }
                }
                if (!string.IsNullOrEmpty(drLocation["subsublocation"].ToString()))
                {
                    if (!result.Contains("<span style='color:#0000FF'> >>>> " + drLocation["subsublocation"].ToString() + "</span>"))
                    {
                        result.Add("<span style='color:#0000FF'> >>>> " + drLocation["subsublocation"].ToString() + "</span>");
                        kv = new KeyValuePair<string, string>(drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + "," + drLocation["subsublocationid"].ToString() + ":4", "<span style='color:#0000FF'> >>>> " + drLocation["subsublocation"].ToString() + "</span>");
                        lst.Add(kv);
                    }
                }
                if (!string.IsNullOrEmpty(drLocation["groupname"].ToString()))
                {
                    if (!result.Contains("<span style='color:#9AC435'> > " + drLocation["groupname"].ToString() + "</span>"))
                    {
                        result.Add("<span style='color:#9AC435'> > " + drLocation["groupname"].ToString() + "</span>");
                        kv = new KeyValuePair<string, string>(drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + "," + drLocation["subsublocationid"].ToString() + "," + drLocation["location_groupid"].ToString() + ":5", "<span style='color:#9AC435'> > " + drLocation["groupname"].ToString() + "</span>");
                        lst.Add(kv);
                    }
                }
            }
            drLocation.Close();
            drLocation.Dispose();
            //result.Insert(0, "- Anywhere -");
            kv = new KeyValuePair<string, string>("0", "<span>- Anywhere -</span>");
            lst.Insert(0, kv);
            return lst;
        }

        public List<KeyValuePair<string, string>> SearchLocationWithOutAny(string code)
        {
            KeyValuePair<string, string> kv = new KeyValuePair<string, string>();
            List<KeyValuePair<string, string>> lst = new List<KeyValuePair<string, string>>();
            List<string> result = new List<string>();
            MySqlDataReader drLocation = CommonDataAccess.searchLocation(code);
            while (drLocation.Read())
            {
                if (!string.IsNullOrEmpty(drLocation["name"].ToString()))
                {
                    if (!result.Contains("<span style='color:#9AC435'> > " + drLocation["name"].ToString() + "</span>"))
                    {
                        result.Add("<span style='color:#9AC435'> > " + drLocation["name"].ToString() + "</span>");
                        kv = new KeyValuePair<string, string>(drLocation["countryid"].ToString() + ":1", "<span style='color:#9AC435'> > " + drLocation["name"].ToString() + "</span>");
                        lst.Add(kv);
                    }
                }
                if (!string.IsNullOrEmpty(drLocation["locationname"].ToString()))
                {
                    if (!result.Contains("<span style='color:#124812'> >> " + drLocation["locationname"].ToString() + "</span>"))
                    {
                        result.Add("<span style='color:#124812'> >> " + drLocation["locationname"].ToString() + "</span>");
                        kv = new KeyValuePair<string, string>(drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + ":2", "<span style='color:#124812'> >> " + drLocation["locationname"].ToString() + "</span>");
                        lst.Add(kv);
                    }
                }
                if (!string.IsNullOrEmpty(drLocation["sublocation"].ToString()))
                {
                    if (!result.Contains("<span style='color:#51C0EE'> >>> " + drLocation["sublocation"].ToString() + "</span>"))
                    {
                        result.Add("<span style='color:#51C0EE'> >>> " + drLocation["sublocation"].ToString() + "</span>");
                        kv = new KeyValuePair<string, string>(drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + ":3", "<span style='color:#51C0EE'> >>> " + drLocation["sublocation"].ToString() + "</span>");
                        lst.Add(kv);
                    }
                }
                if (!string.IsNullOrEmpty(drLocation["subsublocation"].ToString()))
                {
                    if (!result.Contains("<span style='color:#0000FF'> >>>> " + drLocation["subsublocation"].ToString() + "</span>"))
                    {
                        result.Add("<span style='color:#0000FF'> >>>> " + drLocation["subsublocation"].ToString() + "</span>");
                        kv = new KeyValuePair<string, string>(drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + "," + drLocation["subsublocationid"].ToString() + ":4", "<span style='color:#0000FF'> >>>> " + drLocation["subsublocation"].ToString() + "</span>");
                        lst.Add(kv);
                    }
                }
                if (!string.IsNullOrEmpty(drLocation["groupname"].ToString()))
                {
                    if (!result.Contains("<span style='color:#9AC435'> > " + drLocation["groupname"].ToString() + "</span>"))
                    {
                        result.Add("<span style='color:#9AC435'> > " + drLocation["groupname"].ToString() + "</span>");
                        kv = new KeyValuePair<string, string>(drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + "," + drLocation["subsublocationid"].ToString() + "," + drLocation["location_groupid"].ToString() + ":5", "<span style='color:#9AC435'> > " + drLocation["groupname"].ToString() + "</span>");
                        lst.Add(kv);
                    }
                }
            }
            drLocation.Close();
            drLocation.Dispose();
            //result.Insert(0, "- Anywhere -");
            //kv = new KeyValuePair<string, string>("0", "<span>- Anywhere -</span>");
            // lst.Insert(0, kv);
            return lst;
        }

        public string GetLocationGroupDetails(int groupId)
        {
            string ids = string.Empty;
            MySqlDataReader dr = CommonDataAccess.getGroupLocations(groupId);
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    switch (DataAccess.getString(dr, "locationtype"))
                    {
                        case "1":
                            ids = DataAccess.getString(dr, "countryid") + ":" + DataAccess.getString(dr, "locationtype") + "," + ids;
                            break;

                        case "2":
                            ids = DataAccess.getString(dr, "countryid") + "," + DataAccess.getString(dr, "locationid") + ":" + DataAccess.getString(dr, "locationtype") + "," + ids;
                            break;

                        case "3":
                            ids = DataAccess.getString(dr, "countryid") + "," + DataAccess.getString(dr, "locationid") + "," + DataAccess.getString(dr, "sublocationid") + ":" + DataAccess.getString(dr, "locationtype") + "," + ids;
                            break;

                        case "4":
                            ids = DataAccess.getString(dr, "countryid") + "," + DataAccess.getString(dr, "locationid") + "," + DataAccess.getString(dr, "sublocationid") + "," + DataAccess.getString(dr, "subsublocationid") + ":" + DataAccess.getString(dr, "locationtype") + "," + ids;
                            break;
                    }
                    //ids = DataAccess.getString(dr, "countryid") + "," + DataAccess.getString(dr, "locationid") + "," + DataAccess.getString(dr, "sublocationid") + "," + DataAccess.getString(dr, "subsublocationid") + ":" + DataAccess.getString(dr, "locationtype") + "," + ids;
                }
            }

            return ids;
        }

        public List<string> GetLocationDetails(int groupId)
        {
            List<string> result = new List<string>();
            MySqlDataReader dr = CommonDataAccess.getLocations(groupId);
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    result.Add(DataAccess.getString(dr, "locationid"));
                }
            }
            return result;
        }

        public string getCountryId(int locationId)
        {
            string id = string.Empty;
            MySqlDataReader dr = CommonDataAccess.getcountryid(locationId);
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    id = DataAccess.getString(dr, "countryid");
                }
            }
            return id;
        }

        public List<string> GetCountryList(int groupId)
        {
            List<string> countrylist = new List<string>();
            MySqlDataReader dr = CommonDataAccess.getcountrylist(groupId);
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    countrylist.Add(DataAccess.getString(dr, "locationid"));
                }
            }
            return countrylist;
        }

        public List<KeyValuePair<string, string>> SearchOccupationWithAny(string code)
        {
            KeyValuePair<string, string> kv = new KeyValuePair<string, string>();
            List<KeyValuePair<string, string>> lst = new List<KeyValuePair<string, string>>();
            List<string> result = new List<string>();
            MySqlDataReader drOccupation = CommonDataAccess.searchOccupation(code);

            while (drOccupation.Read())
            {
                if (!result.Contains("> " + drOccupation["c1"].ToString() + " " + drOccupation["d1"].ToString()))
                {
                    result.Add("> " + drOccupation["c1"].ToString() + " " + drOccupation["d1"].ToString());

                    kv = new KeyValuePair<string, string>(drOccupation["id1"].ToString() + " " + drOccupation["c1"].ToString(), "<span style='color:#9AC435'>> " + "Tier 1 -" + " " + drOccupation["d1"].ToString() + "</span>");
                    lst.Add(kv);
                }
                if (!result.Contains(">> " + drOccupation["c2"].ToString() + " " + drOccupation["d2"].ToString()))
                {
                    result.Add(">> " + drOccupation["c2"].ToString() + " " + drOccupation["d2"].ToString());
                    kv = new KeyValuePair<string, string>(drOccupation["id2"].ToString() + " " + drOccupation["c2"].ToString(), "<span style='color:#124812'>>> " + "Tier 2 -" + " " + drOccupation["d2"].ToString() + "</span>");
                    lst.Add(kv);
                }
                if (!result.Contains(">>> " + drOccupation["c3"].ToString() + " " + drOccupation["d3"].ToString()))
                {
                    result.Add(">>> " + drOccupation["c3"].ToString() + " " + drOccupation["d3"].ToString());
                    kv = new KeyValuePair<string, string>(drOccupation["id3"].ToString() + " " + drOccupation["c3"].ToString(), "<span style='color:#51C0EE '>>>> " + "Tier 3 -" + " " + drOccupation["d3"].ToString() + "</span>");
                    lst.Add(kv);
                }

                result.Add(">>>> " + drOccupation["c4"].ToString() + " " + drOccupation["d4"].ToString());
                kv = new KeyValuePair<string, string>(drOccupation["id4"].ToString() + " " + drOccupation["c4"].ToString(), "<span style='color:#0000FF'>>>>> " + "Tier 4 -" + " " + drOccupation["d4"].ToString() + "</span>");
                lst.Add(kv);
            }
            drOccupation.Close();
            drOccupation.Dispose();

            kv = new KeyValuePair<string, string>("0", "<span>- Any -</span>");
            lst.Insert(0, kv);
            return lst;
        }

        public DataTable GetOccupation(string ids)
        {
            MySqlDataReader drOccupation = CommonDataAccess.getOccupation(ids);
            DataTable dtOccupation = new DataTable();
            dtOccupation.Columns.Add("isco08id");
            dtOccupation.Columns.Add("title");
            DataRow dr;
            while (drOccupation.Read())
            {
                dr = dtOccupation.NewRow();
                if (drOccupation["type"].ToString() == "1")
                {
                    dr[0] = drOccupation["isco08id"].ToString() + " " + drOccupation["groupcode"].ToString();
                    dr[1] = "> " + "Tier 1 -" + " " + drOccupation["title"].ToString();
                    dtOccupation.Rows.Add(dr);
                }
                if (drOccupation["type"].ToString() == "2")
                {
                    dr[0] = drOccupation["isco08id"].ToString() + " " + drOccupation["groupcode"].ToString();
                    dr[1] = ">> " + "Tier 2 -" + " " + drOccupation["title"].ToString();
                    dtOccupation.Rows.Add(dr);
                }
                if (drOccupation["type"].ToString() == "3")
                {
                    dr[0] = drOccupation["isco08id"].ToString() + " " + drOccupation["groupcode"].ToString();
                    dr[1] = ">>> " + "Tier 3 -" + " " + drOccupation["title"].ToString();
                    dtOccupation.Rows.Add(dr);
                }
                if (drOccupation["type"].ToString() == "4")
                {
                    dr[0] = drOccupation["isco08id"].ToString() + " " + drOccupation["groupcode"].ToString();
                    dr[1] = ">>>> " + "Tier 4 -" + " " + drOccupation["title"].ToString();
                    dtOccupation.Rows.Add(dr);
                }
            }
            drOccupation.Close();
            drOccupation.Dispose();

            return dtOccupation;
        }

        public List<KeyValuePair<string, string>> SearchOccupation(string code)
        {
            KeyValuePair<string, string> kv = new KeyValuePair<string, string>();
            List<KeyValuePair<string, string>> lst = new List<KeyValuePair<string, string>>();
            List<string> result = new List<string>();
            MySqlDataReader drOccupation = CommonDataAccess.searchOccupation(code);

            while (drOccupation.Read())
            {
                if (!result.Contains("> " + drOccupation["c1"].ToString() + " " + drOccupation["d1"].ToString()))
                {
                    result.Add("> " + drOccupation["c1"].ToString() + " " + drOccupation["d1"].ToString());

                    kv = new KeyValuePair<string, string>(drOccupation["id1"].ToString(), "<span style='color:#9AC435'>> " + "Tier 1 -" + " " + drOccupation["d1"].ToString() + "</span>");
                    lst.Add(kv);
                }
                if (!result.Contains(">> " + drOccupation["c2"].ToString() + " " + drOccupation["d2"].ToString()))
                {
                    result.Add(">> " + drOccupation["c2"].ToString() + " " + drOccupation["d2"].ToString());
                    kv = new KeyValuePair<string, string>(drOccupation["id2"].ToString(), "<span style='color:#124812'>>> " + "Tier 2 -" + " " + drOccupation["d2"].ToString() + "</span>");
                    lst.Add(kv);
                }
                if (!result.Contains(">>> " + drOccupation["c3"].ToString() + " " + drOccupation["d3"].ToString()))
                {
                    result.Add(">>> " + drOccupation["c3"].ToString() + " " + drOccupation["d3"].ToString());
                    kv = new KeyValuePair<string, string>(drOccupation["id3"].ToString(), "<span style='color:#51C0EE '>>>> " + "Tier 3 -" + " " + drOccupation["d3"].ToString() + "</span>");
                    lst.Add(kv);
                }

                result.Add(">>>> " + drOccupation["c4"].ToString() + " " + drOccupation["d4"].ToString());
                kv = new KeyValuePair<string, string>(drOccupation["id4"].ToString(), "<span style='color:#0000FF'>>>>> " + "Tier 4 -" + " " + drOccupation["d4"].ToString() + "</span>");
                lst.Add(kv);
            }
            drOccupation.Close();
            drOccupation.Dispose();

            return lst;
        }

        public List<KeyValuePair<string, string>> SearchIndustry(string code)
        {
            KeyValuePair<string, string> kv = new KeyValuePair<string, string>();
            List<KeyValuePair<string, string>> lst = new List<KeyValuePair<string, string>>();
            List<string> result = new List<string>();
            MySqlDataReader drOccupation = CommonDataAccess.searchIndustry(code);

            while (drOccupation.Read())
            {
                if (!result.Contains("> " + drOccupation["c1"].ToString() + " " + drOccupation["d1"].ToString()))
                {
                    result.Add("> " + drOccupation["c1"].ToString() + " " + drOccupation["d1"].ToString());
                    kv = new KeyValuePair<string, string>(drOccupation["id1"].ToString(), "<span style='color:#9AC435'>> " + "Tier 1 -" + " " + drOccupation["d1"].ToString() + "</span>");
                    lst.Add(kv);
                }
                if (!result.Contains(">> " + drOccupation["c2"].ToString() + " " + drOccupation["d2"].ToString()))
                {
                    result.Add(">> " + drOccupation["c2"].ToString() + " " + drOccupation["d2"].ToString());
                    kv = new KeyValuePair<string, string>(drOccupation["id2"].ToString(), "<span style='color:#124812'>>> " + "Tier 2 -" + " " + drOccupation["d2"].ToString() + "</span>");
                    lst.Add(kv);
                }
                if (!result.Contains(">>> " + drOccupation["c3"].ToString() + " " + drOccupation["d3"].ToString()))
                {
                    result.Add(">>> " + drOccupation["c3"].ToString() + " " + drOccupation["d3"].ToString());
                    kv = new KeyValuePair<string, string>(drOccupation["id3"].ToString(), "<span style='color:#51C0EE'>>>> " + "Tier 3 -" + " " + drOccupation["d3"].ToString() + "</span>");
                    lst.Add(kv);
                }

                result.Add(">>>> " + drOccupation["c4"].ToString() + " " + drOccupation["d4"].ToString());
                kv = new KeyValuePair<string, string>(drOccupation["id4"].ToString(), "<span style='color:#0000FF'>>>>> " + "Tier 4 -" + " " + drOccupation["d4"].ToString() + "</span>");
                lst.Add(kv);
            }
            drOccupation.Close();
            drOccupation.Dispose();

            return lst;
        }

        public List<KeyValuePair<string, string>> SearchIndustryWithAny(string code)
        {
            KeyValuePair<string, string> kv = new KeyValuePair<string, string>();
            List<KeyValuePair<string, string>> lst = new List<KeyValuePair<string, string>>();
            List<string> result = new List<string>();
            MySqlDataReader drOccupation = CommonDataAccess.searchIndustry(code);

            while (drOccupation.Read())
            {
                if (!result.Contains("> " + drOccupation["c1"].ToString() + " " + drOccupation["d1"].ToString()))
                {
                    result.Add("> " + drOccupation["c1"].ToString() + " " + drOccupation["d1"].ToString());
                    kv = new KeyValuePair<string, string>(drOccupation["id1"].ToString() + " " + drOccupation["c1"].ToString(), "<span style='color:#9AC435'>> " + "Tier 1 -" + " " + drOccupation["d1"].ToString() + "</span>");
                    lst.Add(kv);
                }
                if (!result.Contains(">> " + drOccupation["c2"].ToString() + " " + drOccupation["d2"].ToString()))
                {
                    result.Add(">> " + drOccupation["c2"].ToString() + " " + drOccupation["d2"].ToString());
                    kv = new KeyValuePair<string, string>(drOccupation["id2"].ToString() + " " + drOccupation["c2"].ToString() + " " + drOccupation["c1"].ToString(), "<span style='color:#124812'>>> " + "Tier 2 -" + " " + drOccupation["d2"].ToString() + "</span>");
                    lst.Add(kv);
                }
                if (!result.Contains(">>> " + drOccupation["c3"].ToString() + " " + drOccupation["d3"].ToString()))
                {
                    result.Add(">>> " + drOccupation["c3"].ToString() + " " + drOccupation["d3"].ToString());
                    kv = new KeyValuePair<string, string>(drOccupation["id3"].ToString() + " " + drOccupation["c3"].ToString() + " " + drOccupation["c1"].ToString(), "<span style='color:#51C0EE'>>>> " + "Tier 3 -" + " " + drOccupation["d3"].ToString() + "</span>");
                    lst.Add(kv);
                }

                result.Add(">>>> " + drOccupation["c4"].ToString() + " " + drOccupation["d4"].ToString());
                kv = new KeyValuePair<string, string>(drOccupation["id4"].ToString() + " " + drOccupation["c4"].ToString() + " " + drOccupation["c1"].ToString(), "<span style='color:#0000FF'>>>>> " + "Tier 4 -" + " " + drOccupation["d4"].ToString() + "</span>");
                lst.Add(kv);
            }
            drOccupation.Close();
            drOccupation.Dispose();

            kv = new KeyValuePair<string, string>("0", "<span>- Any -</span>");
            lst.Insert(0, kv);

            return lst;
        }

        public DataTable GetIndustry(string ids)
        {
            MySqlDataReader drIndustry = CommonDataAccess.getIndustry(ids);
            DataTable dtIndustry = new DataTable();
            dtIndustry.Columns.Add("isco08id");
            dtIndustry.Columns.Add("title");
            DataRow dr;
            while (drIndustry.Read())
            {
                dr = dtIndustry.NewRow();
                if (drIndustry["level"].ToString() == "1")
                {
                    dr[0] = drIndustry["isicRev4id"].ToString() + " " + drIndustry["code"].ToString();
                    dr[1] = "> " + "Tier 1 -" + " " + drIndustry["description"].ToString();
                    dtIndustry.Rows.Add(dr);
                }
                if (drIndustry["level"].ToString() == "2")
                {
                    dr[0] = drIndustry["isicRev4id"].ToString() + " " + drIndustry["code"].ToString();
                    dr[1] = ">> " + "Tier 2 -" + " " + drIndustry["description"].ToString();
                    dtIndustry.Rows.Add(dr);
                }
                if (drIndustry["level"].ToString() == "3")
                {
                    dr[0] = drIndustry["isicRev4id"].ToString() + " " + drIndustry["code"].ToString();
                    dr[1] = ">>> " + "Tier 3 -" + " " + drIndustry["description"].ToString();
                    dtIndustry.Rows.Add(dr);
                }
                if (drIndustry["level"].ToString() == "4")
                {
                    dr[0] = drIndustry["isicRev4id"].ToString() + " " + drIndustry["code"].ToString();
                    dr[1] = ">>>> " + "Tier 4 -" + " " + drIndustry["description"].ToString();
                    dtIndustry.Rows.Add(dr);
                }
            }
            drIndustry.Close();
            drIndustry.Dispose();

            return dtIndustry;
        }

        public List<string> SearchAlertOccupation(string code)
        {
            List<string> result = new List<string>();
            MySqlDataReader drOccupation = CommonDataAccess.searchOccupation(code);
            while (drOccupation.Read())
            {
                //if (drOccupation["type"].ToString() == "2")
                //    result.Add("&nbsp; " + drOccupation["title"].ToString());
                if (!result.Contains("> " + drOccupation["c1"].ToString() + " " + drOccupation["d1"].ToString()))
                {
                    result.Add("> " + drOccupation["c1"].ToString() + " " + drOccupation["d1"].ToString());
                }
                if (!result.Contains(">> " + drOccupation["c2"].ToString() + " " + drOccupation["d2"].ToString()))
                {
                    result.Add(">> " + drOccupation["c2"].ToString() + " " + drOccupation["d2"].ToString());
                }
                if (!result.Contains(">>> " + drOccupation["c3"].ToString() + " " + drOccupation["d3"].ToString()))
                {
                    result.Add(">>> " + drOccupation["c3"].ToString() + " " + drOccupation["d3"].ToString());
                }

                result.Add(">>>> " + drOccupation["c4"].ToString() + " " + drOccupation["d4"].ToString());
            }
            drOccupation.Close();
            drOccupation.Dispose();
            result.Insert(0, "- All -");
            return result;
        }

        public DataTable SearchOccupationDt(string code)
        {
            List<string> result = new List<string>();
            DataTable dt = new DataTable();
            dt.Columns.Add("isco08id");
            dt.Columns.Add("title");
            DataRow dr;
            MySqlDataReader drOccupation = CommonDataAccess.searchOccupation(code);
            //dt.Load(drOccupation);
            while (drOccupation.Read())
            {
                if (!result.Contains(">" + drOccupation["d1"].ToString()))
                {
                    result.Add(">" + drOccupation["d1"].ToString());
                    dr = dt.NewRow();
                    dr["isco08id"] = drOccupation["id1"].ToString();
                    dr["title"] = "> " + drOccupation["c1"].ToString() + " " + drOccupation["d1"].ToString();
                    dt.Rows.Add(dr);
                }
                if (!result.Contains(">>" + drOccupation["d2"].ToString()))
                {
                    result.Add(">>" + drOccupation["d2"].ToString());
                    dr = dt.NewRow();
                    dr["isco08id"] = drOccupation["id2"].ToString();
                    dr["title"] = ">> " + drOccupation["c2"].ToString() + " " + drOccupation["d2"].ToString();
                    dt.Rows.Add(dr);
                }
                if (!result.Contains(">>>" + drOccupation["d3"].ToString()))
                {
                    result.Add(">>>" + drOccupation["d3"].ToString());
                    dr = dt.NewRow();
                    dr["isco08id"] = drOccupation["id3"].ToString();
                    dr["title"] = ">>> " + drOccupation["c3"].ToString() + " " + drOccupation["d3"].ToString();
                    dt.Rows.Add(dr);
                }

                result.Add(drOccupation["d4"].ToString());
                dr = dt.NewRow();
                dr["isco08id"] = drOccupation["id4"].ToString();
                dr["title"] = ">>>> " + drOccupation["c4"].ToString() + " " + drOccupation["d4"].ToString();
                dt.Rows.Add(dr);
            }
            drOccupation.Close();
            drOccupation.Dispose();
            return dt;
        }

        public DataTable SearchClassification(string classification)
        {
            DataTable dt = new DataTable();
            MySqlDataReader reader = CommonDataAccess.searchJobIndustry(classification);
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public DataSet GetConsultantsLocations()
        {
            DataSet ds = new DataSet();
            MySqlDataReader reader = CommonDataAccess.getConsultantsLocations();
            ds.EnforceConstraints = false;
            ds.Load(reader, LoadOption.PreserveChanges, new string[2]);
            reader.Close();
            reader.Dispose();
            return ds;
        }

        public DataTable GetConsultantsByLocation(int locationId, int locationType, int countryid)
        {
            DataTable dt = new DataTable();
            MySqlDataReader reader = CommonDataAccess.getConsultantsByLocationId(locationId, locationType, countryid);
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public DataSet GetConsultantDetails(int consultantId)
        {
            DataSet ds = new DataSet();
            ds.EnforceConstraints = false;
            MySqlDataReader reader = CommonDataAccess.getConsultantProfile(consultantId);
            ds.Load(reader, LoadOption.PreserveChanges, new string[4]);
            reader.Close();
            reader.Dispose();
            return ds;
        }

        public string GetAddressHTML(uint addressid)
        {
            StringBuilder sb = new StringBuilder();
            using (MySqlDataReader drA = CommonDataAccess.getAddress(addressid))
            {
                uint line = 1;
                while (drA.Read())
                {
                    if (line != (uint)drA["line"])
                    {
                        sb.Append("<br />");
                        line = (uint)drA["line"];
                    }
                    if ((uint)drA["position"] > 1)
                    {
                        sb.Append("&nbsp;&nbsp;");
                    }
                    sb.Append(drA["value"]);
                }

                if (drA.NextResult())
                {
                    if (drA.Read())
                    {
                        sb.Append("<br />");
                        sb.Append(drA["name"]);
                    }
                }
            }
            return sb.ToString();
        }

        public bool CandidateFileExists(int candidateid, int filetype, int filesize, string filename)
        {
            return CommonDataAccess.CandidateFileExist(candidateid, filetype, filename, filesize);
        }

        public DataTable GetFileType(int filetypeid)
        {
            MySqlDataReader dr = CommonDataAccess.GetFileExtension(filetypeid);
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            dr.Dispose();
            return dt;
        }

        public DataTable TitleList()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("id");
            dt.Columns.Add("title");
            DataRow dr;
            MySqlDataReader reader = CommonDataAccess.listTitle();
            while (reader.Read())
            {
                dr = dt.NewRow();
                dr[0] = DataAccess.getString(reader, "title");
                dr[1] = DataAccess.getString(reader, "title");
                dt.Rows.Add(dr);
            }
            //dt.Load(reader);
            reader.Close();
            reader.Dispose();
            DataRow newRow = dt.NewRow();
            newRow[0] = "";
            newRow[1] = "-Select-";
            dt.Rows.InsertAt(newRow, 0);
            return dt;
        }

        public DataTable GenderList()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("id");
            dt.Columns.Add("text");

            DataRow dr = dt.NewRow();

            dr["text"] = "-Select-";
            dr["id"] = "";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["text"] = "Male";
            dr["id"] = "M";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["text"] = "Female";
            dr["id"] = "F";

            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["text"] = "Unknown";
            dr["id"] = "U";
            dt.Rows.Add(dr);

            return dt;
        }

        public DataTable MaritalList()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("text");
            dt.Columns.Add("id");
            DataRow dr = dt.NewRow();
            dr["text"] = "-Select-";
            dr["id"] = "";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["text"] = "Married";
            dr["id"] = "M";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["text"] = "Single";
            dr["id"] = "S";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["text"] = "Divorced";
            dr["id"] = "D";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["text"] = "Widowed";
            dr["id"] = "W";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["text"] = "De Facto";
            dr["id"] = "F";

            dt.Rows.Add(dr);
            return dt;
        }

        public DataTable PhoneTypeList()
        {
            DataTable dt = new DataTable();
            MySqlDataReader reader = CommonDataAccess.listPhoneNumberTypes();
            dt.Load(reader);
            DataRow dr = dt.NewRow();
            dr["phonenumbertypeid"] = -1;
            dr["name"] = "-Select-";
            dt.Rows.InsertAt(dr, 0);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public DataTable CurrencyList()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("currencyid");
            dt.Columns.Add("name");
            MySqlDataReader reader = CommonDataAccess.listCurrencies();
            DataRow dr;
            while (reader.Read())
            {
                dr = dt.NewRow();
                dr[0] = DataAccess.getString(reader, "currencyid");
                dr[1] = DataAccess.getString(reader, "currencyid");
                dt.Rows.Add(dr);
            }
            reader.Close();
            reader.Dispose();
            dr = dt.NewRow();
            dr[0] = "";
            dr[1] = "-Select-";
            dt.Rows.InsertAt(dr, 0);
            return dt;
        }

        public DataTable FrequencyList()
        {
            DataTable dt = new DataTable();
            MySqlDataReader reader = CommonDataAccess.listFrequency();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            DataRow dr = dt.NewRow();
            dr[0] = -1;
            dr[1] = "-Select-";
            dt.Rows.InsertAt(dr, 0);
            return dt;
        }

        public DataTable EmailTypeList()
        {
            DataTable dt = new DataTable();
            MySqlDataReader reader = CommonDataAccess.listEmailTypes();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            DataRow dr = dt.NewRow();
            dr[0] = 0;
            dr[1] = "-Select-";
            dt.Rows.InsertAt(dr, 0);
            return dt;
        }

        public List<KeyValuePair<string, string>> SearchNation(string code)
        {
            KeyValuePair<string, string> kv = new KeyValuePair<string, string>();
            List<KeyValuePair<string, string>> lst = new List<KeyValuePair<string, string>>();
            MySqlDataReader drNation = CommonDataAccess.searchActiveCountries(code);
            while (drNation.Read())
            {
                kv = new KeyValuePair<string, string>(drNation["countryid"].ToString(), drNation["name"].ToString());
                lst.Add(kv);
            }
            return lst;
        }

        public List<KeyValuePair<string, string>> SearchEmployer(string code)
        {
            KeyValuePair<string, string> kv = new KeyValuePair<string, string>();
            List<KeyValuePair<string, string>> lst = new List<KeyValuePair<string, string>>();
            MySqlDataReader drEmployer = CommonDataAccess.SearchEmployer(code);
            while (drEmployer.Read())
            {
                kv = new KeyValuePair<string, string>(drEmployer["employerid"].ToString(), drEmployer["employername"].ToString());
                lst.Add(kv);
            }
            drEmployer.Close();
            drEmployer.Dispose();
            return lst;
        }

        public DataTable GetCountryByIds(string ids)
        {
            DataTable dt = new DataTable();
            MySqlDataReader dr = CommonDataAccess.getCountryByIds(ids);
            dt.Load(dr);
            dr.Close();
            dr.Dispose();

            return dt;
        }

        public DateTime dateFromDisplay(string display)
        {
            string[] split = display.Split(",".ToCharArray());
            DateTime dte = new DateTime(Convert.ToInt32(split[0]), 1, 1);
            if (split[1].Length > 0)
            {
                if (split[1] != "")
                {
                    dte = dte.AddMonths(Array.IndexOf<string>(months, split[1]));
                    if (split[2].Length > 0)
                    {
                        dte = dte.AddDays(Convert.ToInt32(split[2]) - 1);
                    }
                }
            }
            return dte;
        }

        public DataTable Tooltips()
        {
            MySqlDataReader dr = CommonDataAccess.getTooltip();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            dr.Dispose();
            return dt;
        }

        public static MySqlDataReader listCountries()
        {
            CommonDataAccess cda = new CommonDataAccess();
            MySqlDataReader reader = cda.listCountries();
            return reader;
        }

        public static MySqlDataReader GetCities(int cityId)
        {
            CommonDataAccess cda = new CommonDataAccess();
            MySqlDataReader reader = cda.GetCities(cityId);
            return reader;
        }

        public static MySqlDataReader GetConsultants(int cityId)
        {
            CommonDataAccess cda = new CommonDataAccess();
            MySqlDataReader reader = cda.GetConsultants(cityId);
            return reader;
        }

        public static MySqlDataReader GetConsultantsfromInfo(int cityId)
        {
            CommonDataAccess cda = new CommonDataAccess();
            MySqlDataReader reader = cda.GetConsultantsfromInfo(cityId);
            return reader;
        }

        public static MySqlDataReader GetContactDetails(int cityId)
        {
            CommonDataAccess cda = new CommonDataAccess();
            MySqlDataReader reader = cda.GetContactDetails(cityId);
            return reader;
        }
    }
}