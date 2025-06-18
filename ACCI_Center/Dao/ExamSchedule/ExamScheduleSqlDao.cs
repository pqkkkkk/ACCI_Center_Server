using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACCI_Center.Configuraion;
using ACCI_Center.Dto;
using ACCI_Center.Entity;
using ACCI_Center.FilterField;

namespace ACCI_Center.Dao.ExamSchedule
{
    public class ExamScheduleSqlDao : IExamScheduleDao
    {
        private readonly IDataClient dataClient;
        private DbConnection dbConnection;
        public ExamScheduleSqlDao(IDataClient dataClient)
        {
            this.dataClient = dataClient;
            dbConnection = dataClient.GetDbConnection();
        }
        Func<DbDataReader, Test> testMapFunc = reader =>
        {
            return new Test
            {
                MaBaiThi = reader.GetInt32(reader.GetOrdinal("MaBaiThi")),
                TenBaiThi = reader.GetString(reader.GetOrdinal("TenBaiThi")),
                SoLuongThiSinhToiDa = reader.GetInt32(reader.GetOrdinal("SoLuongThiSinhToiDa")),
                SoLuongThiSinhToiThieu = reader.GetInt32(reader.GetOrdinal("SoLuongThiSinhToiThieu")),
                GiaDangKy = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("GiaDangKy"))),
                LoaiBaiThi = reader.GetString(reader.GetOrdinal("LoaiBaiThi")),
                ThoiGianThi = reader.GetInt32(reader.GetOrdinal("ThoiGianThi"))
            };
        };
        private string BuildWhereClauseForTestQuery(TestFilterObject testFilterObject)
        {
            var whereClauses = new List<string>();
            if (!string.IsNullOrEmpty(testFilterObject.LoaiBaiThi))
            {
                whereClauses.Add("LoaiBaiThi = @LoaiBaiThi");
            }
            if (!string.IsNullOrEmpty(testFilterObject.TenBaiThi))
            {
                whereClauses.Add("TenBaiThi LIKE @TenBaiThi");
            }
            return whereClauses.Count > 0 ? "WHERE " + string.Join(" AND ", whereClauses) : string.Empty;
        }
        private DbParameter[] BuidlerDbParametersForTestQuery(TestFilterObject testFilterObject)
        {
            var parameters = new List<DbParameter>();
            if (!string.IsNullOrEmpty(testFilterObject.LoaiBaiThi))
            {
                var param = dbConnection.CreateCommand().CreateParameter();
                param.ParameterName = "@LoaiBaiThi";
                param.Value = testFilterObject.LoaiBaiThi;
                parameters.Add(param);
            }
            if (!string.IsNullOrEmpty(testFilterObject.TenBaiThi))
            {
                var param = dbConnection.CreateCommand().CreateParameter();
                param.ParameterName = "@TenBaiThi";
                param.Value = "%" + testFilterObject.TenBaiThi + "%"; // Thêm ký tự % để tìm kiếm LIKE
                parameters.Add(param);
            }
            return parameters.ToArray();
        }
        public PagedResult<Test> GetTests(int pageSize, int currentPageNumber, TestFilterObject testFilterObject)
        {
            string baseSql = @"
                SELECT * 
                FROM BAITHI";
            string whereClause = BuildWhereClauseForTestQuery(testFilterObject);
            if (!string.IsNullOrEmpty(whereClause))
            {
                baseSql += " " + whereClause;
            }
            string orderByClause = "ORDER BY MaBaiThi";

            var dbParameters = BuidlerDbParametersForTestQuery(testFilterObject);

            return Helper.PaginationHelper.ExecutePagedAsync<Entity.Test>(
                dbConnection,
                baseSql,
                orderByClause,
                testMapFunc,
                currentPageNumber,
                pageSize,
                dbParameters).GetAwaiter().GetResult();
        }
        private DbParameter[] BuildParametersForAddExamSchedule(Entity.ExamSchedule examSchedule)
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
        public int AddExamSchedule(Entity.ExamSchedule examSchedule, int employeeId)
        {
            try
            {
                if (dbConnection.State != System.Data.ConnectionState.Open)
                {
                    dbConnection.Open();
                }

                int examScheduleId = -1;

                string sql = """
                INSERT INTO LICHTHI (BaiThi, NgayThi, SoLuongThiSinhHienTai, DaNhapKetQuaThi, DaThongBaoKetQuaThi, PhongThi)
                VALUES (@BaiThi, @NgayThi, @SoLuongThiSinhHienTai, @DaNhapKetQuaThi, @DaThongBaoKetQuaThi, @PhongThi);
                SELECT CAST(SCOPE_IDENTITY() AS int);
                """;
                DbParameter[] parameters = BuildParametersForAddExamSchedule(examSchedule);
                using (var command = dbConnection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Parameters.AddRange(parameters);

                    var result = command.ExecuteScalar();
                    examScheduleId = result != null ? Convert.ToInt32(result) : -1;
                }

                string insertEmployeeSql = """
                INSERT INTO NHANVIENCOITHI (MaNhanVien, MaLichThi)
                VALUES (@MaNhanVien, @MaLichThi);
                """;
                using (var command = dbConnection.CreateCommand())
                {
                    command.CommandText = insertEmployeeSql;
                    var employeeParam = command.CreateParameter();
                    employeeParam.ParameterName = "@MaNhanVien";
                    employeeParam.Value = employeeId;
                    command.Parameters.Add(employeeParam);
                    var examScheduleIdParam = command.CreateParameter();
                    examScheduleIdParam.ParameterName = "@MaLichThi";
                    examScheduleIdParam.Value = examScheduleId;
                    command.Parameters.Add(examScheduleIdParam);
                    command.ExecuteNonQuery();
                }

                return examScheduleId;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while adding exam schedule: " + ex.Message, ex);
            }
            finally
            {
                if (dbConnection.State == System.Data.ConnectionState.Open)
                {
                    dbConnection.Close();
                }
            }
        }

        public double GetFeeOfTheTest(int testId)
        {
            try
            {
                string sql = """
                SELECT GiaDangKy FROM BAITHI
                WHERE MaBaiThi = @MaBaiThi;
                """;

                if (dbConnection.State != System.Data.ConnectionState.Open)
                    dbConnection.Open();

                using (var command = dbConnection.CreateCommand())
                {
                    command.CommandText = sql;
                    var param = command.CreateParameter();
                    param.ParameterName = "@MaBaiThi";
                    param.Value = testId;
                    command.Parameters.Add(param);

                    var result = command.ExecuteScalar();
                    return result != null && result != DBNull.Value
                        ? Convert.ToDouble(result)
                        : -1.0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while getting fee of the test: " + ex.Message, ex);
            }
            finally {
                if (dbConnection.State == System.Data.ConnectionState.Open)
                {
                    dbConnection.Close();
                }
            }
        }

        public List<int> GetAllEmptyRoomIds(DateTime desiredExamTime, int testId)
        {
            try
            {
                string sql = """
                        select MaPhongThi
                        from PHONGTHI
                        except
                        select PhongThi
                        from LICHTHI lt join BAITHI bt on lt.BaiThi = bt.MaBaiThi
                        where (lt.NgayThi <= @desiredEndExamTime and lt.NgayThi >= @desiredStartExamTime)
                        		or (DATEADD(minute, bt.ThoiGianThi, lt.NgayThi) >= @desiredStartExamTime and DATEADD(minute, bt.ThoiGianThi, lt.NgayThi) <= @desiredStartExamTime);
                        """;

                if(dbConnection.State != System.Data.ConnectionState.Open)
                    dbConnection.Open();

                using (var command = dbConnection.CreateCommand())
                {
                    command.CommandText = sql;
                    var startExamTimeParam = command.CreateParameter();
                    startExamTimeParam.ParameterName = "@desiredStartExamTime";
                    startExamTimeParam.Value = desiredExamTime;
                    command.Parameters.Add(startExamTimeParam);
                    var endExamTimeParam = command.CreateParameter();
                    endExamTimeParam.ParameterName = "@desiredEndExamTime";
                    endExamTimeParam.Value = desiredExamTime.AddMinutes(30);
                    command.Parameters.Add(endExamTimeParam);

                    var reader = command.ExecuteReader();
                    List<int> emptyRoomIds = new List<int>();

                    while (reader.Read())
                    {
                        emptyRoomIds.Add(reader.GetInt32(0));
                    }

                    return emptyRoomIds;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while getting empty room IDs: " + ex.Message, ex);
            }
            finally
            {
                if (dbConnection.State == System.Data.ConnectionState.Open)
                {
                    dbConnection.Close();
                }
            }
        }

        public List<int> GetAllFreeEmployeeIds(DateTime desiredExamTime, int testId)
        {
            try
            {
                string sql = """
                select MaNhanVien
                from NHANVIEN
                except
                select MaNhanVien
                from NHANVIENCOITHI nvct join LICHTHI lt on lt.MaLichThi = nvct.MaLichThi
                						 join BAITHI bt on lt.BaiThi = bt.MaBaiThi
                where (lt.NgayThi <= @desiredEndExamTime2 and lt.NgayThi >= @desiredStartExamTime2)
                		or (DATEADD(minute, bt.ThoiGianThi, lt.NgayThi) >= @desiredStartExamTime2 and DATEADD(minute, bt.ThoiGianThi, lt.NgayThi) <= @desiredStartExamTime2);
                """;

                if (dbConnection.State != System.Data.ConnectionState.Open)
                    dbConnection.Open();

                using (var command = dbConnection.CreateCommand())
                {
                    command.CommandText = sql;
                    var startExamTimeParam = command.CreateParameter();
                    startExamTimeParam.ParameterName = "@desiredStartExamTime2";
                    startExamTimeParam.Value = desiredExamTime;
                    command.Parameters.Add(startExamTimeParam);
                    var endExamTimeParam = command.CreateParameter();
                    endExamTimeParam.ParameterName = "@desiredEndExamTime2";
                    endExamTimeParam.Value = desiredExamTime.AddMinutes(30);
                    command.Parameters.Add(endExamTimeParam);

                    var reader = command.ExecuteReader();
                    List<int> freeEmployeeIds = new List<int>();
                    while (reader.Read())
                    {
                        freeEmployeeIds.Add(reader.GetInt32(0));
                    }

                    return freeEmployeeIds;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while getting free employee IDs: " + ex.Message, ex);
            }
            finally
            {
                if (dbConnection.State == System.Data.ConnectionState.Open)
                {
                    dbConnection.Close();
                }
            }
        }

        public Test GetTestById(int testId)
        {
            try
            {
                string sql = """
                select * from BAITHI
                where MaBaiThi = @MaBaiThi;
                """;

                if(dbConnection.State != System.Data.ConnectionState.Open)
                    dbConnection.Open();

                using (var command = dbConnection.CreateCommand())
                {
                    command.CommandText = sql;
                    var param = command.CreateParameter();
                    param.ParameterName = "@MaBaiThi";
                    param.Value = testId;
                    command.Parameters.Add(param);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return testMapFunc(reader);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while getting test by ID: " + ex.Message, ex);
            }
            finally
            {
                if (dbConnection.State == System.Data.ConnectionState.Open)
                {
                    dbConnection.Close();
                }
            }
        }
    }
}
