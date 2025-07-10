namespace ACCI_Center.Dto.Response
{
    public class ApproveOrganizationRegisterResponse
    {
        public Entity.RegisterInformation registerInformation { get; set; } = new Entity.RegisterInformation();
        public Entity.ExamSchedule examSchedule { get; set; } = new Entity.ExamSchedule();
        public List<Entity.CandidateInformation> candidateInformations { get; set; } = new List<Entity.CandidateInformation>();
        public int statusCode { get; set; } = 200;
        public string message { get; set; } = "Success";
    }
}
