using GlobalPanda.BusinessInfo;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net.Mail;
using System.Text;

/// <summary>
/// Summary description for MenuItemDataProvider
/// </summary>
namespace GlobalPanda.DataProviders
{
    public class CandidateDataProvider
    {
        public static uint insertCandidate(uint tenantid, string title, string first, string middle, string last, string nickname, DateTime? dob, string gender, bool nodob, string namesuffix = "", string dobformat = "DMY")
        {
            //uint userid = UserDataProvider.insertUser(username, password, (uint)UserRoleDataProvider.UserRole.Candidate, tenantid);
            string sql = "insert into candidates (tenantid, title, first, middle, last, nickname, dob, gender,nodob,dobformat,namesuffix) values (?tenantid, ?title, ?first, ?middle, ?last, ?nickname, ?dob, ?gender,?nodob,?dobformat,?namesuffix); select last_insert_id()";
            uint candidateid = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("tenantid", tenantid), new MySqlParameter("title", title), new MySqlParameter("first", first), new MySqlParameter("middle", middle),
                new MySqlParameter("last", last), new MySqlParameter("nickname", nickname), new MySqlParameter("dob", dob), new MySqlParameter("gender", gender), new MySqlParameter("nodob", nodob),
                new MySqlParameter("dobformat", dobformat), new MySqlParameter("namesuffix", namesuffix)));

            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.Candidate;
            info.TypeId = (int)HistoryInfo.ActionType.Add;
            info.RecordId = candidateid;
            info.ParentRecordId = candidateid;
            info.Details = new List<HistoryDetailInfo>();

            HistoryDetailInfo detail = new HistoryDetailInfo();
            detail.ColumnName = "All";
            detail.NewValue = "tenantid: " + tenantid + " ,title: " + title + " ,first: " + first + " ,last: " + last + " ,nickname: " + nickname + " ,dob:" + dob + " ,gender:" + gender;

            info.Details.Add(detail);

            HistoryDataProvider history = new HistoryDataProvider();
            history.insertHistory(info);

            return candidateid;
        }

        public static uint insertCandidate_DupCheck(uint tenantid, string title, string first, string middle, string last, string nickname, DateTime? dob, string gender, bool nodob)
        {
            string sql = "select candidateid from candidates where tenantid=?tenantid and first=?first and middle=?middle and last=?last limit 1";
            uint candidateid = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("tenantid", tenantid), new MySqlParameter("first", first), new MySqlParameter("middle", middle), new MySqlParameter("last", last)));

            if (candidateid <= 0)
            {
                candidateid = insertCandidate(tenantid, title, first, middle, last, nickname, dob, gender, nodob, string.Empty);
            }
            return candidateid;
        }

        public static MySqlDataReader searchCandidates(string keyword, uint tenantid)
        {
            return searchCandidates(keyword, tenantid, 50, 1);
        }

        public static MySqlDataReader searchCandidates(string keyword, uint tenantid, int rowsPerPage, int pageNumber)
        {
            int offset = (pageNumber - 1) * rowsPerPage;

            string sql = @"

drop table if exists tmp_cf;
drop table if exists tmp_cf_sum;
drop table if exists tmp_cn;
drop table if exists tmp_ce;
drop table if exists tmp_c;
drop table if exists tmp_hits;

create temporary table tmp_cf as (
select  cf.candidateid,
        cf.fileid,
        f.name as filename,
        ft.name as filetype,
        match(cf.content) against (?keyword in boolean mode) as rank
from    candidates_files as cf
  left join files as f on f.fileid = cf.fileid
  left join filetypes as ft on ft.filetypeid = cf.filetypeid
where deleted=0 and  match(cf.content) against (?keyword in boolean mode));

create temporary table tmp_cf_sum as (
select  candidateid,
        sum(rank) as rank
from    tmp_cf
group by candidateid);

create temporary table tmp_cn as (
select  cn.candidateid,
        sum(match(cn.note) against (?keyword in boolean mode)) as rank
from    candidates_notes as cn
where   match(cn.note) against (?keyword in boolean mode)
group by cn.candidateid);

create temporary table tmp_c as (
select  c.candidateid,
        c.title,
        c.first,
        c.middle,
        c.last,
        c.nickname,
        match(c.first, c.middle, c.last, c.nickname) against (?keyword in boolean mode) + if(?keyword not regexp ('[^0-9]') and c.candidateid = cast(?keyword as unsigned integer),100,0) as rank
from    candidates as c
where   c.tenantid = ?tenantid
  and   (match(c.first, c.middle, c.last, c.nickname) against (?keyword in boolean mode))
  or    (?keyword not regexp ('[^0-9]') and c.candidateid = cast(?keyword as unsigned integer)));

create temporary table tmp_ce as (
select  ce.candidateid,
        sum(match(e.email) against (?keyword in boolean mode)) as rank
from    emails as e
  join  candidates_emails as ce on ce.emailid = e.emailid
where   match(e.email) against (?keyword in boolean mode)
group by ce.candidateid );

create temporary table tmp_hits as (
select  sql_calc_found_rows
        candidateid,
        count(candidateid) as hits,
        sum(rank) as rank_overall,
        sum(if(type='C',rank,0)) as rank_candidate,
        sum(if(type='F',rank,0)) as rank_file,
        sum(if(type='N',rank,0)) as rank_note,
        sum(if(type='E',rank,0)) as rank_email
from    (   select  candidateid, 'C' as type, rank
            from    tmp_c

            union all

            select  candidateid, 'F' as type, rank
            from    tmp_cf

            union all

            select  candidateid, 'N' as type, rank
            from    tmp_cn

            union all

            select  candidateid, 'E' as type, rank
            from    tmp_ce ) as h
group by candidateid
order by rank desc, candidateid
);

set @foundrows = found_rows();

select  c.candidateid,
        c.title,
        c.first,
        c.middle,
        c.last,
        c.nickname,
        c.restrictedaccess,
        null as username,
        h.hits,
        h.rank_overall,
        h.rank_candidate,
        h.rank_file,
        h.rank_note,
        h.rank_email
from    candidates as c
  join  tmp_hits as h on h.candidateid = c.candidateid
order by h.rank_overall desc, c.candidateid;

select  @foundrows as foundrows;

select  candidateid,
        fileid,
        filename,
        filetype,
        rank
from    tmp_cf
where   candidateid in ( select candidateid from tmp_hits )
order by candidateid, fileid;

drop table if exists tmp_cf;
drop table if exists tmp_cf_sum;
drop table if exists tmp_cn;
drop table if exists tmp_ce;
drop table if exists tmp_c;
drop table if exists tmp_hits;";

            return DAO.ExecuteReader(sql, new MySqlParameter("tenantid", tenantid), new MySqlParameter("keyword", keyword), new MySqlParameter("offset", offset), new MySqlParameter("rowsperpage", rowsPerPage));
            /*string sql = @"
select  sql_calc_found_rows
        c.candidateid,
        c.title,
        c.first,
        c.middle,
        c.last,
        c.nickname,
        u.username,
        cf.candidateid,
        cn.candidateid,
        match(c.first, c.middle, c.last, c.nickname, u.username) against (?keyword in boolean mode) as c_rank,
        cf.rank as cf_rank,
        cn.rank as cn_rank,
        ifnull(match(c.first, c.middle, c.last, c.nickname, u.username) against (?keyword in boolean mode),0) + ifnull(cf.rank,0) + ifnull(cn.rank,0) + if(cast(c.candidateid as char) = ?keyword,10,0) as masterrank
from    candidates as c
  left join (   select  cf.candidateid,
                        sum(match(cf.content) against (?keyword in boolean mode)) as rank
                from    candidates_files as cf
                where   match(cf.content) against (?keyword in boolean mode)
                group by cf.candidateid ) as cf on cf.candidateid = c.candidateid
  left join (   select  cn.candidateid,
                        sum(match(cn.note) against (?keyword in boolean mode)) as rank
                from    candidates_notes as cn
                where   match(cn.note) against (?keyword in boolean mode)
                group by cn.candidateid ) as cn on cn.candidateid = c.candidateid
  left join users as u on u.userid = c.userid
where   c.tenantid = ?tenantid
  and   (match(c.first, c.middle, c.last, c.nickname, u.username) against (?keyword in boolean mode)
  or    cast(c.candidateid as char) = ?keyword
  or    cf.rank or cn.rank)
order by masterrank desc, c.candidateid
limit ?offset, ?rowsperpage

select  found_rows() as foundrows;

select  cf.candidateid,
        cf.fileid,
        f.name as filename,
        ft.name as filetype,
        match(cf.content) against (?keyword in boolean mode) as rank
from    candidates_files as cf
  left join files as f on f.fileid = cf.fileid
  left join filetypes as ft on ft.filetypeid = cf.filetypeid
where   match(cf.content) against (?keyword in boolean mode)
order by candidateid, fileid;";
                //"limit offset.ToString() + ", " + rowsPerPage.ToString() + ";";*/
        }

        public static MySqlDataReader searchCandidatesProcedure(string keyword, uint tenantid, string nation, string location, string locationpreference, string occupation, string industry, string frombd, string tobd, string title, string gender, string marital)
        {
            string sql = "candidate_search4";
            return DAO.ExecuteReader(sql, CommandType.StoredProcedure, new MySqlParameter("tenantid", tenantid), new MySqlParameter("keyword", keyword), new MySqlParameter("searchlocation", location),
                new MySqlParameter("searchlocationpreference", locationpreference), new MySqlParameter("searchnation", nation), new MySqlParameter("searchoccupation", occupation), new MySqlParameter("searchindustry", industry)
                , new MySqlParameter("frombd", frombd), new MySqlParameter("tobd", tobd), new MySqlParameter("searchtitle", title), new MySqlParameter("searchgender", gender), new MySqlParameter("searchmarital", marital));
        }

        public static MySqlDataReader searchCandidateProcedure(string keyword, string filter)
        {
            string sql = "candidate_search6";
            return DAO.ExecuteReader(sql, CommandType.StoredProcedure, new MySqlParameter("tenantid", 1), new MySqlParameter("searchtxt", keyword), new MySqlParameter("searchfilter", filter));
        }

        public static MySqlDataReader searchCandidateProcedure(string keyword, string filter, string join)
        {
            string sql = "drop table if exists tmp_cf;drop table if exists tmp_location; drop table if exists tmp_location1; drop table if exists tmp_cn;drop table if exists tmp_ce;drop table if exists tmp_c;drop table if exists tmp_hits;drop table if exists tmp_hits1; " +
        " create temporary table tmp_location (country varchar(500),location varchar(1000),sublocation varchar(1000),subsublocation varchar(5000),countryid int,locationid int,sublocationid int,subsublocationid int) ; " +
        " insert into tmp_location select * from (select c.name as country,'' as location, '' as sublocation, '' as subsublocation,c.countryid,0 as locationid,0 as sublocationid,0 as subsublocationid from countries c   " +
        " union select c.name as country,coalesce(concat(c.name,' > ',l.name),'') as location, '' as sublocation, '' as subsublocation,0 as countryid,l.locationid,ls.sublocationid,lss.subsublocationid from countries c  " +
        " join locations l on c.countryid=l.countryid left join locationsub ls on l.locationid=ls.locationid left join locationsub_subs lss on ls.sublocationid=lss.sublocationid " +
        " union select c.name as country,l.name as location, concat(c.name,' > ',l.name,' > ',ls.sublocation) as sublocation, '' as subsublocation,0 as countryid,0 as locationid,ls.sublocationid,lss.subsublocationid  " +
        " from countries c  join locations l on c.countryid=l.countryid join locationsub ls on l.locationid=ls.locationid left join locationsub_subs lss on ls.sublocationid=lss.sublocationid " +
        " union select c.name as country,l.name as location, concat(c.name,' > ',l.name,' > ',ls.sublocation) as sublocation, concat(c.name,' > ',l.name,' > ',ls.sublocation,' > ',lss.name) as subsublocation,0 as countryid,0 as locationid," +
        " 0 as sublocationid,lss.subsublocationid from countries c  join locations l on c.countryid=l.countryid join locationsub ls on l.locationid=ls.locationid join locationsub_subs lss on ls.sublocationid=lss.sublocationid " +
        " ) as tbl group by country,location,sublocation,subsublocation; " +
        " create temporary table tmp_location1 (country varchar(500),location varchar(1000),sublocation varchar(1000),subsublocation varchar(5000),countryid int,locationid int,sublocationid int,subsublocationid int) ;" +
        " insert into tmp_location1 select * from (select c.name as country,'' as location, '' as sublocation, '' as subsublocation,c.countryid,0 as locationid,0 as sublocationid,0 as subsublocationid from countries c   " +
        " union select c.name as country,coalesce(concat(c.name,' > ',l.name),'') as location, '' as sublocation, '' as subsublocation,0 as countryid,l.locationid,ls.sublocationid,lss.subsublocationid from countries c  " +
        " join locations l on c.countryid=l.countryid left join locationsub ls on l.locationid=ls.locationid left join locationsub_subs lss on ls.sublocationid=lss.sublocationid " +
        " union select c.name as country,l.name as location, concat(c.name,' > ',l.name,' > ',ls.sublocation) as sublocation, '' as subsublocation,0 as countryid,0 as locationid,ls.sublocationid,lss.subsublocationid " +
        " from countries c  join locations l on c.countryid=l.countryid join locationsub ls on l.locationid=ls.locationid left join locationsub_subs lss on ls.sublocationid=lss.sublocationid " +
        " union select c.name as country,l.name as location, concat(c.name,' > ',l.name,' > ',ls.sublocation) as sublocation, concat(c.name,' > ',l.name,' > ',ls.sublocation,' > ',lss.name) as subsublocation,0 as countryid,0 as locationid," +
         " 0 as sublocationid,lss.subsublocationid from countries c  join locations l on c.countryid=l.countryid join locationsub ls on l.locationid=ls.locationid join locationsub_subs lss on ls.sublocationid=lss.sublocationid " +
        " ) as tbl group by country,location,sublocation,subsublocation; " +

        " create temporary table tmp_c as (select  c.candidateid, c.title, c.first, c.middle, c.last, c.nickname, match(c.first, c.middle, c.last, c.nickname) against (?keyword in boolean mode) + if(?keyword not regexp ('[^0-9]') and c.candidateid = ?keyword ,100,0) as rank " +
        " from  candidates as c where   c.tenantid = tenantid and ((match(c.first, c.middle, c.last, c.nickname) against (?keyword  in boolean mode))  or  (?keyword  not regexp ('[^0-9]') and c.candidateid = ?keyword ) ) group by c.candidateid );" +

        " create temporary table tmp_cf as (select cf.candidateid,cf.fileid,f.name as filename,ft.name as filetype,match(cf.content) against (?keyword in boolean mode) as rank,cf.content from  candidates c " +
        " join  candidates_files as cf on c.candidateid=cf.candidateid join files as f on f.fileid = cf.fileid join filetypes as ft on ft.filetypeid = cf.filetypeid " +
        " where deleted=0 and  match(cf.content) against (?keyword in boolean mode) group by c.candidateid,cf.fileid ) ;" +

        " create temporary table tmp_cn as (select  cn.candidateid,sum(match(cn.note) against (?keyword in boolean mode)) as rank,cn.note from   candidates c join candidates_notes as cn on c.candidateid=cn.candidateid " +
        " where   match(cn.note) against (?keyword in boolean mode) group by cn.candidateid); " +

        " create temporary table tmp_ce as (select  ce.candidateid,sum(match(e.email) against (?keyword in boolean mode)) as rank,e.email from emails as e join  candidates_emails as ce on ce.emailid = e.emailid " +
        " where   match(e.email) against (?keyword in boolean mode) group by ce.candidateid );" +

        " create temporary table tmp_hits as (select  sql_calc_found_rows  candidateid,email,note,content,count(candidateid) as hits,sum(rank) as rank_overall,sum(if(type='C',rank,0)) as rank_candidate,sum(if(type='F',rank,0)) as rank_file," +
        " sum(if(type='N',rank,0)) as rank_note,sum(if(type='E',rank,0)) as rank_email from  ( select  candidateid, 'C' as type, rank,'' as email,'' as content,'' as note from  tmp_c " +
        " union all select  candidateid, 'F' as type, rank, '' as email, content,'' as note from tmp_cf union all select  candidateid, 'N' as type, rank,'' as email,'' as content, note from tmp_cn  union all " +
        " select  candidateid, 'E' as type, rank, email,'' as content,'' as note from    tmp_ce ) as h group by candidateid,email, note, content order by rank desc, candidateid) ;" +
        " set @foundrows = found_rows(); ";

            if (string.IsNullOrEmpty(keyword.Trim()))
                sql += " create temporary table tmp_hits1 as ( select  c.candidateid,c.title,c.first,c.middle,c.last,c.nickname,c.restrictedaccess,c.nojobalerts,IFNULL(ja.job_alertid,0) as jobalertid, null as username,coalesce(h.hits,0) as hits,coalesce(h.rank_overall,0) as rank_overall, " +
                 " coalesce(h.rank_candidate,0) as rank_candidate,coalesce(h.rank_file,0) as rank_file,coalesce(h.rank_note,0) as rank_note,coalesce(h.rank_email,0) as rank_email from  candidates as c " +
                 " left join  tmp_hits as h on h.candidateid = c.candidateid " + join + "left join job_alert ja on c.candidateid = ja.candidateid where c.tenantid = 1 " + filter + " group by c.candidateid order by h.rank_overall desc, c.candidateid);";
            else
                sql += " create temporary table tmp_hits1 as ( select  c.candidateid,c.title, c.first,c.middle,c.last,c.nickname,c.nojobalerts,c.restrictedaccess,IFNULL(ja.job_alertid,0) as jobalertid,null as username,coalesce(h.hits,0) as hits,coalesce(h.rank_overall,0) as rank_overall, " +
                     " coalesce(h.rank_candidate,0) as rank_candidate,coalesce(h.rank_file,0) as rank_file,coalesce(h.rank_note,0) as rank_note,coalesce(h.rank_email,0) as rank_email from  candidates as c " +
                     " join  tmp_hits as h on h.candidateid = c.candidateid " + join + "left join job_alert ja on c.candidateid = ja.candidateid  where c.tenantid = 1 " + filter + " group by c.candidateid order by h.rank_overall desc, c.candidateid);  ";

            sql += " select * from tmp_hits1; " +
                 " select  @foundrows as foundrows; " +
                 " select  candidateid,fileid,filename,filetype,rank from tmp_cf where   candidateid in ( select candidateid from tmp_hits1 ) order by candidateid, fileid; " +
                 "drop table if exists tmp_cf;drop table if exists tmp_location; drop table if exists tmp_location1; drop table if exists tmp_cn;drop table if exists tmp_ce;drop table if exists tmp_c;drop table if exists tmp_hits;drop table if exists tmp_hits1;";//"candidate_search7";
            // return DAO.ExecuteReader(sql, CommandType.StoredProcedure, new MySqlParameter("tenantid", 1), new MySqlParameter(""+keyword+"", keyword), new MySqlParameter("searchfilter", filter),new MySqlParameter("filterjoin",join));
            return DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword));
        }

        public static MySqlDataReader searchCandidates_withTemp(string keyword, uint tenantid, int rowsPerPage, int pageNumber)
        {
            int offset = (pageNumber - 1) * rowsPerPage;

            string sql =
                "drop temporary table if exists tmp_files; " +
                "drop temporary table if exists tmp_notes; " +
                "drop temporary table if exists tmp_candidates; " +

                "create temporary table tmp_files as " +
                "select cf.candidateid, cf.fileid, f.name as filename, ft.name as filetype, match(cf.content) against (?keyword in boolean mode) as rank " +
                "from candidates_files as cf " +
                "join files as f on f.fileid = cf.fileid " +
                "join filetypes as ft on ft.filetypeid = cf.filetypeid " +
                "where match(cf.content, f.name) against (?keyword in boolean mode); " +

                "create temporary table tmp_notes as " +
                "select cn.candidateid, sum(match(cn.note) against (?keyword in boolean mode)) as rank " +
                "from candidates_notes as cn " +
                "where match(cn.note) against (?keyword in boolean mode); " +

                "create temporary table tmp_candidates as " +
                "select c.candidateid, c.title, c.first, c.middle, c.last, c.nickname, u.username, " +
                "cf.candidateid as cf_candidateid, cn.candidateid as cn_candidateid, " +
                "match(c.first, c.middle, c.last, c.nickname, u.username) against (?keyword in boolean mode) as c_rank, cf.rank as cf_rank, cn.rank as cn_rank, " +
                "ifnull(match(c.first, c.middle, c.last, c.nickname, u.username) against (?keyword in boolean mode),0) + ifnull(cf.rank,0) + ifnull(cn.rank,0) as masterrank " +
                "from candidates as c " +
                "left join ( select candidateid, sum(rank) as rank from tmp_files group by candidateid ) as cf on cf.candidateid = c.candidateid " +
                "left join ( select candidateid, sum(rank) as rank from tmp_notes group by candidateid ) as cn on cn.candidateid = c.candidateid " +
                "left join users as u on u.userid = c.userid " +
                "where c.tenantid = ?tenantid " +
                "and (match(c.first, c.middle, c.last, c.nickname, u.username) against (?keyword in boolean mode) " +
                "or cf.rank or cn.rank) " +
                "order by masterrank desc, c.candidateid " +
                "limit " + offset.ToString() + ", " + rowsPerPage.ToString() + "; " +

                "select * from tmp_candidates; " +

                "select cf.* " +
                "from tmp_files as cf " +
                "where candidateid in ( select candidateid from tmp_candidates ) " +
                "order by candidateid, fileid; " +

                "drop temporary table if exists tmp_files; " +
                "drop temporary table if exists tmp_notes; " +
                "drop temporary table if exists tmp_candidates;";

            return DAO.ExecuteReader(sql, new MySqlParameter("tenantid", tenantid), new MySqlParameter("keyword", keyword));
        }

        public static int existCandidate(string email)
        {
            int candidateId = 0;
            string sql = "select * from candidates_emails ce inner join emails e on ce.emailid=e.emailid  where e.email like concat_ws(?email,'%','%') order by ce.candidateid";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("email", email));
            if (reader.HasRows)
            {
                candidateId = Convert.ToInt32(DAO.getInt(reader, "candidateid"));
                //while (reader.Read())
                //{
                //    candidateId = Convert.ToInt32(DAO.getInt(reader, "candidateid"));
                //}
            }
            reader.Close();
            reader.Dispose();
            return candidateId;
        }

        public static MySqlDataReader getJobAlert(uint candidateid)
        {
            string sql = "Select ja.job_alertId,ja.candidateId,ja.frequencyId,ja.phonecode,ja.phonenumber,e.email,ja.confirmed from job_alert ja left join emails e on ja.emailid=e.emailid " +
                "where ja.candidateid=?candidateid; " +

      //"select jl.candidateid,jl.locationid,lo.name from jobalert_location jl " +
      //"left outer join locations lo on jl.locationid = lo.locationid " +
      //"where jl.candidateid=?candidateid; " +

      //           "Select jl.candidateid,jl.locationtype,jl.locationid,(select concat('Anywhere',',','0',',','0,0,0') from jobalert_location  where jl.locationid=0 and jl.locationtype=1 " +
      //  " union select concat(c.name,',',CAST(c.countryid as char),',','0,0,0') from countries c where c.countryid=jl.locationid and jl.locationtype=1 " +
      // " union select concat(concat(c.name,' > ',l.name),',',CAST(c.countryid as char),',',cast(l.locationid as char),',', '0,0')  from countries c inner join locations l on l.countryid=c.countryid where l.locationid=jl.locationid and jl.locationtype=2 " +
      // " union select concat(concat(c.name,' > ',l.name,' > ',s.sublocation),',',CAST(c.countryid as char),',',cast(l.locationid as char),',', cast(s.sublocationid as char),',','0')  from countries c inner join locations l on l.countryid=c.countryid " +
      //                   " inner join locationsub s on s.locationid=l.locationid where s.sublocationid=jl.locationid and jl.locationtype=3 " +
      // " union select concat(concat(c.name,' > ',l.name,' > ',s.sublocation,' > ',ss.name),',',CAST(c.countryid as char),',',cast(l.locationid as char),',', cast(s.sublocationid as char),',',cast(ss.subsublocationid as char) ) from countries c inner join locations l on l.countryid=c.countryid " +
      //" inner join locationsub s on s.locationid=l.locationid inner join locationsub_subs ss on ss.sublocationid=s.sublocationid where ss.subsublocationid=jl.locationid and jl.locationtype=4 " +
      //                   " ) as location from jobalert_location jl where jl.candidateid=?candidateid; " +

      "select jl.candidateid,jl.locationtype,jl.locationid, concat('Anywhere',',','0',',','0,0,0') as location,concat('0',':1') as lid ,concat('0',',','0,0,0') as locationids  from jobalert_location jl  where jl.locationid=0 and jl.locationtype=1 and jl.candidateid=?candidateid  " +
      " union select jl.candidateid,jl.locationtype,jl.locationid,concat(c.name,',',CAST(c.countryid as char),',','0,0,0') as location,concat(c.countryid,':1') as lid ,concat(CAST(c.countryid as char),',','0,0,0') as locationids from countries c join jobalert_location jl on c.countryid=jl.locationid and jl.locationtype=1 and jl.locationid!=0 where jl.candidateid=?candidateid  " +
      " union select jl.candidateid,jl.locationtype,jl.locationid,concat(concat(c.name,' > ',l.name),',',CAST(c.countryid as char),',',cast(l.locationid as char),',', '0,0') as location,concat(l.locationid,':2') as lid ,concat(CAST(c.countryid as char),',',cast(l.locationid as char),',', '0,0') as locationids  from countries c inner join locations l on l.countryid=c.countryid " +
                        " join jobalert_location jl on l.locationid=jl.locationid and jl.locationtype=2 where  jl.candidateid=?candidateid " +
      " union select jl.candidateid,jl.locationtype,jl.locationid,concat(concat(c.name,' > ',l.name,' > ',s.sublocation),',',CAST(c.countryid as char),',',cast(l.locationid as char),',', cast(s.sublocationid as char),',','0') as location,concat(s.sublocationid,':3') as lid ,concat(CAST(c.countryid as char),',',cast(l.locationid as char),',', cast(s.sublocationid as char),',','0') as locationids  from countries c inner join locations l on l.countryid=c.countryid " +
                        " inner join locationsub s on s.locationid=l.locationid join jobalert_location jl on s.sublocationid=jl.locationid and jl.locationtype=3 where  jl.candidateid=?candidateid  " +
                        " union select jl.candidateid,jl.locationtype,jl.locationid,concat(concat(c.name,' > ',l.name,' > ',s.sublocation,' > ',ss.name),',',CAST(c.countryid as char),',',cast(l.locationid as char),',', cast(s.sublocationid as char),',',cast(ss.subsublocationid as char) ) as location,concat(ss.subsublocationid,':4') as lid ,concat(CAST(c.countryid as char),',',cast(l.locationid as char),',', cast(s.sublocationid as char),',',cast(ss.subsublocationid as char) ) as locationids from countries c inner " +
                        " join locations l on l.countryid=c.countryid  inner join locationsub s on s.locationid=l.locationid " +
                        " inner join locationsub_subs ss on ss.sublocationid=s.sublocationid join jobalert_location jl on ss.subsublocationid=jl.locationid and jl.locationtype=4 where jl.candidateid=?candidateid " +
                        " union  select jl.candidateid,jl.locationtype,jl.locationid,concat(groupname,',','0',',','0,0,0',',',location_groupid) as location,concat(location_groupid,':5') as lid ,concat('0',',','0,0,0',',',location_groupid) as locationids from jobalert_location jl join location_group g on g.location_groupid=jl.locationid and jl.locationtype=5 " +
                " where  jl.candidateid=?candidateid; " +

                "select jw.job_typeid,jt.type from jobalert_worktype jw " +
                "left outer join jobtype jt on jw.job_typeid = jt.jobtypeid " +
                "where jw.candidateid = ?candidateid; " +

                " select ja.candidateid,coalesce( ja.isicrev4id,0) as isicrev4id,isc.description,isc.code,isc.level,v.c1 from jobalert_industry ja join isicrev4 isc on ja.isicrev4id=isc.isicrev4id " +
                " join v_isicrev4 v  on ((isc.isicrev4id=v.id1 and isc.level=1) or (isc.isicrev4id=v.id2 and isc.level=2) or (isc.isicrev4id=v.id3 and isc.level=3) or (isc.isicrev4id=v.id4 and isc.level=4)) " +
                " where ja.candidateid=?candidateid group by ja.candidateid,ja.isicrev4id,isc.description,isc.code,isc.level,v.c1 " +
                " union select ja.candidateid,coalesce( ja.isicrev4id,0) as isicrev4id,'- Any -' as description,'' as code,'0' as level,'' as c1 from jobalert_industry ja where ja.candidateid=?candidateid and ja.isicrev4id=0; " +

                "select coalesce(jw.isco08id,0) as isco08id,jt.groupcode,jt.title,jt.type from jobalert_isoc08 jw " +
                " join isco08 jt on jw.isco08id = jt.isco08id " +
                "where jw.candidateid = ?candidateid union select coalesce(jw.isco08id,0) as isco08id,'' as groupcode,'- Any -' as title,'0' as type from jobalert_isoc08 jw where jw.candidateid = ?candidateid and jw.isco08id =0 ; ";

            //"select ja.candidateid,ja.jobindustrysubid,ji.subclassification from " +
            //"jobalert_industry ja left outer join (Select distinct a.jobindustrysubid,concat(b.classification,' - ',a.subclassification) " +
            //"as subclassification from jobindustry b left outer join jobindustrysub a on b.jobindustryid = a.jobindustryid " +
            //"order by b.classification) ji on ja.jobindustrysubid = ji.jobindustrysubid " +
            //"where ja.candidateid=?candidateid order by subclassification";

            return DAO.ExecuteReader(sql, new MySqlParameter("candidateid", candidateid));
        }

        public static DataTable getCandidateEmail(int candidateId)
        {
            string sql = "select e.emailid, ce.emailtypeid, e.email, ce.defaultemail " +
                "from candidates_emails as ce " +
                "join emails as e on e.emailid = ce.emailid " +
                "where ce.candidateid = ?candidateid " +
                "order by ce.emailid; ";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("candidateId", candidateId));
            DataTable dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public static DataTable getCandidatePhoneNumber(int candidateId)
        {
            string sql = "select pn.phonenumberid, cpn.phonenumbertypeid, pn.number,pn.countrycode,pn.areacode " +
                "from candidates_phonenumbers as cpn " +
                "join phonenumbers as pn on pn.phonenumberid = cpn.phonenumberid " +
                "where cpn.candidateid = ?candidateid " +
                "order by pn.phonenumberid; ";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("candidateId", candidateId));
            DataTable dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public static MySqlDataReader getCandidateEmail(int candidateId, string email)
        {
            string sql = "select e.emailid, ce.emailtypeid, e.email, ce.defaultemail " +
                "from candidates_emails as ce " +
                "join emails as e on e.emailid = ce.emailid " +
                "where ce.candidateid = ?candidateid and e.email like concat_ws(?email,'%','%')" +
                "order by ce.emailid; ";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("candidateId", candidateId), new MySqlParameter("email", email));

            return reader;
        }

        public static uint getCandidateEmailId(int candidateId, string email)
        {
            uint emailId = 0;
            string sql = "select e.emailid from emails e inner join candidates_emails ce on ce.emailid=e.emailid where candidateid=?candidateid and e.email=?email";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("candidateid", candidateId), new MySqlParameter("email", email));
            if (dr.HasRows)
            {
                dr.Read();
                emailId = Convert.ToUInt32(DAO.getInt(dr, "emailid"));
            }
            dr.Close();
            dr.Dispose();
            return emailId;
        }

        public static MySqlDataReader getCandidate(uint candidateid)
        {
            string sql = "select c.*, u.username " +
                "from candidates as c " +
                "left join users as u on u.userid = c.userid " +
                "where c.candidateid = ?candidateid; " +

                "select cs.salarytype,cs.fromamount,cs.toamount,cs.currency,cs.frequency,cs.candidateid,cs.salarydate,cs.sourcetype,cs.sourceid,cs.userid,u.username from candidates as c inner join candidates_salary cs on cs.candidateid=c.candidateid " +
                " left join users u on cs.userid=u.userid " +
                "where cs.candidateid = ?candidateid; " +

                "select cn.countryid, c.name " +
                "from candidates_nationalities as cn " +
                "join countries as c on c.countryid = cn.countryid " +
                "where cn.candidateid = ?candidateid " +
                "order by c.name; " +

                "select cc.consultantid, c.first, c.last " +
                "from candidates_consultants as cc " +
                "join consultants as c on c.consultantid = cc.consultantid " +
                "where cc.candidateid = ?candidateid " +
                "order by c.last, c.first; " +

                "select cd.divisionid, d.name " +
                "from candidates_divisions as cd " +
                "join divisions as d on d.divisionid = cd.divisionid " +
                "where cd.candidateid = ?candidateid " +
                "order by d.name; " +

                "select cd.disciplineid, d.name " +
                "from candidates_disciplines as cd " +
                "join disciplines as d on d.disciplineid = cd.disciplineid " +
                "where cd.candidateid = ?candidateid " +
                "order by d.name; " +

                "select cs.sectorid, s.name " +
                "from candidates_sectors as cs " +
                "join sectors as s on s.sectorid = cs.sectorid " +
                "where cs.candidateid = ?candidateid " +
                "order by s.name; " +

                "select cl.locationid, l.name as location, c.name as country, cl.preferred " +
                "from candidates_locations as cl " +
                "join locations as l on l.locationid = cl.locationid " +
                "join countries as c on c.countryid = l.countryid " +
                "where cl.candidateid = ?candidateid " +
                "order by c.name, l.name; " +

                "select e.emailid, ce.emailtypeid, e.email, ce.defaultemail,case when j.emailid>0 then 1 else 0 end alertemail " +
                "from candidates_emails as ce " +
                "join emails as e on e.emailid = ce.emailid left join job_alert j on j.emailid=e.emailid " +
                "where ce.candidateid = ?candidateid group by e.emailid, ce.emailtypeid, e.email " +
                "order by ce.emailid; " +

                "select pn.phonenumberid, cpn.phonenumbertypeid, pn.number,pn.countrycode,pn.areacode " +
                "from candidates_phonenumbers as cpn " +
                "join phonenumbers as pn on pn.phonenumberid = cpn.phonenumberid " +
                "where cpn.candidateid = ?candidateid " +
                "order by pn.phonenumberid; " +

                "select ca.addressid, ca.addresstypeid " +
                "from candidates_addresses as ca " +
                "where ca.candidateid = ?candidateid " +
                "order by ca.addressid; " +

                "select coc.candidateothercontactid, coc.alternativecontactid, coc.details " +
                "from candidates_othercontacts as coc " +
                "where coc.candidateid = ?candidateid " +
                "order by coc.candidateothercontactid; " +

                "select cq.candidatequalificatonid, cq.obtained, cq.qualificationid, cq.institutionid " +
                "from candidates_qualifications as cq " +
                "where cq.candidateid = ?candidateid " +
                "order by cq.candidatequalificatonid; " +

                "select cl.candidatelanguageid, cl.languageid, cl.spoken, cl.written,cl.listening,cl.reading " +
                "from candidates_languages as cl " +
                "where cl.candidateid = ?candidateid " +
                "order by cl.candidatelanguageid; " +

                "select cl.candidatesoftwareid, cl.softwareid " +
                "from candidates_softwares as cl " +
                "where cl.candidateid = ?candidateid " +
                "order by cl.candidatesoftwareid; " +

                //"select cwh.candidateworkhistoryid, cwh.from, cwh.from_display, cwh.to, cwh.to_display, cwh.employer, cwh.locationid, cwh.industryid, cwh.positionid, cwh.salary, cwh.salarycurrency,concat('UNIT ',i.groupcode,' ',i.title) as isco " +
                //" ,concat('CLASS ',r.code,' ',r.description) as isicrev ,case when cwh.locationtype=1 then c.name else case when cwh.locationtype=2 then l.name else case when cwh.locationtype=3 then s.sublocation else ss.name end end end as location " +
                //"from candidates_workhistories as cwh left join isco08 i on cwh.positionid=i.isco08id left join isicrev4 r on cwh.industryid=r.isicrev4id left join countries c on cwh.locationid=c.countryid and cwh.locationtype=1 " +
                //"left join locations l on cwh.locationid=l.locationid and cwh.locationtype=2 left join locationsub s on cwh.locationid=s.sublocationid and cwh.locationtype=3 left join locationsub_subs ss on cwh.locationid=ss.subsublocationid and cwh.locationtype=4 " +
                //"where cwh.candidateid = ?candidateid order by cwh.from, cwh.to; " +

                "select cwh.candidateworkhistoryid, cwh.from, cwh.from_display, cwh.to, cwh.to_display, cwh.employer, cwh.locationid, cwh.industryid, cwh.positionid, cwh.salary, cwh.salarycurrency,concat('UNIT ',i.groupcode,' ',i.title) as isco " +
                " ,concat('CLASS ',r.code,' ',r.description) as isicrev ,case when cwh.locationtype=1 then vl.name else case when cwh.locationtype=2 then concat(vl.name,' > ',vl.locationname) else case when cwh.locationtype=3 then  " +
                " concat(vl.name,' > ',vl.locationname,' > ',vl.sublocation) else case when cwh.locationtype=4 then concat(vl.name,' > ',vl.locationname,' > ',vl.sublocation,' > ',vl.subsublocation) " +
                " else lg.groupname end end end end as location,e.employername,cwh.employerid " +
                "from candidates_workhistories as cwh left join isco08 i on cwh.positionid=i.isco08id left join isicrev4 r on cwh.industryid=r.isicrev4id " +
                " left join v_locations vl on (cwh.locationid=vl.countryid and cwh.locationtype=1 ) or (cwh.locationid=vl.locationid and cwh.locationtype=2) or (cwh.locationid=vl.sublocationid and cwh.locationtype=3 ) " +
                " or (cwh.locationid=vl.subsublocationid  and cwh.locationtype=4 ) left join location_group lg on (cwh.locationtype=5 or cwh.locationid=lg.location_groupid) left join employer e on cwh.employerid=e.employerid " +

                " where cwh.candidateid = ?candidateid group by cwh.candidateworkhistoryid order by cwh.from, cwh.to; " +

                "select cn.candidatenoteid, cn.created, cn.note, cn.userid, u.username, cn.consultantid, concat_ws(' ',c.first,c.last) as consultant,private " +
                "from candidates_notes as cn " +
                "join users as u on u.userid = cn.userid " +
                "left join consultants as c on c.consultantid = cn.consultantid " +
                "where cn.candidateid = ?candidateid " +
                "order by cn.created desc, cn.candidatenoteid; " +

                "select cf.fileid, f.name, cf.uploaded,date_format(cf.uploaded,'%d-%b-%Y-%T') as uploaded_format, ft.name as filetype, f.size,cf.filetypeid,f.deleted, cf.defaultimage," +
                " coalesce(cf.uploadsource,0) as uploadsource,cf.jobdetailid,cf.userid " +
                "from candidates_files as cf " +
                "join files as f on f.fileid = cf.fileid " +
                "join filetypes as ft on ft.filetypeid = cf.filetypeid " +

                "where cf.candidateid = ?candidateid " +
                "order by cf.uploaded desc, cf.fileid";

            return DAO.ExecuteReader(sql, new MySqlParameter("candidateid", candidateid));
        }

        public static MySqlDataReader getOnlyCandidate(int candidateid)
        {
            string sql = "select c.*,date_format(c.dob,'%d-%b-%Y') as dob_format, u.username " +
                "from candidates as c " +
                "left join users as u on u.userid = c.userid " +
                "where c.candidateid = ?candidateid; ";

            return DAO.ExecuteReader(sql, new MySqlParameter("candidateid", candidateid));
        }

        public static MySqlDataReader getCandidateLanguages(int candidateId)
        {
            string sql = "select cl.candidatelanguageid, cl.languageid, cl.spoken, cl.written,l.name as language,cl.listening,cl.reading " +
                "from candidates_languages as cl inner join languages l on cl.languageid=l.languageid " +
                " where cl.candidateid = ?candidateid " +
                "order by cl.candidatelanguageid; ";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("candidateid", candidateId));

            return reader;
        }

        public static MySqlDataReader getCandidateNationality(int candidateId)
        {
            string sql = "select cn.countryid, c.name " +
                "from candidates_nationalities as cn " +
                "join countries as c on c.countryid = cn.countryid " +
                "where cn.candidateid = ?candidateid " +
                "order by c.name; ";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("candidateid", candidateId));
            return reader;
        }

        public static MySqlDataReader getCandidateAddress(int candidateId)
        {
            string sql = "select ca.addressid, ca.addresstypeid " +
                "from candidates_addresses as ca " +
                "where ca.candidateid = ?candidateid " +
                "order by ca.addressid; ";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("candidateid", candidateId));
            return reader;
        }

        public static MySqlDataReader getCandidateAddressWithCountry(int candidateId)
        {
            string sql = "select ca.addressid, ca.addresstypeid,c.name as country " +
                " from candidates_addresses as ca inner join addresses a on ca.addressid=a.addressid inner join countries c on a.countryid=c.countryid " +
                " where ca.candidateid = ?candidateid " +
                "order by ca.addressid; ";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("candidateid", candidateId));
            return reader;
        }

        public static DataTable getCandidateFiles(int candidateId)
        {
            DataTable dt = new DataTable();
            string sql = "select cf.fileid, f.name, cf.uploaded,date_format(cf.uploaded,'%d-%b-%Y-%T') as uploaded_format, ft.name as filetype, f.size,ft.filetypeid,filename,f.deleted, cf.defaultimage," +
                " coalesce(cf.uploadsource,0) as uploadsource,cf.jobdetailid,cf.userid  " +
                "from candidates_files as cf " +
                "join files as f on f.fileid = cf.fileid " +
                "join filetypes as ft on ft.filetypeid = cf.filetypeid " +
                "where cf.candidateid = ?candidateid " +
                "order by cf.uploaded desc, cf.fileid";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("candidateid", candidateId));
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public static DataTable getCandidateNonDeletedFiles(int candidateId)
        {
            DataTable dt = new DataTable();
            string sql = "select cf.fileid, f.name, cf.uploaded,date_format(cf.uploaded,'%d-%b-%Y-%T') as uploaded_format, ft.name as filetype, f.size,ft.filetypeid,filename,f.deleted, cf.defaultimage," +
                " coalesce(cf.uploadsource,0) as uploadsource,cf.jobdetailid,cf.userid  " +
                "from candidates_files as cf " +
                "join files as f on f.fileid = cf.fileid " +
                "join filetypes as ft on ft.filetypeid = cf.filetypeid " +
                "where cf.candidateid = ?candidateid and f.deleted=0 and ft.filetypeid not in(6,17) " +
                "order by cf.uploaded desc, cf.fileid";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("candidateid", candidateId));
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public static DataTable getCandidateImageFiles(int candidateId)
        {
            DataTable dt = new DataTable();
            string sql = "select cf.fileid, f.name, cf.uploaded,date_format(cf.uploaded,'%d-%b-%Y-%T') as uploaded_format, ft.name as filetype, f.size,ft.filetypeid,filename,f.deleted, cf.defaultimage " +
                "from candidates_files as cf " +
                "join files as f on f.fileid = cf.fileid " +
                "join filetypes as ft on ft.filetypeid = cf.filetypeid " +
                "where cf.candidateid = ?candidateid and f.deleted=0 and ft.filetypeid in(6,17) " +
                "order by cf.uploaded desc, cf.fileid";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("candidateid", candidateId));
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public static DataTable getCandidateActiveFiles(int candidateId)
        {
            DataTable dt = new DataTable();
            string sql = "select cf.fileid, f.name, cf.uploaded,date_format(cf.uploaded,'%d-%b-%Y-%T') as uploaded_format, ft.name as filetype, f.size,ft.filetypeid,filename,f.deleted, cf.defaultimage," +
                " coalesce(cf.uploadsource,0) as uploadsource,cf.jobdetailid,cf.userid  " +
                "from candidates_files as cf " +
                "join files as f on f.fileid = cf.fileid " +
                "join filetypes as ft on ft.filetypeid = cf.filetypeid " +
                "where cf.candidateid = ?candidateid and f.deleted=0 " +
                "order by cf.uploaded desc, cf.fileid";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("candidateid", candidateId));
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            return dt;
        }

        public static MySqlDataReader getCandidateNotes(int candidateId)
        {
            string sql = "select cn.candidatenoteid, cn.created, cn.note, cn.userid, u.username, cn.consultantid, concat_ws(' ',c.first,c.last) as consultant,private " +
                "from candidates_notes as cn " +
                "join users as u on u.userid = cn.userid " +
                "left join consultants as c on c.consultantid = cn.consultantid " +
                "where cn.candidateid = ?candidateid " +
                "order by cn.created desc, cn.candidatenoteid;";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("candidateid", candidateId));

            return reader;
        }

        public static void updateCandidate(uint candidateid, string title, string first, string middlenames, string last, string nickname, DateTime? dob, bool nodob, string dobformat, string gender, string maritalstatus, List<uint> nationalities,
            DateTime? notavailableuntil, bool deceased, string status, List<uint> consultants, List<uint> divisions, List<uint> disciplines, List<uint> sectors, List<uint> prefered, List<uint> nonpreferred,
           List<CandidateSalaryInfo> salaryList, List<string[]> emails, List<string[]> numbers, List<string[]> addresses, List<string[]> othercontacts, List<string[]> qualifications, List<string[]> languages,
            List<string[]> softwares, List<string[]> workhistories, bool restrictedaccess, bool nomarketing, bool nojobalerts, bool nomailshots, DateTime? dod, string namesuffix, DateTime? currentlocationDate, DateTime? preferredlocationdate)
        {
            HistoryInfo historyInfo = new HistoryInfo();
            HistoryDataProvider history = new HistoryDataProvider();

            string sql;
            DataSet ds = new DataSet();
            MySqlDataReader reader = getCandidate(candidateid);
            string[] tbl = new string[18];
            ds.EnforceConstraints = false;
            ds.Load(reader, LoadOption.OverwriteChanges, tbl);
            reader.Close();
            reader.Dispose();
            //candidates
            sql = @"update candidates set
                    title = ?title,
                    first = ?first,
                    middle = ?middlenames,
                    last = ?last,
                    nickname = ?nickname,
                    dob = ?dob,
                    nodob=?nodob,
                    dobformat=?dobformat,
                    gender = ?gender,
                    notavailableuntil = ?notavailableuntil,
                    maritalstatus = ?maritalstatus,
                    deceased = ?deceased,
                    status = ?status,

                    restrictedaccess=?restrictedaccess,
                    nomarketing=?nomarketing,
                    nojobalerts=?nojobalerts,
                    nomailshots=?nomailshots,
                    dateofdeath=?dod,
                    namesuffix=?namesuffix,
                    currentlocationdate=?currentlocationdate,
                    preferredlocationdate=?preferredlocationdate
                    where candidateid = ?candidateid;";
            //MySqlParameter sqlSalaryMin = new MySqlParameter("salarymin", MySqlDbType.Decimal);
            //if (salarymin.HasValue) { sqlSalaryMin.Value = salarymin; } else { sqlSalaryMin.Value = DBNull.Value; }
            //MySqlParameter sqlSalaryMax = new MySqlParameter("salarymax", MySqlDbType.Decimal);
            //if (salarymax.HasValue) { sqlSalaryMax.Value = salarymax; } else { sqlSalaryMax.Value = DBNull.Value; }
            //MySqlParameter sqlSalaryCurrency = new MySqlParameter("salarycurrency", MySqlDbType.VarChar, 3);
            //if (salarycurrency.Length > 0) { sqlSalaryCurrency.Value = salarycurrency; } else { sqlSalaryCurrency.Value = DBNull.Value; }
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("title", title), new MySqlParameter("first", first), new MySqlParameter("middlenames", middlenames), new MySqlParameter("last", last),
                new MySqlParameter("nickname", nickname), new MySqlParameter("dob", dob), new MySqlParameter("nodob", nodob), new MySqlParameter("dobformat", dobformat), new MySqlParameter("gender", gender), new MySqlParameter("notavailableuntil", notavailableuntil), new MySqlParameter("maritalstatus", maritalstatus),
                new MySqlParameter("deceased", deceased), new MySqlParameter("status", status), new MySqlParameter("restrictedaccess", restrictedaccess), new MySqlParameter("nomarketing", nomarketing),
                new MySqlParameter("nojobalerts", nojobalerts), new MySqlParameter("nomailshots", nomailshots), new MySqlParameter("dod", dod), new MySqlParameter("namesuffix", namesuffix)
                , new MySqlParameter("currentlocationdate", currentlocationDate), new MySqlParameter("preferredlocationdate", preferredlocationdate));

            #region History

            historyInfo.UserId = GPSession.UserId;
            historyInfo.ModuleId = (int)HistoryInfo.Module.Candidate;
            historyInfo.TypeId = (int)HistoryInfo.ActionType.Edit;
            historyInfo.RecordId = candidateid;
            historyInfo.ParentRecordId = candidateid;

            historyInfo.ModifiedDate = DateTime.Now;
            historyInfo.Details = new List<HistoryDetailInfo>();

            if (ds.Tables[0].Rows[0]["first"].ToString() != first)
            {
                historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "first", OldValue = ds.Tables[0].Rows[0]["first"].ToString(), NewValue = first });
            }

            if (ds.Tables[0].Rows[0]["last"].ToString() != last)
            {
                historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "last", OldValue = ds.Tables[0].Rows[0]["last"].ToString(), NewValue = last });
            }

            if (ds.Tables[0].Rows[0]["middle"].ToString() != middlenames)
            {
                historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "middle", OldValue = ds.Tables[0].Rows[0]["middle"].ToString(), NewValue = middlenames });
            }
            if (ds.Tables[0].Rows[0]["nickname"].ToString() != nickname)
            {
                historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "nickname", OldValue = ds.Tables[0].Rows[0]["nickname"].ToString(), NewValue = nickname });
            }
            if (ds.Tables[0].Rows[0]["dob"].ToString() != (dob == null ? "" : dob.ToString()))
            {
                historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "dob", OldValue = ds.Tables[0].Rows[0]["dob"].ToString(), NewValue = dob.ToString() });
            }
            if (ds.Tables[0].Rows[0]["gender"].ToString() != gender)
            {
                historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "gender", OldValue = ds.Tables[0].Rows[0]["gender"].ToString(), NewValue = gender });
            }
            if (ds.Tables[0].Rows[0]["notavailableuntil"].ToString() != notavailableuntil.ToString())
            {
                historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "notavailableuntil", OldValue = ds.Tables[0].Rows[0]["notavailableuntil"].ToString(), NewValue = notavailableuntil.ToString() });
            }
            if (ds.Tables[0].Rows[0]["maritalstatus"].ToString() != maritalstatus)
            {
                historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "maritalstatus", OldValue = ds.Tables[0].Rows[0]["maritalstatus"].ToString(), NewValue = maritalstatus });
            }
            if (Convert.ToBoolean(ds.Tables[0].Rows[0]["deceased"]) != deceased)
            {
                historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "deceased", OldValue = ds.Tables[0].Rows[0]["deceased"].ToString(), NewValue = deceased.ToString() });
            }
            if (ds.Tables[0].Rows[0]["status"].ToString() != status)
            {
                historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "status", OldValue = ds.Tables[0].Rows[0]["status"].ToString(), NewValue = status });
            }
            //if (ds.Tables[0].Rows[0]["salarymin"].ToString() != salarymin.ToString())
            //{
            //    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "salarymin", OldValue = ds.Tables[0].Rows[0]["salarymin"].ToString(), NewValue = salarymin.ToString() });
            //}
            //if (ds.Tables[0].Rows[0]["salarymax"].ToString() != salarymax.ToString())
            //{
            //    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "salarymax", OldValue = ds.Tables[0].Rows[0]["salarymax"].ToString(), NewValue = salarymax.ToString() });
            //}
            //if (ds.Tables[0].Rows[0]["salarycurrency"].ToString() != salarycurrency)
            //{
            //    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "salarycurrency", OldValue = ds.Tables[0].Rows[0]["salarycurrency"].ToString(), NewValue = salarycurrency });
            //}

            if (Convert.ToBoolean(ds.Tables[0].Rows[0]["restrictedaccess"]) != restrictedaccess)
            {
                historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "restrictedaccess", OldValue = ds.Tables[0].Rows[0]["restrictedaccess"].ToString(), NewValue = restrictedaccess.ToString() });
            }

            if (Convert.ToBoolean(ds.Tables[0].Rows[0]["nomarketing"]) != nomarketing)
            {
                historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "nomarketing", OldValue = ds.Tables[0].Rows[0]["nomarketing"].ToString(), NewValue = nomarketing.ToString() });
            }

            if (Convert.ToBoolean(ds.Tables[0].Rows[0]["nojobalerts"]) != nojobalerts)
            {
                historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "nojobalerts", OldValue = ds.Tables[0].Rows[0]["nojobalerts"].ToString(), NewValue = nojobalerts.ToString() });
            }

            if (Convert.ToBoolean(ds.Tables[0].Rows[0]["nomailshots"]) != nomailshots)
            {
                historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "nomailshots", OldValue = ds.Tables[0].Rows[0]["nomailshots"].ToString(), NewValue = nomailshots.ToString() });
            }

            if (historyInfo.Details.Count > 0)
            {
                history.insertHistory(historyInfo);
            }

            #endregion History

            //candidate Salary
            if (salaryList != null && salaryList.Count > 0)
            {
                foreach (CandidateSalaryInfo salInfo in salaryList)
                {
                    addCandidateSalary(candidateid, salInfo.SalaryType, salInfo.MinAmount, salInfo.MaxAmount, salInfo.Frequency, salInfo.Currency, salInfo.SalaryDate);
                }
            }
            //candidates_nationalities
            StringBuilder values = new StringBuilder();
            foreach (uint countryid in nationalities)
            {
                if (values.Length > 0) values.Append(", ");
                values.Append("(?candidateid," + countryid + ")");
            }
            sql = "delete from candidates_nationalities where candidateid = ?candidateid; ";
            if (values.Length > 0) sql += "insert into candidates_nationalities (candidateid, countryid) values " + values.ToString();
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid));

            //consultants
            values.Remove(0, values.Length);
            foreach (uint consultantid in consultants)
            {
                if (values.Length > 0) values.Append(", ");
                values.Append("(?candidateid," + consultantid + ")");
            }
            sql = "delete from candidates_consultants where candidateid = ?candidateid; ";
            if (values.Length > 0) sql += "insert into candidates_consultants (candidateid, consultantid) values " + values.ToString();
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid));

            //divisions
            values.Remove(0, values.Length);
            foreach (uint divisionid in divisions)
            {
                if (values.Length > 0) values.Append(", ");
                values.Append("(?candidateid," + divisionid + ")");
            }
            sql = "delete from candidates_divisions where candidateid = ?candidateid; ";
            if (values.Length > 0) sql += "insert into candidates_divisions (candidateid, divisionid) values " + values.ToString();
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid));

            //disciplines
            values.Remove(0, values.Length);
            foreach (uint disciplineid in disciplines)
            {
                if (values.Length > 0) values.Append(", ");
                values.Append("(?candidateid," + disciplineid + ")");
            }
            sql = "delete from candidates_disciplines where candidateid = ?candidateid; ";
            if (values.Length > 0) sql += "insert into candidates_disciplines (candidateid, disciplineid) values " + values.ToString();
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid));

            //sectors
            values.Remove(0, values.Length);
            foreach (uint sectorid in sectors)
            {
                if (values.Length > 0) values.Append(", ");
                values.Append("(?candidateid," + sectorid + ")");
            }
            sql = "delete from candidates_sectors where candidateid = ?candidateid; ";
            if (values.Length > 0) sql += "insert into candidates_sectors (candidateid, sectorid) values " + values.ToString();
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid));

            //prefered & nonpreferred
            values.Remove(0, values.Length);
            foreach (uint locationid in prefered)
            {
                if (values.Length > 0) values.Append(", ");
                values.Append("(?candidateid," + locationid + ",1)");
            }
            foreach (uint locationid in nonpreferred)
            {
                if (values.Length > 0) values.Append(", ");
                values.Append("(?candidateid," + locationid + ",0)");
            }
            sql = "delete from candidates_locations where candidateid = ?candidateid;";
            if (values.Length > 0) sql += " insert into candidates_locations (candidateid, locationid, preferred) values " + values.ToString();
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid));

            //emails

            foreach (string[] email in emails)
            {
                uint emailid = Convert.ToUInt32(email[0]);
                string address = email[1];
                uint emailtypeid = Convert.ToUInt32(email[2]);
                bool defaultemail = Convert.ToBoolean(email[3]);

                updateEmail(candidateid, emailid, address, emailtypeid, defaultemail);

                DataRow[] drSeq = ds.Tables[8].Select("emailid=" + emailid);

                if (drSeq.Length > 0)
                {
                    historyInfo = new HistoryInfo();
                    historyInfo.UserId = GPSession.UserId;
                    historyInfo.ModuleId = (int)HistoryInfo.Module.candidates_emails;
                    historyInfo.TypeId = (int)HistoryInfo.ActionType.Edit;
                    historyInfo.RecordId = emailid;
                    historyInfo.ParentRecordId = candidateid;
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
                uint phonenumberid = Convert.ToUInt32(number[0]);
                string code = number[1];
                string areacode = number[2];
                string phonenumber = number[3];
                uint phonenumbertypeid = Convert.ToUInt32(number[4]);
                updatePhoneNumber(candidateid, phonenumberid, phonenumber, phonenumbertypeid, code, areacode);

                DataRow[] drSeq = ds.Tables[9].Select("phonenumberid=" + phonenumberid);

                if (drSeq.Length > 0)
                {
                    historyInfo = new HistoryInfo();
                    historyInfo.UserId = GPSession.UserId;
                    historyInfo.ModuleId = (int)HistoryInfo.Module.candidates_phonenumbers;
                    historyInfo.TypeId = (int)HistoryInfo.ActionType.Edit;
                    historyInfo.RecordId = phonenumberid;
                    historyInfo.ParentRecordId = candidateid;
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
                updateAddressType(candidateid, addressid, addresstypeid);

                DataRow[] drSeq = ds.Tables[10].Select("addressid=" + addressid);

                historyInfo = new HistoryInfo();
                historyInfo.UserId = GPSession.UserId;
                historyInfo.ModuleId = (int)HistoryInfo.Module.candidates_addresses;
                historyInfo.TypeId = (int)HistoryInfo.ActionType.Edit;
                historyInfo.RecordId = addressid;
                historyInfo.ParentRecordId = candidateid;
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

                DataRow[] drSeq = ds.Tables[11].Select("candidateothercontactid=" + othercontactid);

                if (drSeq.Length > 0)
                {
                    historyInfo = new HistoryInfo();
                    historyInfo.UserId = GPSession.UserId;
                    historyInfo.ModuleId = (int)HistoryInfo.Module.candidates_othercontacts;
                    historyInfo.TypeId = (int)HistoryInfo.ActionType.Edit;
                    historyInfo.RecordId = othercontactid;
                    historyInfo.ParentRecordId = candidateid;
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

            //qualifications
            foreach (string[] qual in qualifications)
            {
                uint candidatequalificationid = Convert.ToUInt32(qual[0]);
                DateTime obtained = Convert.ToDateTime(qual[1] + "-01-01");
                uint qualificationid = Convert.ToUInt32(qual[2]);
                uint institutionid = Convert.ToUInt32(qual[3]);
                updateQualification(candidatequalificationid, obtained, qualificationid, institutionid);

                DataRow[] drSeq = ds.Tables[12].Select("candidatequalificatonid=" + candidatequalificationid);

                if (drSeq.Length > 0)
                {
                    historyInfo = new HistoryInfo();
                    historyInfo.UserId = GPSession.UserId;
                    historyInfo.ModuleId = (int)HistoryInfo.Module.candidates_qualifications;
                    historyInfo.TypeId = (int)HistoryInfo.ActionType.Edit;
                    historyInfo.RecordId = candidatequalificationid;
                    historyInfo.ParentRecordId = candidateid;
                    historyInfo.ModifiedDate = DateTime.Now;
                    historyInfo.Details = new List<HistoryDetailInfo>();

                    if (drSeq[0]["qualificationid"].ToString() != qualificationid.ToString())
                    {
                        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "qualificationid", OldValue = drSeq[0]["qualificationid"].ToString(), NewValue = qualificationid.ToString() });
                    }
                    if (drSeq[0]["institutionid"].ToString() != institutionid.ToString())
                    {
                        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "institutionid", OldValue = drSeq[0]["institutionid"].ToString(), NewValue = institutionid.ToString() });
                    }
                    if (drSeq[0]["obtained"].ToString() != obtained.ToString())
                    {
                        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "obtained", OldValue = drSeq[0]["obtained"].ToString(), NewValue = obtained.ToString() });
                    }

                    if (historyInfo.Details.Count > 0)
                    {
                        history.insertHistory(historyInfo);
                    }
                }
            }

            //languages
            foreach (string[] language in languages)
            {
                uint candidatelanguageid = Convert.ToUInt32(language[0]);
                uint languageid = Convert.ToUInt32(language[1]);
                uint spoken = Convert.ToUInt32(language[2]);
                uint written = Convert.ToUInt32(language[4]);

                uint? listening = null;
                if (!string.IsNullOrEmpty(language[3]))
                    listening = Convert.ToUInt32(language[3]);
                uint? reading = null;
                if (!string.IsNullOrEmpty(language[5]))
                    reading = Convert.ToUInt32(language[5]);
                updateLanguage(candidatelanguageid, languageid, spoken, written, listening, reading);

                DataRow[] drSeq = ds.Tables[13].Select("candidatelanguageid=" + candidatelanguageid);

                if (drSeq.Length > 0)
                {
                    historyInfo = new HistoryInfo();
                    historyInfo.UserId = GPSession.UserId;
                    historyInfo.ModuleId = (int)HistoryInfo.Module.candidates_languages;
                    historyInfo.TypeId = (int)HistoryInfo.ActionType.Edit;
                    historyInfo.RecordId = candidatelanguageid;
                    historyInfo.ParentRecordId = candidateid;
                    historyInfo.ModifiedDate = DateTime.Now;
                    historyInfo.Details = new List<HistoryDetailInfo>();

                    if (drSeq[0]["languageid"].ToString() != languageid.ToString())
                    {
                        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "languageid", OldValue = drSeq[0]["languageid"].ToString(), NewValue = languageid.ToString() });
                    }
                    if (drSeq[0]["spoken"].ToString() != spoken.ToString())
                    {
                        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "spoken", OldValue = drSeq[0]["spoken"].ToString(), NewValue = spoken.ToString() });
                    }
                    if (drSeq[0]["written"].ToString() != written.ToString())
                    {
                        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "written", OldValue = drSeq[0]["written"].ToString(), NewValue = written.ToString() });
                    }
                    if (drSeq[0]["listening"].ToString() != listening.ToString())
                    {
                        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "listening", OldValue = drSeq[0]["listening"].ToString(), NewValue = written.ToString() });
                    }
                    if (drSeq[0]["reading"].ToString() != reading.ToString())
                    {
                        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "reading", OldValue = drSeq[0]["reading"].ToString(), NewValue = written.ToString() });
                    }
                    if (historyInfo.Details.Count > 0)
                    {
                        history.insertHistory(historyInfo);
                    }
                }
            }

            //softwares
            foreach (string[] software in softwares)
            {
                uint candidatesoftwreid = Convert.ToUInt32(software[0]);
                uint softwareid = Convert.ToUInt32(software[1]);
                updateSoftware(candidatesoftwreid, softwareid);

                DataRow[] drSeq = ds.Tables[14].Select("candidatesoftwareid=" + candidatesoftwreid);
                if (drSeq.Length > 0)
                {
                    historyInfo = new HistoryInfo();
                    historyInfo.UserId = GPSession.UserId;
                    historyInfo.ModuleId = (int)HistoryInfo.Module.candidates_softwares;
                    historyInfo.TypeId = (int)HistoryInfo.ActionType.Edit;
                    historyInfo.RecordId = candidatesoftwreid;
                    historyInfo.ParentRecordId = candidateid;
                    historyInfo.ModifiedDate = DateTime.Now;
                    historyInfo.Details = new List<HistoryDetailInfo>();

                    if (drSeq[0]["softwareid"].ToString() != softwareid.ToString())
                    {
                        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "softwareid", OldValue = drSeq[0]["softwareid"].ToString(), NewValue = softwareid.ToString() });
                    }

                    if (historyInfo.Details.Count > 0)
                    {
                        history.insertHistory(historyInfo);
                    }
                }
            }

            CVFormatDataProvider.updateCVFormatByCandidateId(candidateid, languages, nationalities);
        }

        public static void addCandidateSalary(uint candidateId, int type, int fromSalary, int toSalary, int frequency, string currency, DateTime? salarydate, int? sourceid = null, int sourcetype = 4)
        {
            HistoryInfo historyInfo = new HistoryInfo();
            HistoryDataProvider history = new HistoryDataProvider();
            historyInfo = new HistoryInfo();
            historyInfo.UserId = GPSession.UserId;
            if (type == 1)
                historyInfo.ModuleId = (int)HistoryInfo.Module.Candidate_CurrentSalary;
            else
                historyInfo.ModuleId = (int)HistoryInfo.Module.Candidate_ExpectedSalary;
            historyInfo.TypeId = (int)HistoryInfo.ActionType.Edit;
            historyInfo.RecordId = candidateId;
            historyInfo.ParentRecordId = candidateId;
            historyInfo.ModifiedDate = DateTime.Now;
            historyInfo.Details = new List<HistoryDetailInfo>();

            string sql = "select * from candidates_salary where candidateid=?candidateid and salarytype=?salarytype";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("candidateid", candidateId), new MySqlParameter("salarytype", type));
            if (dr.HasRows)
            {
                dr.Read();
                sql = "update candidates_salary set fromamount=?fromamount,toamount=?toamount,frequency=?frequency,currency=?currency,salarydate=?salarydate,sourcetype=?sourcetype,sourceid=?sourceid,userid=?userid where candidateid=?candidateid and salarytype=?salarytype";
                if (DAO.getString(dr, "fromamount") != fromSalary.ToString())
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "From salary", OldValue = DAO.getString(dr, "currency") + " " + DAO.getString(dr, "fromamount"), NewValue = currency + " " + fromSalary.ToString() });
                }
                if (DAO.getString(dr, "toamount") != toSalary.ToString())
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "To salary", OldValue = DAO.getString(dr, "currency") + " " + DAO.getString(dr, "toamount"), NewValue = currency + " " + toSalary.ToString() });
                }
                if (DAO.getString(dr, "frequency") != frequency.ToString())
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "Frequency", OldValue = DAO.getString(dr, "frequency"), NewValue = frequency.ToString() });
                }
                if (DAO.getString(dr, "currency") != currency)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "Currency", OldValue = DAO.getString(dr, "currency"), NewValue = currency });
                }
                DateTime? salDate = DAO.getDateTime(dr, "salarydate");
                string strDate = string.Empty;
                if (salDate != null)
                    strDate = Convert.ToDateTime(salDate).ToString("dd/MM/yyyy");
                string strSaldate = string.Empty;
                if (salarydate != null)
                    strSaldate = Convert.ToDateTime(salarydate).ToString("dd/MM/yyyy");

                if (strDate != strSaldate)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "Salary date", OldValue = strDate, NewValue = strSaldate });
                }
            }
            else
                sql = "insert into candidates_salary (candidateid,fromamount,toamount,frequency,currency,salarytype,salarydate,sourcetype,sourceid,userid) values (?candidateid,?fromamount,?toamount,?frequency,?currency,?salarytype,?salarydate,?sourcetype,?sourceid,?userid)";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateId), new MySqlParameter("fromamount", fromSalary), new MySqlParameter("toamount", toSalary), new MySqlParameter("frequency", frequency),
                new MySqlParameter("currency", currency), new MySqlParameter("salarytype", type), new MySqlParameter("salarydate", salarydate), new MySqlParameter("sourcetype", sourcetype), new MySqlParameter("sourceid", sourceid), new MySqlParameter("userid", GPSession.UserId));

            if (historyInfo.Details.Count > 0)
            {
                history.insertHistory(historyInfo);
            }
            dr.Close();
            dr.Dispose();
        }

        public static uint addEmail(uint candidateid, string email, uint emailtypeid)
        {
            return addEmail(candidateid, email, emailtypeid, false);
        }

        public static uint addEmail(uint candidateid, string email, uint emailtypeid, bool defaultemail)
        {
            uint emailid = EmailDataProvider.insertEmail(email);

            string sql = "insert into candidates_emails (candidateid, emailid, emailtypeid, defaultemail) values (?candidateid, ?emailid, ?emailtypeid, ?defaultemail);select last_insert_id()";
            uint id = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("emailid", emailid), new MySqlParameter("emailtypeid", emailtypeid), new MySqlParameter("defaultemail", defaultemail)));

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.candidates_emails;
            info.TypeId = (int)HistoryInfo.ActionType.Add;
            info.RecordId = emailid;
            info.ParentRecordId = candidateid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "All", NewValue = "emailid:" + emailid + " ,email:" + email + " ,candidateid:" + candidateid + " ,emailtypeid:" + emailtypeid + " ,defaultemail:" + defaultemail });

            history.insertHistory(info);

            return emailid;
        }

        public static void updateEmail(uint candidateid, uint emailid, string email, uint emailtypeid, bool defaultemail)
        {
            EmailDataProvider.updateEmail(emailid, email);
            string sql = "update candidates_emails set " +
                "emailtypeid = ?emailtypeid, " +
                "defaultemail = ?defaultemail " +
                "where candidateid = ?candidateid " +
                "and emailid = ?emailid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("emailid", emailid), new MySqlParameter("emailtypeid", emailtypeid), new MySqlParameter("defaultemail", defaultemail));
        }

        public static void removeEmail(uint candidateid, uint emailid)
        {
            string email = EmailDataProvider.getEmail(emailid);
            string sql = "delete from candidates_emails where candidateid = ?candidateid and emailid = ?emailid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("emailid", emailid));
            EmailDataProvider.removeEmail(emailid);

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.candidates_emails;
            info.TypeId = (int)HistoryInfo.ActionType.Delete;
            info.RecordId = emailid;
            info.ParentRecordId = candidateid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "Delete", NewValue = email });

            history.insertHistory(info);
        }

        public static void updateDefaultEmail(uint candidateid, uint emailid, string fullname)
        {
            string email = EmailDataProvider.getEmail(emailid);
            string sql = "update candidates_emails set defaultemail=0 where candidateid = ?candidateid; update candidates_emails set defaultemail=1 where candidateid = ?candidateid and emailid = ?emailid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("emailid", emailid));

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.candidates_emails;
            info.TypeId = (int)HistoryInfo.ActionType.Edit;
            info.RecordId = emailid;
            info.ParentRecordId = candidateid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "default", NewValue = email + " has been set as the default email address for " + fullname + " (Candidate ID " + candidateid + ")." });

            history.insertHistory(info);
        }

        public static void updateAlertEmail(uint candidateid, uint emailid, string fullname)
        {
            int alertemailId = JobAlertDataProvider.getJobAlertEmailId(candidateid);
            string email = EmailDataProvider.getEmail(emailid);
            string sql = "update job_alert set emailid=?emailid where candidateid = ?candidateid;";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("emailid", emailid));

            if (alertemailId != emailid)
            {
                HistoryDataProvider history = new HistoryDataProvider();
                HistoryInfo info = new HistoryInfo();
                info.UserId = GPSession.UserId;
                info.ModuleId = (int)HistoryInfo.Module.candidates_emails;
                info.TypeId = (int)HistoryInfo.ActionType.Edit;
                info.RecordId = emailid;
                info.ParentRecordId = candidateid;
                info.ModifiedDate = DateTime.Now;
                info.Details = new List<HistoryDetailInfo>();
                info.Details.Add(new HistoryDetailInfo { ColumnName = "jobalert", NewValue = email + " has been set as the Job Alerts email address for " + fullname + " (Candidate ID " + candidateid + ")." });

                history.insertHistory(info);
            }
        }

        public static void addPhoneNumber(uint candidateid, string number, uint phonenumbertypeid, string countrycode, string areacode)
        {
            uint phonenumberid = PhoneNumberDataProvider.insertPhoneNumber(number, countrycode, areacode);
            string sql = "insert into candidates_phonenumbers (candidateid, phonenumberid, phonenumbertypeid) values (?candidateid, ?phonenumberid, ?phonenumbertypeid);select last_insert_id()";
            uint id = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("phonenumberid", phonenumberid), new MySqlParameter("phonenumbertypeid", phonenumbertypeid)));

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.candidates_phonenumbers;
            info.TypeId = (int)HistoryInfo.ActionType.Add;
            info.RecordId = phonenumberid;
            info.ParentRecordId = candidateid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "All", NewValue = "phonenumberid:" + phonenumberid + " ,number:" + number + " ,candidateid:" + candidateid + " ,phonenumbertypeid:" + phonenumbertypeid });

            history.insertHistory(info);
        }

        public static void updatePhoneNumber(uint candidateid, uint phonenumberid, string number, uint phonenumbertypeid, string countrycode, string areacode)
        {
            PhoneNumberDataProvider.updatePhoneNumber(phonenumberid, number, countrycode, areacode);
            string sql = "update candidates_phonenumbers set " +
                "phonenumbertypeid = ?phonenumbertypeid " +
                "where candidateid = ?candidateid " +
                "and phonenumberid = ?phonenumberid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("phonenumberid", phonenumberid), new MySqlParameter("phonenumbertypeid", phonenumbertypeid));
        }

        public static void removePhoneNumber(uint candidateid, uint phonenumberid)
        {
            string number = PhoneNumberDataProvider.getPhoneNumber(phonenumberid);
            string sql = "delete from candidates_phonenumbers where candidateid = ?candidateid and phonenumberid = ?phonenumberid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("phonenumberid", phonenumberid));
            PhoneNumberDataProvider.removePhoneNumber(phonenumberid);

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.candidates_phonenumbers;
            info.TypeId = (int)HistoryInfo.ActionType.Delete;
            info.RecordId = phonenumberid;
            info.ParentRecordId = candidateid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "Delete", NewValue = number });

            history.insertHistory(info);
        }

        public static void addAddress(uint candidateid, uint addresstypeid, uint addressid)
        {
            string sql = "insert into candidates_addresses (candidateid, addressid, addresstypeid) values (?candidateid, ?addressid, ?addresstypeid)";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("addressid", addressid), new MySqlParameter("addresstypeid", addresstypeid));

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.candidates_addresses;
            info.TypeId = (int)HistoryInfo.ActionType.Add;
            info.RecordId = addressid;
            info.ParentRecordId = candidateid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "All", NewValue = "addressid:" + addressid + " ,candidateid:" + candidateid + ", addresstypeId:" + addresstypeid });

            history.insertHistory(info);
        }

        public static void updateAddressType(uint candidateid, uint addressid, uint addresstypeid)
        {
            string sql = "update candidates_addresses set addresstypeid = ?addresstypeid where candidateid = ?candidateid and addressid = ?addressid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("addressid", addressid), new MySqlParameter("addresstypeid", addresstypeid));
        }

        public static void removeAddress(uint candidateid, uint addressid)
        {
            string address = AddressDataProvider.getAddressHTML(addressid);
            string sql = "delete from candidates_addresses where candidateid = ?candidateid and addressid = ?addressid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("addressid", addressid));
            AddressDataProvider.removeAddress(addressid);

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.candidates_addresses;
            info.TypeId = (int)HistoryInfo.ActionType.Delete;
            info.RecordId = addressid;
            info.ParentRecordId = candidateid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "Delete", NewValue = address });

            history.insertHistory(info);
        }

        public static void addOtherContact(uint candidateid, string details, uint alternativecontactid)
        {
            string sql = "insert into candidates_othercontacts (candidateid, alternativecontactid, details) values (?candidateid, ?alternativecontactid, ?details);select last_insert_id()";
            uint id = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("alternativecontactid", alternativecontactid), new MySqlParameter("details", details)));

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.candidates_othercontacts;
            info.TypeId = (int)HistoryInfo.ActionType.Add;
            info.RecordId = id;
            info.ParentRecordId = candidateid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "All", NewValue = "alternativecontactid:" + alternativecontactid + " ,candidateid:" + candidateid + " ,details:" + details });

            history.insertHistory(info);
        }

        public static void updateOtherContact(uint candidateothercontactid, string details, uint alternativecontactid)
        {
            string sql = "update candidates_othercontacts set " +
                "alternativecontactid = ?alternativecontactid, " +
                "details = ?details " +
                "where candidateothercontactid = ?candidateothercontactid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateothercontactid", candidateothercontactid), new MySqlParameter("alternativecontactid", alternativecontactid), new MySqlParameter("details", details));
        }

        public static void removeOtherContact(uint candidateid, uint candidateothercontactid)
        {
            string sql = "select details from candidates_othercontacts where candidateothercontactid = ?candidateothercontactid";
            string detail = (string)DAO.ExecuteScalar(sql, new MySqlParameter("candidateothercontactid", candidateothercontactid));
            sql = "delete from candidates_othercontacts where candidateothercontactid = ?candidateothercontactid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateothercontactid", candidateothercontactid));

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.candidates_othercontacts;
            info.TypeId = (int)HistoryInfo.ActionType.Delete;
            info.RecordId = candidateothercontactid;
            info.ParentRecordId = candidateid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "Delete", NewValue = detail });

            history.insertHistory(info);
        }

        public static void addQualification(uint candidateid, DateTime obtained, uint qualificationid, uint institutionid)
        {
            string sql = "insert into candidates_qualifications (candidateid, obtained, qualificationid, institutionid) values (?candidateid, ?obtained, ?qualificationid, ?institutionid);select last_insert_id()";
            uint id = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("obtained", obtained), new MySqlParameter("qualificationid", qualificationid), new MySqlParameter("institutionid", institutionid)));

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.candidates_qualifications;
            info.TypeId = (int)HistoryInfo.ActionType.Add;
            info.RecordId = id;
            info.ParentRecordId = candidateid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "All", NewValue = "qualificationid:" + qualificationid + " ,candidateid:" + candidateid + " ,obtained:" + obtained + " ,institutionid" + institutionid });

            history.insertHistory(info);
        }

        public static void updateQualification(uint candidatequalificatonid, DateTime obtained, uint qualificationid, uint institutionid)
        {
            string sql = "update candidates_qualifications set " +
                "obtained = ?obtained, " +
                "qualificationid = ?qualificationid, " +
                "institutionid = ?institutionid " +
                "where candidatequalificatonid = ?candidatequalificatonid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidatequalificatonid", candidatequalificatonid), new MySqlParameter("obtained", obtained), new MySqlParameter("qualificationid", qualificationid), new MySqlParameter("institutionid", institutionid));
        }

        public static void removeQualification(uint candidateid, uint candidatequalificatonid)
        {
            string sql = "select name from candidates_qualifications cq inner join qualifications q on cq.qualificationid=q.qualificationid where candidatequalificatonid = ?candidatequalificatonid";
            string qualification = (string)DAO.ExecuteScalar(sql, new MySqlParameter("candidatequalificatonid", candidatequalificatonid));

            sql = "delete from candidates_qualifications where candidatequalificatonid = ?candidatequalificatonid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidatequalificatonid", candidatequalificatonid));

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.candidates_qualifications;
            info.TypeId = (int)HistoryInfo.ActionType.Delete;
            info.RecordId = candidatequalificatonid;
            info.ParentRecordId = candidateid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "Delete", NewValue = qualification });

            history.insertHistory(info);
        }

        public static void addLanguage(uint candidateid, uint languageid, uint spoken, uint written, uint listening, uint reading)
        {
            string sql = "insert into candidates_languages (candidateid, languageid, spoken, written,listening,reading) values (?candidateid, ?languageid, ?spoken, ?written,?listening,?reading);select last_insert_id()";
            uint id = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("languageid", languageid), new MySqlParameter("spoken", spoken), new MySqlParameter("written", written),
                new MySqlParameter("listening", listening), new MySqlParameter("reading", reading)));

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.candidates_languages;
            info.TypeId = (int)HistoryInfo.ActionType.Add;
            info.RecordId = id;
            info.ParentRecordId = candidateid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "All", NewValue = "languageid:" + languageid + " ,candidateid:" + candidateid + " ,spoken:" + spoken + " ,written" + written });

            history.insertHistory(info);

            CVFormatDataProvider.updateCVFormatLanguageByCandidateId(candidateid, languageid, spoken, written, listening, reading);
        }

        public static void updateLanguage(uint candidatelanguageid, uint languageid, uint spoken, uint written, uint? listening, uint? reading)
        {
            string sql = "update candidates_languages set " +
                "languageid = ?languageid, " +
                "spoken = ?spoken, " +
                "written = ?written, " +
                "listening = ?listening, " +
                "reading =?reading " +
                "where candidatelanguageid = ?candidatelanguageid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidatelanguageid", candidatelanguageid), new MySqlParameter("languageid", languageid), new MySqlParameter("spoken", spoken), new MySqlParameter("written", written),
                new MySqlParameter("listening", listening), new MySqlParameter("reading", reading));
        }

        public static void removeLanguage(uint candidateid, uint candidatelanguageid)
        {
            string sql = "select name,l.languageid from candidates_languages cl inner join languages l on cl.languageid=l.languageid where candidatelanguageid = ?candidatelanguageid";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("candidatelanguageid", candidatelanguageid));
            reader.Read();
            string language = DAO.getString(reader, "name");
            int languageid = Convert.ToInt32(DAO.getString(reader, "languageid"));

            reader.Close();
            reader.Dispose();

            sql = "delete from candidates_languages where candidatelanguageid = ?candidatelanguageid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidatelanguageid", candidatelanguageid));

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.candidates_languages;
            info.TypeId = (int)HistoryInfo.ActionType.Delete;
            info.RecordId = candidatelanguageid;
            info.ParentRecordId = candidateid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "Delete", NewValue = language });

            history.insertHistory(info);

            CVFormatDataProvider.removeCVFormatLanguageByCandidateId(candidateid, languageid);
        }

        public static void removeLanguage(int candidateid)
        {
            string sql = "delete from candidates_languages where candidateid = ?candidateid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid));
        }

        public static void addLanguagefromCVFormat(int candidateid, int languageid, int spoken, int written, int listening, int reading)
        {
            string sql = "insert into candidates_languages (candidateid, languageid, spoken, written,listening,reading) values (?candidateid, ?languageid, ?spoken, ?written,?listening,?reading);select last_insert_id()";
            uint id = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("languageid", languageid), new MySqlParameter("spoken", spoken), new MySqlParameter("written", written),
                new MySqlParameter("listening", listening), new MySqlParameter("reading", reading)));
        }

        public static void addSoftware(uint candidateid, uint softwareid)
        {
            string sql = "insert into candidates_softwares (candidateid, softwareid) values (?candidateid, ?softwareid);select last_insert_id()";
            uint id = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("softwareid", softwareid)));

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.candidates_softwares;
            info.TypeId = (int)HistoryInfo.ActionType.Add;
            info.RecordId = id;
            info.ParentRecordId = candidateid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "All", NewValue = "softwareid:" + softwareid + " ,candidateid:" + candidateid });

            history.insertHistory(info);
        }

        public static void updateSoftware(uint candidatesoftwareid, uint softwareid)
        {
            string sql = "update candidates_softwares set " +
                "softwareid = ?softwareid " +
                "where candidatesoftwareid = ?candidatesoftwareid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidatesoftwareid", candidatesoftwareid), new MySqlParameter("softwareid", softwareid));
        }

        public static void removeSoftware(uint candidateid, uint candidatesoftwareid)
        {
            string sql = "select name from candidates_softwares cs inner join softwares s on cs.softwareid=s.softwareid where candidatesoftwareid = ?candidatesoftwareid";
            string software = (string)DAO.ExecuteScalar(sql, new MySqlParameter("candidatesoftwareid", candidatesoftwareid));
            sql = "delete from candidates_softwares where candidatesoftwareid = ?candidatesoftwareid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidatesoftwareid", candidatesoftwareid));

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.candidates_softwares;
            info.TypeId = (int)HistoryInfo.ActionType.Delete;
            info.RecordId = candidatesoftwareid;
            info.ParentRecordId = candidateid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "Delete", NewValue = software });

            history.insertHistory(info);
        }

        public static void addWorkHistory(uint candidateid, DateTime? from, string from_display, DateTime? to, string to_display, string employer, uint? locationid, int? locationtype, uint? industryid, uint? positionid, decimal? salary, string salarycurrency, int? employerid)
        {
            if (!string.IsNullOrEmpty(employer))
            {
                if (employerid == 0)
                {
                    employerid = CommonDataProvider.existEmployer(employer, 0);
                    if (employerid == 0)
                    {
                        employerid = CommonDataProvider.addEmployer(employer);
                    }
                    //    MySqlDataReader drEmp = CommonDataProvider.SearchEmployer(employer);
                    //    if (drEmp.HasRows)
                    //    {
                    //        drEmp.Read();
                    //        employerid = Convert.ToInt32(DAO.getString(drEmp, "employerid"));
                    //    }
                    //    else
                    //        employerid = CommonDataProvider.addEmployer(employer);
                    //    drEmp.Close();
                    //    drEmp.Dispose();
                }
            }

            string sql = "insert into candidates_workhistories (candidateid, `from`, from_display, `to`, to_display, employer, locationid, industryid, positionid, salary, salarycurrency,locationtype,employerid) " +
                         " values (?candidateid, ?from, ?from_display, ?to, ?to_display, ?employer, ?locationid, ?industryid, ?positionid, ?salary, ?salarycurrency,?locationtype,?employerid);select last_insert_id()";
            MySqlParameter sqlSalary = new MySqlParameter("salary", MySqlDbType.Decimal);
            if (salary == null)
            {
                sqlSalary.Value = DBNull.Value;
            }
            else
            {
                sqlSalary.Value = salary;
            }
            uint id = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("from", from), new MySqlParameter("from_display", from_display), new MySqlParameter("to", to),
                new MySqlParameter("to_display", to_display), new MySqlParameter("employer", employer), new MySqlParameter("locationid", locationid), new MySqlParameter("industryid", industryid), new MySqlParameter("positionid", positionid),
                sqlSalary, new MySqlParameter("salarycurrency", salarycurrency), new MySqlParameter("locationtype", locationtype), new MySqlParameter("employerid", employerid)));

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.candidates_workhistories;
            info.TypeId = (int)HistoryInfo.ActionType.Add;
            info.RecordId = id;
            info.ParentRecordId = candidateid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo
            {
                ColumnName = "All",
                NewValue = "candidateid:" + candidateid + " ,from:" + from_display.Replace(",", "-") + " ,to:" + (to != null ? to_display.Replace(",", "-") : "Present") + " ,employer:" + employer + " ,locationid:" + locationid + " " + locationtype
                 + " ,industryid:" + industryid + " ,positionid:" + positionid
            });

            history.insertHistory(info);
        }

        public static void updateWorkHistory(uint candidateworkhistoryid, DateTime? from, string from_display, DateTime? to, string to_display, string employer, uint? locationid, int? locationtype, uint? industryid, uint? positionid, decimal? salary, string salarycurrency, int employerid)
        {
            if (!string.IsNullOrEmpty(employer))
            {
                if (employerid == 0)
                {
                    employerid = CommonDataProvider.existEmployer(employer, 0);
                    if (employerid == 0)
                    {
                        employerid = CommonDataProvider.addEmployer(employer);
                    }
                    //    MySqlDataReader drEmp = CommonDataProvider.SearchEmployer(employer);
                    //    if (drEmp.HasRows)
                    //    {
                    //        drEmp.Read();
                    //        employerid = Convert.ToInt32(DAO.getString(drEmp, "employerid"));
                    //    }
                    //    else
                    //        employerid = CommonDataProvider.addEmployer(employer);
                    //    drEmp.Close();
                    //    drEmp.Dispose();
                }
            }

            MySqlDataReader dr = getWorkHistoryById(candidateworkhistoryid);

            string sql = "update candidates_workhistories set " +
                "`from` = ?from, " +
                "from_display = ?from_display, " +
                "`to` = ?to, " +
                "to_display = ?to_display, " +
                "employer = ?employer, " +
                "locationid = ?locationid, " +
                "industryid = ?industryid, " +
                "positionid = ?positionid, " +
                "salary = ?salary, " +
                "salarycurrency = ?salarycurrency, " +
                "locationtype=?locationtype " +
                ", employerid=?employerid " +
                "where candidateworkhistoryid = ?candidateworkhistoryid";
            MySqlParameter sqlSalary = new MySqlParameter("salary", MySqlDbType.Decimal);
            if (salary == null)
            {
                sqlSalary.Value = DBNull.Value;
            }
            else
            {
                sqlSalary.Value = salary;
            }
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateworkhistoryid", candidateworkhistoryid), new MySqlParameter("from", from), new MySqlParameter("from_display", from_display), new MySqlParameter("to", to),
                new MySqlParameter("to_display", to_display), new MySqlParameter("employer", employer), new MySqlParameter("locationid", locationid), new MySqlParameter("industryid", industryid), new MySqlParameter("positionid", positionid),
                sqlSalary, new MySqlParameter("salarycurrency", salarycurrency), new MySqlParameter("locationtype", locationtype), new MySqlParameter("employerid", employerid));

            if (dr.HasRows)
            {
                dr.Read();
                HistoryDataProvider history = new HistoryDataProvider();
                HistoryInfo historyInfo = new HistoryInfo();

                historyInfo = new HistoryInfo();
                historyInfo.UserId = GPSession.UserId;
                historyInfo.ModuleId = (int)HistoryInfo.Module.candidates_workhistories;
                historyInfo.TypeId = (int)HistoryInfo.ActionType.Edit;
                historyInfo.RecordId = candidateworkhistoryid;
                historyInfo.ParentRecordId = Convert.ToUInt32(DAO.getString(dr, "candidateid"));
                historyInfo.ModifiedDate = DateTime.Now;
                historyInfo.Details = new List<HistoryDetailInfo>();

                if (DAO.getString(dr, "from_display") != from_display)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "from", OldValue = DAO.getString(dr, "from_display").Replace(",", "-"), NewValue = from_display.Replace(",", "-") });
                }
                if (DAO.getString(dr, "to_display") != to_display)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "to", OldValue = DAO.getString(dr, "to_display").Replace(",", "-"), NewValue = (to != null ? to_display.Replace(",", "-") : "Present") });
                }
                if (DAO.getString(dr, "employerid") != employerid.ToString())
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "employer", OldValue = DAO.getString(dr, "employer"), NewValue = employer.ToString() });
                }
                if (DAO.getUInt(dr, "locationid") != locationid)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "locationid", OldValue = DAO.getString(dr, "locationid") + " " + DAO.getString(dr, "locationtype"), NewValue = locationid.ToString() + " " + locationtype });
                }
                if (DAO.getUInt(dr, "industryid") != industryid)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "industryid", OldValue = DAO.getString(dr, "industryid"), NewValue = industryid.ToString() });
                }
                if (DAO.getUInt(dr, "positionid") != positionid)
                {
                    historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "positionid", OldValue = DAO.getString(dr, "positionid"), NewValue = positionid.ToString() });
                }

                if (historyInfo.Details.Count > 0)
                {
                    history.insertHistory(historyInfo);
                }
            }

            dr.Close();
            dr.Dispose();
        }

        public static void updateWorkHistoryEmployer(int employerId, int dupEmployerId)
        {
            string sql = "update candidates_workhistories set employerid=?employerid where employerid=?dupEmployerid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("employerid", employerId), new MySqlParameter("dupEmployerid", dupEmployerId));
        }

        public static void removeWorkHistory(uint candidateid, uint candidateworkhisotryid)
        {
            string sql = "select coalesce(employername,'') as employer from candidates_workhistories cw left join employer e on cw.employerid=e.employerid where candidateworkhistoryid = ?candidateworkhisotryid";
            string employer = (string)DAO.ExecuteScalar(sql, new MySqlParameter("candidateworkhisotryid", candidateworkhisotryid));
            sql = "delete from candidates_workhistories where candidateworkhistoryid = ?candidateworkhisotryid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateworkhisotryid", candidateworkhisotryid));

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.candidates_workhistories;
            info.TypeId = (int)HistoryInfo.ActionType.Delete;
            info.RecordId = candidateworkhisotryid;
            info.ParentRecordId = candidateid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo { ColumnName = "Delete", NewValue = employer });

            history.insertHistory(info);
        }

        public static bool ExistWorkHistory(int historyId, uint candidateid, DateTime? from, string from_display, DateTime? to, string to_display, uint? locationid, int? locationtype, uint? industryid, uint? positionid, int? employerid)
        {
            bool exist = false;
            string sql = "Select candidateid from candidates_workhistories where candidateworkhistoryid!=?historyid and candidateid=?candidateid and (`from` = ?frm or ?frm is null)  and " +
                " from_display = ?from_display and " +
                " (`to` = ?to or ?to is null) and " +
                " to_display = ?to_display and " +
                " (locationid = ?locationid or ?locationid is null) and " +
                " (industryid = ?industryid or ?industryid is null) and " +
                " (positionid = ?positionid or ?positionid is null) and " +
                " (locationtype=?locationtype or ?locationtype is null) and " +
                " employerid=?employerid ";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("historyid", historyId), new MySqlParameter("candidateid", candidateid), new MySqlParameter("frm", from), new MySqlParameter("from_display", from_display),
                new MySqlParameter("to", to), new MySqlParameter("to_display", to_display), new MySqlParameter("locationid", locationid), new MySqlParameter("industryid", industryid), new MySqlParameter("positionid", positionid),
                 new MySqlParameter("locationtype", locationtype), new MySqlParameter("employerid", employerid));
            if (dr.HasRows)
                exist = true;
            dr.Close();
            dr.Dispose();
            return exist;
        }

        public static MySqlDataReader getWorkHistory(uint candidateid)
        {
            string sql = "select cwh.candidateworkhistoryid, cwh.from, cwh.from_display, cwh.to, cwh.to_display, cwh.employer, cwh.locationid, cwh.industryid, cwh.positionid, cwh.salary, cwh.salarycurrency,concat('UNIT ',i.groupcode,' ',i.title) as isco " +
                " ,concat('CLASS ',r.code,' ',r.description) as isicrev ,case when cwh.locationtype=1 then vl.name else case when cwh.locationtype=2 then concat(vl.name,' > ',vl.locationname) else case when cwh.locationtype=3 then  " +
                " concat(vl.name,' > ',vl.locationname,' > ',vl.sublocation) else concat(vl.name,' > ',vl.locationname,' > ',vl.sublocation,' > ',vl.subsublocation) end end end as location,cwh.employerid,e.employername " +
                "from candidates_workhistories as cwh left join isco08 i on cwh.positionid=i.isco08id left join isicrev4 r on cwh.industryid=r.isicrev4id " +
                " left join v_locations vl on (cwh.locationid=vl.countryid and cwh.locationtype=1 ) or (cwh.locationid=vl.locationid and cwh.locationtype=2) or (cwh.locationid=vl.sublocationid and cwh.locationtype=3 ) " +
                " or (cwh.locationid=vl.subsublocationid  and cwh.locationtype=4 ) " +
                " left join employer e on cwh.employerid=e.employerid " +
                //" left join countries c on cwh.locationid=c.countryid and cwh.locationtype=1 " +
                //"left join locations l on cwh.locationid=l.locationid and cwh.locationtype=2 left join locationsub s on cwh.locationid=s.sublocationid and cwh.locationtype=3 left join locationsub_subs ss on cwh.locationid=ss.subsublocationid " +
                //" and cwh.locationtype=4 left join countries c1 on c1.countryid=l.countryid left join locations l1 on l1.locationid=s.locationid and l1.countryid=c1.countryid left join locationsub s1 on s1.locationid=l1.locationid " +
                //" and ss.sublocationid=s1.sublocationid "+
                " where cwh.candidateid = ?candidateid group by cwh.candidateworkhistoryid order by cwh.from, cwh.to; ";

            return DAO.ExecuteReader(sql, new MySqlParameter("candidateid", candidateid));
        }

        public static MySqlDataReader getWorkHistoryById(uint workhistoryid)
        {
            string sql = "select cwh.candidateworkhistoryid,cwh.candidateid, cwh.from, cwh.from_display, cwh.to, cwh.to_display, cwh.employer, cwh.locationid, cwh.industryid, cwh.positionid, cwh.salary, cwh.salarycurrency,i.title as isco,r.description as isicrev " +
                ",case when cwh.locationtype=1 then c.name else case when cwh.locationtype=2 then l.name else case when cwh.locationtype=3 then s.sublocation else ss.name end end end as location,cwh.locationtype,cwh.employerid,e.employername " +
                "from candidates_workhistories as cwh left join isco08 i on cwh.positionid=i.isco08id left join isicrev4 r on cwh.industryid=r.isicrev4id left join countries c on cwh.locationid=c.countryid and cwh.locationtype=1 " +
                "left join locations l on cwh.locationid=l.locationid and cwh.locationtype=2 left join locationsub s on cwh.locationid=s.sublocationid and cwh.locationtype=3 left join locationsub_subs ss on cwh.locationid=ss.subsublocationid and cwh.locationtype=4 " +
                " left join employer e on cwh.employerid=e.employerid where cwh.candidateworkhistoryid = ?workhistoryid " +
                "order by cwh.from, cwh.to; ";

            return DAO.ExecuteReader(sql, new MySqlParameter("workhistoryid", workhistoryid));
        }

        public static MySqlDataReader getWorkhistoryEmployer(string keyword)
        {
            string sql = "select distinct employer from candidates_workhistories where employer like concat('%',?keyword,'%')";
            return DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword));
        }

        public static uint addNote(uint candidateid, uint userid, string note, uint? consultantid, bool isprivate, bool isdefault = false)
        {
            string sql = "insert into candidates_notes (candidateid, created, note, userid, consultantid,private,auto) values (?candidateid, utc_timestamp(), ?note, ?userid, ?consultantid,?private,?isdefault);select last_insert_id()";
            MySqlParameter sqlOnBehalfOf = new MySqlParameter("consultantid", MySqlDbType.UInt32);
            if (consultantid == null)
            {
                sqlOnBehalfOf.Value = DBNull.Value;
            }
            else
            {
                sqlOnBehalfOf.Value = consultantid;
            }
            uint id = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("note", note), new MySqlParameter("userid", userid), sqlOnBehalfOf, new MySqlParameter("private", isprivate), new MySqlParameter("?isdefault", isdefault)));

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo info = new HistoryInfo();
            info.UserId = GPSession.UserId;
            info.ModuleId = (int)HistoryInfo.Module.candidates_notes;
            info.TypeId = (int)HistoryInfo.ActionType.Add;
            info.RecordId = id;
            info.ParentRecordId = candidateid;
            info.ModifiedDate = DateTime.Now;
            info.Details = new List<HistoryDetailInfo>();
            info.Details.Add(new HistoryDetailInfo
            {
                ColumnName = "All",
                NewValue = "candidateid:" + candidateid + " ,note:" + note + " ,userid:" + userid
            });

            history.insertHistory(info);

            return id;
        }

        public static MySqlDataReader fetchFileExtension(uint filetypeid)
        {
            string sql = "select fileextension, coalesce(filesize,0) as filesize from filetypes where filetypeid = ?filetypeid";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("filetypeid", filetypeid));
            return reader;
        }

        public static bool checkDuplicateFile(uint candidateid, uint filetype, string filename, int filesize)
        {
            string sql = "select c.candidateid, f.name, f.size from candidates_files as c join files as f  on f.fileid = c.fileid where c.candidateid = ?candidateid and f.name = ?name and f.size = ?size and filetypeid=?filetypeid";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("name", filename), new MySqlParameter("size", filesize), new MySqlParameter("filetypeid", filetype));
            if (reader.HasRows)
                return true;
            else
                return false;
        }

        public static int addFile(uint candidateid, uint filetypeid, string filename, Stream file, int uploadsource, int? jobId = null, uint? userId = null)
        {
            try
            {
                MySqlParameter sqlContent = new MySqlParameter("content", MySqlDbType.Text);
                switch (filename.Substring(filename.Length - 4, 4))
                {
                    case ".pdf":
                        sqlContent.Value = MySqlHelper.EscapeString(FileDataProvider.parsePdf(file));
                        break;

                    case ".doc":
                        try
                        {
                            sqlContent.Value = MySqlHelper.EscapeString(FileDataProvider.parseDoc(file));
                        }
                        catch (DIaLOGIKa.b2xtranslator.StructuredStorage.Common.MagicNumberException mnex)
                        {
                            try
                            {
                                file.Position = 0;
                                filename = filename.Substring(0, filename.Length - 4) + ".rtf";
                                sqlContent.Value = MySqlHelper.EscapeString(FileDataProvider.parseRtf(file));
                            }
                            catch (ArgumentException ae)
                            {
                                file.Position = 0;
                                filename = filename.Substring(0, filename.Length - 4) + ".docx";
                                sqlContent.Value = MySqlHelper.EscapeString(FileDataProvider.parseDocx(file));
                            }
                        }
                        break;

                    case ".rtf":
                        sqlContent.Value = MySqlHelper.EscapeString(FileDataProvider.parseRtf(file));
                        break;

                    case "docx":
                        sqlContent.Value = MySqlHelper.EscapeString(FileDataProvider.parseDocx(file));
                        break;

                    case ".txt":
                        sqlContent.Value = MySqlHelper.EscapeString(FileDataProvider.parseTxt(file));
                        break;

                    case ".eml":
                        sqlContent.Value = MySqlHelper.EscapeString(FileDataProvider.parseTxt(file));
                        break;

                    case "html":
                    case ".htm":
                        sqlContent.Value = MySqlHelper.EscapeString(FileDataProvider.parseTxt(file));
                        break;

                    default:
                        sqlContent.Value = DBNull.Value;
                        break;
                }
                uint fileid = FileDataProvider.insertFile(file, filename);
                string sql = "insert into candidates_files (candidateid, fileid, uploaded, filetypeid, content,uploadsource,jobdetailid,userid) values (?candidateid, ?fileid, utc_timestamp(), ?filetypeid, ?content,?uploadsource,?jobdetailid,?userid);" +
                    "select last_insert_id()";
                uint id = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("fileid", fileid), new MySqlParameter("filetypeid", filetypeid), sqlContent,
                    new MySqlParameter("uploadsource", uploadsource), new MySqlParameter("jobdetailid", jobId), new MySqlParameter("userid", userId)));

                HistoryDataProvider history = new HistoryDataProvider();
                HistoryInfo info = new HistoryInfo();
                info.UserId = GPSession.UserId;
                info.ModuleId = (int)HistoryInfo.Module.candidates_files;
                info.TypeId = (int)HistoryInfo.ActionType.Add;
                info.RecordId = id;
                info.ParentRecordId = candidateid;
                info.ModifiedDate = DateTime.Now;
                info.Details = new List<HistoryDetailInfo>();
                info.Details.Add(new HistoryDetailInfo
                {
                    ColumnName = "All",
                    NewValue = "candidateid:" + candidateid + " ,fileid:" + fileid + " ,filetypeid:" + filetypeid
                });

                history.insertHistory(info);

                return Convert.ToInt32(fileid);
            }
            catch (Exception ex)
            {
                addFileFailedNotificatoin(candidateid, filename, ex.Message, jobId);
                throw ex;
            }
        }

        private static void addFileFailedNotificatoin(uint candidateid, string filename, string errorMsg, int? jobid)
        {
            string firstname = string.Empty;
            string middlename = string.Empty;
            string lastname = string.Empty;
            string email = string.Empty;

            MySqlDataReader drCandidate = CandidateDataProvider.getOnlyCandidate(Convert.ToInt32(candidateid));
            if (drCandidate.HasRows)
            {
                drCandidate.Read();
                firstname = DAO.getString(drCandidate, "first");
                middlename = DAO.getString(drCandidate, "middle");
                lastname = DAO.getString(drCandidate, "last");
            }
            drCandidate.Close();
            drCandidate.Dispose();
            DataTable dtEmail = CandidateDataProvider.getCandidateEmail(Convert.ToInt32(candidateid));
            if (dtEmail.Rows.Count > 0)
            {
                email = dtEmail.Rows[0]["email"].ToString();
            }
            string referenceNo = string.Empty;
            string jobTitle = string.Empty;
            if (jobid != null)
            {
                MySqlDataReader drJob = JobDetailDataProvider.getJobById(Convert.ToUInt32(jobid));
                if (drJob.HasRows)
                {
                    drJob.Read();
                    referenceNo = DAO.getString(drJob, "ReferenceNo");
                    jobTitle = DAO.getString(drJob, "title");
                }
            }
            string body = "A job application was accompanied by a file or files which GP failed to import.<br/><table><tr><td> Job Reference Code:</td><td>" + referenceNo + "</td><tr><td> Job Title:</td><td> " + jobTitle + "</td></tr>" +
            "<tr><td>Candidate name:</td> <td>" + firstname + " " + middlename + " " + lastname.ToUpper() + " (" + candidateid + ") </td></tr><tr><td> Candidate ID:</td><td>  " + candidateid + "</td></tr><tr>  Candidate email: </td><td>" + email + " </td></tr>" +
            "<tr><td> Date/time of application:</td> <td>" + DateTime.Now.ToString("dd-MMM-yyyy  hh:mm:ss tt") + " UTC 0  </td></tr><tr><td>GP error code/message:</td> <td> " + errorMsg + "</td></tr></table> Regards,<br/> GP <br/>--------------------";

            MailMessage message = new MailMessage
            {
                From = new MailAddress(ConfigurationSettings.AppSettings["fromemail"].ToString()),
                Subject = "Bad file - " + firstname + " " + lastname.ToUpper() + " (" + candidateid + ") for " + referenceNo,
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add(ConfigurationSettings.AppSettings["fileExceptionAdmin"].ToString());

            SmtpClient sc = new SmtpClient();
            sc.Host = ConfigurationSettings.AppSettings["smtpHost"].ToString();
            string smtpUser = ConfigurationSettings.AppSettings["smtpUserName"].ToString();
            string smtpPwd = ConfigurationSettings.AppSettings["smtpPassword"].ToString();
            sc.Credentials = new System.Net.NetworkCredential(smtpUser, smtpPwd);
            sc.Send(message);
        }

        public static void addDiscipline(uint candidateid, uint disciplineid)
        {
            string sql = "select count(candidateid) as cnt from candidates_disciplines where candidateid=?candidateid and disciplineid=?disciplineid";
            uint cnt = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("disciplineid", disciplineid)));
            if (cnt == 0)
            {
                sql = "insert into candidates_disciplines (candidateid, disciplineid) values (?candidateid, ?disciplineid);select last_insert_id()";
                uint id = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("disciplineid", disciplineid)));

                HistoryDataProvider history = new HistoryDataProvider();
                HistoryInfo info = new HistoryInfo();
                info.UserId = GPSession.UserId;
                info.ModuleId = (int)HistoryInfo.Module.candidates_disciplines;
                info.TypeId = (int)HistoryInfo.ActionType.Add;
                info.RecordId = id;
                info.ParentRecordId = candidateid;
                info.ModifiedDate = DateTime.Now;
                info.Details = new List<HistoryDetailInfo>();
                info.Details.Add(new HistoryDetailInfo
                {
                    ColumnName = "All",
                    NewValue = "candidateid:" + candidateid + " ,disciplineid:" + disciplineid
                });

                history.insertHistory(info);
            }
        }

        public static void ClearExistingUploads()
        {
            //string path = System.Web.HttpContext.Current.Server.MapPath("~");
            string path = ConfigurationManager.AppSettings.Get("filePath");

            //string sql = "select min(fileid) as fileid from candidates_files where filetypeid = 3";
            //uint? fileid = (uint?)DAO.ExecuteScalar(sql);
            uint fileid = 24;

            if (fileid > 0)
            {
                string sql = "select * from files where fileid >= ?fileid";
                using (MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("fileid", fileid)))
                {
                    while (dr.Read())
                    {
                        //File.Delete(path + "\\..\\files\\" + dr["filename"] + ".zip");
                        string filename = dr["filename"].ToString();
                        path = path + filename.Substring(0, 2) + "\\" + filename.Substring(2, 2) + "\\" + filename.Substring(4, 2) + "\\";
                        File.Delete(path + filename + ".zip");
                    }
                }

                sql = "delete from candidates where candidateid in ( select distinct candidateid from candidates_files where fileid >= ?fileid )";
                DAO.ExecuteNonQuery(sql, new MySqlParameter("fileid", fileid));
                sql = "delete from files where fileid >= ?fileid";
                DAO.ExecuteNonQuery(sql, new MySqlParameter("fileid", fileid));
                sql = "delete from candidates_files where fileid >= ?fileid";
                DAO.ExecuteNonQuery(sql, new MySqlParameter("fileid", fileid));
            }
        }

        public static void updateCandidateBirthDate(int candidateId, DateTime birthDate, bool nodob, string dobformat)
        {
            string sql = "update candidates set dob=?dob,nodob=?nodob,dobformat=?dobformat where candidateid=?candidateid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateId), new MySqlParameter("dob", birthDate), new MySqlParameter("nodob", nodob), new MySqlParameter("dobformat", dobformat));
        }

        public static void updateCandidateMaritalStatus(int candidateId, string maritalstatus)
        {
            string sql = "update candidates set maritalstatus=?maritalstatus where candidateid=?candidateid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateId), new MySqlParameter("maritalstatus", maritalstatus));
        }

        public static void addNationfromCVFormat(int candidateid, List<int> countryList)
        {
            StringBuilder values = new StringBuilder();
            foreach (int countryid in countryList)
            {
                if (values.Length > 0) values.Append(", ");
                values.Append("(?candidateid," + countryid + ")");
            }
            string sql = "delete from candidates_nationalities where candidateid = ?candidateid; ";
            sql += "insert into candidates_nationalities (candidateid, countryid) values " + values.ToString();
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid));
        }

        public static void updateDefaultImage(int candidateid, int fileid)
        {
            string query = "update candidates_files set defaultimage = 0 where candidateid = ?candidateid";
            DAO.ExecuteNonQuery(query, new MySqlParameter("candidateid", candidateid));

            string sql = "update candidates_files set defaultimage = 1 where candidateid = ?candidateid and fileid = ?fileid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("fileid", fileid));
        }

        public static void addCurrentlocation(uint candidateid, int locationid, int locationtype)
        {
            string sql = "insert into candidates_currentlocations (candidateid,locationid,locationtype) values (?candidateid,?locationid,?locationtype)";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("locationid", locationid), new MySqlParameter("locationtype", locationtype));
        }

        public static void deleteCurrentlocation(uint candidateid, int locationid, int locationtype)
        {
            string sql = "delete from candidates_currentlocations where candidateid=?candidateid and locationid=?locationid and locationtype=?locationtype";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("locationid", locationid), new MySqlParameter("locationtype", locationtype));
        }

        public static MySqlDataReader getCurrentlocations(uint candidateId)
        {
            string sql = "select cl.candidateid,cl.locationtype,cl.locationid, concat('Anywhere') as location,concat('0',',','0,0,0,0') as locationids  from candidates_currentlocations cl  where cl.locationid=0 and cl.locationtype=1 and cl.candidateid=?candidateid  " +
      " union select cl.candidateid,cl.locationtype,cl.locationid,concat(c.name) as location,concat(CAST(c.countryid as char),',','0,0,0,0') as locationids from countries c join candidates_currentlocations cl on c.countryid=cl.locationid and cl.locationtype=1 and cl.locationid!=0 where cl.candidateid=?candidateid  " +
       " union select cl.candidateid,cl.locationtype,cl.locationid,concat(c.name,' > ',l.name) as location,concat(CAST(c.countryid as char),',',cast(l.locationid as char),',', '0,0,0') as locationids  from countries c inner join locations l on l.countryid=c.countryid " +
        " join candidates_currentlocations cl on l.locationid=cl.locationid and cl.locationtype=2 where  cl.candidateid=?candidateid " +
         " union select cl.candidateid,cl.locationtype,cl.locationid,concat(c.name,' > ',l.name,' > ',s.sublocation) as location,concat(CAST(c.countryid as char),',',cast(l.locationid as char),',', cast(s.sublocationid as char),',','0,0') as locationids  from countries c inner join locations l on l.countryid=c.countryid " +
         " inner join locationsub s on s.locationid=l.locationid join candidates_currentlocations cl on s.sublocationid=cl.locationid and cl.locationtype=3 where cl.candidateid=?candidateid  " +
         " union select cl.candidateid,cl.locationtype,cl.locationid,concat(c.name,' > ',l.name,' > ',s.sublocation,' > ',ss.name) as location,concat(CAST(c.countryid as char),',',cast(l.locationid as char),',', cast(s.sublocationid as char),',',cast(ss.subsublocationid as char),',0' ) as locationids from countries c inner " +
          " join locations l on l.countryid=c.countryid  inner join locationsub s on s.locationid=l.locationid " +
          " inner join locationsub_subs ss on ss.sublocationid=s.sublocationid join candidates_currentlocations cl on ss.subsublocationid=cl.locationid and cl.locationtype=4 where cl.candidateid=?candidateid " +
            " union  select cl.candidateid,cl.locationtype,cl.locationid,concat(groupname) as location,concat('0',',','0,0,0',',',location_groupid) as locationids from candidates_currentlocations cl join location_group g on g.location_groupid=cl.locationid and cl.locationtype=5 " +
             " where  cl.candidateid=?candidateid; ";

            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("candidateid", candidateId));
            return dr;
        }

        public static void addPreferredlocation(uint candidateid, int locationid, int locationtype)
        {
            string sql = "insert into candidates_preferedlocations (candidateid,locationid,locationtype) values (?candidateid,?locationid,?locationtype)";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("locationid", locationid), new MySqlParameter("locationtype", locationtype));
        }

        public static void deletePreferredlocation(uint candidateid, int locationid, int locationtype)
        {
            string sql = "delete from candidates_preferedlocations where candidateid=?candidateid and locationid=?locationid and locationtype=?locationtype";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("locationid", locationid), new MySqlParameter("locationtype", locationtype));
        }

        public static MySqlDataReader getPreferredlocations(uint candidateId)
        {
            string sql = "select cl.candidateid,cl.locationtype,cl.locationid, concat('Anywhere') as location,concat('0',',','0,0,0,0') as locationids  from candidates_preferedlocations cl  where cl.locationid=0 and cl.locationtype=1 and cl.candidateid=?candidateid  " +
      " union select cl.candidateid,cl.locationtype,cl.locationid,concat(c.name) as location,concat(CAST(c.countryid as char),',','0,0,0,0') as locationids from countries c join candidates_preferedlocations cl on c.countryid=cl.locationid and cl.locationtype=1 and cl.locationid!=0 where cl.candidateid=?candidateid  " +
       " union select cl.candidateid,cl.locationtype,cl.locationid,concat(c.name,' > ',l.name) as location,concat(CAST(c.countryid as char),',',cast(l.locationid as char),',', '0,0,0') as locationids  from countries c inner join locations l on l.countryid=c.countryid " +
        " join candidates_preferedlocations cl on l.locationid=cl.locationid and cl.locationtype=2 where  cl.candidateid=?candidateid " +
         " union select cl.candidateid,cl.locationtype,cl.locationid,concat(c.name,' > ',l.name,' > ',s.sublocation) as location,concat(CAST(c.countryid as char),',',cast(l.locationid as char),',', cast(s.sublocationid as char),',','0,0') as locationids  from countries c inner join locations l on l.countryid=c.countryid " +
         " inner join locationsub s on s.locationid=l.locationid join candidates_preferedlocations cl on s.sublocationid=cl.locationid and cl.locationtype=3 where cl.candidateid=?candidateid  " +
         " union select cl.candidateid,cl.locationtype,cl.locationid,concat(c.name,' > ',l.name,' > ',s.sublocation,' > ',ss.name) as location,concat(CAST(c.countryid as char),',',cast(l.locationid as char),',', cast(s.sublocationid as char),',',cast(ss.subsublocationid as char),',0' ) as locationids from countries c inner " +
          " join locations l on l.countryid=c.countryid  inner join locationsub s on s.locationid=l.locationid " +
          " inner join locationsub_subs ss on ss.sublocationid=s.sublocationid join candidates_preferedlocations cl on ss.subsublocationid=cl.locationid and cl.locationtype=4 where cl.candidateid=?candidateid " +
            " union  select cl.candidateid,cl.locationtype,cl.locationid,concat(groupname) as location,concat('0',',','0,0,0',',',location_groupid) as locationids from candidates_preferedlocations cl join location_group g on g.location_groupid=cl.locationid and cl.locationtype=5 " +
             " where  cl.candidateid=?candidateid; ";

            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("candidateid", candidateId));
            return dr;
        }

        public static MySqlDataReader getCandidateByEmployerId(int employerId)
        {
            string sql = "select c.candidateid,c.first,replace(c.middle,'|',' ') as middle,c.last from candidates c join candidates_workhistories cw on c.candidateid=cw.candidateid where cw.employerid=?employerId group by c.candidateid";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("employerId", employerId));
            return dr;
        }

        public static MySqlDataReader getCandidateSearchSpecific(int candidateid, string firstname, string lastname)
        {
            //string sql = "select candidateid,title,first,middle,last from candidates where (candidateid=?candidateid or ?candidateid=0) and (first=?first or ?first='') and (last=?last or ?last='')";

            string sql = "select c.candidateid,title,first,middle,last,c.restrictedaccess,coalesce(a.job_alertid,0) as job_alertid  from candidates c left join job_alert a on c.candidateid=a.candidateid " +
                " where (c.candidateid=?candidateid or ?candidateid=-1) and (match(first) against(?first in boolean mode) or ?first='' or first=?first) and (match(last) against(?last in boolean mode) or ?last='' or last=?last)";

            return DAO.ExecuteReader(sql, new MySqlParameter("candidateid", candidateid), new MySqlParameter("first", firstname), new MySqlParameter("last", lastname));
        }
    }
}