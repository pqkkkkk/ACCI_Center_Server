using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCI_Center.Entity
{
    public class ExamSchedule : INotifyPropertyChanged
    {
        public int MaLichThi { get; set; }
        public DateTime NgayThi { get; set; }
        public string BaiThi { get; set; }
        public int SoLuongThiSinhToiDa { get; set; }
        public int SoLuongThiSinhToiThieu { get; set; }
        public int SoLuongThiSinhHienTai { get; set; }
        public int PhongThi { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
