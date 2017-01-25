using ICSharpCode.SharpZipLib.Zip;
using Sample.Web.ModalLogin.Helpers;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using YG_Business;

namespace Sample.Web.ModalLogin.Controllers
{
    public abstract class BaseController : Controller
    {
        private static string[] months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        protected BaseController()
        {
        }

        protected override void HandleUnknownAction(string actionName)
        {
            base.View(this.DefaultViewName).ExecuteResult(base.ControllerContext);
        }

        public void ShowHotJobs()
        {
            Session["HotJobs"] = null;
            Job objJob = new Job();
            int limit = Convert.ToInt32(ConfigurationManager.AppSettings["NumberOfHotJobsToDisplayInModule"].ToString());
            Session["HotJobs"] = objJob.HotJobs(limit);
        }

        public void GetPhoneCodes()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Server.MapPath("~/countryphonecodes.xml"));
            XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("/countries/country");
            DataTable dt = new DataTable();
            dt.Columns.Add("phoneCode");
            for (int i = 0; i < nodeList.Count; i++)
            {
                DataRow dr;
                dr = dt.NewRow();
                dr[0] = string.Format("(+{0}) {1}", nodeList[i].Attributes["phoneCode"].Value, nodeList[i].Attributes["name"].Value);
                dt.Rows.Add(dr);
            }
            Session.Add("PhoneCodes", dt);
        }

        public virtual ActionResult Index()
        {
            Common obj = new Common();
            if (Session["ClassificationList"] == null)
            {
                Session.Add("ClassificationList", obj.JobClassificationList());
            }

            if (Session["LocationList"] == null)
            {
                Session.Add("LocationList", obj.LocationList());
            }

            if (Session["WorkTypeList"] == null)
            {
                Session.Add("WorkTypeList", obj.WorkTypeList());
            }

            if (Session["FrequencyList"] == null)
            {
                Session.Add("FrequencyList", obj.AlertFrequnecyList());
            }

            if (Session["emailtype"] == null)
            {
                Session.Add("emailtype", obj.EmailTypeList());
            }
            if (Session["title"] == null)
            {
                Session.Add("title", obj.TitleList());
            }
            if (Session["gender"] == null)
            {
                Session.Add("gender", obj.GenderList());
            }

            if (Session["marital"] == null)
            {
                Session.Add("marital", obj.MaritalList());
            }

            if (Session["phonetype"] == null)
            {
                Session.Add("phonetype", obj.PhoneTypeList());
            }

            if (Session["currency"] == null)
            {
                Session.Add("currency", obj.CurrencyList());
            }

            if (Session["frequency"] == null)
            {
                Session.Add("frequency", obj.FrequencyList());
            }

            if (Session["JobIndustryList"] == null)
            {
                DataTable dt = obj.JobIndustryList();
                DataRow dr = dt.NewRow();
                dr[0] = "";
                dr[1] = "-- Any --";
                dr[2] = 0;
                dt.Rows.InsertAt(dr, 0);
                Session.Add("JobIndustryList", dt);
            }

            if (Session["JobIndustryResumeList"] == null)
            {
                DataTable dt = ((DataTable)Session["JobIndustryList"]).Copy();
                dt.Rows.RemoveAt(0);
                DataRow dr = dt.NewRow();
                dr[0] = "";
                dr[1] = "- Please Select -";
                dr[2] = -1;
                dt.Rows.InsertAt(dr, 0);
                dr = dt.NewRow();
                dr[0] = "Other";
                dr[1] = "Other";
                dr[2] = 0;
                dt.Rows.InsertAt(dr, 1);
                Session.Add("JobIndustryResumeList", dt);
            }

            if (Session["JobIndustySub"] == null)
            {
                DataTable dt = obj.JobIndustrySubList();
                DataRow dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "-- Any --";
                dr[2] = "";
                dt.Rows.InsertAt(dr, 0);
                Session.Add("JobIndustySub", dt);
            }

            if (Session["SearchLocationList"] == null)
            {
                DataTable dt = obj.LocationList();
                DataRow dr = dt.NewRow();
                dr[0] = "";
                dr[1] = "-- Any --";
                dr[2] = 0;
                dt.Rows.InsertAt(dr, 0);
                Session.Add("SearchLocationList", dt);
            }

            if (Session["SearchWorkTypeList"] == null)
            {
                DataTable dt = obj.WorkTypeList();
                DataRow dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "-- Any --";
                dr[2] = "";
                dt.Rows.InsertAt(dr, 0);
                Session.Add("SearchWorkTypeList", dt);
            }

            ListItemCollection y = new ListItemCollection();
            y.Add("");
            ListItemCollection y1 = new ListItemCollection();
            y1.Add("");
            y1.Add("Present");
            for (int i = DateTime.Today.Year; i > (DateTime.Today.Year - 100); i--)
            {
                y.Add(new ListItem(i.ToString()));
                y1.Add(new ListItem(i.ToString()));
            }
            Session.Add("year", y);
            Session.Add("Toyear", y1);

            ListItemCollection m = new ListItemCollection();
            m.Add("");
            foreach (string month in months)
            {
                m.Add(new ListItem(month));
            }
            Session.Add("month", m);

            ListItemCollection d = new ListItemCollection();
            d.Add("");
            for (int i = 1; i <= 31; i++)
            {
                d.Add(new ListItem(i.ToString()));
            }
            Session.Add("day", d);

            return base.View(this.DefaultViewName);
        }

        protected bool ProcessUpload()
        {
            ZipOutputStream stream = null;
            bool flag;
            string str = (base.Request["SubmitSource"] ?? "default").ToLower().Trim();
            string address = ConfigurationManager.AppSettings.Get("emailAddress");
            string displayName = ConfigurationManager.AppSettings.Get("emailName");
            string replyAddress = "";
            string replyDisplayName = "";
            try
            {
                TableCell cell8;
                StringBuilder builder = new StringBuilder();
                builder.Append("<html><head><style type='text/css'> body { font-family: Arial, Verdana, Sans-Serif; } </style></head><body>");
                builder.Append("<p>The following details have been submitted at the YOU Global website from page " + base.Request.UrlReferrer + ";</p>");
                Table table = new Table
                {
                    CellPadding = 3,
                    CellSpacing = 0,
                    BorderWidth = 1
                };
                TableRow row = new TableRow();
                TableHeaderCell cell = new TableHeaderCell
                {
                    BorderWidth = 1,
                    Text = "Field"
                };
                row.Cells.Add(cell);
                TableHeaderCell cell2 = new TableHeaderCell
                {
                    BorderWidth = 1,
                    ColumnSpan = 2,
                    Text = "Value"
                };
                row.Cells.Add(cell2);
                table.Rows.Add(row);
                foreach (string str4 in base.Request.Form.AllKeys)
                {
                    TableRow row2 = new TableRow();
                    TableCell cell3 = new TableCell
                    {
                        BorderWidth = 1,
                        Text = str4 + ":"
                    };
                    if (str4 == "Email")
                    {
                        replyAddress = base.Request.Params[str4].Replace("\r", "").Replace("\n", "<br />");
                    }
                    if (str4 == "Full Name")
                    {
                        replyDisplayName = base.Request.Params[str4].Replace("\r", "").Replace("\n", "<br />");
                    }
                    row2.Cells.Add(cell3);
                    TableCell cell4 = new TableCell
                    {
                        BorderWidth = 1,
                        ColumnSpan = 2,
                        Text = base.Request.Params[str4].Replace("\r", "").Replace("\n", "<br />")
                    };
                    row2.Cells.Add(cell4);
                    table.Rows.Add(row2);
                }
                TableRow row3 = new TableRow();
                TableCell cell7 = new TableCell
                {
                    BorderWidth = 1,
                    Text = "Attached Files:"
                };
                row3.Cells.Add(cell7);
                int num = 0;
                for (int i = 0; i < base.Request.Files.Count; i++)
                {
                    HttpPostedFileBase base2 = base.Request.Files[i];
                    if (base2.ContentLength > 0)
                    {
                        cell8 = new TableCell
                        {
                            BorderWidth = 1,
                            Text = Path.GetFileName(base2.FileName)
                        };
                        row3.Cells.Add(cell8);
                        TableCell cell9 = new TableCell
                        {
                            BorderWidth = 1,
                            Text = (base2.ContentLength / 0x400) + " KB"
                        };
                        row3.Cells.Add(cell9);
                        table.Rows.Add(row3);
                        row3 = new TableRow();
                        num++;
                    }
                }
                if (num == 0)
                {
                    cell8 = new TableCell
                    {
                        BorderWidth = 1,
                        ColumnSpan = 2,
                        Text = "None"
                    };
                    row3.Cells.Add(cell8);
                    //table.Rows.Add(row3);
                    num++;
                }
                cell7.RowSpan = num;
                StringBuilder sb = new StringBuilder();
                StringWriter writer = new StringWriter(sb);
                HtmlTextWriter writer2 = new HtmlTextWriter(writer);
                table.RenderControl(writer2);
                builder.Append(sb.ToString());
                builder.Append("</body></html>");
                MemoryStream baseOutputStream = new MemoryStream();
                stream = new ZipOutputStream(baseOutputStream);
                stream.SetLevel(9);
                stream.UseZip64 = UseZip64.Off;
                byte[] buffer = new byte[0x8000];
                int num3 = 0;
                for (int j = 0; j < base.Request.Files.Count; j++)
                {
                    HttpPostedFileBase base3 = base.Request.Files[j];
                    if (base3.ContentLength > 0)
                    {
                        int num6;
                        num3++;
                        ZipEntry entry = new ZipEntry(Path.GetFileName(base3.FileName))
                        {
                            Size = base3.ContentLength
                        };
                        stream.PutNextEntry(entry);
                        for (long k = base3.InputStream.Length; k > 0L; k -= num6)
                        {
                            num6 = base3.InputStream.Read(buffer, 0, buffer.Length);
                            stream.Write(buffer, 0, num6);
                        }
                        stream.CloseEntry();
                    }
                }
                stream.Finish();
                stream.Close();
                string str5 = ConfigurationManager.AppSettings.Get(str + ".emailAddress") ?? (ConfigurationManager.AppSettings.Get("default.emailAddress") ?? "admin@you-global.com");
                string str6 = ConfigurationManager.AppSettings.Get(str + ".emailName") ?? (ConfigurationManager.AppSettings.Get("default.emailName") ?? "YOU Global");
                string str7 = ConfigurationManager.AppSettings.Get(str + ".subject") ?? (ConfigurationManager.AppSettings.Get("default.subject") ?? "YOU Global website form - Other Services inquiry");
                if (base.Request.UrlReferrer.ToString().Contains("ContactUsHtml"))
                {
                    str7 = string.Format("{0}-{1}", ConfigurationManager.AppSettings.Get(str + ".subject"), replyDisplayName) ?? (ConfigurationManager.AppSettings.Get("default.subject") ?? "You Global Contact Us Submisson");
                }
                MailMessage message = new MailMessage
                {
                    From = new MailAddress(address, displayName),
                    Subject = str7,
                    Body = builder.ToString(),
                    IsBodyHtml = true
                };
                message.ReplyToList.Add(new MailAddress(replyAddress, replyDisplayName));
                message.To.Add(new MailAddress(str5, str6));
                string str8 = ConfigurationManager.AppSettings.Get("bcc.emailAddresses");
                string str9 = ConfigurationManager.AppSettings.Get("bcc.emailNames");
                if (!string.IsNullOrEmpty(str8) && !string.IsNullOrEmpty(str9))
                {
                    string[] strArray = str8.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    string[] strArray2 = str9.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (((strArray.Length > 0) && (strArray2.Length > 0)) && (strArray.Length == strArray2.Length))
                    {
                        for (int m = 0; m < strArray.Length; m++)
                        {
                            message.Bcc.Add(new MailAddress(strArray[m], strArray2[m]));
                        }
                    }
                }
                if (num3 > 0)
                {
                    message.Attachments.Add(new Attachment(new MemoryStream(baseOutputStream.ToArray()), "Attachments.zip"));
                }
                SmtpClient sc = new SmtpClient();
                sc.Host = ConfigurationManager.AppSettings["smtpHost"].ToString();
                string smtpUser = ConfigurationManager.AppSettings["smtpUserName"].ToString();
                string smtpPwd = ConfigurationManager.AppSettings["smtpPassword"].ToString();
                sc.Credentials = new System.Net.NetworkCredential(smtpUser, smtpPwd);
                sc.Send(message);
                flag = true;
            }
            catch (Exception exception)
            {
                StringBuilder builder3 = new StringBuilder();
                builder3.Append(string.Concat(new object[] { exception.Message, '\r', '\n', exception.StackTrace }));
                
                MailMessage message3 = new MailMessage
                {
                    From = new MailAddress(address, displayName),
                    Subject = "Dunst Consulting Site Error - Contact Us",
                    Priority = MailPriority.High,
                    Body = builder3.ToString(),
                    IsBodyHtml = false
                };
                message3.To.Add(new MailAddress(address, displayName));
                SmtpClient sc = new SmtpClient();
                sc.Host = ConfigurationManager.AppSettings["smtpHost"].ToString();
                string smtpUser = ConfigurationManager.AppSettings["smtpUserName"].ToString();
                string smtpPwd = ConfigurationManager.AppSettings["smtpPassword"].ToString();
                sc.Credentials = new System.Net.NetworkCredential(smtpUser, smtpPwd);
                sc.Send(message3);
                flag = false;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
            return flag;
        }

        public void SendMail(string mailid,string link, bool isRegistered)
        {
            string body = "";
            if (isRegistered)
            {
                body = "<p>Email From:({0})</p><p>Message:<a href=" + link + ">Please click to activate your account</a></p>";
            }
            else
            {
                body = "<p>New Email id has been updated in our system</p><p>Please login with the new username and password you have registered with us. </p> ";
            }
            string address = ConfigurationManager.AppSettings.Get("emailAddress");
            string displayName = ConfigurationManager.AppSettings.Get("emailName");
            try
            {
                MailMessage message = new MailMessage
                {
                    From = new MailAddress(address, displayName),
                    Subject = "YOU Global - user registration - activation request",
                    Body = body,
                    IsBodyHtml = true
                };
                string str = "YOU Global";
                message.To.Add(new MailAddress(mailid, str));
                string str8 = ConfigurationManager.AppSettings.Get("bcc.emailAddresses");
                string str9 = ConfigurationManager.AppSettings.Get("bcc.emailNames");
                SmtpClient sc = new SmtpClient();
                sc.Host = ConfigurationManager.AppSettings["smtpHost"].ToString();
                string smtpUser = ConfigurationManager.AppSettings["smtpUserName"].ToString();
                string smtpPwd = ConfigurationManager.AppSettings["smtpPassword"].ToString();
                sc.Credentials = new System.Net.NetworkCredential(smtpUser, smtpPwd);
                sc.Send(message);
            }
            catch (Exception ex)
            {
                StringBuilder builder3 = new StringBuilder();
                builder3.Append(string.Concat(new object[] { ex.Message, '\r', '\n', ex.StackTrace }));
                MailMessage message3 = new MailMessage
                {
                    From = new MailAddress(address, displayName),
                    Subject = "Dunst Consulting Site Error - Contact Us",
                    Priority = MailPriority.High,
                    Body = builder3.ToString(),
                    IsBodyHtml = false
                };
                message3.To.Add(new MailAddress(address, displayName));
                SmtpClient sc = new SmtpClient();
                sc.Host = ConfigurationManager.AppSettings["smtpHost"].ToString();
                string smtpUser = ConfigurationManager.AppSettings["smtpUserName"].ToString();
                string smtpPwd = ConfigurationManager.AppSettings["smtpPassword"].ToString();
                sc.Credentials = new System.Net.NetworkCredential(smtpUser, smtpPwd);
                sc.Send(message3);
            }
        }

        public abstract string DefaultViewName { get; }
    }
}