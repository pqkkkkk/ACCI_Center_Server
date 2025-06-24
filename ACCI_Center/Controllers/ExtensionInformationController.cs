using ACCI_Center.Dto;
using ACCI_Center.Dto.Request;
using ACCI_Center.Dto.Response;
using ACCI_Center.Entity;
using ACCI_Center.FilterField;
using ACCI_Center.Service;
using ACCI_Center.Service.TTGiaHan;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ACCI_Center.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExtensionInformationController : ControllerBase
    {
        private IExtensionInformationService _extensionInformationService;
        public ExtensionInformationController(IExtensionInformationService extensionInformationService)
        {
            _extensionInformationService = extensionInformationService;
        }
        [HttpPost("free")]
        public ActionResult<string> FreeExtension()
        {
            // Logic for free extension
            return Ok("Free extension applied successfully.");
        }
        [HttpPost("paid")]
        public ActionResult<string> PaidExtension([FromBody] PaidRenewalRequest request)
        {
            if(_extensionInformationService.ExtendExamTimePaid(new ExtensionInformation
            {
                ThoiDiemGiaHan = DateTime.Now,
                LoaiGiaHan = "Gia hạn có phí",
                LyDo = request.LyDo,
                TrangThai = "Chưa thanh toán",
                PhiGiaHan = request.PhiGiaHan,
                MaTTDangKy = request.MaTTDangKy
            }, request.MaLichThiMoi)  > 0) 
                return Ok("Paid extension applied successfully.");
            return BadRequest("Failed to apply paid extension. Please check the request data and try again.");
        }
        [HttpGet]
        public ActionResult<PagedResult<ExtensionInformationDto>> GetPagedExtensions([FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? maTTGiaHan = null,
            [FromQuery] string? loaiGiaHan = null,
            [FromQuery] string? trangThai = null)
        {
            var filter = new ExtensionInformationFilterObject
            {
                MaTTGiaHan = maTTGiaHan,
                LoaiGiaHan = loaiGiaHan,
                TrangThai = trangThai
            };
            var allExtensions = _extensionInformationService.LoadExtendInformation(pageSize, page, filter);
            var itemCount = allExtensions.Count;

            var result = new PagedResult<ExtensionInformationDto>(
                allExtensions.Select(x => new ExtensionInformationDto
                {
                    MaTTGiaHan = x.MaTTGiaHan,
                    ThoiDiemGiaHan = x.ThoiDiemGiaHan,
                    LoaiGiaHan = x.LoaiGiaHan,
                    LyDo = x.LyDo,
                    TrangThai = x.TrangThai,
                    PhiGiaHan = x.PhiGiaHan
                }).ToList(),
                itemCount,
                page,
                pageSize
            );
            return Ok(result);
        }

    }
}
