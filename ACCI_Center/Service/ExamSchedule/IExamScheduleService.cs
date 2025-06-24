using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACCI_Center.FilterField;
using ACCI_Center.Entity;
using ACCI_Center.Dto;
using ACCI_Center.Dto.Reponse;
namespace ACCI_Center.Service.ExamSchedule
{
    public interface IExamScheduleService
    {
        public int CreateExamSchedule(Entity.ExamSchedule examSchedule);
        public int EnterExamResult();
        public int NotifyAboutReceivingExamResult();
        public List<Entity.ExamSchedule> LoadExamSchedules();
        public PagedResult<Entity.Test> LoadTests(int pageSize, int currentPageNumber, TestFilterObject testFilterObject);
        public Entity.ExamSchedule? GetExamScheduleById(int MaLichThi);
        public List<Entity.CandidateInformation> LoadCandidatesOfAExamSchedule(int MaLichThi);
        public List<AvailableExamScheduleReponse> LoadAvailableExamSchedules();
    }
}
