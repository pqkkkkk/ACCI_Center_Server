using ACCI_Center.Controllers.Binder;
using ACCI_Center.Entity;

namespace ACCI_Center.Dto.Request
{
    public class OrganizationRegisterRequest
    {
        [FromJson]
        public Entity.RegisterInformation registerInformation { get; set; }
        public string testName { get; set; }
        public int testId { get; set; }
        public DateTime desiredExamTime { get; set; }
        [FromJson]
        public List<Entity.CandidateInformation> candidatesInformation { get; set; }
        public IFormFile candidateInformationsFile { get; set; }
    }
}
