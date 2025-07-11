using ACCI_Center.Dto.Request;
using ACCI_Center.Dto.Response;

namespace ACCI_Center.Service.ExamSchedule
{
    public interface IExamScheduleServiceV2
    {
        public CreateExamScheduleResponse CreateExamSchedule(CreateExamScheduleRequest request);
        public AvailableEmployeesResponse GetAvailableEmployees(DateTime desiredExamTime, int testId);
        public AvailableRoomsResponse GetAvailableRooms(DateTime desiredExamTime, int testId);
    }
}
