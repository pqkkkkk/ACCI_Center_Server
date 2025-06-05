using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACCI_Center.BusinessResult;
using ACCI_Center.Dao.ExamSchedule;
using ACCI_Center.Dao.RegisterInformation;
using ACCI_Center.FilterField;

namespace ACCI_Center.Service.TTDangKy
{
    class RegisterInformationService : IRegisterInformationService
    {
        private IRegisterInformationDao TTDangKyDao;
        private IExamScheduleDao LichThiDao;
        public RegisterInformationService(IRegisterInformationDao ttDangKyDao, IExamScheduleDao lichThiDao)
        {
            TTDangKyDao = ttDangKyDao;
            LichThiDao = lichThiDao;
        }
        public TestRegisterResult ValidateRegisterRequest()
        {
            throw new NotImplementedException();
        }
        public TestRegisterResult RegisterForOrganization()
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

        public List<Entity.RegisterInformation> LoadRegisterInformation(Dictionary<RegisterInformationFilterField, object> filterFields)
        {
            throw new NotImplementedException();
        }

        public List<Entity.RegisterInformation> LoadRegisterInformation(int MaTTDangKy)
        {
            throw new NotImplementedException();
        }
    }
}
