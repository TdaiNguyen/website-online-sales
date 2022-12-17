using FASTFOOD.Code;
using FASTFOOD.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace FASTFOOD.Controllers
{
    public class FoodController : Controller
    {
        public FoodController()
        {
            GetData();
        }
        private void GetData()
        {
            ViewBag.type = (from s in db.LOAIMONANs select s).OrderBy(t => t.MSLOAI);
           
            ViewBag.count = 0;
        }
        fastfoodEntities db = new fastfoodEntities();
        // GET: Food
        public ActionResult Index(int? page)
        {
            //var listfood = from s in db.MONANs select s;
            //return View(listfood);
            if (page == null) page = 1;
            var list = (from s in db.MONANs select s).OrderBy(x => x.MSMONAN);
            int PageSize = 6;
            int PageNumber = (page ?? 1);
            return View(list.ToPagedList(PageNumber, PageSize));

        }

        [HttpPost]
        public JsonResult AddToCart(int id)
        {
            //Process Add To Cart
            List<CartItem> listCartItem;
            if (Session["ShoppingCart"] == null)
            {
                //Create New Shopping Cart Session 
                listCartItem = new List<CartItem>();
                listCartItem.Add(new CartItem { Quantity = 1, foodOrder = db.MONANs.Find(id) });
                Session["ShoppingCart"] = listCartItem;
            }
            else
            {
                bool flag = false;
                listCartItem = (List<CartItem>)Session["ShoppingCart"];
                foreach (CartItem item in listCartItem)
                {
                    if (item.foodOrder.MSMONAN == id)
                    {
                        item.Quantity++; flag = true;
                        break;
                    }
                }

                if (!flag)
                    listCartItem.Add(new CartItem { Quantity = 1, foodOrder = db.MONANs.Find(id) });

                Session["ShoppingCart"] = listCartItem;
            }
            //Count item in shopping cart 
            int cartcount = 0;
            List<CartItem> ls = (List<CartItem>)Session["ShoppingCart"];
            foreach (CartItem item in ls)
            {
                cartcount += item.Quantity;
            }
            return Json(new { ItemAmount = cartcount });
        }

        [HttpPost]
        public RedirectToRouteResult RemoveCart(int MSMONAN)
        {
            List<CartItem> cart = (List<CartItem>)Session["ShoppingCart"];
            CartItem remove = cart.SingleOrDefault(m => m.foodOrder.MSMONAN == MSMONAN);
            if (remove != null)
            {
                cart.Remove(remove);
            }

            return RedirectToAction("ShoppingCart");
        }


        [HttpPost]
        public JsonResult update(int id, string quantity)
        {
            List<CartItem> listCartItem = ((List<CartItem>)Session["ShoppingCart"]);
            double sum = 0, total = 0;
            string err = "";
            int cartcount = 0;
            if (!int.TryParse(quantity, out int a) || int.Parse(quantity) < 0)
            {
                err = "Số lượng không hợp lệ";
                foreach (CartItem item in listCartItem)
                {
                    if (item.foodOrder.MSMONAN == id)
                    {
                        a = item.Quantity;
                        sum = item.SumPrice();
                        break;
                    }
                }
            }
            else
            {
                if (Session["ShoppingCart"] != null)
                {
                    foreach (CartItem item in listCartItem)
                    {
                        if (item.foodOrder.MSMONAN == id)
                        {
                            item.Quantity = a;
                            sum = item.SumPrice();
                            break;
                        }
                    }
                    Session["ShoppingCart"] = listCartItem;
                }
            }
            foreach(CartItem i in listCartItem)
            {
                total += i.SumPrice();
                cartcount += i.Quantity;

            }
            return Json(new { CartItem = a, SumPrice = sum, Total = total, Error = err, cartcount });

        }

                   

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MONAN monan = db.MONANs.SingleOrDefault(s => s.MSMONAN == id);
            if (monan == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(monan);
        } 
        public ActionResult ShoppingCart()
        {
            return View();
        }

        public RedirectToRouteResult Buy(int Total)
        {
            if (SessionHelper.GetSession() == null) return RedirectToAction("ShoppingCart");
            List<CartItem> cart = (List<CartItem>)Session["ShoppingCart"];
            var user = SessionHelper.GetSession().get();
            TAIKHOAN taikhoan = db.TAIKHOANs.Where(a => a.EMAIL == user).SingleOrDefault();
           
            CreateReceipt(taikhoan, Total, cart);
            return RedirectToAction("OrderList");
        }

        public void CreateReceipt(TAIKHOAN taikhoan, int total, List<CartItem> cart)
        {
            DATMONAN hoadon = new DATMONAN()
            {
                IDUSER = taikhoan.IDUSER,
                THANHTIEN = (long)total,
                NGAYLAP = DateTime.Today,
                TRANGTHAI = 1
            };
            try
            {
                db.DATMONANs.Add(hoadon);
                db.SaveChanges();
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Error Save Data");
            }

            foreach (CartItem item in cart)
            {
                HOADON chitiethoadon = new HOADON()
                {
                    MSDATMONAN = hoadon.MSDATMONAN,
                    MSMONAN = item.foodOrder.MSMONAN,
                    SOLUONG = item.Quantity
                    
                    
                };
                db.HOADONs.Add(chitiethoadon);
                db.SaveChanges();
            }
            Session.Remove("ShoppingCart");

        }

        public ActionResult OrderList (int? page)
        {
            if (SessionHelper.GetSession() == null) return RedirectToAction("Index");
            if (page == null) page = 1;
            int PageSize = 8;
            int PageNumber = (page ?? 1);
            var user = SessionHelper.GetSession().get();
            TAIKHOAN taikhoan = db.TAIKHOANs.Where(a => a.EMAIL == user).SingleOrDefault();
            var list = db.DATMONANs.Where(s => s.IDUSER == taikhoan.IDUSER).OrderBy(x => x.MSDATMONAN).ToPagedList(PageNumber, PageSize);
            return View(list);
        }

        public ActionResult OrderDetails(int? id, int? page)
        {
            if (SessionHelper.GetSession() == null) return RedirectToAction("Index");
            if (page == null) page = 1;
            int PageSize = 8;
            int PageNumber = (page ?? 1);
            var list = db.HOADONs.Where(s => s.MSDATMONAN == id).OrderBy(x => x.IDHD).ToPagedList(PageNumber, PageSize);
            return View(list);
        }


        public ActionResult SearchFood(string term, int? page)
        {
            if (term == null) return Redirect("/Food");
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
        public ActionResult Type(int ?id, int ? page)
        {
            if (page == null) page = 1;
            var result = db.MONANs.Where(i => i.MSLOAI == id).OrderBy(x => x.MSLOAI).ToList();
            int PageSize = 8;
            int PageNumber = (page ?? 1);

            return View("SearchFood", result.ToPagedList(PageNumber, PageSize));
        }
        

        

     
    }
}