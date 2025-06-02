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
        public int MaTTGiaHan { get; set; }
        public DateTime ThoiDiemGiaHan { get; set; }
        public string LoaiGiaHan { get; set; }
        public double PhiGiaHan { get; set; }
        public int MaTTDangKy { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
