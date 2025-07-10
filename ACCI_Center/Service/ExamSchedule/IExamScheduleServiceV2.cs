using ACCI_Center.Dto.Request;
using ACCI_Center.Dto.Response;

namespace ACCI_Center.Service.ExamSchedule
{
    public interface IExamScheduleServiceV2
    {
        public CreateExamScheduleResponse CreateExamSchedule(CreateExamScheduleRequest request);
    }
}
