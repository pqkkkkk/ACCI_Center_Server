using System.Data.Common;

namespace ACCI_Center.Dao.ExamSchedule
{
    public static class ExamScheduleDaoUtil
    {
        public static Func<DbDataReader, Entity.Room> roomMapper = (reader) =>
        {
            if (reader == null) return null;
            return new Entity.Room
            {
                MaPhongThi = reader.GetInt32(reader.GetOrdinal("MaPhongThi")),
                SucChua = reader.IsDBNull(reader.GetOrdinal("SucChua")) ? 0 : reader.GetInt32(reader.GetOrdinal("SucChua")),
            };
        };
        public static Func<DbDataReader, Entity.Employee> employeeMapper = (reader) =>
        {
            if (reader == null) return null;
            return new Entity.Employee
            {
                MaNhanVien = reader.GetInt32(reader.GetOrdinal("MaNhanVien")),
                HoTen = reader.GetString(reader.GetOrdinal("HoTen")),
                SDT = reader.GetString(reader.GetOrdinal("SDT")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                NgaySinh = reader.GetDateTime(reader.GetOrdinal("NgaySinh")),
            };
        };

        public static DbParameter[] BuildParametersForAddExamSchedule(Entity.ExamSchedule examSchedule, DbConnection dbConnection)
        {
            var parameters = new List<DbParameter>();
            var baiThiParam = dbConnection.CreateCommand().CreateParameter();

            baiThiParam.ParameterName = "@BaiThi";
            baiThiParam.Value = examSchedule.BaiThi;
            parameters.Add(baiThiParam);

            var ngayThiParam = dbConnection.CreateCommand().CreateParameter();
            ngayThiParam.ParameterName = "@NgayThi";
            ngayThiParam.Value = examSchedule.NgayThi;
            parameters.Add(ngayThiParam);

            var soLuongThiSinhHienTaiParam = dbConnection.CreateCommand().CreateParameter();
            soLuongThiSinhHienTaiParam.ParameterName = "@SoLuongThiSinhHienTai";
            soLuongThiSinhHienTaiParam.Value = examSchedule.SoLuongThiSinhHienTai;
            parameters.Add(soLuongThiSinhHienTaiParam);

            var daNhapKetQuaThiParam = dbConnection.CreateCommand().CreateParameter();
            daNhapKetQuaThiParam.ParameterName = "@DaNhapKetQuaThi";
            daNhapKetQuaThiParam.Value = examSchedule.DaNhapKetQuaThi;
            parameters.Add(daNhapKetQuaThiParam);

            var daThongBaoKetQuaThiParam = dbConnection.CreateCommand().CreateParameter();
            daThongBaoKetQuaThiParam.ParameterName = "@DaThongBaoKetQuaThi";
            daThongBaoKetQuaThiParam.Value = examSchedule.DaThongBaoKetQuaThi;
            parameters.Add(daThongBaoKetQuaThiParam);

            var daPhatHanhPhieuDuThiParam = dbConnection.CreateCommand().CreateParameter();
            daPhatHanhPhieuDuThiParam.ParameterName = "@DaPhatHanhPhieuDuThi";
            daPhatHanhPhieuDuThiParam.Value = examSchedule.DaPhatHanhPhieuDuThi;
            parameters.Add(daPhatHanhPhieuDuThiParam);

            var trangThaiDuyetParam = dbConnection.CreateCommand().CreateParameter();
            trangThaiDuyetParam.ParameterName = "@TrangThaiDuyet";
            trangThaiDuyetParam.Value = examSchedule.TrangThaiDuyet;
            parameters.Add(trangThaiDuyetParam);

            var loaiLichThiParam = dbConnection.CreateCommand().CreateParameter();
            loaiLichThiParam.ParameterName = "@LoaiLichThi";
            loaiLichThiParam.Value = examSchedule.LoaiLichThi;
            parameters.Add(loaiLichThiParam);

            var phongThiParam = dbConnection.CreateCommand().CreateParameter();
            phongThiParam.ParameterName = "@PhongThi";
            phongThiParam.Value = examSchedule.PhongThi == 0 ? DBNull.Value : examSchedule.PhongThi;
            parameters.Add(phongThiParam);

            return parameters.ToArray();
        }
    }
}
