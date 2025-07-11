using System.Transactions;
using ACCI_Center.Dao.ExamSchedule;
using ACCI_Center.Dto.Request;
using ACCI_Center.Dto.Response;

namespace ACCI_Center.Service.ExamSchedule
{
    public class ExamScheduleServiceV2 : IExamScheduleServiceV2
    {
        private readonly IExamScheduleDao examScheduleDao;
        private readonly IExamScheduleDaoV2 examScheduleDaoV2;

        public ExamScheduleServiceV2(IExamScheduleDao examScheduleDao, IExamScheduleDaoV2 examScheduleDaoV2)
        {
            this.examScheduleDao = examScheduleDao;
            this.examScheduleDaoV2 = examScheduleDaoV2;
        }
        public CreateExamScheduleResponse CreateExamSchedule(CreateExamScheduleRequest request)
        {
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    int newExamScheduleId = examScheduleDaoV2.AddExamSchedule(request.examSchedule, request.supervisorIds);
                    if (newExamScheduleId <= 0)
                    {
                        return new CreateExamScheduleResponse
                        {
                            examSchedule = null,
                            statusCode = 400,
                            message = "Failed to create exam schedule"
                        };
                    }

                    request.examSchedule.MaLichThi = newExamScheduleId;

                    transaction.Complete();

                    return new CreateExamScheduleResponse
                    {
                        examSchedule = request.examSchedule,
                        statusCode = 200,
                        message = "Exam schedule created successfully"
                    };
                }
                catch (Exception ex)
                {
                    transaction.Dispose();
                    return new CreateExamScheduleResponse
                    {
                        examSchedule = null,
                        statusCode = 500,
                        message = "An error occurred while creating the exam schedule"
                    };
                }
            }
        }

        public AvailableEmployeesResponse GetAvailableEmployees(DateTime desiredExamTime, int testId)
        {
            try
            {
                List<Entity.Employee> availableEmployees = examScheduleDaoV2.GetAllFreeEmployeeIds(desiredExamTime, testId);
                if (availableEmployees == null || availableEmployees.Count == 0)
                {
                    return new AvailableEmployeesResponse
                    {
                        statusCode = 404,
                        message = "No available employees found for the specified exam time",
                    };
                }

                return new AvailableEmployeesResponse
                {
                    statusCode = 200,
                    message = "Available employees fetched successfully",
                    data = availableEmployees
                };
            }
            catch (Exception ex)
            {
                return new AvailableEmployeesResponse
                {
                    statusCode = 500,
                    message = "An error occurred while fetching available employees",
                };
            }
        }

        public AvailableRoomsResponse GetAvailableRooms(DateTime desiredExamTime, int testId)
        {
            try
            {
                List<Entity.Room> availableRooms = examScheduleDaoV2.GetAllEmptyRoomIds(desiredExamTime, testId);
                if (availableRooms == null || availableRooms.Count == 0)
                {
                    return new AvailableRoomsResponse
                    {
                        statusCode = 404,
                        message = "No available rooms found for the specified exam time",
                    };
                }

                return new AvailableRoomsResponse
                {
                    statusCode = 200,
                    message = "Available rooms fetched successfully",
                    data = availableRooms
                };
            }
            catch (Exception ex)
            {
                return new AvailableRoomsResponse
                {
                    statusCode = 500,
                    message = "An error occurred while fetching available rooms",
                };
            }
        }
    }
}
