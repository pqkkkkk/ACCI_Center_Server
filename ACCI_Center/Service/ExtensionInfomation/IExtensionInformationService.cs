using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACCI_Center.BusinessResult;
using ACCI_Center.Dto;
using ACCI_Center.Dto.Request;
using ACCI_Center.Dto.Response;
using ACCI_Center.FilterField;

namespace ACCI_Center.Service.TTGiaHan
{
    public interface IExtensionInformationService
    {
        public ExtensionResult ValidateExtensionRequest(int maTTDangKy, int newExamScheduleId);
        public ExtensionResponse ExtendExamTimeFree(ExtensionRequest request);
        public ExtensionResponse ExtendExamTimePaid(ExtensionRequest request);
        public PagedResult<Entity.ExtensionInformation> LoadExtendInformation(int pageSize, int currentPageNumber,
            ExtensionInformationFilterObject extensionInformationFilterObject);
        public Entity.ExtensionInformation LoadExtendInformationById(int maTTGiaHan);
    }
}
