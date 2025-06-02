using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACCI_Center.FilterField;
using ACCI_Center.Entity;

namespace ACCI_Center.Service.ExamSchedule
{
    public class ExamScheduleService  : PaginationService<Entity.ExamSchedule>, IExamScheduleService
    {
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

        public List<Entity.ExamSchedule> LoadExamSchedules()
        {
            throw new NotImplementedException();
        }

        public List<Entity.ExamSchedule> LoadExamSchedules(Dictionary<ExamScheduleFilterField, object> filterFields)
        {
            throw new NotImplementedException();
        }

        public int NotifyAboutReceivingExamResult()
        {
            throw new NotImplementedException();
        }

        public int ReleaseExamRegisterForm()
        {
            throw new NotImplementedException();
        }
    }
}
