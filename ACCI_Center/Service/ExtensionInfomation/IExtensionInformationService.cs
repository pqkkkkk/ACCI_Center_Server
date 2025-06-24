using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACCI_Center.BusinessResult;
using ACCI_Center.FilterField;

namespace ACCI_Center.Service.TTGiaHan
{
    public interface IExtensionInformationService
    {
        public ValidateExtendRequestResult ValidateExtensionRequest(int maTTDangKy, DateTime desiredExamDate);
        public int ExtendExamTimeFree(Entity.ExtensionInformation TTGiaHan);
        public int ExtendExamTimePaid(Entity.ExtensionInformation TTGiaHan, int MaLichThiMoi);
        public List<Entity.ExtensionInformation> LoadExtendInformation();
        public List<Entity.ExtensionInformation> LoadExtendInformation(int pageSize, int currentPageNumber,
            ExtensionInformationFilterObject extensionInformationFilterObject);
        public Entity.ExtensionInformation LoadExtendInformationById(int maTTGiaHan);
    }
}
