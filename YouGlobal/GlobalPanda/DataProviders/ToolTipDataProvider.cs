using MySql.Data.MySqlClient;
using System;

/// <summary>
/// Summary description for ToolTipDataProvider
/// </summary>
namespace GlobalPanda.DataProviders
{
    public class ToolTipDataProvider
    {
        public static void insertTooltip(string helpText, string label, int menuId)
        {
            string sql = "insert into tooltips(helptext,label,menuid,userid,modified) values (?helptext,?label,?menuid,?userid,?modified)";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("helptext", helpText), new MySqlParameter("label", label), new MySqlParameter("menuid", menuId), new MySqlParameter("userid", GPSession.UserId), new MySqlParameter("modified", DateTime.UtcNow));
        }

        public static void updateTooltip(int tooltipId, string helptext, string label, int menuid)
        {
            string sql = "update tooltips set helptext=?helptext,menuid=?menuid,label=?label,userid=?userid,modified=?modified where tooltipid=?tooltipid ";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("helptext", helptext), new MySqlParameter("menuid", menuid), new MySqlParameter("tooltipid", tooltipId), new MySqlParameter("label", label),
                new MySqlParameter("userid", GPSession.UserId), new MySqlParameter("modified", DateTime.UtcNow));
        }

        public static void updateTooltip(int tooltipId, string helptext)
        {
            string sql = "update tooltips set helptext=?helptext where tooltipid=?tooltipid ";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("helptext", helptext), new MySqlParameter("tooltipid", tooltipId));
        }

        public static MySqlDataReader listAllTooltips()
        {
            string sql = "select tooltipid,helptext,menuid,t.userid,username,date_format(t.modified,'%d-%b-%Y-%T') as modified,date_format(t.modified,'%d-%b-%Y') as modified1,label,m.name as menuname, (select name from menuitems where menuitemid=m.parentmenuitemid)  as parentmenu " +
                         " from tooltips t inner join users u on t.userid=u.userid inner join menuitems m on t.menuid=m.menuitemid";
            MySqlDataReader dr = DAO.ExecuteReader(sql);
            return dr;
        }

        public static MySqlDataReader getTooltipById(int tooltipId)
        {
            string sql = "select tooltipid,helptext,menuid,userid,modified,date_format(modified,'%d-%b-%Y') as modified1 from tooltips where tooltipid=?tooltipid";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("tooltipid", tooltipId));
            return dr;
        }

        public static MySqlDataReader getTooltipByIds(int fromId, int toId)
        {
            string sql = "select tooltipid,helptext,menuid,userid,modified,date_format(modified,'%d-%b-%Y') as modified1 from tooltips where tooltipid>=?fromId and tooltipid<=?toId";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("fromId", fromId), new MySqlParameter("toId", toId));
            return dr;
        }

        public static void insertWebsiteTooltip(string helptext, string label)
        {
            string sql = "insert into website_tooltips (helptext,label,userid,modified) values (?helptext,?label,?userid,?modified)";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("helptext", helptext), new MySqlParameter("label", label), new MySqlParameter("userid", GPSession.UserId), new MySqlParameter("modified", DateTime.UtcNow));
        }

        public static void updateWebsiteTooltip(int toolTipId, string helptext, string label)
        {
            string sql = "update website_tooltips set helptext=?helptext,label=?label,userid=?userid,modified=?modified where website_tooltipid=?tooltipid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("helptext", helptext), new MySqlParameter("tooltipid", toolTipId), new MySqlParameter("label", label),
                new MySqlParameter("userid", GPSession.UserId), new MySqlParameter("modified", DateTime.UtcNow));
        }

        public static MySqlDataReader listAllWebsiteTooltips()
        {
            string sql = "select website_tooltipid,helptext,t.userid,username,date_format(t.modified,'%d-%b-%Y-%T') as modified,date_format(t.modified,'%d-%b-%Y') as modified1,label " +
                         " from website_tooltips t inner join users u on t.userid=u.userid ";
            MySqlDataReader dr = DAO.ExecuteReader(sql);
            return dr;
        }

        public static MySqlDataReader getWebsiteTooltipById(int tooltipId)
        {
            string sql = "select website_tooltipid,helptext,userid,modified,date_format(modified,'%d-%b-%Y') as modified1 from website_tooltips where website_tooltipid=?tooltipid";
            MySqlDataReader dr = DAO.ExecuteReader(sql, new MySqlParameter("tooltipid", tooltipId));
            return dr;
        }
    }
}