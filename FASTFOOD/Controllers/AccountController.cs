using FASTFOOD.Code;
using FASTFOOD.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace FASTFOOD.Controllers
{

    public class AccountController : Controller
    {

        private readonly fastfoodEntities _db = new fastfoodEntities();

        // GET: Login
        public ActionResult Index()
        {
            if (SessionHelper.GetSession() != null)
            { return Redirect("/"); }
            return View();
        }

        //GET: Register
        public ActionResult Register()
        {
            if (SessionHelper.GetSession() != null)
            { return RedirectToAction("Index"); }
            return View();
        }

        //POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            var res = _db.TAIKHOANs.Where(a => a.EMAIL == model.Email.ToString()).SingleOrDefault();
            if (res != null)
            {
                ViewBag.error = "Tài khoản đã tồn tại!";
            }
            else
            {
                if (model.Password == model.ConfirmPassword)
                {
                    TAIKHOAN taikhoan = new TAIKHOAN { FIRSTNAME = model.FirstName, LASTNAME = model.LastName,  EMAIL = model.Email, PASSWORD = model.Password,DIACHI = model.DiaChi, AUTH = 0};
                    try
                    {
                        if (ModelState.IsValid)
                        {
                            //model.Password = GetMD5(model.Password);
                            _db.Configuration.ValidateOnSaveEnabled = false;

                            _db.TAIKHOANs.Add(taikhoan);                          
                            _db.SaveChanges();
                            return RedirectToAction("Index");


                        }
                    }
                    catch (RetryLimitExceededException)
                    {
                        ModelState.AddModelError("", "Error Save Data");
                    }
                }
                else
                {
                    ViewBag.error = "Mật khẩu nhập lại không đúng!";
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginModel model)
        {
            var result = new AccountModel().Login(model.Email, model.Password);
            if (result && ModelState.IsValid)
            {
                SessionHelper.SetSession(new UserSession(model.Email));
                return Redirect("/");

            }
            else
            { 
                ViewBag.ErrorMessage = "Tài khoản hoặc mặt khẩu không chính xác!";
            }
            return View(model);
        
        }




        //Logout
        public ActionResult Logout()
        {
            SessionHelper.RemoveSession();
            return Redirect("/");
        }

        public ActionResult DetailCustomer()
        {
            if (SessionHelper.GetSession() == null)
            { return Redirect("/"); }

            var username = SessionHelper.GetSession().get();
            var profile = _db.TAIKHOANs.Where(s => s.EMAIL == username).SingleOrDefault();

            return View(profile);
        }
        [HttpGet]
        public ActionResult EditInfo(int ? id)
        {
            if (SessionHelper.GetSession() == null)
            { return Redirect("/"); }
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            TAIKHOAN taikhoan = _db.TAIKHOANs.SingleOrDefault(s => s.IDUSER == id); 
            return View(taikhoan);
        }


   

        [HttpPost, ActionName("EditInfo")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditInfo(int id, HttpPostedFileBase file)
        {
            var taikhoanupdate = _db.TAIKHOANs.Find(id);
            if (TryUpdateModel(taikhoanupdate, "", new string[] { "FIRSTNAME", "LASTNAME", "DIACHI", "AVATAR" }))
            {               
                    try
                    {
                        _db.Entry(taikhoanupdate).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                    catch (RetryLimitExceededException)
                    {
                        ModelState.AddModelError("", "Error Save Data");
                    }
                    return RedirectToAction("DetailCustomer");
                
            }

            return View(taikhoanupdate);
        }
      

    }



}
