# Sole Mate — Website bán giày demo

Đồ án xây dựng website bán giày đơn giản bằng ASP.NET Core Razor Pages. Website có giao diện tiếng Việt, phân quyền giữa khách hàng và quản trị viên, cùng cơ sở dữ liệu SQLite.

## Công nghệ sử dụng

- ASP.NET Core Razor Pages trên .NET 10
- Entity Framework Core 10 với SQLite
- Cookie Authentication và role-based authorization
- Giỏ hàng lưu trong session

## Chức năng

### Khách hàng

- Xem trang chủ và danh sách sản phẩm.
- Xem thông tin chi tiết sản phẩm.
- Thêm sản phẩm vào giỏ hàng và cập nhật số lượng.
- Đặt hàng; hệ thống lưu đơn hàng và cập nhật tồn kho.

### Quản trị viên

- Xem tổng quan số đơn hàng, sản phẩm, khách hàng và doanh thu.
- Quản lý sản phẩm: thêm, sửa, xóa.
- Xem đơn hàng và cập nhật trạng thái.
- Xem danh sách khách hàng và lịch sử mua hàng tổng hợp.
- Tạo tài khoản khách hàng hoặc quản trị viên.

## Yêu cầu

- Cài đặt [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).
- Kết nối Internet để tải ảnh sản phẩm từ Unsplash và font chữ từ Google Fonts.

## Chạy ứng dụng

Mở terminal tại thư mục chứa `Razor-WebApplication.csproj`, sau đó chạy:

```powershell
dotnet restore
dotnet run
```

Mở địa chỉ HTTP/HTTPS được hiển thị trong terminal.

Khi ứng dụng khởi động lần đầu, Entity Framework Core tạo file `shoestore.db` và thêm tài khoản cùng sản phẩm mẫu. Cơ sở dữ liệu mặc định được cấu hình trong `appsettings.json`. Các bảng và quan hệ được định nghĩa bằng code trong `Data/StoreDbContext.cs`.

> Đồ án sử dụng `EnsureCreated` để tạo database đơn giản cho mục đích demo. Khi khởi động, ứng dụng bổ sung cột `OrderItems.Size` nếu database SQLite hiện có chưa có cột này; các dòng đơn cũ được gán size mặc định EU 40.

## Tài khoản demo

| Vai trò | Email | Mật khẩu |
|---|---|---|
| Quản trị viên | `admin@shoestore.vn` | `Admin@123` |
| Khách hàng | `user@shoestore.vn` | `User@123` |

Đây là thông tin đăng nhập mẫu dành cho chạy thử; không sử dụng các mật khẩu này trong môi trường thực tế.

## Cấu trúc dữ liệu chính

- `Users`: tài khoản, email, mật khẩu đã hash và vai trò.
- `Products`: thông tin sản phẩm, giá và tồn kho.
- `Orders`: đơn hàng, khách hàng, trạng thái và tổng tiền.
- `OrderItems`: sản phẩm, size EU, số lượng và đơn giá tại thời điểm đặt hàng.

## Cấu trúc thư mục

```text
Data/       DbContext, khởi tạo dữ liệu mẫu và tiện ích giỏ hàng
Models/     Các entity tài khoản, sản phẩm và đơn hàng
Pages/      Razor Pages dành cho khách hàng và quản trị viên
wwwroot/    CSS và tài nguyên tĩnh
```
