using ACCI_Center.Dto;
using ACCI_Center.Dto.Request;
using ACCI_Center.Dto.Response;
using ACCI_Center.Entity;
using ACCI_Center.FilterField;
using ACCI_Center.Service;
using ACCI_Center.Service.TTGiaHan;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ACCI_Center.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ExtensionInformationController : ControllerBase
    {
        private IExtensionInformationService _extensionInformationService;
        public ExtensionInformationController(IExtensionInformationService extensionInformationService)
        {
            _extensionInformationService = extensionInformationService;
        }
        [HttpPost("free")]
        public ActionResult<ExtensionResponse> HandleFreeExtension([FromBody] ExtensionRequest request)
        {
            var response = _extensionInformationService.ExtendExamTimeFree(request);

            if(response.statusCode != StatusCodes.Status200OK)
            {
                return StatusCode(response.statusCode, response);
            }

            return Ok(response);
        }
        [HttpPost("paid")]
        public ActionResult<ExtensionResponse> HandlePaidExtension([FromBody] ExtensionRequest request)
        {
            var response = _extensionInformationService.ExtendExamTimePaid(request);

            if (response.statusCode != StatusCodes.Status200OK)
            {
                return StatusCode(response.statusCode, response);
            }

            return Ok(response);

        }
        [HttpGet("validate")]
        public ActionResult<ValidateExtensionRequestResponse> ValidateExtensionRequest(
                        [FromQuery] int registerInformationId)
        {
            var response = _extensionInformationService.ValidateExtensionRequest(registerInformationId);

            if(response.statusCode != StatusCodes.Status200OK)
            {
                return StatusCode(response.statusCode, response);
            }

            return Ok(response);
        }
        [HttpGet]
        public ActionResult<PagedResult<Entity.ExtensionInformation>> GetPagedExtensions(
            [FromQuery] int pageSize,
            [FromQuery] int currentPageNumber,
            [FromQuery] ExtensionInformationFilterObject filterObject
            )
        {

            var result = _extensionInformationService.LoadExtendInformation(pageSize, currentPageNumber, filterObject);

            if(result.items == null)
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving extension information.");
            if (result.items.Count() == 0)
                return NotFound("No extension information found matching the provided criteria.");

            return Ok(result);
        }

    }
}
