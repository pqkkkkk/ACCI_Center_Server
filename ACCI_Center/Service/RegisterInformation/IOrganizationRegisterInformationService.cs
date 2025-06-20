using ACCI_Center.BusinessResult;
using ACCI_Center.Dto.Request;
using ACCI_Center.Dto.Response;

namespace ACCI_Center.Service.RegisterInformation
{
    public interface IOrganizationRegisterInformationService
    {
        bool IsValidOrganizationInformation(Entity.RegisterInformation registerInformation);
        bool IsValidTestInformation(int testId, string testName);
        bool IsValidDesiredExamTime(DateTime desiredExamTime, int testId);
        RegisterResult ValidateRegisterRequest(OrganizationRegisterRequest organizationRegisterRequest);
        OrganizationRegisterResponse RegisterForOrganization(OrganizationRegisterRequest organizationRegisterRequest);
        double CalculateTotalFee(int testId, string testName, int candidateCount);
    }
}
