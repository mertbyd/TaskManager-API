using TaskManager.API.Models;

namespace TaskManager.API.Repositories
{
    public interface IReportRepository
    {
        Task<List<TaskItem>> GetTasksByDateRange(string userId, DateTime startDate, DateTime endDate);// Belirli tarih aralığındaki görevleri getirir
    }
}

