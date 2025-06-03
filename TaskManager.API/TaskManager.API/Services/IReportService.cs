using TaskManager.API.DTOs;
using TaskManager.API.Models;

namespace TaskManager.API.Services
{
    public interface IReportService
    {
       
        Task<List<Models.TaskItem>?> GetDailyReportTasks(string userId, DateTime? date); // Günlük rapor 

       
        Task<List<Models.TaskItem>?> GetWeeklyReportTasks(string userId, DateTime? date); // Haftalık rapor 

        
        Task<List<Models.TaskItem>?> GetMonthlyReportTasks(string userId, DateTime? date);// Aylık rapor
    }
}