using CloudinaryDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Shekel.Models;
using System.IO;
using CloudinaryDotNet.Actions;
using System.Net.Mail;
using System.Text;
using Newtonsoft.Json;

namespace Shekel.Controllers
{
    public class Methods
    {
        #region Variables

        string SMTP = System.Configuration.ConfigurationManager.AppSettings["SMTP"].ToString();
        string SMTPPort = System.Configuration.ConfigurationManager.AppSettings["SMTPPort"].ToString();
        string SMTPUserName = System.Configuration.ConfigurationManager.AppSettings["SMTPUserName"].ToString();
        string SMTPPassword = System.Configuration.ConfigurationManager.AppSettings["SMTPPassword"].ToString();

        static readonly Account account = new Account(
             "kazumbafoundation",
             "473666744374452",
             "5P1OtG-jl97yFrfzIquZIRWy3ww");

        Cloudinary cloudinary = new Cloudinary(account);

        DB.ShekelEntities db = new DB.ShekelEntities();

        #endregion

        #region Methods

        public int Log(int ID, string UserID, string Method, string Request, string Response)
        {
            var newlog = new DB.Log();
            int LogID = 0;
            try
            {

                var log = (from a in db.Logs where a.ID == ID select a).FirstOrDefault();
                if (log != null)
                {

                    log.ResponseData = Response;
                    log.DateModified = DateTime.Now;
                    db.SaveChanges();

                    LogID = log.ID;
                }
                else
                {
                    newlog.UserID = UserID;
                    newlog.Method = Method;
                    newlog.RequestData = Request;
                    newlog.DateCreated = DateTime.Now;
                    db.Logs.Add(newlog);
                    db.SaveChanges();

                    LogID = newlog.ID;
                }


            }
            catch (Exception)
            {
                LogID = 0;
            }

            return LogID;
        }

        public object Countries()
        {
            int LogID = 0;
            var gm = new GenericModel();
            try
            {
                LogID = Log(LogID, null, "Countries", null, null);
                var x = (from a in db.Countries where a.Active == true select a).ToList();

                return x;
            }
            catch (Exception ex)
            {
                gm.Status = "Failed";
                string Error = ex.Message + "\n" + ex.InnerException + "\n" + ex.StackTrace;
                LogID = Log(LogID, null, "Countries", null, Error);

                return gm;
            }


        }

        public string UserDuplicate(string Text, string Type)
        {
            string Status = "";
            var duplicate = "";
            try
            {
                if (Type == "Email")
                {
                    duplicate = (from a in db.Users where a.Email == Text select a.Email).FirstOrDefault();
                }

                if (Type == "Telephone")
                {
                    duplicate = (from a in db.Users where a.Telephone == Text select a.Telephone).FirstOrDefault();
                }

                if (duplicate != null)
                {
                    Status = "YES";
                }
                else
                {
                    Status = "NO";
                }

            }
            catch (Exception)
            {
                Status = "YES";
            }
            return Status;
        }

        public object User(string UserID)
        {
            int LogID = 0;
            var gm = new GenericModel();
            try
            {
                LogID = Log(LogID, UserID, "GetUser", UserID, null); 

                int ID;
                bool isNumeric = int.TryParse(UserID, out ID);

                var u = (from a in db.Users 
                         join b in db.Accounts on a.UserID equals b.UserID
                         join c in db.Countries on a.Country equals c.Iso2
                         where a.UserID == UserID || a.ID == ID select 
                         new UserModel { 
                         
                             ID = a.ID,   
                             UserID = a.UserID,   
                             UserType = a.UserType,   
                             DisplayName = a.DisplayName,   
                             Name = a.Name,   
                             Surname = a.Surname,   
                             Email = a.Email,   
                             Telephone = a.Telephone,   
                             Country = c.Name,   
                             LastLogin = a.LastLogin,   
                               
                             ReferralCode = a.ReferralCode,
                             LockedOut = a.LockedOut,
                             Verified=  a.Verified,
                             DocumentType = a.DocumentType,
                             DocumentNumber = a.DocumentNumber,
                             Active = a.Active,
                             DateModified = a.DateModified,
                             ModifiedBy = a.ModifiedBy,
                             Balance = b.Balance,
                         }).FirstOrDefault();

                if (u != null)
                {
                    return u;
                }
                else
                {
                    throw new Exception("User not found");
                }
            }
            catch (Exception ex)
            {
                gm.Status = "Failed";
                gm.Data = ex.Message;
                string Error = ex.Message + "\n" + ex.InnerException + "\n" + ex.StackTrace;
                LogID = Log(LogID, null, "GetUser", null, Error);

                return gm;
            }
        }

        public object Users(string UserID, string Term, string Filter)
        {
            int LogID = 0;
            var gm = new GenericModel();
            try
            {
                LogID = Log(LogID, UserID, "Users", UserID, null);

                if (Filter == "Email")
                {
                     var k = (from a in db.Users where a.Email.Contains(Term) select a).ToList();
                    return k;
                }

                else if (Filter == "Name")
                {
                     var k = (from a in db.Users where a.Name.Contains(Term) select a).ToList();
                    return k;
                }

                else
                {
                     var k = (from a in db.Users where a.Telephone.Contains(Term) select a).ToList();
                    return k;
                }
            
            }
            catch (Exception ex)
            {
                gm.Status = "Failed";
                gm.Data = ex.Message;
                string Error = ex.Message + "\n" + ex.InnerException + "\n" + ex.StackTrace;
                LogID = Log(LogID, null, "Users", null, Error);

                return gm;
            }
        }

        public void NewReferral(string Referrer, string Referrent)
        {
            int LogID = 0;
            var gm = new GenericModel();
            var refe = new DB.Referral();
            var bon = new DB.Bonus();
            var input = string.Format("Referrer: {0} and Referrent: {1}", Referrer, Referrent);
            try
            {
                LogID = Log(LogID, Referrent, "NewReferral", input, null);

                var u = (from a in db.Users where a.ReferralCode == Referrer select a).FirstOrDefault(); //the one who reffered

                if (u != null)
                {
                    var x = (from a in db.Users where a.ReferralCode == Referrent select a).FirstOrDefault(); //the one being referred

                    if (x != null)
                    {
                        refe.Referrer = Referrer;
                        refe.Referent = Referrent;
                        refe.Active = true;
                        refe.CreatedBy = x.Email;
                        refe.DateCreated = DateTime.Now;
                        db.Referrals.Add(refe);
                        db.SaveChanges();
                    }
                    else
                    {
                        throw new Exception("Referrent not found");
                    }
                }
                else
                {
                    throw new Exception("Referrer not found");
                }
            }
            catch (Exception ex)
            {
                gm.Status = "Failed";
                gm.Data = ex.Message;
                string Error = ex.Message + "\n" + ex.InnerException + "\n" + ex.StackTrace;
                LogID = Log(LogID, null, "NewReferral", null, Error);
            }
        }

        public string UploadFile(HttpPostedFileBase Logo)
        {
            string Path = "";
            try
            {
                byte[] Byte = new byte[Logo.ContentLength];
                using (BinaryReader theReader = new BinaryReader(Logo.InputStream))
                {
                    Byte = theReader.ReadBytes(Logo.ContentLength);
                }

                Stream Stream = new MemoryStream(Byte);
                Stream.Position = 0;

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(Logo.FileName, Stream),
                    PublicId = Logo.FileName,
                };

                var Result = cloudinary.Upload(uploadParams);

                Path = Result.SecureUrl.AbsoluteUri;

            }
            catch (Exception)
            {
                Path = "Failed";
            }
            return Path;
        }

        public string KYCStatus(string UserID)
        {
            string Status = "";
            int LogID = 0;
            try
            {
                LogID = Log(LogID, UserID, "KYCStatus", UserID, null);
                var u = (from a in db.KYCs where a.UserID == UserID && a.Active == true orderby a.ID descending select a).First();

                if (u != null)
                {
                    Status = u.Status;
                }
                else
                {
                    throw new Exception("User not found in submitted kyc's");
                }

            }
            catch (Exception ex)
            {
                Status = "REJECTED";
                string Error = ex.Message + "\n" + ex.InnerException + "\n" + ex.StackTrace;
                LogID = Log(LogID, null, "KYCStatus", null, Error);
            }
            return Status;
        }

        public bool IsSystemAdmin(string UserID)
        {
            bool Status = false;
            int LogID = 0;
            try
            {
                LogID = Log(LogID, UserID, "IsSystemAdmin", UserID, null);
                var u = (from a in db.Users where a.UserID == UserID || a.Email == UserID select a).First();

                if (u != null)
                {
                    if (u.UserType == "SystemAdmin")
                    {
                        Status = true;
                    }
                    else
                    {
                        throw new Exception("Access denied, unauthorized");
                    }
                }
                else
                {
                    throw new Exception("User not found");
                }

            }
            catch (Exception ex)
            {
                Status = false;
                string Error = ex.Message + "\n" + ex.InnerException + "\n" + ex.StackTrace;
                LogID = Log(LogID, null, "IsSystemAdmin", null, Error);
            }
            return Status;
        }

        public void SendEmail(string EmailTo, string Message, string Subject)
        {
            int LogID = 0;
            try
            {
                LogID = Log(LogID, EmailTo, "SendEmail", Message, null);

                //send email
                SmtpClient smtp = new SmtpClient(SMTP)
                {
                    Host = SMTP,
                    Port = int.Parse(SMTPPort),
                    Credentials = new System.Net.NetworkCredential(SMTPUserName, SMTPPassword)
                };

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(SMTPUserName, "TFS"),
                };

                mail.Subject = Subject;

                mail.To.Add(new MailAddress(EmailTo));
                mail.IsBodyHtml = true;
                mail.BodyEncoding = Encoding.UTF8;


                mail.Body = Message;

                mail.IsBodyHtml = true;

                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                string Error = ex.Message + "\n" + ex.InnerException + "\n" + ex.StackTrace;
                LogID = Log(LogID, null, "SendEmail", null, Error);
            }
        }

        public object AccountReload(string Sender, string Recipient, decimal Amount, string Method, string Description)
        {
            int LogID = 0;
            var gm = new GenericModel();
            var T = new DB.Transaction();
         
            try
            {
                string input = "Sender: " + Sender + " RecipientID: " + Recipient + " Amount: " + Amount + " Method: " + Method + " Description: " + Description;

                LogID = Log(LogID, Sender, "AccountReload", JsonConvert.SerializeObject(input), null);

                var RecAccount = (from a in db.Accounts where a.UserID == Recipient select a).FirstOrDefault();

                if(RecAccount != null)
                {
                    var Sen = (from a in db.Users where a.Email == Sender select a).FirstOrDefault();

                    if(Sen != null)
                    {
                        var SenAccount = (from a in db.Accounts where a.UserID == a.UserID select a).FirstOrDefault();

                        var Rec = (from a in db.Users where a.UserID == Recipient select a).FirstOrDefault();

                        if(Rec != null)
                        {
                            RecAccount.Balance = RecAccount.Balance + Amount;
                            RecAccount.ModifiedBy = Sender;
                            RecAccount.DateModified = DateTime.Now;
                            db.SaveChanges();

                            T.TransactionID = Guid.NewGuid().ToString();
                            T.RunningBalance = RecAccount.Balance;
                            T.SourceUserID = ad.UserID;
                            T.SourceTelephone = ad.Telephone;
                            T.SourceEmail = ad.Email;
                            T.SourceCurrency = SenderAcc.Currency;
                            T.SourceAmount = Amount;
                            T.Fee = 0;
                            T.FeeCurrency = acc.Currency;
                            T.DestinationUserID = us.UserID;
                            T.DestinationEmail = us.Email;
                            T.DestinationTelephone = us.Telephone;
                            T.DestinationAmount = Amount - T.Fee;
                            T.DestinationCurrency = acc.Currency;
                            T.TransactionType = "Account Reload";
                            T.Method = Method;
                            T.DescriptionEnglish = "Account reload via cash deposit";
                            T.PaymentReference = ad.ReferralCode;
                            T.Status = "COMPLETED";
                            T.Active = true;
                            T.CreatedBy = ad.Email;
                            T.DateCreated = DateTime.Now;

                            db.Transactions.Add(T);
                            db.SaveChanges();

                            gm.Status = "Success";
                            gm.Data = "Account reloaded";

                        }
                        else
                        {
                            throw new Exception("Invalid user account");
                        }

                    }
                    else
                    {
                        throw new Exception("Invalid admin account");
                    }
                   
                }
                else
                {
                    throw new Exception("Invalid user account");
                }
            }
            catch (Exception ex)
            {
                gm.Status = "Failed";
                string Error = ex.Message + "\n" + ex.InnerException + "\n" + ex.StackTrace;
               Log(LogID, null, "AccountReload", null, Error);
            }
            return gm;
        }

        #endregion
    }
}