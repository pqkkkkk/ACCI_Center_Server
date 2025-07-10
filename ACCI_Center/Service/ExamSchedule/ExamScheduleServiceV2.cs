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
    }
}
