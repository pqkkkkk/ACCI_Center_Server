using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCI_Center.Dto.Request
{
    public class PaidRenewalRequest
    {
        public string? LyDo { get; set; }
        public double PhiGiaHan { get; set; }
        public int MaTTDangKy { get; set; }
        public int MaLichThiMoi { get; set; }
    }
}
