using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCI_Center.Dto.Response
{
    public class ExtensionInformationDto
    {
        public int MaTTGiaHan { get; set; }
        public string? TenKhachHang { get; set; }
        public int MaKyThi { get; set; }
        public DateTime NgayThiMoi { get; set; }
        public DateTime ThoiDiemGiaHan { get; set; }
        public string? LoaiGiaHan { get; set; }
        public string? LyDo { get; set; }
        public string? TrangThai { get; set; }
        public double PhiGiaHan { get; set; }
    }
}
