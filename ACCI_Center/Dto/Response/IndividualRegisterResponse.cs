using ACCI_Center.BusinessResult;
using ACCI_Center.Entity;
using System.Text.Json.Serialization;

namespace ACCI_Center.Dto.Response
{
    public class IndividualRegisterResponse
    {
        public RegisterInformation registerInformation { get; set; }
        public CandidateInformation candidateInformation { get; set; } 
        public Test? test { get; set; }
        public ExamSchedule? examSchedule { get; set; }
        public Invoice? invoice { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RegisterResult registerResult { get; set; } = RegisterResult.UnknownError;
        public int statusCode { get; set; } = 500;
    }
}
