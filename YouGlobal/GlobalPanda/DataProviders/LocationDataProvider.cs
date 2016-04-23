using GlobalPanda;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

/// <summary>
/// Summary description for LocationDataProvider
/// </summary>
public class LocationDataProvider
{
    public static void insertLocationGroup(int locationId, List<int> countryList)
    {
        string sql = "insert into location_countries (locationid,countryid) values(?locationid,?countryid);";
        deleteLocationGroup(locationId);
        foreach (int countryid in countryList)
        {
            DAO.ExecuteNonQuery(sql, new MySqlParameter("locationid", locationId), new MySqlParameter("countryid", countryid));
        }
    }

    public static void deleteLocationGroup(int locationId)
    {
        string sql = "delete from location_countries where locationid=?locationid";
        DAO.ExecuteNonQuery(sql, new MySqlParameter("locationid", locationId));
    }

    public static MySqlDataReader getLocationGroupCountry(int locationid)
    {
        string sql = "select c.countryid,c.name as country from countries c inner join location_countries lc on c.countryid=lc.countryid where lc.locationid=?locationid";
        MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("locationid", locationid));

        return reader;
    }

    public static void insertSublocation(int locationId, string subLocation)
    {
        string sql = "insert into locationsub (sublocation,locationid) values (?subLocation,?locationId)";
        DAO.ExecuteNonQuery(sql, new MySqlParameter("locationId", locationId), new MySqlParameter("subLocation", subLocation));
    }

    public static bool isDuplicateSubLocation(string name, int locationid, int id)
    {
        string sql = "select count(locationid) as `exists` from locationsub where sublocation = ?name and locationid = ?locationid and sublocationid!=?id";
        uint exists = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("name", name), new MySqlParameter("locationid", locationid), new MySqlParameter("id", id)));
        return (exists > 0);
    }

    public static MySqlDataReader listAllSublocation()
    {
        string sql = "select sublocationid,sublocation,l.locationid,name as location from locationsub ls inner join locations l on ls.locationid=l.locationid";
        MySqlDataReader reader = DAO.ExecuteReader(sql);

        return reader;
    }

    public static MySqlDataReader getSubLocationById(int id)
    {
        string sql = "select sublocationid,sublocation,l.locationid,name as location from locationsub ls inner join locations l on ls.locationid=l.locationid where sublocationid=?id";
        MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("id", id));

        return reader;
    }

    public static int insertCountry(string name, string countryCode, string phoneCode)
    {
        int id = 0;
        string sql = "insert into countries (name,code,active,phonecode) values (?name,?coutrycode,?active,?phonecode); select last_insert_id() ";
        id = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("name", name), new MySqlParameter("active", true), new MySqlParameter("coutrycode", countryCode), new MySqlParameter("phonecode", phoneCode)));
        return id;
    }

    public static void updateCountry(int countryId, string name, string countryCode, string phoneCode)
    {
        string sql = "update countries set name=?name,code=?countrycode,phonecode=?phonecode where countryid=?countryid";
        DAO.ExecuteNonQuery(sql, new MySqlParameter("name", name), new MySqlParameter("countrycode", countryCode), new MySqlParameter("phonecode", phoneCode), new MySqlParameter("countryid", countryId));
    }

    public static void deleteCountry(int countryId)
    {
        string sql = "delete from countries where countryid=?countryid";
        DAO.ExecuteNonQuery(sql, new MySqlParameter("countryid", countryId));
    }

    public static int insertLocation(int countryId, string name, string locationCode, string level)
    {
        int id = 0;
        string sql = "insert into locations(name,countryid,code,level)  values (?name,?countryid,?code,?level);select last_insert_id() ";
        id = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("name", name), new MySqlParameter("countryid", countryId), new MySqlParameter("code", locationCode), new MySqlParameter("level", level)));
        return id;
    }

    public static void updateLocation(int locationId, int countryId, string name, string locationCode, string level)
    {
        string sql = "update locations set name=?name, countryid=?countryid,code=?code,level=?level where locationid=?locationid";
        DAO.ExecuteNonQuery(sql, new MySqlParameter("name", name), new MySqlParameter("countryid", countryId), new MySqlParameter("code", locationCode), new MySqlParameter("locationid", locationId), new MySqlParameter("level", level));
    }

    public static void deleteLocation(int locationId)
    {
        string sql = "delete from locationsub_Subs where sublocationid in(select sublocationid from locationsub s inner join locations l on s.locationid=l.locationid where l.locationid=?locationid);" +
                     " delete from locationsub where locationid in(select locationid from locations where locationid=?locationid); " +
                     " delete from locations where locationid=?locationid";
        DAO.ExecuteNonQuery(sql, new MySqlParameter("locationid", locationId));
    }

    public static int insertSubLocation(int locationId, string name)
    {
        int id = 0;
        string sql = "insert into locationsub(sublocation,locationid) values (?name,?locationid);select last_insert_id() ";
        id = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("name", name), new MySqlParameter("locationid", locationId)));
        return id;
    }

    public static void updateSublocation(int id, int locationid, string subLocation)
    {
        string sql = "update locationsub set sublocation=?sublocation,locationid=?locationid where sublocationid=?sublocationid";
        DAO.ExecuteNonQuery(sql, new MySqlParameter("sublocationid", id), new MySqlParameter("locationid", locationid), new MySqlParameter("sublocation", subLocation));
    }

    public static void deleteSublocation(int id)
    {
        string sql = "delete from locationsub_Subs where sublocationid in(select sublocationid from locationsub where sublocationid=?sublocationid); " +
                     " delete from locationsub where sublocationid=?sublocationid";
        DAO.ExecuteNonQuery(sql, new MySqlParameter("sublocationid", id));
    }

    //public static void updateSublocation(int sublocationId, int locationId, string name)
    //{
    //    string sql = "update locationsub set locationid=?locationid,sublocation=?name where sublocationid=?sublocationid";
    //    DAO.ExecuteNonQuery(sql, new MySqlParameter("name", name), new MySqlParameter("locationid", locationId), new MySqlParameter("sublocationid", sublocationId));
    //}

    //public static void deleteSublocation(int sublocationid)
    //{
    //    string sql = "delete from locationsub where sublocationid=?sublocationid";
    //    DAO.ExecuteNonQuery(sql, new MySqlParameter("sublocationid", sublocationid));
    //}

    public static int insertSubsublocation(int sublocationid, string name)
    {
        int id = 0;
        string sql = "insert into locationsub_subs(name,sublocationid) values (?name,?sublocationid);select last_insert_id() ";
        id = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("name", name), new MySqlParameter("sublocationid", sublocationid)));
        return id;
    }

    public static void updateSubsublocation(int subsubloationId, int sublocationid, string name)
    {
        string sql = "update locationsub_Subs set sublocationid=?sublocationid,name=?name where subsublocationid=?subsublocationid";
        DAO.ExecuteNonQuery(sql, new MySqlParameter("sublocationid", sublocationid), new MySqlParameter("name", name), new MySqlParameter("subsublocationid", subsubloationId));
    }

    public static void deleteSubsublocation(int subsublocationid)
    {
        string sql = "delete from locationsub_subs where subsublocationid=?subsublocationid";
        DAO.ExecuteNonQuery(sql, new MySqlParameter("subsublocationid", subsublocationid));
    }

    public static bool isDuplicateSubLocation(string name, int sublocationid)
    {
        string sql = "select count(locationid) as `exists` from locationsub where sublocation = ?name and sublocationid != ?sublocationid";
        uint exists = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("name", name), new MySqlParameter("sublocationid", sublocationid)));
        return (exists > 0);
    }

    public static bool isDuplicateSubsubLocation(string name, int subsublocationid)
    {
        string sql = "select count(sublocationid) as `exists` from locationsub_subs where name = ?name and subsublocationid != ?subsublocationid";
        uint exists = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("name", name), new MySqlParameter("subsublocationid", subsublocationid)));
        return (exists > 0);
    }

    public static void deleteAll(int countryid)
    {
        string sql = "delete from locationsub_Subs where sublocationid in(select sublocationid from locationsub s inner join locations l on s.locationid=l.locationid inner join countries c on l.countryid=c.countryid where c.countryid=?countryid);" +
                     " delete from locationsub where locationid in ( select locationid from locations l inner join countries c on l.countryid=c.countryid where c.countryid=?countryid);  " +
                     " delete from locations where countryid in ( select countryid from countries where countryid=?countryid); " +
                     " delete from countries where countryid=?countryid";
        DAO.ExecuteNonQuery(sql, new MySqlParameter("countryid", countryid));
    }

    public static MySqlDataReader getLocations(string keyword)
    {
        string sql = "select * from(" +
                      " select c.countryid,c.name,0 as locationid, '' as locationname,0 as sublocationid,'' as sublocation,0 as subsublocationid,''  as subsublocation from countries c " +
                      " where c.name like concat('%',?keyword,'%') union " +
                      " select c.countryid,c.name,l.locationid, l.name as locationname ,0 as sublocationid,'' as sublocation,0 as subsublocationid,''  as subsublocation from countries c left join locations l on l.countryid=c.countryid " +
                      " where   l.name like concat('%',?keyword,'%') union " +
                      " select c.countryid,c.name,l.locationid, l.name as locationname,s.sublocationid,s.sublocation,0 as subsublocationid,''  as subsublocation from countries c left join locations l on l.countryid=c.countryid " +
                      " left join locationsub s on s.locationid=l.locationid where  sublocation like concat('%',?keyword,'%') union " +
                      " select c.countryid,c.name,l.locationid, l.name as locationname,s.sublocationid,s.sublocation,ss.subsublocationid,ss.name as subsublocation from countries c left join locations l on l.countryid=c.countryid " +
                      " left join locationsub s on s.locationid=l.locationid left join locationsub_subs ss on ss.sublocationid=s.sublocationid where  ss.name like concat('%',?keyword,'%') " +
                      " ) as tbl order by 2,4,6,8 asc";
        MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword));
        return dr;
    }

    public static MySqlDataReader getLocationswithGroup(string keyword)
    {
        string sql = "select * from(" +
                      " select c.countryid,c.name,0 as locationid, '' as locationname,0 as sublocationid,'' as sublocation,0 as subsublocationid,''  as subsublocation from countries c " +
                      " where c.name like concat('%',?keyword,'%') union " +
                      " select c.countryid,c.name,l.locationid, l.name as locationname ,0 as sublocationid,'' as sublocation,0 as subsublocationid,''  as subsublocation from countries c left join locations l on l.countryid=c.countryid " +
                      " where  c.name like concat('%',?keyword,'%') or l.name like concat('%',?keyword,'%') union " +
                      " select c.countryid,c.name,l.locationid, l.name as locationname,s.sublocationid,s.sublocation,0 as subsublocationid,''  as subsublocation from countries c left join locations l on l.countryid=c.countryid " +
                      " left join locationsub s on s.locationid=l.locationid where c.name like concat('%',?keyword,'%') or l.name like concat('%',?keyword,'%') or sublocation like concat('%',?keyword,'%') union " +
                      " select c.countryid,c.name,l.locationid, l.name as locationname,s.sublocationid,s.sublocation,ss.subsublocationid,ss.name as subsublocation from countries c left join locations l on l.countryid=c.countryid " +
                      " left join locationsub s on s.locationid=l.locationid left join locationsub_subs ss on ss.sublocationid=s.sublocationid where c.name like concat('%',?keyword,'%') or l.name like concat('%',?keyword,'%') or sublocation like concat('%',?keyword,'%') or ss.name like concat('%',?keyword,'%') " +
                      " union select  vl.countryid,vl.name,vl.locationid, vl.locationname as locationname,vl.sublocationid,vl.sublocation,vl.subsublocationid,vl.name as subsublocation " +
                      " from location_group g inner join location_groupdetails gd on g.location_groupid=gd.location_groupid  left join v_locations vl on (vl.countryid=gd.locationid and gd.locationtype=1) " +
                      " or (vl.locationid=gd.locationid and gd.locationtype=2) or (vl.sublocationid=gd.locationid and gd.locationtype=3) or (vl.subsublocationid=gd.locationid and gd.locationtype=4) " +
                      " where groupname like concat('%',?keyword,'%') group by gd.locationid,gd.locationtype " +
                      " ) as tbl order by name,locationname,sublocation,subsublocation asc";
        MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword));
        return dr;
    }

    public static MySqlDataReader getLocationswithGroupName(string keyword)
    {
        string sql = "select * from(" +
                      " select c.countryid,c.name,0 as locationid, '' as locationname,0 as sublocationid,'' as sublocation,0 as subsublocationid,''  as subsublocation,'0' as location_groupid ,' ' as groupname from countries c " +
                      " where c.name like concat('%',?keyword,'%') union " +
                      " select c.countryid,c.name,l.locationid, l.name as locationname ,0 as sublocationid,'' as sublocation,0 as subsublocationid,''  as subsublocation,'0' as location_groupid ,' ' as groupname from countries c left join locations l on l.countryid=c.countryid " +
                      " where  c.name like concat('%',?keyword,'%') or l.name like concat('%',?keyword,'%') union " +
                      " select c.countryid,c.name,l.locationid, l.name as locationname,s.sublocationid,s.sublocation,0 as subsublocationid,''  as subsublocation,'0' as location_groupid ,' ' as groupname from countries c left join locations l on l.countryid=c.countryid " +
                      " left join locationsub s on s.locationid=l.locationid where c.name like concat('%',?keyword,'%') or l.name like concat('%',?keyword,'%') or sublocation like concat('%',?keyword,'%') union " +
                      " select c.countryid,c.name,l.locationid, l.name as locationname,s.sublocationid,s.sublocation,ss.subsublocationid,ss.name as subsublocation,'0' as location_groupid ,' ' as groupname from countries c left join locations l on l.countryid=c.countryid " +
                      " left join locationsub s on s.locationid=l.locationid left join locationsub_subs ss on ss.sublocationid=s.sublocationid where c.name like concat('%',?keyword,'%') or l.name like concat('%',?keyword,'%') or sublocation like concat('%',?keyword,'%') or ss.name like concat('%',?keyword,'%') " +
                      " union select  '0' as countryid,' ' as name,'0' as locationid, ' ' as locationname,'0' as sublocationid,' ' sublocation,'0' as subsublocationid,' ' as subsublocation,g.location_groupid,groupname " +
                      " from location_group g " +
                      //" inner join location_groupdetails gd on g.location_groupid=gd.location_groupid  left join v_locations vl on (vl.countryid=gd.locationid and gd.locationtype=1) " +
                      //" or (vl.locationid=gd.locationid and gd.locationtype=2) or (vl.sublocationid=gd.locationid and gd.locationtype=3) or (vl.subsublocationid=gd.locationid and gd.locationtype=4) " +
                      " where groupname like concat('%',?keyword,'%') " +
                      " ) as tbl order by name,locationname,sublocation,subsublocation asc";
        MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword));
        return dr;
    }

    public static MySqlDataReader getPartialLocations(string keyword)
    {
        string sql = "select * from(" +
                      " select c.countryid,c.name,0 as locationid, '' as locationname,0 as sublocationid,'' as sublocation,0 as subsublocationid,''  as subsublocation from countries c " +
                      " where c.name like concat('%',?keyword,'%') union " +
                      " select c.countryid,c.name,l.locationid, l.name as locationname ,0 as sublocationid,'' as sublocation,0 as subsublocationid,''  as subsublocation from countries c join locations l on l.countryid=c.countryid " +
                      " where   l.name like concat('%',?keyword,'%') or c.name like concat('%',?keyword,'%') ) as tbl order by 2,4,6,8 asc";
        MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword));
        return dr;
    }

    public static void insertLocationgroup(string groupname, DataTable dtGropus)
    {
        string sql = "insert into location_group (groupname,userid,created) values(?groupname,?userid,?created); select last_insert_id();";
        int id = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("groupname", groupname), new MySqlParameter("userid", GPSession.UserId), new MySqlParameter("created", DateTime.UtcNow)));

        insertGroupDetails(id, dtGropus);
    }

    private static void insertGroupDetails(int groupId, DataTable dtGroups)
    {
        string sql = string.Empty;
        foreach (DataRow dr in dtGroups.Rows)
        {
            sql = "insert into location_groupdetails (location_groupid,locationid,locationtype) values (?groupid,?locationId,?locationtype) ";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("groupid", groupId), new MySqlParameter("locationId", dr["locationId"]), new MySqlParameter("locationtype", dr["locationtype"]));
        }
    }

    public static MySqlDataReader getLocationGroups()
    {
        string sql = "select location_groupid,groupname from location_group ";
        MySqlDataReader dr = DAO.ExecuteReader(sql);
        return dr;
    }

    public static void deleteLocationgroup(int groupId)
    {
        string sql = "delete from  location_groupdetails where location_groupid=?groupid; delete from location_group where location_groupid=?groupid";
        DAO.ExecuteNonQuery(sql, new MySqlParameter("groupid", groupId));
    }

    public static DataSet getLocationGroupById(int groupId)
    {
        string sql = "select location_groupid,groupname from location_group where location_groupid=?groupid; " +
                     " select location_groupid,locationid,locationtype, (select concat(c.name,':',CAST(c.countryid as char),',','0,0,0') from countries c where c.countryid =g.locationid and g.locationtype=1 union " +
                     " select concat(concat(c.name,' > ',l.name),':',CAST(c.countryid as char),',',cast(l.locationid as char),',', '0,0') from countries c join locations l on l.countryid=c.countryid where l.locationid=g.locationid and g.locationtype=2 union " +
                     " select concat(concat(c.name,' > ',l.name,' > ',s.sublocation),':',CAST(c.countryid as char),',',cast(l.locationid as char),',', cast(s.sublocationid as char),',','0') from countries c join locations l on l.countryid=c.countryid join locationsub s on s.locationid=l.locationid " +
                     " where  s.sublocationid=g.locationid and g.locationtype=3 union " +
                     " select concat(concat(c.name,' > ',l.name,' > ',s.sublocation,' > ',ss.name),':',CAST(c.countryid as char),',',cast(l.locationid as char),',', cast(s.sublocationid as char),',',cast(ss.subsublocationid as char) ) from countries c join locations l on l.countryid=c.countryid  " +
                     " join locationsub s on s.locationid=l.locationid join locationsub_subs ss on ss.sublocationid=s.sublocationid where  ss.subsublocationid=g.locationid and g.locationtype=4 ) as parentids " +
                     " from location_groupdetails g where location_groupid=?groupid order by parentids asc; ";
        MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("groupid", groupId));
        DataSet ds = new DataSet();
        ds.EnforceConstraints = false;
        ds.Load(dr, LoadOption.PreserveChanges, new string[2]);
        dr.Close();
        dr.Dispose();
        return ds;
    }

    public static MySqlDataReader getLocationByIdAndType(int Id, int type)
    {
        string sql = string.Empty;
        switch (type)
        {
            case 1:
                sql = " select c.name as location from countries c where c.countryid =?id ";
                break;

            case 2:
                sql = " select concat(c.name,' > ',l.name) as location from countries c inner join locations l on l.countryid=c.countryid where   l.locationid=?id  ";
                break;

            case 3:
                sql = " select concat(c.name,' > ',l.name,' > ',s.sublocation) as location from countries c inner join locations l on l.countryid=c.countryid inner join locationsub s on s.locationid=l.locationid  where  s.sublocationid=?id ";
                break;

            case 4:
                sql = " select concat(c.name,' > ',l.name,' > ',s.sublocation,' > ',ss.name) as location from countries c left join locations l on l.countryid=c.countryid  join locationsub s on s.locationid=l.locationid " +
                 " join locationsub_subs ss on ss.sublocationid=s.sublocationid where  ss.subsublocationid=?id ";
                break;
        }
        MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("id", Id));

        return dr;
    }

    public static void updateLocationgroup(int groupId, string groupname, DataTable dtGroups)
    {
        string sql = "update location_group set groupname=?groupname,modified=?modified where location_groupid=?groupid";
        DAO.ExecuteNonQuery(sql, new MySqlParameter("groupname", groupname), new MySqlParameter("modified", DateTime.UtcNow), new MySqlParameter("groupid", groupId));
        sql = "delete from  location_groupdetails where location_groupid=?groupid";
        DAO.ExecuteNonQuery(sql, new MySqlParameter("groupid", groupId));
        insertGroupDetails(groupId, dtGroups);
    }

    public static MySqlDataReader getCountry(string keyword)
    {
        string sql = "select countryid,name,code from countries where name like concat('%',?keyword,'%')";
        MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword));
        return dr;
    }

    public static string getCountry(int countryid)
    {
        string country = string.Empty;
        string sql = "select name from countries where countryid = ?countryid";
        MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("countryid", countryid));
        dr.Read();
        country = DAO.getString(dr, "name");
        return country;
    }

    public static MySqlDataReader getLocation(string keyword)
    {
        string sql = "select locationid,name from locations where name like concat('%',?keyword,'%')";
        MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword));
        return dr;
    }
}