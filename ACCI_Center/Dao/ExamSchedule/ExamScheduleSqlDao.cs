using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACCI_Center.Configuraion;
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
        public List<Entity.ExamSchedule> LoadData(int itemsPerPage, int currentPage)
        {
            dbConnection.Open();

            var command = dbConnection.CreateCommand();
            command.CommandText = "SELECT * FROM ExamSchedule ORDER BY NgayThi OFFSET @Offset ROWS FETCH NEXT @Fetch ROWS ONLY";
            var reader = command.ExecuteReader();

            List<Entity.ExamSchedule> examSchedules = new List<Entity.ExamSchedule>();
            while (reader.Read())
            {
                var examSchedule = DataReaderMapper.MapToObject<Entity.ExamSchedule>(reader);
                examSchedules.Add(examSchedule);
            }

            reader.Close();

            return examSchedules;
        }
    }
}
