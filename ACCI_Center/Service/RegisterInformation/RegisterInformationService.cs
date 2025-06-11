using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACCI_Center.BusinessResult;
using ACCI_Center.Dao.ExamSchedule;
using ACCI_Center.Dao.Invoice;
using ACCI_Center.Dao.RegisterInformation;
using ACCI_Center.Dto.Request;
using ACCI_Center.FilterField;

namespace ACCI_Center.Service.TTDangKy
{
    class RegisterInformationService : IRegisterInformationService
    {
        private IRegisterInformationDao registerInformationDao;
        private IExamScheduleDao examScheduleDao;
        private IInvoiceDao invoiceDao;
        public RegisterInformationService(IRegisterInformationDao ttDangKyDao, IExamScheduleDao lichThiDao,
                                          IInvoiceDao invoiceDao)
        {
            registerInformationDao = ttDangKyDao;
            examScheduleDao = lichThiDao;
            this.invoiceDao = invoiceDao;
        }
        public TestRegisterResult ValidateRegisterRequest(OrganizationRegisterRequest organizationRegisterRequest)
        {
            throw new NotImplementedException();
        }
        public TestRegisterResult RegisterForIndividual()
        {
            throw new NotImplementedException();
        }

        public List<Entity.RegisterInformation> LoadRegisterInformation()
        {
            throw new NotImplementedException();
        }

        public List<Entity.RegisterInformation> LoadRegisterInformation(RegisterInformationFilterObject registerInformationFilterObject)
        {
            throw new NotImplementedException();
        }

        public List<Entity.RegisterInformation> LoadRegisterInformation(int MaTTDangKy)
        {
            throw new NotImplementedException();
        }

        public List<Entity.RegisterInformation> LoadRegisterInformation(int pageSize, int currentPageNumber, RegisterInformationFilterObject registerInformationFilterObject)
        {
            throw new NotImplementedException();
        }
    }
}
