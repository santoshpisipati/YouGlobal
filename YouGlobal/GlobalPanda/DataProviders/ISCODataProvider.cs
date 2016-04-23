using MySql.Data.MySqlClient;
using System.Data;

/// <summary>
/// Summary description for ISCODataProvider
/// </summary>
///
namespace GlobalPanda.DataProviders
{
    public class ISCODataProvider
    {
        public static DataTable searchISCO(string search, string columns, string columns2, string columns3, string columns4, int level)
        {
            string sql = "Select groupcode as macode,title as matitle,'' as scode,'' as stitle,'' as mincode,'' as mintitle,'' as ucode,'' as utitle,keyword,type,groupcode,definition,Tasksinclude,includedoccupations,excludedoccupations,notes " +
                         " from isco08 where (type=1 and ( ?searchlevel=0 or ?searchlevel=1)) and  (match(" + columns + ") against (?search in BOOLEAN MODE) or ?search='') " +
                         " Union  Select t1.groupcode as macode,t1.title as matitle,t2.groupcode as scode,t2.title as stitle,'' as mincode,'' as mintitle,'' as ucode,'' as utitle,t2.keyword,t2.type,t2.groupcode,t2.definition,t2.Tasksinclude, " +
                         " t2.includedoccupations,t2.excludedoccupations,t2.notes from isco08 t1  join isco08 t2 on t1.groupcode=t2.parentcode where (t2.type=2 and " +
                         " ( ?searchlevel=0 or ?searchlevel=2)) and  (match(" + columns2 + ") against (?search in BOOLEAN MODE) or ?search='')" +
                         " Union Select t1.groupcode as macode,t1.title as matitle,t2.groupcode as scode,t2.title as stitle,t3.groupcode as mincode,t3.title as mintitle,'' as ucode,'' as utitle,t3.keyword,t3.type,t3.groupcode,t3.definition," +
                         " t3.Tasksinclude,t3.includedoccupations,t3.excludedoccupations,t3.notes " +
                         " from isco08 t1  join isco08 t2 on t1.groupcode=t2.parentcode and t2.type=2  join isco08 t3 on t2.groupcode=t3.parentcode and t2.type=2 where (t3.type=3 and ( ?searchlevel=0 or ?searchlevel=3)) and  (match(" + columns3 + ") against (?search in BOOLEAN MODE) or ?search='')" +
                         " Union Select t1.groupcode as macode,t1.title as matitle,t2.groupcode as scode,t2.title as stitle,t3.groupcode as mincode,t3.title as mintitle ,t4.groupcode as ucode,t4.title as utitle,t4.keyword,t4.type,t4.groupcode," +
                         " t4.definition,t4.Tasksinclude,t4.includedoccupations,t4.excludedoccupations,t4.notes " +
                         " from isco08 t1 join isco08 t2 on t1.groupcode=t2.parentcode  join isco08 t3 on t2.groupcode=t3.parentcode  join isco08 t4 on t3.groupcode=t4.parentcode and t3.type =3 where (t4.type=4 and ( ?searchlevel=0 or ?searchlevel=4)) and  (match(" + columns4 + ") against (?search in BOOLEAN MODE) or ?search='')";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("search", search), new MySqlParameter("searchlevel", level));
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            dr.Dispose();
            return dt;
        }

        public static void updateKeyword(string code, string keyword)
        {
            string sql = "update isco08 set keyword=?keyword where groupcode=?code";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("keyword", keyword), new MySqlParameter("code", code));
        }

        public static MySqlDataReader listISCO08(int type)
        {
            string sql = "select `isco08id`,groupcode,title from isco08 where type=?type";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("type", type));
            return dr;
        }

        public static MySqlDataReader searchgroup(string search, int type)
        {
            //string sql = "select `isco08id`,groupcode,title from isco08 where (title like concat('%',?search,'%') or keyword like concat('%',?search,'%')) and type=?type ";
            string sql = "Select * from v_isco08  where ( d1 like concat('%',?keyword,'%') or d2 like concat('%',?keyword,'%') or d3 like concat('%',?keyword,'%') or d4 like concat('%',?keyword,'%') " +
                      " or c1 like concat('%',?keyword,'%') or c2 like concat('%',?keyword,'%') or c3 like concat('%',?keyword,'%') or c4 like concat('%',?keyword,'%') " +
                      " or k1 like concat('%',?keyword,'%') or k2 like concat('%',?keyword,'%') or k3 like concat('%',?keyword,'%') or k4 like concat('%',?keyword,'%') ) order by id4";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("keyword", search));
            return dr;
        }

        public static MySqlDataReader getISCO08ByID(int id)
        {
            string sql = "select `isco08id`,groupcode,title from isco08 where isco08id=?id";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("id", id));
            return dr;
        }
    }
}