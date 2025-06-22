namespace ACCI_Center.Dto.Reponse
{
    public class AvailableExamScheduleReponse
    {
        public int MaLichThi { get; set; }
        public string TenBaiThi { get; set; }
        public string LoaiBaiThi { get; set; }
        public DateTime NgayThi { get; set; }
        public int PhongThi { get; set; }
        public int SoLuongThiSinhHienTai { get; set; }
        public int SoLuongThiSinhToiDa { get; set; }
        public double GiaDangKy { get; set; }
    }
}
