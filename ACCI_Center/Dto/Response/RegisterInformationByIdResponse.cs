namespace ACCI_Center.Dto.Response
{
    public class RegisterInformationByIdResponse
    {
        public Entity.RegisterInformation RegisterInformation { get; set; } = new Entity.RegisterInformation();
        public Entity.ExamSchedule? examSchedule { get; set; } = null;
        public Entity.Test? test { get; set; } = null;
        public List<Entity.CandidateInformation>? candidateInformations { get; set; } = null;
        public int statusCode { get; set; } = StatusCodes.Status500InternalServerError;
        public string message { get; set; } = "Success";
    }
}
