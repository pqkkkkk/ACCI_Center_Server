using ACCI_Center.BusinessResult;
using ACCI_Center.Dto.Request;
using ACCI_Center.Dto.Response;

namespace ACCI_Center.Service.RegisterInformation
{
    public interface IRegisterInformationServiceV2
    {
        public IndividualRegisterResponse CreateRegisterInformationForIndividual(IndividualRegisterRequest request);
        public OrganizationRegisterResponse CreateRegisterInformationForOrganization(OrganizationRegisterRequestV2 request);
        public RegisterResult ValidateOrganizationRegisterInformation(int registerInformationId);
        public ApproveOrganizationRegisterResponse ApproveOrganizationRegisterResponse(int registerInformationId, ApproveOrganizationRegisterRequest request);
        public UpdateRegisterInformationResponse UpdateRegisterInformation(UpdateRegisterInformationRequest request);
    }
}
