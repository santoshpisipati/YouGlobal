using MySql.Data.MySqlClient;
using System;

/// <summary>
/// Summary description for JobLocationDataProvider
/// </summary>
namespace GlobalPanda.DataProviders
{
    public class JobLocationDataProvider
    {
        public static uint insertJobLocation(uint jobId, int locationId, int locationtype)
        {
            string sql = "insert into jobs_locations (jobdetailid, locationid,locationtype) values (?jobId,?locationid,?locationtype); select last_insert_id()";
            uint id = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("jobId", jobId), new MySqlParameter("locationid", locationId), new MySqlParameter("locationtype", locationtype)));
            return id;
        }

        public static void deleteJobLocation(int jobId)
        {
            string sql = "delete from jobs_locations where jobdetailid = ?jobId ";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("jobId", jobId));
        }

        public static MySqlDataReader searchJobLocation(int jobId)
        {
            string sql = "Select jl.jobdetailid,jl.locationtype,jl.locationid,(select concat('Anywhere',',','0',',','0,0,0') from jobs_locations  where jl.locationid=0 and jl.locationtype=1 " +
        " union select concat(c.name,',',CAST(c.countryid as char),',','0,0,0') from countries c where c.countryid=jl.locationid and jl.locationtype=1 and jl.locationid!=0 " +
        " union select concat(concat(c.name,'>',l.name),',',CAST(c.countryid as char),',',cast(l.locationid as char),',', '0,0')  from countries c inner join locations l on l.countryid=c.countryid where l.locationid=jl.locationid and jl.locationtype=2 " +
        " union select concat(concat(c.name,'>',l.name,'>',s.sublocation),',',CAST(c.countryid as char),',',cast(l.locationid as char),',', cast(s.sublocationid as char),',','0')  from countries c inner join locations l on l.countryid=c.countryid " +
                          " inner join locationsub s on s.locationid=l.locationid where s.sublocationid=jl.locationid and jl.locationtype=3 " +
        " union select concat(concat(c.name,'>',l.name,'>',s.sublocation,'>',ss.name),',',CAST(c.countryid as char),',',cast(l.locationid as char),',', cast(s.sublocationid as char),',',cast(ss.subsublocationid as char) ) from countries c inner join locations l on l.countryid=c.countryid " +
       " inner join locationsub s on s.locationid=l.locationid inner join locationsub_subs ss on ss.sublocationid=s.sublocationid where ss.subsublocationid=jl.locationid and jl.locationtype=4 " +
          " union select  concat(groupname,',','0,0,0,0,',g.location_groupid) groupname from location_group g where g.location_groupid=jl.locationid and jl.locationtype=5   " +
       " ) as location from jobs_locations jl where jl.jobdetailid=?jobId  ";
            return DAO.ExecuteReader(sql, new MySqlParameter("jobId", jobId));
        }

        public static void copyLocation(int jobId, int newjobId)
        {
            string sql = "insert into jobs_locations (jobdetailid, locationid,locationtype)" +
            "select ?newjobId, locationid,locationtype from jobs_locations where jobdetailid=?jobId";

            DAO.ExecuteScalar(sql, new MySqlParameter("jobId", jobId), new MySqlParameter("newjobId", newjobId));
        }

        public static MySqlDataReader getJobLocation(int jobId)
        {
            //      string sql = "Select jl.jobdetailid,jl.locationtype,jl.locationid,(select concat('Anywhere') from jobs_locations  where jl.locationid=0 and jl.locationtype=1 " +
            //  " union select concat(c.name) from countries c where c.countryid=jl.locationid and jl.locationtype=1 " +
            // " union select concat(c.name,'> <br/>',l.name)  from countries c inner join locations l on l.countryid=c.countryid where l.locationid=jl.locationid and jl.locationtype=2 " +
            // " union select concat(c.name,'> <br/>',l.name,'> <br/>',s.sublocation)  from countries c inner join locations l on l.countryid=c.countryid " +
            //                   " inner join locationsub s on s.locationid=l.locationid where s.sublocationid=jl.locationid and jl.locationtype=3 " +
            // " union select concat(c.name,'> <br/>',l.name,'> <br/>',s.sublocation,'> <br/>',ss.name) from countries c inner join locations l on l.countryid=c.countryid " +
            //" inner join locationsub s on s.locationid=l.locationid inner join locationsub_subs ss on ss.sublocationid=s.sublocationid where ss.subsublocationid=jl.locationid and jl.locationtype=4 " +
            //                   " ) as location from jobs_locations jl where jl.jobdetailid=?jobId ";

            string sql = "select concat('Anywhere') as location from jobs_locations jl  where jl.locationid=0 and jl.locationtype=1 and jl.jobdetailid=?jobId" +
        " union select concat(c.name) as location from jobs_locations jl join countries c on c.countryid=jl.locationid where  jl.locationtype=1 and jl.locationid!=0 and jl.jobdetailid=?jobId" +
       " union select concat(c.name,' > ',l.name) as location from countries c inner join locations l on l.countryid=c.countryid join jobs_locations jl on l.locationid=jl.locationid " +
       " where  jl.locationtype=2 and jl.jobdetailid=?jobId" +
       " union select concat(c.name,' > ',l.name,' > ',s.sublocation) as location from countries c inner join locations l on l.countryid=c.countryid " +
       " inner join locationsub s on s.locationid=l.locationid join jobs_locations jl on jl.locationid=s.sublocationid where  jl.locationtype=3 and jl.jobdetailid=?jobId" +
       " union select concat(c.name,' > ',l.name,' >',s.sublocation,' > ',ss.name) as location from countries c inner join locations l on l.countryid=c.countryid " +
      " inner join locationsub s on s.locationid=l.locationid inner join locationsub_subs ss on ss.sublocationid=s.sublocationid join jobs_locations jl on ss.subsublocationid=jl.locationid where jl.locationtype=4 and jl.jobdetailid=?jobId" +
       " union select groupname as location from jobs_locations jl join location_group g on g.location_groupid=jl.locationid where  jl.locationtype=5  and jl.jobdetailid=?jobId  ";
            return DAO.ExecuteReader(sql, new MySqlParameter("jobId", jobId));
        }

        public static MySqlDataReader getJobCountry(int jobId, string keyword)
        {
            string sql = "select c.countryid,c.name,c.code from countries c join jobs_locations jl on jl.locationid=c.countryid and jl.locationtype=1 where name like concat('%',?keyword,'%') and jl.jobdetailid=?jobid";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("jobid", jobId), new MySqlParameter("keyword", keyword));
            return dr;
        }

        public static MySqlDataReader getJobLocation(int jobId, string keyword)
        {
            string sql = "select l.locationid,l.name from locations l join jobs_locations jl on jl.locationid=l.locationid and jl.locationtype=2 where name like concat('%',?keyword,'%') and jl.jobdetailid=?jobid";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("jobid", jobId), new MySqlParameter("keyword", keyword));
            return dr;
        }
    }
}