using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCI_Center.Entity
{
    public class ExamSchedule
    {
        public int MaLichThi { get; set; }
        public DateTime NgayThi { get; set; }
        public DateTime ThoiDiemKetThuc { get; set; }
        public int BaiThi { get; set; }
        public int SoLuongThiSinhHienTai { get; set; }
        public bool DaNhapKetQuaThi { get; set; }
        public bool DaPhatHanhPhieuDuThi { get; set; }
        public bool DaThongBaoKetQuaThi { get; set; }
        public string TrangThaiDuyet { get; set; } = "Chưa duyệt";
        public string LoaiLichThi { get; set; } = "Tự do"; // Loại lịch thi, có thể là "Lịch thi tự do" hoặc "Lịch thi cho đơn vị"
        public int PhongThi { get; set; }
    }
}
