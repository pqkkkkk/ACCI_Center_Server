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
    }
}
