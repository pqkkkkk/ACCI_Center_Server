using ACCI_Center.BusinessResult;
using ACCI_Center.Entity;

namespace ACCI_Center.Dto.Response
{
    public class OrganizationRegisterResponse
    {
        public RegisterInformation registerInformation;
        public List<CandidateInformation> candidatesInformation;
        public string testName;
        public int testId;
        public ExamSchedule examSchedule;
        public Invoice invoice;
        public RegisterResult registerResult;
        public int statusCode;
    }
}
