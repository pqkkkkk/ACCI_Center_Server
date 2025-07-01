using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACCI_Center.BusinessResult;
using ACCI_Center.Dao.Invoice;
using ACCI_Center.Dao.ExamSchedule;
using ACCI_Center.Dao.RegisterInformation;
using ACCI_Center.Dao.ExtensionInformation;
using ACCI_Center.FilterField;
using ACCI_Center.Entity;
using ACCI_Center.Dto;
using ACCI_Center.Dto.Response;
using ACCI_Center.Dto.Request;
using System.Transactions;

namespace ACCI_Center.Service.TTGiaHan
{
    public class ExtensionInformationService : IExtensionInformationService
    {
        private const int MAX_EXTENSION_TIME = 2;
        private const int MINIMUM_EXTENSION_LEAD_HOURS = 24;
        private IExtensionInformationDao extensionInformationDao;
        private IRegisterInformationDao registerInformationDao;
        private IExamScheduleDao examScheduleDao;
        private IInvoiceDao invoiceDao;

        public ExtensionInformationService(IExtensionInformationDao extensionInformationDao,
                                        IRegisterInformationDao registerInformationDao,
                                        IExamScheduleDao examScheduleDao, IInvoiceDao invoiceDao)
        {
            this.extensionInformationDao = extensionInformationDao;
            this.registerInformationDao = registerInformationDao;
            this.examScheduleDao = examScheduleDao;
            this.invoiceDao = invoiceDao;
        }
        public ExtensionResult ValidateExtensionRequest(int maTTDangKy, int? newExamScheduleId)
        {
            if (newExamScheduleId != null)
            {
                Entity.ExamSchedule? newExamSchedule = examScheduleDao.GetExamScheduleById(newExamScheduleId ?? 0);

                if (newExamSchedule == null)
                    return ExtensionResult.ExamScheduleNotAvailable;
            }

            Entity.RegisterInformation? registerInformation = registerInformationDao.LoadRegisterInformationById(maTTDangKy);
            if(registerInformation == null)
                return ExtensionResult.RegisterInformationNotFound;

            Entity.ExamSchedule? oldExamSchedule = examScheduleDao.GetExamScheduleById(registerInformation?.MaLichThi ?? 0);
            if (oldExamSchedule == null)
                return ExtensionResult.OldExamScheduleNotFound;

            DateTime examTime = oldExamSchedule?.NgayThi ?? DateTime.MinValue;
            DateTime now = DateTime.Now;
            if ((examTime - now).TotalHours < MINIMUM_EXTENSION_LEAD_HOURS)
                return ExtensionResult.TooLate;


            int extensionTime = extensionInformationDao.GetExtensionTime(maTTDangKy);
            if (extensionTime >= MAX_EXTENSION_TIME)
                return ExtensionResult.ExceedExtendTimeLimit;

            return ExtensionResult.Ok;

        }
        public ValidateExtensionRequestResponse ValidateExtensionRequest(int maTTDangKy)
        {
            try
            {
                ExtensionResult extensionResult = ValidateExtensionRequest(maTTDangKy, null);
                if (extensionResult != ExtensionResult.Ok)
                {
                    return new ValidateExtensionRequestResponse()
                    {
                        isValid = false,
                        statusCode = StatusCodes.Status200OK,
                        result = extensionResult
                    };
                }

                return new ValidateExtensionRequestResponse()
                {
                    isValid = true,
                    statusCode = StatusCodes.Status200OK,
                    result = ExtensionResult.Ok
                };
            }
            catch (Exception ex)
            {
                return new ValidateExtensionRequestResponse()
                {
                    isValid = false,
                    statusCode = StatusCodes.Status500InternalServerError,
                    result = ExtensionResult.UnknownError
                };
            }
        }
        public ExtensionResponse ExtendExamTimeFree(ExtensionRequest request)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    // Validate request
                    ExtensionResult extensionResult = ValidateExtensionRequest(request.extensionInformation.MaTTDangKy, request.newExamScheduleId);
                    if (extensionResult != ExtensionResult.Ok)
                    {
                        return new ExtensionResponse()
                        {
                            extensionResult = extensionResult,
                            statusCode = StatusCodes.Status200OK
                        };
                    }

                    // Update the register information with the new exam schedule
                    var updateRegisterInformationResult = registerInformationDao.UpdateExamSchedule(request.extensionInformation.MaTTDangKy, request.newExamScheduleId);
                    if (updateRegisterInformationResult <= 0)
                    {
                        throw new Exception("Failed to update register information with new exam schedule.");
                    }

                    // Add extension information
                    request.extensionInformation.LoaiGiaHan = "Gia hạn miễn phí";
                    request.extensionInformation.PhiGiaHan = 0;
                    request.extensionInformation.TrangThai = "Đã thanh toán";
                    request.extensionInformation.ThoiDiemGiaHan = DateTime.Now;
                    int extensionInformationId = extensionInformationDao.AddExtensionInformation(request.extensionInformation);
                    if (extensionInformationId <= 0)
                    {
                        throw new Exception("Failed to add extension information.");
                    }

                    var newExamSchedule = examScheduleDao.GetExamScheduleById(request.newExamScheduleId);

                    // Commit the transaction
                    transaction.Complete();
                    return new ExtensionResponse()
                    {
                        extensionInformation = request.extensionInformation,
                        newExamSchedule = newExamSchedule,
                        statusCode = StatusCodes.Status200OK,
                        extensionResult = ExtensionResult.Ok,
                    };
                }
            }
            catch (Exception ex)
            {
                return new ExtensionResponse()
                {
                    statusCode = StatusCodes.Status500InternalServerError,
                    extensionResult = ExtensionResult.UnknownError
                };
            }
        }

        public ExtensionResponse ExtendExamTimePaid(ExtensionRequest request)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    // Validate request
                    ExtensionResult extensionResult = ValidateExtensionRequest(request.extensionInformation.MaTTDangKy, request.newExamScheduleId);
                    if (extensionResult != ExtensionResult.Ok)
                    {
                        return new ExtensionResponse()
                        {
                            extensionResult = extensionResult,
                            statusCode = StatusCodes.Status200OK
                        };
                    }

                    // Update the register information with the new exam schedule
                    var updateRegisterInformationResult = registerInformationDao.UpdateExamSchedule(request.extensionInformation.MaTTDangKy, request.newExamScheduleId);
                    if(updateRegisterInformationResult <= 0)
                    {
                        throw new Exception("Failed to update register information with new exam schedule.");
                    }

                    // Add extension information
                    int extensionInformationId = extensionInformationDao.AddExtensionInformation(request.extensionInformation);
                    if (extensionInformationId <= 0)
                    {
                        throw new Exception("Failed to add extension information.");
                    }


                    // Add invoice for the extension
                    //Invoice invoice = new Invoice
                    //{
                    //    ThoiDiemTao = DateTime.Now,
                    //    TongTien = request.extensionInformation.PhiGiaHan,
                    //    TrangThai = "Chưa thanh toán",
                    //    LoaiHoaDon = "Gia hạn thi",
                    //    MaTTDangKy = -1,
                    //    MaTTGiaHan = extensionInformationId
                    //};
                    //int invoiceId = invoiceDao.AddInvoice(invoice);
                    //if (invoiceId <= 0)
                    //{
                    //    throw new Exception("Failed to create invoice for extension.");
                    //}

                    var newExamSchedule = examScheduleDao.GetExamScheduleById(request.newExamScheduleId);

                    // Commit the transaction
                    transaction.Complete();
                    return new ExtensionResponse()
                    {
                        extensionInformation = request.extensionInformation,
                        newExamSchedule = newExamSchedule,
                        statusCode = StatusCodes.Status200OK,
                        extensionResult = ExtensionResult.Ok,
                    };
                }
            }
            catch (Exception ex)
            {
                return new ExtensionResponse()
                {
                    statusCode = StatusCodes.Status500InternalServerError,
                    extensionResult = ExtensionResult.UnknownError
                };
            }
        }

        public PagedResult<Entity.ExtensionInformation> LoadExtendInformation(int pageSize, int currentPageNumber, ExtensionInformationFilterObject extensionInformationFilterObject)
        {
            try
            {
                return extensionInformationDao.LoadExtendInformation(pageSize, currentPageNumber, extensionInformationFilterObject);
            }
            catch (Exception ex)
            {
                return new PagedResult<ExtensionInformation>(null, 0, 0, 0);
            }
        }

        public Entity.ExtensionInformation LoadExtendInformationById(int maTTGiaHan)
        {
            throw new NotImplementedException();
        }


    }
}
