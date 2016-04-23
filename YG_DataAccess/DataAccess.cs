using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace YG_DataAccess
{
    public class DataAccess
    {
        private const int _timeout = 30;

        public static int ExecuteNonQuery(string sql, params MySqlParameter[] parameters)
        {
            return ExecuteNonQuery(sql, CommandType.Text, _timeout, parameters);
        }

        public static int ExecuteNonQuery(string sql, CommandType ctype, params MySqlParameter[] parameters)
        {
            return ExecuteNonQuery(sql, ctype, _timeout, parameters);
        }

        public static int ExecuteNonQuery(string sql, CommandType ctype, int timeout, params MySqlParameter[] parameters)
        {
            MySqlConnection cn = null;
            MySqlCommand cmd = null;
            int affectedrows = 0;

            try
            {
                cn = new MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["globalpanda"].ConnectionString);
                cmd = new MySqlCommand(sql, cn);
                cmd.CommandTimeout = timeout;
                cmd.CommandType = ctype;

                foreach (MySqlParameter param in parameters)
                {
                    cmd.Parameters.Add(param);
                }
                cn.Open();
                affectedrows = cmd.ExecuteNonQuery();
            }
            catch (MySqlException me)
            {
                throw me;
            }
            finally
            {
                cn.Close();
            }
            return affectedrows;
        }

        public static object ExecuteScalar(string sql, params MySqlParameter[] parameters)
        {
            return ExecuteScalar(sql, _timeout, parameters);
        }

        public static object ExecuteScalar(string sql, int timeout, params MySqlParameter[] parameters)
        {
            MySqlConnection cn = null;
            MySqlCommand cmd = null;
            object retrunvalue = null;

            try
            {
                cn = new MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["globalpanda"].ConnectionString);
                cmd = new MySqlCommand(sql, cn);
                cmd.CommandTimeout = timeout;
                cmd.CommandType = CommandType.Text;

                foreach (MySqlParameter param in parameters)
                {
                    cmd.Parameters.Add(param);
                }
                cn.Open();
                retrunvalue = cmd.ExecuteScalar();
            }
            catch (MySqlException me)
            {
                throw me;
            }
            finally
            {
                cn.Close();
            }
            return retrunvalue;
        }

        public static MySqlDataReader ExecuteReader(string sql, params MySqlParameter[] parameters)
        {
            return ExecuteReader(sql, CommandType.Text, _timeout, parameters);
        }

        public static MySqlDataReader ExecuteReader(string sql, CommandType ctype, params MySqlParameter[] parameters)
        {
            return ExecuteReader(sql, ctype, _timeout, parameters);
        }

        public static MySqlDataReader ExecuteReader(string sql, int timeout, params MySqlParameter[] parameters)
        {
            return ExecuteReader(sql, CommandType.Text, timeout, parameters);
        }

        public static MySqlDataReader ExecuteReader(string sql, CommandType ctype, int timeout, params MySqlParameter[] parameters)
        {
            MySqlConnection cn = null;
            MySqlCommand cmd = null;
            MySqlDataReader dr = null;

            try
            {
                cn = new MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["globalpanda"].ConnectionString);
                cmd = new MySqlCommand(sql, cn);
                cmd.CommandTimeout = timeout;
                cmd.CommandType = ctype;

                foreach (MySqlParameter param in parameters)
                {
                    cmd.Parameters.Add(param);
                }
                cn.Open();
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (MySqlException me)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                cn.Close();
                throw me;
            }

            return dr;
        }

        public static uint? getUInt(MySqlDataReader dr, string column)
        {
            int ordinal = dr.GetOrdinal(column);
            if (dr.IsDBNull(ordinal))
            {
                return null;
            }
            else
            {
                return dr.GetUInt32(ordinal);
            }
        }

        public static int? getInt(MySqlDataReader dr, string column)
        {
            int ordinal = dr.GetOrdinal(column);
            if (dr.IsDBNull(ordinal))
            {
                return null;
            }
            else
            {
                return dr.GetInt32(ordinal);
            }
        }

        public static String getString(MySqlDataReader dr, string column)
        {
            int ordinal = dr.GetOrdinal(column);
            if (dr.IsDBNull(ordinal))
            {
                return null;
            }
            else
            {
                return dr.GetString(ordinal);
            }
        }

        public static double? getDouble(MySqlDataReader dr, string column)
        {
            int ordinal = dr.GetOrdinal(column);
            if (dr.IsDBNull(ordinal))
            {
                return null;
            }
            else
            {
                return dr.GetDouble(ordinal);
            }
        }

        public static bool? getBool(MySqlDataReader dr, string column)
        {
            int ordinal = dr.GetOrdinal(column);
            if (dr.IsDBNull(ordinal))
            {
                return null;
            }
            else
            {
                return dr.GetBoolean(ordinal);
            }
        }

        public static DateTime? getDateTime(MySqlDataReader dr, string column)
        {
            int ordinal = dr.GetOrdinal(column);
            if (dr.IsDBNull(ordinal))
            {
                return null;
            }
            else
            {
                return dr.GetDateTime(ordinal);
            }
        }
    }
}