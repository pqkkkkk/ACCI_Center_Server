using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACCI_Center.BusinessResult;
using ACCI_Center.FilterField;

namespace ACCI_Center.Service.TTDangKy
{
    public interface IRegisterInformationService
    {
        public TestRegisterResult RegisterForIndividual();
        public TestRegisterResult ValidateRegisterRequest();
        public TestRegisterResult RegisterForOrganization();
        public List<Entity.RegisterInformation> LoadRegisterInformation();
        public List<Entity.RegisterInformation> LoadRegisterInformation(int MaTTDangKy);
        public List<Entity.RegisterInformation> LoadRegisterInformation(Dictionary<RegisterInformationFilterField, object> filterFields);
    }
}
