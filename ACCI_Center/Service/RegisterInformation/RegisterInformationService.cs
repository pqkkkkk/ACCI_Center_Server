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
using ACCI_Center.Entity;;
using ACCI_Center.Service.EmailService;

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
        public RegisterResult RegisterForIndividual(IndividualRegisterRequest individualRegisterRequest)
        {
            try
            {
                for(int i=0; i < individualRegisterRequest.SelectedExamScheduleId.Count; i++)
                {
                    individualRegisterRequest.registerInformation.ThoiDiemDangKy = DateTime.Now;
                    individualRegisterRequest.registerInformation.TrangThai = "Chưa thanh toán";
                    individualRegisterRequest.registerInformation.MaLichThi = individualRegisterRequest.SelectedExamScheduleId[i];
                    individualRegisterRequest.registerInformation.LoaiKhachHang = "Cá nhân";
                    int registerInformationId = registerInformationDao.AddRegisterInformation(individualRegisterRequest.registerInformation);
                    if (registerInformationId == -1)
                    {
                        return RegisterResult.UnknownError;
                    }
                    // Add candidate information for the individual registration
                    int result = registerInformationDao.AddCandidateInformationsOfARegisterInformation(registerInformationId, individualRegisterRequest.candidateInformation);
                    if (result == -1)
                    {
                        return RegisterResult.UnknownError;
                    }
                    int testId = examScheduleDao.GetTestIdByExamScheduleId(individualRegisterRequest.SelectedExamScheduleId[i]);
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
                        return RegisterResult.UnknownError;
                    }
                    bool updated = examScheduleDao.UpdateQuantityOfExamSchedule(individualRegisterRequest.SelectedExamScheduleId[i], 1);
                    if (!updated)
                    {
                        return RegisterResult.UnknownError;
                    }
                }
                return RegisterResult.Success;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error during individual registration: {ex.Message}");
                return RegisterResult.UnknownError;
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

       public List<Entity.RegisterInformation> LoadRegisterInformation(int pageSize, int currentPageNumber, RegisterInformationFilterObject registerInformationFilterObject)
       {
           throw new NotImplementedException();
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
