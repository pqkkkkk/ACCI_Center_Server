using System.Text.Json.Serialization;
using ACCI_Center.BusinessResult;

namespace ACCI_Center.Dto.Response
{
    public class ValidateRegisterInformationResponse
    {
        public int statusCode { get; set; } = StatusCodes.Status200OK;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RegisterResult message { get; set; } = RegisterResult.Success;
        public bool isValid { get; set; } = true;
    }
}
