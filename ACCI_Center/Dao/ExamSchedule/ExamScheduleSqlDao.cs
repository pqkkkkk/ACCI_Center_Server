using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACCI_Center.Configuraion;
using ACCI_Center.Dto;
using ACCI_Center.Entity;

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

        public PagedResult<Test> GetTests(int pageSize, int currentPageNumber)
        {
            string baseSql = @"
                SELECT * 
                FROM BAITHI";
            string orderByClause = "ORDER BY MaBaiThi";

            Func<DbDataReader, Test> mapFunc = reader => new Test
            {
                MaBaiThi = reader.GetInt32(reader.GetOrdinal("MaBaiThi")),
                TenBaiThi = reader.GetString(reader.GetOrdinal("TenBaiThi")),
                SoLuongThiSinhToiDa = reader.GetInt32(reader.GetOrdinal("SoLuongThiSinhToiDa")),
                SoLuongThiSinhToiThieu = reader.GetInt32(reader.GetOrdinal("SoLuongThiSinhToiThieu")),
                GiaDangKy = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("GiaDangKy"))),
                LoaiBaiThi = reader.GetString(reader.GetOrdinal("LoaiBaiThi")),
                ThoiGianThi = reader.GetInt32(reader.GetOrdinal("ThoiGianThi"))
            };

            var dbParameters = new DbParameter[] { };

            return Helper.PaginationHelper.ExecutePagedAsync<Entity.Test>(
                dbConnection,
                baseSql,
                orderByClause,
                mapFunc,
                currentPageNumber,
                pageSize,
                dbParameters).GetAwaiter().GetResult();
        }
    }
}
