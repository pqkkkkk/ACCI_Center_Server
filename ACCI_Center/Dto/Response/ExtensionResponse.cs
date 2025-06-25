using System.Text.Json.Serialization;
using ACCI_Center.BusinessResult;

namespace ACCI_Center.Dto.Response
{
    public class ExtensionResponse
    {
        public Entity.ExamSchedule newExamSchedule { get; set; }
        public Entity.ExtensionInformation extensionInformation { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ExtensionResult extensionResult { get; set; }
        public int statusCode { get; set; }
    }
}
