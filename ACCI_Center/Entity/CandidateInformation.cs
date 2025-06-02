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
        public int MaTTThiSinh { get; set; }
        public string HoTen { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }
        public bool DaNhanChungChi { get; set; }
        public bool DaGuiPhieuDuThi { get; set; }
        public int MaTTDangKy { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
