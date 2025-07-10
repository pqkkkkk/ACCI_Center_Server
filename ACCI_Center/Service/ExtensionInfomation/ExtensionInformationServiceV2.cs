using System.Transactions;
using ACCI_Center.BusinessResult;
using ACCI_Center.Dao.ExamSchedule;
using ACCI_Center.Dao.ExtensionInformation;
using ACCI_Center.Dao.RegisterInformation;
using ACCI_Center.Dto.Request;
using ACCI_Center.Dto.Response;

namespace ACCI_Center.Service.ExtensionInfomation
{
    public class ExtensionInformationServiceV2 : IExtensionInformationServiceV2
    {
        private const int MAX_EXTENSION_TIME = 2;
        private const int MINIMUM_EXTENSION_LEAD_HOURS = 24;
        private readonly IExtensionInformationDao extensionInformationDao;
        private readonly IExamScheduleDao examScheduleDao;
        private readonly IRegisterInformationDao registerInformationDao;

        public ExtensionInformationServiceV2(IExtensionInformationDao extensionInformationDao, IExamScheduleDao examScheduleDao, IRegisterInformationDao registerInformationDao)
        {
            this.extensionInformationDao = extensionInformationDao;
            this.examScheduleDao = examScheduleDao;
            this.registerInformationDao = registerInformationDao;
        }
        public ExtensionResponse CreateExtensionInformation(ExtensionRequest request)
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
                    request.extensionInformation.ThoiDiemGiaHan = DateTime.Now;
                    request.extensionInformation.MaLichThiMoi = request.newExamScheduleId;
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

        public ExtensionResult ValidateExtensionRequest(int maTTDangKy, int? newExamScheduleId)
        {
            if (newExamScheduleId != null)
            {
                Entity.ExamSchedule? newExamSchedule = examScheduleDao.GetExamScheduleById(newExamScheduleId ?? 0);

                if (newExamSchedule == null)
                    return ExtensionResult.ExamScheduleNotAvailable;
            }

            Entity.RegisterInformation? registerInformation = registerInformationDao.LoadRegisterInformationById(maTTDangKy);
            if (registerInformation == null)
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
    }
}
