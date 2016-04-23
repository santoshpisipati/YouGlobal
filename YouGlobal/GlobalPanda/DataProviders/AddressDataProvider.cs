using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Summary description for EmailDataProvider
/// </summary>

namespace GlobalPanda.DataProviders
{
    public class AddressDataProvider
    {
        public enum AddressType { Business = 1, Postal = 2, Home = 3 };

        public static uint insertAddress(uint countryid, string address, uint columnspan, Dictionary<uint, string> values)
        {
            string sql = "insert into addresses (countryid) values (?countryid); select last_insert_id()";
            uint addressid = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("countryid", countryid)));

            uint line = 1;
            string name;
            address = address.Replace("\r\n", "\n");
            foreach (string addressline in address.Split("\n".ToCharArray()))
            {
                name = "Address Line " + line.ToString();
                sql = "insert into addresslines (addressid, name, line, position, columnspan, value) " +
                    "values (?addressid, ?name, ?line, 1, ?columnspan, ?value)";
                DAO.ExecuteNonQuery(sql, new MySqlParameter("addressid", addressid), new MySqlParameter("line", line), new MySqlParameter("name", name), new MySqlParameter("columnspan", columnspan), new MySqlParameter("value", addressline));
                line++;
            }

            line--;

            foreach (uint addressformatlineid in values.Keys)
            {
                sql = "insert into addresslines (addressid, name, line, position, columnspan, value) " +
                    "select ?addressid, afl.name, afl.line + ?line, afl.position, afl.columnspan, ?value " +
                    "from addressformatlines as afl " +
                    "where addressformatlineid = ?addressformatlineid";
                DAO.ExecuteNonQuery(sql, new MySqlParameter("addressid", addressid), new MySqlParameter("line", line), new MySqlParameter("addressformatlineid", addressformatlineid), new MySqlParameter("value", values[addressformatlineid]));
            }

            return addressid;
        }

        public static void updateAddress(uint addressid, string address, uint columnspan, Dictionary<uint, string> values)
        {
            string sql = "delete from addresslines where addressid = ?addressid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("addressid", addressid));

            uint line = 1;
            string name;
            address = address.Replace("\r\n", "\n");
            foreach (string addressline in address.Split("\n".ToCharArray()))
            {
                name = "Address Line " + line.ToString();
                sql = "insert into addresslines (addressid, name, line, position, columnspan, value) " +
                    "values (?addressid, ?name, ?line, 1, ?columnspan, ?value)";
                DAO.ExecuteNonQuery(sql, new MySqlParameter("addressid", addressid), new MySqlParameter("line", line), new MySqlParameter("name", name), new MySqlParameter("columnspan", columnspan), new MySqlParameter("value", addressline));
                line++;
            }

            line--;

            foreach (uint addressformatlineid in values.Keys)
            {
                sql = "insert into addresslines (addressid, name, line, position, columnspan, value) " +
                    "select ?addressid, afl.name, afl.line + ?line, afl.position, afl.columnspan, ?value " +
                    "from addressformatlines as afl " +
                    "where addressformatlineid = ?addressformatlineid";
                DAO.ExecuteNonQuery(sql, new MySqlParameter("addressid", addressid), new MySqlParameter("line", line), new MySqlParameter("addressformatlineid", addressformatlineid), new MySqlParameter("value", values[addressformatlineid]));
            }
        }

        public static void removeAddress(uint addressid)
        {
            string sql = "delete from addresses where addressid = ?addressid; " +
                "delete from addresslines where addressid = ?addressid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("addressid", addressid));
        }

        public static MySqlDataReader listAddressTypes()
        {
            string sql = "select addresstypeid, name from addresstypes order by name";
            return DAO.ExecuteReader(sql);
        }

        public static string getAddressType(int typeId)
        {
            string sql = "select  name from addresstypes where addresstypeid=?addresstypeid";
            return (string)DAO.ExecuteScalar(sql, new MySqlParameter("addresstypeid", typeId));
        }

        public static bool isDuplicateAddressType(string name, uint addresstypeid)
        {
            string sql = "select count(addresstypeid) as `exists` from addresstypes where name = ?name and addresstypeid != ?addresstypeid";
            uint exists = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("name", name), new MySqlParameter("addresstypeid", addresstypeid)));
            return (exists > 0);
        }

        public static MySqlDataReader searchCountries(string startWith, int returnCount)
        {
            string sql = "select name " +
                "from countries " +
                "where name like concat_ws(?keyword,'','%') " +
                "order by name " +
                "limit " + returnCount.ToString();
            return DAO.ExecuteReader(sql, new MySqlParameter("keyword", startWith));
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
            return DAO.ExecuteReader(sql, new MySqlParameter("addressid", addressid));
        }

        public static string getAddressHTML(uint addressid)
        {
            StringBuilder sb = new StringBuilder();
            using (MySqlDataReader drA = getAddress(addressid))
            {
                uint line = 1;
                while (drA.Read())
                {
                    if (line != (uint)DAO.getUInt(drA, "line"))
                    {
                        sb.Append("<br />");
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
                        sb.Append("<br />");
                        sb.Append(DAO.getString(drA, "name"));
                    }
                }
            }
            return sb.ToString();
        }

        public static MySqlDataReader listActiveCountries()
        {
            string sql = "select countryid, name " +
                "from countries " +
                "where active = 1 " +
                "order by name";
            return DAO.ExecuteReader(sql);
        }

        public static MySqlDataReader listAllCountries()
        {
            string sql = "select countryid, name " +
                "from countries " +
                "order by name";
            return DAO.ExecuteReader(sql);
        }

        public static bool isDuplicateCountry(string name, uint countryid)
        {
            string sql = "select count(countryid) as `exists` from countries where name = ?name and countryid != ?countryid";
            uint exists = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("name", name), new MySqlParameter("countryid", countryid)));
            return (exists > 0);
        }

        public static bool isDuplicateCountryCode(string code, uint countryid)
        {
            string sql = "select count(countryid) as `exists` from countries where code = ?code and countryid != ?countryid";
            uint exists = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("code", code), new MySqlParameter("countryid", countryid)));
            return (exists > 0);
        }

        public static MySqlDataReader listLocations()
        {
            string sql = "select l.locationid, l.name as location, c.name as country " +
                "from locations as l " +
                "join countries as c on c.countryid = l.countryid " +
                "order by l.locationid,c.name, l.name";
            return DAO.ExecuteReader(sql);
        }

        public static bool isDuplicateLocation(string name, uint countryid, uint locationid)
        {
            string sql = "select count(locationid) as `exists` from locations where name = ?name and countryid = ?countryid and locationid != ?locationid";
            uint exists = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("name", name), new MySqlParameter("countryid", countryid), new MySqlParameter("locationid", locationid)));
            return (exists > 0);
        }

        public static MySqlDataReader getAddressFormat(uint countryid)
        {
            string sql = "select afl.addressformatlineid, afl.line, afl.columnspan, afl.position, afl.name, afl.type, afl.required, afl.validation " +
                "from addressformatlines as afl " +
                "join addressformats as af on af.addressformatid = afl.addressformatid " +
                "where af.countryid = ?countryid " +
                "order by afl.line, afl.position";
            return DAO.ExecuteReader(sql, new MySqlParameter("countryid", countryid));
        }

        public static List<string> getListValues(string validation)
        {
            switch (validation)
            {
                case "listAUStates":
                    return listAUStates();

                case "listAEEmirates":
                    return listAEEmirates();

                default:
                    return new List<string>();
            }
        }

        public static List<string> listAUStates()
        {
            List<string> states = new List<string>();
            states.Add("ACT");
            states.Add("NSW");
            states.Add("NT");
            states.Add("QLD");
            states.Add("SA");
            states.Add("TAS");
            states.Add("VIC");
            states.Add("WA");
            return states;
        }

        public static List<string> listAEEmirates()
        {
            List<string> emirates = new List<string>();
            emirates.Add("Abu Zaby (Abu Dhabi)");
            emirates.Add("‘Ajman");
            emirates.Add("Al Fujayrah");
            emirates.Add("Ash Shariqah");
            emirates.Add("Dubayy (Dubai)");
            emirates.Add("Ra’s al Khaymah");
            emirates.Add("Umm al Qaywayn");
            return emirates;
        }
    }
}