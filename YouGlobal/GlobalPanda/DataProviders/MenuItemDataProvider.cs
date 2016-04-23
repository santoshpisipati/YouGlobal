using MySql.Data.MySqlClient;

/// <summary>
/// Summary description for MenuItemDataProvider
/// </summary>
namespace GlobalPanda.DataProviders
{
    public class MenuItemDataProvider
    {
        public static MySqlDataReader listMenuItems(uint userid)
        {
            string sql = "select mi.menuitemid, mi.name, mi.icon, mi.url, mi.querystring, mi.target, mi.parentmenuitemid, mi.displayorder " +
                         "from menuitems as mi " +
                         "join userroles_menuitems as urmi on urmi.menuitemid = mi.menuitemid " +
                         "join users as u on u.userroleid = urmi.userroleid " +
                         "where u.userid = ?userid " +
                         "order by mi.parentmenuitemid, mi.displayorder";
            MySqlDataReader drMI = DAO.ExecuteReader(sql, new MySqlParameter("userid", userid));
            return drMI;
        }

        public static MySqlDataReader listmenuitems()
        {
            string sql = "select mi.menuitemid, mi.name,  mi.parentmenuitemid, mi.icon, mi.displayorder,m1.name as parentmenu,(select count(m2.menuitemid) from menuitems m2 where m2.parentmenuitemid=mi.menuitemid ) haschild " +
                ",(select count(userroleid) from userroles_menuitems where menuitemid = mi.menuitemid ) as permissionlevel " +
                " from menuitems as mi   left join menuitems as m1 on mi.parentmenuitemid=m1.menuitemid order by mi.parentmenuitemid, mi.displayorder";
            MySqlDataReader drMI = DAO.ExecuteReader(sql);
            return drMI;
        }

        public static void updateMenu(int menuId, string menuname, int parentmenuitemid, int displayorder, int permissionlevel, string icon)
        {
            string sql = "update menuitems set name=?name,parentmenuitemid=?parentmenuid,displayorder=?displayorder,icon=?icon where menuitemid=?menuid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("name", menuname), new MySqlParameter("parentmenuid", parentmenuitemid), new MySqlParameter("displayorder", displayorder), new MySqlParameter("menuid", menuId), new MySqlParameter("icon", icon));
            sql = "delete from userroles_menuitems where menuitemid=?menuid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("menuid", menuId));
            sql = "insert into userroles_menuitems (userroleid,menuitemid) values (?userroleid,?menuid)";
            if (permissionlevel == 1)
                DAO.ExecuteNonQuery(sql, new MySqlParameter("userroleid", 1), new MySqlParameter("menuid", menuId));
            else if (permissionlevel == 2)
            {
                DAO.ExecuteNonQuery(sql, new MySqlParameter("userroleid", 1), new MySqlParameter("menuid", menuId));
                DAO.ExecuteNonQuery(sql, new MySqlParameter("userroleid", 2), new MySqlParameter("menuid", menuId));
            }
        }
    }
}