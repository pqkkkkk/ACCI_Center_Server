using ACCI_Center.Dto;
using ACCI_Center.Service.ExamSchedule;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ACCI_Center.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamScheduleController : ControllerBase
    {
        private readonly IExamScheduleService examScheduleService;

        public ExamScheduleController(IExamScheduleService examScheduleService)
        {
            this.examScheduleService = examScheduleService;
        }
        [HttpGet]
        public ActionResult<PagedResult<Entity.Test>> GetTests(
            [FromQuery] int pageSize,
            [FromQuery] int currentPageNumber
            )
        {
            var result = examScheduleService.LoadTests(pageSize, currentPageNumber);

            if (result == null || result.items == null || result.items.Count() == 0)
            {
                return NotFound("No tests found.");
            }

            return Ok(result);
        }
    }
}
