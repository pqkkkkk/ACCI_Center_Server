using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ACCI_Center.Configuraion;
using ACCI_Center.Dto;
using ACCI_Center.Entity;
using ACCI_Center.FilterField;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ACCI_Center.Dao.RegisterInformation
{
    public class RegisterInformationSqlDao : IRegisterInformationDao
    {
        private readonly IDataClient dataClient;

        public RegisterInformationSqlDao(IDataClient dataClient)
        {
            this.dataClient = dataClient;
        }
        Func<DbDataReader, Entity.RegisterInformation> registerInformationMapFunc = reader =>
        {
            return new Entity.RegisterInformation
            {
                MaTTDangKy = reader.GetInt32(reader.GetOrdinal("MaTTDangKy")),
                HoTen = reader.GetString(reader.GetOrdinal("HoTen")),
                SDT = reader.GetString(reader.GetOrdinal("SDT")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                DiaChi = reader.GetString(reader.GetOrdinal("DiaChi")),
                ThoiDiemDangKy = reader.GetDateTime(reader.GetOrdinal("ThoiDiemDangKy")),
                MaLichThi = reader.GetInt32(reader.GetOrdinal("MaLichThi")),
                TrangThaiThanhToan = reader.GetString(reader.GetOrdinal("TrangThai")),
                LoaiKhachHang = reader.GetString(reader.GetOrdinal("LoaiKhachHang"))
            };
        };
        Func<DbDataReader, Entity.CandidateInformation> candidateInformationMapFunc = reader =>
        {
            return new Entity.CandidateInformation
            {
                MaTTThiSinh = reader.GetInt32(reader.GetOrdinal("MaTTThiSinh")),
                MaTTDangKy = reader.GetInt32(reader.GetOrdinal("MaTTDangKy")),
                HoTen = reader.GetString(reader.GetOrdinal("HoTen")),
                SDT = reader.GetString(reader.GetOrdinal("SDT")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                DaGuiPhieuDuThi = reader.GetBoolean(reader.GetOrdinal("DaGuiPhieuDuThi")),
                DaNhanChungChi = reader.GetBoolean(reader.GetOrdinal("DaNhanChungChi"))
            };
        };
        private DbParameter[] buildParametersForRegisterInformation(Entity.RegisterInformation registerInformation, DbConnection dbConnection)
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
            trangThaiParam.Value = registerInformation.TrangThaiThanhToan;
            parameters.Add(trangThaiParam);

            var loaiKhachHangParam = dbConnection.CreateCommand().CreateParameter();
            loaiKhachHangParam.ParameterName = "@LoaiKhachHang";
            loaiKhachHangParam.Value = registerInformation.LoaiKhachHang;
            parameters.Add(loaiKhachHangParam);

            return parameters.ToArray();
        }
        public int AddRegisterInformation(Entity.RegisterInformation registerInformation)
        {
            using (var dbConnection = dataClient.GetDbConnection())
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
                    DbParameter[] parameters = buildParametersForRegisterInformation(registerInformation, dbConnection);

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
                catch (Exception ex)
                {
                    throw new Exception("Error while adding register information", ex);
                }
            }
        }

        public int AddCandidateInformationsOfARegisterInformation(int maTTDangKy, List<CandidateInformation> candidateInformations)
        {
            using (var dbConnection = dataClient.GetDbConnection())
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
                    if (dbConnection.State != System.Data.ConnectionState.Open)
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
            }
        }
        public int UpdateExamSchedule(int maTTDangKy, int maLichThi)
        {
            using (var dbConnection = dataClient.GetDbConnection())
            {
                try
                {
                    string sql = """
                UPDATE ACCI_Center.dbo.TTDANGKY
                SET MaLichThi = @MaLichThi
                WHERE MaTTDangKy = @MaTTDangKy;
                """;

                    int rowsAffected = 0;

                    using (var command = dbConnection.CreateCommand())
                    {
                        if (dbConnection.State != System.Data.ConnectionState.Open)
                        {
                            dbConnection.Open();
                        }
                        command.CommandText = sql;

                        var maTTDangKyParam = command.CreateParameter();
                        maTTDangKyParam.ParameterName = "@MaTTDangKy";
                        maTTDangKyParam.Value = maTTDangKy;
                        command.Parameters.Add(maTTDangKyParam);

                        var maLichThiParam = command.CreateParameter();
                        maLichThiParam.ParameterName = "@MaLichThi";
                        maLichThiParam.Value = maLichThi;
                        command.Parameters.Add(maLichThiParam);

                        rowsAffected = command.ExecuteNonQuery();
                    }
                    return rowsAffected;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error while updating exam schedule", ex);
                }
            }
        }
        public void UpdateCandidateStatus(int mathisinh, bool DaGuiPhieuDuThi)
       {
            using (var dbConnection = dataClient.GetDbConnection())
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
        private string BuildWhereClauseForRegisterInformationQuery(RegisterInformationFilterObject filterObject)
        {
            List<string> whereClauses = new List<string>();
            if (filterObject.MaTTDangKy.HasValue)
            {
                whereClauses.Add("MaTTDangKy = @MaTTDangKy");
            }
            if (!string.IsNullOrEmpty(filterObject.HoTen))
            {
                whereClauses.Add("HoTen LIKE @HoTen");
            }
            if (!string.IsNullOrEmpty(filterObject.SDT))
            {
                whereClauses.Add("SDT LIKE @SDT");
            }
            if (!string.IsNullOrEmpty(filterObject.Email))
            {
                whereClauses.Add("Email LIKE @Email");
            }
            if (!string.IsNullOrEmpty(filterObject.DiaChi))
            {
                whereClauses.Add("DiaChi LIKE @DiaChi");
            }
            if (filterObject.ThoiDiemDangKyBatDau.HasValue)
            {
                whereClauses.Add("ThoiDiemDangKy >= @ThoiDiemDangKyBatDau");
            }
            if (filterObject.ThoiDiemDangKyKetThuc.HasValue)
            {
                whereClauses.Add("ThoiDiemDangKy <= @ThoiDiemDangKyKetThuc");
            }
            if (filterObject.MaLichThi.HasValue)
            {
                whereClauses.Add("MaLichThi = @MaLichThi");
            }
            if (!string.IsNullOrEmpty(filterObject.TrangThai))
            {
                whereClauses.Add("TrangThai = @TrangThai");
            }
            if (!string.IsNullOrEmpty(filterObject.LoaiKhachHang))
            {
                whereClauses.Add("LoaiKhachHang = @LoaiKhachHang");
            }

            return whereClauses.Count > 0
                ? "WHERE " + string.Join(" AND ", whereClauses)
                : string.Empty;
        }
        private DbParameter[] BuidlerDbParametersForRegisterInformationQuery(RegisterInformationFilterObject filterObject, DbConnection dbConnection)
        {
            var dbParameters = new List<DbParameter>();

            if (filterObject.MaTTDangKy.HasValue)
            {
                var maTTDangKyParam = dbConnection.CreateCommand().CreateParameter();
                maTTDangKyParam.ParameterName = "@MaTTDangKy";
                maTTDangKyParam.Value = filterObject.MaTTDangKy.Value;
                dbParameters.Add(maTTDangKyParam);
            }
            if (!string.IsNullOrEmpty(filterObject.HoTen))
            {
                var hoTenParam = dbConnection.CreateCommand().CreateParameter();
                hoTenParam.ParameterName = "@HoTen";
                hoTenParam.Value = "%" + filterObject.HoTen + "%";
                dbParameters.Add(hoTenParam);
            }
            if (!string.IsNullOrEmpty(filterObject.SDT))
            {
                var sdtParam = dbConnection.CreateCommand().CreateParameter();
                sdtParam.ParameterName = "@SDT";
                sdtParam.Value = "%" + filterObject.SDT + "%";
                dbParameters.Add(sdtParam);
            }
            if (!string.IsNullOrEmpty(filterObject.Email))
            {
                var emailParam = dbConnection.CreateCommand().CreateParameter();
                emailParam.ParameterName = "@Email";
                emailParam.Value = "%" + filterObject.Email + "%";
                dbParameters.Add(emailParam);
            }
            if (!string.IsNullOrEmpty(filterObject.DiaChi))
            {
                var diaChiParam = dbConnection.CreateCommand().CreateParameter();
                diaChiParam.ParameterName = "@DiaChi";
                diaChiParam.Value = "%" + filterObject.DiaChi + "%";
                dbParameters.Add(diaChiParam);
            }
            if (filterObject.ThoiDiemDangKyBatDau.HasValue)
            {
                var thoiDiemDangKyBatDauParam = dbConnection.CreateCommand().CreateParameter();
                thoiDiemDangKyBatDauParam.ParameterName = "@ThoiDiemDangKyBatDau";
                thoiDiemDangKyBatDauParam.Value = filterObject.ThoiDiemDangKyBatDau.Value;
                dbParameters.Add(thoiDiemDangKyBatDauParam);
            }
            if (filterObject.ThoiDiemDangKyKetThuc.HasValue)
            {
                var thoiDiemDangKyKetThucParam = dbConnection.CreateCommand().CreateParameter();
                thoiDiemDangKyKetThucParam.ParameterName = "@ThoiDiemDangKyKetThuc";
                thoiDiemDangKyKetThucParam.Value = filterObject.ThoiDiemDangKyKetThuc.Value;
                dbParameters.Add(thoiDiemDangKyKetThucParam);
            }
            if (filterObject.MaLichThi.HasValue)
            {
                var maLichThiParam = dbConnection.CreateCommand().CreateParameter();
                maLichThiParam.ParameterName = "@MaLichThi";
                maLichThiParam.Value = filterObject.MaLichThi.Value;
                dbParameters.Add(maLichThiParam);
            }
            if (!string.IsNullOrEmpty(filterObject.TrangThai))
            {
                var trangThaiParam = dbConnection.CreateCommand().CreateParameter();
                trangThaiParam.ParameterName = "@TrangThai";
                trangThaiParam.Value = filterObject.TrangThai;
                dbParameters.Add(trangThaiParam);
            }
            if (!string.IsNullOrEmpty(filterObject.LoaiKhachHang))
            {
                var loaiKhachHangParam = dbConnection.CreateCommand().CreateParameter();
                loaiKhachHangParam.ParameterName = "@LoaiKhachHang";
                loaiKhachHangParam.Value = filterObject.LoaiKhachHang;
                dbParameters.Add(loaiKhachHangParam);
            }


            return dbParameters.ToArray();
        }
        public PagedResult<Entity.RegisterInformation> LoadRegisterInformation(int pageSize, int currentPageNumber, RegisterInformationFilterObject filterObject)
        {
            using (var dbConnection = dataClient.GetDbConnection())
            {
                string baseSql = @"
                SELECT * 
                FROM TTDANGKY";
                string whereClause = BuildWhereClauseForRegisterInformationQuery(filterObject);
                if (!string.IsNullOrEmpty(whereClause))
                {
                    baseSql += " " + whereClause;
                }
                string orderByClause = "ORDER BY MaTTDangKy";

                var dbParameters = BuidlerDbParametersForRegisterInformationQuery(filterObject, dbConnection);

                return Helper.PaginationHelper.ExecutePagedAsync<Entity.RegisterInformation>(
                    dbConnection,
                    baseSql,
                    orderByClause,
                    registerInformationMapFunc,
                    currentPageNumber,
                    pageSize,
                    dbParameters).GetAwaiter().GetResult();
            }
        }

        public Entity.RegisterInformation? LoadRegisterInformationById(int maTTDangKy)
        {
            using (var dbConnection = dataClient.GetDbConnection())
            {
                try
                {
                    string sql = @"
                SELECT * 
                FROM TTDANGKY
                WHERE MaTTDangKy = @MaTTDangKy";
                    if (dbConnection.State != System.Data.ConnectionState.Open)
                    {
                        dbConnection.Open();
                    }
                    using (var command = dbConnection.CreateCommand())
                    {
                        command.CommandText = sql;
                        var maTTDangKyParam = command.CreateParameter();
                        maTTDangKyParam.ParameterName = "@MaTTDangKy";
                        maTTDangKyParam.Value = maTTDangKy;
                        command.Parameters.Add(maTTDangKyParam);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return registerInformationMapFunc(reader);
                            }
                            else
                            {
                                return null; // No record found
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error while loading register information by ID", ex);
                }
            }
        }

        public List<CandidateInformation> LoadCandidatesInformation(int maTTDangKy)
        {
            using(var  dbConnection = dataClient.GetDbConnection())
            {
                try
                {
                    string sql = """
                        SELECT * FROM TTHISINH
                        WHERE MaTTDangKy = @MaTTDangKy
                        """;

                    if (dbConnection.State != System.Data.ConnectionState.Open)
                    {
                        dbConnection.Open();
                    }

                    using (var command = dbConnection.CreateCommand())
                    {
                        command.CommandText = sql;
                        var maTTDangKyParam = command.CreateParameter();
                        maTTDangKyParam.ParameterName = "@MaTTDangKy";
                        maTTDangKyParam.Value = maTTDangKy;
                        command.Parameters.Add(maTTDangKyParam);
                        using (var reader = command.ExecuteReader())
                        {
                            List<CandidateInformation> candidates = new List<CandidateInformation>();
                            if(!reader.HasRows)
                            {
                                return candidates; // Return empty list if no candidates found
                            }
                            while (reader.Read())
                            {
                                var candidate = candidateInformationMapFunc(reader);
                                candidates.Add(candidate);
                            }
                            return candidates;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error while loading candidates information", ex);
                }
            }
        }

        public int UpdateRegisterInformation(Entity.RegisterInformation registerInformation)
        {
            using (var connection = dataClient.GetDbConnection())
            {
                try
                {
                    string sql = """
                    UPDATE TTDANGKY
                    SET 
                        HoTen = @HoTen,
                        SDT = @SDT,
                        Email = @Email,
                        DiaChi = @DiaChi,
                        ThoiDiemDangKy = @ThoiDiemDangKy,
                        MaLichThi = @MaLichThi,
                        TrangThai = @TrangThai,
                        LoaiKhachHang = @LoaiKhachHang
                    WHERE MaTTDangKy = @MaTTDangKy;
                    """;
                    if (connection.State != System.Data.ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    DbParameter[] parameters = buildParametersForRegisterInformation(registerInformation, connection);

                    using (var command = connection.CreateCommand()) { 
                        command.CommandText = sql;
                        command.Parameters.AddRange(parameters);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected;
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception("Error while updating register information", ex);
                }

            }
        }
    }
}
