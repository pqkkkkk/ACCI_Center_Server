using ACCI_Center.BusinessResult;
using ACCI_Center.Dto.Request;

namespace ACCI_Center.Service.RegisterInformation
{
    public interface IRegisterInformationValidation
    {
        public RegisterResult ValidateRegisterRequest(OrganizationRegisterRequest organizationRegisterRequest);
        public RegisterResult ValidateRegisterRequest(OrganizationRegisterRequestV2 organizationRegisterRequest);
    }
}
