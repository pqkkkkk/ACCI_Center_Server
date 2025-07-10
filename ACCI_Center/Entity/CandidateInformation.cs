using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCI_Center.Entity
{
    public class CandidateInformation : INotifyPropertyChanged
    {
        public int? MaTTThiSinh { get; set; } = 0;
        public string HoTen { get; set; } = string.Empty;
        public string SDT { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime NgaySinh { get; set; } = DateTime.MinValue;
        public string CCCD { get; set; } = string.Empty;
        public bool DaNhanChungChi { get; set; } = false;
        public bool DaGuiPhieuDuThi { get; set; } = false;
        public int DiemThi { get; set; } = 0;
        public string TenLoaiChungChi { get; set; } = string.Empty;
        public int? MaTTDangKy { get; set; } = 0;
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
