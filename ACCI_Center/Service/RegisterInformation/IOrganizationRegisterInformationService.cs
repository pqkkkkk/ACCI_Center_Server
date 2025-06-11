using ACCI_Center.BusinessResult;
using ACCI_Center.Dto.Request;

namespace ACCI_Center.Service.RegisterInformation
{
    public interface IOrganizationRegisterInformationService
    {
        bool IsValidOrganizationInformation(Entity.RegisterInformation registerInformation);
        bool IsValidTestInformation(int testId, string testName);
        bool IsValidDesiredExamTime(DateTime desiredExamTime, int testId);
        TestRegisterResult ValidateRegisterRequest(OrganizationRegisterRequest organizationRegisterRequest);
        TestRegisterResult RegisterForOrganization(OrganizationRegisterRequest organizationRegisterRequest);
        double CalculateTotalFee(int testId, string testName, int candidateCount);
    }
}
