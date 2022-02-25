using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Shekel.Models;

namespace Shekel.Controllers
{
    public class ApiController : Controller
    {

        #region Global Variables
        DB.ShekelEntities db = new DB.ShekelEntities();

        Methods api = new Methods();
        #endregion

        #region Account
        [HttpPost]
        public ActionResult Login(Login l)
        {

            string Status = "Success";
            string Data = "";
            int LogID = 0;
            try
            {
                LogID = api.Log(LogID, l.Email, "Login", JsonConvert.SerializeObject(l), null);

                //Validate input
                if (string.IsNullOrEmpty(l.Email))
                {
                    throw new Exception("Email required");
                }

                if (string.IsNullOrEmpty(l.Password))
                {
                    throw new Exception("Password required");
                }

                var User = (from a in db.Users where a.Email == l.Email select a).FirstOrDefault();

                if (User != null)
                {
                    if (User.Password == l.Password)
                    {

                        User.LastLogin = DateTime.Now;
                        User.DateModified = DateTime.Now;
                        User.LoginAttempts = 0;
                        User.ModifiedBy = l.Email;
                        db.SaveChanges();

                        Session["UserID"] = User.UserID;
                        Session["Email"] = User.Email;
                    }
                    else
                    {
                        if (User.LoginAttempts > 4)
                        {
                            User.LockedOut = true;
                            User.DateModified = DateTime.Now;
                            User.ModifiedBy = l.Email;
                            db.SaveChanges();

                            throw new Exception("Your account has been locked due to multiple failed login attempts, please contact customer support for assistance");
                        }
                        else
                        {
                            User.LoginAttempts = User.LoginAttempts + 1;
                            User.ModifiedBy = l.Email;
                            User.DateModified = DateTime.Now;
                            db.SaveChanges();

                            throw new Exception("Incorrect password");
                        }
                    }
                }
                else
                {
                    throw new Exception("Invalid user email");
                }
            }
            catch (Exception ex)
            {
                Status = "Failed";
                Data = ex.Message;
                string Error = ex.Message + "\n" + ex.InnerException + "\n" + ex.StackTrace;
                LogID = api.Log(LogID, null, "Login", null, Error);
            }
            return Json(new { Status, Data }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            string Status = "Success";
            string Data = "";

            try
            {
                Session["UserID"] = null;
                Session["Email"] = null;

            }
            catch (Exception)
            {
                Response.Redirect("../Home/Index");
            }
            return Json(new { Status, Data }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Countries()
        {
            string Status = "Success";
            string Data = "";
            var Countries = new CountryModel();

            int LogID = 0;
            try
            {
                LogID = api.Log(LogID, null, "Countries", null, null);

                Data = JsonConvert.SerializeObject(api.Countries());

            }
            catch (Exception ex)
            {
                Status = "Failed";
                Data = ex.Message;
                string Error = ex.Message + "\n" + ex.InnerException + "\n" + ex.StackTrace;
                LogID = api.Log(LogID, null, "Countries", null, Error);
            }
            return Json(new { Status, Data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SignUp(UserModel u)
        {

            string Status = "Success";
            string Data = "";
            int LogID = 0;
            string Check;
            var user = new DB.User();
            try
            {
                LogID = api.Log(LogID, u.Email, "SignUp", JsonConvert.SerializeObject(u), null);

                //Validate input
                if (string.IsNullOrEmpty(u.Name))
                {
                    throw new Exception("Name required");
                }
                if (string.IsNullOrEmpty(u.Surname))
                {
                    throw new Exception("Surname required");
                }
                if (string.IsNullOrEmpty(u.Email))
                {
                    throw new Exception("Email required");
                }
                if (string.IsNullOrEmpty(u.Telephone))
                {
                    throw new Exception("Telephone required");
                }
                if (string.IsNullOrEmpty(u.Password))
                {
                    throw new Exception("Password required");
                }

                if ((Check = api.UserDuplicate(u.Email, "Email")) == "YES")
                {
                    throw new Exception("The email provided is already taken, please try another email");
                }

                if ((Check = api.UserDuplicate(u.Telephone, "Telephone")) == "YES")
                {
                    throw new Exception("The telephone number provided is already taken, please try another number");
                }

                var c = (from a in db.Countries where a.PhoneCode == u.Country select a).FirstOrDefault();

                user.UserID = Guid.NewGuid().ToString();
                user.Name = u.Name;
                user.Surname = u.Surname;
                user.Email = u.Email;
                user.Telephone = u.Telephone;
                user.Country = c.Iso2;
                user.Password = u.Password;
                user.LastPassword = u.Password;
                user.LastLogin = DateTime.Now;
                user.LoginAttempts = 0;
                user.LockedOut = false;
                user.ReferralCode = DateTime.Now.ToString("ddfffMMss");
                user.Verified = false;
                user.Active = true;
                user.CreatedBy = u.Email;
                user.DateCreated = DateTime.Now;

                db.Users.Add(user);
                db.SaveChanges();

                //if (!string.IsNullOrEmpty(u.ReferralCode))
                //{
                //    api.NewReferral(u.ReferralCode, user.ReferralCode);
                //}
                Session["UserID"] = user.UserID;
                Session["Email"] = user.Email;

                Data = "Welcome to TFS";
            }
            catch (Exception ex)
            {
                Status = "Failed";
                Data = ex.Message;
                string Error = ex.Message + "\n" + ex.InnerException + "\n" + ex.StackTrace;
                LogID = api.Log(LogID, null, "SignUp", null, Error);
            }
            return Json(new { Status, Data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ForgotPassword(string Email)
        {

            string Status = "Success";
            string Data = "";
            int LogID = 0;
            string emailContent = "";
            try
            {

                LogID = api.Log(LogID, Email, "ForgotPassword", Email, null);

                //Validate input
                if (string.IsNullOrEmpty(Email))
                {
                    throw new Exception("Account email required");
                }

                var user = (from a in db.Users where a.Email == Email select a).FirstOrDefault();

                if (user != null)
                {
                    emailContent = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"Templates\ForgotPassword.html");
                    emailContent = emailContent.Replace("{Name}", user.Name);
                    emailContent = emailContent.Replace("{Email}", user.Email);
                    emailContent = emailContent.Replace("{Password}", user.Password);

                    api.SendEmail(user.Email, emailContent, "Account Recovery");
                }
                else
                {
                    throw new Exception("This email address does not belong to any user account");
                }

                Data = "Your account credentials were sent to your email address";
            }
            catch (Exception ex)
            {
                Status = "Failed";
                Data = ex.Message;
                string Error = ex.Message + "\n" + ex.InnerException + "\n" + ex.StackTrace;
                LogID = api.Log(LogID, null, "ForgotPassword", null, Error);
            }
            return Json(new { Status, Data }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Portal

        [HttpPost]
        public ActionResult UpdateProfile(UserModel u)
        {

            string Status = "Success";
            string Data = "";
            int LogID = 0;
            try
            {
                LogID = api.Log(LogID, u.Email, "UpdateProfile", JsonConvert.SerializeObject(u), null);

                //Validate input
                if (string.IsNullOrEmpty(u.Name))
                {
                    throw new Exception("Name required");
                }
                if (string.IsNullOrEmpty(u.Surname))
                {
                    throw new Exception("Surname required");
                }

                string Email = Session["Email"].ToString();

                var user = (from a in db.Users where a.Email == Email select a).FirstOrDefault();

                if (user != null)
                {
                    user.Name = u.Name;
                    user.Surname = u.Surname;
                    user.DateModified = DateTime.Now;
                    user.ModifiedBy = Email;

                    db.SaveChanges();
                }
                else
                {
                    throw new Exception("Faild to update your profile details, please try again later");
                }

                Data = "Your profile has been updated";
            }
            catch (Exception ex)
            {
                Status = "Failed";
                Data = ex.Message;
                string Error = ex.Message + "\n" + ex.InnerException + "\n" + ex.StackTrace;
                LogID = api.Log(LogID, null, "UpdateProfile", null, Error);
            }
            return Json(new { Status, Data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdatePassword(PasswordModel u)
        {

            string Status = "Success";
            string Data = "";
            int LogID = 0;
            try
            {
                string Email = Session["Email"].ToString();

                LogID = api.Log(LogID, Email, "UpdatePassword", JsonConvert.SerializeObject(u), null);

                //Validate input
                if (string.IsNullOrEmpty(u.OPassword))
                {
                    throw new Exception("Current password required");
                }
                if (string.IsNullOrEmpty(u.NPassword))
                {
                    throw new Exception("New password required");
                }

                var user = (from a in db.Users where a.Email == Email select a).FirstOrDefault();

                if (user != null)
                {
                    if (user.Password != u.OPassword)
                    {
                        throw new Exception("The password you entered does not match your current account password");
                    }

                    user.LastPassword = u.OPassword;
                    user.Password = u.NPassword;
                    user.DateModified = DateTime.Now;
                    user.ModifiedBy = Email;

                    db.SaveChanges();
                }
                else
                {
                    throw new Exception("Failed to update your account credentials, please try again later");
                }

                Data = "Your account credentials have been updated";
            }
            catch (Exception ex)
            {
                Status = "Failed";
                Data = ex.Message;
                string Error = ex.Message + "\n" + ex.InnerException + "\n" + ex.StackTrace;
                LogID = api.Log(LogID, null, "UpdatePassword", null, Error);
            }
            return Json(new { Status, Data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult KYC(KYCModel u)
        {

            string Status = "Success";
            string Data = "";
            int LogID = 0;
            var kyc = new DB.KYC();
            try
            {
                string Email = Session["Email"].ToString();
                string input = "Document number: " + u.DocumentNumber + ", Document Type: " + u.DocumentType + ", KYCOTP: " + u.KYCOTP;

                LogID = api.Log(LogID, Email, "KYC", input, null);

                //Validate input
                if (u.Document.ContentLength <= 0)
                {
                    throw new Exception("Identity document required");
                }
                if (u.Selfie.ContentLength <= 0)
                {
                    throw new Exception("Selfie picture required");
                }
                if (string.IsNullOrEmpty(u.KYCOTP))
                {
                    throw new Exception("KYC OTP required");
                }
                if (string.IsNullOrEmpty(u.DocumentNumber))
                {
                    throw new Exception("Document number required");
                }
                if (string.IsNullOrEmpty(u.DocumentType))
                {
                    throw new Exception("Document type required");
                }

                var user = (from a in db.Users where a.Email == Email select a).FirstOrDefault();

                if (user != null)
                {
                    kyc.Status = "SUBMITTED";
                    kyc.UserID = user.UserID;
                    kyc.DocumentType = u.DocumentType;
                    kyc.DocumentNumber = u.DocumentNumber;
                    kyc.SelfiePath = api.UploadFile(u.Selfie);
                    kyc.DocumentPath = api.UploadFile(u.Document);
                    kyc.OTP = u.KYCOTP;
                    kyc.CreatedBy = Email;
                    kyc.DateCreated = DateTime.Now;
                    kyc.Active = true;

                    db.KYCs.Add(kyc);
                    db.SaveChanges();

                }
                else
                {
                    throw new Exception("Failed to verify your account, please try again later");
                }

                Data = "Your documents were submitted successfully, your account will be verified within 24 hours";
            }
            catch (Exception ex)
            {
                Status = "Failed";
                Data = ex.Message;
                string Error = ex.Message + "\n" + ex.InnerException + "\n" + ex.StackTrace;
                LogID = api.Log(LogID, null, "KYC", null, Error);
            }
            return Json(new { Status, Data }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetKYC()
        {
            string Status = "Success";
            string Data = "";

            int LogID = 0;
            try
            {
                string Email = Session["Email"].ToString();

                LogID = api.Log(LogID, Email, "GetKYC", null, null);

                var k = (from a in db.KYCs
                         join b in db.Users on a.UserID equals b.UserID
                         where a.Active == true
                         select new KYCModel
                         {
                             ID = a.ID,
                             UserID = a.UserID,
                             DocumentNumber = a.DocumentNumber,
                             DocumentType = a.DocumentType,
                             SelfiePath = a.SelfiePath,
                             DocumentPath = a.DocumentPath,
                             KYCOTP = a.OTP,
                             Status = a.Status,
                             Active = a.Active,
                             CreatedBy = a.CreatedBy,
                             DateCreated = a.DateCreated,

                             Name = b.Name,
                             Surname = b.Surname,
                             Email = b.Email,


                         }).ToList();

                Data = JsonConvert.SerializeObject(k);

            }
            catch (Exception ex)
            {
                Status = "Failed";
                Data = ex.Message;
                string Error = ex.Message + "\n" + ex.InnerException + "\n" + ex.StackTrace;
                LogID = api.Log(LogID, null, "GetKYC", null, Error);
            }
            return Json(new { Status, Data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult KYCDocs(int ID)
        {

            string Status = "Success";
            string Data = "";
            string Selfie = "";
            string Doc = "";
            int LogID = 0;
            try
            {
                string Email = Session["Email"].ToString();
                string UserID = Session["UserID"].ToString();

                LogID = api.Log(LogID, Email, "KYCDocs", JsonConvert.SerializeObject(ID), null);

                //Validate input
                if (string.IsNullOrEmpty(ID.ToString()))
                {
                    throw new Exception("KYC ID required");
                }

                if (!api.IsSystemAdmin(UserID))
                {
                    throw new Exception("Access denied, unauthorized");
                }

                var k = (from a in db.KYCs where a.ID == ID select a).FirstOrDefault();

                if (k != null)
                {
                    Doc = k.DocumentPath;
                    Selfie = k.SelfiePath;
                }
                else
                {
                    throw new Exception("KYC documents not found");
                }

            }
            catch (Exception ex)
            {
                Status = "Failed";
                Data = ex.Message;
                string Error = ex.Message + "\n" + ex.InnerException + "\n" + ex.StackTrace;
                LogID = api.Log(LogID, null, "KYCDocs", null, Error);
            }
            return Json(new { Status, Data, Selfie, Doc }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ProcessKYC(int ID, int KYCStatus)
        {

            string Status = "Success";
            string Data = "";
            int LogID = 0;
            string emailContent = "";
            try
            {
                string Email = Session["Email"].ToString();
                string UserID = Session["UserID"].ToString();

                LogID = api.Log(LogID, Email, "ProcessKYC", JsonConvert.SerializeObject("KYC ID: " + ID + " Status: " + KYCStatus), null);

                //Validate input
                if (string.IsNullOrEmpty(ID.ToString()))
                {
                    throw new Exception("KYC ID required");
                }

                if (!api.IsSystemAdmin(UserID))
                {
                    throw new Exception("Access denied, unauthorized");
                }

                var k = (from a in db.KYCs where a.ID == ID select a).FirstOrDefault();

                if (k != null)
                {
                    if (k.Status != "SUBMITTED")
                    {
                        throw new Exception("KYC already processed");
                    }

                    var u = (from a in db.Users where a.UserID == k.UserID select a).FirstOrDefault();

                    if (u != null)
                    {
                        if (KYCStatus == 1)
                        {
                            u.DocumentNumber = k.DocumentNumber;
                            u.DocumentType = k.DocumentType;
                            u.Verified = true;
                            u.ModifiedBy = Email;
                            u.DateModified = DateTime.Now;

                            k.Status = "APPROVED";
                            k.Active = false;
                            k.ModifiedBy = Email;
                            k.DateModified = DateTime.Now;

                            db.SaveChanges();

                            emailContent = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"Templates\KYCApproved.html");
                            emailContent = emailContent.Replace("{Name}", u.Name);
                            emailContent = emailContent.Replace("{AccountCode}", u.ReferralCode);

                            api.SendEmail(u.Email, emailContent, "FICA verification");

                            Data = "KYC approved";
                        }
                        else
                        {
                            u.Verified = false;
                            u.ModifiedBy = Email;
                            u.DateModified = DateTime.Now;

                            k.Status = "REJECTED";
                            k.Active = false;
                            k.ModifiedBy = Email;
                            k.DateModified = DateTime.Now;

                            db.SaveChanges();

                            api.SendEmail(u.Email, emailContent, "FICA verification");

                            Data = "KYC rejected";
                        }
                    }
                    else
                    {
                        throw new Exception("User not found");
                    }
                }
                else
                {
                    throw new Exception("KYC not found");
                }

            }
            catch (Exception ex)
            {
                Status = "Failed";
                Data = ex.Message;
                string Error = ex.Message + "\n" + ex.InnerException + "\n" + ex.StackTrace;
                LogID = api.Log(LogID, null, "ProcessKYC", null, Error);
            }
            return Json(new { Status, Data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Users(string Term, string Filter)
        {

            string Status = "Success";
            string Data = "";
            int LogID = 0;
            try
            {
                string Email = Session["Email"].ToString();
                string UserID = Session["UserID"].ToString();

                LogID = api.Log(LogID, Email, "UserList", JsonConvert.SerializeObject("Term : " + Term + " Filter: " + Filter), null);

                //Validate input
                if (string.IsNullOrEmpty(Filter))
                {
                    throw new Exception("Search criteria required");
                }

                if (string.IsNullOrEmpty(Term))
                {
                    throw new Exception("Search term required");
                }

                if (!api.IsSystemAdmin(UserID))
                {
                    throw new Exception("Access denied, unauthorized");
                }


                Data = JsonConvert.SerializeObject(api.Users(UserID, Term, Filter));

            }
            catch (Exception ex)
            {
                Status = "Failed";
                Data = ex.Message;
                string Error = ex.Message + "\n" + ex.InnerException + "\n" + ex.StackTrace;
                LogID = api.Log(LogID, null, "UserList", null, Error);
            }
            return Json(new { Status, Data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public new ActionResult User(int ID)
        {

            string Status = "Success";
            string Data = "";
            int LogID = 0;
            bool exist = false;
            try
            {
                string Email = Session["Email"].ToString();
                string UserID = Session["UserID"].ToString();

                LogID = api.Log(LogID, Email, "User", JsonConvert.SerializeObject(ID), null);

                //Validate input
                if (string.IsNullOrEmpty(ID.ToString()))
                {
                    throw new Exception("User ID required");
                }

                if (!api.IsSystemAdmin(UserID))
                {
                    throw new Exception("Access denied, unauthorized");
                }

                dynamic u = api.User(ID.ToString());

                Type typeOfDynamic = u.GetType();

                if (!( exist = typeOfDynamic.GetProperties().Where(p => p.Name.Equals("Status")).Any()))
                {
                    Data = JsonConvert.SerializeObject(u);
                }
                else
                {
                    throw new Exception(u.Data);
                }
            }
            catch (Exception ex)
            {
                Status = "Failed";
                Data = ex.Message;
                string Error = ex.Message + "\n" + ex.InnerException + "\n" + ex.StackTrace;
                LogID = api.Log(LogID, null, "User", null, Error);
            }
            return Json(new { Status, Data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public  ActionResult AccountReload(string UserID, decimal Amount)
        {

            string Status = "Success";
            string Data = "";
            int LogID = 0;
            var gm = new GenericModel();
            try
            {
                string Email = Session["Email"].ToString();
                string Admin = Session["UserID"].ToString();

                LogID = api.Log(LogID, Email, "AccountReload", JsonConvert.SerializeObject("RecipientID: " + UserID + " Amount: " + Amount), null);

                //Validate input
                if (string.IsNullOrEmpty(UserID))
                {
                    throw new Exception("User ID required");
                }

                if (string.IsNullOrEmpty(Amount.ToString()))
                {
                    throw new Exception("Amount required");
                }

                if (!api.IsSystemAdmin(Admin))
                {
                    throw new Exception("Access denied, unauthorized");
                }

                gm = (GenericModel)api.AccountReload(Email, UserID, Amount, "Cash Deposit");

                if (gm.Status == "Success")
                {
                    gm.Status = Status;
                    gm.Data = Data;
                }
                else
                {
                    throw new Exception(gm.Data);
                }

            }
            catch (Exception ex)
            {
                Status = "Failed";
                Data = ex.Message;
                string Error = ex.Message + "\n" + ex.InnerException + "\n" + ex.StackTrace;
                LogID = api.Log(LogID, null, "AccountReload", null, Error);
            }
            return Json(new { Status, Data }, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}