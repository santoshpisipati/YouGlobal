using AjaxControlToolkit;
using GlobalPanda.BusinessInfo;
using GlobalPanda.DataProviders;
using MySql.Data.MySqlClient;
using PagedList;
using Sample.Web.ModalLogin.Classes;
using Sample.Web.ModalLogin.Controllers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using YG_Business;
using YG_DataAccess;
using YG_MVC.Models;

namespace YG_MVC.Controllers
{
    [HandleError]
    public class JobsController : BaseController
    {
        public List<OccupationCount> OccupationCountList
        {
            get;
            set;
        }

        public List<IndustryCount> IndustryCountList
        {
            get;
            set;
        }

        public override ActionResult Index()
        {
            return base.RedirectToRoute(new { controller = "Work", action = "LookingForWork" });
        }

        public ActionResult JobListing()
        {
            return this.List();
        }

        public ActionResult LookingForWork()
        {
            return this.Index();
        }

        public ActionResult JobAlert()
        {
            GetOccupationCount();
            GetIndustryCount();
            return base.View("JobAlert");
        }

        public ActionResult Search()
        {
            this.ShowHotJobs();
            return this.Index();
        }

        public ActionResult JobAlertSuccess()
        {
            return base.View("JobAlertSuccess");
        }

        public ActionResult JobAlertExist()
        {
            return base.View("JobAlertExist");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetOtherServices()
        {
            return base.View("OtherServices");
        }

        public ActionResult SubscribeOtherServices(FormCollection Data)
        {
            if (!base.ProcessUpload())
            {
                return RedirectToAction("MessageFailure", "Work");
            }
            return View("ThankyouOtherServices");
        }

        public ActionResult ThankyouOtherServices()
        {
            return base.View("ThankyouOtherServices");
        }

        public ActionResult JobSeeker()
        {
            this.ShowHotJobs();
            GetOccupationCount();
            GetIndustryCount();
            return base.View("JobSeeker");
        }

        public ActionResult JobSeekerEmp()
        {
            this.ShowHotJobs();
            GetOccupationCount();
            GetIndustryCount();
            return base.View("JobSeekerEmp");
        }

        public ActionResult UploadCv()
        {
            this.ShowHotJobs();
            return base.View("UploadCv");
        }

        public ActionResult JobAdvertising()
        {
            return base.View("JobAdvertising");
        }

        public ActionResult UnSignedEmployerRecruiter()
        {
            return base.View("UnSignedEmployerRecruiter");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadResume(FormCollection data)
        {
            if (!ProcessUpload())
            {
                return RedirectToAction("MessageFailure", "Work");
            }
            return View("Thankyou");
        }

        public ActionResult PostJob()
        {
            int memberId = !string.IsNullOrEmpty(Session["memberID"].ToString()) ? int.Parse(Session["memberID"].ToString()) : 0;
            JobDetailInfo jobDetailInfo = new JobDetailInfo();
            string JobCodePrefix = GetJobPrefix(memberId);
            if (!string.IsNullOrEmpty(JobCodePrefix))
            {
                string jobRefeCode = JobCodePrefix + DateTime.UtcNow.Year + JobDetailDataProvider.jobReferenceCode(JobCodePrefix + DateTime.UtcNow.Year);
                Session["ReferenceNo"] = jobRefeCode.ToUpper();
                WorkTypeList(); listJobType();
                listCurrencies(); listFrequency();
                jobDetailInfo.EssentialCriteriaList = new List<EssentialCriteriaInfo>();
                jobDetailInfo.SelectedLocationList = new List<string>();
                jobDetailInfo.AdFooter = "<p></p><p>To apply online, please click on the appropriate link below.</p>";
            }
            else
            {
                jobDetailInfo.EssentialCriteriaList = new List<EssentialCriteriaInfo>();
                jobDetailInfo.SelectedLocationList = new List<string>();
                jobDetailInfo.AdFooter = "<p></p><p>To apply online, please click on the appropriate link below.</p>";
                TempData["errorPostJob"] = "Your Job Ref Code Prefix is missing from the system. Please contact the system administrator.";//"Plase update your Job code prefix
            }
            return base.View("PostJob", jobDetailInfo);
        }

        public String GetJobPrefix(int memberId)
        {
            string prefix = ConsultantDataProvider.getJobcodePrefix(memberId);
            return prefix;
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult PostJob(JobDetailInfo model, string returnUrl)
        {
            if (ModelState.IsValid) { }

            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.ReferenceNo))
                {
                    int jobId = Save(model);
                    if (jobId > 0)
                    {
                        JobDetailDataProvider.updateJobApprove(jobId);
                        string referenceNo = model.ReferenceNo;
                        HistoryDataProvider history = new HistoryDataProvider();
                        HistoryInfo historyInfo = new HistoryInfo();
                        historyInfo.UserId = (Session["userID"] != null ? Convert.ToUInt32(Session["userID"]) : 0);
                        historyInfo.ModuleId = (int)HistoryInfo.Module.Job;
                        historyInfo.TypeId = (int)HistoryInfo.ActionType.Edit;
                        historyInfo.RecordId = Convert.ToUInt32(jobId);
                        historyInfo.Details = new List<HistoryDetailInfo>();
                        historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = "approve", NewValue = referenceNo });
                        history.insertHistory(historyInfo);
                        return View("PostJob", model);
                    }
                }
                else
                {
                    return View("PostJob", model);
                }
            }
            return View("PostJob", model);
        }

        public ActionResult OtherServices()
        {
            listNations();
            return base.View("OtherServices");
        }

        public DataTable listNations()
        {
            DataTable dt = new DataTable();
            MySqlDataReader reader = AddressDataProvider.listActiveCountries();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            DataRow dr = dt.NewRow();
            dr[0] = 0;
            dr[1] = "-- Any --";
            dt.Rows.InsertAt(dr, 0);
            Session.Add("Countries", dt);
            return dt;
        }

        //[AcceptVerbs(HttpVerbs.Post)]
        [HttpPost]
        public ActionResult Search(SearchCriteria searchCriteria)
        {
            Job objJob = new Job();
            base.Session["LastSearchCriteria"] = null;
            base.Session.Remove("LastSearchCriteria");
            base.Session["LastSearchCriteria"] = searchCriteria;
            var pageSize = 6;
            var items = objJob.SearchJobs(searchCriteria.Industry, searchCriteria.Role, searchCriteria.Location, searchCriteria.WorkArrangement, searchCriteria.Keywords, "");
            ViewBag.OnePageOfProducts = 1;
            PagedList<JobInfo> listitems = new PagedList<JobInfo>(items, 1, pageSize);
            return base.View("JobsListing", listitems);
            //return base.View("Index");
        }

        public JsonResult searchOccupationCount(string term)
        {
            List<string> result = new List<string>();
            MySqlDataReader drOccupation = CommonDataAccess.searchOccupation(term);
            List<Tier> treeViewResult = new List<Tier>();
            Tier1 Tier1 = new Tier1();
            Tier2 Tier2 = new Tier2();
            Tier3 Tier3 = new Tier3();
            Tier Tier = new Tier();
            Job jda = new Job();
            Int32 t1jobsCount = 0;
            Int32 t2jobsCount = 0;
            Int32 t4jobsCount = 0;
            Int32 t1id = 0;
            int total = 0;
            while (drOccupation.Read())
            {
                t4jobsCount = drOccupation["t4Count"].ToString() != null ? Convert.ToInt32(drOccupation["t4Count"].ToString()) : 0;
                if (!result.Contains("> " + drOccupation["c1"].ToString() + " " + drOccupation["d1"].ToString()))
                {
                    total = 0; total = total + t4jobsCount;
                    Tier = new Tier();
                    Tier.TierId = drOccupation["id1"].ToString() != null ? Convert.ToInt32(drOccupation["id1"].ToString()) : 0;
                    Tier.ParentMenuID = 0;
                    Tier.children = new List<Tier1>();
                    Tier.key = drOccupation["id1"].ToString() + " " + drOccupation["c1"].ToString();
                    Tier.title = string.Format("{0} ({1})", drOccupation["d1"].ToString(), total);
                    result.Add("> " + drOccupation["c1"].ToString() + " " + drOccupation["d1"].ToString());
                }
                else
                {
                    total = total + t4jobsCount; Tier.title = string.Format("{0} ({1})", drOccupation["d1"].ToString(), total);
                }

                if (!result.Contains(">> " + drOccupation["c2"].ToString() + " " + drOccupation["d2"].ToString()))
                {
                    if (Tier.children == null)
                    {
                        t1jobsCount = 0; t1jobsCount = t1jobsCount + t4jobsCount;
                        Tier.children = new List<Tier1>();
                        Tier1.title = string.Format("{0} ({1})", drOccupation["d2"].ToString(), t1jobsCount);
                        Tier1.Tier1Id = drOccupation["id2"].ToString() != null ? Convert.ToInt32(drOccupation["id2"].ToString()) : 0;
                        Tier1.key = drOccupation["id2"].ToString() + " " + drOccupation["c2"].ToString();
                        Tier1.parentString += "," + drOccupation["id1"].ToString();
                        Tier.children.Add(Tier1);
                    }
                    else
                    {
                        t1jobsCount = 0; t1jobsCount = t1jobsCount + t4jobsCount;
                        Tier1 = new Tier1();
                        Tier1.key = drOccupation["id2"].ToString() + " " + drOccupation["c2"].ToString();
                        Tier1.Tier1Id = drOccupation["id2"].ToString() != null ? Convert.ToInt32(drOccupation["id2"].ToString()) : 0;
                        Tier1.title = string.Format("{0} ({1})", drOccupation["d2"].ToString(), t1jobsCount);
                        Tier1.parentString += "," + drOccupation["id1"].ToString();
                        Tier.children.Add(Tier1);
                    }
                    result.Add(">> " + drOccupation["c2"].ToString() + " " + drOccupation["d2"].ToString());
                }
                else
                {
                    t1jobsCount = t1jobsCount + t4jobsCount; Tier1.title = string.Format("{0} ({1})", drOccupation["d2"].ToString(), t1jobsCount);
                }
                if (!result.Contains(">>> " + drOccupation["c3"].ToString() + " " + drOccupation["d3"].ToString()))
                {
                    string tree = drOccupation["d3"].ToString();
                    if (Tier1.children == null)
                    {
                        t2jobsCount = 0;
                        t2jobsCount = t4jobsCount;
                        Tier1.children = new List<Tier2>();
                        Tier2 = new Tier2();
                        Tier2.title = string.Format("{0} ({1})", drOccupation["d3"].ToString(), t2jobsCount);
                        Tier2.Tier2Id = drOccupation["id3"].ToString() != null ? Convert.ToInt32(drOccupation["id3"].ToString()) : 0;
                        Tier2.key = drOccupation["id3"].ToString() + " " + drOccupation["c3"].ToString();
                        Tier2.parentString += drOccupation["id1"].ToString() + "," + drOccupation["id2"].ToString();
                        Tier1.children.Add(Tier2);
                    }
                    else
                    {
                        t2jobsCount = t4jobsCount;
                        Tier2 = new Tier2();
                        Tier2.key = drOccupation["id3"].ToString() + " " + drOccupation["c3"].ToString();
                        Tier2.title = string.Format("{0} ({1})", drOccupation["d3"].ToString(), t2jobsCount);
                        Tier2.Tier2Id = drOccupation["id3"].ToString() != null ? Convert.ToInt32(drOccupation["id3"].ToString()) : 0;
                        Tier2.parentString += drOccupation["id1"].ToString() + "," + drOccupation["id2"].ToString();
                        Tier1.children.Add(Tier2);
                    }
                    result.Add(">>> " + drOccupation["c3"].ToString() + " " + drOccupation["d3"].ToString());
                }
                else
                {
                    t2jobsCount = t2jobsCount + t4jobsCount; Tier2.title = string.Format("{0} ({1})", drOccupation["d3"].ToString(), t2jobsCount);
                }
                if (t4jobsCount > 0)
                {
                    if (Tier2.children == null)
                    {
                        Tier2.children = new List<Tier3>();
                        Tier3 = new Tier3();
                        Tier3.title = string.Format("{0} ({1})", drOccupation["d4"].ToString(), t4jobsCount);
                        Tier3.key = drOccupation["id4"].ToString() + " " + drOccupation["c4"].ToString();
                        Tier3.parentString += drOccupation["id1"].ToString() + "," + drOccupation["id2"].ToString() + "," + drOccupation["id3"].ToString();
                        Tier2.children.Add(Tier3);
                    }
                    else
                    {
                        Tier3 = new Tier3();
                        Tier3.title = string.Format("{0} ({1})", drOccupation["d4"].ToString(), t4jobsCount);
                        Tier3.key = drOccupation["id4"].ToString() + " " + drOccupation["c4"].ToString();
                        Tier3.parentString += drOccupation["id1"].ToString() + "," + drOccupation["id2"].ToString() + "," + drOccupation["id3"].ToString();
                        if (Tier2.children == null)
                        {
                            Tier2.children = new List<Tier3>();
                            Tier2.children.Add(Tier3);
                        }
                        else
                        {
                            Tier2.children.Add(Tier3);
                        }
                        result.Add(">>>> " + drOccupation["c4"].ToString() + " " + drOccupation["d4"].ToString());
                    }
                }
                if (Tier.TierId != t1id)
                {
                    t1id = Tier.TierId;
                    treeViewResult.Add(Tier);
                }
            }
            drOccupation.Close();
            drOccupation.Dispose();
            Tier = new Tier();
            Tier.title = "- Any -";
            treeViewResult.Insert(0, Tier);
            return Json(treeViewResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult searchOccupation(string term)
        {
            List<string> result = new List<string>();
            MySqlDataReader drOccupation = CommonDataAccess.searchOccupation(term);
            List<Tier> treeViewResult = new List<Tier>();
            Tier1 Tier1 = new Tier1();
            Tier2 Tier2 = new Tier2();
            Tier3 Tier3 = new Tier3();
            Tier Tier = new Tier();
            Job jda = new Job();
            Int32 t1id = 0;
            while (drOccupation.Read())
            {
                if (!result.Contains("> " + drOccupation["c1"].ToString() + " " + drOccupation["d1"].ToString()))
                {
                    Tier = new Tier();
                    Tier.TierId = drOccupation["id1"].ToString() != null ? Convert.ToInt32(drOccupation["id1"].ToString()) : 0;
                    Tier.ParentMenuID = 0;
                    Tier.children = new List<Tier1>();
                    Tier.key = drOccupation["id1"].ToString() + " " + drOccupation["c1"].ToString();
                    Tier.title = drOccupation["d1"].ToString();
                    result.Add("> " + drOccupation["c1"].ToString() + " " + drOccupation["d1"].ToString());
                }

                if (!result.Contains(">> " + drOccupation["c2"].ToString() + " " + drOccupation["d2"].ToString()))
                {
                    if (Tier.children == null)
                    {
                        Tier.children = new List<Tier1>();
                        Tier1.title = drOccupation["d2"].ToString();
                        Tier1.Tier1Id = drOccupation["id21"].ToString() != null ? Convert.ToInt32(drOccupation["id2"].ToString()) : 0;
                        Tier1.key = drOccupation["id2"].ToString() + " " + drOccupation["c2"].ToString();
                        Tier.children.Add(Tier1);
                    }
                    else
                    {
                        Tier1 = new Tier1();
                        Tier1.key = drOccupation["id2"].ToString() + " " + drOccupation["c2"].ToString();
                        Tier1.Tier1Id = drOccupation["id2"].ToString() != null ? Convert.ToInt32(drOccupation["id2"].ToString()) : 0;
                        Tier1.title = drOccupation["d2"].ToString();
                        Tier.children.Add(Tier1);
                    }
                    result.Add(">> " + drOccupation["c2"].ToString() + " " + drOccupation["d2"].ToString());
                }

                if (!result.Contains(">>> " + drOccupation["c3"].ToString() + " " + drOccupation["d3"].ToString()))
                {
                    if (Tier1.children == null)
                    {
                        Tier1.children = new List<Tier2>();
                        Tier2 = new Tier2();
                        Tier2.title = drOccupation["d3"].ToString();
                        Tier2.Tier2Id = drOccupation["id3"].ToString() != null ? Convert.ToInt32(drOccupation["id3"].ToString()) : 0;
                        Tier2.key = drOccupation["id3"].ToString() + " " + drOccupation["c3"].ToString();
                        Tier1.children.Add(Tier2);
                    }
                    else
                    {
                        Tier2 = new Tier2();
                        Tier2.key = drOccupation["id3"].ToString() + " " + drOccupation["c3"].ToString();
                        Tier2.title = drOccupation["d3"].ToString();
                        Tier2.Tier2Id = drOccupation["id3"].ToString() != null ? Convert.ToInt32(drOccupation["id3"].ToString()) : 0;
                        Tier1.children.Add(Tier2);
                    }
                    result.Add(">>> " + drOccupation["c3"].ToString() + " " + drOccupation["d3"].ToString());
                }
               

                if (Tier2.children == null)
                {
                    Tier2.children = new List<Tier3>();
                    Tier3 = new Tier3();
                    Tier3.title = drOccupation["d4"].ToString();
                    Tier3.key = drOccupation["id4"].ToString() + " " + drOccupation["c4"].ToString();
                    Tier2.children.Add(Tier3);
                }
                else
                {
                    Tier3 = new Tier3();
                    Tier3.title = drOccupation["d4"].ToString();
                    Tier3.key = drOccupation["id4"].ToString() + " " + drOccupation["c4"].ToString();
                    if (Tier2.children == null)
                    {
                        Tier2.children = new List<Tier3>();
                        Tier2.children.Add(Tier3);
                    }
                    else
                    {
                        Tier2.children.Add(Tier3);
                    }
                    result.Add(">>>> " + drOccupation["c4"].ToString() + " " + drOccupation["d4"].ToString());
                }
                if (Tier.TierId != t1id)
                {
                    t1id = Tier.TierId;
                    treeViewResult.Add(Tier);
                }
            }
            drOccupation.Close();
            drOccupation.Dispose();
            Tier = new Tier();
            Tier.title = "- Any -";
            treeViewResult.Insert(0, Tier);
            return Json(treeViewResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult searchIndustryCount(string term)
        {
            List<string> result = new List<string>();
            MySqlDataReader drIndustry = CommonDataAccess.searchIndustry(term);
            List<Tier> treeViewResult = new List<Tier>();
            Tier1 Tier1 = new Tier1();
            Tier2 Tier2 = new Tier2();
            Tier3 Tier3 = new Tier3();
            Tier Tier = new Tier();
            Job jda = new Job();
            Int32 t1jobsCount = 0;
            Int32 t2jobsCount = 0;
            Int32 t4jobsCount = 0;
            Int32 t1id = 0;
            int total = 0;
            while (drIndustry.Read())
            {
                t4jobsCount = drIndustry["Total"].ToString() != null ? Convert.ToInt32(drIndustry["Total"].ToString()) : 0;
                if (!result.Contains("> " + drIndustry["c1"].ToString() + " " + drIndustry["d1"].ToString()))
                {
                    total = 0; total = total + t4jobsCount;
                    Tier = new Tier();
                    Tier.TierId = drIndustry["id1"].ToString() != null ? Convert.ToInt32(drIndustry["id1"].ToString()) : 0;
                    Tier.ParentMenuID = 0;
                    Tier.children = new List<Tier1>();
                    Tier.key = drIndustry["id1"].ToString() + " " + drIndustry["c1"].ToString();
                    Tier.title = string.Format("{0} ({1})", drIndustry["d1"].ToString(), total);
                    result.Add("> " + drIndustry["c1"].ToString() + " " + drIndustry["d1"].ToString());
                }
                else
                {
                    total = total + t4jobsCount; Tier.title = string.Format("{0} ({1})", drIndustry["d1"].ToString(), total);
                }

                if (!result.Contains(">> " + drIndustry["c2"].ToString() + " " + drIndustry["d2"].ToString()))
                {
                    if (Tier.children == null)
                    {
                        t1jobsCount = 0; t1jobsCount = t1jobsCount + t4jobsCount;
                        Tier.children = new List<Tier1>();
                        Tier1.title = string.Format("{0} ({1})", drIndustry["d2"].ToString(), t1jobsCount);
                        Tier1.Tier1Id = drIndustry["id21"].ToString() != null ? Convert.ToInt32(drIndustry["id2"].ToString()) : 0;
                        Tier1.key = drIndustry["id2"].ToString() + " " + drIndustry["c2"].ToString();
                        Tier.children.Add(Tier1);
                    }
                    else
                    {
                        t1jobsCount = 0; t1jobsCount = t1jobsCount + t4jobsCount;
                        Tier1 = new Tier1();
                        Tier1.key = drIndustry["id2"].ToString() + " " + drIndustry["c2"].ToString();
                        Tier1.Tier1Id = drIndustry["id2"].ToString() != null ? Convert.ToInt32(drIndustry["id2"].ToString()) : 0;
                        Tier1.title = string.Format("{0} ({1})", drIndustry["d2"].ToString(), t1jobsCount);
                        Tier.children.Add(Tier1);
                    }
                    result.Add(">> " + drIndustry["c2"].ToString() + " " + drIndustry["d2"].ToString());
                }
                else
                {
                    t1jobsCount = t1jobsCount + t4jobsCount; Tier1.title = string.Format("{0} ({1})", drIndustry["d2"].ToString(), t1jobsCount);
                }
                if (!result.Contains(">>> " + drIndustry["c3"].ToString() + " " + drIndustry["d3"].ToString()))
                {
                    string tree = drIndustry["d3"].ToString();
                    if (Tier1.children == null)
                    {
                        t2jobsCount = 0;
                        t2jobsCount = t4jobsCount;
                        Tier1.children = new List<Tier2>();
                        Tier2 = new Tier2();
                        Tier2.title = string.Format("{0} ({1})", drIndustry["d3"].ToString(), t2jobsCount);
                        Tier2.Tier2Id = drIndustry["id3"].ToString() != null ? Convert.ToInt32(drIndustry["id3"].ToString()) : 0;
                        Tier2.key = drIndustry["id3"].ToString() + " " + drIndustry["c3"].ToString();
                        Tier1.children.Add(Tier2);
                    }
                    else
                    {
                        t2jobsCount = t4jobsCount;
                        Tier2 = new Tier2();
                        Tier2.key = drIndustry["id3"].ToString() + " " + drIndustry["c3"].ToString();
                        Tier2.title = string.Format("{0} ({1})", drIndustry["d3"].ToString(), t2jobsCount);
                        Tier2.Tier2Id = drIndustry["id3"].ToString() != null ? Convert.ToInt32(drIndustry["id3"].ToString()) : 0;
                        Tier1.children.Add(Tier2);
                    }
                    result.Add(">>> " + drIndustry["c3"].ToString() + " " + drIndustry["d3"].ToString());
                }
                else
                {
                    t2jobsCount = t2jobsCount + t4jobsCount; Tier2.title = string.Format("{0} ({1})", drIndustry["d3"].ToString(), t2jobsCount);
                }
                if (t4jobsCount > 0)
                {
                    if (Tier2.children == null)
                    {
                        Tier2.children = new List<Tier3>();
                        Tier3 = new Tier3();
                        Tier3.title = string.Format("{0} ({1})", drIndustry["d4"].ToString(), t4jobsCount);
                        Tier3.key = drIndustry["id4"].ToString() + " " + drIndustry["c4"].ToString();
                        Tier2.children.Add(Tier3);
                    }
                    else
                    {
                        Tier3 = new Tier3();
                        Tier3.title = string.Format("{0} ({1})", drIndustry["d4"].ToString(), t4jobsCount);
                        Tier3.key = drIndustry["id4"].ToString() + " " + drIndustry["c4"].ToString();
                        if (Tier2.children == null)
                        {
                            Tier2.children = new List<Tier3>();
                            Tier2.children.Add(Tier3);
                        }
                        else
                        {
                            Tier2.children.Add(Tier3);
                        }
                        result.Add(">>>> " + drIndustry["c4"].ToString() + " " + drIndustry["d4"].ToString());
                    }
                }
                if (Tier.TierId != t1id)
                {
                    t1id = Tier.TierId;
                    treeViewResult.Add(Tier);
                }
            }
            drIndustry.Close();
            drIndustry.Dispose();
            Tier = new Tier();
            Tier.title = "- Any -";
            treeViewResult.Insert(0, Tier);
            return Json(treeViewResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult searchIndustry(string term)
        {
            List<string> result = new List<string>();
            MySqlDataReader drIndustry = CommonDataAccess.searchIndustry(term);
            List<Tier> treeViewResult = new List<Tier>();
            Tier1 Tier1 = new Tier1();
            Tier2 Tier2 = new Tier2();
            Tier3 Tier3 = new Tier3();
            string menuName = string.Empty;
            int ParentMenuID = 0;
            List<IndustryCount> list = new List<IndustryCount>();
            Tier Tier = new Tier();
            while (drIndustry.Read())
            {
                if (!result.Contains("> " + drIndustry["c1"].ToString() + " " + drIndustry["d1"].ToString()))
                {
                    Tier = new Tier();
                    Tier.TierId = ParentMenuID;
                    Tier.ParentMenuID = 0;
                    Tier.children = new List<Tier1>();
                    Tier.key = drIndustry["id1"].ToString() + " " + drIndustry["c1"].ToString();
                    Tier.title = drIndustry["d1"].ToString();
                    result.Add("> " + drIndustry["c1"].ToString() + " " + drIndustry["d1"].ToString());
                }
                if (!result.Contains(">> " + drIndustry["c2"].ToString() + " " + drIndustry["d2"].ToString()))
                {
                    if (Tier.children == null)
                    {
                        Tier.children = new List<Tier1>();
                        Tier1.title = drIndustry["d2"].ToString();
                        Tier1.key = drIndustry["id2"].ToString() + " " + drIndustry["c2"].ToString() + " " + drIndustry["c1"].ToString();
                        Tier.children.Add(Tier1);
                    }
                    else
                    {
                        Tier1 = new Tier1();
                        Tier1.key = drIndustry["id2"].ToString() + " " + drIndustry["c2"].ToString() + " " + drIndustry["c1"].ToString();
                        Tier1.title = drIndustry["d2"].ToString();
                        Tier.children.Add(Tier1);
                    }
                    result.Add(">> " + drIndustry["c2"].ToString() + " " + drIndustry["d2"].ToString());
                }
                if (!result.Contains(">>> " + drIndustry["c3"].ToString() + " " + drIndustry["d3"].ToString()))
                {
                    if (Tier1.children == null)
                    {
                        Tier1.children = new List<Tier2>();
                        Tier2 = new Tier2();
                        Tier2.key = drIndustry["id3"].ToString() + " " + drIndustry["c3"].ToString() + " " + drIndustry["c1"].ToString();
                        Tier2.title = drIndustry["d3"].ToString();
                        Tier1.children.Add(Tier2);
                    }
                    else
                    {
                        Tier2 = new Tier2();
                        Tier2.key = drIndustry["id3"].ToString() + " " + drIndustry["c3"].ToString() + " " + drIndustry["c1"].ToString();
                        Tier2.title = drIndustry["d3"].ToString();
                        if (Tier2.children == null)
                        {
                            Tier1.children = new List<Tier2>();
                            Tier1.children.Add(Tier2);
                        }
                        else
                        {
                            Tier1.children.Add(Tier2);
                        }
                    }
                    result.Add(">>> " + drIndustry["c3"].ToString() + " " + drIndustry["d3"].ToString());
                }
                if (Tier2.children == null)
                {
                    Tier2.children = new List<Tier3>();
                    Tier3 = new Tier3();
                    Tier3.title = drIndustry["d4"].ToString();
                    Tier3.key = drIndustry["id4"].ToString() + " " + drIndustry["c4"].ToString() + " " + drIndustry["c1"].ToString();
                    Tier2.children.Add(Tier3);
                }
                else
                {
                    Tier3 = new Tier3();
                    Tier3.title = drIndustry["d4"].ToString();
                    Tier3.key = drIndustry["id4"].ToString() + " " + drIndustry["c4"].ToString() + " " + drIndustry["c1"].ToString();
                    if (Tier2.children == null)
                    {
                        Tier2.children = new List<Tier3>();
                        Tier2.children.Add(Tier3);
                    }
                    else
                    {
                        Tier2.children.Add(Tier3);
                    }
                    result.Add(">>>> " + drIndustry["c4"].ToString() + " " + drIndustry["d4"].ToString());
                }
                if (Tier.title != menuName)
                {
                    menuName = Tier.title;
                    treeViewResult.Add(Tier);
                }
            }
            drIndustry.Close();
            drIndustry.Dispose();
            Tier = new Tier();
            Tier.title = "- Any -";
            treeViewResult.Insert(0, Tier);
            return Json(treeViewResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchIndustryWithAny(string term)
        {
            List<string> result = new List<string>();
            MySqlDataReader drIndustry = CommonDataAccess.searchIndustry(term);
            List<Tier> treeViewResult = new List<Tier>();
            Tier1 Tier1 = new Tier1();
            Tier2 Tier2 = new Tier2();
            Tier3 Tier3 = new Tier3();
            string menuName = string.Empty;
            int ParentMenuID = 0;
            List<IndustryCount> list = new List<IndustryCount>();
            Tier Tier = new Tier();
            while (drIndustry.Read())
            {
                if (!result.Contains("> " + drIndustry["c1"].ToString() + " " + drIndustry["d1"].ToString()))
                {
                    Tier = new Tier();
                    Tier.TierId = ParentMenuID;
                    Tier.ParentMenuID = 0;
                    Tier.children = new List<Tier1>();
                    Tier.key = drIndustry["id1"].ToString() + " " + drIndustry["c1"].ToString();
                    Tier.title = drIndustry["d1"].ToString();
                    result.Add("> " + drIndustry["c1"].ToString() + " " + drIndustry["d1"].ToString());
                }
                if (!result.Contains(">> " + drIndustry["c2"].ToString() + " " + drIndustry["d2"].ToString()))
                {
                    if (Tier.children == null)
                    {
                        Tier.children = new List<Tier1>();
                        Tier1.title = drIndustry["d2"].ToString();
                        Tier1.key = drIndustry["id2"].ToString() + " " + drIndustry["c2"].ToString() + " " + drIndustry["c1"].ToString();
                        Tier.children.Add(Tier1);
                    }
                    else
                    {
                        Tier1 = new Tier1();
                        Tier1.key = drIndustry["id2"].ToString() + " " + drIndustry["c2"].ToString() + " " + drIndustry["c1"].ToString();
                        Tier1.title = drIndustry["d2"].ToString();
                        Tier.children.Add(Tier1);
                    }
                    result.Add(">> " + drIndustry["c2"].ToString() + " " + drIndustry["d2"].ToString());
                }
                if (!result.Contains(">>> " + drIndustry["c3"].ToString() + " " + drIndustry["d3"].ToString()))
                {
                    if (Tier1.children == null)
                    {
                        Tier1.children = new List<Tier2>();
                        Tier2 = new Tier2();
                        Tier2.key = drIndustry["id3"].ToString() + " " + drIndustry["c3"].ToString() + " " + drIndustry["c1"].ToString();
                        Tier2.title = drIndustry["d3"].ToString();
                        Tier1.children.Add(Tier2);
                    }
                    else
                    {
                        Tier2 = new Tier2();
                        Tier2.key = drIndustry["id3"].ToString() + " " + drIndustry["c3"].ToString() + " " + drIndustry["c1"].ToString();
                        Tier2.title = drIndustry["d3"].ToString();
                        if (Tier2.children == null)
                        {
                            Tier1.children = new List<Tier2>();
                            Tier1.children.Add(Tier2);
                        }
                        else
                        {
                            Tier1.children.Add(Tier2);
                        }
                    }
                    result.Add(">>> " + drIndustry["c3"].ToString() + " " + drIndustry["d3"].ToString());
                }
                if (Tier2.children == null)
                {
                    Tier2.children = new List<Tier3>();
                    Tier3 = new Tier3();
                    Tier3.title = drIndustry["d4"].ToString();
                    Tier3.key = drIndustry["id4"].ToString() + " " + drIndustry["c4"].ToString() + " " + drIndustry["c1"].ToString();
                    Tier2.children.Add(Tier3);
                }
                else
                {
                    Tier3 = new Tier3();
                    Tier3.title = drIndustry["d4"].ToString();
                    Tier3.key = drIndustry["id4"].ToString() + " " + drIndustry["c4"].ToString() + " " + drIndustry["c1"].ToString();
                    if (Tier2.children == null)
                    {
                        Tier2.children = new List<Tier3>();
                        Tier2.children.Add(Tier3);
                    }
                    else
                    {
                        Tier2.children.Add(Tier3);
                    }
                    result.Add(">>>> " + drIndustry["c4"].ToString() + " " + drIndustry["d4"].ToString());
                }
                if (Tier.title != menuName)
                {
                    menuName = Tier.title;
                    treeViewResult.Add(Tier);
                }
            }
            drIndustry.Close();
            drIndustry.Dispose();
            Tier = new Tier();
            Tier.title = "- Any -";
            treeViewResult.Insert(0, Tier);

            return Json(treeViewResult, JsonRequestBehavior.AllowGet);
        }

        public List<OccupationCount> GetOccupationCount()
        {
            OccupationCountList = new List<OccupationCount>();
            List<OccupationCount> DummyCountList = new List<OccupationCount>();
            Job job = new Job();
            MySqlDataReader drOccupationCount = job.GetJobsCountByOccupation();
            if (drOccupationCount.HasRows)
            {
                while (drOccupationCount.Read())
                {
                    OccupationCount occupationCount = new OccupationCount();
                    if (!string.IsNullOrEmpty(drOccupationCount["jobdetailid"].ToString()))
                        occupationCount.Id = Convert.ToInt32(drOccupationCount["jobdetailid"].ToString());
                    else
                        occupationCount.Id = 0;
                    //occupationCount.Id = Convert.ToInt32(drOccupationCount["jobdetailid"].ToString());
                    if (!string.IsNullOrEmpty(drOccupationCount["isco08id"].ToString()))
                        occupationCount.Count = Convert.ToInt32(drOccupationCount["isco08id"].ToString());
                    else
                        occupationCount.Count = 0;
                    DummyCountList.Add(occupationCount);
                }
            }
            OccupationCountList = DummyCountList;
            Session["OccupationCountList"] = OccupationCountList;
            drOccupationCount.Close();
            drOccupationCount.Dispose();
            return OccupationCountList;
        }

        public List<IndustryCount> GetIndustryCount()
        {
            IndustryCountList = new List<IndustryCount>();
            Job job = new Job();
            List<IndustryCount> DummyCountList = new List<IndustryCount>();
            MySqlDataReader drIndustryCount = job.GetJobsCountByIndustry();
            if (drIndustryCount.HasRows)
            {
                while (drIndustryCount.Read())
                {
                    IndustryCount industryCount = new IndustryCount();
                    if (!string.IsNullOrEmpty(drIndustryCount["jobdetailid"].ToString()))
                        industryCount.Id = Convert.ToInt32(drIndustryCount["jobdetailid"].ToString());
                    else
                        industryCount.Id = 0;
                    if (!string.IsNullOrEmpty(drIndustryCount["isicRev4id"].ToString()))
                        industryCount.Count = Convert.ToInt32(drIndustryCount["isicRev4id"].ToString());
                    else
                        industryCount.Count = 0;

                    DummyCountList.Add(industryCount);
                }
            }
            IndustryCountList = DummyCountList;
            Session["IndustryCountList"] = IndustryCountList;
            drIndustryCount.Close();
            drIndustryCount.Dispose();
            return IndustryCountList;
        }

        public JsonResult SearchLocationWithId(string term)
        {
            KeyValuePair<string, string> kv = new KeyValuePair<string, string>();
            List<KeyValuePair<string, string>> lst = new List<KeyValuePair<string, string>>();
            List<string> result = new List<string>();
            MySqlDataReader drLocation = CommonDataAccess.searchLocation(term);
            List<Tier> treeViewResult = new List<Tier>();
            Tier1 subItem = new Tier1();
            Tier2 subSubItem = new Tier2();
            Tier3 subSubSubItem = new Tier3();
            Tier4 subSubSubSubItem = new Tier4();
            Tier sitemenu = new Tier();
            string menuName = string.Empty;
            int ParentMenuID = 0;
            while (drLocation.Read())
            {
                if (!string.IsNullOrEmpty(drLocation["name"].ToString()))
                {
                    if (!result.Contains("<span style='color:#9AC435'> > " + drLocation["name"].ToString() + "</span>"))
                    {
                        sitemenu = new Tier();
                        sitemenu.TierId = ParentMenuID;
                        sitemenu.ParentMenuID = 0;
                        sitemenu.children = new List<Tier1>();
                        sitemenu.key = drLocation["countryid"].ToString() + ":1";
                        sitemenu.title = drLocation["name"].ToString();
                        result.Add("<span style='color:#9AC435'> > " + drLocation["name"].ToString() + "</span>");
                        kv = new KeyValuePair<string, string>(drLocation["countryid"].ToString() + ":1", "<span style='color:#9AC435'> > " + drLocation["name"].ToString() + "</span>");
                        lst.Add(kv);
                    }
                }
                if (!string.IsNullOrEmpty(drLocation["locationname"].ToString()))
                {
                    if (!result.Contains("<span style='color:#124812'> >> " + drLocation["locationname"].ToString() + "</span>"))
                    {
                        if (sitemenu.children == null)
                        {
                            sitemenu.children = new List<Tier1>();
                            subItem.title = drLocation["locationname"].ToString();
                            subItem.key = drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + ":2";
                            sitemenu.children.Add(subItem);
                        }
                        else
                        {
                            subItem = new Tier1();
                            subItem.key = drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + ":2";
                            subItem.title = drLocation["locationname"].ToString();
                            sitemenu.children.Add(subItem);
                        }
                        result.Add("<span style='color:#124812'> >> " + drLocation["locationname"].ToString() + "</span>");
                        kv = new KeyValuePair<string, string>(drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + ":2", "<span style='color:#124812'> >> " + drLocation["locationname"].ToString() + "</span>");
                        lst.Add(kv);
                    }
                }
                if (!string.IsNullOrEmpty(drLocation["sublocation"].ToString()))
                {
                    if (!result.Contains("<span style='color:#51C0EE'> >>> " + drLocation["sublocation"].ToString() + "</span>"))
                    {
                        if (subItem.children == null)
                        {
                            subItem.children = new List<Tier2>();
                            subSubItem = new Tier2();
                            subSubItem.key = drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + ":3";
                            subSubItem.title = drLocation["sublocation"].ToString();
                            subItem.children.Add(subSubItem);
                        }
                        else
                        {
                            subSubItem = new Tier2();
                            subSubItem.key = drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + ":3";
                            subSubItem.title = drLocation["sublocation"].ToString();
                            if (subSubItem.children == null)
                            {
                                subItem.children = new List<Tier2>();
                                subItem.children.Add(subSubItem);
                            }
                            else
                            {
                                subItem.children.Add(subSubItem);
                            }
                        }
                        result.Add("<span style='color:#51C0EE'> >>> " + drLocation["sublocation"].ToString() + "</span>");
                        kv = new KeyValuePair<string, string>(drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + ":3", "<span style='color:#51C0EE'> >>> " + drLocation["sublocation"].ToString() + "</span>");
                        lst.Add(kv);
                    }
                }
                if (!string.IsNullOrEmpty(drLocation["subsublocation"].ToString()))
                {
                    if (!result.Contains("<span style='color:#0000FF'> >>>> " + drLocation["subsublocation"].ToString() + "</span>"))
                    {
                        if (subSubItem.children == null)
                        {
                            subSubItem.children = new List<Tier3>();
                            subSubSubItem = new Tier3();
                            subSubSubItem.title = drLocation["subsublocation"].ToString();
                            subSubSubItem.key = drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + "," + drLocation["subsublocationid"].ToString() + ":4";
                            subSubItem.children.Add(subSubSubItem);
                        }
                        else
                        {
                            subSubSubItem = new Tier3();
                            subSubSubItem.title = drLocation["subsublocation"].ToString();
                            subSubSubItem.key = drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + "," + drLocation["subsublocationid"].ToString() + ":4";
                            if (subSubItem.children == null)
                            {
                                subSubItem.children = new List<Tier3>();
                                subSubItem.children.Add(subSubSubItem);
                            }
                            else
                            {
                                subSubItem.children.Add(subSubSubItem);
                            }
                            result.Add("<span style='color:#0000FF'> >>>> " + drLocation["subsublocation"].ToString() + "</span>");
                            kv = new KeyValuePair<string, string>(drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + "," + drLocation["subsublocationid"].ToString() + ":4", "<span style='color:#0000FF'> >>>> " + drLocation["subsublocation"].ToString() + "</span>");
                            lst.Add(kv);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(drLocation["groupname"].ToString()))
                {
                    if (!result.Contains("<span style='color:#0000FF'> >>>> " + drLocation["groupname"].ToString() + "</span>"))
                    {
                        if (subSubSubItem.Nodes == null)
                        {
                            subSubSubItem.Nodes = new List<Tier4>();
                            subSubSubSubItem = new Tier4();
                            subSubSubSubItem.title = drLocation["groupname"].ToString();
                            subSubSubSubItem.key = drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + "," + drLocation["subsublocationid"].ToString() + "," + drLocation["location_groupid"].ToString() + ":5";
                            subSubSubItem.Nodes.Add(subSubSubSubItem);
                        }
                        else
                        {
                            subSubSubSubItem = new Tier4();
                            subSubSubSubItem.title = drLocation["groupname"].ToString();
                            subSubSubSubItem.key = drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + "," + drLocation["subsublocationid"].ToString() + "," + drLocation["location_groupid"].ToString() + ":5";
                            if (subSubSubItem.Nodes == null)
                            {
                                subSubSubItem.Nodes = new List<Tier4>();
                                subSubSubItem.Nodes.Add(subSubSubSubItem);
                            }
                            else
                            {
                                subSubSubItem.Nodes.Add(subSubSubSubItem);
                            }
                            result.Add("<span style='color:#0000FF'> >>>> " + drLocation["groupname"].ToString() + "</span>");
                            kv = new KeyValuePair<string, string>(drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + "," + drLocation["subsublocationid"].ToString() + "," + drLocation["location_groupid"].ToString() + ":5", "<span style='color:#0000FF'> >>>> " + drLocation["subsublocation"].ToString() + "</span>");
                            lst.Add(kv);
                        }
                    }
                }
                if (sitemenu.title != menuName)
                {
                    menuName = sitemenu.title;
                    if (!string.IsNullOrEmpty(sitemenu.title) && sitemenu.title != " ")
                    {
                        treeViewResult.Add(sitemenu);
                    }
                }
            }
            sitemenu = new Tier();
            sitemenu.title = "- Anywhere -";
            treeViewResult.Insert(0, sitemenu);
            drLocation.Close();
            drLocation.Dispose();
            kv = new KeyValuePair<string, string>("0", "<span>- Any -</span>");
            lst.Insert(0, kv);
            return Json(treeViewResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchEmployer(string term)
        {
            KeyValuePair<string, string> kv = new KeyValuePair<string, string>();
            List<KeyValuePair<string, string>> lst = new List<KeyValuePair<string, string>>();
            MySqlDataReader drEmployer = CommonDataAccess.SearchEmployer(term);
            List<Tier> treeViewResult = new List<Tier>();
            while (drEmployer.Read())
            {
                Tier sitemenu = new Tier();
                sitemenu.key = drEmployer["employerid"].ToString();
                sitemenu.title = drEmployer["employername"].ToString();
                sitemenu.children = null;
                new KeyValuePair<string, string>(drEmployer["employerid"].ToString(), drEmployer["employername"].ToString());
                treeViewResult.Add(sitemenu);
            }
            drEmployer.Close();
            drEmployer.Dispose();
            return Json(treeViewResult, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SearchLocationWithOutAny(string term)
        {
            KeyValuePair<string, string> kv = new KeyValuePair<string, string>();
            List<KeyValuePair<string, string>> lst = new List<KeyValuePair<string, string>>();
            List<string> result = new List<string>();
            MySqlDataReader drLocation = CommonDataAccess.searchLocation(term);
            List<Tier> treeViewResult = new List<Tier>();
            Tier1 subItem = new Tier1();
            Tier2 subSubItem = new Tier2();
            Tier3 subSubSubItem = new Tier3();
            Tier4 subSubSubSubItem = new Tier4();
            Tier sitemenu = new Tier();
            string menuName = string.Empty;
            int ParentMenuID = 0;
            while (drLocation.Read())
            {
                if (!string.IsNullOrEmpty(drLocation["name"].ToString()))
                {
                    if (!result.Contains("<span style='color:#9AC435'> > " + drLocation["name"].ToString() + "</span>"))
                    {
                        sitemenu = new Tier();
                        sitemenu.TierId = ParentMenuID;
                        sitemenu.ParentMenuID = 0;
                        sitemenu.children = new List<Tier1>();
                        sitemenu.key = drLocation["countryid"].ToString() + ":1";
                        sitemenu.title = drLocation["name"].ToString();
                        result.Add("<span style='color:#9AC435'> > " + drLocation["name"].ToString() + "</span>");
                        kv = new KeyValuePair<string, string>(drLocation["countryid"].ToString() + ":1", "<span style='color:#9AC435'> > " + drLocation["name"].ToString() + "</span>");
                        lst.Add(kv);
                    }
                }
                if (!string.IsNullOrEmpty(drLocation["locationname"].ToString()))
                {
                    if (!result.Contains("<span style='color:#124812'> >> " + drLocation["locationname"].ToString() + "</span>"))
                    {
                        if (sitemenu.children == null)
                        {
                            sitemenu.children = new List<Tier1>();
                            subItem.title = drLocation["locationname"].ToString();
                            subItem.key = drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + ":2";
                            sitemenu.children.Add(subItem);
                        }
                        else
                        {
                            subItem = new Tier1();
                            subItem.key = drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + ":2";
                            subItem.title = drLocation["locationname"].ToString();
                            sitemenu.children.Add(subItem);
                        }
                        result.Add("<span style='color:#124812'> >> " + drLocation["locationname"].ToString() + "</span>");
                        kv = new KeyValuePair<string, string>(drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + ":2", "<span style='color:#124812'> >> " + drLocation["locationname"].ToString() + "</span>");
                        lst.Add(kv);
                    }
                }
                if (!string.IsNullOrEmpty(drLocation["sublocation"].ToString()))
                {
                    if (!result.Contains("<span style='color:#51C0EE'> >>> " + drLocation["sublocation"].ToString() + "</span>"))
                    {
                        if (subItem.children == null)
                        {
                            subItem.children = new List<Tier2>();
                            subSubItem = new Tier2();
                            subSubItem.key = drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + ":3";
                            subSubItem.title = drLocation["sublocation"].ToString();
                            subItem.children.Add(subSubItem);
                        }
                        else
                        {
                            subSubItem = new Tier2();
                            subSubItem.key = drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + ":3";
                            subSubItem.title = drLocation["sublocation"].ToString();
                            if (subSubItem.children == null)
                            {
                                subItem.children = new List<Tier2>();
                                subItem.children.Add(subSubItem);
                            }
                            else
                            {
                                subItem.children.Add(subSubItem);
                            }
                        }
                        result.Add("<span style='color:#51C0EE'> >>> " + drLocation["sublocation"].ToString() + "</span>");
                        kv = new KeyValuePair<string, string>(drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + ":3", "<span style='color:#51C0EE'> >>> " + drLocation["sublocation"].ToString() + "</span>");
                        lst.Add(kv);
                    }
                }
                if (!string.IsNullOrEmpty(drLocation["subsublocation"].ToString()))
                {
                    if (!result.Contains("<span style='color:#0000FF'> >>>> " + drLocation["subsublocation"].ToString() + "</span>"))
                    {
                        if (subSubItem.children == null)
                        {
                            subSubItem.children = new List<Tier3>();
                            subSubSubItem = new Tier3();
                            subSubSubItem.title = drLocation["subsublocation"].ToString();
                            subSubSubItem.key = drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + "," + drLocation["subsublocationid"].ToString() + ":4";
                            subSubItem.children.Add(subSubSubItem);
                        }
                        else
                        {
                            subSubSubItem = new Tier3();
                            subSubSubItem.title = drLocation["subsublocation"].ToString();
                            subSubSubItem.key = drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + "," + drLocation["subsublocationid"].ToString() + ":4";
                            if (subSubItem.children == null)
                            {
                                subSubItem.children = new List<Tier3>();
                                subSubItem.children.Add(subSubSubItem);
                            }
                            else
                            {
                                subSubItem.children.Add(subSubSubItem);
                            }
                            result.Add("<span style='color:#0000FF'> >>>> " + drLocation["subsublocation"].ToString() + "</span>");
                            kv = new KeyValuePair<string, string>(drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + "," + drLocation["subsublocationid"].ToString() + ":4", "<span style='color:#0000FF'> >>>> " + drLocation["subsublocation"].ToString() + "</span>");
                            lst.Add(kv);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(drLocation["groupname"].ToString()))
                {
                    if (!result.Contains("<span style='color:#0000FF'> >>>> " + drLocation["groupname"].ToString() + "</span>"))
                    {
                        if (subSubSubItem.Nodes == null)
                        {
                            subSubSubItem.Nodes = new List<Tier4>();
                            subSubSubSubItem = new Tier4();
                            subSubSubSubItem.title = drLocation["groupname"].ToString();
                            subSubSubSubItem.key = drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + "," + drLocation["subsublocationid"].ToString() + "," + drLocation["location_groupid"].ToString() + ":5";
                            subSubSubItem.Nodes.Add(subSubSubSubItem);
                        }
                        else
                        {
                            subSubSubSubItem = new Tier4();
                            subSubSubSubItem.title = drLocation["groupname"].ToString();
                            subSubSubSubItem.key = drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + "," + drLocation["subsublocationid"].ToString() + "," + drLocation["location_groupid"].ToString() + ":5";
                            if (subSubSubItem.Nodes == null)
                            {
                                subSubSubItem.Nodes = new List<Tier4>();
                                subSubSubItem.Nodes.Add(subSubSubSubItem);
                            }
                            else
                            {
                                subSubSubItem.Nodes.Add(subSubSubSubItem);
                            }
                            result.Add("<span style='color:#0000FF'> >>>> " + drLocation["groupname"].ToString() + "</span>");
                            kv = new KeyValuePair<string, string>(drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + "," + drLocation["subsublocationid"].ToString() + "," + drLocation["location_groupid"].ToString() + ":5", "<span style='color:#0000FF'> >>>> " + drLocation["subsublocation"].ToString() + "</span>");
                            lst.Add(kv);
                        }
                    }
                }
                if (sitemenu.title != menuName)
                {
                    menuName = sitemenu.title;
                    if (!string.IsNullOrEmpty(sitemenu.title) && sitemenu.title != " ")
                    {
                        treeViewResult.Add(sitemenu);
                    }
                }
            }
            drLocation.Close();
            drLocation.Dispose();
            kv = new KeyValuePair<string, string>("0", "<span>- Any -</span>");
            lst.Insert(0, kv);
            return Json(treeViewResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult List()
        {
            Job objJob = new Job();
            base.Session["LastSearchCriteria"] = null;
            base.Session.Remove("LastSearchCriteria");
            base.Session["LastSearchCriteria"] = new SearchCriteria();
            var pageSize = 10;
            var items = objJob.ListJobs();
            ViewBag.OnePageOfProducts = 1;
            PagedList<JobInfo> listitems = new PagedList<JobInfo>(items, 1, pageSize);
            return base.View("JobsListing", listitems);
        }

        public ViewResult PageIndex(int? page)
        {
            Job objJob = new Job();
            var listitems = objJob.ListJobs();
            var pageSize = 10;
            int PageCount = Convert.ToInt32(Math.Ceiling((double)(listitems.Count()
                        / pageSize)));
            int pageNumber = (page ?? 1);
            ViewBag.OnePageOfProducts = pageNumber;
            return base.View("JobsListing", listitems.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult JobDescription(string id)
        {
            Job objJob = new Job();
            return base.View("JobDescription", objJob.GetJobInfoByReferenceNo(id));
        }

        public ActionResult BackToSearch()
        {
            Job objJob = new Job();
            SearchCriteria searchCriteria = base.Session["LastSearchCriteria"] as SearchCriteria;
            if (searchCriteria != null)
            {
                return base.View("Index", objJob.SearchJobs(searchCriteria.Industry, searchCriteria.Role, searchCriteria.Location, searchCriteria.WorkArrangement, searchCriteria.Keywords, ""));
            }

            return this.Index();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SearchCountryCode(string term)
        {
            Common obj = new Common();
            KeyValuePair<string, string> kv = new KeyValuePair<string, string>();
            List<KeyValuePair<string, string>> lst = new List<KeyValuePair<string, string>>();
            List<string> result = obj.SearchCountryCode(term);
            foreach (string s in result)
            {
                kv = new KeyValuePair<string, string>(s, s);
                lst.Add(kv);
            }
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobAlert(JobAlertModel model)
        {
            if (ModelState.IsValid) { }
            if (model != null)
            {
                YG_Business.JobAlertInfo info = new YG_Business.JobAlertInfo();
                info.JobAlertId = model.JobAlertId;
                info.CadidateId = model.CandidateId;
                info.FirstName = model.Name;
                info.SurName = model.SurName;
                info.Email = model.Email;
                info.PhoneCode = model.PhoneCode;
                info.PhoneNo = model.ContactNumber;
                info.CreatedDate = DateTime.Now;
                info.FrequencyId = model.MailFrequency;
                info.WorkTypeList = new List<YG_Business.JobAlertWorkType>();
                info.LocationList = new List<YG_Business.JobAlertLocation>();
                info.IndustryList = new List<YG_Business.JobAlertIndustry>();
                info.OccupationList = new List<YG_Business.JobAlertOccupation>();
                YG_Business.JobAlertIndustry industryInfo;
                foreach (int industry in model.IndustrySelectID)
                {
                    industryInfo = new YG_Business.JobAlertIndustry();
                    industryInfo.SubIndustryId = industry;
                    info.IndustryList.Add(industryInfo);
                }
                YG_Business.JobAlertLocation locationInfo;
                foreach (string location in model.SelectedLocationIDList)
                {
                    locationInfo = new YG_Business.JobAlertLocation();
                    locationInfo.LocationId = int.Parse(location);
                    info.LocationList.Add(locationInfo);
                }
                YG_Business.JobAlertWorkType workTypeInfo;
                //foreach (int workType in model.WorkTypeSelect)
                //{
                //    workTypeInfo = new YG_Business.JobAlertWorkType();
                //    workTypeInfo.WorkTypeId = workType;
                //    info.WorkTypeList.Add(workTypeInfo);
                //}
                info.WorkTypeList.Add(new YG_Business.JobAlertWorkType());
                string candidateguid = model.CandidateGUID;

                MailMessage message = new MailMessage();
                string address = ConfigurationManager.AppSettings.Get("emailAddress");
                string displayName = ConfigurationManager.AppSettings.Get("emailName");
                string body;
                JobAlert alert = new JobAlert();
                int alertId = 0;
                bool alertExist = false;
                if (model.JobAlertId == 0)
                {
                    alertId = alert.getJobAlertId(info.Email, ref candidateguid);
                }

                if (alertId > 0)
                {
                    body = System.IO.File.ReadAllText(Server.MapPath("~/Template") + "/alertExist.htm");
                    message = new MailMessage
                    {
                        From = new MailAddress(address, displayName),
                        Subject = "Please edit your YOU Global Job Alert subscription",
                        Body = string.Format(body, model.Name, "<a href='" + ConfigurationManager.AppSettings["baseURL"].ToString() + "Jobs/JobAlertEdit.shtml" + "?alertId=" + alertId + "&CID=" + candidateguid + "' > click here</a>", "<img src='" + ConfigurationManager.AppSettings["logoURL"].ToString() + "' />"),
                        IsBodyHtml = true
                    };
                    message.To.Add(model.Email);
                    alertExist = true;
                }
                else
                {
                    alertId = alert.InsertJobAlert(info, ref candidateguid);
                    body = System.IO.File.ReadAllText(Server.MapPath("~/Template") + "/alertemail.html");
                    body = string.Format(body, model.Name, "<a href='" + ConfigurationManager.AppSettings["subscribeConfirm"].ToString() + "?aD=" + alertId + "&CID=" + candidateguid + "' > click here</a>", "<img src='" + ConfigurationManager.AppSettings["logoURL"].ToString() + "' />");

                    if (model.JobAlertId == 0)
                    {
                        message = new MailMessage
                        {
                            From = new MailAddress(address, displayName),
                            Subject = "Please activate your YOU Global Job Alert subscription",
                            Body = body,
                            IsBodyHtml = true
                        };
                        message.To.Add(model.Email);
                    }
                    else
                    {
                        DataSet ds = alert.getJobAlert(alertId, candidateguid);
                        if (ds.Tables.Count > 0)
                        {
                            body = System.IO.File.ReadAllText(Server.MapPath("~/Template") + "/alertConfirmedmail.htm");
                            string name = ds.Tables[0].Rows[0]["first"].ToString();
                            string sector = "<table>";
                            string location = "<table>";
                            string worktype = "<table>";
                            foreach (DataRow drSector in ds.Tables[3].Rows)
                            {
                                sector += "<tr><td>" + drSector["subclassification"].ToString() + "</td></tr>";
                            }
                            sector += "</table>";
                            foreach (DataRow drLocation in ds.Tables[1].Rows)
                            {
                                location += "<tr><td>" + drLocation["name"].ToString() + "</td></tr>";
                            }
                            location += "</table>";

                            foreach (DataRow drWorkType in ds.Tables[2].Rows)
                            {
                                worktype += "<tr><td>" + drWorkType["type"].ToString() + "</td></tr>";
                            }
                            worktype += "</table>";

                            string frequney = ds.Tables[0].Rows[0]["frequency"].ToString();
                            body = string.Format(body, name, sector, location, worktype, frequney, ConfigurationManager.AppSettings["baseURL"].ToString() + "Jobs/JobAlertEdit.shtml" + "?alertId=" + alertId + "&CID=" + candidateguid, ConfigurationManager.AppSettings["logoURL"].ToString());
                        }
                        message = new MailMessage
                        {
                            From = new MailAddress(address, displayName),
                            Subject = "Your YOU Global Job Alert subscription is confirmed",
                            Body = body,
                            IsBodyHtml = true
                        };
                        message.To.Add(model.Email);
                    }
                }
                SmtpClient sc = new SmtpClient();
                sc.Host = ConfigurationManager.AppSettings["smtpHost"].ToString();
                string smtpUser = ConfigurationManager.AppSettings["smtpUserName"].ToString();
                string smtpPwd = ConfigurationManager.AppSettings["smtpPassword"].ToString();
                sc.Credentials = new System.Net.NetworkCredential(smtpUser, smtpPwd);
                sc.Send(message);

                if (alertExist)
                    return base.View("JobAlertExist");
                else
                {
                    if (model.JobAlertId == 0)
                    {
                        return base.View("JobAlertSuccess", alertId);
                    }
                    else
                    {
                        model.JobAlertId = alertId;
                        model.CandidateGUID = candidateguid;
                        return base.View("JobAlertEditSuccess", model);
                    }
                }
            }
            return base.View("JobAlertSuccess");
        }

        public ActionResult JobAlertEdit(int alertId, string CID)
        {
            JobAlertModel model = new JobAlertModel();
            JobAlert alert = new JobAlert();
            if (string.IsNullOrEmpty(CID))
            {
                DataTable dt = new DataTable();
                dt = alert.getJobAlert(alertId);
                if (dt.Rows.Count == 0)
                    return View("JobAlertNotExist");
                else
                {
                    if (string.IsNullOrEmpty(dt.Rows[0]["candidateguid"].ToString()))
                    {
                        CID = alert.updateCandidateGUID(Convert.ToInt32(dt.Rows[0]["candidateid"].ToString()));
                        string body = System.IO.File.ReadAllText(Server.MapPath("~/Template") + "/alertupgradesecurity.htm");
                        string address = ConfigurationManager.AppSettings.Get("emailAddress");
                        string displayName = ConfigurationManager.AppSettings.Get("emailName");
                        MailMessage message = new MailMessage
                        {
                            From = new MailAddress(address, displayName),
                            Subject = "Please use the new link to manage your subscription",
                            Body = string.Format(body, dt.Rows[0]["first"].ToString(), "<a href='" + ConfigurationManager.AppSettings["baseURL"].ToString() + "Jobs/JobAlertEdit.shtml" + "?alertId=" + alertId + "&CID=" + CID + "' > click here</a>", "<img src='" + ConfigurationManager.AppSettings["logoURL"].ToString() + "' />"),
                            IsBodyHtml = true
                        };
                        message.To.Add(dt.Rows[0]["email"].ToString());
                        SmtpClient sc = new SmtpClient();
                        sc.Host = ConfigurationManager.AppSettings["smtpHost"].ToString();
                        string smtpUser = ConfigurationManager.AppSettings["smtpUserName"].ToString();
                        string smtpPwd = ConfigurationManager.AppSettings["smtpPassword"].ToString();
                        sc.Credentials = new System.Net.NetworkCredential(smtpUser, smtpPwd);
                        sc.Send(message);

                        return View("JobAlertSecurityUpgrade");
                    }
                    else
                        return View("JobAlertNotExist");
                }
            }

            DataSet ds = alert.getJobAlert(alertId, CID);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    model.JobAlertId = Convert.ToInt32(ds.Tables[0].Rows[0]["job_alertId"].ToString());
                    model.CandidateId = Convert.ToInt32(ds.Tables[0].Rows[0]["candidateId"].ToString());
                    model.CandidateGUID = CID;
                    model.Name = ds.Tables[0].Rows[0]["first"].ToString();
                    model.SurName = ds.Tables[0].Rows[0]["last"].ToString();
                    model.Email = ds.Tables[0].Rows[0]["email"].ToString();
                    model.PhoneCode = ds.Tables[0].Rows[0]["phonecode"].ToString();
                    model.ContactNumber = ds.Tables[0].Rows[0]["phonenumber"].ToString();
                    model.MailFrequency = Convert.ToInt32(ds.Tables[0].Rows[0]["frequencyId"].ToString());

                    model.IndustrySelect = new List<int>();
                    foreach (DataRow drSector in ds.Tables[3].Rows)
                    {
                        model.IndustrySelect.Add(Convert.ToInt32(drSector["jobindustrysubid"].ToString()));
                    }

                    model.LocationSelect = new List<int>();
                    foreach (DataRow drLocation in ds.Tables[1].Rows)
                    {
                        model.LocationSelect.Add(Convert.ToInt32(drLocation["locationid"].ToString()));
                    }

                    model.WorkTypeSelect = new List<int>();
                    foreach (DataRow drWorktype in ds.Tables[2].Rows)
                    {
                        model.WorkTypeSelect.Add(Convert.ToInt32(drWorktype["job_typeid"].ToString()));
                    }

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
                    return View("JobAlertUpdate", model);
                }
            }
            return View("JobAlertNotExist");
        }

        public ActionResult JobAlertDelete(int alertId)
        {
            JobAlert obj = new JobAlert();
            obj.AlertDelete(alertId, 0, "", "", "");

            return View("JobAlertDelete");
        }

        public string GetRole(string classification)
        {
            if (Session["JobIndustySub"] == null)
            {
                Common obj = new Common();
                DataTable dt = obj.JobIndustrySubList();
                DataRow dr = dt.NewRow();
                dr[0] = 0;
                dr[1] = "-- Any --";
                dr[2] = "";
                dt.Rows.InsertAt(dr, 0);
                Session.Add("JobIndustySub", dt);
            }
            DataTable dtIndustrySub = (DataTable)Session["JobIndustySub"];
            string str = "<option key=''>-- Any --</option>";
            if (!string.IsNullOrEmpty(classification))
            {
                DataRow[] drSeq = dtIndustrySub.Select("Classification='" + classification + "'");
                foreach (DataRow item in drSeq)
                {
                    str += "<option class='" + item["Classification"].ToString() + "' key='" + item["SubClassification"].ToString() + "'>" + item["SubClassification"].ToString() + "</option>";
                }
            }
            return str;
        }

        public string GetClassification(string classification)
        {
            Common obj = new Common();
            DataTable dtIndustry = string.IsNullOrEmpty(classification) ? obj.JobClassificationList() : obj.SearchClassification(classification);
            string str = string.Empty;

            foreach (DataRow item in dtIndustry.Rows)
            {
                str += "<option key='" + item["JobIndustrySubId"].ToString() + "'>" + item["classification"].ToString() + "</option>";
            }

            return str;
        }

        public override string DefaultViewName
        {
            get
            {
                return "Index";
            }
        }

        public DataTable WorkTypeList()
        {
            DataTable dt = new DataTable();
            MySqlDataReader reader = JobDetailDataProvider.listJobType();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            DataRow dr = dt.NewRow();
            dr[0] = 0;
            dr[1] = "-- Any --";
            dt.Rows.InsertAt(dr, 0);
            Session.Add("JobType", dt);
            return dt;
        }

        public DataTable listJobType()
        {
            DataTable dt = new DataTable();
            MySqlDataReader reader = ClientDataProvider.listActiveClient();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            DataRow dr = dt.NewRow();
            dr[0] = 0;
            dr[1] = "-- Any --";
            dt.Rows.InsertAt(dr, 0);
            Session.Add("JobTypes", dt);
            return dt;
        }

        public DataTable listCurrencies()
        {
            DataTable dt = new DataTable();
            MySqlDataReader reader = CommonDataProvider.listCurrencies();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            Session.Add("Currencies", dt);
            return dt;
        }

        public DataTable listFrequency()
        {
            DataTable dt = new DataTable();
            MySqlDataReader reader = CommonDataProvider.listFrequency();
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            Session.Add("Frequency", dt);
            return dt;
        }

        private int Save(JobDetailInfo model)
        {
            int jobId = 0;

            string jobContent = string.Empty;
            JobDetailInfo jobInfo = new JobDetailInfo();
            jobInfo.ISCO08Id = model.ISCO08Id;
            jobInfo.ISICRev4Id = model.ISICRev4Id;
            jobInfo.TypeId = Convert.ToInt32(model.TypeId);
            jobInfo.SalaryMin = model.SalaryMin;
            jobInfo.SalaryMax = model.SalaryMax;
            jobInfo.ReferenceNo = model.ReferenceNo.Trim().ToUpper();
            jobInfo.Title = !string.IsNullOrEmpty(model.Title) ? model.Title.ToUpper() : "";
            jobInfo.Bullet1 = !string.IsNullOrEmpty(model.Bullet1) ? model.Bullet1.ToUpper() : "";
            jobInfo.Bullet2 = !string.IsNullOrEmpty(model.Bullet2) ? model.Bullet2.ToUpper() : "";
            jobInfo.Bullet3 = !string.IsNullOrEmpty(model.Bullet3) ? model.Bullet3.ToUpper() : "";
            jobInfo.Summary = !string.IsNullOrEmpty(model.Summary) ? model.Summary.ToUpper() : "";

            jobInfo.AdFooter = !string.IsNullOrEmpty(model.AdFooter) ? model.AdFooter.ToUpper() : "";
            jobInfo.Status = 1;
            jobInfo.IsApprove = true;
            jobInfo.WebsiteURL = !string.IsNullOrEmpty(model.WebsiteURL) ? model.WebsiteURL.ToUpper() : "";
            jobInfo.ClientId = Convert.ToInt32(model.ClientId);
            jobInfo.CreatedDate = DateTime.UtcNow;
            jobInfo.HotJob = model.HotJob;
            jobInfo.SalaryCurrency = !string.IsNullOrEmpty(model.SalaryCurrency) ? model.SalaryCurrency.ToUpper() : "";
            if (model.SalaryFrequency.HasValue && model.SalaryFrequency.Value > 0)
                jobInfo.SalaryFrequency = Convert.ToInt32(model.SalaryFrequency.Value);

            List<EssentialCriteriaInfo> lst = new List<EssentialCriteriaInfo>();
            List<DesirableCriteriaInfo> lstDesi = new List<DesirableCriteriaInfo>();
            List<JobConsultantInfo> lstConsultant = model.ConsultantList != null ? model.ConsultantList : new List<JobConsultantInfo>();
            List<JobLocation> locationlist = model.LocationList != null ? model.LocationList : new List<JobLocation>();
            jobContent = !string.IsNullOrEmpty(model.JobContent) ? model.JobContent : "";
            if (string.IsNullOrEmpty(jobContent))
            {
                TempData["errorPostJob"] = "should have job content.";
                return jobId;
            }
            if (model.SelectedLocationList != null && model.SelectedLocationTypeList != null && model.SelectedLocationIDList != null)
            {
                for (int i = 1; i <= model.SelectedLocationList.Count; i++)
                {
                    locationlist.Add(new JobLocation { Location = model.SelectedLocationList[i - 1], Locationtype = int.Parse(model.SelectedLocationTypeList[i - 1]), LocationId = int.Parse(model.SelectedLocationIDList[i - 1]) });
                }
                int inc = 1;
                if (model.DutiesList != null && model.DutiesList.Count > 0)
                {
                    inc = 1;
                    jobContent += "<p><strong> RESPONSIBILITIES </strong></p><ul class='list'>";
                    foreach (string key in model.DutiesList)
                    {
                        jobContent += string.Format("<li>{0}</li>", key);
                    }
                    jobContent += "</ul>";
                }
                else
                {
                    TempData["errorPostJob"] = "Must Add Atleast One job responsibility.";
                    return jobId;
                }

                if (model.MustHaveList != null && model.MustHaveList.Count > 0)
                {
                    inc = 1;
                    jobContent += "<p><strong> MUST HAVES </strong></p><ul class='list'>";
                    foreach (string key in model.MustHaveList)
                    {
                        inc = inc + 1;
                        lst.Add(new EssentialCriteriaInfo { EssentialCriteriaId = 0, AnswerLength = 500, Description = key, SortOrder = inc });
                        jobContent += string.Format("<li>{0}</li>", key);
                    }
                    jobContent += "</ul>";
                }
                else
                {
                    TempData["errorPostJob"] = "Must Add Atleast one musthaves.";
                    return jobId;
                }
                if (model.NiceToHaveList != null && model.NiceToHaveList.Count > 0)
                {
                    inc = 1;
                    jobContent += "<p><strong> NICE TO HAVES </strong></p><ul class='list'>";
                    foreach (string key in model.NiceToHaveList)
                    {
                        inc = inc + 1;
                        lstDesi.Add(new DesirableCriteriaInfo { DesirableCriteriaId = 0, AnswerLength = 500, Description = key, SortOrder = inc });
                        jobContent += string.Format("<li>{0}</li>", key);
                    }
                    jobContent += "</ul>";
                }
                else
                {
                    TempData["errorPostJob"] = "Must Add nice to have list.";
                    return jobId;
                }
                if (model.Remuneration != null && model.Remuneration.Count > 0)
                {
                    jobContent += "<p><strong> REMUNERATION </strong></p><ul class='list'>";
                    foreach (string key in model.Remuneration)
                    {
                        jobContent += string.Format("<li>{0}</li>", key);
                    }
                    jobContent += "</ul><p>&nbsp;</p>";
                }
                else
                {
                    TempData["errorPostJob"] = "Must Add Remuneration.";
                    return jobId;
                }

                jobInfo.JobContent = jobContent;
                jobInfo.EssentialCriteriaList = lst;
                jobInfo.DesirableCriteriaList = lstDesi;
                jobInfo.ConsultantList = lstConsultant;
                jobInfo.LocationList = locationlist;

                jobInfo.JobId = Convert.ToInt32(JobDetailDataProvider.insertJobDetail(jobInfo));
                jobInfo.Version = 1;
                jobId = jobInfo.JobId;
                JobDetailEditDataProvider.insertJobDetailEdit(jobInfo, true);
            }
            else
            {
                TempData["errorPostJob"] = "Must Add Atleast One Location.";
            }
            return jobId;
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Reset(JobDetailInfo model, string returnUrl)
        {
            if (model != null)
            {
                model.TypeId = 0;
                model.SalaryMin = "";
                model.SalaryMax = "";
                model.ReferenceNo = "";
                model.Title = "";
                model.Bullet1 = "";
                model.Bullet2 = "";
                model.Bullet3 = "";
                model.Summary = "";
                model.JobContent = "";
                model.AdFooter = "";
                model.IsApprove = false;
                model.WebsiteURL = "";
                model.ClientId = 0;
                model.CreatedDate = DateTime.UtcNow;
                model.HotJob = false;
                model.SalaryCurrency = "";
                //model.ISCO08Id = Convert.ToInt32(string.IsNullOrEmpty(hdfOccupationId.Value) ? "-1" : hdfOccupationId.Value);
                //model.ISICRev4Id = Convert.ToInt32(string.IsNullOrEmpty(hdfIndustryId.Value) ? "-1" : hdfIndustryId.Value);
                model.SalaryFrequency = 0;
            }
            return View("postjob", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult getOccupation(string term)
        {
            List<string> result = new List<string>();
            MySqlDataReader dr = ISCODataProvider.searchgroup(term, 4);
            while (dr.Read())
            {
                if (!result.Contains("<span style='color:#9AC435'>>" + dr["c1"].ToString() + " " + dr["d1"].ToString() + "</span>"))
                    result.Add("<span style='color:#9AC435'>>" + dr["c1"].ToString() + " " + dr["d1"].ToString() + "</span>");
                if (!result.Contains("<span style='color:#124812'>>>" + dr["c2"].ToString() + " " + dr["d2"].ToString() + "</span>"))
                    result.Add("<span style='color:#124812'>>>" + dr["c2"].ToString() + " " + dr["d2"].ToString() + "</span>");
                if (!result.Contains("<span style='color:#51C0EE'>>>>" + dr["c3"].ToString() + " " + dr["d3"].ToString() + "</span>"))
                    result.Add("<span style='color:#51C0EE'>>>>" + dr["c3"].ToString() + " " + dr["d3"].ToString() + "</span>");
                result.Add(AutoCompleteExtender.CreateAutoCompleteItem("<span style='color:#0000FF'>UNIT " + dr["c4"].ToString() + " " + dr["d4"].ToString() + "</span>", dr["id4"].ToString()));
            }
            dr.Close();
            dr.Dispose();
            result.Insert(0, AutoCompleteExtender.CreateAutoCompleteItem("Anywhere", "0,0,0,0"));
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult getIndustry(string term)
        {
            List<string> result = new List<string>();
            DataTable dt = new DataTable();
            MySqlDataReader dr = ISICRevDataProvider.searchISICRev4withHierarchy(term);
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    if (!result.Contains("<span style='color:#9AC435'>>" + dr["c1"].ToString() + " " + dr["d1"].ToString() + "</span>"))
                        result.Add("<span style='color:#9AC435'>>" + dr["c1"].ToString() + " " + dr["d1"].ToString() + "</span>");
                    if (!result.Contains("<span style='color:#124812'>>>" + dr["c2"].ToString() + " " + dr["d2"].ToString() + "</span>"))
                        result.Add("<span style='color:#124812'>>>" + dr["c2"].ToString() + " " + dr["d2"].ToString() + "</span>");
                    if (!result.Contains("<span style='color:#51C0EE'>>>>" + dr["c3"].ToString() + " " + dr["d3"].ToString() + "</span>"))
                        result.Add("<span style='color:#51C0EE'>>>>" + dr["c3"].ToString() + " " + dr["d3"].ToString() + "</span>");
                    result.Add(AutoCompleteExtender.CreateAutoCompleteItem("<span style='color:#0000FF'>CLASS " + dr["c4"].ToString() + " " + dr["d4"].ToString() + "</span>", dr["id4"].ToString()));
                }
            }
            else
            {
                // result.Add(prefixText + " didn't match any item.");
            }
            dr.Close();
            dr.Dispose();
            result.Insert(0, AutoCompleteExtender.CreateAutoCompleteItem("Anywhere", "0,0,0,0"));
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //[AcceptVerbs(HttpVerbs.Post)]
        //public MySqlDataReader getLocation(string term)
        //{
        //    List<string> result = new List<string>();
        //    MySqlDataReader drLocation = LocationDataProvider.getLocationswithGroupName(term);
        //    return drLocation;
        //}
        public ActionResult getLocation(string term)
        {
            KeyValuePair<string, string> kv = new KeyValuePair<string, string>();
            List<KeyValuePair<string, string>> lst = new List<KeyValuePair<string, string>>();
            List<string> result = new List<string>();
            MySqlDataReader drLocation = LocationDataProvider.getLocationswithGroupName(term);
            List<Tier> treeViewResult = new List<Tier>();
            Tier1 subItem = new Tier1();
            Tier2 subSubItem = new Tier2();
            Tier3 subSubSubItem = new Tier3();
            Tier4 subSubSubSubItem = new Tier4();
            Tier sitemenu = new Tier();
            string menuName = string.Empty;
            int ParentMenuID = 0;
            string item = string.Empty;
            while (drLocation.Read())
            {
                if (!string.IsNullOrEmpty(drLocation["name"].ToString()))
                {
                    if (!result.Contains("<span style='color:#9AC435'> > " + drLocation["name"].ToString() + "</span>"))
                    {
                        sitemenu = new Tier();
                        sitemenu.TierId = ParentMenuID;
                        sitemenu.ParentMenuID = 0;
                        sitemenu.children = new List<Tier1>();
                        sitemenu.key = drLocation["countryid"].ToString() + ":1";
                        item = drLocation["name"].ToString();
                        sitemenu.title = drLocation["name"].ToString();
                        result.Add("<span style='color:#9AC435'> > " + drLocation["name"].ToString() + "</span>");
                        kv = new KeyValuePair<string, string>(drLocation["countryid"].ToString() + ":1", "<span style='color:#9AC435'> > " + drLocation["name"].ToString() + "</span>");
                        lst.Add(kv);
                    }
                }
                if (!string.IsNullOrEmpty(drLocation["locationname"].ToString()))
                {
                    if (!result.Contains("<span style='color:#124812'> >> " + drLocation["locationname"].ToString() + "</span>"))
                    {
                        item = item + " > " + drLocation["locationname"].ToString();
                        if (sitemenu.children == null)
                        {
                            sitemenu.children = new List<Tier1>();
                            subItem.title = item;
                            subItem.key = drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + ":2";
                            sitemenu.children.Add(subItem);
                        }
                        else
                        {
                            subItem = new Tier1();
                            subItem.key = drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + ":2";
                            subItem.title = item;
                            sitemenu.children.Add(subItem);
                        }
                        result.Add("<span style='color:#124812'> >> " + drLocation["locationname"].ToString() + "</span>");
                        kv = new KeyValuePair<string, string>(drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + ":2", "<span style='color:#124812'> >> " + drLocation["locationname"].ToString() + "</span>");
                        lst.Add(kv);
                    }
                }
                if (!string.IsNullOrEmpty(drLocation["sublocation"].ToString()))
                {
                    if (!result.Contains("<span style='color:#51C0EE'> >>> " + drLocation["sublocation"].ToString() + "</span>"))
                    {
                        item = item + " > " + drLocation["sublocation"].ToString();
                        if (subItem.children == null)
                        {
                            subItem.children = new List<Tier2>();
                            subSubItem = new Tier2();
                            subSubItem.key = drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + ":3";
                            subSubItem.title = item;
                            subItem.children.Add(subSubItem);
                        }
                        else
                        {
                            subSubItem = new Tier2();
                            subSubItem.key = drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + ":3";
                            subSubItem.title = item;
                            if (subSubItem.children == null)
                            {
                                subItem.children = new List<Tier2>();
                                subItem.children.Add(subSubItem);
                            }
                            else
                            {
                                subItem.children.Add(subSubItem);
                            }
                        }
                        result.Add("<span style='color:#51C0EE'> >>> " + drLocation["sublocation"].ToString() + "</span>");
                        kv = new KeyValuePair<string, string>(drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + ":3", "<span style='color:#51C0EE'> >>> " + drLocation["sublocation"].ToString() + "</span>");
                        lst.Add(kv);
                    }
                }
                if (!string.IsNullOrEmpty(drLocation["subsublocation"].ToString()))
                {
                    if (!result.Contains("<span style='color:#0000FF'> >>>> " + drLocation["subsublocation"].ToString() + "</span>"))
                    {
                        item = item + " > " + drLocation["subsublocation"].ToString();
                        if (subSubItem.children == null)
                        {
                            subSubItem.children = new List<Tier3>();
                            subSubSubItem = new Tier3();
                            subSubSubItem.title = item;
                            subSubSubItem.key = drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + "," + drLocation["subsublocationid"].ToString() + ":4";
                            subSubItem.children.Add(subSubSubItem);
                        }
                        else
                        {
                            subSubSubItem = new Tier3();
                            subSubSubItem.title = item;
                            subSubSubItem.key = drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + "," + drLocation["subsublocationid"].ToString() + ":4";
                            if (subSubItem.children == null)
                            {
                                subSubItem.children = new List<Tier3>();
                                subSubItem.children.Add(subSubSubItem);
                            }
                            else
                            {
                                subSubItem.children.Add(subSubSubItem);
                            }
                            result.Add("<span style='color:#0000FF'> >>>> " + drLocation["subsublocation"].ToString() + "</span>");
                            kv = new KeyValuePair<string, string>(drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + "," + drLocation["subsublocationid"].ToString() + ":4", "<span style='color:#0000FF'> >>>> " + drLocation["subsublocation"].ToString() + "</span>");
                            lst.Add(kv);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(drLocation["groupname"].ToString()))
                {
                    if (!result.Contains("<span style='color:#0000FF'> >>>> " + drLocation["groupname"].ToString() + "</span>"))
                    {
                        item = item + " > " + drLocation["groupname"].ToString();
                        if (subSubSubItem.Nodes == null)
                        {
                            subSubSubItem.Nodes = new List<Tier4>();
                            subSubSubSubItem = new Tier4();
                            subSubSubSubItem.title = item;
                            subSubSubSubItem.key = drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + "," + drLocation["subsublocationid"].ToString() + "," + drLocation["location_groupid"].ToString() + ":5";
                            subSubSubItem.Nodes.Add(subSubSubSubItem);
                        }
                        else
                        {
                            subSubSubSubItem = new Tier4();
                            subSubSubSubItem.title = item;
                            subSubSubSubItem.key = drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + "," + drLocation["subsublocationid"].ToString() + "," + drLocation["location_groupid"].ToString() + ":5";
                            if (subSubSubItem.Nodes == null)
                            {
                                subSubSubItem.Nodes = new List<Tier4>();
                                subSubSubItem.Nodes.Add(subSubSubSubItem);
                            }
                            else
                            {
                                subSubSubItem.Nodes.Add(subSubSubSubItem);
                            }
                            result.Add("<span style='color:#0000FF'> >>>> " + drLocation["groupname"].ToString() + "</span>");
                            kv = new KeyValuePair<string, string>(drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + "," + drLocation["subsublocationid"].ToString() + "," + drLocation["location_groupid"].ToString() + ":5", "<span style='color:#0000FF'> >>>> " + drLocation["subsublocation"].ToString() + "</span>");
                            lst.Add(kv);
                        }
                    }
                }
                if (sitemenu.title != menuName)
                {
                    menuName = sitemenu.title;
                    if (!string.IsNullOrEmpty(sitemenu.title) && sitemenu.title != " ")
                    {
                        treeViewResult.Add(sitemenu);
                    }
                }
            }
            drLocation.Close();
            drLocation.Dispose();
            kv = new KeyValuePair<string, string>("0", "<span>- Any -</span>");
            lst.Insert(0, kv);
            return Json(treeViewResult, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult getLocationwithGroup(string term)
        {
            List<string> result = new List<string>();
            MySqlDataReader drLocation = LocationDataProvider.getLocationswithGroupName(term);
            while (drLocation.Read())
            {
                string item = drLocation["name"].ToString();
                if (!string.IsNullOrEmpty(drLocation["name"].ToString().Trim()))
                {
                    if (!result.Contains(AutoCompleteExtender.CreateAutoCompleteItem("<span style='color:#9AC435'>" + drLocation["name"].ToString() + "</span>", drLocation["countryid"].ToString())))
                        result.Add(AutoCompleteExtender.CreateAutoCompleteItem("<span style='color:#9AC435'>" + drLocation["name"].ToString() + "</span>", drLocation["countryid"].ToString()));
                    if (!string.IsNullOrEmpty(drLocation["locationname"].ToString().Trim()))
                    {
                        item = item + " > " + drLocation["locationname"].ToString();
                        if (!result.Contains(AutoCompleteExtender.CreateAutoCompleteItem("<span style='color:#124812'>" + item + "</span>", drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString())))
                            result.Add(AutoCompleteExtender.CreateAutoCompleteItem("<span style='color:#124812'>" + item + "</span>", drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString()));
                        if (!string.IsNullOrEmpty(drLocation["sublocation"].ToString().Trim()))
                        {
                            item = item + " > " + drLocation["sublocation"].ToString();
                            if (!result.Contains(AutoCompleteExtender.CreateAutoCompleteItem("<span style='color:#51C0EE'>" + item + "</span>", drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString())))
                                result.Add(AutoCompleteExtender.CreateAutoCompleteItem("<span style='color:#51C0EE'>" + item + "</span>", drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString()));
                            if (!string.IsNullOrEmpty(drLocation["subsublocation"].ToString().Trim()))
                            {
                                item = item + " > " + drLocation["subsublocation"].ToString();
                                result.Add(AutoCompleteExtender.CreateAutoCompleteItem("<span style='color:#0000FF'>" + item + "</span>", drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + "," + drLocation["subsublocationid"].ToString()));
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(drLocation["groupname"].ToString().Trim()))
                {
                    result.Add(AutoCompleteExtender.CreateAutoCompleteItem("<span style='color:#0000FF'>" + drLocation["groupname"].ToString() + "</span>", drLocation["countryid"].ToString() + "," + drLocation["locationid"].ToString() + "," + drLocation["sublocationid"].ToString() + "," + drLocation["subsublocationid"].ToString() + "," + drLocation["location_groupid"].ToString()));
                }
            }
            drLocation.Close();
            drLocation.Dispose();
            result.Insert(0, AutoCompleteExtender.CreateAutoCompleteItem("Anywhere", "0,0,0,0"));
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public List<string> GetLocationDetails(int groupId)
        {
            List<string> result = new List<string>();
            MySqlDataReader dr = CommonDataAccess.getLocations(groupId);
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    result.Add(DataAccess.getString(dr, "locationid"));
                }
            }
            return result;
        }
    }

    public class IndustryCount
    {
        public int Id { get; set; }
        public int Count { get; set; }
    }

    public class OccupationCount
    {
        public int Id { get; set; }
        public int Count { get; set; }
    }
}