using System.Transactions;
using ACCI_Center.BusinessResult;
using ACCI_Center.Dao.ExamSchedule;
using ACCI_Center.Dao.Invoice;
using ACCI_Center.Dao.RegisterInformation;
using ACCI_Center.Dto.Request;
using ACCI_Center.Dto.Response;

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
        public bool IsValidCandidateQuantity(int candidateCount, int tesetId)
        {
            Entity.Test test = examScheduleDao.GetTestById(tesetId);
            int minimumCandidateCount = test.SoLuongThiSinhToiThieu;
            int maximumCandidateCount = test.SoLuongThiSinhToiDa;

            if (candidateCount < minimumCandidateCount || candidateCount > maximumCandidateCount)
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
            if(!IsValidCandidateQuantity(organizationRegisterRequest.candidatesInformation.Count, organizationRegisterRequest.testId))
            {
                return RegisterResult.CandidateQuantityTooLow;
            }
            if (!IsValidDesiredExamTime(organizationRegisterRequest.desiredExamTime, organizationRegisterRequest.testId))
            {
                return RegisterResult.NoAvailableTimeSlot;
            }

            return RegisterResult.Success;

        }
        public OrganizationRegisterResponse RegisterForOrganization(OrganizationRegisterRequest organizationRegisterRequest)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    // Validate the registration request
                    RegisterResult testRegisterResult = ValidateRegisterRequest(organizationRegisterRequest);
                    if (testRegisterResult != RegisterResult.Success)
                    {
                        return new OrganizationRegisterResponse
                        {
                            registerInformation = organizationRegisterRequest.registerInformation,
                            candidatesInformation = organizationRegisterRequest.candidatesInformation,
                            test = null,
                            examSchedule = null,
                            invoice = null,
                            registerResult = testRegisterResult,
                            statusCode = 400
                        };
                    }

                    // Add exam schedule record
                    Entity.ExamSchedule examSchedule = new Entity.ExamSchedule
                    {
                        BaiThi = organizationRegisterRequest.testId,
                        NgayThi = organizationRegisterRequest.desiredExamTime,
                        SoLuongThiSinhHienTai = organizationRegisterRequest.candidatesInformation.Count,
                        DaNhapKetQuaThi = false,
                        DaThongBaoKetQuaThi = false,
                        PhongThi = examScheduleDao.GetAllEmptyRoomIds(organizationRegisterRequest.desiredExamTime, organizationRegisterRequest.testId).FirstOrDefault(),
                    };
                    int employeeId = examScheduleDao.GetAllFreeEmployeeIds(organizationRegisterRequest.desiredExamTime, organizationRegisterRequest.testId).FirstOrDefault();
                    int examScheduleId = examScheduleDao.AddExamSchedule(examSchedule, employeeId);
                    if (examScheduleId == -1)
                    {
                        return new OrganizationRegisterResponse
                        {
                            registerInformation = organizationRegisterRequest.registerInformation,
                            candidatesInformation = organizationRegisterRequest.candidatesInformation,
                            test = null,
                            examSchedule = null,
                            invoice = null,
                            registerResult = RegisterResult.UnknownError,
                            statusCode = 500
                        };
                    }

                    // Add register information record
                    organizationRegisterRequest.registerInformation.MaLichThi = examScheduleId;
                    organizationRegisterRequest.registerInformation.ThoiDiemDangKy = DateTime.Now;
                    organizationRegisterRequest.registerInformation.LoaiKhachHang = "Đơn vị";
                    organizationRegisterRequest.registerInformation.TrangThai = "Chưa thanh toán";
                    int registerInformationId = registerInformationDao.AddRegisterInformation(organizationRegisterRequest.registerInformation);
                    if (registerInformationId == -1)
                    {
                        return new OrganizationRegisterResponse
                        {
                            registerInformation = organizationRegisterRequest.registerInformation,
                            candidatesInformation = organizationRegisterRequest.candidatesInformation,
                            test = null,
                            examSchedule = null,
                            invoice = null,
                            registerResult = RegisterResult.UnknownError,
                            statusCode = 500
                        };
                    }
                    organizationRegisterRequest.registerInformation.MaTTDangKy = registerInformationId;

                    if (registerInformationDao.AddCandidateInformationsOfARegisterInformation(registerInformationId, organizationRegisterRequest.candidatesInformation) == -1)
                    {
                        return new OrganizationRegisterResponse {
                            registerInformation = organizationRegisterRequest.registerInformation,
                            candidatesInformation = organizationRegisterRequest.candidatesInformation,
                            test = null,
                            examSchedule = null, invoice = null,
                            registerResult = RegisterResult.UnknownError,
                            statusCode = 500 };
                    }

                    // Add invoice record
                    Entity.Invoice invoice = new Entity.Invoice
                    {
                        MaTTDangKy = registerInformationId,
                        TrangThai = "Chưa thanh toán",
                        TongTien = CalculateTotalFee(organizationRegisterRequest.testId, organizationRegisterRequest.testName, organizationRegisterRequest.candidatesInformation.Count),
                        ThoiDiemTao = DateTime.Now,
                        ThoiDiemThanhToan = null,
                        LoaiHoaDon = "Đăng ký thi",
                        MaTTGiaHan = -1
                    };
                    if (invoiceDao.AddInvoice(invoice) == -1)
                    {
                        return new OrganizationRegisterResponse
                        {
                            registerInformation = organizationRegisterRequest.registerInformation,
                            candidatesInformation = organizationRegisterRequest.candidatesInformation,
                            test = null,
                            examSchedule = null,
                            invoice = null,
                            registerResult = RegisterResult.UnknownError,
                            statusCode = 500
                        };
                    }

                    var test = examScheduleDao.GetTestById(organizationRegisterRequest.testId);
                    // Commit the transaction
                    transaction.Complete();
                    return new OrganizationRegisterResponse
                    {
                        registerInformation = organizationRegisterRequest.registerInformation,
                        candidatesInformation = organizationRegisterRequest.candidatesInformation,
                        test = test,
                        examSchedule = examSchedule,
                        invoice = invoice,
                        registerResult = RegisterResult.Success,
                        statusCode = 200
                    };
                }
            }
            catch (Exception ex)
            {
                return new OrganizationRegisterResponse
                {
                    registerInformation = organizationRegisterRequest.registerInformation,
                    candidatesInformation = organizationRegisterRequest.candidatesInformation,
                    test = null,
                    examSchedule = null,
                    invoice = null,
                    registerResult = RegisterResult.UnknownError,
                    statusCode = 500
                };
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
