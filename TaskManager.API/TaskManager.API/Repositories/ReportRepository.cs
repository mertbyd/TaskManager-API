using MongoDB.Driver;
using System.Threading.Tasks;
using TaskManager.API.Configurations;
using TaskManager.API.Models;

namespace TaskManager.API.Repositories
{
    public class ReportRepository: IReportRepository
    {
        public ReportRepository()
        {
            _collection = DatabaseSetup.GetTasksCollection(); // TaskItem collection'ı al
        }
        private readonly IMongoCollection<TaskItem> _collection;
        public async Task<List<TaskItem>> GetTasksByDateRange(string userId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var tasks = await _collection.Find(x => x.user_id == userId && x.created_at >= startDate && x.created_at <= endDate).ToListAsync();//Taskin oluşturulma tarihi bizden istenile aralıkta mı diye kontrol etme
                Console.WriteLine($"Repository: {tasks.Count} görev bulundu - Tarih: {startDate:dd/MM/yyyy} - {endDate:dd/MM/yyyy}");
                return tasks;
            }
            catch( Exception ex ) 
            {
                Console.WriteLine($"ReportRepository GetTasksByDateRange hatası: {ex.Message}");
                return new List<TaskItem>(); // Hata durumunda boş liste döndür
            }
        }

    }
}
