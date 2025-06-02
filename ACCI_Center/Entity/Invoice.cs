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
        public int MaHoaDon { get; set; }
        public DateTime ThoiDiemTao { get; set; }
        public DateTime ThoiDiemThanhToan { get; set; }
        public double TongTien { get; set; }
        public string TrangThai { get; set; }
        public string LoaiHoaDon { get; set; }
        public int MaTTDangKy { get; set; }
        public int MaTTGiaHan { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
