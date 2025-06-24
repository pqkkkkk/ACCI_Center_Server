using ACCI_Center.BusinessResult;
using ACCI_Center.Dto;
using ACCI_Center.Dto.Request;
using ACCI_Center.Dto.Response;
using ACCI_Center.Entity;
using ACCI_Center.Helper;
using ACCI_Center.Service.RegisterInformation;
using ACCI_Center.Service.TTDangKy;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ACCI_Center.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterInformationController : ControllerBase
    {
        private readonly IOrganizationRegisterInformationService organizationRegisterInformationService;
        private readonly IRegisterInformationService registerInformationService;

        Func<IRow, CandidateInformation> candidateInformationMapper = (row) =>
        {
            if (row == null) return null;
            return new Entity.CandidateInformation
            {
                MaTTThiSinh = 0,
                MaTTDangKy = 0,
                HoTen = row.GetCell(0)?.ToString() ?? string.Empty,
                SDT = row.GetCell(1)?.ToString() ?? string.Empty,
                Email = row.GetCell(2)?.ToString() ?? string.Empty,
                DaGuiPhieuDuThi = false,
                DaNhanChungChi = false
            };
        };
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
        public ActionResult<OrganizationRegisterResponse> RegisterForOrganization([FromForm] OrganizationRegisterRequest request)
        {
            IFormFile candidatesInformationFile = request.candidateInformationsFile;
            List<CandidateInformation> candidatesInformation = ExcelReaderHelper.ReadExcelFileFormIFormFile<CandidateInformation>(candidatesInformationFile, candidateInformationMapper);

            request.candidatesInformation = candidatesInformation;

            OrganizationRegisterResponse registerResponse = organizationRegisterInformationService.RegisterForOrganization(request);

            if(registerResponse.registerResult == RegisterResult.UnknownError)
            {
                return StatusCode(registerResponse.statusCode, registerResponse);
            }

            return Ok(registerResponse);
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
