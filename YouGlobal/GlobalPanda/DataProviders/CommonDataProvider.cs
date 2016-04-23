using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
/// Summary description for MenuItemDataProvider
/// </summary>
namespace GlobalPanda.DataProviders
{
    public class CommonDataProvider
    {
        public static MySqlDataReader listSalutations()
        {
            string sql = "select s.title " +
                         "from salutations as s " +
                         "order by s.title";
            MySqlDataReader drS = DAO.ExecuteReader(sql);
            return drS;
        }

        public static bool isDuplicateSalutation(string title, uint salutationid)
        {
            string sql = "select count(salutationid) as `exists` from salutations where title = ?title and salutationid != ?salutationid";
            uint exists = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("title", title), new MySqlParameter("salutationid", salutationid)));
            return (exists > 0);
        }

        public static MySqlDataReader listCurrencies()
        {
            //string sql = "select currencyid, concat(name,' (',currencyid,')') as name " +
            string sql = "select currencyid, currencyid as name " +
                                   "from currencies as c " +
                                   "order by c.name";
            MySqlDataReader drS = DAO.ExecuteReader(sql);
            return drS;
        }

        public static MySqlDataReader listFrequency()
        {
            string sql = "select frequencyId, frequency as name " +
                          "from frequency as c " +
                          "order by c.sort";
            MySqlDataReader drS = DAO.ExecuteReader(sql);
            return drS;
        }

        public static string getSalaryFrequencyName(int id)
        {
            string sql = "select frequency from frequency where frequencyId=?id;";
            string name = DAO.ExecuteScalar(sql, new MySqlParameter("id", id)).ToString();
            return name;
        }

        public static bool isDuplicateCurrencyId(string currencyid)
        {
            string sql = "select count(currencyid) as `exists` from currencies where currencyid = ?currencyid";
            uint exists = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("currencyid", currencyid)));
            return (exists > 0);
        }

        public static bool isDuplicateCurrencyName(string name, string currencyid)
        {
            string sql = "select count(currencyid) as `exists` from currencies where name = ?name and currencyid != ?currencyid";
            uint exists = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("name", name), new MySqlParameter("currencyid", currencyid)));
            return (exists > 0);
        }

        public static MySqlDataReader listDivisions(uint tenantid)
        {
            string sql = "select d.divisionid, d.name " +
                         "from divisions as d " +
                         "order by d.name";
            return DAO.ExecuteReader(sql);
        }

        public static bool isDuplicateDivision(string name, uint divisionid)
        {
            string sql = "select count(divisionid) as `exists` from divisions where name = ?name and divisionid != ?divisionid";
            uint exists = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("name", name), new MySqlParameter("divisionid", divisionid)));
            return (exists > 0);
        }

        public static MySqlDataReader listDisciplines(uint tenantid)
        {
            string sql = "select d.disciplineid, d.name " +
                         "from disciplines as d " +
                         "order by d.name";
            return DAO.ExecuteReader(sql);
        }

        public static bool isDuplicateDiscipline(string name, uint disciplineid)
        {
            string sql = "select count(disciplineid) as `exists` from disciplines where name = ?name and disciplineid != ?disciplineid";
            uint exists = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("name", name), new MySqlParameter("disciplineid", disciplineid)));
            return (exists > 0);
        }

        public static MySqlDataReader listSecotrs(uint tenantid)
        {
            string sql = "select s.sectorid, s.name " +
                         "from sectors as s " +
                         "order by s.name";
            return DAO.ExecuteReader(sql);
        }

        public static bool isDuplicateSector(string name, uint sectorid)
        {
            string sql = "select count(sectorid) as `exists` from sectors where name = ?name and sectorid != ?sectorid";
            uint exists = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("name", name), new MySqlParameter("sectorid", sectorid)));
            return (exists > 0);
        }

        public static MySqlDataReader listQualifications(uint tenantid)
        {
            string sql = "select q.qualificationid, q.name " +
                         "from qualifications as q " +
                         "order by q.name";
            return DAO.ExecuteReader(sql);
        }

        public static bool isDuplicateQualification(string name, uint qualificationid)
        {
            string sql = "select count(qualificationid) as `exists` from qualifications where name = ?name and qualificationid != ?qualificationid";
            uint exists = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("name", name), new MySqlParameter("qualificationid", qualificationid)));
            return (exists > 0);
        }

        public static MySqlDataReader listInstitutions(uint tenantid)
        {
            string sql = "select i.institutionid, i.name as institution, c.name as country " +
                         "from institutions as i " +
                         "join countries as c on c.countryid = i.countryid " +
                         "order by c.name, i.name";
            return DAO.ExecuteReader(sql);
        }

        public static bool isDuplicateInstitution(string name, uint institutionid)
        {
            string sql = "select count(institutionid) as `exists` from institutions where name = ?name and institutionid != ?institutionid";
            uint exists = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("name", name), new MySqlParameter("institutionid", institutionid)));
            return (exists > 0);
        }

        public static MySqlDataReader listLanguages()
        {
            string sql = "select l.languageid, l.name " +
                         "from languages as l " +
                         "order by l.name";
            return DAO.ExecuteReader(sql);
        }

        public static MySqlDataReader listRelationships()
        {
            string sql = "select relationshipsid, name " +
                         "from Relationships  " +
                         "order by name";
            return DAO.ExecuteReader(sql);
        }

        public static bool isDuplicateLanguage(string name, uint Languageid)
        {
            string sql = "select count(Languageid) as `exists` from Languages where name = ?name and Languageid != ?Languageid";
            uint exists = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("name", name), new MySqlParameter("Languageid", Languageid)));
            return (exists > 0);
        }

        public static bool isDuplicateRelationship(string name, uint relationshipsid)
        {
            string sql = "select count(relationshipsid) as `exists` from Relationships where name = ?name and relationshipsid != ?relationshipsid";
            uint exists = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("name", name), new MySqlParameter("relationshipsid", relationshipsid)));
            return (exists > 0);
        }

        public static MySqlDataReader listSoftwares(uint tenantid)
        {
            string sql = "select s.softwareid, s.name " +
                         "from softwares as s " +
                         "order by s.name";
            return DAO.ExecuteReader(sql);
        }

        public static bool isDuplicateSoftware(string name, uint softwareid)
        {
            string sql = "select count(softwareid) as `exists` from softwares where name = ?name and softwareid != ?softwareid";
            uint exists = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("name", name), new MySqlParameter("softwareid", softwareid)));
            return (exists > 0);
        }

        public static Dictionary<uint, string> listLanguageProficiencies()
        {
            Dictionary<uint, string> prof = new Dictionary<uint, string>();
            prof.Add(0, "None");
            prof.Add(1, "Low");
            prof.Add(5, "Average");
            prof.Add(9, "Fluent");
            prof.Add(10, "Native");
            prof.Add(11, "Unknown");
            return prof;
        }

        public static MySqlDataReader listIndustries(uint tenantid)
        {
            string sql = "select i.industryid, i.name " +
                         "from industries as i " +
                         "order by i.name";
            return DAO.ExecuteReader(sql);
        }

        public static bool isDuplicateIndustry(string name, uint industryid)
        {
            string sql = "select count(industryid) as `exists` from industries where name = ?name and industryid != ?industryid";
            uint exists = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("name", name), new MySqlParameter("industryid", industryid)));
            return (exists > 0);
        }

        public static MySqlDataReader listPositions(uint tenantid)
        {
            string sql = "select p.positionid, p.name " +
                         "from positions as p " +
                         "order by p.name";
            return DAO.ExecuteReader(sql);
        }

        public static bool isDuplicatePosition(string name, uint positionid)
        {
            string sql = "select count(positionid) as `exists` from positions where name = ?name and positionid != ?positionid";
            uint exists = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("name", name), new MySqlParameter("positionid", positionid)));
            return (exists > 0);
        }

        public static MySqlDataReader listAlternativeContacts()
        {
            string sql = "select ac.alternativecontactid, ac.name " +
                "from alternativecontacts as ac " +
                "order by name";
            return DAO.ExecuteReader(sql);
        }

        public static string getAlternativeContact(int id)
        {
            string sql = "select ac.name " +
                "from alternativecontacts as ac where  alternativecontactid=?id";
            return (string)DAO.ExecuteScalar(sql, new MySqlParameter("id", id));
        }

        public static bool isDuplicateAlternativeContact(string name, uint alternativecontactid)
        {
            string sql = "select count(alternativecontactid) as `exists` from alternativecontacts where name = ?name and alternativecontactid != ?alternativecontactid";
            uint exists = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("name", name), new MySqlParameter("alternativecontactid", alternativecontactid)));
            return (exists > 0);
        }

        public static void insertErrorLog(System.Exception ex)
        {
            if (ex != null)
            {
                string sql = "insert into errorlogs (description, details, timestamp, userid) values (?description, ?details, utc_timestamp, ?userid)";
                DAO.ExecuteNonQuery(sql, new MySqlParameter("description", ex.Message), new MySqlParameter("details", ex.StackTrace), new MySqlParameter("userid", GPSession.UserId));
            }
        }

        public static MySqlDataReader searchCountryCode(string keyword)
        {
            string sql = "select concat(name,' (', phonecode,')') as Code from countries where phonecode is not null and (phonecode like concat(?keyword,'%') or " +
                         "name like concat(?keyword,'%') ) order by name asc;";
            return DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword));
        }

        public static MySqlDataReader listAllOpenJobs(string keyword)
        {
            string sql = "select jobdetailId,referenceno from jobdetail where ReferenceNo like concat('%',?keyword,'%') and status in (2,6)";
            return DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword));
        }

        public static MySqlDataReader listLocations(string keyword)
        {
            string sql = "select l.locationid, name,countryid,code ,case when (coalesce(jl.locationid,0) =0 and coalesce( al.locationid,0) =0) then 0 else 1 end as jobid,level " +
                         " from locations l left join jobs_locations jl on jl.locationid=l.locationid and jl.locationtype=2  left join v_distictAlertLocations al on al.locationid=l.locationid and al.locationtype=2 " +
                         "  where name like concat('%',?keyword,'%') group by l.locationid, name,countryid,code ";
            return DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword));
        }

        public static MySqlDataReader listLocationsByCountryId(string keyword, int countryId)
        {
            string sql = "select locationid, name,countryid,code from locations where name like concat('%',?keyword,'%') and countryid=?countryid";
            return DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword), new MySqlParameter("countryid", countryId));
        }

        public static MySqlDataReader listCountries(string keyword)
        {
            string sql = "select countryid,name,code,phonecode,coalesce(jl.jobdetailid,0) as jobid from countries c left join jobs_locations jl on jl.locationid=c.countryid and jl.locationtype=1 where name like concat('%',?keyword,'%') group by countryid,name,code,phonecode";
            return DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword));
        }

        public static MySqlDataReader listSubLocations()
        {
            string sql = "select sublocationid,sublocation,ls.locationid,coalesce(jl.jobdetailid,0) as jobid from locationsub ls left join jobs_locations jl on jl.locationid=ls.sublocationid and jl.locationtype=3 " +
                " left join v_distictAlertLocations al on al.locationid=ls.sublocationid and al.locationtype=3";
            MySqlDataReader reader = DAO.ExecuteReader(sql);
            return reader;
        }

        public static MySqlDataReader listSubLocationsByLocationId(string keyword, int locationId)
        {
            string sql = "select sublocationid,sublocation,locationid from locationsub where sublocation like concat('%',?keyword,'%') and locationid=?locationid";
            MySqlDataReader reader = DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword), new MySqlParameter("locationid", locationId));
            return reader;
        }

        public static MySqlDataReader listSubsubLocations()
        {
            string sql = "select subsublocationid,sublocationid,name,coalesce(jl.jobdetailid,0) as jobid from locationsub_subs ls left join jobs_locations jl on jl.locationid=ls.subsublocationid and jl.locationtype=4 " +
               " left join v_distictAlertLocations al on al.locationid=ls.subsublocationid and al.locationtype=4";
            MySqlDataReader reader = DAO.ExecuteReader(sql);
            return reader;
        }

        public static string RemoveHtml(string source)
        {
            return Regex.Replace(source, "<.*?>", string.Empty);
        }

        public static MySqlDataReader SearchEmployer(string keyword, string order = "e.employerid asc")
        {
            string sql = "select e.employerid,employername,coalesce( cw.employerid>0,0) as used from employer e left join candidates_workhistories cw on e.employerid=cw.employerid " +
                " where employername like concat('%',?keyword,'%') group by e.employerid order by " + order;
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword));
            return dr;
        }

        public static int addEmployer(string name)
        {
            int id = 0;
            string sql = "insert into employer(employername) values(?name);select last_insert_id()";
            id = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("name", name)));
            return id;
        }

        public static int existEmployer(string name, int employerId)
        {
            int id = 0;
            string sql = "select employerid from employer where employerid !=?employerid and employername =?name";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("employerid", employerId), new MySqlParameter("name", name));
            if (dr.HasRows)
            {
                dr.Read();
                id = Convert.ToInt32(DAO.getString(dr, "employerid"));
            }
            dr.Close();
            dr.Dispose();
            return id;
        }

        public static void updateEmployer(int id, string name)
        {
            string sql = "update employer set employername=?name where employerid=?id";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("name", name), new MySqlParameter("id", id));
        }

        public static void deleteEmployer(int id)
        {
            string sql = "delete from employer where employerid=?id";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("id", id));
        }

        public static string getEmployerName(int id)
        {
            string sql = "select employername from employer where employerid=?id";
            string name = (string)DAO.ExecuteScalar(sql, new MySqlParameter("id", id));
            return name;
        }
    }
}