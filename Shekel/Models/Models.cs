using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shekel.Models
{
    public class Login
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class GenericModel
    {
        public string Status { get; set; }
        public string Data { get; set; }

    }

    public class CountryModel
    {
        public int ID { get; set; }
        public string CountryID { get; set; }
        public string Name { get; set; }
        public string Iso2 { get; set; }
        public string Iso3 { get; set; }
        public string PhoneCode { get; set; }
        public string UNCode { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public System.DateTime DateCreated { get; set; }
        public Nullable<System.DateTime> DateModified { get; set; }
    }

    public class UserModel
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public string UserType { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public Nullable<System.DateTime> Age { get; set; }
        public string Gender { get; set; }
        public string Nationality { get; set; }
        public string Country { get; set; }
        public string Password { get; set; }
        public string LastPassword { get; set; }
        public System.DateTime LastLogin { get; set; }
        public int LoginAttempts { get; set; }
        public bool LockedOut { get; set; }
        public string ReferralCode { get; set; }
        public Nullable<bool> Verified { get; set; }
        public string DocumentType { get; set; }
        public string DocumentNumber { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public System.DateTime DateCreated { get; set; }
        public Nullable<System.DateTime> DateModified { get; set; }
        public string DisplayName { get; set; }
        public decimal Balance { get; set; }

    }

    public class PasswordModel
    {
        public string OPassword { get; set; }
        public string NPassword { get; set; }
    }

    public partial class KYCModel
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public string DocumentNumber { get; set; }
        public string DocumentType { get; set; }
        public string SelfiePath { get; set; }
        public string DocumentPath { get; set; }
        public string Status { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public System.DateTime DateCreated { get; set; }
        public Nullable<System.DateTime> DateModified { get; set; }
        public HttpPostedFileBase Selfie { get; set; }
        public HttpPostedFileBase Document { get; set; }
        public string KYCOTP { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
    }
}