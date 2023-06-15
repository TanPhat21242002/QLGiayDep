Create Database DbYikes
Go
Use DbYikes
Go
--Drop table KhachHang
--Go
Create Table	KhachHang
(
	MaKH INT IDENTITY(1,1),
	HoTen nVarchar(50) NOT NULL,
	TaiKhoan Varchar(50) UNIQUE,
	MatKhau Varchar(50) NOT NULL,
	Email Varchar(100) UNIQUE,
	DiaChiKH nVarchar(200),
	DienThoaiKH Varchar(50),
	CONSTRAINT PK_KhachHang PRIMARY KEY(MaKH)
)
Go
--Drop table ThuongHieu
--Go
Create Table	ThuongHieu
(
	MaThuongHieu	Int Identity(1,1),
	TenThuongHieu	Nvarchar(50)	Not Null,
	Constraint	Pk_ThuongHieu Primary Key(MaThuongHieu),
)
Go
--Drop table SanPham
--Go
Create Table	SanPham
(
	MaSP	Int	Identity(1,1),
	TenSP	Nvarchar(100)	Not Null,
	GiaBan	Decimal,
	MoTa	Nvarchar(Max),
	AnhDD	Varchar(50),
	MaThuongHieu	Int,
	Constraint	Pk_SanPham	Primary Key(MaSP),
	Constraint	Fk_ThuongHieu	Foreign Key(MaThuongHieu) References	ThuongHieu(MaThuongHieu)
)
Go
--Drop table DonDatHang
--Go
CREATE TABLE DonDatHang
(
	MaDonHang INT IDENTITY(1,1),
	NgayDat Datetime,
	NgayGiaoDuKien Datetime,
	MaKH INT,
	CONSTRAINT FK_KhachHang FOREIGN KEY (MaKH) REFERENCES KhachHang(MaKH),
	CONSTRAINT PK_DonDatHang PRIMARY KEY (MaDonHang)
)
Go
--Drop table ChiTietDatHang
--Go
CREATE TABLE ChiTietDatHang
(
	MaDonHang INT,
	MaSP INT,
	SoLuong Int Check(SoLuong>0),
	DonGia Decimal(18,0) Check(DonGia>=0),
	CONSTRAINT PK_CTDatHang PRIMARY KEY(MaDonHang,MaSP),
	CONSTRAINT FK_DonHang FOREIGN KEY (MaDonHang) REFERENCES DonDatHang(MaDonHang),
	CONSTRAINT FK_SanPham FOREIGN KEY (MaSP) REFERENCES SanPham(MaSP)
)
Go
--Thêm dữ liệu:
---Loại
	Insert into ThuongHieu values (N'Nike')
	Insert into ThuongHieu values (N'Adidas')
	Insert into ThuongHieu values (N'Puma')
	Insert into ThuongHieu values (N'Converse')
	Insert into ThuongHieu values (N'Vans')
select *from ThuongHieu
---Sản phẩm
	---Nike
	Insert into SanPham values (N'Nike Air Force 1',2900000,N'Phiên bản mới nhất của giày bóng rổ kinh điển Nike Air Force 1 vẫn giữ được sự rực rỡ đặc trưng của nó. Sản phẩm này mang lại một cái nhìn mới lạ với các lớp phủ được may chắc, hoàn thiện sạch sẽ và có độ lấp lánh đủ để bạn tỏa sáng.','nike_1.png',1)
	Insert into SanPham values (N'Air Jordan 1 Hi FlyEase',3400000,N'Air Jordan 1 Hi FlyEase kết hợp phong cách đáng mơ ước của đôi giày ký hiệu đầu tiên của Michael Jordan với hệ thống đóng mở dễ dàng bằng một tay. Không cần phải buộc dây giày, chỉ cần có một khóa kéo quanh và hai dải đeo để đảm bảo việc tùy chỉnh kích cỡ vừa vặn.','nike_2.png',1)
	---Adidas
	Insert into SanPham values (N'NMD_R1 SHOES',3900000,N'Với mỗi chuyển động vặn người, rẽ hướng, xoay trục và tạm ngừng, đôi giày adidas này đều hiểu rằng mình chỉ có một nhiệm vụ, đó là đàn hồi. Kiểu dáng ôm vừa khít như một đôi tất nhờ thân giày bằng vải dệt kim co giãn làm từ sợi chuyên dụng thích ứng theo bàn chân bạn. Lớp đệm BOOST hoàn trả năng lượng trên từng sải bước, cùng lớp lót mềm mại để bạn có thể mang tất hay không tùy ý. Viền gót giày biểu tượng mang phong cách NMD đặc trưng tạo nên thiết kế tân tiến cùng bạn chinh phục ngày mới.','adidas_1.jpg',2)
	Insert into SanPham values (N'FORUM LOW SHOES',2500000,N'Không chỉ là một đôi giày, mà chính là một tuyên ngôn. Dòng adidas Forum ra mắt năm 1984 và cực kỳ được ưa chuộng cả trên sân bóng rổ lẫn trong giới âm nhạc. Mẫu mới của dòng giày kinh điển này tái hiện tinh thần thập niên 80, nguồn năng lượng bùng nổ trên sân đấu cũng như thiết kế quai cổ chân chữ X đặc trưng, kết tinh thành phiên bản cổ thấp đậm chất đường phố.','adidas_2.jpg',2)
	---Puma
	Insert into SanPham values (N'Puma Slipstream Lo',2500000,N'Giày sneakers unisex cổ thấp Slipstream Lo Men','puma_1.jpg',3)
	Insert into SanPham values (N'Puma Rebound Joy Low',1800000,N'Giày sneakers unisex cổ thấp Rebound Joy Low','puma_2.jpg',3)
	---Converse
	Insert into SanPham values (N'Chuck Taylor All Star Classic Black',1400000,N'Converse Chuck Taylor Classic với thiết kế kinh điển phù hợp với mọi độ tuổi, giới tính được ưa chuộng bởi sự đơn giản, thời trang, đặc biệt không kén người mang. Sở hữu phối màu basic đặc trưng, thêm vào đó là chất liệu canvas thoáng mát cùng bộ đế cao su bền chắc tạo độ bám. Item “huyền thoại” này với sự đột phá về phong cách thời trang hứa hẹn sẽ mang lại cho bạn những outfit cực chất.','converse_1.jpg',4)
	Insert into SanPham values (N'Chuck Taylor All Star Classic White',1400000,N'Converse Chuck Taylor Classic với thiết kế kinh điển phù hợp với mọi độ tuổi, giới tính được ưa chuộng bởi sự đơn giản, thời trang, đặc biệt không kén người mang. Sở hữu phối màu basic đặc trưng, thêm vào đó là chất liệu canvas thoáng mát cùng bộ đế cao su bền chắc tạo độ bám. Item “huyền thoại” này với sự đột phá về phong cách thời trang hứa hẹn sẽ mang lại cho bạn những outfit cực chất.','converse_2.jpg',4)
	---Vans
	Insert into SanPham values (N'Vans Old Skool 36 DX Anaheim Factory',1900000,N'Vans Old Skool Anaheim Factory đã giữ lại kiểu dáng Old Skool cổ điển với đệm giày được nâng cấp công nghệ OrthoLite® hiện đại. Điểm khác biệt nằm ở sắc trắng ngà của đế giày và đường viền foxing cao hơn, to và bóng hơn mang đến cảm giác Retro cho người mang.','vans_1.jpg',5)
	Insert into SanPham values (N'Vans UA SK8 Hi Vintage Pop ',1800000,N'Vans Vintage Pop SK8-Hi đánh gục cộng đồng sneakerhead bởi tông màu trắng ngà chủ đạo điểm xuyết thêm sắc xanh rêu vô cùng sang trọng và thời thượng. Thiết kế cổ cao SK8-Hi kết hợp cùng sọc Sidestripe và logo “Vans Off The Wall” đã trở thành biểu tượng văn hóa đặc trưng, độc đáo của nhà giày trượt ván.','vans_2.jpg',5)
select *from SanPham
--Dữ liệu cập nhật: Tài khoản quản trị
Create table Admin
(
	UserAdmin varchar(30) primary key,
	PassAdmin varchar(30) not null,
	Hoten nVarchar(50)
)
Insert into Admin values('admin','123456','Nguyen Tan Phat')
Insert into Admin values('user','123456','Mr Phát')
select *from Admin

ALTER TABLE DonDatHang
ADD TTGiaoHang BIT not null,
    TTThanhToan BIT not null;