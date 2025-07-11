using ACCI_Center.Configuraion;
using ACCI_Center.Entity;
using System.Data.Common;

namespace ACCI_Center.Dao.ExamSchedule
{
    public class ExamScheduleSqlDaoV2 : IExamScheduleDaoV2
    {
        private readonly IDataClient dataClient;
        public ExamScheduleSqlDaoV2(IDataClient dataClient)
        {
            this.dataClient = dataClient;
        }
        public int AddExamSchedule(Entity.ExamSchedule examSchedule, List<int> supervisorIds)
        {
            using (var dbConnection = dataClient.GetDbConnection())
            {
                try
                {
                    if (dbConnection.State != System.Data.ConnectionState.Open)
                    {
                        dbConnection.Open();
                    }

                    int examScheduleId = -1;

                    string sql = """
                            INSERT INTO LICHTHI (BaiThi, NgayThi, SoLuongThiSinhHienTai, DaNhapKetQuaThi, DaThongBaoKetQuaThi, PhongThi, LoaiLichThi)
                            VALUES (@BaiThi, @NgayThi, @SoLuongThiSinhHienTai, @DaNhapKetQuaThi, @DaThongBaoKetQuaThi, @PhongThi, @LoaiLichThi);
                            SELECT CAST(SCOPE_IDENTITY() AS int);
                            """;
                    DbParameter[] parameters = ExamScheduleDaoUtil.BuildParametersForAddExamSchedule(examSchedule, dbConnection);
                    using (var command = dbConnection.CreateCommand())
                    {
                        command.CommandText = sql;
                        command.Parameters.AddRange(parameters);

                        var result = command.ExecuteScalar();
                        examScheduleId = result != null ? Convert.ToInt32(result) : -1;
                    }

                    // Insert multiple supervisors
                    if (supervisorIds?.Any() == true)
                    {
                        string insertEmployeeSql = """
                                INSERT INTO NHANVIENCOITHI (MaNhanVien, MaLichThi)
                                VALUES (@MaNhanVien, @MaLichThi);
                                """;

                        foreach (int supervisorId in supervisorIds)
                        {
                            using (var command = dbConnection.CreateCommand())
                            {
                                command.CommandText = insertEmployeeSql;

                                var employeeParam = command.CreateParameter();
                                employeeParam.ParameterName = "@MaNhanVien";
                                employeeParam.Value = supervisorId;
                                command.Parameters.Add(employeeParam);

                                var examScheduleIdParam = command.CreateParameter();
                                examScheduleIdParam.ParameterName = "@MaLichThi";
                                examScheduleIdParam.Value = examScheduleId;
                                command.Parameters.Add(examScheduleIdParam);

                                command.ExecuteNonQuery();
                            }
                        }
                    }

                    return examScheduleId;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error while adding exam schedule: " + ex.Message, ex);
                }
            }
        }

        public List<Room> GetAllEmptyRoomIds(DateTime desiredExamTime, int testId)
        {
            using (var dbConnection = dataClient.GetDbConnection())
            {
                try
                {
                    string sql = """
                        SELECT * FROM PHONGTHI WHERE MaPhongThi IN (
                                select MaPhongThi
                                from PHONGTHI
                                except
                                select PhongThi
                                from LICHTHI lt join BAITHI bt on lt.BaiThi = bt.MaBaiThi
                                where (lt.NgayThi <= @desiredEndExamTime and lt.NgayThi >= @desiredStartExamTime)
                        		        or (DATEADD(minute, bt.ThoiGianThi, lt.NgayThi) >= @desiredStartExamTime and DATEADD(minute, bt.ThoiGianThi, lt.NgayThi) <= @desiredStartExamTime)
                                )
                        """;

                    if (dbConnection.State != System.Data.ConnectionState.Open)
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
                        List<Entity.Room> emptyRooms = new List<Entity.Room>();

                        while (reader.Read())
                        {
                            var room = ExamScheduleDaoUtil.roomMapper(reader);
                            if (room != null)
                            {
                                emptyRooms.Add(room);
                            }
                        }

                        return emptyRooms;
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
        }
        
        public List<Employee> GetAllFreeEmployeeIds(DateTime desiredExamTime, int testId)
        {
            using (var dbConnection = dataClient.GetDbConnection())
            {
                try
                {
                    string sql = """
                    SELECT * FROM NHANVIEN WHERE MaNhanVien IN (
                                select MaNhanVien
                                from NHANVIEN
                                except
                                select MaNhanVien
                                from NHANVIENCOITHI nvct join LICHTHI lt on lt.MaLichThi = nvct.MaLichThi
                						                 join BAITHI bt on lt.BaiThi = bt.MaBaiThi
                                where (lt.NgayThi <= @desiredEndExamTime2 and lt.NgayThi >= @desiredStartExamTime2)
                		                or (DATEADD(minute, bt.ThoiGianThi, lt.NgayThi) >= @desiredStartExamTime2 and DATEADD(minute, bt.ThoiGianThi, lt.NgayThi) <= @desiredStartExamTime2)
                                        )
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
                        List<Entity.Employee> freeEmployees = new List<Entity.Employee>();
                        while (reader.Read())
                        {
                            var employee = ExamScheduleDaoUtil.employeeMapper(reader);
                            if (employee != null)
                            {
                                freeEmployees.Add(employee);
                            }
                        }

                        return freeEmployees;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error while getting free employee IDs: " + ex.Message, ex);
                }
            }
        }

        public int UpdateExamSchedule(Entity.ExamSchedule examSchedule, List<int> supervisorIds)
        {
            using (var dbConnection = dataClient.GetDbConnection())
            {
                try
                {
                    if (dbConnection.State != System.Data.ConnectionState.Open)
                    {
                        dbConnection.Open();
                    }

                    string sql = """
                            UPDATE LICHTHI
                            SET BaiThi = @BaiThi,
                                NgayThi = @NgayThi,
                                SoLuongThiSinhHienTai = @SoLuongThiSinhHienTai,
                                DaNhapKetQuaThi = @DaNhapKetQuaThi,
                                DaThongBaoKetQuaThi = @DaThongBaoKetQuaThi,
                                DaPhatHanhPhieuDuThi = @DaPhatHanhPhieuDuThi,
                                LoaiLichThi = @LoaiLichThi,
                                PhongThi = @PhongThi,
                                TrangThaiDuyet = @TrangThaiDuyet
                            WHERE MaLichThi = @MaLichThi;
                            """;
                    DbParameter[] parameters = ExamScheduleDaoUtil.BuildParametersForAddExamSchedule(examSchedule, dbConnection);
                    var examScheduleIdParamForUpdate = dbConnection.CreateCommand().CreateParameter();
                    examScheduleIdParamForUpdate.ParameterName = "@MaLichThi";
                    examScheduleIdParamForUpdate.Value = examSchedule.MaLichThi;

                    using (var command = dbConnection.CreateCommand())
                    {
                        command.CommandText = sql;
                        command.Parameters.AddRange(parameters);
                        command.Parameters.Add(examScheduleIdParamForUpdate);

                        var rowAffected = command.ExecuteNonQuery();
                        if (rowAffected == 0)
                        {
                            throw new Exception("No rows were updated. Please check the exam schedule ID.");
                        }
                    }

                    // Insert multiple supervisors
                    if (supervisorIds?.Any() == true)
                    {
                        string insertEmployeeSql = """
                                INSERT INTO NHANVIENCOITHI (MaNhanVien, MaLichThi)
                                VALUES (@MaNhanVien, @MaLichThi);
                                """;

                        foreach (int supervisorId in supervisorIds)
                        {
                            using (var command = dbConnection.CreateCommand())
                            {
                                command.CommandText = insertEmployeeSql;

                                var employeeParam = command.CreateParameter();
                                employeeParam.ParameterName = "@MaNhanVien";
                                employeeParam.Value = supervisorId;
                                command.Parameters.Add(employeeParam);

                                var examScheduleIdParam = command.CreateParameter();
                                examScheduleIdParam.ParameterName = "@MaLichThi";
                                examScheduleIdParam.Value = examSchedule.MaLichThi;
                                command.Parameters.Add(examScheduleIdParam);

                                command.ExecuteNonQuery();
                            }
                        }
                    }

                    return 1;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error while adding exam schedule: " + ex.Message, ex);
                }
            }
        }
    }
}
