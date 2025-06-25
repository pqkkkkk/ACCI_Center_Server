using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACCI_Center.BusinessResult;
using ACCI_Center.Dto.Request;
using ACCI_Center.FilterField;
using ACCI_Center.Dto.Reponse;
using ACCI_Center.Dto;
using ACCI_Center.Dto.Response;

namespace ACCI_Center.Service.TTDangKy
{
    public interface IRegisterInformationService
    {
        public int ReleaseExamRegisterForm();
        public IndividualRegisterResponse RegisterForIndividual(IndividualRegisterRequest individualRegisterRequest);
        public RegisterResult ValidateRegisterRequest(OrganizationRegisterRequest organizationRegisterRequest);
        public List<Entity.RegisterInformation> LoadRegisterInformation();
        public List<Entity.RegisterInformation> LoadRegisterInformation(int MaTTDangKy);
        public PagedResult<Entity.RegisterInformation> LoadRegisterInformation(int pageSize, int currentPageNumber,
                                                RegisterInformationFilterObject registerInformationFilterObject);
    }
}
