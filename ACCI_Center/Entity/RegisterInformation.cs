using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCI_Center.Entity
{
    public class RegisterInformation : INotifyPropertyChanged
    {
        public int? MaTTDangKy { get; set; } = 0;
        public string HoTen { get; set; } = string.Empty;
        public string SDT { get; set; } = string.Empty;
        public string CCCD { get; set; } = string.Empty;
        public DateTime NgaySinh { get; set; } = DateTime.MinValue;
        public string Email { get; set; } = string.Empty;
        public string DiaChi { get; set; } = string.Empty;
        public DateTime ThoiDiemDangKy { get; set; } = DateTime.Now;
        public int? MaLichThi { get; set; } = 0;
        public string TrangThaiThanhToan { get; set; } = "Chưa thanh toán"; // Trạng thái thanh toán, có thể là "Đã thanh toán" hoặc "Chưa thanh toán"
        public string TrangThaiDangKy { get; set; } = "Chưa duyệt"; // Trạng thái đăng ký, có thể là "Đã duyệt" hoặc "Chưa duyệt" hoặc "Không đủ điều kiện"
        public string LoaiKhachHang { get; set; } = string.Empty; // Loại khách hàng, có thể là "Cá nhân" hoặc "Đơn vị"
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
