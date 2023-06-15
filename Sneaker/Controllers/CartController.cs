using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sneaker.Models;

namespace Sneaker.Controllers
{
    public class CartController : Controller
    {
        DataYikesDataContext data = new DataYikesDataContext();
        public List<Cart> TakeACart()
        {
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if (lstCart == null)
            {
                lstCart = new List<Cart>();
                Session["Cart"] = lstCart;
            }
            return lstCart;
        }
        public ActionResult AddToCart(int iMaSP, string strUrl)
        {
            List<Cart> lstCart = TakeACart();
            Cart sanpham = lstCart.Find(n => n.iMaSP == iMaSP);
            if (sanpham == null)
            {
                sanpham = new Cart(iMaSP);
                lstCart.Add(sanpham);
                return Redirect(strUrl);
            }
            else
            {
                sanpham.iSoLuong++;
                return Redirect(strUrl);
            }
        }
        public ActionResult Logout()
        {
            // Lưu giỏ hàng vào TempData để giữ lại sau khi đăng xuất
            TempData["Cart"] = Session["Cart"];
            Session["TaiKhoan"] = null;
            Session["Cart"] = null;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Login()
        {
            // Khôi phục giỏ hàng từ TempData sau khi đăng nhập lại
            List<Cart> cart = TempData["Cart"] as List<Cart>;
            if (cart != null)
            {
                Session["Cart"] = cart;
            }

            return RedirectToAction("Index", "Home");
        }
        private int TotalQuantity()
        {
            int iTotalQuantity = 0;
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if (lstCart != null)
            {
                iTotalQuantity = lstCart.Sum(n => n.iSoLuong);
            }
            return iTotalQuantity;
        }
        private double TotalAmount()
        {
            double iTotalAmount = 0;
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if (lstCart != null)
            {
                iTotalAmount = lstCart.Sum(n => n.dThanhTien);
            }
            return iTotalAmount;
        }
        public ActionResult Cart()
        {
            List<Cart> lstCart = TakeACart();
            if (lstCart.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }
            ViewBag.TotalQuantity = TotalQuantity();
            ViewBag.TotalAmount = TotalAmount();
            return View(lstCart);
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CartPartial()
        {
            ViewBag.TotalQuantity = TotalQuantity();
            ViewBag.TotalAmount = TotalAmount();
            return PartialView();
        }
        public ActionResult DeleteCart(int iMaSP)
        {
            List<Cart> lstCart = TakeACart();
            Cart sanpham = lstCart.SingleOrDefault(n => n.iMaSP == iMaSP);
            if (sanpham != null)
            {
                lstCart.RemoveAll(n => n.iMaSP == iMaSP);
                return RedirectToAction("Cart");
            }
            if (lstCart.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Cart");
        }
        public ActionResult UpdateCart(int iMaSP, FormCollection f)
        {
            List<Cart> lstCart = TakeACart();
            Cart sanpham = lstCart.SingleOrDefault(n => n.iMaSP == iMaSP);
            if (sanpham != null)
            {
                int quantity = int.Parse(f["txtSoLuong"].ToString());
                if (quantity > 0)
                {
                    sanpham.iSoLuong = quantity;
                }
                else
                {
                    lstCart.RemoveAll(n => n.iMaSP == iMaSP);
                }
            }
            return RedirectToAction("Cart");
        }

        public ActionResult DeleteAllCart()
        {
            List<Cart> lstCart = TakeACart();
            lstCart.Clear();
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public ActionResult Order()
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("Register", "Member");
            }
            if (Session["Cart"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            // lấy giỏ hàng từ session
            List<Cart> lstCart = TakeACart();
            ViewBag.TotalQuantity = TotalQuantity();
            ViewBag.TotalAmount = TotalAmount();
            return View(lstCart);
        }
        public ActionResult Order(FormCollection collection)
        {
            DonDatHang ddh = new DonDatHang();
            KhachHang kh = (KhachHang)Session["TaiKhoan"];
            List<Cart> gh = TakeACart();
            ddh.MaKH = kh.MaKH;
            ddh.NgayDat = DateTime.Now;
            data.DonDatHangs.InsertOnSubmit(ddh);
            data.SubmitChanges();
            // thêm chi tiết đơn hàng
            foreach (var item in gh)
            {
                ChiTietDatHang ctdh = new ChiTietDatHang();
                ctdh.MaDonHang = ddh.MaDonHang;
                ctdh.MaSP = item.iMaSP;
                ctdh.SoLuong = item.iSoLuong;
                ctdh.DonGia = (decimal)item.dGiaBan;
                data.ChiTietDatHangs.InsertOnSubmit(ctdh);
            }
            data.SubmitChanges();
            Session["Cart"] = null;
            return RedirectToAction("OrderSuccess", "Cart");

        }
        public ActionResult OrderSuccess()
        {
            return View();
        }
    }
}