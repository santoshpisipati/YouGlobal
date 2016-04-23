using System;

//using System.Data;
//using System.Configuration;
//using System.Web;
//using System.Web.Security;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using System.Web.UI.WebControls.WebParts;
//using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for GlobalPanda
/// </summary>
///
namespace GlobalPanda
{
    public class GPSession : System.Web.SessionState.IRequiresSessionState
    {
        public static uint UserId
        {
            get
            {
                if (System.Web.HttpContext.Current.Session == null || System.Web.HttpContext.Current.Session["GlobalPanda.UserId"] == null)
                {
                    return 0;
                }
                else
                {
                    return Convert.ToUInt32(System.Web.HttpContext.Current.Session["GlobalPanda.UserId"]);
                }
            }
        }

        public static string UserName
        {
            get
            {
                return Convert.ToString(System.Web.HttpContext.Current.Session["GlobalPanda.UserName"]);
            }
        }

        public static uint UserRoleId
        {
            get
            {
                return Convert.ToUInt32(System.Web.HttpContext.Current.Session["GlobalPanda.UserRoleId"]);
            }
        }

        public static string UserRoleName
        {
            get
            {
                return Convert.ToString(System.Web.HttpContext.Current.Session["GlobalPanda.UserRoleName"]);
            }
        }

        public static uint TenantId
        {
            get
            {
                return Convert.ToUInt32(System.Web.HttpContext.Current.Session["GlobalPanda.TenantId"]);
            }
        }

        public static string TenantName
        {
            get
            {
                return Convert.ToString(System.Web.HttpContext.Current.Session["GlobalPanda.TenantName"]);
            }
        }

        public static int UTCOffset
        {
            get
            {
                return Convert.ToInt32(System.Web.HttpContext.Current.Session["GlobalPanda.UTCOffset"]);
            }
        }

        public static bool hasSearch(string module)
        {
            if (System.Web.HttpContext.Current.Session["GlobalPanda.Search." + module] != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string getSearch(string module)
        {
            return Convert.ToString(System.Web.HttpContext.Current.Session["GlobalPanda.Search." + module]);
        }

        public static void setSearch(string module, string value)
        {
            System.Web.HttpContext.Current.Session["GlobalPanda.Search." + module] = value;
        }
    }
}