using MongoDB.Bson;
using TaskManager.API.Models;

namespace TaskManager.API.Repositories
{
    public interface ITaskRepository
    {
      
        public Task<Models.TaskItem?> Insert(Models.TaskItem task);      // Yeni görev ekler
        public Task<List<Models.TaskItem>?> GetAll();  // Tüm görevleri getirme --Test için
        public Task<List<Models.TaskItem>?> GetByUserId(string userId);  // Kullanıcının tüm görevlerini getirir
        public Task<Models.TaskItem?> GetByIdAndUserId(string taskId, string userId);    // ID ve kullanıcı ile tek görev getirir
        public Task<bool> DeleteByUserId(string taskId, string userId);        // Task silme

        // Görevi tamamen günceller
        public Task<bool?> UpdateTask(string taskId, string userId, Models.TaskItem updatedTask);//Task günceleme -------Models.TaskItem updatedTask servis katmanından bunu istiyorum çünkü hem
                                                                                                 //repistoryde hemde serviste sadece 2 kere find kulanma yetiyor bu şekilde güncelnen veriyi gösterebiliyorum
        public Task<bool?> PatchTask(string taskId, string userId, Dictionary<string, object> updates);  // Görevi kısmi günceller
        public Task<List<TaskItem>> GetTasksByPeriod(string userId, string period);// Periyoda göre görevleri getirir
    }
}