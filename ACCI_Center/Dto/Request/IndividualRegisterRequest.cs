using ACCI_Center.Entity;

namespace ACCI_Center.Dto.Request
{
    public class IndividualRegisterRequest
    {
        public RegisterInformation registerInformation { get; set; }
        public CandidateInformation candidateInformation { get; set; }
        public int SelectedExamScheduleId { get; set; }
    }
}
