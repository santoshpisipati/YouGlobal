using MySql.Data.MySqlClient;
using Sample.Web.ModalLogin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using YG_Business;
using YG_DataAccess;
using YG_MVC.Controllers;

namespace Sample.Web.ModalLogin.Controllers
{
    public class ContactController : BaseController
    {
        public override string DefaultViewName
        {
            get
            {
                return "ContactUsHtml";
            }
        }

        public ActionResult ContactUsHtml()
        {
            ContactUsModel consm = new ContactUsModel();
            consm.CountryList = new SelectList(getStateList(), "Id", "Text");
            consm.CountryId = 1;
            return base.View("ContactUsHtml", consm);
        }


        public List<CountryList> getStateList()
        {
            List<CountryList> countryList = new List<CountryList>();
            MySqlDataReader sdr = Common.listCountries();
            countryList.Add(new CountryList { Id = 0, Text = "--Please Select--" });
            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    CountryList country = new CountryList();
                    if (!string.IsNullOrEmpty(sdr["Id"].ToString()))
                        country.Id = Convert.ToInt32(sdr["Id"].ToString());
                    else
                        country.Id = 0;
                    if (!string.IsNullOrEmpty(sdr["name"].ToString()))
                        country.Text = Convert.ToString(sdr["name"].ToString());
                    else
                        country.Text = "";
                    countryList.Add(country);
                }
            }
            return countryList;
        }

        public JsonResult GetCities(int CityId)
        {
            List<CountryList> Locations = new List<CountryList>();
            CountryList cl = new CountryList();
            MySqlDataReader sdr = Common.GetCities(CityId);
            Locations.Add(new CountryList { Id = 0, Text = "--Please Select--" });
            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    CountryList country = new CountryList();
                    if (!string.IsNullOrEmpty(sdr["Id"].ToString()))
                        country.Id = Convert.ToInt32(sdr["Id"].ToString());
                    else
                        country.Id = 0;
                    if (!string.IsNullOrEmpty(sdr["name"].ToString()))
                        country.Text = Convert.ToString(sdr["name"].ToString());
                    else
                        country.Text = "";
                    Locations.Add(country);
                }
            }
            return Json(Locations, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetConsultants(int CityId, bool fromInfo)
        {
            MySqlDataReader sdr = null;
            List<CountryList> Locations = new List<CountryList>();
            CountryList cl = new CountryList();
            if (fromInfo)
            {
                sdr = Common.GetConsultantsfromInfo(CityId);
            }
            else
            {
                sdr = Common.GetConsultants(CityId);
            }
            Locations.Add(new CountryList { Id = 0, Text = "--Select Consultant--" });
            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    CountryList country = new CountryList();
                    if (!string.IsNullOrEmpty(sdr["Id"].ToString()))
                        country.Id = Convert.ToInt32(sdr["Id"].ToString());
                    else
                        country.Id = 0;
                    if (!string.IsNullOrEmpty(sdr["ConsultantName"].ToString()))
                        country.Text = Convert.ToString(sdr["ConsultantName"].ToString());
                    else
                        country.Text = "";
                    Locations.Add(country);
                }
            }
            return Json(Locations, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetContactDetails(int ContactInfoid)
        {
            ContactUsModel cusm = new ContactUsModel();
            MySqlDataReader sdr = Common.GetContactDetails(ContactInfoid);
            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    cusm = new ContactUsModel();
                    cusm.Address = Convert.ToString(sdr["Address"].ToString());
                    cusm.AddressLine1 = Convert.ToString(sdr["AddressLine1"].ToString());
                    cusm.addressLine2 = Convert.ToString(sdr["addressLine2"].ToString());
                    cusm.bioImage = string.Format("/Content/flas/photos/{0}", Convert.ToString(sdr["bioImage"].ToString()));
                    cusm.bioName = Convert.ToString(sdr["bioName"].ToString());
                    cusm.bioText = Convert.ToString(sdr["bioText"].ToString());
                    cusm.ConsultantName = Convert.ToString(sdr["ConsultantName"].ToString());
                    cusm.contactPerson = Convert.ToString(sdr["contactPerson"].ToString());
                    cusm.Country = Convert.ToString(sdr["Country"].ToString());
                    cusm.CountryId = 0;
                    cusm.Id = Convert.ToInt32(sdr["Id"].ToString());
                    cusm.Designation = Convert.ToString(sdr["Designation"].ToString());
                    cusm.email = Convert.ToString(sdr["email"].ToString());
                    cusm.faxNumber = Convert.ToString(sdr["faxNumber"].ToString());
                    cusm.FormalQualifications = Convert.ToString(sdr["FormalQualifications"].ToString());
                    cusm.GeographicFamiliarity = Convert.ToString(sdr["GeographicFamiliarity"].ToString());
                    cusm.IndustrySpecialisation = Convert.ToString(sdr["IndustrySpecialisation"].ToString()); 
                    cusm.Languages = Convert.ToString(sdr["Languages"].ToString());
                    cusm.mobileNumber = Convert.ToString(sdr["mobileNumber"].ToString());
                    cusm.Name = Convert.ToString(sdr["Name"].ToString());
                    cusm.skype = Convert.ToString(sdr["skype"].ToString());
                    cusm.telephoneNumber = Convert.ToString(sdr["telephoneNumber"].ToString());
                }
            }
            return Json(cusm, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult SendQuery(FormCollection formCollection)
        {
            if (!base.ProcessUpload())
            {
                return RedirectToAction("MessageFailure", "Work");
            }
            return View("Thankyou");
        }

        public ActionResult Thankyou()
        {
            return base.View("Thankyou");
        }
    }
}