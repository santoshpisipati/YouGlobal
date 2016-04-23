using System;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for GlobalPanda
/// </summary>

namespace GlobalPanda
{
    public class Common
    {
        private static string[] exmptPages = { "/globalpanda/login.aspx", "/globalpanda/_navigation/logout.aspx", "/globalpanda/default.aspx", "/globalpanda/_errors/default.aspx", "/globalpanda/files/addfile.aspx" };
        private static string[] months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        public static bool IsExempt(string path)
        {
            if (path.Contains(".aspx"))
            {
                int idx = Array.IndexOf<string>(exmptPages, path.ToLower());
                if (idx >= 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public static string CreateQueryString(string url, string qrystr)
        {
            string page = url;
            if (page.IndexOf("/") >= 0) page = url.Substring(url.LastIndexOf("/") + 1);
            string a = HashQueryString(page, qrystr);

            string hashedurl = url;
            if (qrystr != null && qrystr.Length > 0)
            {
                hashedurl += "?" + qrystr + "&a=" + a;
            }
            else
            {
                hashedurl += "?a=" + a;
            }

            return hashedurl;
        }

        public static string HashQueryString(string page, string qrystr)
        {
            string salt = System.Web.HttpContext.Current.Session.SessionID;
            string secret = "walkerglobal";
            //Create an encoding object to ensure the encoding standard for the source text
            string data = salt + page + qrystr + secret;
            UTF8Encoding utf8enc = new UTF8Encoding();
            //Retrieve a byte array based on the source text
            byte[] bytes = utf8enc.GetBytes(data);
            //Instantiate an MD5 Provider object
            MD5CryptoServiceProvider md5crypto = new MD5CryptoServiceProvider();
            //Compute the hash value from the source
            byte[] hash = md5crypto.ComputeHash(bytes);
            //Cycle through the hash and convert to string
            return System.Web.HttpServerUtility.UrlTokenEncode(hash);
        }

        public static string Base2Base(string str, int inbase, int outbase)
        {
            string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string stream = "";
            int num;
            int res;
            int i;
            int j;

            if (inbase != 10)
            {
                num = 0;
                i = str.Length - 1;
                j = 0;
                while (i >= 0)
                {
                    num = num + Convert.ToInt32((Math.Pow(inbase, j) * (chars.IndexOf(str.Substring(i, 1)))));
                    i--;
                    j++;
                }
            }
            else
            {
                num = Convert.ToInt32(str);
            }

            while (num > 0)
            {
                res = num % outbase;
                num = Convert.ToInt32(Math.Floor(Convert.ToDouble(num / outbase)));
                stream = chars.Substring(res, 1) + stream;
            }

            return stream;
        }

        public static string Base2Base_Old(string inputnumber, int inputbase, int outputbase)
        {
            int j;
            int k;
            float decimalvalue;
            int x;
            int inputnumberlength;
            string numericbasedata = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int maxbase = numericbasedata.Length;
            string outputnumber;

            if (inputbase > maxbase || outputbase > maxbase || inputbase < 2 || outputbase < 2)
            {
                outputnumber = "invalid";
            }
            else
            {
                inputnumberlength = inputnumber.Length;
                decimalvalue = 0;

                //convert to base 10
                for (j = 0; j < inputnumberlength; j++)
                {
                    for (k = 0; k < inputbase; k++)
                    {
                        if (inputnumber.Substring(j, 1) == numericbasedata.Substring(k, 1))
                        {
                            decimalvalue = decimalvalue + (int)((k) * Math.Pow(inputbase, (inputnumberlength - (j + 1))) + 0.5);
                            k = inputbase;
                        }
                    }
                }

                //Convert the Base 10 value (DecimalValue) to the desired output base
                outputnumber = "";
                while (decimalvalue > 0)
                {
                    //set @x = cast(((@decimalvalue / @outputbase) - cast(@decimalvalue / @outputbase as int)) * @outputbase + 1.5 as int)
                    //x = Convert.ToInt32(((decimalvalue / outputbase) - (int)(@decimalvalue / @outputbase)) * @outputbase + 0.5);
                    double y = (decimalvalue / outputbase) - Math.Floor(decimalvalue / outputbase);
                    x = (int)Math.Round(((y * outputbase) + 0.5), 0, MidpointRounding.AwayFromZero);
                    outputnumber = numericbasedata.Substring(x - 1, 1) + outputnumber;
                    decimalvalue = (int)Math.Floor(decimalvalue / outputbase);
                }
            }
            return outputnumber;
        }

        public static DateTime getLocalTime(DateTime utcdatetime)
        {
            return utcdatetime.AddMinutes(GPSession.UTCOffset);
        }

        public static void seedDateDDL(DropDownList y, DropDownList m, DropDownList d)
        {
            y.Items.Add(new ListItem(""));
            for (int i = DateTime.Today.Year; i > (DateTime.Today.Year - 100); i--)
            {
                y.Items.Add(new ListItem(i.ToString()));
            }

            m.Items.Add(new ListItem(""));
            foreach (string month in months)
            {
                m.Items.Add(new ListItem(month));
            }

            d.Items.Add(new ListItem(""));
            for (int i = 1; i <= 31; i++)
            {
                d.Items.Add(new ListItem(i.ToString()));
            }
        }

        public static DateTime dateFromDisplay(string display)
        {
            string[] split = display.Split(",".ToCharArray());
            DateTime dte = new DateTime(Convert.ToInt32(split[0]), 1, 1);
            if (split[1].Length > 0)
            {
                if (split[1] != "0")
                {
                    dte = dte.AddMonths(Array.IndexOf<string>(months, split[1]));
                    if (split[2].Length > 0)
                    {
                        dte = dte.AddDays(Convert.ToInt32(split[2]) - 1);
                    }
                }
            }
            return dte;
        }

        public static DateTime? ParseDate(string strDate, string strFormat)
        {
            DateTime? dFormattedDate = null;
            System.Globalization.CultureInfo MyCultureInfo = new System.Globalization.CultureInfo("en-AU");
            System.Globalization.DateTimeFormatInfo dtfi = new System.Globalization.DateTimeFormatInfo();
            dtfi.ShortDatePattern = strFormat;
            MyCultureInfo.DateTimeFormat = dtfi;

            DateTime tmpDate;
            if (DateTime.TryParseExact(strDate, strFormat, MyCultureInfo, System.Globalization.DateTimeStyles.None, out tmpDate))
            { dFormattedDate = tmpDate; }
            return dFormattedDate;
        }
    }
}