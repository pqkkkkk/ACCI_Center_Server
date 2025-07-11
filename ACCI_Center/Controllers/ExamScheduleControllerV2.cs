using ACCI_Center.Dto.Request;
using ACCI_Center.Dto.Response;
using ACCI_Center.Service.ExamSchedule;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ACCI_Center.Controllers
{
    [Route("api/v2/ExamSchedule")]
    [ApiController]
    public class ExamScheduleControllerV2 : ControllerBase
    {
        private readonly IExamScheduleServiceV2 examScheduleServiceV2;

        public ExamScheduleControllerV2(IExamScheduleServiceV2 examScheduleServiceV2)
        {
            this.examScheduleServiceV2 = examScheduleServiceV2;
        }

        [HttpPost]
        public ActionResult<CreateExamScheduleResponse> CreateExamSchedule([FromBody] CreateExamScheduleRequest request)
        {

            var response = examScheduleServiceV2.CreateExamSchedule(request);
            if (response.statusCode != StatusCodes.Status200OK)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create exam schedule.");
            }
            return Ok(response);
        }
        [HttpGet("employee/available")]
        public ActionResult<AvailableEmployeesResponse> GetAvailableEmployees([FromQuery] DateTime desiredExamTime, [FromQuery] int testId)
        {
            var response = examScheduleServiceV2.GetAvailableEmployees(desiredExamTime, testId);
            if (response.statusCode != StatusCodes.Status200OK)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve available employees.");
            }
            return Ok(response);
        }
        [HttpGet("room/available")]
        public ActionResult<AvailableRoomsResponse> GetAvailableRooms([FromQuery] DateTime desiredExamTime, [FromQuery] int testId)
        {
            var response = examScheduleServiceV2.GetAvailableRooms(desiredExamTime, testId);
            if (response.statusCode != StatusCodes.Status200OK)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve available rooms.");
            }
            return Ok(response);
        }
    }
}
