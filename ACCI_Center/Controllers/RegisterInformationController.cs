using ACCI_Center.BusinessResult;
using ACCI_Center.Dto;
using ACCI_Center.Dto.Request;
using ACCI_Center.Service.RegisterInformation;
using ACCI_Center.Service.TTDangKy;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ACCI_Center.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterInformationController : ControllerBase
    {
        private readonly IOrganizationRegisterInformationService organizationRegisterInformationService;
        private readonly IRegisterInformationService registerInformationService;

        public RegisterInformationController(IOrganizationRegisterInformationService organizationRegisterInformationService,
                                             IRegisterInformationService registerInformationService)
        {
            this.organizationRegisterInformationService = organizationRegisterInformationService;
            this.registerInformationService = registerInformationService;
        }
        [HttpPost("individual")]
        public ActionResult<string> RegisterForIndividual([FromBody] IndividualRegisterRequest request)
        {
            RegisterResult registerResult = registerInformationService.RegisterForIndividual(request);
            if (registerResult == RegisterResult.UnknownError)
            {
                return StatusCode(500, RegisterResult.UnknownError.ToString());
            }
            return Ok(registerResult.ToString());
        }
        [HttpPost("organization")]
        public ActionResult<RegisterResult> RegisterForOrganization([FromBody] OrganizationRegisterRequest request)
        {
            RegisterResult registerResult = organizationRegisterInformationService.RegisterForOrganization(request);

            if(registerResult == RegisterResult.UnknownError)
            {
                return StatusCode(500, RegisterResult.UnknownError.ToString());
            }

            return Ok(registerResult.ToString());
        }
        [HttpPut("CandidateInformation/ExamRegisterForm")]
        public ActionResult<string> ReleaseExamRegisterForms()
        {
            // Logic to release exam register forms for candidates
            // This is a placeholder implementation
            return Ok("Exam register forms released successfully.");
        }
        [HttpGet]
        public ActionResult<PagedResult<Entity.RegisterInformation>> LoadRegisterInformations()
        {
            return null;
        }

    }
}
