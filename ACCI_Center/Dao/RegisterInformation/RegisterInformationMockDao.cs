using ACCI_Center.Dto;
using ACCI_Center.Entity;
using ACCI_Center.FilterField;

namespace ACCI_Center.Dao.RegisterInformation
{
    public class RegisterInformationMockDao : IRegisterInformationDao
    {
        public int AddCandidateInformationsOfARegisterInformation(int maTTDangKy, List<CandidateInformation> candidateInformations)
        {
            throw new NotImplementedException();
        }

        public int AddRegisterInformation(Entity.RegisterInformation registerInformation)
        {
            throw new NotImplementedException();
        }

        public List<CandidateInformation> LoadCandidatesInformation(int maTTDangKy)
        {
            throw new NotImplementedException();
        }

        public PagedResult<Entity.RegisterInformation> LoadRegisterInformation(int pageSize, int currentPageNumber, RegisterInformationFilterObject filterObject)
        {
            throw new NotImplementedException();
        }

        public Entity.RegisterInformation LoadRegisterInformationById(int maTTDangKy)
        {
            throw new NotImplementedException();
        }

        public void UpdateCandidateStatus(int mathisinh, bool DaGuiPhieuDuThi)
        {
            return;
        }

        public int UpdateExamSchedule(int maTTDangKy, int maLichThi)
        {
            throw new NotImplementedException();
        }

        public int UpdateRegisterInformation(Entity.RegisterInformation registerInformation)
        {
            throw new NotImplementedException();
        }
    }
}
