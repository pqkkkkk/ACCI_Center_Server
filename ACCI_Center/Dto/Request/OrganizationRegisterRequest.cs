using ACCI_Center.Entity;

namespace ACCI_Center.Dto.Request
{
    public class OrganizationRegisterRequest
    {
        public Entity.RegisterInformation registerInformation { get; set; }
        public string testName { get; set; }
        public int testId { get; set; }
        public DateTime desiredExamTime { get; set; }
        public List<Entity.CandidateInformation> candidateInformations { get; set; }
    }
}
