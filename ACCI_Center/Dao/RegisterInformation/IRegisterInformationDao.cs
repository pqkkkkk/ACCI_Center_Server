using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCI_Center.Dao.RegisterInformation
{
    public interface IRegisterInformationDao
    {
        public int AddRegisterInformation(Entity.RegisterInformation registerInformation);
        public int AddCandidateInformationsOfARegisterInformation(int maTTDangKy,
            List<Entity.CandidateInformation> candidateInformations);
        public int UpdateExamSchedule(int maTTDangKy, int maLichThi);
    }
}
