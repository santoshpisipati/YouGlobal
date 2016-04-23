using MySql.Data.MySqlClient;
using System.Data;

/// <summary>
/// Summary description for ISICRevDataProvider
/// </summary>
namespace GlobalPanda.DataProviders
{
    public class ISICRevDataProvider
    {
        public static MySqlDataReader getSections()
        {
            string sql = "select code,description,ExplanatoryNoteInclusion from isicrev4 where level=1";
            MySqlDataReader dr = DAO.ExecuteReader(sql);
            return dr;
        }

        public static MySqlDataReader getChildByParentCode(string parentcode)
        {
            string sql = "select code,description,ExplanatoryNoteInclusion from isicrev4 where parentcode=?parentcode";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("parentcode", parentcode));
            return dr;
        }

        public static DataTable getAll()
        {
            string sql = "select code,description,ExplanatoryNoteInclusion,level,parentcode,keyword from isicrev4";
            MySqlDataReader dr = DAO.ExecuteReader(sql);
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            dr.Dispose();

            return dt;
        }

        public static void updateKeyword(string code, string keyword)
        {
            string sql = "update isicrev4 set keyword=?keyword where code=?code";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("keyword", keyword), new MySqlParameter("code", code));
        }

        public static DataTable getISICRev4(string keyword)
        {
            MySqlDataReader drCC = DAO.ExecuteReader("getISICRev4", CommandType.StoredProcedure, new MySqlParameter("search", keyword));

            DataTable dt = new DataTable();
            dt.Load(drCC);
            drCC.Close();
            drCC.Dispose();
            return dt;
        }

        public static MySqlDataReader getISICRev4ById(int id)
        {
            string sql = "select code,description,ExplanatoryNoteInclusion,level,parentcode,keyword from isicrev4 where isicrev4id=?id";
            MySqlDataReader drCC = DAO.ExecuteReader(sql, new MySqlParameter("id", id));

            return drCC;
        }

        public static DataTable searchISICRev4(string keyword, string columns, string columns2, string columns3, string columns4, int level)
        {
            DataTable dt = new DataTable();
            string sql = "select * from (select code as sectioncode,description as sectiondescription, '' as divisioncode,'' as divisiondescription,'' as groupcode,'' as groupdescription,'' as classcode,'' as classdescription,keyword,level,code," +
                "ExplanatoryNoteInclusion,ExplanatoryNoteExclusion from isicrev4 where (level=1 and ( ?searchlevel=0 or ?searchlevel=1)) and  (match(" + columns + ") against (?search in BOOLEAN MODE) or ?search='') union " +
                " select t1.code as sectioncode,t1.description as sectiondescription,t2.code as divisioncode,t2.description as divisiondescription, '' as groupcode,'' as groupdescription,'' as classcode,'' as classdescription ,t2.keyword,t2.level,t2.code," +
                " t2.ExplanatoryNoteInclusion,t2.ExplanatoryNoteExclusion from isicrev4 t1 left join isicrev4 t2 on t1.code=t2.parentcode  where (t2.level=2 and ( ?searchlevel=0 or ?searchlevel=2))  and " +
                " (match(" + columns2 + ") against (?search in BOOLEAN MODE)  or ?search='')" +
                " union  select t1.code as sectioncode,t1.description as sectiondescription,t2.code as divisioncode,t2.description as divisiondescription, t3.code as groupcode,t3.description as groupdescription,'' as classcode,'' as classdescription ," +
                " t3.keyword,t3.level,t3.code, t3.ExplanatoryNoteInclusion,t3.ExplanatoryNoteExclusion from isicrev4 t1 left join isicrev4 t2 on t1.code=t2.parentcode and t2.level=2  left join isicrev4 t3 on t2.code=t3.parentcode " +
                " where (t3.level=3 and ( ?searchlevel=0 or ?searchlevel=3)) and  (match(" + columns3 + ") against (?search in BOOLEAN MODE)  or ?search='') union select t1.code as sectioncode,t1.description as sectiondescription,t2.code as divisioncode," +
                " t2.description as divisiondescription, t3.code as groupcode,t3.description as groupdescription,t4.code as classcode,t4.description as classdescription,t4.keyword,t4.level,t4.code, t4.ExplanatoryNoteInclusion,t4.ExplanatoryNoteExclusion " +
                " from isicrev4 t1 left join isicrev4 t2 on t1.code=t2.parentcode and t2.level=2 left join isicrev4 t3 on t2.code=t3.parentcode and t3.level=3 left join isicrev4 t4 on t3.code=t4.parentcode " +
                " where (t4.level=4 and ( ?searchlevel=0 or ?searchlevel=4)) and  (match(" + columns4 + ") against (?search in BOOLEAN MODE) or ?search='') ) as tbl  order by level, sectioncode,divisioncode,groupcode,classcode asc;";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("search", keyword), new MySqlParameter("searchlevel", level));
            dt.Load(dr);
            dr.Close();
            dr.Dispose();
            return dt;
        }

        public static MySqlDataReader searchISICRev4Class(string keyword)
        {
            string sql = " select concat(t1.description,'>',t2.description,'>',t3.description,'>',t4.description) as title,t4.description, t4.isicrev4id from isicrev4 t1 join isicrev4 t2 on t1.code=t2.parentcode and t2.level=2 " +
                         " join isicrev4 t3 on t2.code=t3.parentcode and t3.level=3 join isicrev4 t4 on t3.code=t4.parentcode and t4.level=4 where t1.level=1 " +
                         " and (t4.description like concat('%',?keyword,'%') or t3.description like concat('%',?keyword,'%')  or t2.description like concat('%',?keyword,'%') or t1.description like concat('%',?keyword,'%') " +
                         " or t4.keyword like concat('%',?keyword,'%') or t3.keyword like concat('%',?keyword,'%')  or t2.keyword like concat('%',?keyword,'%') or t1.keyword like concat('%',?keyword,'%'))";

            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword));
            return dr;
        }

        public static MySqlDataReader searchISICRev4withHierarchy(string keyword)
        {
            string sql = "Select * from v_isicrev4  where ( d1 like concat('%',?keyword,'%') or d2 like concat('%',?keyword,'%') or d3 like concat('%',?keyword,'%') or d4 like concat('%',?keyword,'%') " +
                       " or c1 like concat('%',?keyword,'%') or c2 like concat('%',?keyword,'%') or c3 like concat('%',?keyword,'%') or c4 like concat('%',?keyword,'%') " +
                       " or k1 like concat('%',?keyword,'%') or k2 like concat('%',?keyword,'%') or k3 like concat('%',?keyword,'%') or k4 like concat('%',?keyword,'%') ) order by id4,code";

            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("keyword", keyword));
            return dr;
        }
    }
}