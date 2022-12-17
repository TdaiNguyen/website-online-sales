using FASTFOOD.Code;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace FASTFOOD.Areas.admin.Controllers
{

    public class TypeController : Controller
    {
        fastfoodEntities db = new fastfoodEntities();
        // GET: admin/Type
        public ActionResult getListType(int? page)
        {
            //var listType = from s in db.LOAIMONANs select s;
            //return View(listType);

            if (!SessionHelper.IsAdmin()) return Redirect("/admin/Home/Index");
            if (page == null) page = 1;
            var list = (from s in db.LOAIMONANs select s).OrderBy(x => x.MSLOAI);
            int PageSize = 4;
            int PageNumber = (page ?? 1);
            return View(list.ToPagedList(PageNumber, PageSize));

        }

        public ActionResult CreateType()
        {
            if (!SessionHelper.IsAdmin()) return Redirect("/admin/Home/Index");
            return View();
        }

        [HttpPost, ActionName("CreateType")]
        [ValidateInput(false)]
        public ActionResult CreateType(LOAIMONAN loaimonan)
        {

            try
            {
                if (ModelState.IsValid)
                {

                    db.LOAIMONANs.Add(loaimonan);
                    db.SaveChanges();
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Error Save Data");
            }
            //Cập nhật lại danh sách hiển thị
            var listfood = from s in db.MONANs select s;
            return RedirectToAction("getListType");
        }
        //Chỉnh sửa thông tin món ăn
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (!SessionHelper.IsAdmin()) return Redirect("/admin/Home/Index");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LOAIMONAN monan = db.LOAIMONANs.SingleOrDefault(s => s.MSLOAI == id);
            return View(monan);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id)
        {
            var typeUpdate = db.LOAIMONANs.Find(id);
            if (TryUpdateModel(typeUpdate, "", new string[] { "TENLOAI" }))
            {
                try
                {
                    db.Entry(typeUpdate).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Error Save Data");
                }
            }

            return RedirectToAction("getListType");
        }

        //Hiển thị thông tin một sách cần xoá

        //public ActionResult Delete(int? id)
        //{
        //    if (SessionHelper.GetSession() == null) return Redirect("/admin/Type/Delete");
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    LOAIMONAN loaimonan = db.LOAIMONANs.SingleOrDefault(n => n.MSLOAI == id);

        //    if (loaimonan == null)
        //    {
        //        Response.StatusCode = 404;
        //        return null;
        //    }
        //    var fkeyExist = db.MONANs.Any(x => x.MSLOAI == loaimonan.MSLOAI);
        //    if (fkeyExist)
        //    {
        //        TempData["alert"] = "Các sản phẩm thuộc loại này tồn tại!";
        //        return RedirectToAction("getListType");
        //    }

           
        //    db.LOAIMONANs.Remove(loaimonan);
        //    db.SaveChanges();
        //    return RedirectToAction("getListType");
        //}

        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (!SessionHelper.IsAdmin()) return Redirect("/admin/Home/Index");
            LOAIMONAN loaimonan = db.LOAIMONANs.SingleOrDefault(n => n.MSLOAI == id);
            if (loaimonan == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(loaimonan);
        }

        //Xác nhận xoá
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            LOAIMONAN loaimonan = db.LOAIMONANs.SingleOrDefault(n => n.MSLOAI == id);
         

            if (loaimonan == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            var fkeyExist = db.MONANs.Any(x => x.MSLOAI == loaimonan.MSLOAI);
            if (fkeyExist)
            {
                TempData["alert"] = "Các sản phẩm thuộc loại này tồn tại!";
                return RedirectToAction("getListType");
            }

            db.LOAIMONANs.Remove(loaimonan);
            db.SaveChanges();
            return RedirectToAction("getListType");
        }
    }


 

}
