using System.Text.Json.Serialization;
using ACCI_Center.BusinessResult;

namespace ACCI_Center.Dto.Response
{
    public class ValidateExtensionRequestResponse
    {
        public bool isValid { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ExtensionResult result { get; set; }
        public int statusCode { get; set; }
    }
}
