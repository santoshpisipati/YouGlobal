using GlobalPanda.BusinessInfo;
using MySql.Data.MySqlClient;
using System;

namespace GlobalPanda.DataProviders
{
    /// <summary>
    /// Summary description for JobBoardDataprovider
    /// </summary>
    public class JobBoardDataprovider
    {
        public static int insertJobBoard(JobBoardInfo info)
        {
            int boardId = 0;
            string sql = "insert into job_boards (boardname,boardURL,notes,userid,createddate,username,password) values (?boardname,?boardURL,?notes,?userid,?createddate,?username,?password); select last_insert_id()";
            boardId = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("boardname", info.BoardName), new MySqlParameter("boardURL", info.BoardURL), new MySqlParameter("notes", info.Notes),
                new MySqlParameter("userid", GPSession.UserId), new MySqlParameter("createddate", DateTime.UtcNow), new MySqlParameter("username", info.Username), new MySqlParameter("password", info.Password)));

            foreach (JobBoardIndustry industry in info.Industry)
            {
                insertJobboardIndustry(boardId, industry.ISICRev4Id);
            }

            foreach (JobBoardLocation location in info.Locations)
            {
                insertJobBoardLocation(boardId, location.LocationId, location.LocationType);
            }
            return boardId;
        }

        public static void updateJobBoard(JobBoardInfo info)
        {
            string sql = "update job_boards set boardname=?boardname,boardURL=?boardURL,notes=?notes,username=?username,password=?password where job_boardid=?boardid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("boardid", info.JobBoardId), new MySqlParameter("boardname", info.BoardName), new MySqlParameter("boardURL", info.BoardURL), new MySqlParameter("notes", info.Notes),
                new MySqlParameter("username", info.Username), new MySqlParameter("password", info.Password));

            deleteJobBoardIndustry(info.JobBoardId);
            foreach (JobBoardIndustry industry in info.Industry)
            {
                insertJobboardIndustry(info.JobBoardId, industry.ISICRev4Id);
            }

            deleteJobBoardLocation(info.JobBoardId);
            foreach (JobBoardLocation location in info.Locations)
            {
                insertJobBoardLocation(info.JobBoardId, location.LocationId, location.LocationType);
            }
        }

        public static void deleteJobBoard(int boardId)
        {
            string sql = "delete from job_boards where job_boardid=?boardid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("boardid", boardId));
        }

        private static void insertJobboardIndustry(int boardId, int isicrev4Id)
        {
            string sql = "insert into jobboard_industry (job_boardid,isicrev4id) values (?boardid,?isicrev4id)";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("boardid", boardId), new MySqlParameter("isicrev4id", isicrev4Id));
        }

        public static void deleteJobBoardIndustry(int boardId)
        {
            string sql = "delete from jobboard_industry where job_boardid=?boardid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("boardId", boardId));
        }

        private static void insertJobBoardLocation(int boardId, int locationId, int locationtype)
        {
            string sql = "insert into jobboard_locations (job_boardid,locationid,locationtype) values (?boardid,?locationid,?locationtype)";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("boardid", boardId), new MySqlParameter("locationid", locationId), new MySqlParameter("locationtype", locationtype));
        }

        public static void deleteJobBoardLocation(int boardId)
        {
            string sql = "delete from jobboard_locations where job_boardid=?boardid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("boardid", boardId));
        }

        public static MySqlDataReader searchJobBoard(string keyword)
        {
            string sql = "select b.job_boardid,boardname,boardURL,notes,username,password,case when count(e.job_boardid)>0 then 1 else 0 end as isused from job_boards b left join jobboard_export e on b.job_boardid=e.job_boardid " +
                " where boardname like concat('%',?keyword,'%') group by b.job_boardid,boardname,boardURL,notes";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword));
            return dr;
        }

        public static MySqlDataReader getAllJobBoard()
        {
            string sql = "select job_boardid,boardname,boardURL,notes from job_boards";
            MySqlDataReader dr = DAO.ExecuteReader(sql);
            return dr;
        }

        public static MySqlDataReader getJobBoardByJobId(int jobid)
        {
            string sql = "select jb.job_boardid,boardname,boardURL,notes,coalesce(je.job_boardid,0) as isExport from job_boards jb left join jobboard_export je on jb.job_boardid=je.job_boardid and jobdetailid=?jobid";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("jobid", jobid));
            return dr;
        }

        public static MySqlDataReader getJobBoard(int id)
        {
            string sql = "select * from job_boards where job_boardid=?id;" +
            " select job_boardid,ji.isicrev4id,code,description,concat( case when level=1 then '> ' else case when level=2 then '>> ' else case when level=3 then '>>> ' else 'CLASS ' end end end, code,' ',description) as industry " +
            " from jobboard_industry ji join isicrev4 i on ji.isicrev4id=i.isicrev4id where job_boardid=?id union select job_boardid,ji.isicrev4id,'' as code,'- Any -' as description,'- Any -'  as industry " +
            " from jobboard_industry ji   where job_boardid=?id and ji.isicrev4id=0;" +
            " select job_boardid,jl.locationid,locationtype,case when jl.locationtype=1 then name else case when jl.locationtype=2 then concat(name,'>',locationname) else case when jl.locationtype=3 then concat(name,'>',locationname,'>',sublocation)" +
            " else concat(name,'>',locationname,'>',sublocation,'>',subsublocation) end end end as location,case when jl.locationtype=1 then concat(countryid,':',1) else case when jl.locationtype=2 then concat(countryid,',',l.locationid,':',2) else" +
            " case when jl.locationtype=3 then concat(countryid,',',l.locationid,'>',sublocationid,':',3) else concat(countryid,',',l.locationid,',',sublocationid,',',subsublocationid,':',4) end end end as locationids from jobboard_locations jl " +
            " join v_locations l on (jl.locationid=l.countryid and jl.locationtype=1 and jl.locationid!=0 ) or (jl.locationid=l.locationid and jl.locationtype=2) or (jl.locationid=l.sublocationid and jl.locationtype=3) or " +
            " (jl.locationid=l.subsublocationid and jl.locationtype=4) where job_boardid=?id group by job_boardid,jl.locationid,locationtype " +
            " union select jl.job_boardid,jl.locationid,jl.locationtype,'Anywhere'as location,concat('0',':',1) as locationids from  jobboard_locations jl where job_boardid=?id and jl.locationid=0 and jl.locationtype=1 " +
            " union select jl.job_boardid,jl.locationid,jl.locationtype,groupname as location,concat('0,0,0,0,',location_groupid,':',5) as locationids from  jobboard_locations jl join location_group g on jl.locationtype=5 and jl.locationid=g.location_groupid where job_boardid=?id  ";

            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("id", id));
            return dr;
        }

        public static MySqlDataReader getJobboardIndustry(int boardId)
        {
            string sql = " select job_boardid,ji.isicrev4id,code,concat( code,' ',description) as description from jobboard_industry ji join isicrev4 i on ji.isicrev4id=i.isicrev4id where job_boardid=?boardId " +
                "union select job_boardid,ji.isicrev4id,'' as code,'- Any -' as description from jobboard_industry ji   where job_boardid=?boardId and ji.isicrev4id=0;";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("boardId", boardId));
            return dr;
        }

        public static MySqlDataReader getJobboardLocation(int boardId)
        {
            string sql = " select job_boardid,jl.locationid,locationtype,case when jl.locationtype=1 then name else case when jl.locationtype=2 then concat(name,'>',locationname) else case when jl.locationtype=3 then " +
                " concat(name,'>',locationname,'>',sublocation) else concat(name,'>',locationname,'>',sublocation,'>',subsublocation) end end end as location from jobboard_locations jl join v_locations l on (jl.locationid=l.countryid and jl.locationtype=1 and jl.locationid !=0) " +
                " or (jl.locationid=l.locationid and jl.locationtype=2) or (jl.locationid=l.sublocationid and jl.locationtype=3) or (jl.locationid=l.subsublocationid and jl.locationtype=4) where job_boardid=?boardId " +
                " union select jl.job_boardid,jl.locationid,jl.locationtype,'Anywhere'as location from  jobboard_locations jl where job_boardid=?boardId and jl.locationid=0 and jl.locationtype=1 " +
                " union select jl.job_boardid,jl.locationid,jl.locationtype,groupname as location from  jobboard_locations jl join location_group g on jl.locationtype=5 and jl.locationid=g.location_groupid where job_boardid=?boardId ";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("boardId", boardId));
            return dr;
        }

        public static MySqlDataReader getGNAds4U_Classification()
        {
            string sql = "select * from GNAds4U_classification";
            MySqlDataReader dr = DAO.ExecuteReader(sql);
            return dr;
        }

        public static void insertJobBoardJob(JobBoardJobInfo info)
        {
            string sql = "insert into jobboard_export (job_boardid,jobdetailid,Adtitle,adText,classification,saletype,countryid,locationid,minsalary,maxsalary,contactno,email,longitude,latitude,expirydate) values ( " +
                "?job_boardid,?jobdetailid,?Adtitle,?adText,?classification,?saletype,?countryid,?locationid,?minsalary,?maxsalary,?contactno,?email,?longitude,?latitude,?expirydate)";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("job_boardid", info.JobBoardId), new MySqlParameter("jobdetailid", info.JobId), new MySqlParameter("Adtitle", info.AdTitle), new MySqlParameter("Adtext", info.AdText),
                new MySqlParameter("classification", info.ClassificationId), new MySqlParameter("saletype", info.SaleType), new MySqlParameter("countryid", info.CountryId), new MySqlParameter("locationid", info.LocationId),
                new MySqlParameter("minsalary", info.MinSalary), new MySqlParameter("maxsalary", info.MaxSalary), new MySqlParameter("contactno", info.ContactNo), new MySqlParameter("email", info.Email),
                new MySqlParameter("longitude", info.LocationId), new MySqlParameter("latitude", info.Latitude), new MySqlParameter("expirydate", info.ExpiryDate));
        }

        public static void updateJobBoardJob(JobBoardJobInfo info)
        {
            string sql = "update jobboard_export set Adtitle=?adtitle,adText=?adtext, classification=?classification,saletype=?saletype,countryid=?countryid,locationid=?locationid,minsalary=?minsalary,maxsalary=?maxsalary,contactno=?contactno,email=?email," +
                " longitude=?longitude,latitude=?latitude,expirydate=?expirydate where job_boardid=?job_boardid and jobdetailid=?jobdetailid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("job_boardid", info.JobBoardId), new MySqlParameter("jobdetailid", info.JobId), new MySqlParameter("adtitle", info.AdTitle), new MySqlParameter("Adtext", info.AdText),
                new MySqlParameter("classification", info.ClassificationId), new MySqlParameter("saletype", info.SaleType), new MySqlParameter("countryid", info.CountryId), new MySqlParameter("locationid", info.LocationId),
                new MySqlParameter("minsalary", info.MinSalary), new MySqlParameter("maxsalary", info.MaxSalary), new MySqlParameter("contactno", info.ContactNo), new MySqlParameter("email", info.Email),
                new MySqlParameter("longitude", info.Longitude), new MySqlParameter("latitude", info.Latitude), new MySqlParameter("expirydate", info.ExpiryDate));
        }

        public static void deleteJobBoardJob(int boardId, int JobId)
        {
            string sql = "delete from jobboard_export where job_boardid=?boardid and jobdetailid=?jobid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("boardid", boardId), new MySqlParameter("jobid", JobId));
        }

        public static MySqlDataReader getJobBoardJob(int boardId, int jobId)
        {
            string sql = "select jobboard_exportid, job_boardid,jobdetailid,Adtitle,AdText,classification,saletype,e.countryid,e.locationid,minsalary,maxsalary,contactno,email,longitude,latitude,c.name as country,l.name as location," +
                " date_format(expirydate,'%d-%m-%Y') as expiry from jobboard_export e left join countries c on e.countryid=c.countryid left join locations l on e.locationid=l.locationid where job_boardid=?boardid and jobdetailid=?jobid";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("boardid", boardId), new MySqlParameter("jobid", jobId));
            return dr;
        }

        public static MySqlDataReader getJobBoardJob(int boardId)
        {
            string sql = "select referenceno as UniqueAdID,AdTitle,AdText,cl.description as Classification,Saletype,c.name as Country,l.name as Location,coalesce(Maxsalary,'') as Maxsalary ,coalesce(Minsalary,'') as Minsalary,Contactno,email as EmailId,Longitude,Latitude" +
                " from jobboard_export e join jobdetail jd on e.jobdetailid=jd.jobdetailid join gnads4u_classification cl on cl.gnads4u_classificationid=e.classification left join countries c on e.countryid=c.countryid " +
                " left join locations l on e.locationid=l.locationid where job_boardid=?boardid ";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("boardid", boardId));
            return dr;
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

            string sql = "select concat('Anywhere') as location,jl.locationid,jl.locationtype,0 as countryid,'' as country,0 as admindivisionid,'' as locationname from jobs_locations jl  where jl.locationid=0 and jl.locationtype=1 and jl.jobdetailid=?jobId" +
        " union select concat(c.name) as location,jl.locationid,jl.locationtype,c.countryid,c.name as country,0 as admindivisionid,'' as locationname from jobs_locations jl join countries c on c.countryid=jl.locationid where  jl.locationtype=1 and jl.locationid!=0 and jl.jobdetailid=?jobId" +
       " union select concat(c.name,' > ',l.name) as location,jl.locationid,jl.locationtype,c.countryid,c.name as country,l.locationid as admindivisionid,l.name as locationname from countries c inner join locations l on l.countryid=c.countryid join jobs_locations jl on l.locationid=jl.locationid " +
       " where  jl.locationtype=2 and jl.jobdetailid=?jobId" +
       " union select concat(c.name,' > ',l.name,' > ',s.sublocation) as location,jl.locationid,jl.locationtype,c.countryid,c.name as country,l.locationid as admindivisionid,l.name as locationname from countries c inner join locations l on l.countryid=c.countryid " +
       " inner join locationsub s on s.locationid=l.locationid join jobs_locations jl on jl.locationid=s.sublocationid where  jl.locationtype=3 and jl.jobdetailid=?jobId" +
       " union select concat(c.name,' > ',l.name,' >',s.sublocation,' > ',ss.name) as location,jl.locationid,jl.locationtype,c.countryid,c.name as country,l.locationid as admindivisionid,l.name as locationname from countries c inner join locations l on l.countryid=c.countryid " +
      " inner join locationsub s on s.locationid=l.locationid inner join locationsub_subs ss on ss.sublocationid=s.sublocationid join jobs_locations jl on ss.subsublocationid=jl.locationid where jl.locationtype=4 and jl.jobdetailid=?jobId" +
                         "  ";
            return DAO.ExecuteReader(sql, new MySqlParameter("jobId", jobId));
        }
    }
}