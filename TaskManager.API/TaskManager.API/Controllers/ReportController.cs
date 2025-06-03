using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.API.Services;
namespace TaskManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // JWT güvenliği
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        // GET /api/Report/daily?date=2023-06-15
        [HttpGet("daily")] // Günlük rapor endpoint'i
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), 200)]
        public async Task<ActionResult<DTOs.DTOs.ApiResponse>> GetDailyReport([FromQuery] DateTime? date)
        {
            try
            {
                // JWT'den kullanıcı ID'sini al
                var userId = User.FindFirst("user_id")?.Value;

                // UserId'nin JWT'de olup olmadığını kontrol et
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new DTOs.DTOs.ApiResponse
                    {
                        data = null,
                        error_message = "User ID bulunamadı",
                        success = false
                    });
                }

                // Service'ten rapor verilerini al
                var tasks = await _reportService.GetDailyReportTasks(userId, date);

                // Rapor oluşturma işleminin başarılı olup olmadığını kontrol et
                if (tasks == null)
                {
                    return StatusCode(500, new DTOs.DTOs.ApiResponse
                    {
                        success = false,
                        error_message = "Rapor oluşturulurken hata oluştu",
                        data = null
                    });
                }

                // Rapor istatistiklerini hesapla
                var reportInfo = new
                {
                    period = "daily",
                    date = date ?? DateTime.Today,
                    total_tasks = tasks.Count,
                    completed_tasks = tasks.Count(t => t.status == "Completed"),
                    pending_tasks = tasks.Count(t => t.status == "Pending"),
                    in_progress_tasks = tasks.Count(t => t.status == "InProgress"),
                    tasks = tasks
                };
                return Ok(new DTOs.DTOs.ApiResponse
                {
                    success = true,
                    error_message = string.Empty,
                    data = reportInfo
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new DTOs.DTOs.ApiResponse
                {
                    success = false,
                    error_message = "Sunucu hatası: " + ex.Message,
                    data = null
                });
            }
        }

        // GET /api/Report/weekly?date=2023-06-15
        [HttpGet("weekly")] // Haftalık rapor endpoint'i
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), 200)]
        public async Task<ActionResult<DTOs.DTOs.ApiResponse>> GetWeeklyReport([FromQuery] DateTime? date)
        {
            try
            {
                var userId = User.FindFirst("user_id")?.Value;

                // UserId'nin JWT'de olup olmadığını kontrol et
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new DTOs.DTOs.ApiResponse
                    {
                        data = null,
                        error_message = "User ID bulunamadı",
                        success = false
                    });
                }

                // Service'ten rapor verilerini al
                var tasks = await _reportService.GetWeeklyReportTasks(userId, date);

                // Rapor oluşturma işleminin başarılı olup olmadığını kontrol et
                if (tasks == null)
                {
                    return StatusCode(500, new DTOs.DTOs.ApiResponse
                    {
                        success = false,
                        error_message = "Rapor oluşturulurken hata oluştu",
                        data = null
                    });
                }

                // Hafta başlangıç/bitiş tarihleri hesapla
                var reportDate = date ?? DateTime.Today;
                var dayOfWeek = (int)reportDate.DayOfWeek;
                var startDate = reportDate.AddDays(-dayOfWeek);
                var endDate = startDate.AddDays(6);

                // Rapor istatistiklerini hesapla
                var reportInfo = new
                {
                    period = "weekly",
                    start_date = startDate,
                    end_date = endDate,
                    total_tasks = tasks.Count,
                    completed_tasks = tasks.Count(t => t.status == "Completed"),
                    pending_tasks = tasks.Count(t => t.status == "Pending"),
                    in_progress_tasks = tasks.Count(t => t.status == "InProgress"),
                    tasks = tasks
                };
                return Ok(new DTOs.DTOs.ApiResponse
                {
                    success = true,
                    error_message = string.Empty,
                    data = reportInfo
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new DTOs.DTOs.ApiResponse
                {
                    success = false,
                    error_message = "Sunucu hatası: " + ex.Message,
                    data = null
                });
            }
        }

        // GET /api/Report/monthly?date=2023-06-15
        [HttpGet("monthly")] // Aylık rapor endpoint'i
        [ProducesResponseType(typeof(DTOs.DTOs.ApiResponse), 200)]
        public async Task<ActionResult<DTOs.DTOs.ApiResponse>> GetMonthlyReport([FromQuery] DateTime? date)
        {
            try
            {
                var userId = User.FindFirst("user_id")?.Value;

                // UserId'nin JWT'de olup olmadığını kontrol et
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new DTOs.DTOs.ApiResponse
                    {
                        data = null,
                        error_message = "User ID bulunamadı",
                        success = false
                    });
                }

                // Service'ten rapor verilerini al
                var tasks = await _reportService.GetMonthlyReportTasks(userId, date);

                // Rapor oluşturma işleminin başarılı olup olmadığını kontrol et
                if (tasks == null)
                {
                    return StatusCode(500, new DTOs.DTOs.ApiResponse
                    {
                        success = false,
                        error_message = "Rapor oluşturulurken hata oluştu",
                        data = null
                    });
                }

                // Ay başlangıç/bitiş tarihleri hesapla
                var reportDate = date ?? DateTime.Today;
                var startDate = new DateTime(reportDate.Year, reportDate.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                // Rapor istatistiklerini hesapla
                var reportInfo = new
                {
                    period = "monthly",
                    start_date = startDate,
                    end_date = endDate,
                    total_tasks = tasks.Count,
                    completed_tasks = tasks.Count(t => t.status == "Completed"),
                    pending_tasks = tasks.Count(t => t.status == "Pending"),
                    in_progress_tasks = tasks.Count(t => t.status == "InProgress"),
                    tasks = tasks
                };
                return Ok(new DTOs.DTOs.ApiResponse
                {
                    success = true,
                    error_message = string.Empty,
                    data = reportInfo
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new DTOs.DTOs.ApiResponse
                {
                    success = false,
                    error_message = "Sunucu hatası: " + ex.Message,
                    data = null
                });
            }
        }
    }
}