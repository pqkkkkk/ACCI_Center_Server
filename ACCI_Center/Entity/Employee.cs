namespace ACCI_Center.Entity
{
    public class Employee
    {
        public int MaNhanVien { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public DateTime NgaySinh { get; set; } = DateTime.MinValue;
        public string SDT { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
