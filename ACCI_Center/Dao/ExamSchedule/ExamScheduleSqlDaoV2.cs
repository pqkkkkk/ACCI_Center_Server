using ACCI_Center.Configuraion;
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
                            INSERT INTO LICHTHI (BaiThi, NgayThi, SoLuongThiSinhHienTai, DaNhapKetQuaThi, DaThongBaoKetQuaThi, PhongThi)
                            VALUES (@BaiThi, @NgayThi, @SoLuongThiSinhHienTai, @DaNhapKetQuaThi, @DaThongBaoKetQuaThi, @PhongThi);
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
    }
}
