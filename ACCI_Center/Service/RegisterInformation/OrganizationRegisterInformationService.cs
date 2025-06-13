using ACCI_Center.BusinessResult;
using ACCI_Center.Dao.ExamSchedule;
using ACCI_Center.Dao.Invoice;
using ACCI_Center.Dao.RegisterInformation;
using ACCI_Center.Dto.Request;

namespace ACCI_Center.Service.RegisterInformation
{
    public class OrganizationRegisterInformationService : IOrganizationRegisterInformationService
    {
        private IRegisterInformationDao registerInformationDao;
        private IExamScheduleDao examScheduleDao;
        private IInvoiceDao invoiceDao;
        public OrganizationRegisterInformationService(IRegisterInformationDao ttDangKyDao, IExamScheduleDao lichThiDao,
                                          IInvoiceDao invoiceDao)
        {
            registerInformationDao = ttDangKyDao;
            examScheduleDao = lichThiDao;
            this.invoiceDao = invoiceDao;
        }
        public bool IsValidOrganizationInformation(Entity.RegisterInformation registerInformation)
        {
            if (registerInformation == null)
                return false;

            // Check required fields are not null or empty
            if (string.IsNullOrWhiteSpace(registerInformation.HoTen) ||
                string.IsNullOrWhiteSpace(registerInformation.SDT) ||
                string.IsNullOrWhiteSpace(registerInformation.Email) ||
                string.IsNullOrWhiteSpace(registerInformation.DiaChi))
            {
                return false;
            }

            // Validate email format
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(registerInformation.Email, emailPattern))
            {
                return false;
            }

            // Validate phone number: starts with 0, 10 digits
            var phonePattern = @"^0\d{9}$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(registerInformation.SDT, phonePattern))
            {
                return false;
            }

            return true;
        }
        public bool IsValidTestInformation(int testId, string testName)
        {
            Entity.Test test = examScheduleDao.GetTestById(testId);
            if (test == null || test.TenBaiThi != testName)
            {
                return false;
            }

            return true;
        }
        public bool IsValidDesiredExamTime(DateTime desiredExamTime, int testId)
        {
            List<int> emptyRoomIds = examScheduleDao.GetAllEmptyRoomIds(desiredExamTime, testId);
            if (emptyRoomIds.Count == 0)
            {
                return false;
            }

            List<int> freeEmployeeIds = examScheduleDao.GetAllFreeEmployeeIds(desiredExamTime, testId);
            if (freeEmployeeIds.Count == 0)
            {
                return false;
            }

            return true;
        }
        public RegisterResult ValidateRegisterRequest(OrganizationRegisterRequest organizationRegisterRequest)
        {
            if (!IsValidOrganizationInformation(organizationRegisterRequest.registerInformation))
            {
                return RegisterResult.InvalidOrganizationInformation;
            }
            if (!IsValidTestInformation(organizationRegisterRequest.testId, organizationRegisterRequest.testName))
            {
                return RegisterResult.InvalidTestInformation;
            }
            if (!IsValidDesiredExamTime(organizationRegisterRequest.desiredExamTime, organizationRegisterRequest.testId))
            {
                return RegisterResult.NoAvailableTimeSlot;
            }

            return RegisterResult.Success;

        }
        public RegisterResult RegisterForOrganization(OrganizationRegisterRequest organizationRegisterRequest)
        {
            try
            {
                RegisterResult testRegisterResult = ValidateRegisterRequest(organizationRegisterRequest);
                if (testRegisterResult != RegisterResult.Success)
                {
                    return testRegisterResult;
                }

                // Proceed with the registration process
                Entity.ExamSchedule examSchedule = new Entity.ExamSchedule
                {
                    BaiThi = organizationRegisterRequest.testId,
                    NgayThi = organizationRegisterRequest.desiredExamTime,
                    SoLuongThiSinhHienTai = organizationRegisterRequest.candidateInformations.Count,
                    DaNhapKetQuaThi = false,
                    DaThongBaoKetQuaThi = false,
                    PhongThi = examScheduleDao.GetAllEmptyRoomIds(organizationRegisterRequest.desiredExamTime, organizationRegisterRequest.testId).FirstOrDefault(),
                };
                int employeeId = examScheduleDao.GetAllFreeEmployeeIds(organizationRegisterRequest.desiredExamTime, organizationRegisterRequest.testId).FirstOrDefault();
                int examScheduleId = examScheduleDao.AddExamSchedule(examSchedule, employeeId);
                if (examScheduleId == -1)
                {
                    return RegisterResult.UnknownError;
                }

                int registerInformationId = registerInformationDao.AddRegisterInformation(organizationRegisterRequest.registerInformation);
                if (registerInformationId == -1)
                {
                    return RegisterResult.UnknownError;
                }

                if (registerInformationDao.AddCandidateInformationsOfARegisterInformation(registerInformationId, organizationRegisterRequest.candidateInformations) == -1)
                {
                    return RegisterResult.UnknownError;
                }

                Entity.Invoice invoice = new Entity.Invoice
                {
                    MaTTDangKy = registerInformationId,
                    TrangThai = "Chưa thanh toán",
                    TongTien = CalculateTotalFee(organizationRegisterRequest.testId, organizationRegisterRequest.testName, organizationRegisterRequest.candidateInformations.Count),
                    ThoiDiemTao = DateTime.Now,
                    ThoiDiemThanhToan = null,
                    LoaiHoaDon = "Đăng ký thi",
                };
                if (invoiceDao.AddInvoice(invoice) == -1)
                {
                    return RegisterResult.UnknownError;
                }


                return RegisterResult.Success;
            }
            catch (Exception ex)
            {
                return RegisterResult.UnknownError;
            }
        }

        public double CalculateTotalFee(int testId, string testName, int candidateCount)
        {
            double feePerCandidate = examScheduleDao.GetFeeOfTheTest(testId);
            double totalFee = feePerCandidate * candidateCount;

            if(candidateCount >= 20)
            {
                totalFee = totalFee * 0.9; // Apply 10% discount for 20 or more candidates
            }

            return totalFee;
        }
    }
}
