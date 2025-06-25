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
    [Route("api/[controller]")]
    [ApiController]
    public class ExtensionInformationController : ControllerBase
    {
        private IExtensionInformationService _extensionInformationService;
        public ExtensionInformationController(IExtensionInformationService extensionInformationService)
        {
            _extensionInformationService = extensionInformationService;
        }
        [HttpPost("free")]
        public ActionResult<ExtensionResponse> FreeExtension([FromBody] ExtensionRequest request)
        {
            // Logic for free extension
            return Ok("Free extension applied successfully.");
        }
        [HttpPost("paid")]
        public ActionResult<ExtensionResponse> PaidExtension([FromBody] ExtensionRequest request)
        {

            return Ok("Paid extension applied successfully.");

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
