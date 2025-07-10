using ACCI_Center.Dto;
using ACCI_Center.FilterField;
using ACCI_Center.Service.ExamSchedule;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ACCI_Center.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ExamScheduleController : ControllerBase
    {
        private readonly IExamScheduleService examScheduleService;

        public ExamScheduleController(IExamScheduleService examScheduleService)
        {
            this.examScheduleService = examScheduleService;
        }
        [HttpGet]
        public ActionResult<PagedResult<Entity.ExamSchedule>> GetExamSchedules(
            [FromQuery] int pageSize,
            [FromQuery] int currentPageNumber,
            [FromQuery] ExamScheduleFilterObject filterObject
            )
        {
            var result = examScheduleService.LoadExamSchedules(pageSize, currentPageNumber, filterObject);

            if(result.items == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching exam schedules.");
            }
            if (result.items.Count() == 0)
            {
                return NotFound("No exam schedules found.");
            }
            return Ok(result);
        }
        [HttpGet("Test")]
        public ActionResult<PagedResult<Entity.Test>> GetTests(
            [FromQuery] int pageSize,
            [FromQuery] int currentPageNumber,
            [FromQuery] TestFilterObject testFilterObject
            )
        {
            var result = examScheduleService.LoadTests(pageSize, currentPageNumber, testFilterObject);

            if (result == null || result.items == null || result.items.Count() == 0)
            {
                return NotFound("No tests found.");
            }

            return Ok(result);
        }
    }
}
