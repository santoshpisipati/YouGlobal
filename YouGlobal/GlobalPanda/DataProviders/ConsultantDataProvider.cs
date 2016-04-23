using GlobalPanda.BusinessInfo;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;

/// <summary>
/// Summary description for MenuItemDataProvider
/// </summary>
namespace GlobalPanda.DataProviders
{
    public class ConsultantDataProvider
    {
        public static uint insertConsultant(uint tenantid, string title, string first, string last, string nickname, string email, uint officeid, string username, string password, string jobcode)
        {
            uint userid = UserDataProvider.insertUser(username, password, (uint)UserRoleDataProvider.UserRole.Consultant, tenantid);
            string sql = "insert into consultants (title, first, last,nickname, officeid, userid,jobcode) values (?title, ?first, ?last,?nickname, ?officeid, ?userid,?jobcode); select last_insert_id()";
            uint consultantid = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("title", title), new MySqlParameter("first", first), new MySqlParameter("last", last), new MySqlParameter("nickname", nickname),
                new MySqlParameter("officeid", officeid), new MySqlParameter("userid", userid), new MySqlParameter("jobcode", jobcode)));

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo historyInfo = new HistoryInfo();
            historyInfo.UserId = GPSession.UserId;
            historyInfo.ModuleId = (int)HistoryInfo.Module.Consultant;
            historyInfo.TypeId = (int)HistoryInfo.ActionType.Add;
            historyInfo.RecordId = consultantid;
            historyInfo.ParentRecordId = consultantid;
            historyInfo.ModifiedDate = DateTime.Now;
            historyInfo.Details = new List<HistoryDetailInfo>();
            historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "All", NewValue = "title:" + title + " ,first:" + first + " ,last:" + last + " ,nickname:" + nickname + " ,officeid:" + officeid + " ,username:" + username + " ,password:" + password });
            history.insertHistory(historyInfo);

            addEmail(consultantid, email, 1, true);
            return consultantid;
        }

        public static void updateConsultantImagePath(uint consultantid, string imagepath)
        {
            string sql = "update consultants set imagepath=?imagepath where consultantid=?consultantid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("consultantid", consultantid), new MySqlParameter("imagepath", imagepath));
        }

        public static void updateConsultant(uint consultantid, string title, string first, string last, string nickname, uint officeid, string jobcode, bool displayatwebsite, string imagepath, string geographicfamiliarity,
            string industryspecialisation, string designation, string profileinfo, string skype, string languages, string qualifications, bool active)
        {
            string sql = "select c.consultantid, c.title, c.first, c.last,c.nickname, c.officeid, c.userid, u.username, c.jobcode, c.displayatwebsite, c.imagepath, c.geographicfamiliarity, c.industryspecialisation, c.designation, c.profileinfo,c.skype " +
                "from consultants c " +
                "join users as u on u.userid = c.userid " +
                "where c.consultantid = ?consultantid; ";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("consultantid", consultantid));

            sql = "update consultants set " +
            "title = ?title, " +
            "first = ?first, " +
            "last = ?last, " +
            "nickname = ?nickname, " +
            "officeid = ?officeid, jobcode=?jobcode, " +
            "displayatwebsite = ?displayatwebsite, " +
            "imagepath = ?imagepath, " +
            "geographicfamiliarity = ?geographicfamiliarity, " +
            "industryspecialisation = ?industryspecialisation, " +
            "designation = ?designation, " +
            "profileinfo = ?profileinfo, " +
            "skype = ?skype, languages=?languages, qualifications=?qualifications, active=?active " +
            "where consultantid = ?consultantid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("consultantid", consultantid), new MySqlParameter("title", title), new MySqlParameter("first", first), new MySqlParameter("last", last), new MySqlParameter("nickname", nickname),
                new MySqlParameter("officeid", officeid), new MySqlParameter("jobcode", jobcode), new MySqlParameter("displayatwebsite", displayatwebsite), new MySqlParameter("imagepath", imagepath), new MySqlParameter("geographicfamiliarity", geographicfamiliarity),
                new MySqlParameter("industryspecialisation", industryspecialisation), new MySqlParameter("designation", designation), new MySqlParameter("profileinfo", profileinfo), new MySqlParameter("skype", skype),
                new MySqlParameter("languages", languages), new MySqlParameter("qualifications", qualifications), new MySqlParameter("active", active));

            if (reader.HasRows)
            {
                reader.Read();
                HistoryDataProvider history = new HistoryDataProvider();
                HistoryInfo historyInfo = new HistoryInfo();
                historyInfo.UserId = GPSession.UserId;
                historyInfo.ModuleId = (int)HistoryInfo.Module.Consultant;
                historyInfo.TypeId = (int)HistoryInfo.ActionType.Edit;
                historyInfo.RecordId = consultantid;
                historyInfo.ParentRecordId = consultantid;
                historyInfo.ModifiedDate = DateTime.Now;
                historyInfo.Details = new List<HistoryDetailInfo>();

                if (DAO.getString(reader, "title") != title)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "title", OldValue = DAO.getString(reader, "title"), NewValue = title });
                }
                if (DAO.getString(reader, "first") != first)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "first", OldValue = DAO.getString(reader, "first"), NewValue = first });
                }
                if (DAO.getString(reader, "last") != last)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "last", OldValue = DAO.getString(reader, "last"), NewValue = last });
                }
                if (DAO.getString(reader, "nickname") != nickname)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "nickname", OldValue = DAO.getString(reader, "nickname"), NewValue = nickname });
                }
                if (DAO.getInt(reader, "officeid") != officeid)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "officeid", OldValue = DAO.getInt(reader, "officeid").ToString(), NewValue = officeid.ToString() });
                }
                if ((string.IsNullOrEmpty(DAO.getString(reader, "jobcode")) ? string.Empty : DAO.getString(reader, "jobcode").ToString()) != jobcode)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "jobcode", OldValue = DAO.getString(reader, "jobcode"), NewValue = jobcode });
                }
                if (DAO.getBool(reader, "displayatwebsite") != displayatwebsite)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "displayatwebsite", OldValue = DAO.getBool(reader, "displayatwebsite").ToString(), NewValue = displayatwebsite.ToString() });
                }
                if (DAO.getString(reader, "imagepath") != imagepath)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "imagepath", OldValue = DAO.getString(reader, "imagepath"), NewValue = imagepath });
                }
                if (DAO.getString(reader, "geographicfamiliarity") != geographicfamiliarity)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "geographicfamiliarity", OldValue = DAO.getString(reader, "geographicfamiliarity"), NewValue = geographicfamiliarity });
                }
                if (DAO.getString(reader, "industryspecialisation") != industryspecialisation)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "industryspecialisation", OldValue = DAO.getString(reader, "industryspecialisation"), NewValue = industryspecialisation });
                }
                if (DAO.getString(reader, "designation") != designation)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "designation", OldValue = DAO.getString(reader, "designation"), NewValue = designation });
                }
                if (DAO.getString(reader, "profileinfo") != profileinfo)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "profileinfo", OldValue = DAO.getString(reader, "profileinfo"), NewValue = profileinfo });
                }
                if (DAO.getString(reader, "skype") != skype)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "skype", OldValue = DAO.getString(reader, "skype"), NewValue = skype });
                }
                if (historyInfo.Details.Count > 0)
                {
                    history.insertHistory(historyInfo);
                }
            }

            generateXML();
        }

        public static string getAddressHTML(uint addressid)
        {
            StringBuilder sb = new StringBuilder();

            using (MySqlDataReader drA = AddressDataProvider.getAddress(addressid))
            {
                uint line = 1;
                while (drA.Read())
                {
                    if (line != (uint)DAO.getUInt(drA, "line"))
                    {
                        sb.Append("_");
                        line = (uint)DAO.getUInt(drA, "line");
                    }
                    if ((uint)DAO.getUInt(drA, "position") > 1)
                    {
                        sb.Append("&nbsp;&nbsp;");
                    }
                    sb.Append(DAO.getString(drA, "value"));
                }

                if (drA.NextResult())
                {
                    if (drA.Read())
                    {
                        sb.Append("_");
                        sb.Append(DAO.getString(drA, "name"));
                    }
                }
            }
            return sb.ToString();
        }

        public static void generateXML()
        {
            //string sql = " select * from (Select case when c.countryid is null then l.countryid else c.countryid end as countryid, case when c.name is null then c1.name else c.name end as country " +
            //             " from Consultants co inner join  consultants_locations cl on cl.consultantid=co.consultantid left join countries c on cl.locationid=c.countryid and cl.locationtype=1 " +
            //             " left join locations l on cl.locationid=l.locationid and cl.locationtype=2 left join countries c1 on l.countryid=c1.countryid  where co.displayatwebsite=1 " +
            //             " group by countryid,country order by country) as t1 where countryid <> ''";
            string sql = "select * from (Select case when c.countryid is null then l.countryid else c.countryid end as countryid, case when c.name is null then c1.name else c.name end as countrty, " +
                        " l.locationid,l.name as locationname,count(cl.consultantid),l.level from Consultants co inner join  consultants_locations cl on cl.consultantid=co.consultantid " +
                        " left join countries c on cl.locationid=c.countryid and cl.locationtype=1 " +
                        " left join locations l on cl.locationid=l.locationid and cl.locationtype=2 left join countries c1 on l.countryid=c1.countryid where co.displayatwebsite=1 group by c.countryid,l.countryid,c.name,l.name) as t1 where countryid <> '';" +
                        " select * from (Select case when c.countryid is null then l.countryid else c.countryid end as countryid, case when c.name is null then c1.name else c.name end as country,l.level " +
                        " from Consultants co inner join  consultants_locations cl on cl.consultantid=co.consultantid left join countries c on cl.locationid=c.countryid and cl.locationtype=1 " +
                        " left join locations l on cl.locationid=l.locationid and cl.locationtype=2 left join countries c1 on l.countryid=c1.countryid  where co.displayatwebsite=1 group by countryid,country order by country) as t1 where countryid <> '';";
            MySqlDataReader dr = DAO.ExecuteReader(sql);
            DataSet ds = new DataSet();
            ds.Load(dr, LoadOption.PreserveChanges, new string[2]);
            dr.Close(); dr.Dispose();
            DataTable dtCountry = new DataTable();
            DataTable dtLocation = new DataTable();
            dtCountry = ds.Tables[1];
            dtLocation = ds.Tables[0];

            sql = "select c.consultantid, c.title, c.first, c.last,c.nickname,  c.imagepath, c.geographicfamiliarity, c.industryspecialisation, c.designation, c.profileinfo,c.skype,c.languages,c.qualifications,imagepath  " +
               "from consultants c where c.displayatwebsite = 1; " +

               "select c.consultantid,pn.phonenumberid, cpn.phonenumbertypeid, pn.number from consultants c join consultants_phonenumbers as cpn on c.consultantid=cpn.consultantid " +
               "join phonenumbers as pn on pn.phonenumberid = cpn.phonenumberid " +
               "where c.displayatwebsite = 1 order by pn.phonenumberid; " +

               "select c.consultantid,ca.addressid, ca.addresstypeid from consultants c join consultants_addresses ca on c.consultantid=ca.consultantid " +
               "where c.displayatwebsite = 1 and ca.addresstypeid=2 order by ca.addressid; " +

               "Select cl.locationid,cl.locationtype,co.consultantid,concat(case when c.name is null then c1.name else c.name end ,coalesce(concat(' (',l.name),')')) as location, case when c.name is null then c1.name else c.name end as country,l.name as adminidivision " +
                " from Consultants co inner join  consultants_locations cl on cl.consultantid=co.consultantid left join countries c on cl.locationid=c.countryid and cl.locationtype=1 " +
                " left join locations l on cl.locationid=l.locationid and cl.locationtype=2 left join countries c1 on l.countryid=c1.countryid where co.displayatwebsite = 1 order by country;";

            //"select email,defaultemail,ce.consultantid from emails e join consultants_emails ce on ce.emailid=e.emailid join consultants c on c.consultantid=ce.consultantid where c.displayatwebsite = 1";

            MySqlDataReader drConsultants = DAO.ExecuteReader(sql);
            DataSet dsConsultant = new DataSet();
            dsConsultant.Load(drConsultants, LoadOption.PreserveChanges, new string[4]);
            drConsultants.Close();
            drConsultants.Dispose();
            DataTable dtConsultant = dsConsultant.Tables[0];
            DataTable dtPhone = dsConsultant.Tables[1];
            DataTable dtAddress = dsConsultant.Tables[2];
            DataTable dtConsultantLocation = dsConsultant.Tables[3];
            //DataTable dtEmails = dsConsultant.Tables[4];

            XmlDocument XDoc = new XmlDocument();
            XmlElement XElemRoot = XDoc.CreateElement("locations");
            XDoc.AppendChild(XElemRoot);
            XmlElement Xlocation = XDoc.CreateElement("location");
            Xlocation.SetAttribute("regionName", "Country");
            Xlocation.SetAttribute("name", "Please select");
            XElemRoot.AppendChild(Xlocation);

            foreach (DataRow drCountry in dtCountry.Rows)
            {
                // Create root node.
                Xlocation = XDoc.CreateElement("location");
                Xlocation.SetAttribute("regionName", "Country");
                Xlocation.SetAttribute("name", drCountry["country"].ToString());

                //Add the node to the document.

                DataRow[] drSelect = dtLocation.Select("countryid=" + drCountry["countryid"].ToString());
                if (drSelect != null)
                {
                    XmlElement XCountry = XDoc.CreateElement("location");
                    XCountry.SetAttribute("regionName", "Location");
                    XCountry.SetAttribute("name", "Please select");
                    Xlocation.AppendChild(XCountry);

                    foreach (DataRow drLocation in drSelect)
                    {
                        DataRow[] drConsultant = dtConsultantLocation.Select("locationtype=2 and locationid=" + (string.IsNullOrEmpty(drLocation["locationid"].ToString()) ? "0" : drLocation["locationid"].ToString()));
                        if (drConsultant.Count() > 0)
                        {
                            XCountry = XDoc.CreateElement("location");
                            XCountry.SetAttribute("regionName", drLocation["level"].ToString());
                            XCountry.SetAttribute("name", drLocation["locationname"].ToString());
                            Xlocation.AppendChild(XCountry);

                            XmlElement Xconsultant = XDoc.CreateElement("location");
                            Xconsultant.SetAttribute("regionName", "Consultant");
                            Xconsultant.SetAttribute("name", "Please select");
                            XCountry.AppendChild(Xconsultant);

                            foreach (DataRow drCons in drConsultant)
                            {
                                Xconsultant = XDoc.CreateElement("location");

                                DataRow[] drConsultantinfo = dtConsultant.Select("consultantid=" + drCons["consultantid"].ToString());

                                if (drConsultantinfo != null)
                                {
                                    foreach (DataRow drInfo in drConsultantinfo)
                                    {
                                        Xconsultant = XDoc.CreateElement("location");
                                        Xconsultant.SetAttribute("regionName", "Consultant");
                                        Xconsultant.SetAttribute("name", drInfo["last"].ToString() + ", " + drInfo["first"].ToString());

                                        XmlElement Xcontact = XDoc.CreateElement("contactDetails");

                                        XmlElement Xaddress = XDoc.CreateElement("contactAddress");
                                        XmlElement Xperson = XDoc.CreateElement("contactPerson");
                                        Xperson.InnerText = drInfo["first"].ToString() + " " + drInfo["last"].ToString();
                                        Xaddress.AppendChild(Xperson);
                                        XmlElement Xdesig = XDoc.CreateElement("designation");
                                        Xdesig.InnerText = drInfo["designation"].ToString();
                                        Xaddress.AppendChild(Xdesig);
                                        XmlElement addressLine1 = XDoc.CreateElement("addressLine1");
                                        addressLine1.InnerText = "";
                                        Xaddress.AppendChild(addressLine1);
                                        XmlElement addressLine2 = XDoc.CreateElement("addressLine2");
                                        addressLine2.InnerText = "";
                                        Xaddress.AppendChild(addressLine2);
                                        XmlElement addressLine3 = XDoc.CreateElement("addressLine3");
                                        addressLine3.InnerText = "";
                                        Xaddress.AppendChild(addressLine3);
                                        XmlElement addressLine4 = XDoc.CreateElement("addressLine4");
                                        addressLine4.InnerText = drLocation["locationname"] == null ? "" : drLocation["locationname"].ToString();
                                        Xaddress.AppendChild(addressLine4);
                                        XmlElement addressLine5 = XDoc.CreateElement("addressLine5");
                                        addressLine5.InnerText = drCountry["country"].ToString();
                                        Xaddress.AppendChild(addressLine5);
                                        XmlElement telephoneNumber = XDoc.CreateElement("telephoneNumber");
                                        DataRow[] drPhone = dtPhone.Select("phonenumbertypeid=2 and consultantid=" + drCons["consultantid"].ToString());
                                        if (drPhone.Count() > 0)
                                        {
                                            telephoneNumber.InnerText = drPhone[0]["number"].ToString();
                                        }
                                        else
                                            telephoneNumber.InnerText = "";
                                        Xaddress.AppendChild(telephoneNumber);
                                        XmlElement mobileNumber = XDoc.CreateElement("mobileNumber");
                                        drPhone = dtPhone.Select("phonenumbertypeid=3 and consultantid=" + drCons["consultantid"].ToString());
                                        if (drPhone.Count() > 0)
                                        {
                                            mobileNumber.InnerText = drPhone[0]["number"].ToString();
                                        }
                                        else
                                            mobileNumber.InnerText = "";
                                        Xaddress.AppendChild(mobileNumber);
                                        XmlElement faxNumber = XDoc.CreateElement("faxNumber");
                                        drPhone = dtPhone.Select("phonenumbertypeid=6 and consultantid=" + drCons["consultantid"].ToString());
                                        if (drPhone.Count() > 0)
                                        {
                                            faxNumber.InnerText = drPhone[0]["number"].ToString();
                                        }
                                        else
                                            faxNumber.InnerText = "";
                                        Xaddress.AppendChild(faxNumber);
                                        XmlElement email = XDoc.CreateElement("email");
                                        //DataRow[] drEmails = dtEmails.Select("consultantid=" + drCons["consultantid"].ToString());
                                        //if (drEmails != null)
                                        //{
                                        //    if (drEmails.Count() > 0)
                                        //    {
                                        //        email.InnerText = "admin@you-global.com";
                                        //    }
                                        //}
                                        email.InnerText = "admin@you-global.com";
                                        Xaddress.AppendChild(email);
                                        XmlElement skype = XDoc.CreateElement("skype");
                                        skype.InnerText = drInfo["skype"].ToString();
                                        Xaddress.AppendChild(skype);

                                        Xcontact.AppendChild(Xaddress);

                                        XmlElement Xpostaladdress = XDoc.CreateElement("postalAddress");
                                        XmlElement Xperson1 = XDoc.CreateElement("contactPerson");
                                        Xperson1.InnerText = drInfo["first"].ToString() + " " + drInfo["last"].ToString();
                                        Xpostaladdress.AppendChild(Xperson1);

                                        XmlElement Xdesig1 = XDoc.CreateElement("designation");
                                        Xdesig1.InnerText = drInfo["designation"].ToString();
                                        Xpostaladdress.AppendChild(Xdesig1);
                                        XmlElement PaddressLine1 = XDoc.CreateElement("addressLine1");
                                        DataRow[] drAddress = dtAddress.Select("consultantid=" + drCons["consultantid"].ToString());
                                        string address = string.Empty;
                                        string[] addressArray = address.Split('\'');
                                        if (drAddress.Count() > 0)
                                        {
                                            address = getAddressHTML(Convert.ToUInt32(drAddress[0]["addressid"].ToString()));
                                            addressArray = address.Split('_');
                                        }
                                        if (!string.IsNullOrEmpty(address.ToString()))
                                            PaddressLine1.InnerText = addressArray[0].ToString();
                                        //PaddressLine1.InnerXml = PaddressLine1.InnerXml.Replace("&lt;", "<").Replace("&gt;", ">");
                                        else
                                            PaddressLine1.InnerText = "";
                                        Xpostaladdress.AppendChild(PaddressLine1);
                                        XmlElement PaddressLine2 = XDoc.CreateElement("addressLine2");
                                        if (addressArray.Length >= 2)
                                            PaddressLine2.InnerText = addressArray[1].ToString();
                                        else
                                            PaddressLine2.InnerText = "";
                                        Xpostaladdress.AppendChild(PaddressLine2);
                                        XmlElement PaddressLine3 = XDoc.CreateElement("addressLine3");
                                        if (addressArray.Length >= 3)
                                            PaddressLine3.InnerText = addressArray[2].ToString();
                                        else
                                            PaddressLine3.InnerText = "";
                                        Xpostaladdress.AppendChild(PaddressLine3);
                                        XmlElement PaddressLine4 = XDoc.CreateElement("addressLine4");
                                        if (addressArray.Length >= 4)
                                            PaddressLine4.InnerText = addressArray[3].ToString();
                                        else
                                            PaddressLine4.InnerText = "";
                                        Xpostaladdress.AppendChild(PaddressLine4);
                                        XmlElement PaddressLine5 = XDoc.CreateElement("addressLine5");
                                        if (addressArray.Length >= 5)
                                            PaddressLine5.InnerText = addressArray[4].ToString();
                                        else
                                            PaddressLine5.InnerText = "";
                                        Xpostaladdress.AppendChild(PaddressLine5);

                                        Xcontact.AppendChild(Xpostaladdress);

                                        XmlElement XbioDetails = XDoc.CreateElement("bioDetails");
                                        XmlElement XbioName = XDoc.CreateElement("bioName");
                                        XbioName.InnerText = drInfo["first"].ToString() + " " + drInfo["last"].ToString() + ", " + drInfo["designation"].ToString();
                                        XbioDetails.AppendChild(XbioName);

                                        XmlElement bioText = XDoc.CreateElement("bioText");

                                        bioText.InnerText = drInfo["profileinfo"].ToString() + " <br></br><br></br><font color=\"#006393\" size=\"11\"><b>Geographic Familiarity:</b></font>" + drInfo["geographicfamiliarity"].ToString() +
                                            "<br></br><br></br><font color=\"#006393\" size=\"11\"><b>Languages:</b></font>" + drInfo["languages"].ToString() +
                                             "<br></br><br></br><font color=\"#006393\" size=\"11\"><b>Industry Specialisation:</b></font>" + drInfo["industryspecialisation"].ToString() +
                                             "<br></br><br></br><font color=\"#006393\" size=\"11\"><b>Formal Qualifications:</b></font>" + drInfo["qualifications"].ToString() + "<br></br>";
                                        bioText.InnerXml = bioText.InnerXml.Replace("&lt;", "<").Replace("&gt;", ">");
                                        XbioDetails.AppendChild(bioText);

                                        XmlElement bioImage = XDoc.CreateElement("bioImage");
                                        if (!string.IsNullOrEmpty(drInfo["imagepath"].ToString()))
                                        {
                                            System.IO.FileInfo fileInfo = new System.IO.FileInfo(drInfo["imagepath"].ToString());
                                            bioImage.InnerText = @"/Content/flas/photos/" + fileInfo.Name.Replace("zip", "jpg");
                                        }
                                        else
                                            bioImage.InnerText = @"/Content/flas/photos/PhotoMain.jpg";
                                        XbioDetails.AppendChild(bioImage);

                                        Xcontact.AppendChild(XbioDetails);

                                        Xconsultant.AppendChild(Xcontact);
                                        XCountry.AppendChild(Xconsultant);
                                    }
                                }
                            }
                            Xlocation.AppendChild(XCountry);
                        }
                        else
                        {
                            drConsultant = dtConsultantLocation.Select("locationtype=1 and locationid=" + drLocation["countryid"].ToString());
                            XmlElement Xconsultant = XDoc.CreateElement("location");

                            foreach (DataRow drCons in drConsultant)
                            {
                                Xconsultant = XDoc.CreateElement("location");

                                DataRow[] drConsultantinfo = dtConsultant.Select("consultantid=" + drCons["consultantid"].ToString());

                                if (drConsultantinfo != null)
                                {
                                    //Xconsultant.SetAttribute("regionName", "Consultant");
                                    //Xconsultant.SetAttribute("name", "Please select");
                                    //Xlocation.AppendChild(Xconsultant);

                                    foreach (DataRow drInfo in drConsultantinfo)
                                    {
                                        //Xconsultant = XDoc.CreateElement("location");
                                        Xconsultant.SetAttribute("regionName", "Consultant");
                                        Xconsultant.SetAttribute("name", drInfo["last"].ToString() + ", " + drInfo["first"].ToString());

                                        XmlElement Xcontact = XDoc.CreateElement("contactDetails");

                                        XmlElement Xaddress = XDoc.CreateElement("contactAddress");
                                        XmlElement Xperson = XDoc.CreateElement("contactPerson");
                                        Xperson.InnerText = drInfo["first"].ToString() + " " + drInfo["last"].ToString();
                                        Xaddress.AppendChild(Xperson);
                                        XmlElement Xdesig = XDoc.CreateElement("designation");
                                        Xdesig.InnerText = drInfo["designation"].ToString();
                                        Xaddress.AppendChild(Xdesig);
                                        XmlElement addressLine1 = XDoc.CreateElement("addressLine1");
                                        addressLine1.InnerText = "";
                                        Xaddress.AppendChild(addressLine1);
                                        XmlElement addressLine2 = XDoc.CreateElement("addressLine2");
                                        addressLine2.InnerText = "";
                                        Xaddress.AppendChild(addressLine2);
                                        XmlElement addressLine3 = XDoc.CreateElement("addressLine3");
                                        addressLine3.InnerText = "";
                                        Xaddress.AppendChild(addressLine3);
                                        XmlElement addressLine4 = XDoc.CreateElement("addressLine4");
                                        addressLine4.InnerText = drLocation["locationname"].ToString();
                                        Xaddress.AppendChild(addressLine4);
                                        XmlElement addressLine5 = XDoc.CreateElement("addressLine5");
                                        addressLine5.InnerText = drCountry["country"].ToString();
                                        Xaddress.AppendChild(addressLine5);
                                        XmlElement telephoneNumber = XDoc.CreateElement("telephoneNumber");
                                        DataRow[] drPhone = dtPhone.Select("phonenumbertypeid=2 and consultantid=" + drCons["consultantid"].ToString());
                                        if (drPhone.Count() > 0)
                                        {
                                            telephoneNumber.InnerText = drPhone[0]["number"].ToString();
                                        }
                                        else
                                            telephoneNumber.InnerText = "";
                                        Xaddress.AppendChild(telephoneNumber);
                                        XmlElement mobileNumber = XDoc.CreateElement("mobileNumber");
                                        drPhone = dtPhone.Select("phonenumbertypeid=3 and consultantid=" + drCons["consultantid"].ToString());
                                        if (drPhone.Count() > 0)
                                        {
                                            mobileNumber.InnerText = drPhone[0]["number"].ToString();
                                        }
                                        else
                                            mobileNumber.InnerText = "";
                                        Xaddress.AppendChild(mobileNumber);
                                        XmlElement faxNumber = XDoc.CreateElement("faxNumber");
                                        drPhone = dtPhone.Select("phonenumbertypeid=6 and consultantid=" + drCons["consultantid"].ToString());
                                        if (drPhone.Count() > 0)
                                        {
                                            faxNumber.InnerText = drPhone[0]["number"].ToString();
                                        }
                                        else
                                            faxNumber.InnerText = "";
                                        Xaddress.AppendChild(faxNumber);
                                        XmlElement email = XDoc.CreateElement("email");
                                        email.InnerText = "admin@you-global.com";
                                        Xaddress.AppendChild(email);
                                        XmlElement skype = XDoc.CreateElement("skype");
                                        skype.InnerText = drInfo["skype"].ToString();
                                        Xaddress.AppendChild(skype);

                                        Xcontact.AppendChild(Xaddress);

                                        XmlElement Xpostaladdress = XDoc.CreateElement("postalAddress");
                                        XmlElement Xperson1 = XDoc.CreateElement("contactPerson");
                                        Xperson1.InnerText = drInfo["first"].ToString() + " " + drInfo["last"].ToString();
                                        Xpostaladdress.AppendChild(Xperson1);

                                        XmlElement Xdesig1 = XDoc.CreateElement("designation");
                                        Xdesig1.InnerText = drInfo["designation"].ToString();
                                        Xpostaladdress.AppendChild(Xdesig1);
                                        XmlElement PaddressLine1 = XDoc.CreateElement("addressLine1");
                                        DataRow[] drAddress = dtAddress.Select("consultantid=" + drCons["consultantid"].ToString());
                                        string address = string.Empty;
                                        string[] addressArray = address.Split('\'');
                                        if (drAddress.Count() > 0)
                                        {
                                            address = getAddressHTML(Convert.ToUInt32(drAddress[0]["addressid"].ToString()));
                                            addressArray = address.Split('_');
                                        }
                                        if (!string.IsNullOrEmpty(address.ToString()))
                                            PaddressLine1.InnerText = addressArray[0].ToString();
                                        //PaddressLine1.InnerXml = PaddressLine1.InnerXml.Replace("&lt;", "<").Replace("&gt;", ">");
                                        else
                                            PaddressLine1.InnerText = "";
                                        Xpostaladdress.AppendChild(PaddressLine1);
                                        XmlElement PaddressLine2 = XDoc.CreateElement("addressLine2");
                                        if (addressArray.Length >= 2)
                                            PaddressLine2.InnerText = addressArray[1].ToString();
                                        else
                                            PaddressLine2.InnerText = "";
                                        Xpostaladdress.AppendChild(PaddressLine2);
                                        XmlElement PaddressLine3 = XDoc.CreateElement("addressLine3");
                                        if (addressArray.Length >= 3)
                                            PaddressLine3.InnerText = addressArray[2].ToString();
                                        else
                                            PaddressLine3.InnerText = "";
                                        Xpostaladdress.AppendChild(PaddressLine3);
                                        XmlElement PaddressLine4 = XDoc.CreateElement("addressLine4");
                                        if (addressArray.Length >= 4)
                                            PaddressLine4.InnerText = addressArray[3].ToString();
                                        else
                                            PaddressLine4.InnerText = "";
                                        Xpostaladdress.AppendChild(PaddressLine4);
                                        XmlElement PaddressLine5 = XDoc.CreateElement("addressLine5");
                                        if (addressArray.Length >= 5)
                                            PaddressLine5.InnerText = addressArray[4].ToString();
                                        else
                                            PaddressLine5.InnerText = "";
                                        Xpostaladdress.AppendChild(PaddressLine5);

                                        Xcontact.AppendChild(Xpostaladdress);

                                        XmlElement XbioDetails = XDoc.CreateElement("bioDetails");
                                        XmlElement XbioName = XDoc.CreateElement("bioName");
                                        XbioName.InnerText = drInfo["first"].ToString() + " " + drInfo["last"].ToString() + ", " + drInfo["designation"].ToString();
                                        XbioDetails.AppendChild(XbioName);

                                        XmlElement bioText = XDoc.CreateElement("bioText");

                                        bioText.InnerText = drInfo["profileinfo"].ToString() + " <br></br><br></br><font color=\"#006393\" size=\"11\"><b>Geographic Familiarity:</b></font>" + drInfo["geographicfamiliarity"].ToString() +
                                            "<br></br><br></br><font color=\"#006393\" size=\"11\"><b>Languages:</b></font>" + drInfo["languages"].ToString() +
                                             "<br></br><br></br><font color=\"#006393\" size=\"11\"><b>Industry Specialisation:</b></font>" + drInfo["industryspecialisation"].ToString() +
                                             "<br></br><br></br><font color=\"#006393\" size=\"11\"><b>Formal Qualifications:</b></font>" + drInfo["qualifications"].ToString() + "<br></br>";
                                        bioText.InnerXml = bioText.InnerXml.Replace("&lt;", "<").Replace("&gt;", ">");
                                        XbioDetails.AppendChild(bioText);

                                        XmlElement bioImage = XDoc.CreateElement("bioImage");
                                        if (!string.IsNullOrEmpty(drInfo["imagepath"].ToString()))
                                        {
                                            System.IO.FileInfo fileInfo = new System.IO.FileInfo(drInfo["imagepath"].ToString());
                                            bioImage.InnerText = @"/Content/flas/photos/" + fileInfo.Name.Replace("zip", "jpg");
                                        }
                                        else
                                            bioImage.InnerText = @"/Content/flas/photos/PhotoMain.jpg";
                                        XbioDetails.AppendChild(bioImage);

                                        Xcontact.AppendChild(XbioDetails);

                                        Xconsultant.AppendChild(Xcontact);
                                        Xlocation.AppendChild(Xconsultant);
                                    }
                                }
                            }
                        }
                    }
                }
                XElemRoot.AppendChild(Xlocation);
            }

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.CheckCharacters = false;
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.UTF8;
            //XDoc.InnerXml = XDoc.InnerText.Replace("&lt;", "<").Replace("&gt;", ">");

            XmlWriter writer = XmlWriter.Create(ConfigurationManager.AppSettings["consultantXML"].ToString(), settings);
            XDoc.Save(writer);
            writer.Close();
        }

        public static void addEmail(uint consultantid, string email, uint emailtypeid, bool defaultemail)
        {
            //test
            uint emailid = EmailDataProvider.insertEmail(email);
            string sql = "insert into consultants_emails (consultantid, emailid, emailtypeid, defaultemail) values (?consultantid, ?emailid, ?emailtypeid, ?defaultemail)";
            DAO.ExecuteScalar(sql, new MySqlParameter("consultantid", consultantid), new MySqlParameter("emailid", emailid), new MySqlParameter("emailtypeid", emailtypeid), new MySqlParameter("defaultemail", defaultemail));

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.consultants_emails;
            info.TypeId = (int)HistoryInfo.ActionType.Add;
            info.RecordId = emailid;
            info.ParentRecordId = consultantid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "All", NewValue = "emailid:" + emailid + " ,email:" + email + " ,consultantid:" + consultantid + " ,emailtypeid:" + emailtypeid + " ,defaultemail:" + defaultemail });

            history.insertHistory(info);
        }

        public static void updateEmail(uint consultantid, uint emailid, string email, uint emailtypeid, bool defaultemail)
        {
            string sql = "select e.emailid, ce.emailtypeid, e.email, ce.defaultemail " +
                "from consultants_emails as ce " +
                "join emails as e on e.emailid = ce.emailid " +
                "where ce.consultantid = ?consultantid " +
                "order by ce.emailid;";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("consultantid", consultantid));

            EmailDataProvider.updateEmail(emailid, email);
            sql = "update consultants_emails set " +
               "emailtypeid = ?emailtypeid, " +
               "defaultemail = ?defaultemail " +
               "where consultantid = ?consultantid " +
               "and emailid = ?emailid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("consultantid", consultantid), new MySqlParameter("emailid", emailid), new MySqlParameter("emailtypeid", emailtypeid), new MySqlParameter("defaultemail", defaultemail));

            if (reader.HasRows)
            {
                reader.Read();

                HistoryDataProvider history = new HistoryDataProvider();
                HistoryInfo historyInfo = new HistoryInfo();
                historyInfo.UserId = GPSession.UserId;
                historyInfo.ModuleId = (int)HistoryInfo.Module.consultants_emails;
                historyInfo.TypeId = (int)HistoryInfo.ActionType.Edit;
                historyInfo.RecordId = emailid;
                historyInfo.ModifiedDate = DateTime.Now;
                historyInfo.Details = new List<HistoryDetailInfo>();

                if (DAO.getInt(reader, "emailtypeid") != emailtypeid)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "emailtypeid", OldValue = DAO.getInt(reader, "emailtypeid").ToString(), NewValue = emailtypeid.ToString() });
                }
                if (DAO.getString(reader, "email") != email)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "email", OldValue = DAO.getString(reader, "email"), NewValue = email });
                }
                if (DAO.getBool(reader, "defaultemail") != defaultemail)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "defaultemail", OldValue = DAO.getBool(reader, "defaultemail").ToString(), NewValue = defaultemail.ToString() });
                }

                if (historyInfo.Details.Count > 0)
                {
                    history.insertHistory(historyInfo);
                }
            }
        }

        public static void removeEmail(uint consultantid, uint emailid)
        {
            string email = EmailDataProvider.getEmail(emailid);
            string sql = "delete from consultants_emails where consultantid = ?consultantid and emailid = ?emailid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("consultantid", consultantid), new MySqlParameter("emailid", emailid));
            EmailDataProvider.removeEmail(emailid);

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.consultants_emails;
            info.TypeId = (int)HistoryInfo.ActionType.Delete;
            info.RecordId = emailid;
            info.ParentRecordId = consultantid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "Delete", NewValue = email });

            history.insertHistory(info);
        }

        public static void addPhoneNumber(uint consultantid, string number, uint phonenumbertypeid, string countrycode)
        {
            uint phonenumberid = PhoneNumberDataProvider.insertPhoneNumber(number, countrycode);
            string sql = "insert into consultants_phonenumbers (consultantid, phonenumberid, phonenumbertypeid) values (?consultantid, ?phonenumberid, ?phonenumbertypeid)";
            DAO.ExecuteScalar(sql, new MySqlParameter("consultantid", consultantid), new MySqlParameter("phonenumberid", phonenumberid), new MySqlParameter("phonenumbertypeid", phonenumbertypeid));
            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.consultants_phonenumbers;
            info.TypeId = (int)HistoryInfo.ActionType.Add;
            info.RecordId = phonenumberid;
            info.ParentRecordId = consultantid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "All", NewValue = "phonenumberid:" + phonenumberid + " ,number:" + number + " ,consultantid:" + consultantid + " ,phonenumbertypeid:" + phonenumbertypeid });

            history.insertHistory(info);
        }

        public static void updatePhoneNumber(uint consultantid, uint phonenumberid, string number, uint phonenumbertypeid, string countrycode)
        {
            string sql = "select pn.phonenumberid, cpn.phonenumbertypeid, pn.number " +
                "from consultants_phonenumbers as cpn " +
                "join phonenumbers as pn on pn.phonenumberid = cpn.phonenumberid " +
                "where cpn.consultantid = ?consultantid " +
                "order by pn.phonenumberid; ";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("consultantid", consultantid));

            PhoneNumberDataProvider.updatePhoneNumber(phonenumberid, number, countrycode);
            sql = "update consultants_phonenumbers set " +
               "phonenumbertypeid = ?phonenumbertypeid " +
               "where consultantid = ?consultantid " +
               "and phonenumberid = ?phonenumberid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("consultantid", consultantid), new MySqlParameter("phonenumberid", phonenumberid), new MySqlParameter("phonenumbertypeid", phonenumbertypeid));

            if (reader.HasRows)
            {
                reader.Read();
                HistoryDataProvider history = new HistoryDataProvider();
                HistoryInfo historyInfo = new HistoryInfo();
                historyInfo.UserId = GPSession.UserId;
                historyInfo.ModuleId = (int)HistoryInfo.Module.consultants_phonenumbers;
                historyInfo.TypeId = (int)HistoryInfo.ActionType.Edit;
                historyInfo.RecordId = phonenumberid;
                historyInfo.ModifiedDate = DateTime.Now;
                historyInfo.Details = new List<HistoryDetailInfo>();

                if (DAO.getInt(reader, "phonenumbertypeid") != phonenumbertypeid)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "phonenumbertypeid", OldValue = DAO.getInt(reader, "phonenumbertypeid").ToString(), NewValue = phonenumbertypeid.ToString() });
                }
                if (DAO.getString(reader, "number") != number)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "number", OldValue = DAO.getString(reader, "number"), NewValue = number });
                }

                if (historyInfo.Details.Count > 0)
                {
                    history.insertHistory(historyInfo);
                }
            }
        }

        public static void removePhoneNumber(uint consultantid, uint phonenumberid)
        {
            string number = PhoneNumberDataProvider.getPhoneNumber(phonenumberid);
            string sql = "delete from consultants_phonenumbers where consultantid = ?consultantid and phonenumberid = ?phonenumberid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("consultantid", consultantid), new MySqlParameter("phonenumberid", phonenumberid));
            PhoneNumberDataProvider.removePhoneNumber(phonenumberid);

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.consultants_phonenumbers;
            info.TypeId = (int)HistoryInfo.ActionType.Delete;
            info.RecordId = phonenumberid;
            info.ParentRecordId = consultantid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "Delete", NewValue = number });

            history.insertHistory(info);
        }

        public static void addAddress(uint consultantid, uint addresstypeid, uint addressid)
        {
            string sql = "insert into consultants_addresses (consultantid, addressid, addresstypeid) values (?consultantid, ?addressid, ?addresstypeid)";
            DAO.ExecuteScalar(sql, new MySqlParameter("consultantid", consultantid), new MySqlParameter("addressid", addressid), new MySqlParameter("addresstypeid", addresstypeid));
        }

        public static void updateAddressType(uint consultantid, uint addressid, uint addresstypeid)
        {
            string sql = "update consultants_addresses set addresstypeid = ?addresstypeid where consultantid = ?consultantid and addressid = ?addressid";
            DAO.ExecuteScalar(sql, new MySqlParameter("consultantid", consultantid), new MySqlParameter("addressid", addressid), new MySqlParameter("addresstypeid", addresstypeid));
        }

        public static void removeAddress(uint consultantid, uint addressid)
        {
            string address = AddressDataProvider.getAddressHTML(addressid);
            string sql = "delete from consultants_addresses where consultantid = ?consultantid and addressid = ?addressid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("consultantid", consultantid), new MySqlParameter("addressid", addressid));
            AddressDataProvider.removeAddress(addressid);

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.consultant_address;
            info.TypeId = (int)HistoryInfo.ActionType.Delete;
            info.RecordId = addressid;
            info.ParentRecordId = consultantid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "Delete", NewValue = address });

            history.insertHistory(info);
        }

        public static MySqlDataReader searchConsultants(string keyword, uint tenantid)
        {
            string sql = "select count(ip.ipaddress) as IPCount, c.consultantid, c.title, c.first, c.last, c.nickname, e.email,  u.username, " +
                "case when c.active is null then 0 else case when c.active then 1 else 0 end end as active, case when c.displayatwebsite is null then 0 else case when c.displayatwebsite then 1 else 0 end end as displayatwebsite  " +
                "from consultants as c " +
                "left join consultants_ipaddress as ip on c.consultantid = ip.consultantid " +
                "join users as u on u.userid = c.userid " +
                "join consultants_emails as ce on ce.consultantid = c.consultantid and ce.defaultemail = 1 " +
                "join emails as e on e.emailid = ce.emailid " +
                //"join offices as o on o.officeid = c.officeid " +
                "where u.tenantid = ?tenantid " +
                "and concat_ws(' ',c.first,c.last,c.nickname,e.email,u.username) like concat_ws(?keyword,'%','%') " +
                "group by c.consultantid, c.title, c.first, c.last,c.nickname, e.email,  u.username,c.active " +
                "order by c.last, c.first ";
            return DAO.ExecuteReader(sql, new MySqlParameter("tenantid", tenantid), new MySqlParameter("keyword", keyword));
        }

        public static MySqlDataReader searchConsultants(string keyword, uint tenantid, int status)
        {
            string sql = "select count(ip.ipaddress) as IPCount, c.consultantid, c.title, c.first, c.last, c.nickname, e.email,  u.username, u.userid, " +
                "case when c.active is null then 0 else case when c.active then 1 else 0 end end as active, case when c.displayatwebsite is null then 0 else case when c.displayatwebsite then 1 else 0 end end as displayatwebsite  " +
                "from consultants as c " +
                "left join consultants_ipaddress as ip on c.consultantid = ip.consultantid " +
                "join users as u on u.userid = c.userid " +
                "left join consultants_emails as ce on ce.consultantid = c.consultantid and ce.defaultemail = 1 " +
                "left join emails as e on e.emailid = ce.emailid " +
                "where u.tenantid = ?tenantid " +
                "and concat_ws(' ',c.first,c.last,c.nickname,e.email,u.username) like concat_ws(?keyword,'%','%') " +
                "and (c.active=?active or ?active=-1) " +
                "group by c.consultantid, c.title, c.first, c.last,c.nickname, e.email,  u.username " +
                "order by c.last, c.first ";
            return DAO.ExecuteReader(sql, new MySqlParameter("tenantid", tenantid), new MySqlParameter("keyword", keyword), new MySqlParameter("active", status));
        }

        public static MySqlDataReader getConsultant(uint consultantid)
        {
            string sql = "select c.consultantid, c.title, c.first, c.last,c.nickname, c.officeid, c.userid, u.username, c.jobcode, case when c.displayatwebsite is null then 0 else case when c.displayatwebsite then 1 else 0 end end as displayatwebsite," +
                 " c.imagepath, c.geographicfamiliarity, c.industryspecialisation, c.designation, c.profileinfo, c.skype, u.active, u.utcoffset, u.timezoneid,c.languages,c.qualifications  " +
                "from consultants c " +
                "join users as u on u.userid = c.userid " +
                "where c.consultantid = ?consultantid; " +

                "select e.emailid, ce.emailtypeid, e.email, ce.defaultemail " +
                "from consultants_emails as ce " +
                "join emails as e on e.emailid = ce.emailid " +
                "where ce.consultantid = ?consultantid " +
                "order by ce.emailid; " +

                "select pn.phonenumberid, cpn.phonenumbertypeid, pn.number " +
                "from consultants_phonenumbers as cpn " +
                "join phonenumbers as pn on pn.phonenumberid = cpn.phonenumberid " +
                "where cpn.consultantid = ?consultantid " +
                "order by pn.phonenumberid; " +

                "select ca.addressid, ca.addresstypeid " +
                "from consultants_addresses as ca " +
                "where ca.consultantid = ?consultantid " +
                "order by ca.addressid; " +

                "select ip.consultants_ipaddressid , ip.ipaddress from consultants_ipaddress as ip " +
                "where ip.consultantid = ?consultantid ";
            return DAO.ExecuteReader(sql, new MySqlParameter("consultantid", consultantid));
        }

        public static MySqlDataReader getConsultantLanguages(uint consultantId)
        {
            string sql = "select cl.consultant_languageid, cl.languageid, cl.spoken, cl.written, l.name as language, cl.listening, cl.reading " +
                "from consultants_languages as cl inner join languages l on cl.languageid=l.languageid " +
                " where cl.consultantid = ?consultantid " +
                "order by cl.consultant_languageid; ";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("consultantid", consultantId));

            return reader;
        }

        public static MySqlDataReader getConsultantQualifications(uint consultantId)
        {
            string sql = "select cl.consultantqualificationid, cl.qualificationid, cl.institutionid, YEAR(cl.obtained) as obtained, l.name as qualification " +
                "from consultants_qualifications as cl inner join qualifications l on cl.qualificationid=l.qualificationid " +
                " where cl.consultantid = ?consultantid " +
                "order by cl.consultantqualificationid; ";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("consultantid", consultantId));

            return reader;
        }

        public static void addQualification(uint consultantid, uint qualificationid, uint institutionid, DateTime obtained)
        {
            string sql = "insert into consultants_qualifications (consultantid, qualificationid, institutionid, obtained) values (?consultantid, ?qualificationid, ?institutionid, ?obtained)";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("consultantid", consultantid), new MySqlParameter("qualificationid", qualificationid), new MySqlParameter("institutionid", institutionid), new MySqlParameter("obtained", obtained));
        }

        public static void updateQualification(uint consultantqualificationid, uint qualificationid, uint institutionid, DateTime obtained)
        {
            string sql = "update consultants_qualifications set " +
                "qualificationid = ?qualificationid, " +
                "institutionid = ?institutionid, " +
                "obtained = ?obtained " +
                "where consultantqualificationid = ?consultantqualificationid ";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("consultantqualificationid", consultantqualificationid), new MySqlParameter("qualificationid", qualificationid), new MySqlParameter("institutionid", institutionid), new MySqlParameter("obtained", obtained));
        }

        public static void updateConsultantStatus(uint consultantid, int active, uint userid)
        {
            string sql = "update consultants set active = ?active where consultantid = ?consultantid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("consultantid", consultantid), new MySqlParameter("active", active));

            string query = "update users set active = ?active where userid = ?userid";
            DAO.ExecuteNonQuery(query, new MySqlParameter("userid", userid), new MySqlParameter("active", active));
        }

        public static void updateConsultantDisplayStatus(uint consultantid, int active)
        {
            string sql = "update consultants set displayatwebsite = ?active where consultantid = ?consultantid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("consultantid", consultantid), new MySqlParameter("active", active));
        }

        public static void removeQualification(uint candidateid, uint consultantqualificationid)
        {
            string sql = "select name,l.qualificationid from consultants_qualifications cl inner join qualifications l on cl.qualificationid=l.qualificationid where consultantqualificationid = ?consultantqualificationid";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("consultantqualificationid", consultantqualificationid));
            reader.Read();
            string qualification = DAO.getString(reader, "name");
            int qualificationid = Convert.ToInt32(DAO.getString(reader, "qualificationid"));

            reader.Close();
            reader.Dispose();

            sql = "delete from consultants_qualifications where consultantqualificationid = ?consultantqualificationid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("consultantqualificationid", consultantqualificationid));

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.candidates_languages;
            info.TypeId = (int)HistoryInfo.ActionType.Delete;
            info.RecordId = consultantqualificationid;
            info.ParentRecordId = candidateid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "Delete", NewValue = qualification });

            history.insertHistory(info);
        }

        public static MySqlDataReader getOnlyConsultant(uint consultantid)
        {
            string sql = "select c.consultantid, c.title, c.first, c.last,c.nickname, c.officeid, c.userid, u.username,c.jobcode " +
                "from consultants c " +
                "join users as u on u.userid = c.userid " +
                "where c.consultantid = ?consultantid; ";
            return DAO.ExecuteReader(sql, new MySqlParameter("consultantid", consultantid));
        }

        public static MySqlDataReader listActiveConsultants(uint tenantid)
        {
            string sql = "select c.consultantid, c.first, c.last,c.nickname " +
                "from consultants as c " +
                "join users as u on u.userid = c.userid " +
                "where u.tenantid = ?tenantid " +
                "and u.active = 1 " +
                "order by c.last, c.first";
            return DAO.ExecuteReader(sql, new MySqlParameter("tenantid", tenantid));
        }

        public static int getConsultantIdByUserId(uint userId)
        {
            string sql = "select c.consultantid " +
                "from consultants as c " +
                "join users as u on u.userid = c.userid " +
                "where u.userId = ?userId ";

            return Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("userId", userId)));
        }

        public static string getConsultantUsername(int consultantId)
        {
            string sql = "select u.username " +
                "from consultants as c " +
                "join users as u on u.userid = c.userid " +
                "where c.consultantid = ?consultantId ";

            return Convert.ToString(DAO.ExecuteScalar(sql, new MySqlParameter("consultantId", consultantId)));
        }

        public static DataTable getConsultantEmailByUserId(uint userId)
        {
            DataTable dt = new DataTable();
            string sql = "select e.emailid, ce.emailtypeid, e.email, ce.defaultemail " +
                "from consultants as c " +
                "join users as u on u.userid = c.userid inner join consultants_emails as ce on ce.consultantid=c.consultantid " +
                "join emails as e on e.emailid = ce.emailid " +
                "where u.userId = ?userId " +
                "order by ce.emailid;";

            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("userid", userId));
            dt.Load(reader);
            reader.Close();
            reader.Dispose();

            return dt;
        }

        public static DataTable getConsultantEmailById(uint Id)
        {
            DataTable dt = new DataTable();
            string sql = "select e.emailid, ce.emailtypeid, e.email, ce.defaultemail " +
                "from consultants_emails as ce " +
                "join emails as e on e.emailid = ce.emailid " +
                "where ce.consultantid = ?consultantid " +
                "order by ce.emailid; ";

            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("consultantid", Id));
            dt.Load(reader);
            reader.Close();
            reader.Dispose();

            return dt;
        }

        public static void updateActive(int consultantid, bool active, bool displayatwebsite)
        {
            string sql = "update consultants set active=?active, displayatwebsite=?displayatwebsite where consultantid=?consultantid ";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("active", active), new MySqlParameter("consultantid", consultantid), new MySqlParameter("displayatwebsite", displayatwebsite));
        }

        public static string getJobcodePrefix(int memberid)
        {
            string jobcode = string.Empty;
            string sql = "SELECT TRIM(UPPER(CONCAT(LEFT(FIRSTNAME , 1),'X',LEFT(LASTNAME , 1))))  FROM MEMBERS WHERE memberid=?memberid ";
            jobcode = DAO.ExecuteScalar(sql, new MySqlParameter("memberid", memberid)).ToString();
            return jobcode;
        }

        public static void addIPAddress(uint consultantid, string ipAddress)
        {
            string sql = "insert into consultants_ipaddress (consultantid, ipaddress) values (?consultantid, ?ipaddress)";
            uint ipaddressid = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("consultantid", consultantid), new MySqlParameter("ipaddress", ipAddress)));

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.consultants_emails;
            info.TypeId = (int)HistoryInfo.ActionType.Add;
            info.RecordId = ipaddressid;
            info.ParentRecordId = consultantid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "Add", NewValue = "ipaddress:" + ipAddress });
            history.insertHistory(info);
        }

        public static string getIPAddress(uint consultants_ipaddressid)
        {
            string sql = "select ipaddress from consultants_ipaddress where consultants_ipaddressid=?consultants_ipaddressid";
            string ipaddress = (string)DAO.ExecuteScalar(sql, new MySqlParameter("consultants_ipaddressid", consultants_ipaddressid));
            return ipaddress;
        }

        public static void removeIPAddress(uint consultantid, uint consultants_ipaddressid)
        {
            string ipaddress = ConsultantDataProvider.getIPAddress(consultants_ipaddressid);
            string sql = "delete from consultants_ipaddress where consultantid = ?consultantid and consultants_ipaddressid = ?consultants_ipaddressid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("consultantid", consultantid), new MySqlParameter("consultants_ipaddressid", consultants_ipaddressid));

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.consultant_ipaddress;
            info.TypeId = (int)HistoryInfo.ActionType.Delete;
            info.RecordId = consultants_ipaddressid;
            info.ParentRecordId = consultantid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "Delete", NewValue = ipaddress });
            history.insertHistory(info);
        }

        public static void updateIPAddress(uint consultantid, string ipaddress, uint consultants_ipaddressid)
        {
            string sql = "update consultants_ipaddress set ipaddress = ?ipaddress where consultantid = ?consultantid and consultants_ipaddressid = ?consultants_ipaddressid";
            DAO.ExecuteScalar(sql, new MySqlParameter("consultantid", consultantid), new MySqlParameter("ipaddress", ipaddress), new MySqlParameter("consultants_ipaddressid", consultants_ipaddressid));
        }

        public static void addLocation(uint consultantId, int locationtype, int locationId)
        {
            string sql = "insert into consultants_locations (consultantid,locationtype,locationid) values (?consultantid,?locationtype,?locationid)";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("consultantid", consultantId), new MySqlParameter("locationtype", locationtype), new MySqlParameter("locationid", locationId));
        }

        public static void removeLocation(uint consultantId, int locationtype, int locationId)
        {
            string sql = "delete from consultants_locations where consultantid=?consultantid and locationtype=?locationtype and locationid=?locationid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("consultantid", consultantId), new MySqlParameter("locationtype", locationtype), new MySqlParameter("locationid", locationId));
        }

        public static DataSet getConsultantLocation(uint consultantId)
        {
            string sql = " select locationid,locationtype, (select concat(c.name,',',CAST(c.countryid as char),',','0,0,0') from countries c where c.countryid =g.locationid and g.locationtype=1 union " +
                         " select concat(concat(c.name,' > ',l.name),',',CAST(c.countryid as char),',',cast(l.locationid as char),',', '0,0') from countries c left join locations l on l.countryid=c.countryid where   l.locationid=g.locationid and g.locationtype=2 union " +
                         " select concat(concat(c.name,' > ',l.name,' > ',s.sublocation),',',CAST(c.countryid as char),',',cast(l.locationid as char),',', cast(s.sublocationid as char),',','0') from countries c left join locations l on l.countryid=c.countryid left join locationsub s on s.locationid=l.locationid " +
                         " where  s.sublocationid=g.locationid and g.locationtype=3 union " +
                         " select concat(concat(c.name,' > ',l.name,' > ',s.sublocation,' > ',ss.name),',',CAST(c.countryid as char),',',cast(l.locationid as char),',', cast(s.sublocationid as char),',',cast(ss.subsublocationid as char) ) from countries c left join locations l on l.countryid=c.countryid  " +
                         " left join locationsub s on s.locationid=l.locationid left join locationsub_subs ss on ss.sublocationid=s.sublocationid where  ss.subsublocationid=g.locationid and g.locationtype=4 ) as parentids " +
                         " from consultants_locations g where consultantid=?consultantid; ";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("consultantid", consultantId));
            DataSet ds = new DataSet();
            ds.EnforceConstraints = false;
            ds.Load(dr, LoadOption.PreserveChanges, new string[1]);
            dr.Close();
            dr.Dispose();
            return ds;
        }

        public static DataTable getConsultantLocationById(int consultantId)
        {
            string sql = "Select concat(case when c.name is null then c1.name else c.name end ,coalesce(concat(' (',l.name),')')) as location, case when c.name is null then c1.name else c.name end as country,l.name as adminidivision " +
                    " from Consultants co inner join  consultants_locations cl on cl.consultantid=co.consultantid left join countries c on cl.locationid=c.countryid and cl.locationtype=1 " +
                    " left join locations l on cl.locationid=l.locationid and cl.locationtype=2 left join countries c1 on l.countryid=c1.countryid where co.consultantid=?consultantid order by country";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("consultantid", consultantId));
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            dr.Dispose();
            return dt;
        }

        public static void addLanguage(uint consultantid, uint languageid, uint spoken, uint written, uint listening, uint reading)
        {
            string sql = "insert into consultants_languages (consultantid, languageid, spoken, written, listening, reading) values (?consultantid, ?languageid, ?spoken, ?written, ?listening, ?reading)";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("consultantid", consultantid), new MySqlParameter("languageid", languageid), new MySqlParameter("spoken", spoken), new MySqlParameter("written", written), new MySqlParameter("listening", listening), new MySqlParameter("reading", reading));
        }

        public static void updateLanguage(uint consultantlanguageid, uint languageid, uint spoken, uint written, uint listening, uint reading)
        {
            string sql = "update consultants_languages set " +
                "languageid = ?languageid, " +
                "spoken = ?spoken, " +
                "written = ?written, " +
                "listening = ?listening, " +
                "reading =?reading " +
                "where consultant_languageid = ?consultantlanguageid ";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("consultantlanguageid", consultantlanguageid), new MySqlParameter("languageid", languageid), new MySqlParameter("spoken", spoken), new MySqlParameter("written", written),
                new MySqlParameter("listening", listening), new MySqlParameter("reading", reading));
        }

        public static void removeLanguage(uint consultantid, uint consultantlanguageid)
        {
            string sql = "delete from consultants_languages where consultant_languageid = ?consultantlanguageid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("consultantlanguageid", consultantlanguageid));
        }
    }
}