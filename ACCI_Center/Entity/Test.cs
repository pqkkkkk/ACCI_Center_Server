using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ACCI_Center.Entity
{
    public class Test
    {
        public int MaBaiThi { get; set; }
        public string TenBaiThi { get; set; }
        public string LoaiBaiThi { get; set; }
        public double GiaDangKy { get; set; }
        public int SoLuongThiSinhToiDa { get; set; }
        public int SoLuongThiSinhToiThieu { get; set; }
        public int ThoiGianThi { get; set; } // Thời gian thi tính bằng phú
    }
}
