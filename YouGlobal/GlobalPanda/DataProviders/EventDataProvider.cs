using GlobalPanda.BusinessInfo;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

/// <summary>
/// Summary description for EventDataProvider
/// </summary>
///
namespace GlobalPanda.DataProviders
{
    public class EventDataProvider
    {
        public static int insertEvent(string name, string description, DateTime eventDate)
        {
            int id = 0;
            string sql = "insert into events (name,description,eventDate,createdDate,userId) values (?name,?description,?eventDate,?createdDate,?userId);select last_insert_id()";
            id = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("name", name), new MySqlParameter("description", description), new MySqlParameter("eventDate", eventDate),
                new MySqlParameter("createdDate", DateTime.Today), new MySqlParameter("userId", GPSession.UserId)));

            return id;
        }

        public static int insertEvent(EventInfo info)
        {
            int id = 0;
            string sql = "insert into events (category,name,description,eventDate,createdDate,userId,enddate,private,allday,remainder,recurred,timezoneid,recurredType,recurredEndDate) " +
                " values (?category,?name,?description,?eventDate,?createdDate,?userId,?enddate,?private,?allday,?remainder,?recurred,?timezoneid,?recurredType,?recurredEndDate);select last_insert_id()";
            id = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("category", info.Category), new MySqlParameter("name", info.EventName), new MySqlParameter("description", info.Description), new MySqlParameter("eventDate", info.EventDate),
            new MySqlParameter("createdDate", DateTime.Today), new MySqlParameter("userId", GPSession.UserId), new MySqlParameter("enddate", info.EndDate), new MySqlParameter("private", info.Private), new MySqlParameter("allday", info.AllDay),
            new MySqlParameter("remainder", info.Remainder), new MySqlParameter("recurred", info.Recurred), new MySqlParameter("timezoneid", info.TimezoneId), new MySqlParameter("recurredType", info.RecurredType), new MySqlParameter("recurredEndDate", info.RecurredEndDate)));

            if (info.PermissionList != null)
            {
                foreach (EventPermissionInfo permission in info.PermissionList)
                {
                    insertEventPermission(id, permission.UserRoleId, permission.View, permission.Edit);
                }
            }
            return id;
        }

        public static void updateEvent(int eventId, string name, string description, bool Private, bool allDay, bool recurred, string timezoneid)
        {
            string sql = "update events set name=?name,description=?description,private=?private,allDay=?allDay,recurred=?recurred,timezoneid=?timezoneid where eventId=?eventId";
            DAO.ExecuteScalar(sql, new MySqlParameter("eventId", eventId), new MySqlParameter("name", name), new MySqlParameter("description", description), new MySqlParameter("private", Private),
                new MySqlParameter("allDay", allDay), new MySqlParameter("recurred", recurred), new MySqlParameter("timezoneid", timezoneid));
        }

        public static void updateEvent(int eventId, string name, string description, DateTime eventDate)
        {
            string sql = "update events set name=?name,description=?description,eventDate=?eventDate where eventId=?eventId";
            DAO.ExecuteScalar(sql, new MySqlParameter("eventId", eventId), new MySqlParameter("name", name), new MySqlParameter("description", description), new MySqlParameter("eventDate", eventDate));
        }

        public static void updateEventEndDate(int eventId, DateTime endDate)
        {
            string sql = "update events set endDate=?endDate where eventId=?eventId";
            DAO.ExecuteScalar(sql, new MySqlParameter("eventId", eventId), new MySqlParameter("endDate", endDate));
        }

        public static void updateEvent(EventInfo info)
        {
            string sql = "update events set name=?name,description=?description,eventDate=?eventDate,enddate=?enddate,private=?private,allday=?allday,remainder=?remainder,recurred=?recurred,timezoneid=?timezoneid,recurredType=?recurredType,recurredEndDate=?recurredEndDate" +
                    " where eventId=?eventId";
            DAO.ExecuteScalar(sql, new MySqlParameter("eventId", info.EventId), new MySqlParameter("name", info.EventName), new MySqlParameter("description", info.Description), new MySqlParameter("eventDate", info.EventDate),
                new MySqlParameter("enddate", info.EndDate), new MySqlParameter("private", info.Private), new MySqlParameter("allday", info.AllDay), new MySqlParameter("remainder", info.Remainder),
                new MySqlParameter("recurred", info.Recurred), new MySqlParameter("timezoneid", info.TimezoneId), new MySqlParameter("recurredType", info.RecurredType), new MySqlParameter("recurredEndDate", info.RecurredEndDate));

            if (info.PermissionList != null)
            {
                foreach (EventPermissionInfo permission in info.PermissionList)
                {
                    if (permission.PermissionId == 0)
                        insertEventPermission(info.EventId, permission.UserRoleId, permission.View, permission.Edit);
                    else
                        updateEventPermission(info.EventId, permission.UserRoleId, permission.View, permission.Edit);
                }
            }
        }

        public static void deleteEvent(int eventid)
        {
            string sql = "delete from event_permission where eventid=?eventid; ";
            sql += " delete  from events where eventid=?eventid; ";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("eventid", eventid));
        }

        public static void insertEventPermission(int eventId, int userroleid, bool view, bool edit)
        {
            string sql = "insert into event_permission (eventid,userroleid,view,edit) values (?eventid,?userroleid,?view,?edit);";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("eventid", eventId), new MySqlParameter("userroleid", userroleid), new MySqlParameter("view", view), new MySqlParameter("edit", edit));
        }

        public static void updateEventPermission(int eventId, int userroleid, bool view, bool edit)
        {
            string sql = "update event_permission set view=?view,edit=?edit where eventid=?eventid and userroleid=?userroleid;";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("eventid", eventId), new MySqlParameter("userroleid", userroleid), new MySqlParameter("view", view), new MySqlParameter("edit", edit));
        }

        public static DataTable getEventPermission(int eventId)
        {
            string sql = "Select event_permissionid,eventid,r.userroleid, view,  edit,r.name as role " +
                " from event_permission p right outer join userroles r on p.userroleid=r.userroleid where (eventid=?eventid or ?eventid=0);";
            if (eventId == 0)
                sql = "Select 0 as event_permissionid,0 as eventid,userroleid,0 as view, 0 as edit,name as role  from  userroles;";

            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("eventid", eventId));
            DataTable dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public static void insertCandidateEvent(int candidateid, string name, string description, DateTime eventDate)
        {
            int eventId = insertEvent(name, description, eventDate);
            string sql = "insert into candidate_events (candidateid,eventid) values (?candidateId,?eventId)";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateId", candidateid), new MySqlParameter("eventid", eventId));
        }

        public static void updateCandidateEvent(int candidateid, int eventId, string name, string description, DateTime eventDate)
        {
            updateEvent(eventId, name, description, eventDate);
        }

        public static int insertCandidateJobEvent(int candidateid, int jobid, EventInfo info)
        {
            int eventId = insertEvent(info);
            string sql = "insert into candidatejob_event (candidateid,jobid,eventid) values (?candidateId,?jobid,?eventId)";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateId", candidateid), new MySqlParameter("jobid", jobid), new MySqlParameter("eventid", eventId));

            return eventId;
        }

        public static MySqlDataReader getCandidateJobEvent(int candidateid, int jobid)
        {
            string sql = "select e.eventid,e.name,e.description,e.eventdate,Date_Format(e.eventdate,'%d/%m/%Y') as eventdate_format,e.createddate,e.private,e.enddate,Date_Format(e.enddate,'%d/%m/%Y') as enddate_format,e.allday,e.remainder,e.recurred "
                + " ,Date_Format(e.eventdate,'%H:%i') as eventtime,Date_Format(e.enddate,'%H:%i') as endtime "
                + " from events e inner join candidatejob_event ce on ce.eventid=e.eventid where ce.candidateid=?candidateid and ce.jobid=?jobid";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("jobid", jobid));

            return reader;
        }

        public static MySqlDataReader getCandidateJobEvent(int candidateid)
        {
            string sql = "select e.eventid,e.name,e.description,Date_Format(e.eventdate,'%d-%b-%Y') as eventdate,Date_Format(e.eventdate,'%d-%m-%Y') as eventdate_format,e.createddate,e.private,j.jobdetailid,j.referenceno,c.title,c.first,c.last "
                + " from events e inner join candidatejob_event ce on ce.eventid=e.eventid inner join jobdetail j on ce.jobid=j.jobdetailid inner join candidates c on ce.candidateid=c.candidateid where ce.candidateid=?candidateid";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("candidateid", candidateid));

            return reader;
        }

        public static int getCandidateIdByEvent(int eventId)
        {
            int candidateId = 0;
            string sql = "select candidateid from candidatejob_event where eventid=?eventid";
            candidateId = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("eventid", eventId)));
            return candidateId;
        }

        public static DataTable listAllEvents()
        {
            string sql = "select * from events";
            MySqlDataReader reader = DAO.ExecuteReader(sql);
            DataTable dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public static DataTable listCandidateEvents()
        {
            string sql = "select e.*,c.title,c.first,c.middle,c.last from events e inner join candidate_events ce on ce.eventId=e.eventId inner join candidates c on ce.candidateid=c.candidateid";
            MySqlDataReader reader = DAO.ExecuteReader(sql);
            DataTable dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        //this method retrieves all events within range start-end
        public static List<EventInfo> getEvents(DateTime start, DateTime end)
        {
            List<EventInfo> events = new List<EventInfo>();
            string sql = "SELECT eventid, description, name,  eventdate,eventdate as startactual,recurredEndDate "
                + " ,enddate,category,allDay,private,recurred,timezoneid, COALESCE(recurredtype,0) as recurredtype FROM events where (eventDate>=?start or recurred) AND (eventDate<=?end or recurred)";

            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("start", start), new MySqlParameter("end", end));
            DateTime eventDate = new DateTime();
            int startMonth = start.Month, endMonth = end.Month;
            DataTable dtEvent = new DataTable();
            dtEvent.Load(reader);
            dtEvent.Constraints.Clear();

            DataRow[] drSqStart = dtEvent.Select("eventDate<#" + start.ToString("yyyy-MM-dd") + "# and  recurred=1");
            DataRow[] drSqEnd = dtEvent.Select("eventDate<#" + end.ToString("yyyy-MM-dd") + "# and eventDate>#" + start.ToString("yyyy-MM-dd") + "# and   recurred=1");

            if (drSqStart.Length > 0)
            {
                string strEventdate = string.Empty;
                string[] arr;
                int incr = start.Month;
                DateTime eventEndDate;
                foreach (DataRow dr in drSqStart)
                {
                    eventEndDate = Convert.ToDateTime(dr["recurredEndDate"]);

                    DataRow drNew = dtEvent.NewRow();
                    drNew.ItemArray = dr.ItemArray.Clone() as object[];

                    eventDate = Convert.ToDateTime(dr["eventDate"]);
                    if (dr["recurredType"].ToString() == "1")
                    {
                        if (eventDate < start)
                        {
                            eventDate = start;
                            do
                            {
                                eventDate = eventDate.AddDays(1);
                                if (eventDate > eventEndDate && eventEndDate.ToString("dd-MM-yyyy") != "01-01-0001")
                                {
                                    break;
                                }

                                if (Convert.ToDateTime(drNew["eventDate"]) != eventDate)
                                {
                                    drNew["eventDate"] = eventDate;
                                    dtEvent.Rows.Add(drNew);
                                }
                                drNew = dtEvent.NewRow();
                                drNew.ItemArray = dtEvent.Rows[dtEvent.Rows.Count - 1].ItemArray.Clone() as object[];
                            } while (eventDate < end);
                        }
                        else
                        {
                            eventDate = eventDate.AddDays(1);
                            dr["eventDate"] = eventDate;
                        }
                    }
                    else if (dr["recurredType"].ToString() == "2")
                    {
                        if (eventDate < start)
                        {
                            //eventDate = start;
                            do
                            {
                                eventDate = eventDate.AddDays(7);
                                if (eventDate > eventEndDate && eventEndDate.ToString("dd-MM-yyyy") != "01-01-0001")
                                {
                                    break;
                                }
                                if (eventDate > start)
                                {
                                    if (Convert.ToDateTime(drNew["eventDate"]) != eventDate)
                                    {
                                        drNew["eventDate"] = eventDate;
                                        dtEvent.Rows.Add(drNew);
                                    }
                                    drNew = dtEvent.NewRow();
                                    drNew.ItemArray = dtEvent.Rows[dtEvent.Rows.Count - 1].ItemArray.Clone() as object[];
                                }
                            } while (eventDate < end);
                        }
                        else
                        {
                            eventDate = eventDate.AddDays(7);
                            dr["eventDate"] = eventDate;
                        }
                    }
                    else
                    {
                        incr = start.Month;
                        do
                        {
                            strEventdate = eventDate.ToString("dd-MM-yyyy HH:mm");
                            arr = strEventdate.Split('-');
                            arr[1] = incr.ToString("00");
                            arr[2] = arr[2].Replace(eventDate.Year.ToString(), start.Year.ToString());
                            strEventdate = string.Empty;
                            foreach (string str in arr)
                            {
                                strEventdate = strEventdate + str + "-";
                            }
                            strEventdate = strEventdate.Remove(strEventdate.Length - 1);
                            eventDate = Convert.ToDateTime(Common.ParseDate(strEventdate, "dd-MM-yyyy HH:mm"));
                            if (eventDate > eventEndDate && eventEndDate.ToString("dd-MM-yyyy") != "01-01-0001")
                            {
                                break;
                            }
                            incr = incr + 1;
                        } while (eventDate < start);
                        dr["eventDate"] = eventDate;
                    }
                }
            }

            if (drSqEnd.Length > 0)
            {
                DateTime eventEndDate;
                string strEventdate = string.Empty;
                string[] arr;
                int incr = start.Month;
                foreach (DataRow dr in drSqEnd)
                {
                    eventEndDate = Convert.ToDateTime(dr["recurredEndDate"]);

                    DataRow drNew = dtEvent.NewRow();
                    drNew.ItemArray = dr.ItemArray.Clone() as object[];
                    eventDate = Convert.ToDateTime(dr["eventDate"]);

                    if (dr["recurredType"].ToString() == "1")
                    {
                        do
                        {
                            eventDate = eventDate.AddDays(1);
                            if (eventDate > eventEndDate && eventEndDate.ToString("dd-MM-yyyy") != "01-01-0001")
                            {
                                break;
                            }
                            if (Convert.ToDateTime(drNew["eventDate"]) != eventDate)
                            {
                                drNew["eventDate"] = eventDate;
                                dtEvent.Rows.Add(drNew);
                            }
                            drNew = dtEvent.NewRow();
                            drNew.ItemArray = dtEvent.Rows[dtEvent.Rows.Count - 1].ItemArray.Clone() as object[];
                        } while (eventDate < end);
                    }
                    else if (dr["recurredType"].ToString() == "2")
                    {
                        do
                        {
                            eventDate = eventDate.AddDays(7);

                            if (eventDate > eventEndDate && eventEndDate.ToString("dd-MM-yyyy") != "01-01-0001")
                            {
                                break;
                            }
                            if (Convert.ToDateTime(drNew["eventDate"]) != eventDate)
                            {
                                drNew["eventDate"] = eventDate;
                                dtEvent.Rows.Add(drNew);
                            }
                            drNew = dtEvent.NewRow();
                            drNew.ItemArray = dtEvent.Rows[dtEvent.Rows.Count - 1].ItemArray.Clone() as object[];
                        } while (eventDate < end);
                    }
                    else
                    {
                        incr = end.Month;
                        do
                        {
                            strEventdate = eventDate.ToString("dd-MM-yyyy HH:mm");
                            arr = strEventdate.Split('-');
                            arr[1] = incr.ToString("00");
                            arr[2] = arr[2].Replace(eventDate.Year.ToString(), end.Year.ToString());
                            strEventdate = string.Empty;
                            foreach (string str in arr)
                            {
                                strEventdate = strEventdate + str + "-";
                            }
                            strEventdate = strEventdate.Remove(strEventdate.Length - 1);
                            eventDate = Convert.ToDateTime(Common.ParseDate(strEventdate, "dd-MM-yyyy HH:mm"));
                            if (eventDate > eventEndDate && eventEndDate.ToString("dd-MM-yyyy") != "01-01-0001")
                            {
                                break;
                            }
                            incr = incr - 1;
                        } while (eventDate > end);
                        if (Convert.ToDateTime(drNew["eventDate"]) != eventDate)
                        {
                            drNew["eventDate"] = eventDate;
                            dtEvent.Rows.Add(drNew);
                        }
                    }
                }
            }

            foreach (DataRow dr in dtEvent.Rows)
            {
                EventInfo cevent = new EventInfo();
                cevent.EventId = (int)dr["eventid"];
                cevent.EventName = dr["name"].ToString();
                cevent.Description = dr["description"].ToString();
                eventDate = (DateTime)dr["eventdate"];
                cevent.EventDate = eventDate;
                cevent.StartActual = (DateTime)dr["startActual"];
                cevent.EndDate = (DateTime)(string.IsNullOrEmpty(dr["enddate"].ToString()) ? dr["eventdate"] : dr["enddate"]);
                cevent.Category = Convert.ToInt32(string.IsNullOrEmpty(dr["category"].ToString()) ? 0 : dr["category"]);
                cevent.AllDay = Convert.ToBoolean(string.IsNullOrEmpty(dr["allDay"].ToString()) ? 0 : dr["allDay"]);
                cevent.Private = Convert.ToBoolean(string.IsNullOrEmpty(dr["Private"].ToString()) ? 0 : dr["Private"]);
                cevent.Recurred = Convert.ToBoolean(string.IsNullOrEmpty(dr["recurred"].ToString()) ? 0 : dr["recurred"]);
                cevent.TimezoneId = dr["timezoneid"].ToString();
                cevent.RecurredType = Convert.ToInt32(dr["recurredType"].ToString());
                cevent.RecurredEndDate = (DateTime)dr["recurredEndDate"];
                events.Add(cevent);
            }

            //while (reader.Read())
            //{
            //    EventInfo cevent = new EventInfo();
            //    cevent.EventId = (int)reader["eventid"];
            //    cevent.EventName = DAO.getString(reader, "name");
            //    cevent.Description = (string)reader["description"];
            //    eventDate = (DateTime)reader["eventdate"];

            //    if (eventDate.Month == startMonth)
            //    {
            //        cevent.EventDate = eventDate;
            //    }
            //    else
            //    {
            //        int incr = 1;
            //        do
            //        {
            //            eventDate = eventDate.AddMonths(incr);
            //            incr = incr + 1;
            //        } while (eventDate.Month + incr >= startMonth && eventDate.Month <= endMonth + incr);
            //        cevent.EventDate = eventDate;
            //    }

            //    cevent.EndDate = (DateTime)(string.IsNullOrEmpty(reader["enddate"].ToString()) ? reader["eventdate"] : reader["enddate"]);
            //    cevent.Category = Convert.ToInt32(DAO.getInt(reader, "category"));
            //    cevent.AllDay = Convert.ToBoolean(DAO.getBool(reader, "allDay"));
            //    cevent.Private = Convert.ToBoolean(DAO.getBool(reader, "Private"));
            //    cevent.Recurred = Convert.ToBoolean(DAO.getBool(reader, "recurred"));
            //    events.Add(cevent);
            //}
            reader.Close();
            reader.Dispose();

            return events;
            //side note: if you want to show events only related to particular users,
            //if user id of that user is stored in session as Session["userid"]
            //the event table also contains a extra field named 'user_id' to mark the event for that particular user
            //then you can modify the SQL as:
            //SELECT event_id, description, title, event_start, event_end FROM event where user_id=@user_id AND event_start>=@start AND event_end<=@end
            //then add paramter as:cmd.Parameters.AddWithValue("@user_id", HttpContext.Current.Session["userid"]);
        }
    }
}