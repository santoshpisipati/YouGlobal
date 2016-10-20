using System.Configuration;
using System.Data;
using System.Net.Mail;
using System.Web.Mvc;
using YG_Business;
using YG_MVC.Models;

namespace Sample.Web.ModalLogin.Controllers
{
    public class WorkController : BaseController
    {
        public override string DefaultViewName
        {
            get
            {
                return "LookingForWork";
            }
        }

        // GET: wORK
        public ActionResult LookingForWork()
        {
            return View();
        }

       

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult UploadResume(FormCollection formCollection)
    {
        if (!base.ProcessUpload())
        {
            return RedirectToAction("MessageFailure", "Work");
        }
        return RedirectToAction("Thankyou", "Contact");
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult EmailFriend(EmailFriendModel model)
    {
        if (ModelState.IsValid)
        {
            string body = System.IO.File.ReadAllText(Server.MapPath("~/Template") + "/friendemail.html");
            body = string.Format(body, "<a href='" + ConfigurationManager.AppSettings["baseURL"].ToString() + "/Jobs/JobDescription/" + model.JobReference + ".shtml' >" + model.JobTitle + "</a>", "<a href='" + model.YourEmail + "' >" + model.YourEmail + "</a>");
            MailMessage message = new MailMessage
            {
                From = new MailAddress(model.YourEmail),
                Subject = "Take a look at this great job",
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add(model.FriendEmail);
            SmtpClient sc = new SmtpClient();
            sc.Host = ConfigurationManager.AppSettings["smtpHost"].ToString();
            string smtpUser = ConfigurationManager.AppSettings["smtpUserName"].ToString();
            string smtpPwd = ConfigurationManager.AppSettings["smtpPassword"].ToString();
            sc.Credentials = new System.Net.NetworkCredential(smtpUser, smtpPwd);
            sc.Send(message);

            return base.View("EmailAFriendSuccess");
        }

        return base.View("EmailAFriend", model);
    }

    public ActionResult ApplyOnline(string id)
    {
        JobApplyModel model = new JobApplyModel();
        Job obj = new Job();
        JobInfo info = obj.GetJobInfoByReferenceNo(id);
        model.JobId = info.JobId;
        model.JobTitle = info.Title;
        model.ReferenceNo = id;
        model.EssentialCriteriaList = obj.GetEssentialCriteria(info.JobId);
        model.DesirableCriteriaList = obj.GetDesirableCriteria(info.JobId);
        return base.View("JobApply", model);
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult ApplyJob(JobApplyModel model, FormCollection col)
    {
        Job obj = new Job();
        string body = System.IO.File.ReadAllText(Server.MapPath("~/Template") + "/jobApplyEmail.html");

        DataTable dtConsultant = new DataTable();
        dtConsultant = obj.GetJobConsultant(model.JobId);
        string consultant = string.Empty;
        foreach (DataRow drConsultant in dtConsultant.Rows)
        {
            consultant = drConsultant["first"].ToString() + ",";
        }

        if (!string.IsNullOrEmpty(consultant))
            consultant = consultant.Remove(consultant.Length - 1, 1);

        DataTable dtEss = new DataTable();
        dtEss = obj.GetEssentialCriteria(model.JobId);
        string essentialCriteria = "<tr><td width='180' align='left' colspan='2'><b>Essential Criteria:</b></td><td align='left'></td></tr>";
        if (dtEss.Rows.Count > 0)
        {
            foreach (DataRow dr in dtEss.Rows)
            {
                essentialCriteria = essentialCriteria + "<tr><td align='left' colspan='2'> &nbsp;&nbsp;&nbsp;<b>Q." + dr["description"].ToString() + "</b> </td></tr><tr><td align='left' colspan='2'> &nbsp;&nbsp;&nbsp;A." + col[dr["EssentialCriteriaId"].ToString()] + "</td></tr>";
            }
        }

        DataTable dtDesi = new DataTable();
        dtDesi = obj.GetDesirableCriteria(model.JobId);
        if (dtDesi.Rows.Count > 0)
        {
            string desirableCriteria = "<tr><td width='180' align='left' colspan='2'><b>Desirable Criteria:</b></td><td align='left'></td></tr>";
            foreach (DataRow dr in dtDesi.Rows)
            {
                desirableCriteria = desirableCriteria + "<tr><td align='left' colspan='2'> &nbsp;&nbsp;&nbsp;<b>Q." + dr["description"].ToString() + "</b> </td></tr><tr><td align='left' colspan='2'> &nbsp;&nbsp;&nbsp;A." + col[dr["DesirableCriteriaId"].ToString()] + "</td></tr>";
            }

            essentialCriteria = essentialCriteria + desirableCriteria;
        }
        body = string.Format(body, model.JobTitle, model.ReferenceNo, consultant, model.FirstName, model.LastName, model.Email, model.ContactNumber, string.IsNullOrEmpty(col["retentionConsentChkBox"]) ? "NO" : "YES", essentialCriteria);

        MailMessage message = new MailMessage
        {
            From = new MailAddress(model.Email, model.FirstName + " " + model.LastName),
            Subject = "Job Application: " + model.JobTitle + " (" + model.FirstName + " " + model.LastName + "), Ref:" + model.ReferenceNo,
            Body = body,
            IsBodyHtml = true
        };
        message.To.Add(ConfigurationManager.AppSettings["jobEmail"].ToString());

        if (model.Attachment != null && model.Attachment.ContentLength > 0)
        {
            var attachment = new Attachment(model.Attachment.InputStream, model.Attachment.FileName);
            message.Attachments.Add(attachment);
        }

        if (model.CoverLetterOptional != null && model.CoverLetterOptional.ContentLength > 0)
        {
            var attachment = new Attachment(model.CoverLetterOptional.InputStream, model.CoverLetterOptional.FileName);
            message.Attachments.Add(attachment);
        }
        else if (!string.IsNullOrEmpty(model.CoverLetter))
        {
            //byte[] data = Encoding.ASCII.GetBytes(model.CoverLetter);
            //MemoryStream stm = new MemoryStream(data, 0, data.Length);
            //var attachment = new Attachment(stm, "coverLetter.doc");
            //message.Attachments.Add(attachment);
            string coverLetter = "<tr><td width='180' align='left' colspan='2'><b>Cover letter:</b></td></tr>";
            coverLetter += "<tr><td align='left' colspan='2'> " + model.CoverLetter + "</b> </td></tr>";
            body = body + coverLetter;
            message.Body = body;
        }

        string ackBody = System.IO.File.ReadAllText(Server.MapPath("~/Template") + "/jobAcknowledgement.html");
        ackBody = string.Format(ackBody, "<img src='" + ConfigurationManager.AppSettings["logoURL"].ToString() + "' />", model.FirstName, model.JobTitle, model.ReferenceNo);
        MailMessage ackMssage = new MailMessage
        {
            From = new MailAddress(ConfigurationManager.AppSettings["joEmailFrom"].ToString()),
            Subject = "Application Acknowledgement For : " + model.JobTitle,
            Body = ackBody,
            IsBodyHtml = true
        };
        ackMssage.To.Add(model.Email);

        SmtpClient sc = new SmtpClient();
        sc.Host = ConfigurationManager.AppSettings["smtpHost"].ToString();
        string smtpUser = ConfigurationManager.AppSettings["smtpUserName"].ToString();
        string smtpPwd = ConfigurationManager.AppSettings["smtpPassword"].ToString();
        sc.Credentials = new System.Net.NetworkCredential(smtpUser, smtpPwd);
        sc.Send(message);
        sc.Send(ackMssage);
        return base.View("JobApplyConfirm");
    }

    public ActionResult JobApplyConfirm()
    {
        return base.View("JobApplyConfirm");
    }

    public ActionResult MessageFailure()
    {
        return base.View();
    }
}
}