using FASTFOOD.Code;
using FASTFOOD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FASTFOOD.Areas.admin.Controllers
{
    public class HomeController : Controller
    {
        // GET: admin/Home
        public ActionResult Index()
        {
            if (SessionHelper.IsAdmin())
            { return Redirect("/admin/Admin"); }
            else return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginModel model)
        {
            var result = new AccountModel().Login(model.Email, model.Password);
            var permission = new AccountModel().GetPermission(model.Email);
            if (permission)
            {
                if (result && ModelState.IsValid)
                {
                    //Nếu thành công chúng ta cần tạo session
                    SessionHelper.SetSession(new UserSession(model.Email));
                    SessionHelper.SetAdmin();
                    return Redirect("/admin/Admin/getListFood");
                }
                else
                { ViewBag.error = "Tài khoản hoặc mặt khẩu không chính xác!"; }
            }
            else { ViewBag.error = "Tài khoản không hợp lệ"; }
            return View(model);
        }
    }
}