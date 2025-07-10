using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCI_Center.Entity
{
    public class Invoice : INotifyPropertyChanged
    {
        public int MaHoaDon { get; set; } = 0;
        public DateTime ThoiDiemTao { get; set; }
        public DateTime? ThoiDiemThanhToan { get; set; }
        public double TongTien { get; set; }
        public double KhuyenMai { get; set; } = 0;
        public double ThanhTien { get; set; } = 0;
        public string TrangThai { get; set; } = "Chưa thanh toán";
        public string LoaiHoaDon { get; set; } = string.Empty;
        public int MaTTDangKy { get; set; } = 0;
        public int MaTTGiaHan { get; set; } = 0;
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
