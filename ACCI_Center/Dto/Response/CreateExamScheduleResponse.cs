namespace ACCI_Center.Dto.Response
{
    public class CreateExamScheduleResponse
    {
        public Entity.ExamSchedule examSchedule { get; set; }
        public int statusCode { get; set; } = 500;
        public string message { get; set; } = "An error occurred while creating the exam schedule.";
    }
}
