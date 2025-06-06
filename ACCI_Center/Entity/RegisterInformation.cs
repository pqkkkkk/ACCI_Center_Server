﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCI_Center.Entity
{
    public class RegisterInformation : INotifyPropertyChanged
    {
        public int MaTTDangKy { get; set; }
        public string HoTen { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }
        public string DiaChi { get; set; }
        public DateTime ThoiDiemDangKy { get; set; }
        public int MaLichThi { get; set; }
        public string TrangThai { get; set; }
        public string LoaiKhachHang { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
