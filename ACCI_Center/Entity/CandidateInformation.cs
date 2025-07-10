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
        public string HoTen { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }
        public bool DaNhanChungChi { get; set; } = false;
        public bool DaGuiPhieuDuThi { get; set; } = false;
        public int? MaTTDangKy { get; set; } = 0;
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
