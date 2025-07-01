using ACCI_Center.Configuraion;
using ACCI_Center.Dto;
using ACCI_Center.FilterField;
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

        public ExtensionInformationSqlDao(IDataClient dataClient)
        {
            this.dataClient = dataClient;
        }
        Func<DbDataReader, Entity.ExtensionInformation> extensionInformationMapFunc = (reader) =>
        {
            return new Entity.ExtensionInformation
            {
                MaTTGiaHan = reader.GetInt32(reader.GetOrdinal("MaTTGiaHan")),
                ThoiDiemGiaHan = reader.GetDateTime(reader.GetOrdinal("ThoiDiemGiaHan")),
                LoaiGiaHan = reader.GetString(reader.GetOrdinal("LoaiGiaHan")),
                LyDo = reader.GetString(reader.GetOrdinal("LyDo")),
                TrangThai = reader.GetString(reader.GetOrdinal("TrangThai")),
                PhiGiaHan = reader.GetFloat(reader.GetOrdinal("PhiGiaHan")),
                MaTTDangKy = reader.GetInt32(reader.GetOrdinal("MaTTDangKy"))
            };
        };
        public int AddExtensionInformation(Entity.ExtensionInformation extension)
        {
            using (var dbConnection = dataClient.GetDbConnection())
            {
                try
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
                        if (dbConnection.State != System.Data.ConnectionState.Open)
                        {
                            dbConnection.Open();
                        }
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

                        var result = command.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : -1;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error while adding extension information: " + ex.Message, ex);
                }
            }
        }
        private string BuildWhereClauseForExtensionInformationQuery(ExtensionInformationFilterObject filterObject)
        {
            var whereClauses = new List<string>();
            if (filterObject != null)
            {
                if (filterObject.MaTTGiaHan.HasValue && filterObject.MaTTGiaHan > 0)
                {
                    whereClauses.Add("MaTTGiaHan = @MaTTGiaHan");
                }
                if(filterObject.LoaiGiaHan != null)
                {
                    whereClauses.Add("LoaiGiaHan = @LoaiGiaHan");
                }
                if(filterObject.TrangThai != null)
                {
                    whereClauses.Add("TrangThai = @TrangThai");
                }
                if(filterObject.ThoiDiemGiaHanBatDau.HasValue)
                {
                    whereClauses.Add("ThoiDiemGiaHan >= @ThoiDiemGiaHanBatDau");
                }
                if (filterObject.ThoiDiemGiaHanKetThuc.HasValue)
                {
                    whereClauses.Add("ThoiDiemGiaHan <= @ThoiDiemGiaHanKetThuc");
                }
                if(filterObject.MaTTDangKy.HasValue)
                {
                    whereClauses.Add("MaTTDangKy = @MaTTDangKy");
                }
            }
            return whereClauses.Count > 0 ? "WHERE " + string.Join(" AND ", whereClauses) : string.Empty;
        }
        private DbParameter[] BuildDbParametersForExtensionInformationQuery(ExtensionInformationFilterObject filterObject, DbConnection dbConnection)
        {
            var dbParameters = new List<DbParameter>();
            if (filterObject != null)
            {
                if (filterObject.MaTTGiaHan.HasValue && filterObject.MaTTGiaHan > 0)
                {
                    var param = dbConnection.CreateCommand().CreateParameter();
                    param.ParameterName = "@MaTTGiaHan";
                    param.Value = filterObject.MaTTGiaHan.Value;
                    dbParameters.Add(param);
                }
                if (filterObject.LoaiGiaHan != null)
                {
                    var param = dbConnection.CreateCommand().CreateParameter();
                    param.ParameterName = "@LoaiGiaHan";
                    param.Value = filterObject.LoaiGiaHan;
                    dbParameters.Add(param);
                }
                if (filterObject.TrangThai != null)
                {
                    var param = dbConnection.CreateCommand().CreateParameter();
                    param.ParameterName = "@TrangThai";
                    param.Value = filterObject.TrangThai;
                    dbParameters.Add(param);
                }
                if (filterObject.ThoiDiemGiaHanBatDau.HasValue)
                {
                    var param = dbConnection.CreateCommand().CreateParameter();
                    param.ParameterName = "@ThoiDiemGiaHanBatDau";
                    param.Value = filterObject.ThoiDiemGiaHanBatDau.Value;
                    dbParameters.Add(param);
                }
                if (filterObject.ThoiDiemGiaHanKetThuc.HasValue)
                {
                    var param = dbConnection.CreateCommand().CreateParameter();
                    param.ParameterName = "@ThoiDiemGiaHanKetThuc";
                    param.Value = filterObject.ThoiDiemGiaHanKetThuc.Value;
                    dbParameters.Add(param);
                }
                if (filterObject.MaTTDangKy.HasValue && filterObject.MaTTDangKy > 0)
                {
                    var param = dbConnection.CreateCommand().CreateParameter();
                    param.ParameterName = "@MaTTDangKy";
                    param.Value = filterObject.MaTTDangKy.Value;
                    dbParameters.Add(param);
                }
            }
            return dbParameters.ToArray();
        }
        public PagedResult<Entity.ExtensionInformation> LoadExtendInformation(int pageSize,
            int currentPageNumber,
            FilterField.ExtensionInformationFilterObject filterObject)
        {
            using (var dbConnection = dataClient.GetDbConnection())
            {
                string baseSql = @"
                SELECT * 
                FROM TTGIAHAN";
                string whereClause = BuildWhereClauseForExtensionInformationQuery(filterObject);
                if (!string.IsNullOrEmpty(whereClause))
                {
                    baseSql += " " + whereClause;
                }
                string orderByClause = "ORDER BY MaTTGiaHan";

                var dbParameters = BuildDbParametersForExtensionInformationQuery(filterObject, dbConnection);

                return Helper.PaginationHelper.ExecutePagedAsync<Entity.ExtensionInformation>(
                    dbConnection,
                    baseSql,
                    orderByClause,
                    extensionInformationMapFunc,
                    currentPageNumber,
                    pageSize,
                    dbParameters).GetAwaiter().GetResult();
            }
        }

        public int GetExtensionTime(int registerInformationId)
        {
            using (var dbConnection = dataClient.GetDbConnection())
            {
                string sql = @"
                SELECT COUNT(*)
                FROM TTGIAHAN
                WHERE MaTTDangKy = @MaTTDangKy
            ";

                using (var command = dbConnection.CreateCommand())
                {
                    command.CommandText = sql;

                    var param = command.CreateParameter();
                    param.ParameterName = "@MaTTDangKy";
                    param.Value = registerInformationId;
                    command.Parameters.Add(param);

                    dbConnection.Open();
                    var result = command.ExecuteScalar();
                    dbConnection.Close();

                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }
    }
}
