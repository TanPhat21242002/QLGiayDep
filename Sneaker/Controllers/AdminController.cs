using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sneaker.Models;
using PagedList;
using PagedList.Mvc;
using System.IO;

namespace Sneaker.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        DataYikesDataContext db = new DataYikesDataContext();
        public ActionResult Index()
        {
            return RedirectToAction("Login", "Admin");
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var tendn = collection["username"];
            var matkhau = collection["password"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }
            else
            {
                Admin ad = db.Admins.SingleOrDefault(n => n.UserAdmin == tendn && n.PassAdmin == matkhau);
                if (ad != null)
                {
                    Session["TaiKhoanAdmin"] = ad;
                    return RedirectToAction("DonDatHang", "Admin");
                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
            return View();
        }
        public ActionResult SanPham(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(db.SanPhams.ToList().OrderBy(n => n.MaSP).ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult ThemSanPham()
        {
            ViewBag.MaThuongHieu = new SelectList(db.ThuongHieus.ToList().OrderBy(n => n.TenThuongHieu), "MaThuongHieu", "TenThuongHieu");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemSanPham(SanPham SanPham, HttpPostedFileBase fileupload)
        {

            //đưa dữ liệu vào dropdownload
            if (string.IsNullOrEmpty(SanPham.TenSP))
            {
                ViewBag.Error = "Không được bỏ trống ô Tên sản phẩm";
                return View("Error");
            }

            // Kiểm tra bỏ trống giá bán
            if (SanPham.GiaBan == null)
            {
                ViewBag.Error = "Không được bỏ trống ô Giá bán";
                return View("Error");
            }

            // Kiểm tra bỏ trống mô tả
            if (string.IsNullOrEmpty(SanPham.MoTa))
            {
                ViewBag.Error = "Không được bỏ trống ô Mô tả";
                return View("Error");
            }

            // Kiểm tra bỏ trống ảnh đại diện
            if (fileupload == null || fileupload.ContentLength == 0)
            {
                ViewBag.Error = "Vui lòng chọn ảnh đại diện";
                return View("Error");
            }
            // thêm vào csdl
            else
            {
                if (ModelState.IsValid)
                {
                    //lưu tên file , lưu ý bổ sung thư việng using System.IO
                    var filename = Path.GetFileName(fileupload.FileName);
                    // lưu đường dẫn của file
                    var path = Path.Combine(Server.MapPath("~/Assets/Images/Products/"), filename);
                    // kiểm tra hình tồn tại chưa?
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        // lưu hình ảnh vào  đường dẫn
                        fileupload.SaveAs(path);
                    }
                    SanPham.AnhDD = filename;
                    db.SanPhams.InsertOnSubmit(SanPham);
                    db.SubmitChanges();
                }
                return RedirectToAction("SanPham");
            }
        }
        public ActionResult ChiTietSanPham(int id)
        {
            SanPham SanPham = db.SanPhams.SingleOrDefault(n => n.MaSP == id);
            ViewBag.MaSP = SanPham.MaSP;
            if (SanPham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(SanPham);
        }
        public ActionResult XoaSanPham(int id)
        {
            SanPham SanPham = db.SanPhams.SingleOrDefault(n => n.MaSP == id);
            ViewBag.MaSP = SanPham.MaSP;
            if (SanPham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(SanPham);
        }
        [HttpPost, ActionName("XoaSanPham")]
        public ActionResult XacNhanXoa(int id)
        {
            SanPham SanPham = db.SanPhams.SingleOrDefault(n => n.MaSP == id);
            ViewBag.MaSP = SanPham.MaSP;
            if (SanPham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.SanPhams.DeleteOnSubmit(SanPham);
            db.SubmitChanges();
            return RedirectToAction("SanPham");
        }
        public ActionResult SuaSanPham(int id)
        {
            SanPham SanPham = db.SanPhams.SingleOrDefault(n => n.MaSP == id);
            if (SanPham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.MaThuongHieu = new SelectList(db.ThuongHieus.ToList().OrderBy(n => n.TenThuongHieu), "MaThuongHieu", "TenThuongHieu", SanPham.MaThuongHieu);
            return View(SanPham);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaSanPham(SanPham SanPham, HttpPostedFileBase fileupload, int MaThuongHieu)
        {
            // Lấy thông tin sản phẩm hiện tại từ CSDL
            SanPham existingSanPham = db.SanPhams.SingleOrDefault(n => n.MaSP == SanPham.MaSP);
            if (existingSanPham == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            // Kiểm tra bỏ trống tên sản phẩm
            if (string.IsNullOrEmpty(SanPham.TenSP))
            {
                ViewBag.Error = "Không được bỏ trống ô Tên sản phẩm";
                return View("Error");
            }

            // Kiểm tra bỏ trống giá bán
            if (SanPham.GiaBan == null)
            {
                ViewBag.Error = "Không được bỏ trống ô Giá bán";
                return View("Error");
            }

            // Kiểm tra bỏ trống mô tả
            if (string.IsNullOrEmpty(SanPham.MoTa))
            {
                ViewBag.Error = "Không được bỏ trống ô Mô tả";
                return View("Error");
            }

            // Cập nhật thông tin sản phẩm
            existingSanPham.TenSP = SanPham.TenSP;
            existingSanPham.GiaBan = SanPham.GiaBan;
            existingSanPham.MoTa = SanPham.MoTa;

            // Kiểm tra xem đã chọn ảnh đại diện mới hay chưa
            if (fileupload != null && fileupload.ContentLength > 0)
            {
                // Lưu tên file ảnh đại diện mới
                var filename = Path.GetFileName(fileupload.FileName);
                // Lưu đường dẫn của file
                var path = Path.Combine(Server.MapPath("~/Assets/Images/Products/"), filename);
                // Lưu hình ảnh vào đường dẫn
                fileupload.SaveAs(path);
                // Cập nhật ảnh đại diện mới
                existingSanPham.AnhDD = filename;
            }

            // Kiểm tra xem đã chọn thương hiệu mới hay chưa
            if (MaThuongHieu != 0)
            {
                existingSanPham.MaThuongHieu = MaThuongHieu;
            }

            db.SubmitChanges();

            return RedirectToAction("SanPham");
        }

        public ActionResult ThuongHieu(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(db.ThuongHieus.ToList().OrderBy(n => n.MaThuongHieu).ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult ThemThuongHieu()
        {
            return View();
        }
        public ActionResult SuaThuongHieu(int id)
        {
            ThuongHieu item = db.ThuongHieus.SingleOrDefault(n => n.MaThuongHieu == id);
            return View(item);
        }
        [HttpPost]
        public ActionResult SuaThuongHieu(ThuongHieu ThuongHieu)
        {
            if (string.IsNullOrEmpty(ThuongHieu.TenThuongHieu))
            {
                ViewBag.Error = "Không được bỏ trống ô này";
                return View("Error");
            }
            ThuongHieu itemm = db.ThuongHieus.SingleOrDefault(n => n.MaThuongHieu == ThuongHieu.MaThuongHieu);
            itemm.TenThuongHieu = ThuongHieu.TenThuongHieu;
            db.SubmitChanges();
            return RedirectToAction("ThuongHieu");
        }
        public ActionResult ChiTietThuongHieu(int id)
        {
            ThuongHieu item = db.ThuongHieus.SingleOrDefault(n => n.MaThuongHieu == id);
            ViewBag.MaThuongHieu = item.MaThuongHieu;
            if (item == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(item);
        }
        public ActionResult XoaThuongHieu(int id)
        {
            ThuongHieu item = db.ThuongHieus.SingleOrDefault(n => n.MaThuongHieu == id);

            if (item == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            if (db.SanPhams.Any(sp => sp.MaThuongHieu == id))
            {
                ViewBag.Error = "Không thể xóa thương hiệu này";
                return View("Error");
            }

            return View(item);
        }
        [HttpPost, ActionName("XoaThuongHieu")]
        public ActionResult XacNhanXoa1(int id)
        {
            ThuongHieu item = db.ThuongHieus.SingleOrDefault(n => n.MaThuongHieu == id);
            ViewBag.MaThuongHieu = item.MaThuongHieu;
            if (item == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.ThuongHieus.DeleteOnSubmit(item);
            db.SubmitChanges();
            return RedirectToAction("ThuongHieu");
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemThuongHieu(ThuongHieu item)
        {
            if (string.IsNullOrEmpty(item.TenThuongHieu))
            {
                ModelState.AddModelError("TenThuongHieu", "Không được bỏ trống ô này");
            }
            else if (db.ThuongHieus.Any(th => th.TenThuongHieu == item.TenThuongHieu))
            {
                ModelState.AddModelError("TenThuongHieu", "Đã có thương hiệu này");
            }

            if (ModelState.IsValid)
            {
                db.ThuongHieus.InsertOnSubmit(item);
                db.SubmitChanges();
                return RedirectToAction("ThuongHieu");
            }

            return View(item);
        }


        public ActionResult KhachHang(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(db.KhachHangs.ToList().OrderBy(n => n.MaKH).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult XoaKH(int id)
        {
            KhachHang item = db.KhachHangs.SingleOrDefault(n => n.MaKH == id);
            ViewBag.MaKH = item.MaKH;
            if (item == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(item);
        }
        [HttpPost, ActionName("XoaKH")]
        public ActionResult XacNhanXoa2(int id)
        {
            KhachHang khachHang = db.KhachHangs.SingleOrDefault(n => n.MaKH == id);
            ViewBag.MaKH = khachHang.MaKH;
            if (khachHang == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            // Xóa các đơn đặt hàng liên quan
            var donDatHangs = db.DonDatHangs.Where(n => n.MaKH == id);
            foreach (var donDatHang in donDatHangs)
            {
                var chiTietDatHangs = db.ChiTietDatHangs.Where(n => n.MaDonHang == donDatHang.MaDonHang);
                db.ChiTietDatHangs.DeleteAllOnSubmit(chiTietDatHangs);
            }
            db.DonDatHangs.DeleteAllOnSubmit(donDatHangs);

            // Xóa khách hàng
            db.KhachHangs.DeleteOnSubmit(khachHang);
            db.SubmitChanges();

            return RedirectToAction("KhachHang");
        }
        public ActionResult ChiTietKH(int id)
        {
            KhachHang item = db.KhachHangs.SingleOrDefault(n => n.MaKH == id);
            ViewBag.MaKH = item.MaKH;
            if (item == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(item);
        }
        public ActionResult SuaKH(int id)
        {
            KhachHang item = db.KhachHangs.SingleOrDefault(n => n.MaKH == id);
            return View(item);
        }
        [HttpPost]
        public ActionResult SuaKH(KhachHang kh)
        {
            KhachHang itemm = db.KhachHangs.SingleOrDefault(n => n.MaKH == kh.MaKH);
            itemm.HoTen = kh.HoTen;
            itemm.TaiKhoan = kh.TaiKhoan;
            itemm.MatKhau = kh.MatKhau;
            itemm.Email = kh.Email;
            itemm.DiaChiKH = kh.DiaChiKH;
            itemm.DienThoaiKH = kh.DienThoaiKH;
            db.SubmitChanges();
            return RedirectToAction("KhachHang");
        }
        public ActionResult DonDatHang(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(db.DonDatHangs.ToList().OrderBy(n => n.MaDonHang).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult SuaDDH(int id)
        {
            DonDatHang item = db.DonDatHangs.SingleOrDefault(n => n.MaDonHang == id);
            return View(item);
        }
        [HttpPost]
        public ActionResult SuaDDH(DonDatHang ddh)
        {
            DonDatHang itemm = db.DonDatHangs.SingleOrDefault(n => n.MaDonHang == ddh.MaDonHang);
            db.SubmitChanges();
            return RedirectToAction("DonDatHang");
        }
        public ActionResult ChiTietDH(int id)
        {
            var chiTietDatHangList = db.ChiTietDatHangs.Where(n => n.MaDonHang == id).ToList();
            ViewBag.MaDonHang = id;
            if (chiTietDatHangList.Count == 0)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(chiTietDatHangList);
        }
        public ActionResult XoaDDH(int id)
        {
            DonDatHang donDatHang = db.DonDatHangs.SingleOrDefault(n => n.MaDonHang == id);
            if (donDatHang == null)
            {
                // Đơn đặt hàng không tồn tại, không cần thông báo lỗi
                return RedirectToAction("DonDatHang");
            }

            // Xóa các chi tiết đặt hàng liên quan
            var chiTietDatHangs = db.ChiTietDatHangs.Where(n => n.MaDonHang == id);
            db.ChiTietDatHangs.DeleteAllOnSubmit(chiTietDatHangs);

            // Xóa đơn đặt hàng
            db.DonDatHangs.DeleteOnSubmit(donDatHang);
            db.SubmitChanges();

            return RedirectToAction("DonDatHang");
        }


        [HttpPost, ActionName("XoaDDH")]
        public ActionResult XacNhanXoa3(int id)
        {
            DonDatHang donDatHang = db.DonDatHangs.SingleOrDefault(n => n.MaDonHang == id);
            if (donDatHang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
         
            // Xóa ràng buộc khóa ngoại của bảng ChiTietDatHang trước
            var chiTietDatHangs = db.ChiTietDatHangs.Where(n => n.MaDonHang == id);
            db.ChiTietDatHangs.DeleteAllOnSubmit(chiTietDatHangs);
            db.SubmitChanges();

            // Xóa đơn đặt hàng
            db.DonDatHangs.DeleteOnSubmit(donDatHang);
            db.SubmitChanges();

            return RedirectToAction("DonDatHang");
        }

    }
}
