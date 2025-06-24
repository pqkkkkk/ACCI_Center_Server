using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACCI_Center.Configuraion;
using ACCI_Center.Dto;
using ACCI_Center.Dto.Reponse;
using ACCI_Center.Entity;
using ACCI_Center.FilterField;
using System.Data;

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

                return emptyRoomIds;
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
        public List<Entity.ExamSchedule> GetExamSchedulesForNext2Week()
       {
           List<Entity.ExamSchedule> examSchedule = new List<Entity.ExamSchedule>();
           string sql = "SELECT * FROM LICHTHI WHERE NgayThi BETWEEN GETDATE() AND DATEADD(WEEK, 2, GETDATE()) ";
           using (var command = dbConnection.CreateCommand())
           {
               if (dbConnection.State != ConnectionState.Open)
               {
                   dbConnection.Open();
               }
               command.CommandText = sql;
               using (var reader = command.ExecuteReader())
               {
                   while (reader.Read())
                   {
                       var schedule = new Entity.ExamSchedule
                       {
                           MaLichThi = reader.GetInt32(reader.GetOrdinal("MaLichThi")),
                           BaiThi = reader.GetInt32(reader.GetOrdinal("BaiThi")),
                           NgayThi = reader.GetDateTime(reader.GetOrdinal("NgayThi")),
                           SoLuongThiSinhHienTai = reader.GetInt32(reader.GetOrdinal("SoLuongThiSinhHienTai")),
                           DaNhapKetQuaThi = reader.GetBoolean(reader.GetOrdinal("DaNhapKetQuaThi")),
                           DaThongBaoKetQuaThi = reader.GetBoolean(reader.GetOrdinal("DaThongBaoKetQuaThi")),
                           PhongThi = reader.GetInt32(reader.GetOrdinal("PhongThi"))
                       };
                       examSchedule.Add(schedule);
                   }


               }
           }
           return examSchedule;
       }
      
       public List<Entity.CandidateInformation> GetCandidatesByExamScheduleId(int id)
       {
           List<Entity.CandidateInformation> candidates = new List<Entity.CandidateInformation>();
           string sql = """
               SELECT ts.*
               FROM TThiSinh ts JOIN TTDangKy dk on ts.MaTTDangKy = dk.MaTTDangKy
               WHERE dk.MaLichThi = @id
           """;
            using (var command = dbConnection.CreateCommand())
            {
                if (dbConnection.State != ConnectionState.Open)
                {
                    dbConnection.Open();
                }
                command.CommandText = sql;
                var idParam = command.CreateParameter();
                idParam.ParameterName = "@id";
                idParam.Value = id;
                command.Parameters.Add(idParam);


                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var candidate = new Entity.CandidateInformation
                        {
                            MaTTThiSinh = reader.GetInt32(reader.GetOrdinal("MaTTThiSinh")),
                            MaTTDangKy = reader.GetInt32(reader.GetOrdinal("MaTTDangKy")),
                            HoTen = reader.GetString(reader.GetOrdinal("HoTen")),
                            SDT = reader.GetString(reader.GetOrdinal("SDT")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            DaNhanChungChi = reader.GetBoolean(reader.GetOrdinal("DaNhanChungChi")),
                            DaGuiPhieuDuThi = reader.GetBoolean(reader.GetOrdinal("DaGuiPhieuDuThi"))
                        };
                        candidates.Add(candidate);
                    }
                }
            }
        }
        public List<AvailableExamScheduleReponse> GetAvailableExamSchedules()
        {
            string sql = """
                SELECT 
                    LT.MaLichThi,
                    BT.TenBaiThi,
                    BT.LoaiBaiThi,
                    LT.NgayThi,
                    LT.PhongThi,
                    LT.SoLuongThiSinhHienTai,
                    BT.SoLuongThiSinhToiDa,
                    BT.GiaDangKy
                FROM LICHTHI LT
                JOIN BAITHI BT ON LT.BaiThi = BT.MaBaiThi
                WHERE 
                    LT.NgayThi > GETDATE()
                    AND LT.SoLuongThiSinhHienTai < BT.SoLuongThiSinhToiDa
                """;
            var result = new List<AvailableExamScheduleReponse>();
            using (var command = dbConnection.CreateCommand())
            {
                command.CommandText = sql;
                if (dbConnection.State != ConnectionState.Open)
                {
                    dbConnection.Open();
                }
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var examSchedule = new AvailableExamScheduleReponse
                        {
                            MaLichThi = reader.GetInt32(reader.GetOrdinal("MaLichThi")),
                            TenBaiThi = reader.GetString(reader.GetOrdinal("TenBaiThi")),
                            LoaiBaiThi = reader.GetString(reader.GetOrdinal("LoaiBaiThi")),
                            NgayThi = reader.GetDateTime(reader.GetOrdinal("NgayThi")),
                            PhongThi = reader.GetInt32(reader.GetOrdinal("PhongThi")),
                            SoLuongThiSinhHienTai = reader.GetInt32(reader.GetOrdinal("SoLuongThiSinhHienTai")),
                            SoLuongThiSinhToiDa = reader.GetInt32(reader.GetOrdinal("SoLuongThiSinhToiDa")),
                            GiaDangKy = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("GiaDangKy")))
                        };
                        result.Add(examSchedule);
                    }
                }
            }
            return result;
        }
        public int GetTestIdByExamScheduleId(int examScheduleId)
        {
            string sql = """
                select BT.MaBaiThi
                from BAITHI BT
                join LICHTHI LT on LT.BaiThi=BT.MaBaiThi
                where LT.MaLichThi= @MaLichThi
                """;
            using (var command = dbConnection.CreateCommand())
            {
                command.CommandText = sql;
                if (dbConnection.State != ConnectionState.Open)
                {
                    dbConnection.Open();
                }
                var param = command.CreateParameter();
                param.ParameterName = "@MaLichThi";
                param.Value = examScheduleId;
                command.Parameters.Add(param);
                var result = command.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : -1;
            }
        }
        public bool UpdateQuantityOfExamSchedule(int examScheduleId, int quantity)
        {
            string sql = """
                UPDATE LICHTHI
                SET SoLuongThiSinhHienTai = SoLuongThiSinhHienTai + @Quantity
                WHERE MaLichThi = @MaLichThi;
                """;
            using (var command = dbConnection.CreateCommand())
            {
                command.CommandText = sql;
                if (dbConnection.State != ConnectionState.Open)
                {
                    dbConnection.Open();
                }
                var quantityParam = command.CreateParameter();
                quantityParam.ParameterName = "@Quantity";
                quantityParam.Value = quantity;
                command.Parameters.Add(quantityParam);
                
                var examScheduleIdParam = command.CreateParameter();
                examScheduleIdParam.ParameterName = "@MaLichThi";
                examScheduleIdParam.Value = examScheduleId;
                command.Parameters.Add(examScheduleIdParam);

                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
       return candidates;
       }
    }
}
