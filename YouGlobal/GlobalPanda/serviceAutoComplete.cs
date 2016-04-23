using GlobalPanda.DataProviders;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Web.Services;

/// <summary>
/// Summary description for serviceAutoComplete
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
// [System.Web.Script.Services.ScriptService]
public class serviceAutoComplete : System.Web.Services.WebService
{
    public serviceAutoComplete()
    {
        //Uncomment the following line if using designed components
        //InitializeComponent();
    }

    [WebMethod]
    public static List<string> getCountryCode(string prefixText, int count, string contextKey)
    {
        List<string> result = new List<string>();
        MySqlDataReader drCountryCode = CommonDataProvider.searchCountryCode(prefixText);
        while (drCountryCode.Read())
        {
            result.Add(drCountryCode["Code"].ToString());
        }
        return result;
    }
}