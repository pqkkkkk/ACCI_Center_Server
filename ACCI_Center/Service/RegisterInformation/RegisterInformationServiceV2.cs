using ACCI_Center.BusinessResult;
using ACCI_Center.Dao.ExamSchedule;
using ACCI_Center.Dao.RegisterInformation;
using System.Transactions;
using ACCI_Center.Dto.Request;
using ACCI_Center.Dto.Response;
using ACCI_Center.Entity;
using ACCI_Center.Dao.Invoice;

namespace ACCI_Center.Service.RegisterInformation
{
    public class RegisterInformationServiceV2 : IRegisterInformationServiceV2
    {
        private readonly IRegisterInformationDao registerInformationDao;
        private readonly IExamScheduleDao examScheduleDao;
        private readonly IExamScheduleDaoV2 examScheduleDaoV2;
        private readonly IRegisterInformationValidation registerInformationValidation;

        public RegisterInformationServiceV2(IRegisterInformationDao registerInformationDao, IExamScheduleDao examScheduleDao,
            IExamScheduleDaoV2 examScheduleDaoV2,
            IRegisterInformationValidation registerInformationValidation)
        {
            this.registerInformationDao = registerInformationDao;
            this.examScheduleDao = examScheduleDao;
            this.registerInformationValidation = registerInformationValidation;
            this.examScheduleDaoV2 = examScheduleDaoV2;
        }

        public ApproveOrganizationRegisterResponse ApproveOrganizationRegisterResponse(int registerInformationId, ApproveOrganizationRegisterRequest request)
        {
            try
            {
                // Load the register information by ID
                var registerInformaion = registerInformationDao.LoadRegisterInformationById(registerInformationId);

                // Load corresponding exam schedule
                var examSchedule = examScheduleDao.GetExamScheduleById(registerInformaion.MaLichThi ?? 0);

                // Update the register information status to "Đã duyệt"
                registerInformaion.TrangThaiDangKy = "Đã duyệt";
                registerInformationDao.UpdateRegisterInformation(registerInformaion);

                // Update the exam schedule status to "Đã duyệt"
                // Update the exam schedule's employee assignments
                // Update the exam schedule's room assignment
                examSchedule.PhongThi = request.roomId;
                examSchedule.TrangThaiDuyet = "Đã duyệt";
                examSchedule.LoaiLichThi = "Lịch thi đơn vị";
                examScheduleDaoV2.UpdateExamSchedule(examSchedule, request.supervisorIds);

                return new ApproveOrganizationRegisterResponse
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Organization registration approved successfully.",
                };


            }
            catch (Exception ex)
            {
                return new ApproveOrganizationRegisterResponse
                {
                    statusCode = StatusCodes.Status500InternalServerError,
                    message = "An error occurred while approving the organization registration."
                };
            }
        }

        public IndividualRegisterResponse CreateRegisterInformationForIndividual(IndividualRegisterRequest request)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    request.registerInformation.ThoiDiemDangKy = DateTime.Now;
                    request.registerInformation.TrangThaiThanhToan = "Chưa thanh toán";
                    request.registerInformation.TrangThaiDangKy = "Đã duyệt";
                    request.registerInformation.MaLichThi = request.SelectedExamScheduleId;
                    request.registerInformation.LoaiKhachHang = "Cá nhân";
                    int registerInformationId = registerInformationDao.AddRegisterInformation(request.registerInformation);
                    if (registerInformationId == -1)
                    {
                        throw new Exception("Failed to add register information for individual registration.");
                    }

                    // Add candidate information for the individual registration
                    int result = registerInformationDao
                                .AddCandidateInformationsOfARegisterInformation(registerInformationId,
                                new List<CandidateInformation> { request.candidateInformation });
                    if (result == -1)
                    {
                        throw new Exception("Failed to add candidate information for individual registration.");
                    }


                    // Update candidate quantity of corresponding exam schedule
                    bool updated = examScheduleDao.UpdateQuantityOfExamSchedule(request.SelectedExamScheduleId, 1);
                    if (!updated)
                    {
                        throw new Exception("Failed to update exam schedule quantity for individual registration.");
                    }

                    // Commit the transaction if all operations are successful
                    transaction.Complete();
                    return new IndividualRegisterResponse()
                    {
                        registerResult = RegisterResult.Success,
                        statusCode = StatusCodes.Status200OK
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during individual registration: {ex.Message}");
                return new IndividualRegisterResponse
                {
                    registerResult = RegisterResult.UnknownError,
                    statusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public OrganizationRegisterResponse CreateRegisterInformationForOrganization(OrganizationRegisterRequestV2 request)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    // Create the exam schedule
                    var test = examScheduleDao.GetTestById(request.testInformation.testId);
                    var examSchedule = new Entity.ExamSchedule
                    {
                        BaiThi = test.MaBaiThi,
                        NgayThi = request.testInformation.desiredExamTime,
                        LoaiLichThi = "Lịch thi đơn vị",
                        TrangThaiDuyet = "Chưa duyệt",
                        SoLuongThiSinhHienTai = request.candidatesInformation.Count,
                        ThoiDiemKetThuc = request.testInformation.desiredExamTime.AddMinutes(test.ThoiGianThi),
                    };
                    int examScheduleId = examScheduleDaoV2.AddExamSchedule(examSchedule, []);

                    // Add register information record
                    request.registerInformation.ThoiDiemDangKy = DateTime.Now;
                    request.registerInformation.LoaiKhachHang = "Đơn vị";
                    request.registerInformation.TrangThaiThanhToan = "Chưa thanh toán";
                    request.registerInformation.TrangThaiDangKy = "Chưa duyệt";
                    request.registerInformation.MaLichThi = examScheduleId;
                    int registerInformationId = registerInformationDao.AddRegisterInformation(request.registerInformation);
                    if (registerInformationId == -1)
                    {
                        return new OrganizationRegisterResponse
                        {
                            registerInformation = request.registerInformation,
                            candidatesInformation = request.candidatesInformation,
                            test = null,
                            examSchedule = null,
                            invoice = null,
                            registerResult = RegisterResult.UnknownError,
                            statusCode = 500
                        };
                    }
                    request.registerInformation.MaTTDangKy = registerInformationId;

                    if (registerInformationDao.AddCandidateInformationsOfARegisterInformation(registerInformationId, request.candidatesInformation) == -1)
                    {
                        return new OrganizationRegisterResponse
                        {
                            registerInformation = request.registerInformation,
                            candidatesInformation = request.candidatesInformation,
                            test = null,
                            examSchedule = null,
                            invoice = null,
                            registerResult = RegisterResult.UnknownError,
                            statusCode = 500
                        };
                    }

                    // Commit the transaction
                    transaction.Complete();
                    return new OrganizationRegisterResponse
                    {
                        registerInformation = request.registerInformation,
                        candidatesInformation = request.candidatesInformation,
                        test = test,
                        examSchedule = examSchedule,
                        registerResult = RegisterResult.Success,
                        statusCode = 200
                    };
                }
            }
            catch (Exception ex)
            {
                return new OrganizationRegisterResponse
                {
                    registerInformation = request.registerInformation,
                    candidatesInformation = request.candidatesInformation,
                    test = null,
                    registerResult = RegisterResult.UnknownError,
                    statusCode = 500
                };
            }
        }

        public UpdateRegisterInformationResponse UpdateRegisterInformation(UpdateRegisterInformationRequest request)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    int rowAffected = registerInformationDao.UpdateRegisterInformation(request.registerInformation);
                    if (rowAffected <= 0)
                    {
                        return new UpdateRegisterInformationResponse
                        {
                            registerInformation = request.registerInformation,
                            statusCode = StatusCodes.Status400BadRequest,
                            message = "No changes were made to the registration information."
                        };
                    }

                    transaction.Complete();

                    return new UpdateRegisterInformationResponse
                    {
                        registerInformation = request.registerInformation,
                        statusCode = StatusCodes.Status200OK,
                        message = "Registration information updated successfully."
                    };
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during updating register information: {ex.Message}");
                return new UpdateRegisterInformationResponse
                {
                    registerInformation = request.registerInformation,
                    statusCode = StatusCodes.Status500InternalServerError,
                    message = "An error occurred while updating the registration information."
                };
            }
        }

        public RegisterResult ValidateOrganizationRegisterInformation(int registerInformationId)
        {
            var registerInformation = registerInformationDao.LoadRegisterInformationById(registerInformationId);
            if (!IsValidOrganizationInformation(registerInformation))
            {
                return RegisterResult.InvalidOrganizationInformation;
            }

            var examSchedule = examScheduleDao.GetExamScheduleById(registerInformation.MaLichThi ?? 0);

            if (!IsValidCandidateQuantity(examSchedule.SoLuongThiSinhHienTai, examSchedule.BaiThi))
            {
                return RegisterResult.CandidateQuantityTooLow;
            }
            if (!IsValidDesiredExamTime(examSchedule.NgayThi, examSchedule.BaiThi))
            {
                return RegisterResult.NoAvailableTimeSlot;
            }

            return RegisterResult.Success;
        }
        public bool IsValidOrganizationInformation(Entity.RegisterInformation registerInformation)
        {
            if (registerInformation == null)
                return false;

            if(!registerInformation.LoaiKhachHang.Equals("Đơn vị"))
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
    }
}
