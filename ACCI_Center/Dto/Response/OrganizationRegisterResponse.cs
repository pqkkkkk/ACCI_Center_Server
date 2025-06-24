using System.Text.Json.Serialization;
using ACCI_Center.BusinessResult;
using ACCI_Center.Entity;

namespace ACCI_Center.Dto.Response
{
    public class OrganizationRegisterResponse
    {
        public RegisterInformation registerInformation { get; set; }
        public List<CandidateInformation> candidatesInformation { get; set; } = new List<CandidateInformation>();
        public Test? test { get; set; }
        public ExamSchedule? examSchedule { get; set; }
        public Invoice? invoice { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RegisterResult registerResult { get; set; } = RegisterResult.UnknownError;
        public int statusCode { get; set; } = 500;
    }
}
