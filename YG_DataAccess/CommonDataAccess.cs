using MySql.Data.MySqlClient;
using System;

namespace YG_DataAccess
{
    public class CommonDataAccess
    {
        public static MySqlDataReader searchJobIndustry(string keyword)
        {
            string sql = "select distinct Js.JobIndustrySubId,concat(Ji.Classification,' - ',Js.SubClassification) as classification from jobindustry Ji " +
                         "inner join jobindustrysub Js on Ji.jobindustryid = Js.jobindustryid where Ji.Classification like concat_ws(?keyword,'%','%') " +
                         "or Js.SubClassification like concat_ws(?keyword,'%','%') order by classification";

            return DataAccess.ExecuteReader(sql, new MySqlParameter("keyword", keyword));
        }

        public static MySqlDataReader searchCountryCode(string keyword)
        {
            string sql = "select SQL_NO_CACHE concat(name,' (', phonecode,')') as Code from countries where phonecode is not null and (phonecode like concat(?keyword,'%') or " +
                         "name like concat(?keyword,'%') ) order by name asc;";
            return DataAccess.ExecuteReader(sql, new MySqlParameter("keyword", keyword));
        }

        public static MySqlDataReader searchOccupation(string keyword)
        {
            string sql = "select SQL_NO_CACHE * from( Select  t1.isco08id as id1,t2.isco08id as id2,t3.isco08id as id3,t4.isco08id as id4,t1.groupcode as c1 ,t1.title as d1,t2.groupcode as c2 ,t2.title as d2 ,t3.groupcode as c3,t3.title as d3 ,t4.groupcode as c4 ," +
                       " t4.title as d4,t1.keyword as k1,t2.keyword as k2,t3.keyword as k3, t4.keyword as k4,JobDetailsCount(t1.isco08id) as t1Count,JobDetailsCount(t2.isco08id) as t2Count,JobDetailsCount(t3.isco08id) as t3Count,(JobDetailsCount(t1.isco08id) + JobDetailsCount(t2.isco08id) + JobDetailsCount(t3.isco08id)" +
                        " + JobDetailsCount(t4.isco08id)) as Total " +
                       " , JobDetailsCount(t4.isco08id) as t4Count from isco08 t1  join isco08 t2 on t1.groupcode=t2.parentcode and t1.type=1  join isco08 t3 on t2.groupcode=t3.parentcode and t2.type=2 " +
                       " join isco08 t4 on t3.groupcode=t4.parentcode and t3.type =3  where t4.type=4  and JobDetailsCount(t4.isco08id)>0) a where ( d1 like concat('%',?keyword,'%') or d2 like concat('%',?keyword,'%') or d3 like concat('%',?keyword,'%') or d4 like concat('%',?keyword,'%') " +
                       " or c1 like concat('%',?keyword,'%') or c2 like concat('%',?keyword,'%') or c3 like concat('%',?keyword,'%') or c4 like concat('%',?keyword,'%') " +
                       " or k1 like concat('%',?keyword,'%') or k2 like concat('%',?keyword,'%') or k3 like concat('%',?keyword,'%') or k4 like concat('%',?keyword,'%') ) order by c4 ";
            return DataAccess.ExecuteReader(sql, new MySqlParameter("keyword", keyword));
        }

        public static MySqlDataReader getOccupation(string ids)
        {
            string sql = "select isco08id,groupcode,title,type from isco08 where isco08id in (" + ids + ")";
            MySqlDataReader dr = DataAccess.ExecuteReader(sql);
            return dr;
        }

        public static MySqlDataReader searchIndustry(string keyword)
        {
            string sql = "Select SQL_NO_CACHE * from (Select  t1.isicRev4id as id1,t2.isicRev4id as id2,t3.isicRev4id as id3, t4.isicRev4id as id4,t1.description as d1, t1.code as c1,t2.description as d2,t2.code as c2,t3.description as d3,t3.code as c3,t4.description as d4,t4.code as c4,t4.code,t1.keyword as k1,t2.keyword k2,t3.keyword as k3,t4.keyword as k4,JobIndustryDetailsCount(t1.isicRev4id) as t1Count, JobIndustryDetailsCount(t2.isicRev4id) as t2Count, JobIndustryDetailsCount(t3.isicRev4id) as t3Count  " +
                      ",JobDetailsCount(t4.isicRev4id) as t4Count,(JobIndustryDetailsCount(t1.isicRev4id) + JobIndustryDetailsCount(t2.isicRev4id) +" +
                        "JobIndustryDetailsCount(t3.isicRev4id) +" +
                        "JobIndustryDetailsCount(t4.isicRev4id)) as Total from ISICRev4 t1 inner join ISICRev4 t2 on t2.parentcode=t1.code and t1.level=1  INNER JOIN ISICRev4 t3 on " + "t3.parentcode=t2.code and t2.level=2 INNER JOIN ISICRev4 t4 on t4.parentcode=t3.code and t3.level=3 " +
                       " where t4.level=4  and JobIndustryDetailsCount(t4.isicRev4id)>0) a where ( d1 like concat('%',?keyword,'%') or d2 like concat('%',?keyword,'%') or d3 like concat('%',?keyword,'%') or d4 like concat('%',?keyword,'%') " +
                       " or c1 like concat('%',?keyword,'%') or c2 like concat('%',?keyword,'%') or c3 like concat('%',?keyword,'%') or c4 like concat('%',?keyword,'%') " +
                       " or k1 like concat('%',?keyword,'%') or k2 like concat('%',?keyword,'%') or k3 like concat('%',?keyword,'%') or k4 like concat('%',?keyword,'%') ) order by c4";

            return DataAccess.ExecuteReader(sql, new MySqlParameter("keyword", keyword));
        }

        public static MySqlDataReader searchAllOccupation(string keyword)
        {
            string sql = "select SQL_NO_CACHE * from( Select  t1.isco08id as id1,t2.isco08id as id2,t3.isco08id as id3,t4.isco08id as id4,t1.groupcode as c1 ,t1.title as d1,t2.groupcode as c2 ,t2.title as d2 ,t3.groupcode as c3,t3.title as d3 ,t4.groupcode as c4 ," +
                       " t4.title as d4,t1.keyword as k1,t2.keyword as k2,t3.keyword as k3, t4.keyword as k4 from isco08 t1  join isco08 t2 on t1.groupcode=t2.parentcode and t1.type=1  join isco08 t3 on t2.groupcode=t3.parentcode and t2.type=2 " +
                       " join isco08 t4 on t3.groupcode=t4.parentcode and t3.type =3  where t4.type=4) a where ( d1 like concat('%',?keyword,'%') or d2 like concat('%',?keyword,'%') or d3 like concat('%',?keyword,'%') or d4 like concat('%',?keyword,'%') " +
                       " or c1 like concat('%',?keyword,'%') or c2 like concat('%',?keyword,'%') or c3 like concat('%',?keyword,'%') or c4 like concat('%',?keyword,'%') " +
                       " or k1 like concat('%',?keyword,'%') or k2 like concat('%',?keyword,'%') or k3 like concat('%',?keyword,'%') or k4 like concat('%',?keyword,'%') ) order by c4 ";
            return DataAccess.ExecuteReader(sql, new MySqlParameter("keyword", keyword));
        }

        public static MySqlDataReader searchAllIndustry(string keyword)
        {
            string sql = "Select SQL_NO_CACHE * from (Select  t1.isicRev4id as id1,t2.isicRev4id as id2,t3.isicRev4id as id3, t4.isicRev4id as id4,t1.description as d1, t1.code as c1,t2.description as d2,t2.code as c2,t3.description as d3,t3.code as c3,t4.description as d4,t4.code as c4,t4.code,t1.keyword as k1,t2.keyword k2,t3.keyword as k3,t4.keyword as k4 " +
                        " from ISICRev4 t1 inner join ISICRev4 t2 on t2.parentcode=t1.code and t1.level=1  INNER JOIN ISICRev4 t3 on t3.parentcode=t2.code and t2.level=2 INNER JOIN ISICRev4 t4 on t4.parentcode=t3.code and t3.level=3 " +
                        " where t4.level=4 ) a where ( d1 like concat('%',?keyword,'%') or d2 like concat('%',?keyword,'%') or d3 like concat('%',?keyword,'%') or d4 like concat('%',?keyword,'%') " +
                        " or c1 like concat('%',?keyword,'%') or c2 like concat('%',?keyword,'%') or c3 like concat('%',?keyword,'%') or c4 like concat('%',?keyword,'%') " +
                        " or k1 like concat('%',?keyword,'%') or k2 like concat('%',?keyword,'%') or k3 like concat('%',?keyword,'%') or k4 like concat('%',?keyword,'%') ) order by id4,code";

            return DataAccess.ExecuteReader(sql, new MySqlParameter("keyword", keyword));
        }

        public static MySqlDataReader getIndustry(string ids)
        {
            string sql = "select isicRev4id,code,description,level from ISICRev4 where isicRev4id in (" + ids + ")";
            MySqlDataReader dr = DataAccess.ExecuteReader(sql);
            return dr;
        }

        public static MySqlDataReader searchLocation(string keyword)
        {
            string sql = "select * from(" +
                     " select c.countryid,c.name,0 as locationid, '' as locationname,0 as sublocationid,'' as sublocation,0 as subsublocationid,''  as subsublocation,0 as location_groupid,'' as groupname from countries c " +
                     " where c.name like concat('%',?keyword,'%') union " +
                     " select c.countryid,c.name,l.locationid, l.name as locationname ,0 as sublocationid,'' as sublocation,0 as subsublocationid,''  as subsublocation,0 as location_groupid,'' as groupname " +
                     " from countries c left join locations l on l.countryid=c.countryid where  c.name like concat('%',?keyword,'%') or l.name like concat('%',?keyword,'%') union " +
                     " select c.countryid,c.name,l.locationid, l.name as locationname,s.sublocationid,s.sublocation,0 as subsublocationid,''  as subsublocation,0 as location_groupid,'' as groupname " +
                     " from countries c left join locations l on l.countryid=c.countryid left join locationsub s on s.locationid=l.locationid where c.name like concat('%',?keyword,'%') or l.name like concat('%',?keyword,'%') or " +
                     " sublocation like concat('%',?keyword,'%') union " +
                     " select c.countryid,c.name,l.locationid, l.name as locationname,s.sublocationid,s.sublocation,ss.subsublocationid,ss.name as subsublocation,0 as location_groupid,'' as groupname from countries c " +
                     " left join locations l on l.countryid=c.countryid left join locationsub s on s.locationid=l.locationid left join locationsub_subs ss on ss.sublocationid=s.sublocationid " +
                     " where c.name like concat('%',?keyword,'%') or l.name like concat('%',?keyword,'%') or sublocation like concat('%',?keyword,'%') or ss.name like concat('%',?keyword,'%') " +
                     " union select 0 as countryid,'' as name,0 as locationid, '' as locationname,0 as sublocationid,'' as sublocation,0 as subsublocationid,''  as subsublocation,g.location_groupid,g.groupname from location_group g " +
                     " where groupname like concat('%',?keyword,'%') " +
                         " ) as tbl order by name,groupname,locationname,sublocation,subsublocation asc ";

            return DataAccess.ExecuteReader(sql, new MySqlParameter("keyword", keyword));
        }

        public static MySqlDataReader getGroupLocations(int groupId)
        {
            string sql = "select vl.countryid,vl.name,vl.locationid, vl.locationname as locationname,vl.sublocationid,vl.sublocation,vl.subsublocationid,vl.subsublocation as subsublocation,g.location_groupid,g.groupname,gd.locationtype " +
                " from location_group g inner join location_groupdetails gd on g.location_groupid=gd.location_groupid  left join v_locations vl on (vl.countryid=gd.locationid and gd.locationtype=1) " +
                " or (vl.locationid=gd.locationid and gd.locationtype=2) or (vl.sublocationid=gd.locationid and gd.locationtype=3) or (vl.subsublocationid=gd.locationid and gd.locationtype=4) " +
                "  where g.location_groupid=?groupid group by gd.locationid,gd.locationtype";
            MySqlDataReader dr = DataAccess.ExecuteReader(sql, new MySqlParameter("groupid", groupId));
            return dr;
        }

        public static MySqlDataReader getLocations(int groupId)
        {
            //string sql = "select CONCAT(lg.groupname,'#',ld.locationtype,'-',ld.locationid) as locationid from location_group lg inner join location_groupdetails ld " +
            //                "on lg.location_groupid = ld.location_groupid where ld.location_groupid = ?groupid";

            //string sql = "select CONCAT(lg.groupname,'#',ld.locationtype,'-',lo.countryid) as locationid from location_group lg " +
            //                "inner join location_groupdetails ld	on lg.location_groupid = ld.location_groupid " +
            //                "inner join locations lo on ld.locationid = lo.locationid where ld.location_groupid = ?groupid";

            string sql = "select CONCAT(g.groupname,'#',gd.locationtype,'-',vl.countryid) as locationid  " +
                " from location_group g inner join location_groupdetails gd on g.location_groupid=gd.location_groupid  left join v_locations vl on (vl.countryid=gd.locationid and gd.locationtype=1) " +
                " or (vl.locationid=gd.locationid and gd.locationtype=2) or (vl.sublocationid=gd.locationid and gd.locationtype=3) or (vl.subsublocationid=gd.locationid and gd.locationtype=4) " +
                "  where g.location_groupid=?groupid group by gd.locationid,gd.locationtype";
            MySqlDataReader dr = DataAccess.ExecuteReader(sql, new MySqlParameter("groupid", groupId));
            return dr;
        }

        public static MySqlDataReader getcountryid(int locationid)
        {
            string sql = "select countryid from locations where locationid = ?locationid";
            MySqlDataReader dr = DataAccess.ExecuteReader(sql, new MySqlParameter("locationid", locationid));
            return dr;
        }

        public static MySqlDataReader getcountrylist(int groupId)
        {
            string sql = "select locationid from location_groupdetails where location_groupid = ?groupid";
            MySqlDataReader dr = DataAccess.ExecuteReader(sql, new MySqlParameter("groupid", groupId));
            return dr;
        }

        public static MySqlDataReader listOccupation()
        {
            //string sql = "select   title,isco08id from isco08";
            string sql = "select * from( Select isco08id,groupcode ,concat('>',title) as title from isco08 where type=1 Union Select t2.isco08id,t2.groupcode ,concat('>>',t2.title ) as title from isco08 t1 left join isco08 t2 on t1.groupcode=t2.parentcode where t2.type=2 " +
                       " Union Select t3.isco08id,t3.groupcode ,concat('>>>',t3.title  ) from isco08 t1 left join isco08 t2 on t1.groupcode=t2.parentcode and t2.type=2 left join isco08 t3 on t2.groupcode=t3.parentcode and t2.type=2 where t3.type=3 " +
                       " Union Select t4.isco08id,t4.groupcode ,concat('>>>>',t4.title ) from isco08 t1 left join isco08 t2 on t1.groupcode=t2.parentcode  left join isco08 t3 on t2.groupcode=t3.parentcode " +
                       " left join isco08 t4 on t3.groupcode=t4.parentcode and t3.type =3 where t4.type=4 ) a  order by groupcode ,title ";
            return DataAccess.ExecuteReader(sql);
        }

        public static MySqlDataReader listJobClassification()
        {
            string sql = "select JobIndustrySubID,concat(Ji.Classification,' - ',Js.SubClassification) as classification from jobindustry Ji " +
                         "inner join jobindustrysub Js on Ji.jobindustryid = Js.jobindustryid order by Ji.Classification";

            return DataAccess.ExecuteReader(sql);
        }

        public static MySqlDataReader listJobIndustry()
        {
            string sql = "SELECT Classification,Classification,JobIndustryID FROM jobindustry order by Classification";

            return DataAccess.ExecuteReader(sql);
        }

        public static MySqlDataReader listJobIndustrySub()
        {
            string sql = "select JobIndustrySubID,Js.SubClassification,Ji.Classification from jobindustry Ji " +
                         "inner join jobindustrysub Js on Ji.jobindustryid = Js.jobindustryid order by Ji.Classification,Js.SubClassification";

            return DataAccess.ExecuteReader(sql);
        }

        public static MySqlDataReader listLocations()
        {
            string sql = "select l.name,l.name as location,l.locationid " +
                "from locations as l " +
                "join countries as c on c.countryid = l.countryid where l.locationid>0 " +
                "order by c.name, l.name";
            return DataAccess.ExecuteReader(sql);
        }

        public static MySqlDataReader listJobType()
        {
            string sql = "select SQL_NO_CACHE JobTypeId, Type,Type from jobType";
            MySqlDataReader drS = DataAccess.ExecuteReader(sql);
            return drS;
        }

        public static MySqlDataReader listAlertFrequency()
        {
            string sql = "select SQL_NO_CACHE alertFrequencyId,Frequency from alert_frequency";
            MySqlDataReader dr = DataAccess.ExecuteReader(sql);
            return dr;
        }

        public static MySqlDataReader getConsultantsLocations()
        {
            string sql = "select * from (Select case when c.countryid is null then l.countryid else c.countryid end as countryid, case when c.name is null then c1.name else c.name end as countrty, " +
                         " l.locationid,l.name as locationname,count(cl.consultantid) from Consultants co inner join  consultants_locations cl on cl.consultantid=co.consultantid " +
                         " left join countries c on cl.locationid=c.countryid and cl.locationtype=1 " +
                         " left join locations l on cl.locationid=l.locationid and cl.locationtype=2 left join countries c1 on l.countryid=c1.countryid where co.displayatwebsite=1 group by c.countryid,l.countryid,c.name,l.name) as t1 where countryid <> '';" +
                         " select * from (Select case when c.countryid is null then l.countryid else c.countryid end as countryid, case when c.name is null then c1.name else c.name end as country " +
                         " from Consultants co inner join  consultants_locations cl on cl.consultantid=co.consultantid left join countries c on cl.locationid=c.countryid and cl.locationtype=1 " +
                         " left join locations l on cl.locationid=l.locationid and cl.locationtype=2 left join countries c1 on l.countryid=c1.countryid  where co.displayatwebsite=1 group by countryid,country order by country) as t1 where countryid <> '';";

            MySqlDataReader dr = DataAccess.ExecuteReader(sql);
            return dr;
        }

        public static MySqlDataReader getConsultantsByLocationId(int locationId, int locationtype)
        {
            string sql = " select c.consultantid, c.first,c.last,concat(c.first,', ',c.last) as name from consultants c inner join consultants_locations cl on cl.consultantid=c.consultantid  " +
                          " left join locations l on cl.locationid=l.locationid left join countries cn on l.countryid=cn.countryid where c.displayatwebsite=1  " +
                           " union select c.consultantid, c.first,c.last,concat(c.first,', ',c.last) as name from consultants c inner join consultants_locations cl on cl.consultantid=c.consultantid " +
                         " where c.displayatwebsite=1 and cl.locationid=?locationid and cl.locationtype=?locationtype group by consultantid, first, last, name";
            MySqlDataReader dr = DataAccess.ExecuteReader(sql, new MySqlParameter("locationid", locationId), new MySqlParameter("locationtype", locationtype));
            return dr;
        }

        public static MySqlDataReader getConsultantsByLocationId(int locationId, int locationtype, int countryid)
        {
            string sql = " select c.consultantid, c.first,c.last,concat(ucase(c.last),', ',lcase(c.first)) as name from consultants c inner join consultants_locations cl on cl.consultantid=c.consultantid  " +
                          " inner join countries cn on cl.locationid=cn.countryid and cl.locationtype=1 where c.displayatwebsite=1 and cn.countryid=?countryid  " +
                           " union select c.consultantid, c.first,c.last,concat(ucase(c.last),', ',lcase(c.first)) as name from consultants c inner join consultants_locations cl on cl.consultantid=c.consultantid " +
                         " where c.displayatwebsite=1 and cl.locationid=?locationid and cl.locationtype=?locationtype group by consultantid, first, last, name order by last";
            MySqlDataReader dr = DataAccess.ExecuteReader(sql, new MySqlParameter("locationid", locationId), new MySqlParameter("locationtype", locationtype), new MySqlParameter("countryid", countryid));
            return dr;
        }

        public static MySqlDataReader getConsultantProfile(int consultantId)
        {
            string sql = "select c.consultantid, c.title, c.first, c.last,c.nickname,  c.imagepath, c.geographicfamiliarity, c.industryspecialisation, c.designation, c.profileinfo,c.skype,c.languages,c.qualifications  " +
               "from consultants c " +
               "where c.consultantid = ?consultantid; " +

               "select pn.phonenumberid, cpn.phonenumbertypeid, pn.number " +
               "from consultants_phonenumbers as cpn " +
               "join phonenumbers as pn on pn.phonenumberid = cpn.phonenumberid " +
               "where cpn.consultantid = ?consultantid " +
               "order by pn.phonenumberid; " +

               "select ca.addressid, ca.addresstypeid " +
               "from consultants_addresses as ca " +
               "where ca.consultantid = ?consultantid " +
               "order by ca.addressid; " +

               // "select cl.consultant_languageid, cl.languageid, cl.spoken, cl.written, l.name as language, cl.listening, cl.reading " +
               //"from consultants_languages as cl inner join languages l on cl.languageid=l.languageid " +
               //" where cl.consultantid = ?consultantid; " +

               //"select cl.consultantqualificationid, cl.qualificationid, cl.institutionid, YEAR(cl.obtained) as obtained, l.name as qualification " +
               //"from consultants_qualifications as cl inner join qualifications l on cl.qualificationid=l.qualificationid " +
               //" where cl.consultantid = ?consultantid; " +

               "Select concat(case when c.name is null then c1.name else c.name end ,coalesce(concat(' (',l.name),')')) as location, case when c.name is null then c1.name else c.name end as country,l.name as adminidivision " +
                " from Consultants co inner join  consultants_locations cl on cl.consultantid=co.consultantid left join countries c on cl.locationid=c.countryid and cl.locationtype=1 " +
                " left join locations l on cl.locationid=l.locationid and cl.locationtype=2 left join countries c1 on l.countryid=c1.countryid where co.consultantid=?consultantid order by country";

            MySqlDataReader dr = DataAccess.ExecuteReader(sql, new MySqlParameter("consultantId", consultantId));
            return dr;
        }

        public static MySqlDataReader getAddress(uint addressid)
        {
            string sql = "select al.* " +
                "from addresslines as al " +
                "where al.addressid = ?addressid " +
                "order by al.line, al.position; " +

                "select c.countryid, c.name, c.code " +
                "from addresses as a " +
                "join countries as c on c.countryid = a.countryid " +
                "where a.addressid = ?addressid";
            return DataAccess.ExecuteReader(sql, new MySqlParameter("addressid", addressid));
        }

        public static bool CandidateFileExist(int candidateid, int filetype, string filename, int filesize)
        {
            bool exist = false;
            string sql = "select * from candidates_files cf join files f on cf.fileid=f.fileid where filetypeid=?filetype and f.name=?filename and f.size=?filesize and cf.candidateid=?candidateid";
            MySqlDataReader dr = DataAccess.ExecuteReader(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("filetype", filetype), new MySqlParameter("filesize", filesize), new MySqlParameter("filename", filename));
            if (dr.HasRows)
                exist = true;
            dr.Close();
            dr.Dispose();

            return exist;
        }

        public static MySqlDataReader GetFileExtension(int filetypeid)
        {
            string sql = "select SQL_NO_CACHE webfileextension, coalesce(webfilesize,0) as filesize from filetypes where filetypeid = ?filetypeid";
            MySqlDataReader reader = DataAccess.ExecuteReader(sql, new MySqlParameter("filetypeid", filetypeid));
            return reader;
        }

        public static MySqlDataReader listTitle()
        {
            string sql = "select SQL_NO_CACHE s.title from salutations as s order by s.title";
            MySqlDataReader drS = DataAccess.ExecuteReader(sql);
            return drS;
        }

        public static MySqlDataReader searchActiveCountries(string keyword)
        {
            string sql = "select SQL_NO_CACHE countryid, name from countries where active = 1 and name like concat('%',?keyword,'%') order by name";
            return DataAccess.ExecuteReader(sql, new MySqlParameter("keyword", keyword));
        }

        public static MySqlDataReader listPhoneNumberTypes()
        {
            string sql = "select SQL_NO_CACHE phonenumbertypeid, name from phonenumbertypes order by name";
            return DataAccess.ExecuteReader(sql);
        }

        public static MySqlDataReader listCurrencies()
        {
            string sql = "select SQL_NO_CACHE currencyid, currencyid as name from currencies as c order by c.name";
            MySqlDataReader drS = DataAccess.ExecuteReader(sql);
            return drS;
        }

        public static MySqlDataReader listFrequency()
        {
            string sql = "select SQL_NO_CACHE frequencyId, frequency as name from frequency as c order by c.sort";
            MySqlDataReader drS = DataAccess.ExecuteReader(sql);
            return drS;
        }

        public static MySqlDataReader SearchEmployer(string keyword)
        {
            string sql = "select employerid,employername from employer where employername like concat('%',?keyword,'%') order by employername";
            MySqlDataReader dr = DataAccess.ExecuteReader(sql, new MySqlParameter("keyword", keyword));
            return dr;
        }

        public static int addEmployer(string name)
        {
            int id = 0;
            string sql = "insert into employer(employername) values(?name);select last_insert_id()";
            id = Convert.ToInt32(DataAccess.ExecuteScalar(sql, new MySqlParameter("name", name)));
            return id;
        }

        public static int existEmployer(string name)
        {
            int id = 0;
            string sql = "select SQL_NO_CACHE employerid from employer where employername =?name";
            MySqlDataReader dr = DataAccess.ExecuteReader(sql, new MySqlParameter("name", name));
            if (dr.HasRows)
            {
                dr.Read();
                id = Convert.ToInt32(DataAccess.getString(dr, "employerid"));
            }
            dr.Close();
            dr.Dispose();
            return id;
        }

        public static MySqlDataReader getCountryByIds(string ids)
        {
            string sql = "select SQL_NO_CACHE countryid,name from countries where countryid in(" + ids + ")";
            MySqlDataReader dr = DataAccess.ExecuteReader(sql);
            return dr;
        }

        public static MySqlDataReader getTooltip()
        {
            string sql = "select SQL_NO_CACHE label,helptext from website_tooltips";
            MySqlDataReader dr = DataAccess.ExecuteReader(sql);
            return dr;
        }

        public static MySqlDataReader listEmailTypes()
        {
            string sql = "select SQL_NO_CACHE emailtypeid, name from emailtypes order by name";
            return DataAccess.ExecuteReader(sql);
        }

        public MySqlDataReader listCountries()
        {
            string sql = "select SQL_NO_CACHE id, name from contactuslocations order by name";
            return DataAccess.ExecuteReader(sql);
        }

        public MySqlDataReader GetCities(int CityId)
        {
            string sql = "select SQL_NO_CACHE id, Name from contactareas  where LocationId = ?CityId  order by Name ";
            MySqlDataReader dr = DataAccess.ExecuteReader(sql, new MySqlParameter("CityId", CityId));
            return dr;
        }

        public MySqlDataReader GetConsultants(int CityId)
        {
            string sql = "select SQL_NO_CACHE id, ConsultantName from contactusinfo  where AreaId = ?CityId  order by ConsultantName ";
            MySqlDataReader dr = DataAccess.ExecuteReader(sql, new MySqlParameter("CityId", CityId));
            return dr;
        }
        public MySqlDataReader GetConsultantsfromInfo(int CityId)
        {
            string sql = "select SQL_NO_CACHE id, ConsultantName from contactusinfo  where LocationId = ?CityId  order by ConsultantName ";
            MySqlDataReader dr = DataAccess.ExecuteReader(sql, new MySqlParameter("CityId", CityId));
            return dr;
        }


        public MySqlDataReader GetContactDetails(int Contactid)
        {
            string sql = "select SQL_NO_CACHE * from contactusinfo  where Id = ?Contactid  order by Name ";
            MySqlDataReader dr = DataAccess.ExecuteReader(sql, new MySqlParameter("Contactid", Contactid));
            return dr;
        }
    } 
}