using ACCI_Center.Dto;
using ACCI_Center.FilterField;
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
        [HttpGet("Test")]
        public ActionResult<PagedResult<Entity.Test>> GetTests(
            [FromQuery] int pageSize,
            [FromQuery] int currentPageNumber,
            [FromQuery] string? LoaiBaiThi,
            [FromQuery] string? TenBaiThi
            )
        {
            TestFilterObject testFilterObject = new TestFilterObject
            {
                LoaiBaiThi = LoaiBaiThi,
                TenBaiThi = TenBaiThi
            };
            var result = examScheduleService.LoadTests(pageSize, currentPageNumber, testFilterObject);

            if (result == null || result.items == null || result.items.Count() == 0)
            {
                return NotFound("No tests found.");
            }

            return Ok(result);
        }
    }
}
