using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCI_Center.Entity
{
    public class ExtensionInformation : INotifyPropertyChanged
    {
        public int MaTTGiaHan { get; set; } = 0;
        public DateTime? ThoiDiemGiaHan { get; set; } = null;
        public string? LoaiGiaHan { get; set; } = "";
        public string? LyDo { get; set; }
        public string? TrangThai { get; set; }
        public int MaLichThiMoi { get; set; } = 0;
        public int MaTTDangKy { get; set; } = 0;
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
