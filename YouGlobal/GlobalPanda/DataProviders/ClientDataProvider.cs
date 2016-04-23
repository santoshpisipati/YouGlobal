using GlobalPanda.BusinessInfo;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

/// <summary>
/// Summary description for ClientDataProvider
/// </summary>
namespace GlobalPanda.DataProviders
{
    public class ClientDataProvider
    {
        public static uint insertClient(string clientname)
        {
            string sql = "insert into client(ClientName) values (?clientname); select last_insert_id()";
            uint clientid = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("clientname", clientname)));

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.Client;
            info.TypeId = (int)HistoryInfo.ActionType.Add;
            info.RecordId = clientid;
            info.ParentRecordId = clientid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "All", NewValue = "ClientName:" + clientname });

            history.insertHistory(info);
            return clientid;
        }

        public static Boolean verifyClientName(uint clientid, string clientname)
        {
            Boolean flag = false;
            string sql = "select clientname from client where clientid<>?clientid and clientname=?clientname";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("clientid", clientid), new MySqlParameter("clientname", clientname));
            if (reader.HasRows)
                flag = true;
            reader.Close();
            reader.Dispose();
            return flag;
        }

        public static Boolean verifyEmail(uint emailid, string email)
        {
            Boolean flag = false;
            string sql = "select e.emailid, e.email from emails as e join client_emails as ce on e.emailid = ce.emailid where e.email=?email and e.emailid<>?emailid;";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("emailid", emailid), new MySqlParameter("email", email));
            if (reader.HasRows)
                flag = true;
            reader.Close();
            reader.Dispose();
            return flag;
        }

        public static void updateClient(uint clientid, string clientname, string address, string phone, string email, string website)
        {
            string sql = "update client set " +
                "ClientName = ?clientname, " +
                "Address = ?address, " +
                "Phone = ?phone, " +
                "Email = ?email " +
                "Website = ?website " +
                "where ClientID = ?clientid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("clientid", clientid), new MySqlParameter("clientname", clientname), new MySqlParameter("address", address), new MySqlParameter("phone", phone), new MySqlParameter("email", email), new MySqlParameter("website", website));
        }

        public static MySqlDataReader searchClient(string keyword)
        {
            string sql = "Select Cl.clientid,Cl.ClientName from client Cl left outer join " +
                         "client_emails CE on Cl.clientid = CE.clientid left outer join emails Es on CE.emailid = Es.emailid " +
                         "where (Cl.ClientName like concat_ws(?keyword,'%','%')) or Es.email like concat_ws(?keyword,'%','%') " +
                         "group by Cl.ClientName order by Cl.ClientName;";
            return DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword));
        }

        public static void addEmail(uint candidateid, string email, uint emailtypeid)
        {
            addEmail(candidateid, email, emailtypeid, false);
        }

        public static void addEmail(uint clientid, string email, uint emailtypeid, bool defaultemail)
        {
            uint emailid = EmailDataProvider.insertEmail(email);
            string sql = "insert into client_emails (clientid, emailid, emailtypeid, defaultemail) values (?clientid, ?emailid, ?emailtypeid, ?defaultemail);select last_insert_id()";
            uint id = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("clientid", clientid), new MySqlParameter("emailid", emailid), new MySqlParameter("emailtypeid", emailtypeid), new MySqlParameter("defaultemail", defaultemail)));

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.client_emails;
            info.TypeId = (int)HistoryInfo.ActionType.Add;
            info.RecordId = id;
            info.ParentRecordId = clientid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "All", NewValue = "emailid:" + emailid + " ,email:" + email + " ,clientid:" + clientid + " ,emailtypeid:" + emailtypeid + " ,defaultemail:" + defaultemail });

            history.insertHistory(info);
        }

        public static void updateEmail(uint clientid, uint emailid, string email, uint emailtypeid, bool defaultemail)
        {
            EmailDataProvider.updateEmail(emailid, email);
            string sql = "update client_emails set " +
                "emailtypeid = ?emailtypeid, " +
                "defaultemail = ?defaultemail " +
                "where clientid = ?clientid " +
                "and emailid = ?emailid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("clientid", clientid), new MySqlParameter("emailid", emailid), new MySqlParameter("emailtypeid", emailtypeid), new MySqlParameter("defaultemail", defaultemail));
        }

        public static void removeEmail(uint clientid, uint emailid)
        {
            string email = EmailDataProvider.getEmail(emailid);
            string sql = "delete from client_emails where clientid = ?clientid and emailid = ?emailid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("clientid", clientid), new MySqlParameter("emailid", emailid));
            EmailDataProvider.removeEmail(emailid);

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.client_emails;
            info.TypeId = (int)HistoryInfo.ActionType.Delete;
            info.RecordId = emailid;
            info.ParentRecordId = clientid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "Delete", NewValue = email });

            history.insertHistory(info);
        }

        public static void addPhone(uint clientid, string phonenumber, uint phonenumbertypeid, string countrycode)
        {
            uint phonenumberid = PhoneNumberDataProvider.insertPhoneNumber(phonenumber, countrycode);
            string sql = "insert into client_phonenumbers (clientid, phonenumberid, phonenumbertypeid) values (?clientid, ?phonenumberid, ?phonenumbertypeid);select last_insert_id()";
            uint id = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("clientid", clientid), new MySqlParameter("phonenumberid", phonenumberid), new MySqlParameter("phonenumbertypeid", phonenumbertypeid), new MySqlParameter("countrycode", countrycode)));

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.client_phonenumbers;
            info.TypeId = (int)HistoryInfo.ActionType.Add;
            info.RecordId = id;
            info.ParentRecordId = clientid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "All", NewValue = "phonenumberid:" + phonenumberid + " ,number:" + phonenumber + " ,clientid:" + clientid + " ,phonenumbertypeid:" + phonenumbertypeid });

            history.insertHistory(info);
        }

        public static void updatePhoneNumber(uint clientid, uint phonenumberid, string number, uint phonenumbertypeid, string countrycode)
        {
            PhoneNumberDataProvider.updatePhoneNumber(phonenumberid, number, countrycode);
            string sql = "update client_phonenumbers set " +
                "phonenumbertypeid = ?phonenumbertypeid " +
                "where clientid = ?clientid " +
                "and phonenumberid = ?phonenumberid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("clientid", clientid), new MySqlParameter("phonenumberid", phonenumberid), new MySqlParameter("phonenumbertypeid", phonenumbertypeid));
        }

        public static void removePhoneNumber(uint clientid, uint phonenumberid)
        {
            string number = PhoneNumberDataProvider.getPhoneNumber(phonenumberid);
            string sql = "delete from client_phonenumbers where clientid = ?clientid and phonenumberid = ?phonenumberid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("clientid", clientid), new MySqlParameter("phonenumberid", phonenumberid));
            PhoneNumberDataProvider.removePhoneNumber(phonenumberid);

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.client_phonenumbers;
            info.TypeId = (int)HistoryInfo.ActionType.Delete;
            info.RecordId = phonenumberid;
            info.ParentRecordId = clientid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "Delete", NewValue = number });

            history.insertHistory(info);
        }

        public static void addAddress(uint clientid, uint addressid, uint addresstypeid)
        {
            string sql = "insert into client_addresses (clientid, addressid, addresstypeid) values (?clientid, ?addressid, ?addresstypeid)";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("clientid", clientid), new MySqlParameter("addressid", addressid), new MySqlParameter("addresstypeid", addresstypeid));
        }

        public static void addRepresentative(uint clientid, uint candidateid, DateTime startdate, DateTime enddate)
        {
            string sql = "insert into client_representatives (clientid, candidateid, startdate, enddate) values (?clientid, ?candidateid, ?startdate, ?enddate);select last_insert_id()";
            uint id = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("clientid", clientid), new MySqlParameter("candidateid", candidateid), new MySqlParameter("startdate", startdate), new MySqlParameter("enddate", enddate)));
        }

        public static void addRepresentative(uint clientid, uint representativeid, DateTime startdate)
        {
            string sql = "insert into client_representatives (clientid, representativeid, startdate) values (?clientid, ?representativeid, ?startdate);select last_insert_id()";
            uint id = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("clientid", clientid), new MySqlParameter("representativeid", representativeid), new MySqlParameter("startdate", startdate)));

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.client_representatives;
            info.TypeId = (int)HistoryInfo.ActionType.Add;
            info.RecordId = id;
            info.ParentRecordId = clientid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "All", NewValue = "representativeid:" + representativeid + " ,clientid:" + clientid + " ,startdate:" + startdate });

            history.insertHistory(info);
        }

        public static void updateRepresentative(uint clientid, uint representativeid, DateTime startdate, DateTime enddate)
        {
            string sql = "update client_representatives set " +
                "startdate = ?startdate, " +
                "enddate = ?enddate " +
                "where clientid = ?clientid " +
                "and representativeid = ?representativeid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("clientid", clientid), new MySqlParameter("representativeid", representativeid), new MySqlParameter("startdate", startdate), new MySqlParameter("enddate", enddate));
        }

        public static void updateRepresentative(uint clientid, uint representativeid, DateTime enddate)
        {
            string sql = "select * from client_representatives where clientid=?clientid and representativeid=?representativeid";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("clientid", clientid), new MySqlParameter("representativeid", representativeid));

            sql = "update client_representatives set " +
            "enddate = ?enddate " +
            "where clientid = ?clientid " +
            "and representativeid = ?representativeid";

            DAO.ExecuteNonQuery(sql, new MySqlParameter("clientid", clientid), new MySqlParameter("representativeid", representativeid), new MySqlParameter("enddate", enddate));

            if (reader.HasRows)
            {
                reader.Read();
                HistoryInfo historyInfo = new HistoryInfo();
                HistoryDataProvider history = new HistoryDataProvider();
                historyInfo.UserId = GPSession.UserId;
                historyInfo.ModuleId = (int)HistoryInfo.Module.client_representatives;
                historyInfo.TypeId = (int)HistoryInfo.ActionType.Edit;
                historyInfo.RecordId = Convert.ToUInt32(DAO.getUInt(reader, "id"));
                historyInfo.ParentRecordId = clientid;
                historyInfo.ModifiedDate = DateTime.Now;
                historyInfo.Details = new List<HistoryDetailInfo>();

                if (reader["enddate"].ToString() != enddate.ToString())
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "enddate:", OldValue = reader["enddate"].ToString(), NewValue = enddate.ToString() });
                }

                if (historyInfo.Details.Count > 0)
                {
                    history.insertHistory(historyInfo);
                }
            }
            reader.Close();
            reader.Dispose();
        }

        public static void removeRepresentative(uint clientid, uint representativeid)
        {
            string sql = "delete from client_representatives where clientid = ?clientid and representativeid = ?representativeid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("clientid", clientid), new MySqlParameter("representativeid", representativeid));
        }

        public static MySqlDataReader listActiveRepresentatives(uint clientid)
        {
            string sql = "Select representativeid,concat(firstname,' ',lastname) as representative from representatives " +
                         "where representativeid not in(Select representativeid from client_representatives where clientid = ?clientid) " +
                         "order by representative;";
            return DAO.ExecuteReader(sql, new MySqlParameter("clientid", clientid));
        }

        public static MySqlDataReader searchActiveRepresentatives(uint clientid, string keyword)
        {
            string sql = "Select representativeid,concat(firstname,' ',lastname) as representative from representatives " +
                         "where (firstname like concat_ws(?keyword,'%','%') or lastname like concat_ws(?keyword,'%','%') ) AND representativeid not in(Select representativeid from client_representatives where clientid = ?clientid) " +
                         "order by representative;";
            return DAO.ExecuteReader(sql, new MySqlParameter("clientid", clientid), new MySqlParameter("keyword", keyword));
        }

        public static MySqlDataReader listActiveClient()
        {
            string sql = "Select clientid,clientname from client order by clientname";
            return DAO.ExecuteReader(sql);
        }

        public static MySqlDataReader listConsultantClient(int consultantid)
        {
            string sql = "Select c.clientid,clientname from client c inner join client_consultants cc on cc.clientid=c.clientid where cc.consultantid=?consultantid order by clientname";
            return DAO.ExecuteReader(sql, new MySqlParameter("consultantid", consultantid));
        }

        public static MySqlDataReader getClient(uint clientid)
        {
            string sql = "select clientid,clientname," +
                "status from client where clientid = ?clientid;" +

                "select e.emailid, ce.emailtypeid, e.email, ce.defaultemail " +
                "from client_emails as ce join emails as e on e.emailid = ce.emailid " +
                "where ce.clientid = ?clientid order by ce.emailid;" +

                "select pn.phonenumberid, cpn.phonenumbertypeid, pn.number, pn.countryCode " +
                "from client_phonenumbers as cpn " +
                "join phonenumbers as pn on pn.phonenumberid = cpn.phonenumberid " +
                "where cpn.clientid = ?clientid " +
                "order by pn.phonenumberid;" +

                "Select Cl.id, Cl.clientid,Cl.representativeid,Concat(C.firstname,' ',C.lastname) as candidate," +
                "Date_Format(Cl.startdate, '%d/%b/%Y') as startdate,Date_Format(Cl.enddate, '%d/%m/%Y') as enddate from " +
                "client_representatives Cl inner join representatives C " +
                "on C.representativeid = Cl.representativeid Where Cl.clientid = ?clientid order by candidate;" +

                "Select Cl.consultantid,C.first,C.last,C.nickname from " +
                "client_consultants Cl inner join consultants C " +
                " on C.consultantid = Cl.consultantid Where Cl.clientid = ?clientid;";

            return DAO.ExecuteReader(sql, new MySqlParameter("clientid", clientid));
        }

        public static MySqlDataReader getClientOnly(uint clientid)
        {
            string sql = "select clientid,clientname," +
                "status from client where clientid = ?clientid;";

            return DAO.ExecuteReader(sql, new MySqlParameter("clientid", clientid));
        }

        public static void updateClient(uint clientid, string clientname, string status, List<string[]> emails, List<string[]> numbers)
        {
            string sql;
            HistoryInfo historyInfo = new HistoryInfo();
            HistoryDataProvider history = new HistoryDataProvider();
            DataSet ds = new DataSet();
            MySqlDataReader reader = getClient(clientid);
            string[] tbl = new string[4];

            ds.Load(reader, LoadOption.OverwriteChanges, tbl);
            reader.Close();
            reader.Dispose();
            //client
            sql = @"update client set
                    ClientName = ?client,
                    status = ?status
                    where ClientID = ?clientid;";

            DAO.ExecuteNonQuery(sql, new MySqlParameter("clientid", clientid), new MySqlParameter("client", clientname), new MySqlParameter("status", status));
            historyInfo.UserId = GPSession.UserId;
            historyInfo.ModuleId = (int)HistoryInfo.Module.Client;
            historyInfo.TypeId = (int)HistoryInfo.ActionType.Edit;
            historyInfo.RecordId = clientid;
            historyInfo.ParentRecordId = clientid;
            historyInfo.ModifiedDate = DateTime.Now;
            historyInfo.Details = new List<HistoryDetailInfo>();

            if (ds.Tables[0].Rows[0]["clientname"].ToString() != clientname)
            {
                historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "clientname:", OldValue = ds.Tables[0].Rows[0]["clientname"].ToString(), NewValue = clientname });
            }

            if (historyInfo.Details.Count > 0)
            {
                history.insertHistory(historyInfo);
            }

            foreach (string[] email in emails)
            {
                uint emailid = Convert.ToUInt32(email[0]);
                string address = email[1];
                uint emailtypeid = Convert.ToUInt32(email[2]);
                bool defaultemail = Convert.ToBoolean(email[3]);
                updateEmail(clientid, emailid, address, emailtypeid, defaultemail);

                DataRow[] drSeq = ds.Tables[1].Select("emailid=" + emailid);
                if (drSeq.Length > 0)
                {
                    historyInfo = new HistoryInfo();
                    historyInfo.UserId = GPSession.UserId;
                    historyInfo.ModuleId = (int)HistoryInfo.Module.client_emails;
                    historyInfo.TypeId = (int)HistoryInfo.ActionType.Edit;
                    historyInfo.RecordId = emailid;
                    historyInfo.ParentRecordId = clientid;
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
                updatePhoneNumber(clientid, phonenumberid, phonenumber, phonenumbertypeid, countrycode);

                DataRow[] drSeq = ds.Tables[2].Select("phonenumberid=" + phonenumberid);
                if (drSeq.Length > 0)
                {
                    historyInfo = new HistoryInfo();
                    historyInfo.UserId = GPSession.UserId;
                    historyInfo.ModuleId = (int)HistoryInfo.Module.client_phonenumbers;
                    historyInfo.TypeId = (int)HistoryInfo.ActionType.Edit;
                    historyInfo.RecordId = phonenumberid;
                    historyInfo.ParentRecordId = clientid;
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
                    if (drSeq[0]["countrycode"].ToString() != countrycode)
                    {
                        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "countrycode", OldValue = drSeq[0]["countrycode"].ToString(), NewValue = countrycode });
                    }

                    if (historyInfo.Details.Count > 0)
                    {
                        history.insertHistory(historyInfo);
                    }
                }
            }
        }

        public static void addConsultant(uint clientId, uint consultantid)
        {
            string sql = "insert into client_consultants (clientid,consultantid) values (?clientid,?consultantid);select last_insert_id()";
            uint id = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("consultantid", consultantid), new MySqlParameter("clientid", clientId)));

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.client_consultants;
            info.TypeId = (int)HistoryInfo.ActionType.Add;
            info.RecordId = id;
            info.ParentRecordId = clientId;
            info.ModifiedDate = DateTime.UtcNow;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "All", NewValue = "clientid:" + clientId + " ,consultantid:" + consultantid });

            history.insertHistory(info);
        }

        public static void removeConsultant(uint clientid, uint consultantid, string clientname, string consultantname)
        {
            string sql = "select client_consultantid from client_consultants where clientid=?clientid and consultantid=?consultantid";
            sql = "delete from client_consultants where clientid=?clientid and consultantid=?consultantid";
            uint id = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("consultantid", consultantid), new MySqlParameter("clientid", clientid)));

            DAO.ExecuteNonQuery(sql, new MySqlParameter("clientid", clientid), new MySqlParameter("consultantid", consultantid));

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.client_consultants;
            info.TypeId = (int)HistoryInfo.ActionType.Delete;
            info.RecordId = id;
            info.ParentRecordId = clientid;
            info.ModifiedDate = DateTime.UtcNow;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "Delete", NewValue = consultantname + " " + consultantid.ToString() + " was disassociated with " + clientname + " (" + clientid.ToString() + ")", OldValue = string.Empty });

            history.insertHistory(info);
        }

        public static DataTable getClientConsultant(uint clientId)
        {
            DataTable dt = new DataTable();
            string sql = "Select Cl.consultantid,C.first,C.last,C.nickname from " +
                "client_consultants Cl inner join consultants C " +
                " on C.consultantid = Cl.consultantid Where Cl.clientid = ?clientid;";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("clientId", clientId));
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }
    }
}