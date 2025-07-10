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
        private readonly IRegisterInformationValidation registerInformationValidation;

        public RegisterInformationServiceV2(IRegisterInformationDao registerInformationDao, IExamScheduleDao examScheduleDao,
            IRegisterInformationValidation registerInformationValidation)
        {
            this.registerInformationDao = registerInformationDao;
            this.examScheduleDao = examScheduleDao;
            this.registerInformationValidation = registerInformationValidation;
        }
        public IndividualRegisterResponse CreateRegisterInformationForIndividual(IndividualRegisterRequest request)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    request.registerInformation.ThoiDiemDangKy = DateTime.Now;
                    request.registerInformation.TrangThai = "Chưa thanh toán";
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

        public OrganizationRegisterResponse CreateRegisterInformationForOrganization(OrganizationRegisterRequest request)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    // Validate the registration request
                    RegisterResult testRegisterResult = registerInformationValidation.ValidateRegisterRequest(request);
                    if (testRegisterResult != RegisterResult.Success)
                    {
                        return new OrganizationRegisterResponse
                        {
                            registerInformation = request.registerInformation,
                            candidatesInformation = request.candidatesInformation,
                            test = null,
                            examSchedule = null,
                            invoice = null,
                            registerResult = testRegisterResult,
                            statusCode = 400
                        };
                    }

                    
                    // Add register information record
                    request.registerInformation.ThoiDiemDangKy = DateTime.Now;
                    request.registerInformation.LoaiKhachHang = "Đơn vị";
                    request.registerInformation.TrangThai = "Chưa thanh toán";
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


                    var test = examScheduleDao.GetTestById(request.testId);
                    // Commit the transaction
                    transaction.Complete();
                    return new OrganizationRegisterResponse
                    {
                        registerInformation = request.registerInformation,
                        candidatesInformation = request.candidatesInformation,
                        test = test,
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
    }
}
