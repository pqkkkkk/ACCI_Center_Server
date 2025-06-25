using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACCI_Center.FilterField;
using ACCI_Center.Entity;
using ACCI_Center.Dao.ExamSchedule;
using ACCI_Center.Dto;
using ACCI_Center.Dto.Reponse;

namespace ACCI_Center.Service.ExamSchedule
{
    public class ExamScheduleService : IExamScheduleService
    {
        private readonly IExamScheduleDao examScheduleDao;
        public ExamScheduleService(IExamScheduleDao examScheduleDao)
        {
            this.examScheduleDao = examScheduleDao;
        }
        public int CreateExamSchedule(Entity.ExamSchedule examSchedule)
        {
            throw new NotImplementedException();
        }

        public int EnterExamResult()
        {
            throw new NotImplementedException();
        }

        public Entity.ExamSchedule? GetExamScheduleById(int MaLichThi)
        {
            throw new NotImplementedException();
        }

        public List<CandidateInformation> LoadCandidatesOfAExamSchedule(int MaLichThi)
        {
            throw new NotImplementedException();
        }

        public PagedResult<Entity.ExamSchedule> LoadExamSchedules(int pageSize, int currentPageNumber, ExamScheduleFilterObject filterObject)
        {
            try
            {
                return examScheduleDao.GetExamSchedules(pageSize, currentPageNumber, filterObject);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new PagedResult<Entity.ExamSchedule>(null, 0, 0, 0);
            }
        }
        public PagedResult<Test> LoadTests(int pageSize, int currentPageNumber, TestFilterObject testFilterObject)
        {
            try
            {
                return examScheduleDao.GetTests(pageSize, currentPageNumber,testFilterObject);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new PagedResult<Test>(null, 0, 0,0);
            }
        }

        public int NotifyAboutReceivingExamResult()
        {
            throw new NotImplementedException();
        }
        public List<AvailableExamScheduleReponse> LoadAvailableExamSchedules()
        {
            try
            {
                return examScheduleDao.GetAvailableExamSchedules();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new List<AvailableExamScheduleReponse>();
            }
        }
    }
}
