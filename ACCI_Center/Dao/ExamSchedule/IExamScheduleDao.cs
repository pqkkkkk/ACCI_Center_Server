using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACCI_Center.FilterField;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ACCI_Center.Dao.ExamSchedule
{
    public interface IExamScheduleDao
    {
        public int AddExamSchedule(Entity.ExamSchedule examSchedule,
                                    int employeeId);

        public Dto.PagedResult<Entity.Test> GetTests(
            int pageSize,
            int currentPageNumber,
            TestFilterObject testFilterObject);

        public double GetFeeOfTheTest(int testId);
        public Entity.Test GetTestById(int testId);
        public List<int> GetAllEmptyRoomIds(DateTime desiredExamTime, int testId);
        public List<int> GetAllFreeEmployeeIds(DateTime desiredExamTime, int testId);
        public List<Entity.ExamSchedule> GetExamSchedulesForNext2Week();
       public List<Entity.CandidateInformation> GetCandidatesByExamScheduleId(int id);

    }
}
