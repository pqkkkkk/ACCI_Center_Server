using ACCI_Center.BusinessResult;
using ACCI_Center.Dao.ExamSchedule;
using ACCI_Center.Dto.Request;

namespace ACCI_Center.Service.RegisterInformation
{
    public class RegisterInformationValidation : IRegisterInformationValidation
    {
        private readonly IExamScheduleDao examScheduleDao;
        public RegisterInformationValidation(IExamScheduleDao examScheduleDao)
        {
            this.examScheduleDao = examScheduleDao;
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
            if (!IsValidCandidateQuantity(organizationRegisterRequest.candidatesInformation.Count, organizationRegisterRequest.testId))
            {
                return RegisterResult.CandidateQuantityTooLow;
            }
            if (!IsValidDesiredExamTime(organizationRegisterRequest.desiredExamTime, organizationRegisterRequest.testId))
            {
                return RegisterResult.NoAvailableTimeSlot;
            }

            return RegisterResult.Success;
        }

        public RegisterResult ValidateRegisterRequest(OrganizationRegisterRequestV2 organizationRegisterRequest)
        {
            if (!IsValidOrganizationInformation(organizationRegisterRequest.registerInformation))
            {
                return RegisterResult.InvalidOrganizationInformation;
            }
            if (!IsValidTestInformation(organizationRegisterRequest.testInformation.testId, organizationRegisterRequest.testInformation.testName))
            {
                return RegisterResult.InvalidTestInformation;
            }
            if (!IsValidCandidateQuantity(organizationRegisterRequest.candidatesInformation.Count, organizationRegisterRequest.testInformation.testId))
            {
                return RegisterResult.CandidateQuantityTooLow;
            }
            if (!IsValidDesiredExamTime(organizationRegisterRequest.testInformation.desiredExamTime, organizationRegisterRequest.testInformation.testId))
            {
                return RegisterResult.NoAvailableTimeSlot;
            }

            return RegisterResult.Success;
        }
    }
}
