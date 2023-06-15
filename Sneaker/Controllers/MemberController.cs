using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Sneaker.Models;

namespace Sneaker.Controllers
{
    public class MemberController : Controller
    {
        // GET: NguoiDung
        DataYikesDataContext data = new DataYikesDataContext();
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(FormCollection collection, KhachHang kh)
        {
            var hoten = collection["HoTenKH"];
            var tendn = collection["TenDN"];
            var matkhau = collection["MatKhau"];
            var matkhaunhaplai = collection["MatKhauNhapLai"];
            var diachi = collection["DiaChi"];
            var email = collection["Email"];
            var dienthoai = collection["DienThoai"];

            // Kiểm tra tên đăng nhập đã tồn tại chưa
            var existingUsername = data.KhachHangs.FirstOrDefault(n => n.TaiKhoan == tendn);
            if (existingUsername != null)
            {
                ViewData["loi2"] = "Tên đăng nhập đã được sử dụng";
            }

            // Kiểm tra email đã tồn tại chưa
            var existingEmail = data.KhachHangs.FirstOrDefault(n => n.Email == email);
            if (existingEmail != null)
            {
                ViewData["loi6"] = "Email đã được sử dụng";
            }

            if (String.IsNullOrEmpty(hoten))
            {
                ViewData["loi1"] = "Họ tên khách hàng không được để trống";
            }
            else if (string.IsNullOrEmpty(tendn))
            {
                ViewData["loi2"] = "Phải nhập tên đăng nhập";
            }
            else if (string.IsNullOrEmpty(matkhau))
            {
                ViewData["loi3"] = "Phải nhập mật khẩu";
            }
            else if (string.IsNullOrEmpty(matkhaunhaplai))
            {
                ViewData["loi4"] = "Phải nhập lại mật khẩu";
            }
            if (string.IsNullOrEmpty(diachi))
            {
                ViewData["loi5"] = "Địa chỉ không được bỏ trống";
            }
            if (string.IsNullOrEmpty(dienthoai))
            {
                ViewData["loi7"] = "Phải nhập điện thoại";
            }
            else if (existingUsername == null && existingEmail == null)
            {
                kh.HoTen = hoten;
                kh.TaiKhoan = tendn;
                kh.MatKhau = matkhau;
                kh.Email = email;
                kh.DiaChiKH = diachi;
                kh.DienThoaiKH = dienthoai;
                data.KhachHangs.InsertOnSubmit(kh);
                data.SubmitChanges();
                return RedirectToAction("Login");
            }

            return this.Register();
        }
        public ActionResult Login(FormCollection collection)
        {
            var tendn = collection["TenDN"];
            var matkhau = collection["MatKhau"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["loi1"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["loi2"] = "Phải nhập mật khẩu";
            }
            else
            {
                KhachHang kh = data.KhachHangs.SingleOrDefault(n => n.TaiKhoan == tendn && n.MatKhau == matkhau);
                if (kh != null)
                {
                    Session["TaiKhoan"] = kh;
                    Session["HoTen"] = kh.HoTen; // Thêm thông tin tên người dùng vào Session
                    return RedirectToAction("Index", "Home");
                }
                else
                    ViewBag.Thongbao = "TÊN ĐĂNG NHẬP HOẶC MẬT KHẨU KHÔNG ĐÚNG";
            }
            return View();
        }
        public ActionResult Logout()
        {
            Session["TaiKhoan"] = null;
            Session["Cart"] = null;
            return RedirectToAction("Index", "Home");
        }
        public ActionResult ChiTietKH(int id)
        {
            KhachHang item = data.KhachHangs.SingleOrDefault(n => n.MaKH == id);
            ViewBag.MaKH = item.MaKH;
            if (item == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(item);
        }
        public ActionResult DonDatHang(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;

            if (Session["TaiKhoan"] != null)
            {
                KhachHang khachHang = (KhachHang)Session["TaiKhoan"];
                int maKH = khachHang.MaKH;
                var donDatHangs = data.DonDatHangs.Where(n => n.MaKH == maKH).OrderBy(n => n.MaDonHang);
                return View(donDatHangs.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                // Xử lý khi chưa đăng nhập
                return RedirectToAction("Login", "Member");
            }
        }
        [HttpGet]
        public ActionResult SuaKH(int id)
        {
        
            KhachHang kh = data.KhachHangs.SingleOrDefault(n => n.MaKH == id);
            if (kh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(kh);
        }

        [HttpPost]
        public ActionResult SuaKH(KhachHang kh)
        {
            if (ModelState.IsValid)
            {
                KhachHang khachHang = data.KhachHangs.SingleOrDefault(n => n.MaKH == kh.MaKH);
                if (khachHang == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }

                // Cập nhật thông tin người dùng
                khachHang.HoTen = kh.HoTen;
                khachHang.TaiKhoan = kh.TaiKhoan;
                khachHang.MatKhau = kh.MatKhau;
                khachHang.Email = kh.Email;
                khachHang.DiaChiKH = kh.DiaChiKH;
                khachHang.DienThoaiKH = kh.DienThoaiKH;

                // Kiểm tra tài khoản đã tồn tại chưa
                var existingUsername = data.KhachHangs.FirstOrDefault(n => n.TaiKhoan == kh.TaiKhoan && n.MaKH != kh.MaKH);
                if (existingUsername != null)
                {
                    ViewData["existingUsernameError"] = "Tên đăng nhập đã được sử dụng";
                }

                // Kiểm tra email đã tồn tại chưa
                var existingEmail = data.KhachHangs.FirstOrDefault(n => n.Email == kh.Email && n.MaKH != kh.MaKH);
                if (existingEmail != null)
                {
                    ViewData["existingEmailError"] = "Email đã được sử dụng";
                }
               
                // Kiểm tra nếu không có thông báo lỗi tài khoản và email
                if (existingUsername == null && existingEmail == null)
                {
                    data.SubmitChanges();
                    return RedirectToAction("ChiTietKH", new { id = kh.MaKH });
                }
            }

            return View(kh);
        }


        public ActionResult ChiTietDH(int id)
        {
            KhachHang khachHang = data.KhachHangs.SingleOrDefault(n => n.MaKH == id);
            if (khachHang == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            var donDatHangs = data.DonDatHangs.Where(n => n.MaKH == id).ToList();
            List<ChiTietDatHang> chiTietDatHangList = new List<ChiTietDatHang>();
            foreach (var donDatHang in donDatHangs)
            {
                var chiTietDatHangs = data.ChiTietDatHangs.Where(n => n.MaDonHang == donDatHang.MaDonHang).ToList();
                chiTietDatHangList.AddRange(chiTietDatHangs);
            }

            ViewBag.MaKH = id;
            if (chiTietDatHangList.Count == 0)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(chiTietDatHangList);
        }
    }
}