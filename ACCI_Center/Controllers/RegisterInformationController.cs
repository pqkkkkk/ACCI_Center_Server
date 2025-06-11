using ACCI_Center.Dto;
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
        public ActionResult<string> RegisterForIndividual()
        {
            return null;
        }
        [HttpPost("organization")]
        public ActionResult<string> RegisterForOrganization()
        {
            return null;
        }
        [HttpGet]
        public ActionResult<PagedResult<Entity.RegisterInformation>> LoadRegisterInformations()
        {
            return null;
        }

    }
}
