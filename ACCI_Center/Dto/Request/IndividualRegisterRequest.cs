using ACCI_Center.Entity;

namespace ACCI_Center.Dto.Request
{
    public class IndividualRegisterRequest
    {
        public RegisterInformation registerInformation { get; set; }
        public List<CandidateInformation> candidateInformation { get; set; }
        public List<int> SelectedExamScheduleId { get; set; }
    }
}
