using ACCI_Center.Controllers.Binder;
using ACCI_Center.Entity;

namespace ACCI_Center.Dto.Request
{
    public class TestInformation
    {
        public string testName { get; set; }
        public int testId { get; set; }
        public DateTime desiredExamTime { get; set; }
    }
    public class OrganizationRegisterRequest
    {
        [FromJson]
        public Entity.RegisterInformation registerInformation { get; set; }
        public string testName { get; set; }
        public int testId { get; set; }
        public DateTime desiredExamTime { get; set; }
        [FromJson]
        public List<Entity.CandidateInformation> candidatesInformation { get; set; } = [];
        public IFormFile candidateInformationsFile { get; set; }
    }
    public class OrganizationRegisterRequestV2
    {
        [FromJson]
        public Entity.RegisterInformation registerInformation { get; set; }
        public TestInformation testInformation { get; set; }
        [FromJson]
        public List<Entity.CandidateInformation> candidatesInformation { get; set; } = [];
        public IFormFile candidateInformationsFile { get; set; }
    }
}
