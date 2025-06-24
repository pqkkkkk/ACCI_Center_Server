using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ACCI_Center.Configuraion;
using ACCI_Center.Entity;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ACCI_Center.Dao.RegisterInformation
{
    public class RegisterInformationSqlDao : IRegisterInformationDao
    {
        private readonly IDataClient dataClient;
        private readonly DbConnection dbConnection;

        public RegisterInformationSqlDao(IDataClient dataClient)
        {
            this.dataClient = dataClient;
            dbConnection = dataClient.GetDbConnection();
        }
        private DbParameter[] buildParametersForRegisterInformation(Entity.RegisterInformation registerInformation)
        {
            var parameters = new List<DbParameter>();

            var hotenParam = dbConnection.CreateCommand().CreateParameter();
            hotenParam.ParameterName = "@HoTen";
            hotenParam.Value = registerInformation.HoTen;
            parameters.Add(hotenParam);

            var sdtParam = dbConnection.CreateCommand().CreateParameter();
            sdtParam.ParameterName = "@SDT";
            sdtParam.Value = registerInformation.SDT;
            parameters.Add(sdtParam);

            var emailParam = dbConnection.CreateCommand().CreateParameter();
            emailParam.ParameterName = "@Email";
            emailParam.Value = registerInformation.Email;
            parameters.Add(emailParam);

            var diaChiParam = dbConnection.CreateCommand().CreateParameter();
            diaChiParam.ParameterName = "@DiaChi";
            diaChiParam.Value = registerInformation.DiaChi;
            parameters.Add(diaChiParam);

            var thoiDiemDangKyParam = dbConnection.CreateCommand().CreateParameter();
            thoiDiemDangKyParam.ParameterName = "@ThoiDiemDangKy";
            thoiDiemDangKyParam.Value = registerInformation.ThoiDiemDangKy;
            parameters.Add(thoiDiemDangKyParam);

            var maLichThiParam = dbConnection.CreateCommand().CreateParameter();
            maLichThiParam.ParameterName = "@MaLichThi";
            maLichThiParam.Value = registerInformation.MaLichThi;
            parameters.Add(maLichThiParam);

            var trangThaiParam = dbConnection.CreateCommand().CreateParameter();
            trangThaiParam.ParameterName = "@TrangThai";
            trangThaiParam.Value = registerInformation.TrangThai;
            parameters.Add(trangThaiParam);

            var loaiKhachHangParam = dbConnection.CreateCommand().CreateParameter();
            loaiKhachHangParam.ParameterName = "@LoaiKhachHang";
            loaiKhachHangParam.Value = registerInformation.LoaiKhachHang;
            parameters.Add(loaiKhachHangParam);

            return parameters.ToArray();
        }
        public int AddRegisterInformation(Entity.RegisterInformation registerInformation)
        {
            try
            {
                string sql = """
                INSERT INTO TTDANGKY (
                    HoTen, SDT, Email, DiaChi, ThoiDiemDangKy, MaLichThi, TrangThai, LoaiKhachHang
                ) VALUES (
                    @HoTen, @SDT, @Email, @DiaChi, @ThoiDiemDangKy, @MaLichThi, @TrangThai, @LoaiKhachHang
                );
                SELECT CAST(SCOPE_IDENTITY() AS int);
                """;
                DbParameter[] parameters = buildParametersForRegisterInformation(registerInformation);

                if (dbConnection.State != System.Data.ConnectionState.Open)
                {
                    dbConnection.Open();
                }
                using (var command = dbConnection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.AddRange(parameters);
                    var result = command.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : -1;
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Error while adding register information", ex);
            }
            finally
            {
                if (dbConnection.State == System.Data.ConnectionState.Open)
                {
                    dbConnection.Close();
                }
            }
        }

        public int AddCandidateInformationsOfARegisterInformation(int maTTDangKy, List<CandidateInformation> candidateInformations)
        {
            try
            {
                string sql = """
                INSERT INTO TTHISINH (
                    MaTTDangKy, HoTen, SDT, Email, DaNhanChungChi, DaGuiPhieuDuThi
                ) VALUES (
                    @MaTTDangKy, @HoTen, @SDT, @Email, @DaNhanChungChi, @DaGuiPhieuDuThi
                );
                """;

                int rowsAffected = 0;
                if(dbConnection.State != System.Data.ConnectionState.Open)
                {
                    dbConnection.Open();
                }
                foreach (var candidate in candidateInformations)
                {
                    using (var command = dbConnection.CreateCommand())
                    {
                        command.CommandText = sql;

                        var maTTDangKyParam = command.CreateParameter();
                        maTTDangKyParam.ParameterName = "@MaTTDangKy";
                        maTTDangKyParam.Value = maTTDangKy;
                        command.Parameters.Add(maTTDangKyParam);

                        var hoTenParam = command.CreateParameter();
                        hoTenParam.ParameterName = "@HoTen";
                        hoTenParam.Value = candidate.HoTen;
                        command.Parameters.Add(hoTenParam);

                        var sdtParam = command.CreateParameter();
                        sdtParam.ParameterName = "@SDT";
                        sdtParam.Value = candidate.SDT;
                        command.Parameters.Add(sdtParam);

                        var emailParam = command.CreateParameter();
                        emailParam.ParameterName = "@Email";
                        emailParam.Value = candidate.Email;
                        command.Parameters.Add(emailParam);

                        var daNhanChungChiParam = command.CreateParameter();
                        daNhanChungChiParam.ParameterName = "@DaNhanChungChi";
                        daNhanChungChiParam.Value = candidate.DaNhanChungChi;
                        command.Parameters.Add(daNhanChungChiParam);

                        var daGuiPhieuDuThiParam = command.CreateParameter();
                        daGuiPhieuDuThiParam.ParameterName = "@DaGuiPhieuDuThi";
                        daGuiPhieuDuThiParam.Value = candidate.DaGuiPhieuDuThi;
                        command.Parameters.Add(daGuiPhieuDuThiParam);

                        rowsAffected += command.ExecuteNonQuery();
                    }
                }

                return rowsAffected;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while adding candidate information", ex);
            }
            finally
            {
                if (dbConnection.State == System.Data.ConnectionState.Open)
                {
                    dbConnection.Close();
                }
            }
        }
        public void UpdateCandidateStatus(int mathisinh, bool DaGuiPhieuDuThi)
       {
           string sql = """
               UPDATE TTHISINH
               SET DaGuiPhieuDuThi = @DaGuiPhieuDuThi
               WHERE MaTTThiSinh = @mathisinh
           """;


           using (var command = dbConnection.CreateCommand())
           {
               if (dbConnection.State != System.Data.ConnectionState.Open)
               {
                   dbConnection.Open();
               }
               command.CommandText = sql;


               var maThiSinhParam = command.CreateParameter();
               maThiSinhParam.ParameterName = "@mathisinh";
               maThiSinhParam.Value = mathisinh;
               command.Parameters.Add(maThiSinhParam);


               var daGuiPhieuDuThiParam = command.CreateParameter();
               daGuiPhieuDuThiParam.ParameterName = "@DaGuiPhieuDuThi";
               daGuiPhieuDuThiParam.Value = DaGuiPhieuDuThi;
               command.Parameters.Add(daGuiPhieuDuThiParam);


               command.ExecuteNonQuery();
           }


       }

    }
}
