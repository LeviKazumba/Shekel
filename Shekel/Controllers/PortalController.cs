using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shekel.Models;

namespace Shekel.Controllers
{
    public class PortalController : Controller
    {
        #region Global Variables
        DB.ShekelEntities db = new DB.ShekelEntities();
        Methods api = new Methods();
        #endregion

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Index()
        {
            bool exist = false;
            try
            {
                if (Session["UserID"] == null)
                {
                    Response.Redirect("../Home/Index");
                }

                dynamic u = api.User(Session["UserID"].ToString());

                Type typeOfDynamic = u.GetType();

                if (!(exist = typeOfDynamic.GetProperties().Where(p => p.Name.Equals("Status")).Any()))
                {
                    ViewBag.KYC = api.KYCStatus(Session["UserID"].ToString());

                    Session["User"] = u;
                    ViewBag.User = u;
                }
                else
                {
                    Response.Redirect("../Home/Index");
                }
            }
            catch (Exception)
            {
                Response.Redirect("../Home/Index");
            }
            return View();
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public new ActionResult Profile()
        {
            bool exist = false;
            try
            {
                if (Session["UserID"] == null)
                {
                    Response.Redirect("../Home/Index");
                }

                dynamic u = api.User(Session["UserID"].ToString());

                Type typeOfDynamic = u.GetType();

                if (!(exist = typeOfDynamic.GetProperties().Where(p => p.Name.Equals("Status")).Any()))
                {
                    Session["User"] = u;
                    ViewBag.User = u;
                }
                else
                {
                    Response.Redirect("../Home/Index");
                }
            }
            catch (Exception)
            {
                Response.Redirect("../Home/Index");
            }
            return View();
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Investments()
        {
            try
            {
                ViewBag.User = Session["User"];
            }
            catch (Exception)
            {
                Response.Redirect("../Home/Index");
            }
            return View();
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Security()
        {
            try
            {
                ViewBag.User = Session["User"];
            }
            catch (Exception)
            {
                Response.Redirect("../Home/Index");
            }
            return View();
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult KYC()
        {
            try
            {
                ViewBag.User = Session["User"];
            }
            catch (Exception)
            {
                Response.Redirect("../Home/Index");
            }
            return View();
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult ProcessKYC()
        {
            bool exist = false;
            try
            {
                dynamic u = api.User(Session["UserID"].ToString());

                Type typeOfDynamic = u.GetType();

                if (!(exist = typeOfDynamic.GetProperties().Where(p => p.Name.Equals("Status")).Any()))
                {
                    if (u.UserType != "SystemAdmin")
                    {
                        Response.Redirect("../Portal/Index");
                    }

                    ViewBag.KYC = api.KYCStatus(Session["UserID"].ToString());

                    Session["User"] = u;
                    ViewBag.User = u;
                }
                else
                {
                    Response.Redirect("../Home/Index");
                }

            }
            catch (Exception)
            {
                Response.Redirect("../Home/Index");
            }
            return View();
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Users()
        {
            bool exist = false;
            try
            {
                dynamic u = api.User(Session["UserID"].ToString());

                Type typeOfDynamic = u.GetType();

                if (!(exist = typeOfDynamic.GetProperties().Where(p => p.Name.Equals("Status")).Any()))
                {
                    if (u.UserType != "SystemAdmin")
                    {
                        Response.Redirect("../Portal/Index");
                    }

                    ViewBag.KYC = api.KYCStatus(Session["UserID"].ToString());

                    Session["User"] = u;
                    ViewBag.User = u;
                }
                else
                {
                    Response.Redirect("../Home/Index");
                }

            }
            catch (Exception)
            {
                Response.Redirect("../Home/Index");
            }
            return View();
        }
    }
}