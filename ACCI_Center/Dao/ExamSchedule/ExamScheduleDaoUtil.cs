using System.Data.Common;

namespace ACCI_Center.Dao.ExamSchedule
{
    public static class ExamScheduleDaoUtil
    {
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

            var phongThiParam = dbConnection.CreateCommand().CreateParameter();
            phongThiParam.ParameterName = "@PhongThi";
            phongThiParam.Value = examSchedule.PhongThi;
            parameters.Add(phongThiParam);

            return parameters.ToArray();
        }
    }
}
