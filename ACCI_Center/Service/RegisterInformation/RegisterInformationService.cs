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
using ACCI_Center.Dto.Reponse;
using ACCI_Center.Entity;
using ACCI_Center.Service.EmailService;
using ACCI_Center.Dto;
using ACCI_Center.Dto.Response;

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
                individualRegisterRequest.registerInformation.ThoiDiemDangKy = DateTime.Now;
                individualRegisterRequest.registerInformation.TrangThai = "Chưa thanh toán";
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
                            new List<CandidateInformation> {individualRegisterRequest.candidateInformation });
                if (result == -1)
                {
                    throw new Exception("Failed to add candidate information for individual registration.");
                }

                // Add invoice for the individual registration
                int testId = examScheduleDao.GetTestIdByExamScheduleId(individualRegisterRequest.SelectedExamScheduleId);
                Invoice invoice = new Invoice
                {
                    MaTTDangKy = registerInformationId,
                    TrangThai = "Chưa thanh toán",
                    TongTien = examScheduleDao.GetFeeOfTheTest(testId),
                    ThoiDiemTao = DateTime.Now,
                    ThoiDiemThanhToan = null,
                    LoaiHoaDon = "Đăng ký thi",
                    MaTTGiaHan = 0,
                };
                int invoiceId = invoiceDao.AddInvoice(invoice);
                if (invoiceId == -1)
                {
                    throw new Exception("Failed to add invoice for individual registration.");
                }
                bool updated = examScheduleDao.UpdateQuantityOfExamSchedule(individualRegisterRequest.SelectedExamScheduleId, 1);
                if (!updated)
                {
                    throw new Exception("Failed to update exam schedule quantity for individual registration.");
                }

                return new IndividualRegisterResponse()
                {
                    registerResult = RegisterResult.Success,
                    statusCode = StatusCodes.Status200OK
                };
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

       public List<Entity.RegisterInformation> LoadRegisterInformation(int MaTTDangKy)
       {
           throw new NotImplementedException();
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
           List<Entity.ExamSchedule> examSchedules = examScheduleDao.GetExamSchedulesForNext2Week();
           int count = 0;


           foreach (var schedule in examSchedules)
           {
               List<Entity.CandidateInformation> candidates = examScheduleDao.GetCandidatesByExamScheduleId(schedule.MaLichThi);
               foreach (var candidate in candidates)
               {
                   if (candidate.DaGuiPhieuDuThi == false)
                   {
                       bool sent = emailService.SendEmail(candidate);
                       if (sent)
                       {
                           candidate.DaGuiPhieuDuThi = true;
                           registerInformationDao.UpdateCandidateStatus(candidate.MaTTThiSinh, candidate.DaGuiPhieuDuThi);
                           count++;
                       }
                   }
               }
           }


           return count;
       }


   }
}
