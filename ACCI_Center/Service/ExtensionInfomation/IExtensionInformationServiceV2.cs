using ACCI_Center.BusinessResult;
using ACCI_Center.Dto.Request;
using ACCI_Center.Dto.Response;

namespace ACCI_Center.Service.ExtensionInfomation
{
    public interface IExtensionInformationServiceV2
    {
        public ExtensionResult ValidateExtensionRequest(int maTTDangKy, int? newExamScheduleId);
        public ExtensionResponse CreateExtensionInformation(ExtensionRequest request);
    }
}
