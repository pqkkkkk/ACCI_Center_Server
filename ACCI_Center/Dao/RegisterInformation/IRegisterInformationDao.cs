using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACCI_Center.Dto;
using ACCI_Center.Entity;
using ACCI_Center.FilterField;

namespace ACCI_Center.Dao.RegisterInformation
{
    public interface IRegisterInformationDao
    {
        public int AddRegisterInformation(Entity.RegisterInformation registerInformation);
        public int AddCandidateInformationsOfARegisterInformation(int maTTDangKy,
                            List<Entity.CandidateInformation> candidateInformations);
        public int UpdateExamSchedule(int maTTDangKy, int maLichThi);
        public void UpdateCandidateStatus(int mathisinh, bool DaGuiPhieuDuThi);
        public PagedResult<Entity.RegisterInformation> LoadRegisterInformation(int pageSize, int currentPageNumber, RegisterInformationFilterObject filterObject);
        public Entity.RegisterInformation? LoadRegisterInformationById(int maTTDangKy);
        public List<Entity.CandidateInformation> LoadCandidatesInformation(int maTTDangKy);
    }
}
