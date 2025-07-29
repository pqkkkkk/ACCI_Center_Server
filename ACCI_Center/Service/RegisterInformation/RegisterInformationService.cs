using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACCI_Center.BusinessResult;
using ACCI_Center.Dao.ExamSchedule;
using ACCI_Center.Dao.Invoice;
using ACCI_Center.Dao.RegisterInformation;
using ACCI_Center.Dto.Request;
using ACCI_Center.FilterField;
using ACCI_Center.Entity;
using ACCI_Center.Service.EmailService;
using ACCI_Center.Dto;
using ACCI_Center.Dto.Response;
using System.Transactions;

namespace ACCI_Center.Service.TTDangKy
{
    class RegisterInformationService : IRegisterInformationService
    {
        private IRegisterInformationDao registerInformationDao;
        private IExamScheduleDao examScheduleDao;
        private IInvoiceDao invoiceDao;
        private readonly IEmailService emailService;

        public RegisterInformationService(IRegisterInformationDao ttDangKyDao, IExamScheduleDao lichThiDao,
                                           IInvoiceDao invoiceDao, IEmailService emailService)
        {
            registerInformationDao = ttDangKyDao;
            examScheduleDao = lichThiDao;
            this.invoiceDao = invoiceDao;
            this.emailService = emailService;
        }
        public RegisterResult ValidateRegisterRequest(OrganizationRegisterRequest organizationRegisterRequest)
        {
            throw new NotImplementedException();
        }
        public IndividualRegisterResponse RegisterForIndividual(IndividualRegisterRequest individualRegisterRequest)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    individualRegisterRequest.registerInformation.ThoiDiemDangKy = DateTime.Now;
                    individualRegisterRequest.registerInformation.TrangThaiThanhToan = "Chưa thanh toán";
                    individualRegisterRequest.registerInformation.MaLichThi = individualRegisterRequest.SelectedExamScheduleId;
                    individualRegisterRequest.registerInformation.LoaiKhachHang = "Cá nhân";
                    int registerInformationId = registerInformationDao.AddRegisterInformation(individualRegisterRequest.registerInformation);
                    if (registerInformationId == -1)
                    {
                        throw new Exception("Failed to add register information for individual registration.");
                    }

                    // Add candidate information for the individual registration
                    int result = registerInformationDao
                                .AddCandidateInformationsOfARegisterInformation(registerInformationId,
                                new List<CandidateInformation> { individualRegisterRequest.candidateInformation });
                    if (result == -1)
                    {
                        throw new Exception("Failed to add candidate information for individual registration.");
                    }

                    // Add invoice for the individual registration
                    //int testId = examScheduleDao.GetTestIdByExamScheduleId(individualRegisterRequest.SelectedExamScheduleId);
                    //Invoice invoice = new Invoice
                    //{
                    //    MaTTDangKy = registerInformationId,
                    //    TrangThai = "Chưa thanh toán",
                    //    TongTien = examScheduleDao.GetFeeOfTheTest(testId),
                    //    ThoiDiemTao = DateTime.Now,
                    //    ThoiDiemThanhToan = null,
                    //    LoaiHoaDon = "Đăng ký thi",
                    //    MaTTGiaHan = 0,
                    //};
                    //int invoiceId = invoiceDao.AddInvoice(invoice);
                    //if (invoiceId == -1)
                    //{
                    //    throw new Exception("Failed to add invoice for individual registration.");
                    //}

                    // Update candidate quantity of corresponding exam schedule
                    bool updated = examScheduleDao.UpdateQuantityOfExamSchedule(individualRegisterRequest.SelectedExamScheduleId, 1);
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

        public List<Entity.RegisterInformation> LoadRegisterInformation()
        {
            throw new NotImplementedException();
        }

        public List<Entity.RegisterInformation> LoadRegisterInformation(RegisterInformationFilterObject registerInformationFilterObject)
        {
            throw new NotImplementedException();
        }

        public RegisterInformationByIdResponse LoadRegisterInformationById(int MaTTDangKy, string? parts)
        {
            try
            {
                Entity.RegisterInformation? registerInformation = registerInformationDao.LoadRegisterInformationById(MaTTDangKy);
                List<Entity.CandidateInformation>? candidatesInformation = null;
                Entity.ExamSchedule? examSchedule = null;
                Entity.Test? test = null;

                if (registerInformation == null)
                {
                    return new RegisterInformationByIdResponse
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Register information not found."
                    };
                }
                if (parts == null)
                {
                    return new RegisterInformationByIdResponse
                    {
                        RegisterInformation = registerInformation,
                        statusCode = StatusCodes.Status200OK,
                        message = "Success"
                    };
                }
                if (parts.Contains("candidateInformation"))
                {
                    List<Entity.CandidateInformation> candidatesInformationResult = registerInformationDao.LoadCandidatesInformation(MaTTDangKy);
                    if (candidatesInformationResult == null || candidatesInformationResult.Count == 0)
                    {
                        return new RegisterInformationByIdResponse
                        {
                            statusCode = StatusCodes.Status404NotFound,
                            message = "No candidate information found for this register information."
                        };
                    }
                    candidatesInformation = candidatesInformationResult;
                }
                if (parts.Contains("examSchedule"))
                {
                    examSchedule = examScheduleDao.GetExamScheduleById(registerInformation.MaLichThi ?? 0);
                    if (examSchedule == null)
                    {
                        return new RegisterInformationByIdResponse
                        {
                            statusCode = StatusCodes.Status404NotFound,
                            message = "Exam schedule not found for this register information."
                        };
                    }
                }
                if (parts.Contains("test"))
                {
                    test = examScheduleDao.GetTestById(examSchedule?.BaiThi ?? 0);
                    if (test == null)
                    {
                        return new RegisterInformationByIdResponse
                        {
                            statusCode = StatusCodes.Status404NotFound,
                            message = "Test not found for this register information."
                        };
                    }
                }

                return new RegisterInformationByIdResponse
                {
                    RegisterInformation = registerInformation,
                    examSchedule = examSchedule,
                    test = test,
                    candidateInformations = candidatesInformation,
                    statusCode = StatusCodes.Status200OK,
                    message = "Success"
                };


            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new RegisterInformationByIdResponse
                {
                    statusCode = StatusCodes.Status500InternalServerError,
                    message = "An error occurred while loading register information."
                };
            }
        }

        public PagedResult<Entity.RegisterInformation> LoadRegisterInformation(int pageSize, int currentPageNumber, RegisterInformationFilterObject registerInformationFilterObject)
        {
            try
            {
                var result = registerInformationDao.LoadRegisterInformation(pageSize, currentPageNumber, registerInformationFilterObject);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new PagedResult<Entity.RegisterInformation>(null, 0, 0, 0);
            }
        }

        public int ReleaseExamRegisterForm()
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    List<Entity.ExamSchedule> examSchedules = examScheduleDao.GetExamSchedules(pageSize: 1000, currentPageNumber: 1,
                        filterObject: new ExamScheduleFilterObject
                        {
                            NgayThiBatDau = DateTime.Now,
                            NgayThiKetThuc = DateTime.Now.AddDays(7),
                            DaPhatHanhPhieuDuThi = false,
                            LoaiLichThi = "Tất cả",
                        }).items.ToList();

                    int count = 0;


                    foreach (var schedule in examSchedules)
                    {
                        Entity.Test test = examScheduleDao.GetTestById(schedule.BaiThi) ?? throw new Exception("Test not found for the exam schedule.");

                        List<Entity.CandidateInformation> candidates = examScheduleDao.GetCandidatesByExamScheduleId(schedule.MaLichThi);

                        foreach (var candidate in candidates)
                        {
                            ExamRegisterFormViewModel viewModel = new ExamRegisterFormViewModel
                            {
                                candidateInformation = candidate,
                                examSchedule = schedule,
                                test = test
                            };
                            if (candidate.DaGuiPhieuDuThi == false)
                            {
                                bool sent = emailService.SendEmail(viewModel);
                                if (sent)
                                {
                                    candidate.DaGuiPhieuDuThi = true;
                                    registerInformationDao.UpdateCandidateStatus(candidate.MaTTThiSinh.GetValueOrDefault(), candidate.DaGuiPhieuDuThi);
                                    count++;
                                }
                            }
                        }

                    }

                    transaction.Complete();
                    return count;
                }
            }
            catch (Exception ex)
            {
                return 0;
            } 
        }
    }
}
