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
    public class AdminController : Controller
    {
        fastfoodEntities db = new fastfoodEntities();
        // GET: admin/Admin

        public IEnumerable<MONAN> ProductsPaged(int? page)
        {
            int PageSize = 8;
            int PageNumber = (page ?? 1);
            return db.MONANs.OrderBy(x => x.MSMONAN).ToPagedList(PageNumber, PageSize);
        }

        public ActionResult getListFood(int? page)
        {
            if (!SessionHelper.IsAdmin()) return Redirect("/admin/Home/Index");
            if (page == null) page = 1;
            var list = ProductsPaged(page);
            //var listfood = from s in db.MONANs select s;
            return View(list);
        }


        private List<SelectListItem> GetTypes()
        {
            var listType = new List<SelectListItem>();
            listType = db.LOAIMONANs.Select(t => new SelectListItem()
            {
                Value = t.MSLOAI.ToString(),
                Text = t.TENLOAI
            }).ToList();
           
            return listType;
        }
        //Thêm sách mới
        public ActionResult CreateFood()
        {
            if (!SessionHelper.IsAdmin()) return Redirect("/admin/Home/Index");
            ViewBag.LOAIMONANs = GetTypes();
            return View();
        }

        [HttpPost, ActionName("CreateFood")]
        [ValidateInput(false)]
        public ActionResult CreateFood(MONAN monan, HttpPostedFileBase fileUpload)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Upload file
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    //Lưu đường dẫn file ảnh 
                    var path = Path.Combine(Server.MapPath("~/Content/Images"), fileName);
                    //Kiểm tra file đã tồn tại
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.ThongBao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        fileUpload.SaveAs(path);
                    }
                    //Them Sach Moi
                    monan.HINHANH = fileUpload.FileName;
                    db.MONANs.Add(monan);
                    db.SaveChanges();
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Error Save Data");
            }
            //Cập nhật lại danh sách hiển thị
            var listfood = from s in db.MONANs select s;
            return RedirectToAction("getListFood");
        }

        //Hiển thị chi tiết
        public ActionResult Details(int? id)
        {
            if (!SessionHelper.IsAdmin()) return Redirect("/admin/Home/Index");
            if (id == null)
            {
                return HttpNotFound();
            }

            var viewModel = db.MONANs.SingleOrDefault(s => s.MSMONAN == id);
            if (viewModel == null)
            {
                return HttpNotFound();
            }
            return View(viewModel);
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
            MONAN monan = db.MONANs.SingleOrDefault(s => s.MSMONAN == id);
            ViewBag.LOAIMONANs = GetTypes();
            return View(monan);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id)
        {
            var foodUpdate = db.MONANs.Find(id);
            if (TryUpdateModel(foodUpdate, "", new string[] { "MSLOAI", "TENMONAN", "DONGIA", "HINHANH" }))
            {
                try
                {
                    db.Entry(foodUpdate).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Error Save Data");
                }
            }

            return RedirectToAction("getListFood");
        }

        //Hiển thị thông tin một sách cần xoá
        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (!SessionHelper.IsAdmin()) return Redirect("/admin/Home/Index");
            MONAN monan = db.MONANs.SingleOrDefault(n => n.MSMONAN == id);
            if (monan == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(monan);
        }

        //Xác nhận xoá
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            MONAN monan = db.MONANs.SingleOrDefault(n => n.MSMONAN == id);
            var path = Path.Combine(Server.MapPath("~/Content/Images"), monan.HINHANH);

            if (monan == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Xoá ảnh trong thư mục ~/Content/Image
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            db.MONANs.Remove(monan);
            db.SaveChanges();
            return RedirectToAction("getListFood");
        }


        public ActionResult Customer()
        {
            var result = from s in db.TAIKHOANs
                         where s.AUTH == 0
                         select s;
            return View(result);
           
        }

    

        public ActionResult hoadon()
        {
            var listorder = from s in db.DATMONANs select s;
            return View(listorder);
        }

        public ActionResult SearchFood(string term, int? page)
        {
            if (term == null) return Redirect("/admin/Admin/getListFood");
            if (page == null) page = 1;
            var s = term.ToLower();
            var result = db.MONANs
                            .Where(b => b.TENMONAN.ToLower().Contains(s) ||

                                   (b.LOAIMONAN.TENLOAI.ToString() ?? String.Empty).ToLower().Contains(s))
                            .OrderBy(x => x.MSMONAN)
                            .ToList();
            int PageSize = 8;
            int PageNumber = (page ?? 1);
            return View(result.ToPagedList(PageNumber, PageSize));
        }

        public ActionResult AcceptBill(int id, int trangthai)
        {

            DATMONAN datmonan = ((List<DATMONAN>)Session["OrderList"]).Find(a => a.MSDATMONAN == id);
            string err = "";
            
            if (trangthai != 1)
            {
                err = "Đã xác nhận";
            }
            else
            {
                datmonan.TRANGTHAI = 2;
                db.Entry(datmonan).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new { ItemAmount = datmonan.TRANGTHAI});
        }



    

    }

}