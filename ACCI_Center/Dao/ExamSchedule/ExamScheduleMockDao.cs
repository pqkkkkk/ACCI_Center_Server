using ACCI_Center.Dto;
using ACCI_Center.Entity;
using ACCI_Center.FilterField;

namespace ACCI_Center.Dao.ExamSchedule
{
    public class ExamScheduleMockDao : IExamScheduleDao
    {
        public int AddExamSchedule(Entity.ExamSchedule examSchedule, int employeeId)
        {
            throw new NotImplementedException();
        }

        public List<int> GetAllEmptyRoomIds(DateTime desiredExamTime, int testId)
        {
            throw new NotImplementedException();
        }

        public List<int> GetAllFreeEmployeeIds(DateTime desiredExamTime, int testId)
        {
            throw new NotImplementedException();
        }

        public List<CandidateInformation> GetCandidatesByExamScheduleId(int id)
        {
            List<CandidateInformation> res = new List<CandidateInformation>()
            {
                new CandidateInformation()
                {
                    MaTTThiSinh = 1,
                    HoTen = "Nguyen Van A",
                    SDT = "0123456789",
                    Email = "crkeo169@gmail.com",
                    DaNhanChungChi = false,
                    DaGuiPhieuDuThi = false,
                    MaTTDangKy = 1,
                    
                },
            };

            return res;
        }

        public Entity.ExamSchedule GetExamScheduleById(int examScheduleId)
        {
            throw new NotImplementedException();
        }

        public PagedResult<Entity.ExamSchedule> GetExamSchedules(int pageSize, int currentPageNumber, ExamScheduleFilterObject filterObject)
        {
            throw new NotImplementedException();
        }

        public List<Entity.ExamSchedule> GetExamSchedulesForNext2Week()
        {
            var res = new List<Entity.ExamSchedule>()
            {
                new Entity.ExamSchedule() {
                    MaLichThi = 1,
                    NgayThi = DateTime.Now.AddDays(1),
                    ThoiDiemKetThuc = DateTime.Now.AddDays(1).AddHours(2),
                    BaiThi = 1,
                    SoLuongThiSinhHienTai = 30,
                    DaNhapKetQuaThi = false,
                    DaThongBaoKetQuaThi = false,
                    PhongThi = 1
                },
            };

            return res;
        }

        public double GetFeeOfTheTest(int testId)
        {
            throw new NotImplementedException();
        }

        public Test GetTestById(int testId)
        {
            throw new NotImplementedException();
        }

        public int GetTestIdByExamScheduleId(int examScheduleId)
        {
            throw new NotImplementedException();
        }

        public PagedResult<Test> GetTests(int pageSize, int currentPageNumber, TestFilterObject testFilterObject)
        {
            throw new NotImplementedException();
        }

        public bool UpdateQuantityOfExamSchedule(int examScheduleId, int quantity)
        {
            throw new NotImplementedException();
        }
    }
}
