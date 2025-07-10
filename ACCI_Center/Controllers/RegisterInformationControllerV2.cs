using ACCI_Center.BusinessResult;
using ACCI_Center.Dto.Request;
using ACCI_Center.Dto.Response;
using ACCI_Center.Entity;
using ACCI_Center.Helper;
using ACCI_Center.Service.RegisterInformation;
using ACCI_Center.Service.TTDangKy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;

namespace ACCI_Center.Controllers
{
    [Route("api/v2/RegisterInformation")]
    [ApiController]
    public class RegisterInformationControllerV2 : ControllerBase
    {
        private readonly IRegisterInformationServiceV2 registerInformationServiceV2;
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
        public RegisterInformationControllerV2(IRegisterInformationServiceV2 registerInformationServiceV2)
        {
            this.registerInformationServiceV2 = registerInformationServiceV2;
        }
        [HttpPut]
        public ActionResult<UpdateRegisterInformationResponse> UpdateRegisterInformation([FromBody] UpdateRegisterInformationRequest request)
        {
            var response = registerInformationServiceV2.UpdateRegisterInformation(request);
            if (response.statusCode != StatusCodes.Status200OK)
            {
                return StatusCode(response.statusCode, response.message);
            }
            return Ok(response);
        }
        [HttpPost("individual")]
        public ActionResult<IndividualRegisterResponse> RegisterForIndividual([FromBody] IndividualRegisterRequest request)
        {
            var response = registerInformationServiceV2.CreateRegisterInformationForIndividual(request);
            if (response.statusCode != StatusCodes.Status200OK)
            {
                return StatusCode(response.statusCode, response.registerResult.ToString());
            }
            return Ok(response);
        }
        [HttpPost("organization")]
        public ActionResult<OrganizationRegisterResponse> RegisterForOrganization([FromForm] OrganizationRegisterRequestV2 request)
        {
            IFormFile candidatesInformationFile = request.candidateInformationsFile;
            List<CandidateInformation> candidatesInformation = ExcelReaderHelper.ReadExcelFileFormIFormFile<CandidateInformation>(candidatesInformationFile, candidateInformationMapper);

            request.candidatesInformation = candidatesInformation;

            OrganizationRegisterResponse registerResponse = registerInformationServiceV2.CreateRegisterInformationForOrganization(request);

            if (registerResponse.registerResult == RegisterResult.UnknownError)
            {
                return StatusCode(registerResponse.statusCode, registerResponse);
            }

            return Ok(registerResponse);
        }
        [HttpPut("organization/approval/{registerInformationId}")]
        public ActionResult<ApproveOrganizationRegisterResponse> ApproveOrganizationRegister([FromRoute] int registerInformationId,
                                                                                             [FromBody] ApproveOrganizationRegisterRequest request)
        {
            var response = registerInformationServiceV2.ApproveOrganizationRegisterResponse(registerInformationId, request);
            if (response.statusCode != StatusCodes.Status200OK)
            {
                return StatusCode(response.statusCode, response.message);
            }
            return Ok(response);

        }
    }
}
