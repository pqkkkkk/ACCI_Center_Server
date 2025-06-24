using ACCI_Center.Configuraion;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCI_Center.Dao.ExtensionInformation
{
    public class ExtensionInformationSqlDao : IExtensionInformationDao
    {
        private readonly IDataClient dataClient;
        private readonly DbConnection dbConnection;

        public ExtensionInformationSqlDao(IDataClient dataClient)
        {
            this.dataClient = dataClient;
            dbConnection = dataClient.GetDbConnection();
        }
        public int AddExtensionInformation(Entity.ExtensionInformation extension)
        {
            string sql = """
                INSERT INTO ACCI_Center.dbo.TTGIAHAN (
                    ThoiDiemGiaHan, LoaiGiaHan, LyDo, TrangThai, PhiGiaHan, MaTTDangKy
                ) VALUES (
                    @ThoiDiemGiaHan, @LoaiGiaHan, @LyDo, @TrangThai, @PhiGiaHan, @MaTTDangKy
                );
                SELECT CAST(SCOPE_IDENTITY() AS int);
            """;

            using (var command = dbConnection.CreateCommand())
            {
                command.CommandText = sql;

                var thoiDiemParam = command.CreateParameter();
                thoiDiemParam.ParameterName = "@ThoiDiemGiaHan";
                thoiDiemParam.Value = extension.ThoiDiemGiaHan;
                command.Parameters.Add(thoiDiemParam);

                var loaiParam = command.CreateParameter();
                loaiParam.ParameterName = "@LoaiGiaHan";
                loaiParam.Value = extension.LoaiGiaHan;
                command.Parameters.Add(loaiParam);

                var lyDoParam = command.CreateParameter();
                lyDoParam.ParameterName = "@LyDo";
                lyDoParam.Value = extension.LyDo;
                command.Parameters.Add(lyDoParam);

                var trangThaiParam = command.CreateParameter();
                trangThaiParam.ParameterName = "@TrangThai";
                trangThaiParam.Value = extension.TrangThai;
                command.Parameters.Add(trangThaiParam);

                var phiParam = command.CreateParameter();
                phiParam.ParameterName = "@PhiGiaHan";
                phiParam.Value = extension.PhiGiaHan;
                command.Parameters.Add(phiParam);

                var maTTParam = command.CreateParameter();
                maTTParam.ParameterName = "@MaTTDangKy";
                maTTParam.Value = extension.MaTTDangKy;
                command.Parameters.Add(maTTParam);

                dbConnection.Open(); 
                var result = command.ExecuteScalar();
                dbConnection.Close(); 
                return result != null ? Convert.ToInt32(result) : -1;
            }
        }
        public List<Entity.ExtensionInformation> LoadExtendInformation(int pageSize,
            int currentPageNumber,
            FilterField.ExtensionInformationFilterObject filterObject)
        {
            string sql = """
                SELECT * FROM ACCI_Center.dbo.TTGIAHAN
                WHERE (@MaTTGiaHan IS NULL OR MaTTGiaHan = @MaTTGiaHan)
                AND (@LoaiGiaHan IS NULL OR LoaiGiaHan = @LoaiGiaHan)
                AND (@TrangThai IS NULL OR TrangThai = @TrangThai)
                ORDER BY ThoiDiemGiaHan DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;
            using (var command = dbConnection.CreateCommand())
            {
                command.CommandText = sql;

                var maTTParam = command.CreateParameter();
                maTTParam.ParameterName = "@MaTTGiaHan";
                maTTParam.Value = filterObject.MaTTGiaHan ?? (object)DBNull.Value;
                command.Parameters.Add(maTTParam);

                var loaiParam = command.CreateParameter();
                loaiParam.ParameterName = "@LoaiGiaHan";
                loaiParam.Value = filterObject.LoaiGiaHan ?? (object)DBNull.Value;
                command.Parameters.Add(loaiParam);

                var trangThaiParam = command.CreateParameter();
                trangThaiParam.ParameterName = "@TrangThai";
                trangThaiParam.Value = filterObject.TrangThai ?? (object)DBNull.Value;
                command.Parameters.Add(trangThaiParam);

                var offsetParam = command.CreateParameter();
                offsetParam.ParameterName = "@Offset";
                offsetParam.Value = (currentPageNumber - 1) * pageSize;
                command.Parameters.Add(offsetParam);

                var pageSizeParam = command.CreateParameter();
                pageSizeParam.ParameterName = "@PageSize";
                pageSizeParam.Value = pageSize;
                command.Parameters.Add(pageSizeParam);
                dbConnection.Open();

                using (var reader = command.ExecuteReader())
                {
                    var results = new List<Entity.ExtensionInformation>();
                    while (reader.Read())
                    {
                        results.Add(new Entity.ExtensionInformation
                        {
                            MaTTGiaHan = reader.GetInt32(reader.GetOrdinal("MaTTGiaHan")),
                            ThoiDiemGiaHan = reader.GetDateTime(reader.GetOrdinal("ThoiDiemGiaHan")),
                            LoaiGiaHan = reader.IsDBNull(reader.GetOrdinal("LoaiGiaHan")) ? null : reader.GetString(reader.GetOrdinal("LoaiGiaHan")),
                            LyDo = reader.IsDBNull(reader.GetOrdinal("LyDo")) ? null : reader.GetString(reader.GetOrdinal("LyDo")),
                            TrangThai = reader.IsDBNull(reader.GetOrdinal("TrangThai")) ? null : reader.GetString(reader.GetOrdinal("TrangThai")),
                            PhiGiaHan = Convert.ToDouble(reader.GetFloat(reader.GetOrdinal("PhiGiaHan"))),
                            MaTTDangKy = reader.GetInt32(reader.GetOrdinal("MaTTDangKy"))
                        }
                        );
                    }
                    dbConnection.Close();
                    return results;
                }
            }
        }
    }
}
