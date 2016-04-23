using GlobalPanda.BusinessInfo;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

/// <summary>
/// Summary description for RepresentativeDataProvider
/// </summary>
///
namespace GlobalPanda.DataProviders
{
    public class RepresentativeDataProvider
    {
        public static uint insertRepresentative(RepresentativeInfo info)
        {
            string sql = "insert into representatives (title, firstname, lastname, positionTitle, username, createdDate) values (?title, ?firstname, ?lastname, ?positionTitle, ?username, ?createdDate); select last_insert_id()";
            uint representativeId = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("title", info.Title), new MySqlParameter("firstname", info.FirstName), new MySqlParameter("lastname", info.LastName), new MySqlParameter("positionTitle", info.PositionTitle), new MySqlParameter("username", info.UserName), new MySqlParameter("createdDate", info.CreatedDate)));
            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo historyInfo = new HistoryInfo();
            historyInfo.UserId = GPSession.UserId;
            historyInfo.ModuleId = (int)HistoryInfo.Module.Representaive;
            historyInfo.TypeId = (int)HistoryInfo.ActionType.Add;
            historyInfo.RecordId = representativeId;
            historyInfo.ParentRecordId = representativeId;
            historyInfo.ModifiedDate = DateTime.Now;
            historyInfo.Details = new List<HistoryDetailInfo>();
            historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "All", NewValue = "title:" + info.Title + " ,first:" + info.FirstName + " ,last:" + info.LastName + " ,positiontitle:" + info.PositionTitle + " ,username:" + info.UserName });

            history.insertHistory(historyInfo);

            return representativeId;
        }

        public static void updateRepresentative(RepresentativeInfo info, List<string[]> emails, List<string[]> numbers, List<string[]> addresses, List<string[]> othercontacts)
        {
            DataSet ds = new DataSet();
            MySqlDataReader reader = getRepresentative(info.RepresentativeId);
            string[] tbl = new string[6];

            ds.Load(reader, LoadOption.OverwriteChanges, tbl);
            reader.Close();
            reader.Dispose();

            string sql = "update representatives set title=?title, firstname=?firstname, lastname=?lastname, positionTitle=?positionTitle, username=?username where representativeid=?representativeid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("title", info.Title), new MySqlParameter("firstname", info.FirstName), new MySqlParameter("lastname", info.LastName), new MySqlParameter("positionTitle", info.PositionTitle), new MySqlParameter("username", info.UserName), new MySqlParameter("representativeid", info.RepresentativeId));

            HistoryInfo historyInfo = new HistoryInfo();
            HistoryDataProvider history = new HistoryDataProvider();

            historyInfo.UserId = GPSession.UserId;
            historyInfo.ModuleId = (int)HistoryInfo.Module.Representaive;
            historyInfo.TypeId = (int)HistoryInfo.ActionType.Edit;
            historyInfo.RecordId = Convert.ToUInt32(info.RepresentativeId);
            historyInfo.ParentRecordId = Convert.ToUInt32(info.RepresentativeId);
            historyInfo.ModifiedDate = DateTime.Now;
            historyInfo.Details = new List<HistoryDetailInfo>();

            if (ds.Tables[0].Rows[0]["title"].ToString() != info.Title)
            {
                historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "title", OldValue = ds.Tables[0].Rows[0]["title"].ToString(), NewValue = info.Title });
            }
            if (ds.Tables[0].Rows[0]["firstname"].ToString() != info.FirstName)
            {
                historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "firstname", OldValue = ds.Tables[0].Rows[0]["firstname"].ToString(), NewValue = info.FirstName });
            }
            if (ds.Tables[0].Rows[0]["lastname"].ToString() != info.LastName)
            {
                historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "lastname", OldValue = ds.Tables[0].Rows[0]["lastname"].ToString(), NewValue = info.LastName });
            }
            if (ds.Tables[0].Rows[0]["positionTitle"].ToString() != info.PositionTitle)
            {
                historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "positionTitle", OldValue = ds.Tables[0].Rows[0]["positionTitle"].ToString(), NewValue = info.PositionTitle });
            }
            if (ds.Tables[0].Rows[0]["username"].ToString() != info.UserName)
            {
                historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "username", OldValue = ds.Tables[0].Rows[0]["username"].ToString(), NewValue = info.UserName });
            }

            if (historyInfo.Details.Count > 0)
            {
                history.insertHistory(historyInfo);
            }

            //emails
            foreach (string[] email in emails)
            {
                uint emailid = Convert.ToUInt32(email[0]);
                string address = email[1];
                uint emailtypeid = Convert.ToUInt32(email[2]);
                bool defaultemail = Convert.ToBoolean(email[3]);
                updateEmail(info.RepresentativeId, emailid, address, emailtypeid, defaultemail);

                DataRow[] drSeq = ds.Tables[1].Select("emailid=" + emailid);
                if (drSeq.Length > 0)
                {
                    historyInfo = new HistoryInfo();
                    historyInfo.UserId = GPSession.UserId;
                    historyInfo.ModuleId = (int)HistoryInfo.Module.representatives_emails;
                    historyInfo.TypeId = (int)HistoryInfo.ActionType.Edit;
                    historyInfo.RecordId = emailid;
                    historyInfo.ParentRecordId = Convert.ToUInt32(info.RepresentativeId);
                    historyInfo.ModifiedDate = DateTime.Now;
                    historyInfo.Details = new List<HistoryDetailInfo>();

                    if (drSeq[0]["email"].ToString() != address)
                    {
                        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "email", OldValue = drSeq[0]["email"].ToString(), NewValue = address });
                    }
                    if (drSeq[0]["emailtypeid"].ToString() != emailtypeid.ToString())
                    {
                        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "emailtypeid", OldValue = drSeq[0]["emailtypeid"].ToString(), NewValue = emailtypeid.ToString() });
                    }

                    if (Convert.ToBoolean(drSeq[0]["defaultemail"]) != defaultemail)
                    {
                        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "defaultemail", OldValue = drSeq[0]["defaultemail"].ToString(), NewValue = defaultemail.ToString() });
                    }

                    if (historyInfo.Details.Count > 0)
                    {
                        history.insertHistory(historyInfo);
                    }
                }
            }

            //numbers
            foreach (string[] number in numbers)
            {
                string countrycode = number[0];
                uint phonenumberid = Convert.ToUInt32(number[1]);
                string phonenumber = number[2];
                uint phonenumbertypeid = Convert.ToUInt32(number[3]);
                updatePhoneNumber(Convert.ToUInt32(info.RepresentativeId), phonenumberid, phonenumber, phonenumbertypeid, countrycode);

                DataRow[] drSeq = ds.Tables[2].Select("phonenumberid=" + phonenumberid);
                if (drSeq.Length > 0)
                {
                    historyInfo = new HistoryInfo();
                    historyInfo.UserId = GPSession.UserId;
                    historyInfo.ModuleId = (int)HistoryInfo.Module.representatives_phonenumbers;
                    historyInfo.TypeId = (int)HistoryInfo.ActionType.Edit;
                    historyInfo.RecordId = phonenumberid;
                    historyInfo.ParentRecordId = Convert.ToUInt32(info.RepresentativeId);
                    historyInfo.ModifiedDate = DateTime.Now;
                    historyInfo.Details = new List<HistoryDetailInfo>();

                    if (drSeq[0]["number"].ToString() != phonenumber)
                    {
                        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "number", OldValue = drSeq[0]["number"].ToString(), NewValue = phonenumber });
                    }
                    if (drSeq[0]["phonenumbertypeid"].ToString() != phonenumbertypeid.ToString())
                    {
                        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "phonenumbertypeid", OldValue = drSeq[0]["phonenumbertypeid"].ToString(), NewValue = phonenumbertypeid.ToString() });
                    }
                    if (drSeq[0]["countrycode"].ToString() != countrycode.ToString())
                    {
                        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "countrycode", OldValue = drSeq[0]["countrycode"].ToString(), NewValue = countrycode.ToString() });
                    }

                    if (historyInfo.Details.Count > 0)
                    {
                        history.insertHistory(historyInfo);
                    }
                }
            }

            //addresses
            foreach (string[] address in addresses)
            {
                uint addressid = Convert.ToUInt32(address[0]);
                uint addresstypeid = Convert.ToUInt32(address[1]);
                updateAddressType(Convert.ToUInt32(info.RepresentativeId), addressid, addresstypeid);

                DataRow[] drSeq = ds.Tables[3].Select("addressid=" + addressid);
                historyInfo = new HistoryInfo();
                historyInfo.UserId = GPSession.UserId;
                historyInfo.ModuleId = (int)HistoryInfo.Module.representatives_address;
                historyInfo.TypeId = (int)HistoryInfo.ActionType.Edit;
                historyInfo.RecordId = addressid;
                historyInfo.ParentRecordId = Convert.ToUInt32(info.RepresentativeId);
                historyInfo.ModifiedDate = DateTime.Now;
                historyInfo.Details = new List<HistoryDetailInfo>();

                if (drSeq[0]["addresstypeid"].ToString() != addresstypeid.ToString())
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "addresstypeid", NewValue = addresstypeid.ToString(), OldValue = drSeq[0]["addresstypeid"].ToString() });
                }

                if (historyInfo.Details.Count > 0)
                {
                    history.insertHistory(historyInfo);
                }
            }

            //othercontacts
            foreach (string[] othercontact in othercontacts)
            {
                uint othercontactid = Convert.ToUInt32(othercontact[0]);
                uint altertativecontactid = Convert.ToUInt32(othercontact[1]);
                string details = othercontact[2];
                updateOtherContact(othercontactid, details, altertativecontactid);

                DataRow[] drSeq = ds.Tables[4].Select("representativeothercontactid=" + othercontactid);
                if (drSeq.Length > 0)
                {
                    historyInfo = new HistoryInfo();
                    historyInfo.UserId = GPSession.UserId;
                    historyInfo.ModuleId = (int)HistoryInfo.Module.representatives_othercontacts;
                    historyInfo.TypeId = (int)HistoryInfo.ActionType.Edit;
                    historyInfo.RecordId = othercontactid;
                    historyInfo.ParentRecordId = Convert.ToUInt32(info.RepresentativeId);
                    historyInfo.ModifiedDate = DateTime.Now;
                    historyInfo.Details = new List<HistoryDetailInfo>();

                    if (drSeq[0]["alternativecontactid"].ToString() != altertativecontactid.ToString())
                    {
                        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "alternativecontactid", OldValue = drSeq[0]["alternativecontactid"].ToString(), NewValue = altertativecontactid.ToString() });
                    }
                    if (drSeq[0]["details"].ToString() != details)
                    {
                        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "details", OldValue = drSeq[0]["details"].ToString(), NewValue = details });
                    }

                    if (historyInfo.Details.Count > 0)
                    {
                        history.insertHistory(historyInfo);
                    }
                }
            }
        }

        public static MySqlDataReader getAllRepresentative()
        {
            string sql = "select * from representatives";
            return DAO.ExecuteReader(sql, null);
        }

        public static MySqlDataReader getRepresentative(int representativeid)
        {
            string sql = "select * from representatives " +
                "where representativeid = ?representativeid; " +

                "select e.emailid, ce.emailtypeid, e.email, ce.defaultemail " +
                "from representatives_emails as ce " +
                "join emails as e on e.emailid = ce.emailid " +
                "where ce.representativeid = ?representativeid " +
                "order by ce.emailid; " +

                "select pn.phonenumberid, pn.countrycode, cpn.phonenumbertypeid, pn.number " +
                "from representatives_phonenumbers as cpn " +
                "join phonenumbers as pn on pn.phonenumberid = cpn.phonenumberid " +
                "where cpn.representativeid = ?representativeid " +
                "order by pn.phonenumberid; " +

                "select ca.addressid, ca.addresstypeid " +
                "from representatives_address as ca " +
                "where ca.representativeid = ?representativeid " +
                "order by ca.addressid; " +

                "select coc.representativeothercontactid, coc.alternativecontactid, coc.details " +
                "from representatives_othercontacts as coc " +
                "where coc.representativeid = ?representativeid " +
                "order by coc.representativeothercontactid; " +

                "select cn.representativenoteid, cn.createdDate, cn.note, cn.userid, u.username, cn.consultantid, concat_ws(' ',c.first,c.last) as consultant " +
                "from representatives_notes as cn " +
                "join users as u on u.userid = cn.userid " +
                "left join consultants as c on c.consultantid = cn.consultantid " +
                "where cn.representativeid = ?representativeid " +
                "order by cn.createdDate desc, cn.representativenoteid; ";

            return DAO.ExecuteReader(sql, new MySqlParameter("representativeid", representativeid));
        }

        public static MySqlDataReader searchRepresentative(string keyword)
        {
            string sql = "Select Cl.representativeid, Cl.firstname, Cl.lastname from representatives Cl left outer join " +
                                    "representatives_emails CE on Cl.representativeid = CE.representativeid left outer join emails Es on CE.emailid = Es.emailid " +
                                    "where ((Cl.firstname like concat_ws(?keyword,'%','%') or Cl.lastname like concat_ws(?keyword,'%','%') ) ) or Es.email like concat_ws(?keyword,'%','%') " +
                                    "group by Cl.firstname, Cl.lastname order by Cl.firstname;";
            return DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword));
        }

        public static void addEmail(int representativeid, string email, int emailtypeid, bool defaultemail)
        {
            uint emailid = EmailDataProvider.insertEmail(email);
            string sql = "insert into representatives_emails (representativeid, emailid, emailtypeid, defaultemail) values (?representativeid, ?emailid, ?emailtypeid, ?defaultemail)";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("representativeid", representativeid), new MySqlParameter("emailid", emailid), new MySqlParameter("emailtypeid", emailtypeid), new MySqlParameter("defaultemail", defaultemail));
            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.representatives_emails;
            info.TypeId = (int)HistoryInfo.ActionType.Add;
            info.RecordId = emailid;
            info.ParentRecordId = Convert.ToUInt32(representativeid);
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "All", NewValue = "emailid:" + emailid + " ,email:" + email + " ,representativeid:" + representativeid + " ,emailtypeid:" + emailtypeid + " ,defaultemail:" + defaultemail });

            history.insertHistory(info);
        }

        public static void updateEmail(int representativeid, uint emailid, string email, uint emailtypeid, bool defaultemail)
        {
            EmailDataProvider.updateEmail(emailid, email);
            string sql = "update representatives_emails set " +
                "emailtypeid = ?emailtypeid, " +
                "defaultemail = ?defaultemail " +
                "where representativeid = ?representativeid " +
                "and emailid = ?emailid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("representativeid", representativeid), new MySqlParameter("emailid", emailid), new MySqlParameter("emailtypeid", emailtypeid), new MySqlParameter("defaultemail", defaultemail));
        }

        public static void removeEmail(uint representativeid, uint emailid)
        {
            string email = EmailDataProvider.getEmail(emailid);
            string sql = "delete from representatives_emails where representativeid = ?representativeid and emailid = ?emailid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("representativeid", representativeid), new MySqlParameter("emailid", emailid));
            EmailDataProvider.removeEmail(emailid);

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.representatives_emails;
            info.TypeId = (int)HistoryInfo.ActionType.Delete;
            info.RecordId = emailid;
            info.ParentRecordId = representativeid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "Delete", NewValue = email });

            history.insertHistory(info);
        }

        public static void addPhoneNumber(uint representativeid, string number, uint phonenumbertypeid, string countrycode)
        {
            uint phonenumberid = PhoneNumberDataProvider.insertPhoneNumber(number, countrycode);
            string sql = "insert into representatives_phonenumbers (representativeid, phonenumberid, phonenumbertypeid) values (?representativeid, ?phonenumberid, ?phonenumbertypeid)";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("representativeid", representativeid), new MySqlParameter("phonenumberid", phonenumberid), new MySqlParameter("phonenumbertypeid", phonenumbertypeid));
            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.representatives_phonenumbers;
            info.TypeId = (int)HistoryInfo.ActionType.Add;
            info.RecordId = phonenumberid;
            info.ParentRecordId = representativeid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "All", NewValue = "phonenumberid:" + phonenumberid + " ,number:" + number + " ,representativeid:" + representativeid + " ,phonenumbertypeid:" + phonenumbertypeid });

            history.insertHistory(info);
        }

        public static void updatePhoneNumber(uint representativeid, uint phonenumberid, string number, uint phonenumbertypeid, string countrycode)
        {
            PhoneNumberDataProvider.updatePhoneNumber(phonenumberid, number, countrycode);
            string sql = "update representatives_phonenumbers set " +
                "phonenumbertypeid = ?phonenumbertypeid " +
                "where representativeid = ?representativeid " +
                "and phonenumberid = ?phonenumberid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("representativeid", representativeid), new MySqlParameter("phonenumberid", phonenumberid), new MySqlParameter("phonenumbertypeid", phonenumbertypeid));
        }

        public static void removePhoneNumber(uint representativeid, uint phonenumberid)
        {
            string number = PhoneNumberDataProvider.getPhoneNumber(phonenumberid);
            string sql = "delete from representatives_phonenumbers where representativeid = ?representativeid and phonenumberid = ?phonenumberid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("representativeid", representativeid), new MySqlParameter("phonenumberid", phonenumberid));
            PhoneNumberDataProvider.removePhoneNumber(phonenumberid);

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.representatives_phonenumbers;
            info.TypeId = (int)HistoryInfo.ActionType.Delete;
            info.RecordId = phonenumberid;
            info.ParentRecordId = representativeid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "Delete", NewValue = number });

            history.insertHistory(info);
        }

        public static void addAddress(uint representative, uint addresstypeid, uint addressid)
        {
            string sql = "insert into representatives_address (representativeid, addressid, addresstypeid) values (?representativeid, ?addressid, ?addresstypeid)";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("representativeid", representative), new MySqlParameter("addressid", addressid), new MySqlParameter("addresstypeid", addresstypeid));

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.representatives_address;
            info.TypeId = (int)HistoryInfo.ActionType.Add;
            info.RecordId = addressid;
            info.ParentRecordId = representative;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "All", NewValue = "addressid:" + addressid + " ,representativeid:" + representative + ", addresstypeId:" + addresstypeid });

            history.insertHistory(info);
        }

        public static void updateAddressType(uint representativeid, uint addressid, uint addresstypeid)
        {
            string sql = "update representatives_address set addresstypeid = ?addresstypeid where representativeid = ?representativeid and addressid = ?addressid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("representativeid", representativeid), new MySqlParameter("addressid", addressid), new MySqlParameter("addresstypeid", addresstypeid));
        }

        public static void removeAddress(uint representativeid, uint addressid)
        {
            string address = AddressDataProvider.getAddressHTML(addressid);
            string sql = "delete from representatives_address where representativeid = ?representativeid and addressid = ?addressid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("representativeid", representativeid), new MySqlParameter("addressid", addressid));
            AddressDataProvider.removeAddress(addressid);

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.representatives_address;
            info.TypeId = (int)HistoryInfo.ActionType.Delete;
            info.RecordId = addressid;
            info.ParentRecordId = representativeid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "Delete", NewValue = address });

            history.insertHistory(info);
        }

        public static void addOtherContact(uint representativeid, string details, uint alternativecontactid)
        {
            string sql = "insert into representatives_othercontacts (representativeid, alternativecontactid, details) values (?representativeid, ?alternativecontactid, ?details);select last_insert_id()";
            uint id = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("representativeid", representativeid), new MySqlParameter("alternativecontactid", alternativecontactid), new MySqlParameter("details", details)));

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.candidates_othercontacts;
            info.TypeId = (int)HistoryInfo.ActionType.Add;
            info.RecordId = id;
            info.ParentRecordId = representativeid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "All", NewValue = "alternativecontactid:" + alternativecontactid + " ,representativeid:" + representativeid + " ,details:" + details });

            history.insertHistory(info);
        }

        public static void updateOtherContact(uint representativeothercontactid, string details, uint alternativecontactid)
        {
            string sql = "update representatives_othercontacts set " +
                "alternativecontactid = ?alternativecontactid, " +
                "details = ?details " +
                "where representativeothercontactid = ?representativeothercontactid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("representativeothercontactid", representativeothercontactid), new MySqlParameter("alternativecontactid", alternativecontactid), new MySqlParameter("details", details));
        }

        public static void removeOtherContact(uint representativeid, uint representativeothercontactid)
        {
            string sql = "select details from representatives_othercontacts where representativeothercontactid = ?representativeothercontactid";
            string detail = (string)DAO.ExecuteScalar(sql, new MySqlParameter("representativeothercontactid", representativeothercontactid));
            sql = "delete from representatives_othercontacts where representativeothercontactid = ?representativeothercontactid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("representativeothercontactid", representativeothercontactid));

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.representatives_othercontacts;
            info.TypeId = (int)HistoryInfo.ActionType.Delete;
            info.RecordId = representativeothercontactid;
            info.ParentRecordId = representativeid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "Delete", NewValue = detail });

            history.insertHistory(info);
        }

        public static void addNote(uint representativeid, uint userid, string note, uint? consultantid)
        {
            string sql = "insert into representatives_notes (representativeid, createdDate, note, userid, consultantid) values (?representativeid, utc_timestamp(), ?note, ?userid, ?consultantid);select last_insert_id()";
            MySqlParameter sqlOnBehalfOf = new MySqlParameter("consultantid", MySqlDbType.UInt32);
            if (consultantid == null)
            {
                sqlOnBehalfOf.Value = DBNull.Value;
            }
            else
            {
                sqlOnBehalfOf.Value = consultantid;
            }
            uint id = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("representativeid", representativeid), new MySqlParameter("note", note), new MySqlParameter("userid", userid), sqlOnBehalfOf));

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.representatives_notes;
            info.TypeId = (int)HistoryInfo.ActionType.Add;
            info.RecordId = id;
            info.ParentRecordId = representativeid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo
            {
                ColumnName = "All",
                NewValue = "representativeid:" + representativeid + " ,note:" + note + " ,userid:" + userid
            });

            history.insertHistory(info);
        }
    }
}