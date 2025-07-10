namespace ACCI_Center.FilterField
{
    public class ExamScheduleFilterObject
    {
        public int? MaLichThi { get; set; } = 0;
        public int? BaiThi { get; set; } = 0;
        public DateTime? NgayThiBatDau { get; set; } = null;
        public DateTime? NgayThiKetThuc { get; set; } = null;
        public bool? DaNhapKetQuaThi { get; set; } = null;
        public bool? DaPhatHanhPhieuDangKyThi { get; set; } = null;
        public bool? DaThongBaoKetQuaThi { get; set; } = null;
        public int? PhongThi { get; set; } = null;
    }
}
