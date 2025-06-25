using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACCI_Center.FilterField;
using ACCI_Center.Dto.Reponse;
using ACCI_Center.Dto;

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
        public PagedResult<Entity.ExamSchedule> GetExamSchedules(
                                            int pageSize,
                                            int currentPageNumber,
                                            ExamScheduleFilterObject filterObject);
        public double GetFeeOfTheTest(int testId);
        public Entity.Test GetTestById(int testId);
        public Entity.ExamSchedule GetExamScheduleById(int examScheduleId);
        public List<int> GetAllEmptyRoomIds(DateTime desiredExamTime, int testId);
        public List<int> GetAllFreeEmployeeIds(DateTime desiredExamTime, int testId);
        public List<AvailableExamScheduleReponse> GetAvailableExamSchedules();
        public int GetTestIdByExamScheduleId(int examScheduleId);
        public bool UpdateQuantityOfExamSchedule(int examScheduleId, int quantity);
        public List<Entity.ExamSchedule> GetExamSchedulesForNext2Week();
        public List<Entity.CandidateInformation> GetCandidatesByExamScheduleId(int id);
    }
}
