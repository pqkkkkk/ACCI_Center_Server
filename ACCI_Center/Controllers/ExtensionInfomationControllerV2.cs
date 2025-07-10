using ACCI_Center.Dto.Request;
using ACCI_Center.Dto.Response;
using ACCI_Center.Service.ExtensionInfomation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ACCI_Center.Controllers
{
    [Route("api/v2/ExtensionInformation")]
    [ApiController]
    public class ExtensionInfomationControllerV2 : ControllerBase
    {
        private readonly IExtensionInformationServiceV2 extensionInformationService;

        public ExtensionInfomationControllerV2(IExtensionInformationServiceV2 extensionInformationService)
        {
            this.extensionInformationService = extensionInformationService;
        }

        [HttpPost]
        public ActionResult<ExtensionResponse> CreateExtensionInformation([FromBody] ExtensionRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request data.");
            }
            var response = extensionInformationService.CreateExtensionInformation(request);
            if (response.statusCode != StatusCodes.Status200OK)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create extension information.");
            }
            return Ok(response);
        }

    }
}
