namespace ACCI_Center.Dto.Request
{
    public class CreateExamScheduleRequest
    {
        public Entity.ExamSchedule examSchedule { get; set; }
        public List<int> supervisorIds { get; set; }
    }
}
