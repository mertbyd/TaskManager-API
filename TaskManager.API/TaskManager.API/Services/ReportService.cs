using TaskManager.API.DTOs;
using TaskManager.API.Models;
using TaskManager.API.Repositories;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace TaskManager.API.Services
{
    public class ReportService : IReportService
    {
        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }
        private readonly IReportRepository _reportRepository;

        public async Task<List<Models.TaskItem>?> GetDailyReportTasks(string userId, DateTime? date) // Günlük rapor oluşturma
        {
            try
            {
                // UserId'nin boş olup olmadığını kontrol et
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("Service: UserId boş");
                    return null;
                }
                var reportDate = date ?? DateTime.Today;//eğer date null ise bugünğn tarihini atar değilse direk atama yapar
                /*
                DateTime reportDate;
                if (date.HasValue)
                {
                    reportDate = date.Value;
                }
                else
                {
                    reportDate = DateTime.Today;
                }
                */
                var startDate = reportDate.Date; //bize verilen günün başı
                var endDate = startDate.AddDays(1).AddSeconds(-1);//verilen gün sonu
                var result = await _reportRepository.GetTasksByDateRange(userId, startDate, endDate);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Service GetDailyReportTasks hatası: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Models.TaskItem>?> GetWeeklyReportTasks(string userId, DateTime? date) // Haftalık rapor oluşturma
        {
            try
            {
                // UserId'nin boş olup olmadığını kontrol et
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("Service: UserId boş");
                    return null;
                }
                var reportDate = date ?? DateTime.Today;
                var dayOfWeek = (int)reportDate.DayOfWeek;//haftanın günlerini int tipinde tutan bir enum gönderir mesala bize verilen gün pazartesi=1  salı =2 verisini verir
                var startDate = reportDate.AddDays(-dayOfWeek);// Günü geri alır. Yani rapor tarihini haftanın ilk günü olan Pazara çeker
                var endDate = startDate.AddDays(7).AddSeconds(-1);//haftayı belirttik
                var result = await _reportRepository.GetTasksByDateRange(userId, startDate, endDate);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Service GetMonthlyReportTasks hatası: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Models.TaskItem>?> GetMonthlyReportTasks(string userId, DateTime? date) // Aylık rapor oluşturma
        {
            try
            {
                // UserId'nin boş olup olmadığını kontrol et
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("Service: UserId boş");
                    return null;
                }
                // Tarih belirtilmemişse bugün
                var reportDate = date ?? DateTime.Today;
                // Ayın başlangıcı ve sonu
                var startDate = new DateTime(reportDate.Year, reportDate.Month, 1);//tarih oluştururuz
                var endDate = startDate.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
                Console.WriteLine($"Service: Aylık rapor oluşturuluyor - {startDate:MM/yyyy}");
                return await _reportRepository.GetTasksByDateRange(userId, startDate, endDate);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Service GetMonthlyReportTasks hatası: {ex.Message}");
                return null;
            }
        }
    }
}